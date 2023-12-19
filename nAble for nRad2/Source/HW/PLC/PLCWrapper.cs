using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Text;
using nTact.DataComm;
using System.Drawing;
using nAble;
using System.Collections.Generic;

namespace nTact.PLC
{
    public enum StackPosID { Clear, LDULD, Docking, MOD1, MOD2, MOD3, MOD1Hi, MOD2Hi, MOD3Hi, MOD1Low, MOD2Low, MOD3Low }
    public class PLCWrapper
    {
        private QCPU2 PLC;
        private List<Alarm> StackAlarms = new List<Alarm>(100);
        private readonly LogEntry _log = null;

        public const float ValveAngleMulti = 111.111111111111F;
        public const float PressureSetpointMulti = 10.0F;

        #region Standard Commands

        public PLCWrapper(LogEntry log)
        {
            _log = log;
            PLC = new QCPU2(_log);

            LoadAlarms();
        }

        public string PLCAddress { get; set; } = "";
        public bool IsConnected { get; private set; } = false;

        public int Connect()
        {
            int nRetVal = -1;

            nRetVal = Connect(PLCAddress);

            return nRetVal;
        }
        public int Connect(string Address)
        {
            int nRetVal = -1;

            if (Address == "")
            {
                _log.log(LogType.TRACE, Category.INFO, "PLCWrapper: Connect called with empty address", "ERROR");
                return -1;
            }
            else
            {
                PLCAddress = Address;
            }

            PLC.ActHostAddress = PLCAddress;

            try
            {
                nTactUtils.Ping(PLCAddress);
            }
            catch (Exception ex)
            {
                string msg = $"PLCWrapper: Connect failed. Could not ping PLC @ address: {Address}, error: {ex.Message}";
                _log.log(LogType.TRACE, Category.INFO, msg, "ERROR");
                return -1;
            }

            nRetVal = PLC.Open();

            if (nRetVal == 0)
            {
                IsConnected = true;
            }

            return nRetVal;
        }

        public int Disconnect()
        {
            int nRetVal = 0;
            PLC.Close();
            return nRetVal;
        }
        #endregion Standard Commands

        #region Status Properties
        public bool StackInMaintMode { get { return GlobalData.Bbit[0x56]; } set { PLC.SetDevice("B56", value ? 1 : 0); } }
        public bool EnableLDULDAbort { get { return GlobalData.Mbit[414]; } set { PLC.SetDevice("M414", value ? 1 : 0); } }

        public bool DC24VOK => GlobalData.Xbit[0x20];
        public bool ESROK => GlobalData.Xbit[0x21];
        public bool SubstrateSensor => GlobalData.Xbit[0x220];
        public bool CDAOK => GlobalData.Xbit[0x23];
        public bool SystemVacOK => GlobalData.Xbit[0x24];
        public bool EndEffectorPresent => GlobalData.Xbit[0x25];
        public bool VacPumpOK => GlobalData.Xbit[0x26];
        public bool VacPumpAtSpeed => GlobalData.Xbit[0x27];
        public bool LiftPinIsDown => GlobalData.Xbit[0x28];
        public bool EndEffectorVacuum => GlobalData.Xbit[0x29];
        public bool RobotDoorClosed => GlobalData.Xbit[0x2A];
        public bool LeftFrontDoorClosed => GlobalData.Xbit[0x2B];
        public bool RightFrontDoorClosed => GlobalData.Xbit[0x2C];
        public bool RightSideDoorClosed => GlobalData.Xbit[0x2D];
        public bool LeftRearDoorClosed => GlobalData.Xbit[0x2E];
        public bool RightRearDoorClosed => GlobalData.Xbit[0x2F];

        public bool LDULDFailure => GlobalData.Mbit[279];
        public bool LDULDFailContinue { get { return GlobalData.Bbit[0x46]; } set { PLC.SetDevice("B46", value ? 1 : 0); } }
        public bool LDULDFailAbort { get { return GlobalData.Bbit[0x47]; } set { PLC.SetDevice("B47", value ? 1 : 0); } }

        public bool RobotDoorBypassed { get { return GlobalData.Mbit[284]; } set { PLC.SetDevice("M284", value ? 1 : 0); } }
        public bool CoaterDoorsBypassed { get { return GlobalData.Mbit[285]; } set { PLC.SetDevice("M285", value ? 1 : 0); } }


        #region Robot / Loader status

        public bool LockQuickChange { get { return GlobalData.Mbit[290]; } set { PLC.SetDevice("M290", value ? 1 : 0); } }
        public bool UnlockQuickChange { get { return GlobalData.Mbit[291]; } set { PLC.SetDevice("M291", value ? 1 : 0); } }

        public bool SubStrateOnTransConv { get { return GlobalData.Mbit[298]; } set { PLC.SetDevice("M298", value ? 1 : 0); } }
        public bool TransConvAtStack { get { return GlobalData.Mbit[299]; } set { PLC.SetDevice("M299", value ? 1 : 0); } }


        public bool EndEffectorVac => GlobalData.Mbit[310];

        public bool HomeLoader { get { return GlobalData.Mbit[321]; } set { PLC.SetDevice("M321", value ? 1 : 0); } }

        public bool ZAtMod1PosHigh => GlobalData.Mbit[350];
        public bool ZAtMod1PosLow => GlobalData.Mbit[351];
        public bool ZAtMod2PosHigh => GlobalData.Mbit[352];
        public bool ZAtMod2PosLow => GlobalData.Mbit[353];
        public bool ZAtMod3PosHigh => GlobalData.Mbit[354];
        public bool ZAtMod3PosLow => GlobalData.Mbit[355];
        public bool ZAtClearPos => GlobalData.Mbit[356];
        public bool ZAtDockingPos => GlobalData.Mbit[357];
        public bool ZAtLDULDPos => GlobalData.Mbit[358];

        public bool YAtMod1ChuckPos => GlobalData.Mbit[370];
        public bool YAtMod2ChuckPos => GlobalData.Mbit[371];
        public bool YAtMod3ChuckPos => GlobalData.Mbit[372];
        public bool YAtDockPos => GlobalData.Mbit[373];
        public bool YAtLDULDPos => GlobalData.Mbit[374];
        public bool YAtClearPos => GlobalData.Mbit[375];


        public bool LoaderMoving => !GlobalData.Mbit[436];
        public bool ManMovesEnabled => GlobalData.Mbit[320];
        public int LoaderZVel => GlobalData.Dword[406];
        public bool LoaderZCmdActive => GlobalData.Mbit[323];
        public bool LoaderZMoving => GlobalData.Xbit[0x15C];
        public int LoaderYVel => GlobalData.Dword[407];
        public bool LoaderYCmdActive => GlobalData.Mbit[324];
        public bool LoaderYMoving => GlobalData.Xbit[0x17C];

        public bool LoaderHomed => GlobalData.Mbit[334] && GlobalData.Mbit[344];

        public bool IsLoadArmInSafePos
        {
            get
            {
                bool bRetVal = true;
                if (!GlobalData.Mbit[350] && !GlobalData.Mbit[351] && !GlobalData.Mbit[354] && !GlobalData.Mbit[355] && !GlobalData.Mbit[358] && !GlobalData.Mbit[357] && !GlobalData.Mbit[356])
                    bRetVal = false;
                else if (GlobalData.Mbit[357])
                    bRetVal = false;
                else if (GlobalData.Mbit[358])
                    bRetVal = false;
                return bRetVal;
            }
        }

        public bool EndEffectorSubstratePresent => GlobalData.Xbit[0x22];

        public int StackSubstrateCount
        {
            get
            {
                int nRetVal = 0;
                if (Mod1SubstratePresent)
                    nRetVal++;
                if (Mod2SubstratePresent)
                    nRetVal++;
                if (Mod3SubstratePresent)
                    nRetVal++;
                if (EndEffectorSubstratePresent)
                    nRetVal++;
                return nRetVal;
            }
        }

        public bool IsLoadArmDocked
        {
            get
            {
                bool bRetVal = false;
                if (GlobalData.Xbit[0x25])
                    bRetVal = true;
                return bRetVal;
            }
        }

        public bool IsEndEffectorOccupied
        {
            get
            {
                bool bRetVal = false;
                if (GlobalData.Xbit[0x22])
                    bRetVal = true;
                return bRetVal;
            }
        }



        #endregion Robot / Loader status

        #region Module 1

        public int Mod1State => GlobalData.Dword[323];
        public bool Mod1SubstratePresent => GlobalData.Bbit[0x210];
        public bool Mod1Aborting => GlobalData.Bbit[0x1E];
        public bool Mod1Loading => GlobalData.Mbit[420];
        public bool Mod1Unloading => GlobalData.Mbit[424];

        public bool Mod1VacSensor => GlobalData.Mbit[150];
        public bool Mod1DoorClosed => GlobalData.Mbit[151];
        public bool Mod1DoorIsOpen => GlobalData.Mbit[152];
        public bool Mod1SubstrateSensor => GlobalData.Mbit[153];

        public bool Mod1VacEnableLockDoorOpen => GlobalData.Mbit[270];
        public bool Mod1VacDoorLockoutCntdwn => GlobalData.Mbit[271];

        public bool Mod1EnablePower { get { return GlobalData.Mbit[1]; } set { PLC.SetDevice("M1", value ? 1 : 0); } }
        public bool Mod1CommEnabled { get { return GlobalData.Bbit[0x00]; } set { PLC.SetDevice("B000", value ? 1 : 0); } }
        public bool Mod1ResetModule { get { return GlobalData.Bbit[0x010]; } set { PLC.SetDevice("B010", value ? 1 : 0); } }
        public bool Mod1MotorError { get { return GlobalData.Mbit[19]; } set { PLC.SetDevice("M19", value ? 1 : 0); } }
        public bool Mod1ResetMotorError { get { return GlobalData.Bbit[0x01D]; } set { PLC.SetDevice("B01D", value ? 1 : 0); } }
        public bool Mod1MotorErrorResetting => GlobalData.Bbit[0x255];

        public int Mod1ChuckSetPoint { get { return GlobalData.Wword[0x200]; } set { PLC.SetDevice("W200", value); } }
        public double Mod1ChuckTemp => GlobalData.Wword[0x201] / 10.0;
        public string Mod1ProcessTimeRemaining => GlobalData.Wword[0x203].ToString("00") + ":" + GlobalData.Wword[0x204].ToString("00") + ":" + GlobalData.Wword[0x205].ToString("00");
        public string Mod1ProcessTotalTimeRemaining => GlobalData.Wword[0x206].ToString("00") + ":" + GlobalData.Wword[0x207].ToString("00") + ":" + GlobalData.Wword[0x208].ToString("00");
        public bool Mod1ProcessActive => GlobalData.Lbit[1];
        public bool Mod1LoadEnabled => GlobalData.Mbit[590];
        public bool Mod1UnloadEnabled => GlobalData.Mbit[594];

        public bool Mod1DoorOpenValve { get { return GlobalData.Bbit[0x057]; } set { PLC.SetDevice("B057", value ? 1 : 0); } }
        public bool Mod1DoorCloseValve { get { return GlobalData.Bbit[0x058]; } set { PLC.SetDevice("B058", value ? 1 : 0); } }
        public void Mod1ToggleCloseDoorValve() { Mod1DoorOpenValve = false; Mod1DoorCloseValve = !Mod1DoorCloseValve; }
        public void Mod1ToggleOpenDoorValve() { Mod1DoorCloseValve = false; Mod1DoorOpenValve = !Mod1DoorOpenValve; }

        public bool Mod1VacValve { get { return GlobalData.Bbit[0x059]; } set { PLC.SetDevice("B059", value ? 1 : 0); } }
        public bool Mod1AntiVacValve { get { return GlobalData.Bbit[0x05A]; } set { PLC.SetDevice("B05A", value ? 1 : 0); } }
        public void Mod1ToggleVac() { Mod1AntiVacValve = false; Mod1VacValve = !Mod1VacValve; }
        public void Mod1ToggleAntiVac() { Mod1VacValve = false; Mod1AntiVacValve = !Mod1AntiVacValve; }
        public bool Mod1TempContactor { get { return GlobalData.Bbit[0x05B]; } set { PLC.SetDevice("B05B", value ? 1 : 0); } }

        public bool Mod1ContactBakeBit { get { return GlobalData.Bbit[0x05C]; } set { PLC.SetDevice("B05C", value ? 1 : 0); } }
        public bool Mod1ProximityBakeBit { get { return GlobalData.Bbit[0x05D]; } set { PLC.SetDevice("B05D", value ? 1 : 0); } }
        public bool Mod1LoadUnloadHeightBit { get { return GlobalData.Bbit[0x05E]; } set { PLC.SetDevice("B05E", value ? 1 : 0); } }
        public bool Mod1HomeBit { get { return GlobalData.Bbit[0x05F]; } set { PLC.SetDevice("B05F", value ? 1 : 0); } }
        /// <summary> Height in mms (3 decimal places)</summary>
        public double Mod1LiftPinHomeOffset { get { return GlobalData.Wword[0x30] / 1000.000; } set { PLC.SetDevice("W30", (int)(value*1000)); } }
        /// <summary> Height in mms (3 decimal places)</summary>
        public double Mod1LiftPinLDULDHeight { get { return GlobalData.Wword[0x31]/1000.000; } set { PLC.SetDevice("W31", (int)(value * 1000)); } }
        public int Mod1LiftPinAccDcc { get { return GlobalData.Wword[0x32]; } set { PLC.SetDevice("W32", value); } }
        public int Mod1LiftPinVel { get { return GlobalData.Wword[0x33]; } set { PLC.SetDevice("W33", value); } }
        public int Mod1LiftPinPostion { get { return GlobalData.Wword[0x20B]; } set { PLC.SetDevice("W20B", value); } }

        public bool Mod1MoveToContBakeReqActv { get { return GlobalData.Mbit[401]; } set { PLC.SetDevice("M401", value ? 1 : 0); } }
        public bool Mod1MoveToProxBakeReqActv { get { return GlobalData.Mbit[402]; } set { PLC.SetDevice("M402", value ? 1 : 0); } }
        public bool Mod1MoveToLDULDReqActv { get { return GlobalData.Mbit[403]; } set { PLC.SetDevice("M403", value ? 1 : 0); } }
        public bool Mod1MoveToHomeReqActv { get { return GlobalData.Mbit[404]; } set { PLC.SetDevice("M404", value ? 1 : 0); } }

        /// <summary> In Seconds </summary>
        public double Mod1DisableDoorOpenTime { get { return GlobalData.Dword[309] / 10.0; } set { PLC.SetDevice("D309", (int)(value * 10)); } }
        public double Mod1EnableDoorTimeRemaining => Mod1DisableDoorOpenTime - (GlobalData.Tword[29] / 10.0);

        #endregion Module 1

        #region Module 2
        public int Mod2State => GlobalData.Dword[324];
        public bool Mod2SubstratePresent => GlobalData.Bbit[0x220];
        public bool Mod2Aborting => GlobalData.Bbit[0x23];
        public bool Mod2Loading => GlobalData.Mbit[421];
        public bool Mod2Unloading => GlobalData.Mbit[425];

        public bool Mod2VacSensor => GlobalData.Mbit[254];
        public bool Mod2DoorClosed => GlobalData.Mbit[159];
        public bool Mod2DoorIsOpen => GlobalData.Mbit[160];
        public bool Mod2SubstrateSensor => GlobalData.Mbit[161];

        public bool Mod2VacEnableLockDoorOpen => GlobalData.Mbit[272];
        public bool Mod2VacDoorLockoutCntdwn => GlobalData.Mbit[273];

        public bool Mod2EnablePower { get { return GlobalData.Mbit[2]; } set { PLC.SetDevice("M2", value ? 1 : 0); } }
        public bool Mod2CommEnabled { get { return GlobalData.Bbit[0x01]; } set { PLC.SetDevice("B001", value ? 1 : 0); } }
        public bool Mod2ResetModule { get { return GlobalData.Bbit[0x011]; } set { PLC.SetDevice("B011", value ? 1 : 0); } }
        public bool Mod2MotorError { get { return GlobalData.Mbit[29]; } set { PLC.SetDevice("M29", value ? 1 : 0); } }
        public bool Mod2ResetMotorError { get { return GlobalData.Bbit[0x022]; } set { PLC.SetDevice("B022", value ? 1 : 0); } }
        public bool Mod2MotorErrorResetting => GlobalData.Bbit[0x256];

        public int Mod2ChuckSetPoint { get { return GlobalData.Wword[0x210]; } set { PLC.SetDevice("W210", value); } }
        public double Mod2ChuckTemp => GlobalData.Wword[0x211] / 10.0;
        public string Mod2ProcessTimeRemaining => GlobalData.Wword[0x213].ToString("00") + ":" + GlobalData.Wword[0x214].ToString("00") + ":" + GlobalData.Wword[0x215].ToString("00");
        public string Mod2ProcessTotalTimeRemaining => GlobalData.Wword[0x216].ToString("00") + ":" + GlobalData.Wword[0x217].ToString("00") + ":" + GlobalData.Wword[0x218].ToString("00");
        public bool Mod2ProcessActive => GlobalData.Lbit[51];
        public bool Mod2LoadEnabled => GlobalData.Mbit[591];
        public bool Mod2UnloadEnabled => GlobalData.Mbit[595];

        public bool Mod2DoorOpenValve { get { return GlobalData.Bbit[0x060]; } set { PLC.SetDevice("B060", value ? 1 : 0); } }
        public bool Mod2DoorCloseValve { get { return GlobalData.Bbit[0x061]; } set { PLC.SetDevice("B061", value ? 1 : 0); } }
        public void Mod2ToggleCloseDoorValve() { Mod2DoorOpenValve = false; Mod2DoorCloseValve = !Mod2DoorCloseValve; }
        public void Mod2ToggleOpenDoorValve() { Mod2DoorCloseValve = false; Mod2DoorOpenValve = !Mod2DoorOpenValve; }

        public bool Mod2VacValve { get { return GlobalData.Bbit[0x062]; } set { PLC.SetDevice("B062", value ? 1 : 0); } }
        public bool Mod2CDASlowRelease { get { return GlobalData.Bbit[0x07C]; } set { PLC.SetDevice("B07C", value ? 1 : 0); } }
        public bool Mod2AntiVacValve { get { return GlobalData.Bbit[0x063]; } set { PLC.SetDevice("B063", value ? 1 : 0); } }
        public void Mod2ToggleVac() { Mod2AntiVacValve = false; Mod2CDASlowRelease = false; Mod2VacValve = !Mod2VacValve; }
        public void Mod2ToggleVacSlowRelesae() { Mod2VacValve = false; Mod2CDASlowRelease = !Mod2CDASlowRelease; }
        public void Mod2ToggleAntiVac() { Mod2VacValve = false; Mod2AntiVacValve = !Mod2AntiVacValve; }
        public void Mod2ToggleVacFastRelease() { Mod2VacValve = false; Mod2CDASlowRelease = !Mod2CDASlowRelease; }

        public void Mod2SetSlowRelease() { Mod2VacValve = false; Mod2CDASlowRelease = false; Mod2AntiVacValve = true; }
        public void Mod2SetFastRelease() { Mod2VacValve = false; Mod2CDASlowRelease = true; Mod2AntiVacValve = true; }
        public void Mod2ResetVacRelease() { Mod2AntiVacValve = false; Mod2CDASlowRelease = false; }

        public bool Mod2FastRelease => Mod2AntiVacValve && Mod2CDASlowRelease;
        public bool Mod2SlowRelease => Mod2AntiVacValve && !Mod2CDASlowRelease;
        public bool Mod2TempContactor { get { return GlobalData.Bbit[0x064]; } set { PLC.SetDevice("B064", value ? 1 : 0); } }

        public bool Mod2ContactBakeBit { get { return GlobalData.Bbit[0x065]; } set { PLC.SetDevice("B065", value ? 1 : 0); } }
        public bool Mod2ProximityBakeBit { get { return GlobalData.Bbit[0x066]; } set { PLC.SetDevice("B066", value ? 1 : 0); } }
        public bool Mod2LoadUnloadHeightBit { get { return GlobalData.Bbit[0x067]; } set { PLC.SetDevice("B067", value ? 1 : 0); } }
        public bool Mod2HomeBit { get { return GlobalData.Bbit[0x068]; } set { PLC.SetDevice("B068", value ? 1 : 0); } }
        public double Mod2LiftPinHomeOffset { get { return GlobalData.Wword[0x35] / 1000.000; } set { PLC.SetDevice("W35", (int)(value * 1000)); } }
        /// <summary> Height in mms (3 decimal places)</summary>
        public double Mod2LiftPinLDULDHeight { get { return GlobalData.Wword[0x36]/1000.000; } set { PLC.SetDevice("W36", (int)(value * 1000)); } }
        public int Mod2LiftPinAccDcc { get { return GlobalData.Wword[0x37]; } set { PLC.SetDevice("W37", value); } }
        public int Mod2LiftPinVel { get { return GlobalData.Wword[0x38]; } set { PLC.SetDevice("W38", value); } }
        public int Mod2LiftPinPostion { get { return GlobalData.Wword[0x21B]; } set { PLC.SetDevice("W21B", value); } }

        public bool Mod2MoveToContBakeReqActv { get { return GlobalData.Mbit[405]; } set { PLC.SetDevice("M405", value ? 1 : 0); } }
        public bool Mod2MoveToProxBakeReqActv { get { return GlobalData.Mbit[406]; } set { PLC.SetDevice("M406", value ? 1 : 0); } }
        public bool Mod2MoveToLDULDReqActv { get { return GlobalData.Mbit[407]; } set { PLC.SetDevice("M407", value ? 1 : 0); } }
        public bool Mod2MoveToHomeReqActv { get { return GlobalData.Mbit[408]; } set { PLC.SetDevice("M408", value ? 1 : 0); } }


        public double Mod2CurVacLevel => plcFunctions.TwosCompD("D000") / 1000.0;    //TODO  NEED PLC ADDR
        public bool Mod2VacDryInProcess => plcFunctions.GetState("");            //TODO  NEED PLC ADDR
        public int Mod2VacDryProcessTimeout => plcFunctions.GetWord("");         //TODO  NEED PLC ADDR
        public double Mod2VacValveAngle => plcFunctions.GetWord("") / ValveAngleMulti;   //TODO  NEED PLC ADDR


        /// <summary> In seconds </summary>
        public double Mod2DisableDoorOpenTime { get { return GlobalData.Dword[310] / 10.0; } set { PLC.SetDevice("D310", (int)(value * 10)); } }
        public double Mod2EnableDoorTimeRemaining => Mod2DisableDoorOpenTime - (GlobalData.Tword[30] / 10.0);

        #endregion Module 2

        #region Module 3
        public int Mod3State => GlobalData.Dword[325];
        public bool Mod3SubstratePresent => GlobalData.Bbit[0x230];
        public bool Mod3Aborting => GlobalData.Bbit[0x28];
        public bool Mod3Loading => GlobalData.Mbit[422];
        public bool Mod3Unloading => GlobalData.Mbit[426];

        public bool Mod3VacSensor => GlobalData.Mbit[158];
        public bool Mod3DoorClosed => GlobalData.Mbit[159];
        public bool Mod3DoorIsOpen => GlobalData.Mbit[160];
        public bool Mod3SubstrateSensor => GlobalData.Mbit[161];

        public bool Mod3VacEnableLockDoorOpen => GlobalData.Mbit[274];
        public bool Mod3VacDoorLockoutCntdwn => GlobalData.Mbit[275];

        public bool Mod3EnablePower { get { return GlobalData.Mbit[3]; } set { PLC.SetDevice("M3", value ? 1 : 0); } }
        public bool Mod3CommEnabled { get { return GlobalData.Bbit[0x02]; } set { PLC.SetDevice("B002", value ? 1 : 0); } }
        public bool Mod3ResetModule { get { return GlobalData.Bbit[0x0B12]; } set { PLC.SetDevice("B012", value ? 1 : 0); } }
        public bool Mod3MotorError { get { return GlobalData.Mbit[39]; } set { PLC.SetDevice("M39", value ? 1 : 0); } }
        public bool Mod3ResetMotorError { get { return GlobalData.Bbit[0x027]; } set { PLC.SetDevice("B027", value ? 1 : 0); } }
        public bool Mod3MotorErrorResetting => GlobalData.Bbit[0x257];
        public int Mod3ChuckSetPoint { get { return GlobalData.Wword[0x220]; } set { PLC.SetDevice("W220", value); } }
        public double Mod3ChuckTemp => GlobalData.Wword[0x221] / 10.0;
        public string Mod3ProcessTimeRemaining => GlobalData.Wword[0x223].ToString("00") + ":" + GlobalData.Wword[0x224].ToString("00") + ":" + GlobalData.Wword[0x225].ToString("00");
        public string Mod3ProcessTotalTimeRemaining => GlobalData.Wword[0x226].ToString("00") + ":" + GlobalData.Wword[0x2217].ToString("00") + ":" + GlobalData.Wword[0x228].ToString("00");
        public bool Mod3ProcessActive => GlobalData.Lbit[101];
        public bool Mod3LoadEnabled => GlobalData.Mbit[592];
        public bool Mod3UnloadEnabled => GlobalData.Mbit[596];

        public bool Mod3DoorOpenValve { get { return GlobalData.Bbit[0x069]; } set { PLC.SetDevice("B069", value ? 1 : 0); } }
        public bool Mod3DoorCloseValve { get { return GlobalData.Bbit[0x06A]; } set { PLC.SetDevice("B06A", value ? 1 : 0); } }
        public void Mod3ToggleCloseDoorValve() { Mod3DoorOpenValve = false; Mod3DoorCloseValve = !Mod3DoorCloseValve; }
        public void Mod3ToggleOpenDoorValve() { Mod3DoorCloseValve = false; Mod3DoorOpenValve = !Mod3DoorOpenValve; }

        public bool Mod3VacValve { get { return GlobalData.Bbit[0x06B]; } set { PLC.SetDevice("B06B", value ? 1 : 0); } }
        public bool Mod3AntiVacValve { get { return GlobalData.Bbit[0x06C]; } set { PLC.SetDevice("B06C", value ? 1 : 0); } }
        public void Mod3ToggleVac() { Mod3AntiVacValve = false; Mod3VacValve = !Mod3VacValve; }
        public void Mod3ToggleAntiVac() { Mod3VacValve = false; Mod3AntiVacValve = !Mod3AntiVacValve; }
        public bool Mod3TempContactor { get { return GlobalData.Bbit[0x06D]; } set { PLC.SetDevice("B06D", value ? 1 : 0); } }

        public bool Mod3ContactBakeBit { get { return GlobalData.Bbit[0x06E]; } set { PLC.SetDevice("B06E", value ? 1 : 0); } }
        public bool Mod3ProximityBakeBit { get { return GlobalData.Bbit[0x06F]; } set { PLC.SetDevice("B06F", value ? 1 : 0); } }
        public bool Mod3LoadUnloadHeightBit { get { return GlobalData.Bbit[0x070]; } set { PLC.SetDevice("B070", value ? 1 : 0); } }
        public bool Mod3HomeBit { get { return GlobalData.Bbit[0x071]; } set { PLC.SetDevice("B071", value ? 1 : 0); } }
        /// <summary> Height in mms (3 decimal places)</summary>
        public double Mod3LiftPinHomeOffset { get { return GlobalData.Wword[0x3A] / 1000.000; } set { PLC.SetDevice("W3A", (int)(value * 1000)); } }
        /// <summary> Height in mms (3 decimal places)</summary>
        public double Mod3LiftPinLDULDHeight { get { return GlobalData.Wword[0x3B]/1000.000; } set { PLC.SetDevice("W3B", (int)(value * 1000)); } }
        public int Mod3LiftPinAccDcc { get { return GlobalData.Wword[0x3C]; } set { PLC.SetDevice("W3C", value); } }
        public int Mod3LiftPinVel { get { return GlobalData.Wword[0x3D]; } set { PLC.SetDevice("W3D", value); } }
        public int Mod3LiftPinPostion { get { return GlobalData.Wword[0x22B]; } set { PLC.SetDevice("W22B", value); } }

        public bool Mod3MoveToContBakeReqActv { get { return GlobalData.Mbit[409]; } set { PLC.SetDevice("M409", value ? 1 : 0); } }
        public bool Mod3MoveToProxBakeReqActv { get { return GlobalData.Mbit[410]; } set { PLC.SetDevice("M410", value ? 1 : 0); } }
        public bool Mod3MoveToLDULDReqActv { get { return GlobalData.Mbit[411]; } set { PLC.SetDevice("M411", value ? 1 : 0); } }
        public bool Mod3MoveToHomeReqActv { get { return GlobalData.Mbit[412]; } set { PLC.SetDevice("M412", value ? 1 : 0); } }

        /// <summary> In Seconds </summary>
        public double Mod3DisableDoorOpenTime { get { return GlobalData.Dword[311] / 10.0; } set { PLC.SetDevice("D311", (int)(value * 10)); } }
        public double Mod3EnableDoorTimeRemaining => Mod3DisableDoorOpenTime - (GlobalData.Tword[29] / 10.0);

        #endregion Module 3

        #region Loader Arms

        public bool AbortLDULDRequests { get { return GlobalData.Mbit[308]; } set { PLC.SetDevice("M308", value ? 0 : 1); } }
        public bool AbortLDULDRequest { get { return GlobalData.Mbit[309]; } set { PLC.SetDevice("M309", value ? 0 : 1); } }

        public bool MoveToLoadPosReq { get { return GlobalData.Mbit[435]; } set { PLC.SetDevice("M435", value ? 0 : 1); } }

        public int LoaderZLoadVelFast { get { return GlobalData.Dword[400]; } set { PLC.SetDevice("D400", value); } }
        public int LoaderZLoadVelSlow { get { return GlobalData.Dword[401]; } set { PLC.SetDevice("D401", value); } }
        public int LoaderZUnloadVelFast { get { return GlobalData.Dword[402]; } set { PLC.SetDevice("D402", value); } }
        public int LoaderZUnloadVelSlow { get { return GlobalData.Dword[403]; } set { PLC.SetDevice("D403", value); } }
        public int LoaderYVelNoSubstrate { get { return GlobalData.Dword[404]; } set { PLC.SetDevice("D404", value); } }
        public int LoaderYVelSubstrate { get { return GlobalData.Dword[405]; } set { PLC.SetDevice("D405", value); } }

        public double Mod1ZHighPos => plcFunctions.TwosCompD("D410") / 100.00;
        public double Mod1ZLowPos => plcFunctions.TwosCompD("D412") / 100.00;
        public double Mod2ZHighPos => plcFunctions.TwosCompD("D414") / 100.00;
        public double Mod2ZLowPos => plcFunctions.TwosCompD("D416") / 100.00;
        public double Mod3ZHighPos => plcFunctions.TwosCompD("D418") / 100.00;
        public double Mod3ZLowPos => plcFunctions.TwosCompD("D420") / 100.00;
        public double ZLDULDPos => plcFunctions.TwosCompD("D426") / 100.00;
        public double ZDockingPos => plcFunctions.TwosCompD("D424") / 100.00;
        public double ZClearPos => plcFunctions.TwosCompD("D422") / 100.00;
        public double Mod1YPos => plcFunctions.TwosCompD("D428") / 100.00;
        public double Mod2YPos => plcFunctions.TwosCompD("D430") / 100.00;
        public double Mod3YPos => plcFunctions.TwosCompD("D432") / 100.00;
        public double YLDULDPos => plcFunctions.TwosCompD("D436") / 100.00;
        public double YDockingPos => plcFunctions.TwosCompD("D434") / 100.00;
        public double YClearPos => plcFunctions.TwosCompD("D430") / 100.00;

        public bool ZAxisManualMode => GlobalData.Ybit[0x15A];
        public bool ZAxisNoError => GlobalData.Xbit[0x15E];
        public bool YAxisManualMode => GlobalData.Ybit[0x17A];
        public bool YAxisNoError => GlobalData.Xbit[0x17E];

        #endregion Loader Arms

        #region Robot arms
        public bool RobotReadPositions { get { return PLC.ReadRobotPositions; } set { PLC.ReadRobotPositions = value; } }
        public int RobotZPositionRaw => plcFunctions.TwosCompD("D6002");
        public int RobotYPositionRaw => plcFunctions.TwosCompD("D6006");
        #endregion Robot arms


        #region Recipe Stuff

        public bool ReadRecipes { get { return PLC.ReadRecipes; } set { PLC.ReadRecipes = value; } }
        public int RecipeIndex { get { return GlobalData.Dword[2897]; } set { PLC.SetDevice("D2897", value); } }
        public int CurrentRecipeNumber { get { return GlobalData.Dword[2898]; } set { PLC.SetDevice("D2898", value); } }
        public int EditRecipeNumber { get { return GlobalData.Dword[2899]; } set { PLC.SetDevice("D2899", value); } }

        public bool Mod1ChgReqActive { get { return GlobalData.Bbit[0x1B]; } set { PLC.SetDevice("B1B", value ? 1 : 0); } }
        public bool Mod2ChgReqActive { get { return GlobalData.Bbit[0x20]; } set { PLC.SetDevice("B20", value ? 1 : 0); } }
        public bool Mod3ChgReqActive { get { return GlobalData.Bbit[0x25]; } set { PLC.SetDevice("B25", value ? 1 : 0); } }
        public bool RecipeHasChanged { get { return GlobalData.Mbit[390]; } set { PLC.SetDevice("M390", value ? 1 : 0); } }

        public bool DownloadRecipe { get { return GlobalData.Lbit[0x500]; } set { PLC.SetDevice("L500", value ? 1 : 0); } }
        public bool SaveRecipe { get { return GlobalData.Lbit[0x501]; } set { PLC.SetDevice("L501", value ? 1 : 0); } }
        public bool SelectedRecipeChanged { get { return GlobalData.Lbit[0x502]; } set { PLC.SetDevice("L502", value ? 1 : 0); } }

        #region Individual Recipe Vals

        public bool SetRecipeParam(string ParamName, int ParamValue)
        {
            bool bRetVal = true;

            switch (ParamName)
            {
                case "Mod1 Temp": { EditRecipeMod1Temp = ParamValue; } break;
                case "Mod1 At Temp Offset": { EditRecipeMod1TempOffset = ParamValue; } break;
                case "Mod1 Prox Bake Height": { EditRecipeMod1ProxBakeHt = ParamValue; } break;
                case "Mod1 Prox Bake Time Hrs": { EditRecipeMod1ProxBakeTimeHrs = ParamValue; } break;
                case "Mod1 Prox Bake Time Mins": { EditRecipeMod1ProxBakeTimeMins = ParamValue; } break;
                case "Mod1 Prox Bake Time Secs": { EditRecipeMod1ProxBakeTimeSecs = ParamValue; } break;
                case "Mod1 Cont Bake Time Hrs": { EditRecipeMod1ContBakeTimeHrs = ParamValue; } break;
                case "Mod1 Cont Bake Time Mins": { EditRecipeMod1ContBakeTimeMins = ParamValue; } break;
                case "Mod1 Cont Bake Time Secs": { EditRecipeMod1ContBakeTimeSecs = ParamValue; } break;
                case "Mod1 Vac Bake/Dry Time Hrs": { EditRecipeMod1VacTimeHrs = ParamValue; } break;
                case "Mod1 Vac Bake/Dry Time Mins": { EditRecipeMod1VacTimeMins = ParamValue; } break;
                case "Mod1 Vac Bake/Dry Time Secs": { EditRecipeMod1VacTimeSecs = ParamValue; } break;
                case "Mod1 Cool/Chill Time Hrs": { EditRecipeMod1CoolTimeHrs = ParamValue; } break;
                case "Mod1 Cool/Chill Time Mins": { EditRecipeMod1CoolTimeMins = ParamValue; } break;
                case "Mod1 Cool/Chill Time Secs": { EditRecipeMod1CoolTimeSecs = ParamValue; } break;
                case "Mod2 Temp": { EditRecipeMod2Temp = ParamValue; } break;
                case "Mod2 At Temp Offset": { EditRecipeMod2TempOffset = ParamValue; } break;
                case "Mod2 Prox Bake Height": { EditRecipeMod2ProxBakeHt = ParamValue; } break;
                case "Mod2 Prox Bake Time Hrs": { EditRecipeMod2ProxBakeTimeHrs = ParamValue; } break;
                case "Mod2 Prox Bake Time Mins": { EditRecipeMod2ProxBakeTimeMins = ParamValue; } break;
                case "Mod2 Prox Bake Time Secs": { EditRecipeMod2ProxBakeTimeSecs = ParamValue; } break;
                case "Mod2 Cont Bake Time Hrs": { EditRecipeMod2ContBakeTimeHrs = ParamValue; } break;
                case "Mod2 Cont Bake Time Mins": { EditRecipeMod2ContBakeTimeMins = ParamValue; } break;
                case "Mod2 Cont Bake Time Secs": { EditRecipeMod2ContBakeTimeSecs = ParamValue; } break;
                case "Mod2 Vac Bake/Dry Time Hrs": { EditRecipeMod2VacTimeHrs = ParamValue; } break;
                case "Mod2 Vac Bake/Dry Time Mins": { EditRecipeMod2VacTimeMins = ParamValue; } break;
                case "Mod2 Vac Bake/Dry Time Secs": { EditRecipeMod2VacTimeSecs = ParamValue; } break;
                case "Mod2 Cool/Chill Time Hrs": { EditRecipeMod2CoolTimeHrs = ParamValue; } break;
                case "Mod2 Cool/Chill Time Mins": { EditRecipeMod2CoolTimeMins = ParamValue; } break;
                case "Mod2 Cool/Chill Time Secs": { EditRecipeMod2CoolTimeSecs = ParamValue; } break;
                case "Mod3 Temp": { EditRecipeMod3Temp = ParamValue; } break;
                case "Mod3 At Temp Offset": { EditRecipeMod3TempOffset = ParamValue; } break;
                case "Mod3 Prox Bake Height": { EditRecipeMod3ProxBakeHt = ParamValue; } break;
                case "Mod3 Prox Bake Time Hrs": { EditRecipeMod3ProxBakeTimeHrs = ParamValue; } break;
                case "Mod3 Prox Bake Time Mins": { EditRecipeMod3ProxBakeTimeMins = ParamValue; } break;
                case "Mod3 Prox Bake Time Secs": { EditRecipeMod3ProxBakeTimeSecs = ParamValue; } break;
                case "Mod3 Cont Bake Time Hrs": { EditRecipeMod3ContBakeTimeHrs = ParamValue; } break;
                case "Mod3 Cont Bake Time Mins": { EditRecipeMod3ContBakeTimeMins = ParamValue; } break;
                case "Mod3 Cont Bake Time Secs": { EditRecipeMod3ContBakeTimeSecs = ParamValue; } break;
                case "Mod3 Vac Bake/Dry Time Hrs": { EditRecipeMod3VacTimeHrs = ParamValue; } break;
                case "Mod3 Vac Bake/Dry Time Mins": { EditRecipeMod3VacTimeMins = ParamValue; } break;
                case "Mod3 Vac Bake/Dry Time Secs": { EditRecipeMod3VacTimeSecs = ParamValue; } break;
                case "Mod3 Cool/Chill Time Hrs": { EditRecipeMod3CoolTimeHrs = ParamValue; } break;
                case "Mod3 Cool/Chill Time Mins": { EditRecipeMod3CoolTimeMins = ParamValue; } break;
                case "Mod3 Cool/Chill Time Secs": { EditRecipeMod3CoolTimeSecs = ParamValue; } break;
                default: { bRetVal = false; } break;
            }

            return bRetVal;
        }

        public bool SetRecipeParam(string ParamName, double ParamValue)
        {
            bool bRetVal = true;

            switch (ParamName)
            {
                case "Step 1 Vlv Angle": { EditRecipeStep1ValveAngle = ParamValue; } break;
                case "Step 2 Vlv Angle": { EditRecipeStep2ValveAngle = ParamValue; } break;
                case "Step 3 Vlv Angle": { EditRecipeStep3ValveAngle = ParamValue; } break;
                case "Step 4 Vlv Angle": { EditRecipeStep4ValveAngle = ParamValue; } break;
                case "Dwell Vlv Angle": { EditRecipeStepDwellHoldValveAngle = ParamValue; } break;
                case "Step 1 Pres Setpoint": { EditRecipeStep1PresSetpoint = ParamValue; } break;
                case "Step 2 Pres Setpoint": { EditRecipeStep2PresSetpoint = ParamValue; } break;
                case "Step 3 Pres Setpoint": { EditRecipeStep3PresSetpoint = ParamValue; } break;
                case "Step 4 Pres Setpoint": { EditRecipeStep4PresSetpoint = ParamValue; } break;
                case "Dwell Pres Setpoint": { EditRecipeStepDwellHoldPresSetpoint = ParamValue; } break;
                default: { bRetVal = false; } break;
            }

            return bRetVal;
        }

        public bool SetRecipeParam(string ParamName, TimeSpan ParamValue)
        {
            bool bRetVal = true;

            switch (ParamName)
            {
                case "Step1 Time": { EditRecipeStep1Time = ParamValue; } break;
                case "Step2 Time": { EditRecipeStep2Time = ParamValue; } break;
                case "Step3 Time": { EditRecipeStep3Time = ParamValue; } break;
                case "Step4 Time": { EditRecipeStep4Time = ParamValue; } break;
                case "Dwell Time": { EditRecipeDwellHoldTime = ParamValue; } break;
                case "MaxVac Time": { EditRecipeMaxVacTime = ParamValue; } break;
                case "VacRelief Time": { EditRecipeVacReliefTime = ParamValue; } break;
                default: { bRetVal = false; } break;
            }

            return bRetVal;
        }


        public int EditRecipeMod1Temp { get { return GlobalData.Dword[2910]; } set { PLC.SetDevice("D2910", value); } }
        public int EditRecipeMod1TempOffset { get { return GlobalData.Dword[2911]; } set { PLC.SetDevice("D2911", value); } }
        public int EditRecipeMod1ProxBakeHt { get { return GlobalData.Dword[2912]; } set { PLC.SetDevice("D2912", value); } }
        public int EditRecipeMod1ProxBakeTimeHrs { get { return GlobalData.Dword[2913]; } set { PLC.SetDevice("D2913", value); } }
        public int EditRecipeMod1ProxBakeTimeMins { get { return GlobalData.Dword[2914]; } set { PLC.SetDevice("D2914", value); } }
        public int EditRecipeMod1ProxBakeTimeSecs { get { return GlobalData.Dword[2915]; } set { PLC.SetDevice("D2915", value); } }
        public int EditRecipeMod1ContBakeTimeHrs { get { return GlobalData.Dword[2916]; } set { PLC.SetDevice("D2916", value); } }
        public int EditRecipeMod1ContBakeTimeMins { get { return GlobalData.Dword[2917]; } set { PLC.SetDevice("D2917", value); } }
        public int EditRecipeMod1ContBakeTimeSecs { get { return GlobalData.Dword[2918]; } set { PLC.SetDevice("D2918", value); } }
        public int EditRecipeMod1VacTimeHrs { get { return GlobalData.Dword[2919]; } set { PLC.SetDevice("D2919", value); } }
        public int EditRecipeMod1VacTimeMins { get { return GlobalData.Dword[2920]; } set { PLC.SetDevice("D2920", value); } }
        public int EditRecipeMod1VacTimeSecs { get { return GlobalData.Dword[2921]; } set { PLC.SetDevice("D2921", value); } }
        public int EditRecipeMod1CoolTimeHrs { get { return GlobalData.Dword[2922]; } set { PLC.SetDevice("D2922", value); } }
        public int EditRecipeMod1CoolTimeMins { get { return GlobalData.Dword[2923]; } set { PLC.SetDevice("D2923", value); } }
        public int EditRecipeMod1CoolTimeSecs { get { return GlobalData.Dword[2924]; } set { PLC.SetDevice("D2924", value); } }
        public int EditRecipeMod2Temp { get { return GlobalData.Dword[2925]; } set { PLC.SetDevice("D2925", value); } }
        public int EditRecipeMod2TempOffset { get { return GlobalData.Dword[2926]; } set { PLC.SetDevice("D2926", value); } }
        public int EditRecipeMod2ProxBakeHt { get { return GlobalData.Dword[2927]; } set { PLC.SetDevice("D2927", value); } }
        public int EditRecipeMod2ProxBakeTimeHrs { get { return GlobalData.Dword[2928]; } set { PLC.SetDevice("D2928", value); } }
        public int EditRecipeMod2ProxBakeTimeMins { get { return GlobalData.Dword[2929]; } set { PLC.SetDevice("D2929", value); } }
        public int EditRecipeMod2ProxBakeTimeSecs { get { return GlobalData.Dword[2930]; } set { PLC.SetDevice("D2930", value); } }
        public int EditRecipeMod2ContBakeTimeHrs { get { return GlobalData.Dword[2931]; } set { PLC.SetDevice("D2931", value); } }
        public int EditRecipeMod2ContBakeTimeMins { get { return GlobalData.Dword[2932]; } set { PLC.SetDevice("D2932", value); } }
        public int EditRecipeMod2ContBakeTimeSecs { get { return GlobalData.Dword[2933]; } set { PLC.SetDevice("D2933", value); } }
        public int EditRecipeMod2VacTimeHrs { get { return GlobalData.Dword[2934]; } set { PLC.SetDevice("D2934", value); } }
        public int EditRecipeMod2VacTimeMins { get { return GlobalData.Dword[2935]; } set { PLC.SetDevice("D2935", value); } }
        public int EditRecipeMod2VacTimeSecs { get { return GlobalData.Dword[2936]; } set { PLC.SetDevice("D2936", value); } }
        public int EditRecipeMod2CoolTimeHrs { get { return GlobalData.Dword[2937]; } set { PLC.SetDevice("D2937", value); } }
        public int EditRecipeMod2CoolTimeMins { get { return GlobalData.Dword[2938]; } set { PLC.SetDevice("D2938", value); } }
        public int EditRecipeMod2CoolTimeSecs { get { return GlobalData.Dword[2939]; } set { PLC.SetDevice("D2939", value); } }
        public int EditRecipeMod3Temp { get { return GlobalData.Dword[2940]; } set { PLC.SetDevice("D2940", value); } }
        public int EditRecipeMod3TempOffset { get { return GlobalData.Dword[2941]; } set { PLC.SetDevice("D2941", value); } }
        public int EditRecipeMod3ProxBakeHt { get { return GlobalData.Dword[2942]; } set { PLC.SetDevice("D2942", value); } }
        public int EditRecipeMod3ProxBakeTimeHrs { get { return GlobalData.Dword[2943]; } set { PLC.SetDevice("D2943", value); } }
        public int EditRecipeMod3ProxBakeTimeMins { get { return GlobalData.Dword[2944]; } set { PLC.SetDevice("D2944", value); } }
        public int EditRecipeMod3ProxBakeTimeSecs { get { return GlobalData.Dword[2945]; } set { PLC.SetDevice("D2945", value); } }
        public int EditRecipeMod3ContBakeTimeHrs { get { return GlobalData.Dword[2946]; } set { PLC.SetDevice("D2946", value); } }
        public int EditRecipeMod3ContBakeTimeMins { get { return GlobalData.Dword[2947]; } set { PLC.SetDevice("D2947", value); } }
        public int EditRecipeMod3ContBakeTimeSecs { get { return GlobalData.Dword[2948]; } set { PLC.SetDevice("D2948", value); } }
        public int EditRecipeMod3VacTimeHrs { get { return GlobalData.Dword[2949]; } set { PLC.SetDevice("D2949", value); } }
        public int EditRecipeMod3VacTimeMins { get { return GlobalData.Dword[2950]; } set { PLC.SetDevice("D2950", value); } }
        public int EditRecipeMod3VacTimeSecs { get { return GlobalData.Dword[2951]; } set { PLC.SetDevice("D2951", value); } }
        public int EditRecipeMod3CoolTimeHrs { get { return GlobalData.Dword[2952]; } set { PLC.SetDevice("D2952", value); } }
        public int EditRecipeMod3CoolTimeMins { get { return GlobalData.Dword[2953]; } set { PLC.SetDevice("D2953", value); } }
        public int EditRecipeMod3CoolTimeSecs { get { return GlobalData.Dword[2954]; } set { PLC.SetDevice("D2954", value); } }
        public string EditRecipeName { get { return GetRecipeName(2900); } set { SetEditRecipeName(value); } }

        public double EditRecipeStep1ValveAngle { get { return GlobalData.Dword[3700] / 10.0; } set { PLC.SetDevice("D3700", (int)(value * 10)); } }
        public double EditRecipeStep1PresSetpoint { get { return GlobalData.Dword[3701] / 10.0; } set { PLC.SetDevice("D3701", (int)(value * 10)); } }
        public TimeSpan EditRecipeStep1Time { get { return GetEditRecipeTime("Step1"); } set { SaveEditRecipeTime("Step1", value); } }
        public double EditRecipeStep2ValveAngle { get { return GlobalData.Dword[3705] / 10.0; } set { PLC.SetDevice("D3705", (int)(value * 10)); } }
        public double EditRecipeStep2PresSetpoint { get { return GlobalData.Dword[3706] / 10.0; } set { PLC.SetDevice("D3706", (int)(value * 10)); } }
        public TimeSpan EditRecipeStep2Time { get { return GetEditRecipeTime("Step2"); } set { SaveEditRecipeTime("Step2", value); } }
        public double EditRecipeStep3ValveAngle { get { return GlobalData.Dword[3710] / 10.0; } set { PLC.SetDevice("D3710", (int)(value * 10)); } }
        public double EditRecipeStep3PresSetpoint { get { return GlobalData.Dword[3711] / 10.0; } set { PLC.SetDevice("D3711", (int)(value * 10)); } }
        public TimeSpan EditRecipeStep3Time { get { return GetEditRecipeTime("Step3"); } set { SaveEditRecipeTime("Step3", value); } }
        public double EditRecipeStep4ValveAngle { get { return GlobalData.Dword[3715] / 10.0; } set { PLC.SetDevice("D3715", (int)(value * 10)); } }
        public double EditRecipeStep4PresSetpoint { get { return GlobalData.Dword[3716] / 10.0; } set { PLC.SetDevice("D3716", (int)(value * 10)); } }
        public TimeSpan EditRecipeStep4Time { get { return GetEditRecipeTime("Step4"); } set { SaveEditRecipeTime("Step4", value); } }
        public double EditRecipeStepDwellHoldValveAngle { get { return GlobalData.Dword[3720] / 10.0; } set { PLC.SetDevice("D3720", (int)(value * 10)); } }
        public double EditRecipeStepDwellHoldPresSetpoint { get { return GlobalData.Dword[3721] / 10.0; } set { PLC.SetDevice("D3721", (int)(value * 10)); } }
        public TimeSpan EditRecipeDwellHoldTime { get { return GetEditRecipeTime("StepDwellHold"); } set { SaveEditRecipeTime("StepDwellHold", value); } }
        public TimeSpan EditRecipeMaxVacTime { get { return GetEditRecipeTime("MaxTime"); } set { SaveEditRecipeTime("MaxTime", value); } }
        public TimeSpan EditRecipeVacReliefTime { get { return GetEditRecipeTime("ReliefTime"); } set { SaveEditRecipeTime("ReliefTime", value); } }

        #endregion Individual Recipe Vals

        #endregion Recipe Stuff

        #endregion Status Properties

        #region Operations
        public bool AbortModuleProcess(int ModuleNum)
        {
            bool bRetVal = true;
            switch (ModuleNum)
            {
                case 1:
                {
                    PLC.SetDevice("B1E", 1);
                }
                break;
                case 2:
                {
                    PLC.SetDevice("B19", 1);
                }
                break;
                case 3:
                {

                }
                break;
            }

            return bRetVal;
        }


        #endregion Operations

        #region Robot Wrappers

        public bool SetRobotManualFlag(bool IsSet)
        {
            bool bRetVal = true;
            if (IsSet)
            {
                if (!GlobalData.Ybit[0x17A])
                    PLC.SetDevice("Y17A", 1);
                if (!GlobalData.Ybit[0x15A])
                    PLC.SetDevice("Y15A", 1);
            }
            else
            {
                if (GlobalData.Ybit[0x17A])
                    PLC.SetDevice("Y17A", IsSet ? 1 : 0);
                if (GlobalData.Ybit[0x15A])
                    PLC.SetDevice("Y15A", IsSet ? 1 : 0);
            }
            return bRetVal;
        }

        public bool ParkEndEffector()
        {
            _log.log(LogType.TRACE, Category.INFO, "PLC Park End Effector Requested", "Info");
            PLC.SetDevice("M297", 1);
            return true;
        }

        public bool PickUpEndEffector()
        {
            _log.log(LogType.TRACE, Category.INFO, "PLC Pick Up End Effector Requested", "Info");
            PLC.SetDevice("M296", 1);
            return true;
        }

        public bool MoveEndEffector2LDULDP()
        {
            bool bRetVal = true;
            _log.log(LogType.TRACE, Category.INFO, "PLC Move End Effector to Load/Unload pos Requested", "Info");
            PLC.SetDevice("M435", 1);
            return bRetVal;
        }

        public bool LoadModule(int ModuleNum)
        {
            bool bRetVal = true;
            switch (ModuleNum)
            {
                case 1:
                {
                    _log.log(LogType.TRACE, Category.INFO, "PLC Load Mod 1", "Info");
                    PLC.SetDevice("M300", 1);
                }
                break;
                case 2:
                {
                    _log.log(LogType.TRACE, Category.INFO, "PLC Load Mod 2", "Info");
                    PLC.SetDevice("M301", 1);
                }
                break;
                case 3:
                {
                    _log.log(LogType.TRACE, Category.INFO, "PLC Load Mod 3", "Info");
                    PLC.SetDevice("M302", 1);
                }
                break;
                default:
                {
                    _log.log(LogType.TRACE, Category.INFO, $"PLC Load request for unknown mod num '{ModuleNum}'", "Warning");
                    bRetVal = false;
                }
                break;
            }

            return bRetVal;
        }
        public bool UnLoadModule(int ModuleNum)
        {
            bool bRetVal = true;
            switch (ModuleNum)
            {
                case 1:
                {
                    _log.log(LogType.TRACE, Category.INFO, "PLC UnLoad Mod 1", "Info");
                    PLC.SetDevice("M304", 1);
                }
                break;
                case 2:
                {
                    _log.log(LogType.TRACE, Category.INFO, "PLC UnLoad Mod 2", "Info");
                    PLC.SetDevice("M305", 1);
                }
                break;
                case 3:
                {
                    _log.log(LogType.TRACE, Category.INFO, "PLC UnLoad Mod 3", "Info");
                    PLC.SetDevice("M306", 1);
                }
                break;
                default:
                {
                    _log.log(LogType.TRACE, Category.INFO, $"PLC UnLoad request for unknown mod num '{ModuleNum}'", "Warning");
                    bRetVal = false;
                }
                break;
            }

            return bRetVal;
        }

        public bool GotoClearPosEndEffector()
        {
            _log.log(LogType.TRACE, Category.INFO, "PLC Goto Clear Position Requested", "Info");
            PLC.SetDevice("M317", 1);
            return true;
        }

        public bool MoveZToStackMod1High()
        {
            bool bRetVal = true;
            PLC.SetDevice("M311", 1);
            return bRetVal;
        }

        public bool MoveZToStackMod1Low()
        {
            bool bRetVal = true;
            PLC.SetDevice("M312", 1);
            return bRetVal;
        }

        public bool MoveZToStackMod2High()
        {
            bool bRetVal = true;
            PLC.SetDevice("M313", 1);
            return bRetVal;
        }

        public bool MoveZToStackMod2Low()
        {
            bool bRetVal = true;
            PLC.SetDevice("M314", 1);
            return bRetVal;
        }

        public bool MoveZToStackMod3High()
        {
            bool bRetVal = true;
            PLC.SetDevice("M315", 1);
            return bRetVal;
        }

        public bool MoveZToStackMod3Low()
        {
            bool bRetVal = true;
            PLC.SetDevice("M316", 1);
            return bRetVal;
        }

        public bool MoveZToClearPos()
        {
            bool bRetVal = true;
            PLC.SetDevice("M317", 1);
            return bRetVal;
        }

        public bool MoveZToDockingPos()
        {
            bool bRetVal = true;
            PLC.SetDevice("M318", 1);
            return bRetVal;
        }

        public bool MoveZToLDULDPos()
        {
            bool bRetVal = true;
            PLC.SetDevice("M319", 1);
            return bRetVal;
        }

        public int SetRobotArmZVel(int NewSpeed)
        {
            int nRetVal;
            nRetVal = PLC.SetDevice("D406", NewSpeed);
            return nRetVal;
        }

        public bool MoveYToStackMod1()
        {
            bool bRetVal = true;
            PLC.SetDevice("M325", 1);
            return bRetVal;
        }

        public bool MoveYToStackMod2()
        {
            bool bRetVal = true;
            PLC.SetDevice("M326", 1);
            return bRetVal;
        }

        public bool MoveYToStackMod3()
        {
            bool bRetVal = true;
            PLC.SetDevice("M327", 1);
            return bRetVal;
        }

        public bool MoveYToClearPos()
        {
            bool bRetVal = true;
            PLC.SetDevice("M328", 1);
            return bRetVal;
        }

        public bool MoveYToLDULDPos()
        {
            bool bRetVal = true;
            PLC.SetDevice("M329", 1);
            return bRetVal;
        }

        public bool MoveYToDockingPos()
        {
            bool bRetVal = true;
            PLC.SetDevice("M328", 1);
            return bRetVal;
        }

        public int SetRobotArmYVel(int NewSpeed)
        {
            int nRetVal;
            nRetVal = PLC.SetDevice("D407", NewSpeed);
            return nRetVal;
        }

        public bool ReqHomeLoader()
        {
            bool bRetVal = true;
            HomeLoader = true;
            return bRetVal;
        }

        public bool DockEndEffector()
        {
            bool bRetVal = true;
            if (GlobalData.Mbit[290])
            {
                PLC.SetDevice("M290", 0);
                PLC.SetDevice("M291", 1);
            }
            else
            {
                PLC.SetDevice("M290", 1);
                PLC.SetDevice("M291", 0);
            }
            return bRetVal;
        }

        /// <summary>
        /// Toggles, sets or resets vacuum cups on End Effector
        /// :  -1 Toggle; 0 Off; 1 On
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        public bool EnableEndEffectorVac(int State)
        {
            bool bRetVal = true;
            switch (State)
            {
                case -1: // Toggle
                {
                    PLC.SetDevice("M310", GlobalData.Mbit[310] ? 0 : 1);
                }
                break;
                case 0:  // Reset
                {
                    PLC.SetDevice("M310", 0);
                }
                break;
                case 1:  // Set
                {
                    PLC.SetDevice("M310", 1);
                }
                break;
            }
            return bRetVal;
        }
        #endregion Robot Wrappers

        #region Teaching Helpers

        // Clear, LDULD, Docking, MOD1, MOD2, MOD3, MOD1Hi, MOD2Hi, MOD3Hi, MOD1Low, MOD2Low, MOD3Low
        public bool TeachZPos(StackPosID PosID)
        {
            bool bRetVal = true;
            switch (PosID)
            {
                case StackPosID.Clear:
                {
                    PLC.SetDevice("D422", GlobalData.Dword[6002]);
                    PLC.SetDevice("D423", GlobalData.Dword[6003]);
                }
                break;
                case StackPosID.LDULD:
                {
                    PLC.SetDevice("D426", GlobalData.Dword[6002]);
                    PLC.SetDevice("D427", GlobalData.Dword[6003]);
                }
                break;
                case StackPosID.Docking:
                {
                    PLC.SetDevice("D424", GlobalData.Dword[6002]);
                    PLC.SetDevice("D425", GlobalData.Dword[6003]);
                }
                break;
                case StackPosID.MOD1Hi:
                {
                    PLC.SetDevice("D410", GlobalData.Dword[6002]);
                    PLC.SetDevice("D411", GlobalData.Dword[6003]);
                }
                break;
                case StackPosID.MOD2Hi:
                {
                    PLC.SetDevice("D414", GlobalData.Dword[6002]);
                    PLC.SetDevice("D415", GlobalData.Dword[6003]);
                }
                break;
                case StackPosID.MOD3Hi:
                {
                    PLC.SetDevice("D418", GlobalData.Dword[6002]);
                    PLC.SetDevice("D419", GlobalData.Dword[6003]);
                }
                break;
                case StackPosID.MOD1Low:
                {
                    PLC.SetDevice("D412", GlobalData.Dword[6002]);
                    PLC.SetDevice("D413", GlobalData.Dword[6003]);
                }
                break;
                case StackPosID.MOD2Low:
                {
                    PLC.SetDevice("D416", GlobalData.Dword[6002]);
                    PLC.SetDevice("D417", GlobalData.Dword[6003]);
                }
                break;
                case StackPosID.MOD3Low:
                {
                    PLC.SetDevice("D420", GlobalData.Dword[6002]);
                    PLC.SetDevice("D421", GlobalData.Dword[6003]);
                }
                break;
                default:
                {
                    bRetVal = false;
                }
                break;
            }

            return bRetVal;
        }

        public bool TeachYPos(StackPosID PosID)
        {
            bool bRetVal = true;
            switch (PosID)
            {
                case StackPosID.Clear:
                {
                    PLC.SetDevice("D438", GlobalData.Dword[6006]);
                    PLC.SetDevice("D439", GlobalData.Dword[6007]);
                }
                break;
                case StackPosID.LDULD:
                {
                    PLC.SetDevice("D436", GlobalData.Dword[6006]);
                    PLC.SetDevice("D437", GlobalData.Dword[6007]);
                }
                break;
                case StackPosID.Docking:
                {
                    PLC.SetDevice("D434", GlobalData.Dword[6006]);
                    PLC.SetDevice("D435", GlobalData.Dword[6007]);
                }
                break;
                case StackPosID.MOD1:
                {
                    PLC.SetDevice("D428", GlobalData.Dword[6006]);
                    PLC.SetDevice("D429", GlobalData.Dword[6007]);
                }
                break;
                case StackPosID.MOD2:
                {
                    PLC.SetDevice("D430", GlobalData.Dword[6006]);
                    PLC.SetDevice("D431", GlobalData.Dword[6007]);
                }
                break;
                case StackPosID.MOD3:
                {
                    PLC.SetDevice("D432", GlobalData.Dword[6006]);
                    PLC.SetDevice("D433", GlobalData.Dword[6007]);
                }
                break;
                default:
                {
                    bRetVal = false;
                }
                break;
            }
            return bRetVal;
        }

        #endregion Teaching Helpers

        #region Recipe Methods
        // Informs the PLC to include Recipe in the data query
        private string GetRecipeName(int startingAddress)
        {
            string sRetVal = "";
            string strRecipeName = "";
            for (int i = startingAddress; i < (startingAddress+10); i++)
            {
                strRecipeName = strRecipeName + plcFunctions.ConvertToAscii(GlobalData.Dword[i]);
            }
            sRetVal = strRecipeName.Replace("\0", "");
            sRetVal = sRetVal.TrimEnd();
            return sRetVal;
        }

        private int[] ConvertRecipeName(string recipeName)
        {
            int[] nRetVal = new int[10];
            recipeName = recipeName.PadRight(20);
            string hexString = "";
            char[] characters = recipeName.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 20; i = i + 2)
            {
                string strChar = string.Format("{0:X}", Convert.ToInt32(characters[i + 1]));
                sb.Append(strChar);
                strChar = string.Format("{0:X}", Convert.ToInt32(characters[i]));
                sb.Append(strChar);
            }
            hexString = sb.ToString();
            int idx = 0;
            for (int i = 0; i < 36; i = i + 4)
            {
                nRetVal[idx] = Int32.Parse(hexString.Substring(i, 4), System.Globalization.NumberStyles.HexNumber);
                idx++;
            }
            return nRetVal;
        }

        private void SetEditRecipeName(string RecipeName)
        {
            int[] nData = new int[10];
            nData = ConvertRecipeName(RecipeName);
            StringBuilder sb = new StringBuilder();
            for (int i = 2900; i < 2910; i++)
            {
                sb.Append("D").Append(i);
                if (i < 2909)
                    sb.Append("\n");
            }
            string deviceList = sb.ToString();
            int nResult = PLC == null ? -1 : PLC.WriteDeviceRandom(deviceList, 10, nData);
            if (nResult == 0)
                return;
        }

        public bool UpdateRecipeRunning(bool RecipeIsRunning)
        {
            bool bRetVal = GlobalData.Mbit[286];
            //if (_bRunningRecipe && !GlobalData.Mbit[286])
            //	PLC.SetDevice("M286", 1);
            //if (!_bRunningRecipe && GlobalData.Mbit[286])
            //	PLC.SetDevice("M286", 0);
            if (RecipeIsRunning != GlobalData.Mbit[286])
            {
                _log.log(LogType.TRACE, Category.INFO, $"Changing Recipe Running flag on PLC to '{RecipeIsRunning}'");
                PLC.SetDevice("M286", RecipeIsRunning ? 1 : 0);
            }
            return bRetVal;
        }

        public TimeSpan GetEditRecipeTime(string StepName)
        {
            TimeSpan tsRetVal = new TimeSpan();

            switch (StepName)
            {
                case "Step1": //D3702
                {
                    tsRetVal = new TimeSpan(GlobalData.Dword[3702], GlobalData.Dword[3703], GlobalData.Dword[3704]);
                }
                break;
                case "Step2": //D3707
                {
                    tsRetVal = new TimeSpan(GlobalData.Dword[3707], GlobalData.Dword[3708], GlobalData.Dword[3709]);
                }
                break;
                case "Step3": //D3712
                {
                    tsRetVal = new TimeSpan(GlobalData.Dword[3712], GlobalData.Dword[3713], GlobalData.Dword[3714]);
                }
                break;
                case "Step4": //D3717
                {
                    tsRetVal = new TimeSpan(GlobalData.Dword[3717], GlobalData.Dword[3718], GlobalData.Dword[3719]);
                }
                break;
                case "StepDwellHold": //D3722
                {
                    tsRetVal = new TimeSpan(GlobalData.Dword[3722], GlobalData.Dword[3723], GlobalData.Dword[3724]);
                }
                break;
                case "MaxTime": //D3725
                {
                    tsRetVal = new TimeSpan(GlobalData.Dword[3725], GlobalData.Dword[3726], GlobalData.Dword[3727]);
                }
                break;
                case "ReliefTime": //D3728
                {
                    tsRetVal = new TimeSpan(GlobalData.Dword[3728], GlobalData.Dword[3729], GlobalData.Dword[3730]);
                }
                break;

                default: break;
            }

            return tsRetVal;
        }

        public bool SaveEditRecipeTime(string StepName, TimeSpan timeSpan)
        {
            bool bRetVal = true;

            switch (StepName)
            {
                case "Step1": //D3702
                {
                    PLC.SetDevice("D3702", timeSpan.Hours);
                    PLC.SetDevice("D3703", timeSpan.Minutes);
                    PLC.SetDevice("D3704", timeSpan.Seconds);
                }
                break;
                case "Step2": //D3707
                {
                    PLC.SetDevice("D3707", timeSpan.Hours);
                    PLC.SetDevice("D3708", timeSpan.Minutes);
                    PLC.SetDevice("D3709", timeSpan.Seconds);
                }
                break;
                case "Step3": //D3712
                {
                    PLC.SetDevice("D3712", timeSpan.Hours);
                    PLC.SetDevice("D3713", timeSpan.Minutes);
                    PLC.SetDevice("D3714", timeSpan.Seconds);
                }
                break;
                case "Step4": //D3717
                {
                    PLC.SetDevice("D3717", timeSpan.Hours);
                    PLC.SetDevice("D3718", timeSpan.Minutes);
                    PLC.SetDevice("D3719", timeSpan.Seconds);
                }
                break;
                case "StepDwellHold": //D3722
                {
                    PLC.SetDevice("D3722", timeSpan.Hours);
                    PLC.SetDevice("D3723", timeSpan.Minutes);
                    PLC.SetDevice("D3724", timeSpan.Seconds);
                }
                break;
                case "MaxTime": //D3725
                {
                    PLC.SetDevice("D3725", timeSpan.Hours);
                    PLC.SetDevice("D3726", timeSpan.Minutes);
                    PLC.SetDevice("D3727", timeSpan.Seconds);
                }
                break;
                case "ReliefTime": //D3728
                {
                    PLC.SetDevice("D3728", timeSpan.Hours);
                    PLC.SetDevice("D3729", timeSpan.Minutes);
                    PLC.SetDevice("D3730", timeSpan.Seconds);
                }
                break;

                default: { bRetVal = false; } break;
            }

            return bRetVal;
        }

        #endregion Recipe Methods


        #region General Stack Ops
        /// <summary>
        /// Toggles, Sets or Resets	maintenance mode for the stack
        /// :  -1 Toggle; 0 Off; 1 On
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        public bool EnableStackMaintMode(int State)
        {
            bool bRetVal = true;
            switch (State)
            {
                case -1:   // Toggle
                {
                    StackInMaintMode = !StackInMaintMode; // PLC.SetDevice("B56", GlobalData.Bbit[0x56] ? 0 : 1);
                }
                break;
                case 0:    // Reset
                {
                    StackInMaintMode = false; // PLC.SetDevice("B56", 0);
                }
                break;
                case 1:    // Set
                {
                    StackInMaintMode = true; // PLC.SetDevice("B56", 1);
                }
                break;
            }
            return bRetVal;
        }

        #region Alarms

        /// <summary>General Test for ANY stack alarms present</summary>
        public bool StackAlarmsExist
        {
            get
            {
                for (int i = 0; i < 100; i++)
                {
                    if (GlobalData.Mbit[1600 + i])
                    {
                        return true; // stop on the first one... just a trigger and we want as fast as possible-
                    }
                }

                return false;
            }
        }

        /// <summary>Count of Currently Active Stack Alarms</summary>
        public int StackAlarmCount
        {
            get
            {
                int nRetVal = 0;
                for (int i = 0; i < 100; i++)
                {
                    if (GlobalData.Mbit[1600 + i])
                        nRetVal++;
                }
                return nRetVal;
            }
        }

        /// <summary>
        /// Determines if a Specific Alarm Exists.
        /// </summary>
        /// <param name="AlarmIndex">0 Based reflecting M1600 bit setting.</param>
        /// <returns></returns>
        public bool AlarmExists(int AlarmIndex)
        {
            return GlobalData.Mbit[1600 + AlarmIndex];
        }

        /// <summary>
        /// Returns the alarm information requested (includes No, Text and Color)
        /// </summary>
        /// <param name="AlarmIndex">0 based index of Alarm</param>
        /// <returns></returns>
        public Alarm GetAlarm(int AlarmIndex)
        {
            if (AlarmIndex < StackAlarms.Count)
            {
                return StackAlarms[AlarmIndex];
            }

            return null;
        }

        /// <summary>Highest Index of Alarm (from csv file)</summary>
        public int MaxAlarmNumber => StackAlarms.Count-1;

        #endregion Alarms

        public void ResetEMO()
        {
            PLC.SetDevice("Y049", 1);
        }
        public bool ResetStackError()
        {
            bool bRetVal = true;
            PLC.SetDevice("M1599", 1);
            return bRetVal;
        }

        #endregion General Stack Ops

        #region Address Maps

        private void LoadAlarms()
        {
            int idx = 0;
            try
            {
                var reader = new StreamReader(File.OpenRead(@"Data\StackAlarms.csv"));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    if (values.Length == 3)
                    {
                        Alarm newAlarm = new Alarm();
                        newAlarm.AlarmNo = int.Parse(values[0]);
                        newAlarm.Message = values[1];
                        newAlarm.ForeColor = Color.FromName(values[2]);
                        StackAlarms.Add(newAlarm);
                        idx++;
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Error Loading Alarms!" + ex.ToString(), "ERROR");
            }
        }

        #endregion Address Maps
    }

    public class Alarm
    {
        public int AlarmNo { get; set; } = 0;
        public string Message { get; set; } = "Alarm";
        public Color ForeColor { get; set; } = Color.White;
    }
}
