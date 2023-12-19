using Galil;
using nAble;
using nAble.Data;
using nAble.DataComm;
using nAble.DataComm.KeyenceLasers;
using nAble.Enums;
using nAble_for_nRad2.Properties;
using nRadLite.DataComm.IR;
using nTact.Recipes;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using static nTact.DataComm.KeyenceComm2;

namespace nTact.DataComm
{
    public enum LiftPinStatus { Idle, Prep2LD, Loading, Unloading, Raising, Lowering, Transfer }
    public enum Usage { PrimingVelocity, DispenseRate, CoatingVelocity, SuckbackRate, ZStageVelocity, JumpVelocity, ShuttleReturnVelocity }
    public enum Zones { Priming, Zone1, Zone2, Zone3, AllChuck }
    public enum StatusEventArgType { Position = 0, Message = 1, RunStatus = 2 }
    public enum PumpOp { Head, Pump }

    public class ParamException : Exception
    {
        public ParamException(string msg = "") : base(msg) { }
    }

    public class StatusEventArgs : EventArgs
    {
        public int ValPos = 0;
        public string Message = "";
        public StatusEventArgType EventType = StatusEventArgType.Message;

        public StatusEventArgs(StatusEventArgType eType, int nValPos, string sMsg)
        {
            ValPos = nValPos;
            Message = sMsg;
            EventType = eType;
        }
    }

    public class RecipeEventArgs : EventArgs
    {
        public int ReturnCode = 0;
        public string Message = "";
        public RecipeEventArgs(int nRC, string sMsg)
        {
            ReturnCode = nRC;
            Message = sMsg;
        }
    }

    public class MotionProfile
    {
        public double MinAcc { get; set; } = 0;
        public double MaxAcc { get; private set; } = 0;

        public MotionProfile(double min, double max)
        {
            MinAcc = min;
            MaxAcc = max;
        }
    }

    public class GalilWrapper2
    {
        #region Properties

        #region Private Properties

        private Galil.Galil _objGalil = null;
        private KeyenceComm2 _keyence = null;
        private readonly LogEntry _log = null;
        private DateTime _dtLastRecord = DateTime.Now;
        private DateTime _dtLastRecordReceived = DateTime.Now;
        private readonly double[] _DiePressureVDC = new double[1000];
        private readonly double[] _DiePressure = new double[1000];
        private readonly object _CommandLock = new object();
        private DateTime _dtNextAllowedQU = DateTime.Now;
        private bool _bCommutating = false;
        private bool _bReadingArray = false;
        private bool _bConnecting = false;
        private bool _bResetting = false;
        private double dLastEMOState = 0, dLastTA3 = 0;
        private bool bLastELOLatched = false;
        private TimeSpan _tsUpdate;
        private int _DiePressureSampleValue = 0;
        private int _DiePressureSampleQty = 100;
        private bool _readStateMsgs = false;
        private double _dLastulConv = 0.0;
        private double _dQHA = -1;
        private double _dQHH = -1;
        private double _dQHF = -1;
        private string _sHomingMessage = "";
        private bool _bHasAnalogInputs = Settings.Default.HasAnalogInputs;
        private LaserData _leftLaser = null;
        private LaserData _rightLaser = null;
        private double _TC = 0;

        private BackgroundWorker _bgwIRReconnect = new BackgroundWorker();

        private int IRReconnectAttempts = 0;

        #endregion

        #region Arrays
        private double[] _dMem = new double[41];
        public double[] Outputs { get; } = new double[50];
        public double[] Inputs { get; } = new double[50];
        public double[] Analogs { get; } = new double[17];
        public double[] Memory { get; set; } = new double[41];
        //public double[] Memory { get; private set; } = new double[36]; // ---  gMem[60] on Galil
        public double[] Devices { get; private set; } = new double[10]; // ---  devices[10] on Galil
        public double[] Homed { get; private set; } = new double[8]; // ---  homed[8] on Galil
        public double[] LimitFlag { get; private set; } = new double[8]; // ---  lmtflg[8] on Galil
        public double[] PosErr { get; private set; } = new double[8]; // ---  PosErrs[8] on Galil
        public bool[] Threads { get; private set; } = new bool[16] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };

        private double[] _dRIO_IO_Buffer = new double[2];
        public bool[] RIO_Inputs { get; private set; } = new bool[16];
        public bool[] RIO_Outputs { get; private set; } = new bool[16];
        public bool AutoMode { get; set; } = false;

        #endregion

        #region Public Properties

        public bool LiftUpState = false;
        public bool LiftDownState = false;
        public bool SubstrateState = false;
        public bool HomedState = false;

        public MachineSettingsII MS { get; private set; }
        public MachineStorage Storage { get; private set; }
        private Recipe CurrentRecipe { get; set; } = null;
        public bool ValveCmdReady(int vlvID)
        {
            return (((vlvID == 0) && ValveACmdReady) || (MS.DualPumpInstalled && ((vlvID == 1) && ValveBCmdReady)));
        }
        public bool ValveACmdReady => (MS.HasRaspberryPiRotary || IsHomed) && !PumpA_Moving && !PrimeRunning && !RunningRecipe && !Threads[6] && ValveAStatus != 1 && ValveAStatus !=6;
        public bool ValveBCmdReady => MS.DualPumpInstalled && (MS.HasRaspberryPiRotary || IsHomed) && !PumpB_Moving && !PrimeRunning && !RunningRecipe && !Threads[6] && ValveBStatus != 1 && ValveBStatus != 6;
        public IIRTransmitter IRTransmitter { get; set; } = null;

        public string X_axis = NAbleEnums.AxisName(Axis.X);
        public string RZ_axis = NAbleEnums.AxisName(Axis.RZ);
        public string LZ_axis = NAbleEnums.AxisName(Axis.LZ);
        public string PumpA_axis = NAbleEnums.AxisName(Axis.PumpA);
        public string PumpB_axis = NAbleEnums.AxisName(Axis.PumpB);
        public string Loader_axis = NAbleEnums.AxisName(Axis.Loader);

        public bool CalibrationPaused { get; private set; } = false;

        #region Recipe Properties

        public string RecipeStateMsg { get; set; } = "";
        public int RecipeRunErrorCode { get; private set; } = 0;

        #endregion

        #region Vacuum Properties

        public bool VacuumDisabled { get; set; } = false;
        public double VacConv { get; set; } = 1.000;

        #endregion

        #region Properties Derived From Array Values

        public bool ZRightBrakeReleased => Outputs[6] == 1.0;
        public bool ZLeftBrakeReleased => Outputs[7] == 1.0;
        public double TheAnswer => Memory[13];

        #endregion

        #region Motion Profile Properties
        public MotionProfile ZMotionProfile { get; set; }
        public MotionProfile XMotionProfile { get; set; }
        public MotionProfile PumpMotionProfile { get; set; }

        #endregion

        #region Limit Switch Properties

        public double XFLS { get; private set; } = 1.0;
        public double XRLS { get; private set; } = 1.0;
        public double ZRFLS { get; private set; } = 1.0;
        public double ZRRLS { get; private set; } = 1.0;
        public double ZLFLS { get; private set; } = 1.0;
        public double ZLRLS { get; private set; } = 1.0;
        public double PumpFLS { get; private set; } = 1.0;
        public double PumpRLS { get; private set; } = 1.0;
        public double PumpBFLS { get; private set; } = 1.0;
        public double PumpBRLS { get; private set; } = 1.0;
        public double LoaderFLS { get; private set; } = 1.0;
        public double LoaderRLS { get; private set; } = 1.0;
        public double HFLS { get; private set; } = 1.0;
        public double HRLS { get; private set; } = 1.0;

        #endregion Limit Switch Properties

        #region Conversion Factors

        public double mmConv => 1600;
        public double umConv => 1.6;
        public double mmConvTransfer => 1574.803;
        public double uLConv
        {
            get
            {
                double dRetVal = 96;
                double dLen = 60;  //60 mm syringe
                double dVol = MS.SyringeVol * 1000;
                int nEnc = MS.PumpEncoderResolution;
                if (POHPumpDetected)
                {
                    //dLen = 65;  //65 mm piston
                    //dVol = MachineSettings.POHVol * 1000;
                    //dRetVal = (dLen / dVol) * (Double)nEnc * (MachineSettings.POHScrewPitch) / (MachineSettings.POHGearRatio);
                    //dRetVal = Math.Round(dRetVal, 1);
                    //_log.log(LogType.TRACE, Category.INFO, "Old Way: " + dRetVal.ToString("0.000"));
                    double dArea = Math.PI * System.Math.Pow((MS.POHDiam / 2), 2);
                    double d1OverArea = 1 / dArea;
                    dRetVal = d1OverArea * (nEnc / MS.POHGearRatio) * (MS.POHScrewPitch);
                    if (_dLastulConv != dRetVal)
                    {
                        _dLastulConv = dRetVal;
                        _log.log(LogType.TRACE, Category.INFO, "New uLConv: " + dRetVal.ToString("0.000"));
                    }
                }
                else
                {
                    dLen = 60;  //60 mm syringe
                    dVol = MS.SyringeVol * 1000;
                    dRetVal = (dLen / dVol) * nEnc;
                    dRetVal = Math.Round(dRetVal, 1);
                }
                //_log.log(LogType.TRACE, Category.INFO, String.Format("uLConv returning '{0}'", dRetVal));
                return dRetVal;
            }
        }
        public double MinPumpAccel => (1024 * Math.Pow(1000.0 / TM, 2)) / uLConv;  // AC resolution divided by uLConv
        public double CurrentmLVolumeUsed
        {
            get
            {
                double dRetVal = 0.0;
                dRetVal = ((PumpA_TP) / uLConv) / 1000;
                return dRetVal;
            }
        }

        public double CurrentDispenseRate
        {
            get
            {
                double dRetVal = 0.0;
                dRetVal = PumpVel;
                return dRetVal;
            }
        }

        public double stpConv => 1.8;

        #endregion Conversion Factors

        #region Connection Information

        public bool Reconnecting = false;
        private bool _bDisconnected = true;

        public bool Connected
        {
            get
            {
                return (!_bDisconnected || ((_objGalil != null) && ((_objGalil.address != "OFFLINE") && (_objGalil.address != ""))));
            }
            set { }
        }

        public string LibraryVersion
        {
            get
            {
                string sRetVal = "";
                if (_objGalil == null)
                    InitializeGalil();
                sRetVal = _objGalil.libraryVersion();
                return sRetVal;
            }
        }

        private string[] _saAddresses = null;

        public string[] Addresses(bool bRefresh)
        {
            if (bRefresh || _saAddresses == null)
            {
                if (_saAddresses != null)
                    _saAddresses = null;

                if (_objGalil != null)
                {
                    _log.log(LogType.TRACE, Category.INFO, "Looking for addresses");
                    object objTemp = _objGalil.addresses();
                    if (objTemp != null)
                    {
                        object[] oAddresses = (object[])objTemp;
                        _saAddresses = new string[oAddresses.Length];
                        int nIdx = 0;
                        foreach (object oAddr in oAddresses)
                        {
                            _saAddresses[nIdx++] = oAddr.ToString();
                            _log.log(LogType.TRACE, Category.INFO, string.Format("Found Controller : '{0}'", oAddr, "INFO"));
                        }
                    }
                    _log.log(LogType.TRACE, Category.INFO, "Finished looking for addresses");
                }
            }
            return _saAddresses;
        }

        public string ConnectionInfo { get; private set; } = "Not Connected";
        public string CoatVersion { get; private set; } = "";
        public string CodeVersion { get; private set; } = "";

        public string Address
        {
            get
            {
                string sRetVal = "OFFLINE";
                if (_objGalil != null)
                    sRetVal = _objGalil.address;
                return sRetVal;
            }
        }

        #endregion Connection Information

        #region Chuck Air/Vac Status

        public LiftPinStatus ChuckLiftPinsStatus
        {
            get
            {
                if (MS.DualZoneLiftPinsEnabled)
                {
                    LiftPinStatus pinStatus = (LiftPinStatus)Memory[17];
                    return pinStatus;
                }
                else
                {
                    LiftPinStatus pinStatus = (LiftPinStatus)Memory[8];
                    return pinStatus;
                }
            }
        }

        public LiftPinStatus PrimingLiftPinsStatus
        {
            get
            {
                if (MS.DualZoneLiftPinsEnabled)
                {
                    LiftPinStatus pinStatus = (LiftPinStatus)Memory[17];
                    return pinStatus;
                }
                else
                    return LiftPinStatus.Idle;
            }
        }

        public bool LiftPinsUpRequest
        {
            get
            {
                if (MS.LiftPinsEnabled)
                {
                    if (MS.DualZoneLiftPinsEnabled)
                        return Outputs[5] == 1.0;
                    else
                        return Outputs[28] == 1.0;
                }
                else
                    return false;
            }
        }

        public bool LiftPinsDownRequest
        {
            get
            {
                if (MS.LiftPinsEnabled)
                {
                    if (MS.DualZoneLiftPinsEnabled)
                        return Outputs[5] == 0.0;
                    else
                        return Outputs[29] == 1.0;
                }
                else
                    return true;
            }
        }

        public bool PrimingLiftPinsUpRequest
        {
            get
            {
                if (MS.DualZoneLiftPinsEnabled)
                    return Outputs[4] == 1.0;
                else
                    return false;
            }
        }

        public bool PrimingLiftPinsDownRequest
        {
            get
            {
                if (MS.DualZoneLiftPinsEnabled)
                    return Outputs[4] == 0.0;
                else
                    return false;
            }
        }

        public bool AlignersUpRequest
        {
            get
            {
                if (MS.AlignersEnabled)
                    return Outputs[30] == 1.0;
                else
                    return false;
            }
        }

        public bool MainAirOK => _bHasAnalogInputs ? MainAirPressure >= MiniumAirPressure : !LowAirOccurred;
        public bool MainVacOK => (VacuumDisabled) || _bHasAnalogInputs ? MainVac >= MiniumVac : Inputs[42] == 0.0;
        public bool PrimingVacOK => (VacuumDisabled && PrimingVacEngaged) || _bHasAnalogInputs ? PrimingVac >= MiniumVac : Inputs[43] == 0.0 || !MS.HasPrimingPlate;
        public bool Zone1VacOK => (VacuumDisabled && Zone1VacEngaged) || _bHasAnalogInputs ? Zone1Vac >= MiniumVac : Inputs[44] == 0.0 || (MS.UsesSelectiveAirVacZones && !MS.SelectiveZone1Enabled);
        public bool Zone2VacOK => (MS.NumChuckAirVacZones < 2) || (VacuumDisabled && Zone2VacEngaged) || _bHasAnalogInputs ? Zone2Vac >= MiniumVac : Inputs[45] == 0.0 || (MS.UsesSelectiveAirVacZones && !MS.SelectiveZone2Enabled);
        public bool Zone3VacOK => (MS.NumChuckAirVacZones < 3) || (VacuumDisabled && Zone3VacEngaged) || _bHasAnalogInputs ? Zone3Vac >= MiniumVac : Inputs[46] == 0.0 || (MS.UsesSelectiveAirVacZones && !MS.SelectiveZone3Enabled);
        public bool TransferSubstratePresent => Inputs[6] == 0.0;

        public bool ReservoirFull => (MS.HasReservoirSensors && Inputs[47] == 0.0);
        public bool ReservoirEmpty => (MS.HasReservoirSensors && Inputs[48] == 1.0);
        public bool IsAtStack => IsClose(LoaderPos, MS.ConvUnloadPosition, 2);
        public bool LiftPin1IsDown
        {
            get
            {
                if (MS.LiftPinsEnabled)
                    return Inputs[9] == 0.0;
                else
                    return true;
            }
        }

        public bool LiftPin1IsUp
        {
            get
            {
                if (MS.LiftPinsEnabled)
                    return Inputs[10] == 0.0;
                else
                    return true;
            }
        }

        public bool LiftPin2IsDown
        {
            get
            {
                if (MS.LiftPinsEnabled)
                    return Inputs[11] == 0.0;
                else
                    return true;
            }
        }

        public bool LiftPin2IsUp
        {
            get
            {
                if (MS.LiftPinsEnabled)
                    return Inputs[12] == 0.0;
                else
                    return true;
            }
        }


        public bool AlignersAreDown => MS.AlignersEnabled ? !AlignersUpRequest : true;

        public bool Aligner1IsDown => MS.AlignersEnabled ? Inputs[13] == 1.0 : true;
        public bool Aligner2IsDown => MS.AlignersEnabled ? Inputs[14] == 1.0 : true;
        public bool Aligner3IsDown => MS.AlignersEnabled ? Inputs[15] == 1.0 : true;

        public double MiniumAirPressure => MS.MinAirPressure;   //PSI

        public double MainAirPressure  // PSI
        {
            get { return Analogs[1] * 50; }
            set { Analogs[1] = value / 50; }
        }

        public double MiniumVac => MS.MinVacuum;
        public double MainVac => ((1.25 - Analogs[2]) / 0.083758 * 2.04) * VacConv;
        public double PrimingVac => ((1.25 - Analogs[3]) / 0.083758 * 2.04) * VacConv;
        public bool PrimingVacEngaged => Outputs[20] == 1.0;  //Was Outputs[20] == 1.0 now Inputs[43] == 0.0 Why?
        public double Zone1Vac => ((1.25 - Analogs[4]) / 0.083758 * 2.04) * VacConv;
        public bool Zone1VacEngaged => Outputs[22] == 1.0; //Was Outputs[22] == 1.0 now Inputs[44] == 0.0 Why?
        public double Zone2Vac => ((1.25 - Analogs[5]) / 0.083758 * 2.04) * VacConv;
        public bool Zone2VacEngaged => Outputs[23] == 1.0; //Was Outputs[23] == 1.0 now Inputs[45] == 0.0 Why?
        public double Zone3Vac => ((1.25 - Analogs[6]) / 0.083758 * 2.04) * VacConv;
        public bool Zone3VacEngaged => Outputs[24] == 1.0; //Was Outputs[24] == 1.0 now Inputs[46] == 0.0 Why?

        public bool GT2sExtended => Outputs[2] == 1.0;
        public void ExtendGT2s() => SetDigitalOut(2, true);
        public void RetractGT2s() => SetDigitalOut(2, false);
        public void ToggleGT2s() => ToggleDigitalOut(2);

        public bool ChuckVacOK
        {
            get
            {
                bool bRetVal = true;
                bRetVal &= Zone1VacEngaged || Zone2VacEngaged || Zone3VacEngaged;
                bRetVal &= !Zone1VacEngaged || (Zone1VacEngaged && Zone1VacOK);
                bRetVal &= !Zone2VacEngaged || (Zone2VacEngaged && Zone2VacOK);
                bRetVal &= !Zone3VacEngaged || (Zone3VacEngaged && Zone3VacOK);
                return bRetVal;
            }
        }

        public bool ChuckVacEngaged
        {
            get
            {
                bool bRetVal = true;

                if (MS.UsesSelectiveAirVacZones)
                {
                    bRetVal &= !MS.SelectiveZone1Enabled || (MS.SelectiveZone1Enabled && Zone1VacEngaged);
                    bRetVal &= !MS.SelectiveZone2Enabled || (MS.SelectiveZone2Enabled && Zone2VacEngaged);
                    bRetVal &= !MS.SelectiveZone3Enabled || (MS.SelectiveZone3Enabled && Zone3VacEngaged);
                }
                else
                {
                    bRetVal = Zone1VacEngaged || Zone2VacEngaged || Zone3VacEngaged;
                }

                return bRetVal;
            }
        }

        public int TC => (int)_TC;

        #endregion Chuck Air/Vac Status

        #region Axis Status Properties

        #region Position Error Flags

        public bool XAxisPositionErr => PosErr[0] == 1.0;
        public bool RightZAxisPositionErr => PosErr[1] == 1.0;
        public bool LeftZAxisPositionErr => PosErr[2] == 1.0;
        public bool PumpAPositionErr => PosErr[3] == 1.0;
        public bool RotaryValveAPositionErr => SyringePumpDetected && PosErr[4] == 1.0;
        public bool PumpBPositionErr => MS.DualPumpInstalled && PosErr[5] == 1.0;
        public bool RotaryValveBPositionErr => MS.DualPumpInstalled && SyringePumpBDetected && PosErr[6] == 1.0;
        public bool LoaderPositionErr => MS.HasLoader && PosErr[7] == 1.0;

        #endregion

        #region Homed Flags

        public bool XAxisHomed => Homed[0] == 1.0;
        public bool RightZAxisHomed => Homed[1] == 1.0;
        public bool LeftZAxisHomed => Homed[2] == 1.0;
        public bool PumpAHomed => Homed[3] == 1.0;
        public bool RotaryValveAHomed => SyringePumpDetected && Homed[4] == 1.0;
        public bool PumpBHomed => MS.DualPumpInstalled && Homed[5] == 1.0;
        public bool RotaryValveBHomed => MS.DualPumpInstalled && SyringePumpBDetected && Homed[6] == 1.0;
        public bool LoaderHomed => MS.HasLoader && Homed[7] == 1.0;

        #endregion

        #region Limit Flags

        public bool XAxisLimitDetected => LimitFlag[0] == 1.0;
        public bool RightZAxisLimitDetected => LimitFlag[1] == 1.0;
        public bool LeftZAxisLimitDetected => LimitFlag[2] == 1.0;
        public bool PumpALimitDetected => LimitFlag[3] == 1.0;
        public bool RotaryValveALimitDetected => SyringePumpDetected && LimitFlag[4] == 1.0;
        public bool PumpBLimitDetected => MS.DualPumpInstalled && LimitFlag[5] == 1.0;
        public bool RotaryValveBLimitDetected => MS.DualPumpInstalled && SyringePumpBDetected && LimitFlag[6] == 1.0;
        public bool LoaderLimitDetected => MS.HasLoader && LimitFlag[7] == 1.0;

        #endregion

        #region Axis Position Properties

        public double XPos => X_TP / mmConv;
        public double ZRPos => RZ_TP / mmConv;
        public double ZLPos => LZ_TP / mmConv;
        public double PumpVolLoc => PumpA_TP / uLConv;
        public double PumpBVolLoc => Loader_TP / uLConv;
        public double LoaderPos => TPH / mmConvTransfer;

        #endregion

        #region Axis Velocity Properties

        public double XVel => X_TV / mmConv;
        public double ZRVel => RZ_TV / mmConv;
        public double ZLVel => LZ_TV / mmConv;
        public double PumpVel => PumpA_TV / uLConv;
        public double PumpBVel => Loader_TV / uLConv;

        #endregion

        #region Hall Properties

        public int HallsH => (int)_dQHH;
        public int HallsF => (int)_dQHF;
        public int HallsA => (int)_dQHA;

        #endregion

        #region TV Properties

        public double X_TV { get; private set; } = 0;
        public double RZ_TV { get; private set; } = 0;
        public double LZ_TV { get; private set; } = 0;
        public double PumpA_TV { get; private set; } = 0;
        public double PumpB_TV { get; private set; } = 0;
        public double Loader_TV { get; private set; } = 0;
        public double TVG { get; private set; } = 0;
        public double TVH { get; private set; } = 0;

        #endregion

        #region Tell Position [TP] Properties

        public double X_TP { get; private set; } = -999.99;
        public double RZ_TP { get; private set; } = -999.99;
        public double LZ_TP { get; private set; } = -999.99;
        public double PumpA_TP { get; private set; } = -999.99;
        public double PumpB_TP { get; private set; } = -999.99;
        public double Loader_TP { get; private set; } = -999.99;
        public double TPG { get; private set; } = -999.99;
        public double TPH { get; private set; } = -999.99;

        #endregion

        #region TD Properties

        public double X_TD { get; private set; } = -999.99;
        public double RZ_TD { get; private set; } = -999.99;
        public double LZ_TD { get; private set; } = -999.99;
        public double PumpA_TD { get; private set; } = -999.99;
        public double PumpB_TD { get; private set; } = -999.99;
        public double Loader_TD { get; private set; } = -999.99;
        public double TDG { get; private set; } = -999.99;
        public double TDH { get; private set; } = -999.99;

        #endregion

        #region Motor Off Properties

        public bool X_MotorOff { get; private set; } = true;
        public bool RZ_MotorOff { get; private set; } = true;
        public bool LZ_MotorOff { get; private set; } = true;
        public bool PumpA_MotorOff { get; private set; } = true;
        public bool PumpB_MotorOff { get; private set; } = true;
        public bool Loader_MotorOff { get; private set; } = true;
        public bool GMotorOff { get; private set; } = true;
        public bool HMotorOff { get; private set; } = true;

        #endregion

        public double LeftGT2Val { get; private set; } = -999.99;
        public double RightGT2Val { get; private set; } = -999.99;

        public KeyenceState LeftGT2State { get; private set; } = KeyenceState.Unknown;
        public KeyenceState RightGT2State { get; private set; } = KeyenceState.Unknown;
        public int LevelAttempt { get; private set; } = 0;

        #region Motion Properties

        public bool XJogging { get { int nFlag = 0x8; return (nFlag == ((int)Memory[4] & nFlag)); } }
        public bool ZJogging { get { int nFlag = 0x10; return (nFlag == ((int)Memory[4] & nFlag)); } }

        public bool Moving
        {
            get
            {
                bool bRetVal = false;
                bRetVal = Connected && (XMoving || ZMoving || PumpA_Moving || PumpB_Moving || Loader_Moving || GMoving || HMoving || RunningGoto || CalibratingDie || PrimeRunning || LevelingDie || RunningRecipe || ReadingKeyence);
                return bRetVal;
            }
        }

        public bool XMoving { get; private set; } = false;
        public bool ZMoving => ZRightMoving || ZLeftMoving;
        public bool LoaderMoving => Loader_Moving;
        public bool ZRightMoving { get; private set; } = false;
        public bool ZLeftMoving { get; private set; } = false;
        public bool PumpA_Moving { get; private set; } = false;
        public bool PumpB_Moving { get; private set; } = false;
        public bool Loader_Moving { get; private set; } = false;
        public bool GMoving { get; private set; } = false;
        public bool HMoving { get; private set; } = false;

        #endregion

        #region Motor Feedback Properties

        public double PosErrX { get; private set; } = 0.0;
        public double TorqueX { get; private set; } = 0.0;
        public double PosErrRZ { get; private set; } = 0.0;
        public double TorqueRZ { get; private set; } = 0.0;
        public double PosErrLZ { get; private set; } = 0.0;
        public double TorqueLZ { get; private set; } = 0.0;
        public double PosErrPumpA { get; private set; } = 0.0;
        public double PosErrPumpB { get; private set; } = 0.0;
        public double PosErrLoader { get; private set; } = 0.0;


        public double TorquePump { get; private set; } = 0.0;
        public double TorquePumpB { get; private set; } = 0.0;
        public double TorqueLoader { get; private set; } = 0.0;

        private double _dLZMaxPosError = 0;
        public double LZMaxPosErr => _dLZMaxPosError / mmConv;

        private double _dRZMaxPosError = 0;
        public double RZMaxPosErr => _dRZMaxPosError / mmConv;

        private double _dXMaxPosError = 0;
        public double XMaxPosErr => _dXMaxPosError / mmConv;

        public double PumpMaxPosErr { get; private set; } = 0;
        public double PumpBMaxPosErr { get; private set; } = 0;

        private double _dFMaxPosError = 0;
        public double LoaderMaxPosErr => _dFMaxPosError / mmConvTransfer;


        public bool AMPErrorOccurred => (0x400 == ((int)Memory[0] & 0x400));
        public bool XAxisPosError => (0x100000 == ((int)Memory[0] & 0x100000));
        public bool RZAxisPosError => (0x200000 == ((int)Memory[0] & 0x200000));
        public bool LZAxisPosError => (0x400000 == ((int)Memory[0] & 0x400000));
        public bool PumpAxisPosError => (0x800000 == ((int)Memory[0] & 0x800000));
        public bool PumpBAxisPosError => (0x1000000 == ((int)Memory[0] & 0x1000000));
        public bool XferAxisPosError => (0x2000000 == ((int)Memory[0] & 0x2000000));

        #endregion Motor Feedback Properties

        #endregion Axis Status

        #region Status Properties

        public DateTime LastRecordReceived => _dtLastRecordReceived;
        public bool IsDemo { get; set; } = false;

        public double TM { get; private set; } = 500; // Galil Time Tick
        public double TA3 { get; private set; } = 0.0;
        public int AxisCount { get; private set; } = 0;
        public bool RestartMessaging { get; set; } = false;
        public int SampleTime { get; set; } = -1;

        public string HomingMessage
        {
            get { return _sHomingMessage; }
            set { _sHomingMessage = value; }
        }

        public double RotValvePlsPerDeg = 17.7778;
        public bool BuzzerOn => Outputs[1] == 1.0;
        public bool KeyenceOn => Outputs[2] == 1.0;
        public bool VentValveOpen => Outputs[35] == 1.0;
        public bool MainAirValveOpen => Outputs[8] == 1.0;


        public bool AllMotorsOn
        {
            get
            {
                bool bRetVal = true;
#if !DESKTOP
                if (!IsDemo)
                {
                    bRetVal &= !X_MotorOff;
                    bRetVal &= !RZ_MotorOff;
                    bRetVal &= !LZ_MotorOff;
                    bRetVal &= !PumpA_MotorOff;
                    if (MS.DualPumpInstalled)
                    {
                        bRetVal &= !PumpB_MotorOff;
                    }
                    if (MS.HasLoader)
                        bRetVal &= !Loader_MotorOff;
                }
#endif
                return bRetVal;
            }
        }

        public bool CarriageInSafeArea => (XPos < MS.ChuckStart) && !XAxisError && !XAxisHomingError && !XAxisCommutationError && IsHomed;
        public bool ELOLatched { get; private set; } = false;
        public bool IsSetup { set; get; } = false;
        public bool IsHoming => Memory[6] == 1.0 || Memory[6] == 4.0;
        public bool ReadingKeyence { get; private set; } = false;
        public bool CalibratingDie { get; private set; } = false;
        public bool LevelingDie { get; private set; } = false;
        public bool RunningRecipe { get; private set; } = false;
        public bool RunningGoto => ((int)Memory[4] & 0x200) == 0x200;
        public bool IsResetting => _bResetting;
        public bool IsConnecting => _bConnecting;
        public bool CommutationFailure => ((int)Memory[0] & 0x80) == 0x80;
#if DESKTOP
        public bool IsHomed => HomedState;
        public bool SubstratePresent => SubstrateState;
        public bool LiftIsDown => !LiftDownState;
        public bool LiftIsUp => LiftUpState;
        public bool LiftPinsAreDown => LiftIsDown && !LiftIsUp;
        public bool AirPressureFaultDuringInit => false;
        public bool SafetyGuardsActive => false;
        public bool LowAirOccurred => false;

#else
        public bool IsHomed => IsDemo || Memory[6] == 2.0;
        public bool SubstratePresent => Inputs[1] == 0.0;
        public bool LiftIsDown => MS.HasLiftAndCenter && Inputs[6] == 0.0;
        public bool LiftIsUp => MS.HasLiftAndCenter && Inputs[7] == 0.0;
        public bool LiftPinsAreDown => (MS.LiftPinsEnabled && LiftPin1IsDown && LiftPin2IsDown) || (MS.HasLiftAndCenter && LiftIsDown);
        public bool AirPressureFaultDuringInit => _bHasAnalogInputs ? Memory[18] == 1 : Inputs[41] == 1.0;
        public bool SafetyGuardsActive => Connected && Inputs[5] != 0.0;
        public bool LowAirOccurred => _bHasAnalogInputs ? (0x2 == ((int)Memory[0] & 0x2)) : Inputs[41] != 0.0;
#endif
        public bool ELOWasTripped => (Inputs[8] != 1.0 || TA3 != 0.0 || (0x400 == ((int)Memory[0] & 0x400)));

        public bool ELOPressed
        {
            get
            {
                bool bRetVal = false;
                if (Connected)
                {
                    bRetVal = (Inputs[8] != 1.0);
                }
                return bRetVal;
            }
        }

        public bool Started
        {
            get
            {
                bool bRetVal = false;
                if (IsDemo)
                    bRetVal = true;
                else
                    if (Connected) { bRetVal = 1 == _objGalil.commandValue("MGstarted"); }
                return bRetVal;
            }
        }

        public bool FirstRecordReceived { get; private set; } = false;
        public bool NeedSlowPoll { get; set; } = false;
        public bool DataRecordRunning { get; private set; } = false;

        #region Homing Status Properties

        public int HomingStatus => (int)Memory[6];
        public int XHomingStatus { get; private set; } = 0;
        public bool XIsHomed => XHomingStatus == 2;
        public int ZRHomingStatus { get; private set; } = 0;
        public int ZLHomingStatus { get; private set; } = 0;
        public int PumpHomingStatus { get; private set; } = 0;
        public int ValveHomingStatus { get; private set; } = 0;
        public int PumpBHomingStatus { get; private set; } = 0;
        public int ValveBHomingStatus { get; private set; } = 0;
        public int LoaderHomingStatus { get; private set; } = 0;

        public bool PumpCommutating => (0x10 == (PumpHomingStatus & 0x10));
        public bool PumpHoming => (0x1 == (PumpHomingStatus & 0x1));
        public bool ValveCommutating => (0x800 == (PumpHomingStatus & 0x800));
        public bool ValveHoming => (0x100 == (PumpHomingStatus & 0x100));
        public bool ValveHomed => (0x200 == (PumpHomingStatus & 0x200));
        public bool ValveHomingError => (0x400 == (PumpHomingStatus & 0x400));

        public bool PumpBCommutating => (0x10 == (PumpBHomingStatus & 0x10));
        public bool PumpBHoming => (0x1 == (PumpBHomingStatus & 0x1));
        public bool ValveBCommutating => (0x800 == (PumpBHomingStatus & 0x800));
        public bool ValveBHoming => (0x100 == (PumpBHomingStatus & 0x100));
        public bool ValveBHomed => (0x200 == (PumpBHomingStatus & 0x200));
        public bool ValveBHomingError => (0x400 == (PumpBHomingStatus & 0x400));

        #endregion

#endregion

        #region Installed Devices Properties

        public bool LoaderInstalled => Devices[1] == 1;
        public bool KeyenceLasersInstalled => Devices[2] == 1;
        public bool AlignersInstalled => Devices[3] == 1;
        public bool LiftPinsInstalled => Devices[4] == 1;
        public bool AirKnifeInstalled => Devices[5] == 1;
        public bool IRLampInstalled => Devices[6] == 1;
        public bool DualPumpInstalled => Devices[7] == 1;
        public bool DualZoneLiftPins => Devices[8] == 1;

        #endregion

        #region Error Properties

        private Hashtable _htErrorCodes { get; set; } = null;

        public Hashtable ErrorCodes => _htErrorCodes;

        public bool ErrorOccurred => Memory[0] != 0.0;
        public string LastError { get; private set; }

        #region Command Error Properties

        public int CommandErrorLineNum => (int)Memory[10];
        public string LastCommandError { get; private set; } = "";
        public string LastCommandErrorLine { get; private set; } = "";

        #endregion

        #region Axis Error Properties

        public bool XAxisError { get { int nMask = 0x4; return (nMask == ((int)Memory[0] & nMask)); } }
        public bool RZAxisError { get { int nMask = 0x8; return (nMask == ((int)Memory[0] & nMask)); } }
        public bool LZAxisError { get { int nMask = 0x10; return (nMask == ((int)Memory[0] & nMask)); } }
        public bool PumpAxisError { get { int nMask = 0x20; return (nMask == ((int)Memory[0] & nMask)); } }
        public bool ValveAxisError { get { int nMask = 0x40; return (nMask == ((int)Memory[0] & nMask)); } }
        public bool PumpBAxisError { get { int nMask = 0x80; return (nMask == ((int)Memory[0] & nMask)); } }
        public bool ValveBAxisError { get { int nMask = 0x10000; return (nMask == ((int)Memory[0] & nMask)); } }
        public bool LoaderAxisError { get { int nMask = 0x40000; return (nMask == ((int)Memory[0] & nMask)); } }

        #endregion

        #region Homing Error Properties

        public bool XAxisHomingError => XHomingStatus == 3;
        public bool ZAxisHomingError => ZRAxisHomingError || ZLAxisHomingError;
        public bool ZRAxisHomingError => ZRHomingStatus == 3;
        public bool ZLAxisHomingError => ZLHomingStatus == 3;
        public bool PumpAxisHomingError => (0x4 == (PumpHomingStatus & 0x4));
        public bool PumpBAxisHomingError => PumpBHomingStatus == 3;
        public bool LoaderAxisHomingError => LoaderHomingStatus == 3;

        #endregion

        #endregion

        #region Commutation Properties

        public int CommutatingAxis => (int)Memory[12];
        public bool XAxisCommutationError => XHomingStatus == 4;
        public bool ZAxisCommutationError => ZRAxisCommutationError || ZLAxisCommutationError;
        public bool ZRAxisCommutationError => ZRHomingStatus == 4;
        public bool ZLAxisCommutationError => ZLHomingStatus == 4;
        public bool PumpAxisCommutationError => (0x8 == (PumpHomingStatus & 0x8));
        public bool PumpBAxisCommutationError => PumpBHomingStatus == 4;
        public bool LoaderAxisCommutationError => LoaderHomingStatus == 4;

        #endregion

        #region Temperature Properties

        public string ChuckTemp { get; set; } = "0.0";
        public string DieTemp { get; set; } = "0.0";
        public string ResvTemp { get; set; } = "0.0";

        #endregion

        #region Die Pressure Properties

        public double DiePresurePSIperVolt
        {
            get
            {
                double dRetVal;

                double dResistor = 250;  // 250Ω resistor
                double dLowCurrentVal = .004; // A  (4mA)
                double dHighCurrentVal = .020; // A  (20mA)
                double dLowPSIVal = -1.8;
                double dHighPSIVal = 36.27;

                dRetVal = (dHighPSIVal - dLowPSIVal) / ((dHighCurrentVal * dResistor) - (dLowCurrentVal * dResistor));

                return dRetVal;
            }
        }
        public double DiePressurePSI
        {
            get
            {
                double dRetVal = double.NaN;
                if (MS.HasDiePressureTransducer)
                {
                    // IMF PI2896
                    // measure range: -1.8  -  36.27 PSI
                    // should be 1-5 V range for above range. (derived from 4-20 mA with 250Ω resistor)
                    // 9.5175 PSI/V	
                    double newVal = Analogs[8] + MS.DiePressureInputVoltageAdjust;
                    if (newVal >= .9)
                        dRetVal = ((newVal - 1) * DiePresurePSIperVolt) + MS.DiePressureOffset;  // NOTE:  this could be more flexible/configurable, but its good enough for 9080. 
                    else
                        dRetVal = double.NaN;
                }
                return dRetVal;
            }
        }
        public double DiePressurePSISmoothed { get; private set; }
        public double DiePressureVDCSmoothed { get; private set; }

        #endregion

        #region Valve Properties

        public bool DieVentOpen => Outputs[33] == 1.0;
        public bool RechargeVlvOpen => Outputs[32] == 1.0;    // POH Charge Vlv
        public bool HeadVentVlvOpen => Outputs[35] == 1.0;    // POH Vent Vlv
        public bool DispenseVlvOpen => Outputs[34] == 1.0;    // POH Dispense Vlv
        public int ValveStepperPosition => (int)Memory[37];   // Step Position Of Rotary Valve [Pico Upgrade]
        public double ValveAngle => Math.Abs(ValveStepperPosition / RotValvePlsPerDeg);
        public int ValveAStatus => (int)Memory[5];
        public int ValveBStatus => (int)Memory[15];
        public string ValveStatusName
        {
            get
            {
                string sRetVal = "Unknown";
                switch (ValveAStatus)
                {
                    case 0: { sRetVal = "Vent"; } break;
                    case 1: { sRetVal = "Homing"; } break;
                    case 2: { sRetVal = "Recharge"; } break;
                    case 3: { sRetVal = "Off"; } break;
                    case 4: { sRetVal = "Dispense"; } break;
                    case 5: { sRetVal = "Failure"; } break;
                    case 6: { sRetVal = "Moving..."; } break;
                    case 7: { sRetVal = "Unknown"; } break;
                    default: { sRetVal = "Unknown"; } break;
                }
                return sRetVal;
            }
        }
        public string ValveBStatusName
        {
            get
            {
                string sRetVal = "Unknown";
                switch (ValveBStatus)
                {
                    case 0: { sRetVal = "Vent"; } break;
                    case 1: { sRetVal = "Homing"; } break;
                    case 2: { sRetVal = "Recharge"; } break;
                    case 3: { sRetVal = "Off"; } break;
                    case 4: { sRetVal = "Dispense"; } break;
                    case 5: { sRetVal = "Failure"; } break;
                    case 6: { sRetVal = "Executing..."; } break;
                    case 7: { sRetVal = "Unknown"; } break;
                    default: { sRetVal = "Unknown"; } break;
                }
                return sRetVal;
            }
        }

        #endregion

        #region Pump Properties

        public double PumpAFlowRate { get; set; } = 0;
        public double PumpBFlowRate { get; set; } = 0;
        public bool AbortingPumpOperation => Memory[2] == 1.0;

        public bool PrimeRunning => (((int)Memory[4]) & 0x7) != 0;
        public bool PumpPrimeRunning => (((int)Memory[4]) & 0x7) == 1;
        public bool HeadPrimeRunning => (((int)Memory[4]) & 0x7) == 2;
        public bool HeadPurgeRunning => (((int)Memory[4]) & 0x7) == 3;
        public bool PumpPurgeRunning => (((int)Memory[4]) & 0x7) == 5;
        public bool StoppingPrime => (((int)Memory[4]) & 0x7) == 4;
        
        public int HeadPurgingVolume { get; set; } = 0;
        public int PrimingCounts { get; set; } = 0;
        public int PrimingCounter => (int)Memory[9];

#if DEMOMODE
        bool forceSyringe = false;

        public bool SyringePumpDetected => forceSyringe;
        public bool POHPumpDetected => !forceSyringe;

        public bool SyringePumpBDetected => forceSyringe && MS.DualPumpInstalled;
        public bool POHPumpBDetected => !forceSyringe && MS.DualPumpInstalled;
        public bool ComboPumpDetected => false;
#else
        public bool SyringePumpDetected => Connected && Inputs[3] == 0.0;
        public bool POHPumpDetected => Connected && Inputs[4] == 0.0;

        public bool SyringePumpBDetected => Connected && MS.DualPumpInstalled && (Inputs[11] == 0.0);
        public bool POHPumpBDetected => Connected && MS.DualPumpInstalled && (Inputs[12] == 0.0);
        public bool ComboPumpDetected => Connected && Inputs[4] == 0.0;
#endif


        #endregion

#endregion

#endregion Properties

        #region Helper Functions

        public bool IsClose(double dPos1, double dPos2, double dRange)
        {
            return (dPos1 <= (dPos2 + dRange)) && (dPos1 > (dPos2 - dRange));
        }

        public double MaxZAtCurrentLoc(double thisXPos)
        {
            if (thisXPos >= MS.Zone5Start)
                return MS.Zone5MaxZ;
            else if (thisXPos >= MS.Zone4Start)
                return MS.Zone4MaxZ;
            else if (thisXPos >= MS.Zone3Start)
                return MS.Zone3MaxZ;
            else if (thisXPos >= MS.Zone2Start)
                return MS.Zone2MaxZ;
            else
                return MS.Zone1MaxZ;
        }

        #endregion

        #region Events

        public delegate void OnConnectionEventHandler(object sender, EventArgs eventArgs);
        public event OnConnectionEventHandler OnConnection;
        public delegate void OnConnectionLostEventHandler(object sender, EventArgs eventArgs);
        public event OnConnectionLostEventHandler OnConnectionLost;
        public delegate void OnELODetectedEventHandler(object sender, EventArgs eventArgs);
        public event OnELODetectedEventHandler OnELODetected;
        public delegate void OnRecipeFinishedEventHandler(object sender, RecipeEventArgs eventArgs);
        public event OnRecipeFinishedEventHandler OnRecipeFinished;

        public event Action<string> CalibrationMessage;

        #endregion Events

        #region Constructors

        public GalilWrapper2(LogEntry log)
        {
            _log = log;
            _keyence = new KeyenceComm2(_log);

            IsDemo = CheckDemoMode();

            SetupErrorCodes();
            ClearInputs();
            InitializeGalil();

            _bgwIRReconnect.DoWork += IRReconnect_DoWork;
            _bgwIRReconnect.RunWorkerCompleted += IRReconnect_RunWorkerCompleted;

            GC.KeepAlive(_keyence);
            ZMotionProfile = new MotionProfile(2.56, 1000);
            XMotionProfile = new MotionProfile(2.56, 1000);
            PumpMotionProfile = new MotionProfile(2.56, 1000);
        }

        #endregion Constructors

        #region Keyence Stuff

        public bool TestKeyence(string sPort, ref KeyenceState eStateRight, ref double dValRight, ref KeyenceState eStateLeft, ref double dValLeft, int nRetries)
        {
            bool bRetVal = false;

            if (_keyence.IsOpen)
                _keyence.ClosePort();

            KeyenceComm2 keyTemp = new KeyenceComm2(_log);
            keyTemp.PortName = sPort;

            if (keyTemp.OpenPort(true))
            {
                bRetVal = keyTemp.ReadAll(ref dValRight, ref eStateRight, ref dValLeft, ref eStateLeft, nRetries);
                if (!bRetVal)
                {
                    LastError = "Could not read GT2 sensor data";
                }
                keyTemp.ClosePort();
            }
            else
            {
                LastError = "Could not open Port " + sPort;
            }

            return bRetVal;
        }

        #endregion Keyence Stuff

        #region X-Axis Operations

        public bool TurnXOn()
        {
            bool bRetVal = false;
            if (XIsHomed)
            {
                _log.log(LogType.TRACE, Category.INFO, "Enabling X Motor");
                RunCommand("SH" + X_axis);
                RunCommand("OE" + X_axis + "=1");
                RunCommand("setting[10]=setting[10]|$1");
                bRetVal = true;
            }
            return bRetVal;
        }
        public bool TurnXOff()
        {
            bool bRetVal = true;
            RunCommand("setting[10]=setting[10]&@COM[$1]");
            RunCommand("OE" + X_axis +"=0");
            RunCommand("MO" + X_axis);
            return bRetVal;
        }

        #endregion

        #region Chuck Operations

        public bool PrepareToLoadSusbtrate()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            DownloadDelays();
            RunCommand(string.Format("params[39]={0}", MS.LiftPinsDuringPrep2Load ? 1 : 0));
            RunCommand("XQ #prep2ld, 3");
            return bRetVal;
        }
        public bool LoadSubstrate()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            DownloadDelays();
            string sCmd = string.Format("XQ #load, 3");
            RunCommand(sCmd);
            return bRetVal;
        }
        public bool UnLoadSubstrate()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            DownloadDelays();
            string sCmd = string.Format("XQ #unload, 3");
            RunCommand(sCmd);
            return bRetVal;
        }
        public bool UnLoadTransferSubstrate()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            DownloadDelays();
            string sCmd = string.Format("XQ #uldnxfr, 3");
            RunCommand(sCmd);
            return bRetVal;
        }

        #endregion Chuck Operations

        #region Priming Chuck Operations

        public bool PrepareToLoadPriming()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            DownloadDelays();

            RunCommand(string.Format("params[39]={0}", MS.LiftPinsDuringPrep2Load ? 1 : 0));
            RunCommand("XQ #prmp2ld, 3");
            return bRetVal;
        }
        public bool LoadPriming()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            DownloadDelays();
            string sCmd = string.Format("XQ #prmload, 3");
            RunCommand(sCmd);
            return bRetVal;
        }
        public bool UnLoadPriming()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            DownloadDelays();
            string sCmd = string.Format("XQ #prmuld, 3");
            RunCommand(sCmd);
            return bRetVal;
        }

        #endregion Priming Chuck Operations

        #region Vacuum Zone / Air Operations

        public void SetSelectiveZones(bool Zone1, bool Zone2, bool Zone3)
        {
            int nMask = 0x00;
            if (Zone1)
                nMask |= 0x01;
            if (Zone2)
                nMask |= 0x02;
            if (Zone3)
                nMask |= 0x04;

            MS.SelectiveZones = nMask;
            MS.Save();
            RunCommand($"setting[12]={nMask}");
        }
        public bool ReleaseVacuum(Zones oZone)
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            string sCmd1 = "", sCmd2 = "";
            string sRC = "";
            switch (oZone)
            {
                case Zones.Priming:
                {
                    if (IsDemo)
                    {
                        Outputs[20] = 0;
                        Analogs[3] = 1.25;
                        return true;
                    }
                    sCmd1 = "CB20; SB21";
                    sCmd2 = "CB21";
                }
                break;

                case Zones.Zone1:
                {
                    if (IsDemo)
                    {
                        Outputs[22] = 0;
                        Analogs[4] = 1.25;
                        return true;
                    }
                    sCmd1 = "CB22; SB25";
                    sCmd2 = "CB25";
                }
                break;

                case Zones.Zone2:
                {
                    if (IsDemo)
                    {
                        Outputs[23] = 0;
                        Analogs[5] = 1.25;
                        return true;
                    }
                    sCmd1 = "CB23; SB26";
                    sCmd2 = "CB26";
                }
                break;

                case Zones.Zone3:
                {
                    if (IsDemo)
                    {
                        Outputs[24] = 0;
                        Analogs[6] = 1.25;
                        return true;
                    }
                    sCmd1 = "CB24; SB27";
                    sCmd2 = "CB27";
                }
                break;

                case Zones.AllChuck:
                {
                    if (MS.UsesSelectiveAirVacZones)
                    {
                        if (MS.SelectiveZone1Enabled)
                        {
                            if (IsDemo)
                            {
                                Outputs[22] = 0;
                                Analogs[4] = 1.25;
                            }
                            sCmd1 += "CB22;SB25;";
                            sCmd2 += "CB25;";
                        }
                        if (MS.SelectiveZone2Enabled)
                        {
                            if (IsDemo)
                            {
                                Outputs[23] = 0;
                                Analogs[5] = 1.25;
                            }
                            sCmd1 += "CB23;SB26;";
                            sCmd2 += "CB26;";
                        }
                        if (MS.SelectiveZone3Enabled)
                        {
                            if (IsDemo)
                            {
                                Outputs[24] = 0;
                                Analogs[6] = 1.25;
                            }
                            sCmd1 += "CB24; SB27;";
                            sCmd2 += "CB27;";
                        }
                    }
                    else if (MS.NumChuckAirVacZones > 2)
                    {
                        if (IsDemo)
                        {
                            Outputs[22] = 0;
                            Outputs[23] = 0;
                            Outputs[24] = 0;
                            Analogs[4] = 1.25;
                            Analogs[5] = 1.25;
                            Analogs[6] = 1.25;
                        }
                        sCmd1 = "CB24; SB27; CB23; SB26; CB22; SB25;";
                        sCmd2 = "CB27;CB26;CB25;";
                    }
                    else if (MS.NumChuckAirVacZones > 1)
                    {
                        if (IsDemo)
                        {
                            Outputs[22] = 0;
                            Outputs[23] = 0;
                            Analogs[4] = 1.25;
                            Analogs[5] = 1.25;
                        }
                        sCmd1 = "CB23; SB26; CB22; SB25;";
                        sCmd2 = "CB26;CB25;";
                    }
                    else
                    {
                        if (IsDemo)
                        {
                            Outputs[22] = 0;
                            Analogs[4] = 1.25;
                        }
                        sCmd1 = "CB22; SB25";
                        sCmd2 = "CB25";
                    }
                }
                break;
            }
            if (IsDemo)
                return true;
            sRC = RunCommand(sCmd1);
            Thread.Sleep(MS.AirPuffDelay);
            sRC += RunCommand(sCmd2);
            return bRetVal;
        }
        public bool EngageVacuum(Zones oZone)
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            string sCmd = "";
            switch (oZone)
            {
                case Zones.Priming:
                {
                    if (IsDemo)
                    {
                        Outputs[20] = 1;
                        Analogs[3] = .08;
                        return true;
                    }
                    sCmd = "CB21;SB20";
                }
                break;

                case Zones.Zone1:
                {
                    if (IsDemo)
                    {
                        Outputs[22] = 1;
                        Analogs[4] = .08;
                        return true;
                    }
                    sCmd = "CB25;SB22";
                }
                break;

                case Zones.Zone2:
                {
                    if (IsDemo)
                    {
                        Outputs[23] = 1;
                        Analogs[5] = .08;
                        return true;
                    }
                    sCmd = "CB26;SB23";
                }
                break;

                case Zones.Zone3:
                {
                    if (IsDemo)
                    {
                        Outputs[24] = 1;
                        Analogs[6] = .08;
                        return true;
                    }
                    sCmd = "CB27;SB24";
                }
                break;

                case Zones.AllChuck:
                {
                    if (MS.UsesSelectiveAirVacZones)
                    {
                        if (MS.SelectiveZone1Enabled)
                        {
                            Outputs[22] = 1;
                            Analogs[4] = .08;
                            sCmd += "CB25;SB22;";
                        }
                        if (MS.SelectiveZone2Enabled)
                        {
                            Outputs[23] = 1;
                            Analogs[5] = .08;
                            sCmd += "CB26;SB23;";
                        }
                        if (MS.SelectiveZone3Enabled)
                        {
                            Outputs[24] = 1;
                            Analogs[6] = .08;
                            sCmd += "CB27;SB24;";
                        }
                    }
                    else if (MS.NumChuckAirVacZones > 2)
                    {
                        if (IsDemo)
                        {
                            Outputs[22] = 1;
                            Outputs[23] = 1;
                            Outputs[24] = 1;
                            Analogs[4] = .08;
                            Analogs[5] = .08;
                            Analogs[6] = .08;
                        }
                        sCmd = "CB27;CB26;CB25;SB24;SB23;SB22;";
                    }
                    else if (MS.NumChuckAirVacZones > 1)
                    {
                        if (IsDemo)
                        {
                            Outputs[22] = 1;
                            Outputs[23] = 1;
                            Analogs[4] = .08;
                            Analogs[5] = .08;
                        }
                        sCmd = "CB26;CB25;SB23;SB22;";
                    }
                    else
                    {
                        if (IsDemo)
                        {
                            Outputs[22] = 1;
                            Analogs[4] = .08;
                        }
                        sCmd = "CB25;SB22;";
                    }
                }
                break;
            }
            if (IsDemo)
                return true;
            RunCommand(sCmd);
            return bRetVal;
        }
        internal bool SetAirState(bool bEnableAir)
        {
            bool bRetVal = false;
            try
            {
                if (IsDemo)
                {
                    if (bEnableAir)
                    {
                        Outputs[8] = 1;
                        Analogs[1] = 1.6;
                    }
                    else
                    {
                        Outputs[8] = 0;
                        Analogs[1] = 0.02;
                    }
                    return true;
                }

                if (!Connected)
                {
                    return false;
                }

                string sCmd = MainAirValveOpen ? "params[1]=0" : "params[1]=1";
                RunCommand(sCmd);
                sCmd = string.Format("XQ #toglair, 2");
                RunCommand(sCmd);
                bRetVal = true;
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception while updating Delays: " + ex.Message, "ERROR");
            }
            return bRetVal;
        }

        #endregion

        #region Die Pressure Operations

        public bool SetDiePressureVoltageAdjust(double curVal, out double dAdjustment)
        {
            bool bRetVal = false;
            dAdjustment = double.NaN;
            if (DiePressureVDCSmoothed != double.NaN)
            {
                dAdjustment = (curVal / DiePresurePSIperVolt) - DiePressureVDCSmoothed + 1;
                MS.DiePressureInputVoltageAdjust = dAdjustment;
                MS.Save();
                bRetVal = true;
            }
            return bRetVal;
        }
        private void AddDiePressureForSmoothing(double dPressure, double dVoltage)
        {
            _DiePressure[_DiePressureSampleValue] = dPressure;
            _DiePressureVDC[_DiePressureSampleValue] = dVoltage;
            _DiePressureSampleValue++;
            if (_DiePressureSampleValue == _DiePressureSampleQty)
                _DiePressureSampleValue = 0;
            double dTemp;
            int i;
            for (i = 0, dTemp = 0; i < _DiePressureSampleQty; dTemp += _DiePressure[i++]) ;    // sum the Pressure samples into dTemp
            DiePressurePSISmoothed = dTemp / _DiePressureSampleQty;                            // Set the smoothed pressure value
            for (i = 0, dTemp = 0; i < _DiePressureSampleQty; dTemp += _DiePressureVDC[i++]) ; // sum the Voltage samples into dTemp
            DiePressureVDCSmoothed = dTemp / _DiePressureSampleQty;                            // Set the smoothed analog input
        }

        #endregion

        #region Aligner Operations

        public bool LowerAligners()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            string sCmd = string.Format("CB30");
            if (AlignersUpRequest)
                RunCommand(sCmd);
            return bRetVal;
        }
        public bool RaiseAligners()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            string sCmd = string.Format("SB30");
            if (!AlignersUpRequest)
                RunCommand(sCmd);
            return bRetVal;
        }

        #endregion

        #region Lift Pin Operations

        public bool RaiseLiftPins()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            DownloadDelays();
            string sCmd = string.Format("XQ #lpup, 3");
            if (!LiftPinsUpRequest)
                RunCommand(sCmd);
            return bRetVal;
        }
        public bool LowerLiftPins()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            DownloadDelays();
            string sCmd = string.Format("XQ #lpdown, 3");
            if (!LiftPinsDownRequest)
                RunCommand(sCmd);
            return bRetVal;
        }

        #endregion

        #region Priming Lift Pin Operations

        public bool RaisePrimingLiftPins()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            DownloadDelays();
            string sCmd = string.Format("XQ #prmlpup, 3");
            if (!PrimingLiftPinsUpRequest)
                RunCommand(sCmd);
            return bRetVal;
        }
        public bool LowerPrimingLiftPins()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            DownloadDelays();
            string sCmd = string.Format("XQ #prmlpdn, 3");
            if (!PrimingLiftPinsDownRequest)
                RunCommand(sCmd);
            return bRetVal;
        }

        #endregion Priming Lift Pin Operations

        #region Pump Operations

        private bool _runningRecirc = false;

        public bool ChangePumpSetup(int selectedPump, double pumpARatio, double pumpBRatio)
        {
            string cmd;

            if (!Connected)
            {
                return false;
            }

            if (MS.DualPumpInstalled)
            {
                cmd = $"params[110]={selectedPump}";
                RunCommand(cmd);
                Thread.Sleep(5);

                cmd = $"params[111]={pumpARatio / 100.000}";
                RunCommand(cmd);
                Thread.Sleep(5);

                cmd = $"params[112]={pumpBRatio / 100.000}";
                RunCommand(cmd);
            }
            else
            {
                cmd = $"params[110]=0";
                RunCommand(cmd);
                Thread.Sleep(5);

                cmd = $"params[111]=100.000";
                RunCommand(cmd);
                Thread.Sleep(5);

                cmd = $"params[112]=0.000";
                RunCommand(cmd);
            }

            return true;
        }

        public bool RunRecirc()
        {
            bool bRetVal = true;

            if (_runningRecirc || PrimeRunning)
            {
                return bRetVal;
            }

            _runningRecirc = true;
            int counts = Storage.RecirculationCount;
            double speed = Storage.SyringePrimingRate;
            double rechargeRate = Storage.SyringePrimingRechargeRate;
            ChangePumpSetup(Storage.SelectedPump, Storage.PumpARatio, 100 - Storage.PumpARatio);
            bRetVal = StartPrime(PumpOp.Pump, counts, speed, rechargeRate, uLConv, 4);
            _runningRecirc = false;

            return bRetVal;
        }

        public bool StartPrime(PumpOp isHead, int counts, double speed, double rechargeRate, 
            double uLConv, double accelMult)
        {
            if (!Connected)
            {
                return false;
            }

            ChangePumpSetup(Storage.SelectedPump, Storage.PumpARatio, 100 - Storage.PumpARatio);

            PrimingCounts = counts;
            int numPulses = GetMaxNumPulses();
            string cmd = isHead == PumpOp.Head ? "hedprim" : "pmpprim";

            RunCommand($"params[15]={numPulses}");
            RunCommand($"params[19]={counts}");
            RunCommand($"params[21]={speed}");
            RunCommand($"params[22]={rechargeRate}");

            RunCommand($"setting[3]={uLConv}");
            RunCommand($"setting[8]={accelMult}");

            RunCommand($"delays[6]={MS.PrimePreDelay}");
            RunCommand($"delays[7]={MS.PrimePostDelay}");

            RunCommand($"XQ #{cmd}, 5");

            return true;
        }

        public bool StartPurge(PumpOp pumpOp, double volume, double speed, double rechargeRate, double uLConv, double accelMult)
        {
            if (!Connected)
            {
                return false;
            }

            ChangePumpSetup(Storage.SelectedPump, Storage.PumpARatio, 100 - Storage.PumpARatio);

            HeadPurgingVolume = (int)volume;
            int numPulses = GetMaxNumPulses();
            string cmd = pumpOp == PumpOp.Head ? "hedpurg" : "pmppurg";

            RunCommand($"params[15]={numPulses}");
            RunCommand($"params[20]={volume}");
            RunCommand($"params[21]={speed}");
            RunCommand($"params[22]={rechargeRate}");

            RunCommand($"setting[3]={uLConv}");
            RunCommand($"setting[8]={accelMult}");

            RunCommand($"XQ #{cmd}, 5");

            return true;
        }

        private int GetMaxNumPulses()
        {
            int numPulses;

            if (Storage.SelectedPump == 1)
            {
                numPulses = SyringePumpBDetected ? Storage.MaxSyringeBPulses : Storage.MaxPOHBPulses;
            }
            else
            {
                numPulses = SyringePumpDetected ? Storage.MaxSyringePulses : Storage.MaxPOHPulses;
            }

            return numPulses;
        }

        public bool StopPrime()
        {
            bool bRetVal = false;
            if (!Connected)
                return false;
            RunCommand("gMem[4]=gMem[4]&@COM[$7]|4"); //Reset Pump Operation Status
            RunCommand("gMem[2]=1");
            RunCommand("params[19]=0"); //Reset # of Cycles To Run
            return bRetVal;
        }

        #endregion Pump Operations

        #region Valve Operations

        public bool HomeSmartValve(int selectedValveId)
        {
            if (selectedValveId == 1)
            {
                //Valve-B Selected
                RunCommand($"XQ #homvlvb,6");
            }
            else
            {
                //Valve-A Selected
                RunCommand($"XQ #homvlv,6");
            }

            return true;
        }

        public bool MoveValveToVent(int selectedValveID) // [Valve-A=0 / Valve-B=1]
        {
            if (ValveCmdReady(selectedValveID))
            {
                SetAllAOrAllB(selectedValveID);
                Thread.Sleep(10);
                RunCommand("XQ#vlvvnt,6;");
                return true;
            }

            return false;
        }

        public bool MoveValveToOff(int selectedValveID)
        {
            if (ValveCmdReady(selectedValveID))
            {
                SetAllAOrAllB(selectedValveID);
                Thread.Sleep(10);
                RunCommand("XQ#vlvoff,6;");
                return true;
            }

            return false;
        }

        public bool MoveValveToRecharge(int selectedValveID = 0)
        {
            if (ValveCmdReady(selectedValveID))
            {
                SetAllAOrAllB(selectedValveID);
                Thread.Sleep(10);
                RunCommand("XQ#vlvrch,6;");
                return true;
            }

            return false;
        }

        public bool MoveValveToDispense(int selectedValveID = 0)
        {
            if (ValveCmdReady(selectedValveID))
            {
                SetAllAOrAllB(selectedValveID);
                Thread.Sleep(10);
                RunCommand("XQ#vlvdsp,6;");
                return true;
            }

            return false;
        }

        private void SetAllAOrAllB(int selectedValveID)
        {
            if (selectedValveID == 0)
            {
                ChangePumpSetup(selectedValveID, 100, 0);
            }
            else
            {
                ChangePumpSetup(selectedValveID, 0, 100);
            }
        }

        #endregion Valve Operations

        #region IR Lamp Operations

        private void IRReconnect_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!MS.IRLampInstalled)
            {
                return;
            }

            IRTransmitter = IRTransmitterFactory.CreateIRTransmitter(MS, IsDemo);
            _log.log(LogType.TRACE, Category.INFO, "IR-Lamp Class Loaded");

            if (IRTransmitter is null)
            {
                string msg = $"Could not create the IRTransmitter class -- the IR Lamp is disabled!";
                _log.log(LogType.TRACE, Category.INFO, msg);
                return;
            }

            IRTransmitter.NewLogMessage += msg => _log.log(LogType.TRACE, Category.INFO, msg);

            if (!IRTransmitter.IsConnected && MS.IRLampInstalled)
            {
                bool bIRCOMExists = COMPortExists(MS.IRCOMPort);

                if (!IRTransmitter.IsConnected && bIRCOMExists && IRTransmitter.Connect())
                {
                    IRTransmitter.IsConnected = true;
                    IRReconnectAttempts = 0;
                    _log.log(LogType.TRACE, Category.INFO, "Connection Established To IR-Lamp!");
                }
                else if (IRTransmitter.IsConnected)
                {
                    IRTransmitter.IsConnected = true;
                    IRReconnectAttempts = 0;
                    _log.log(LogType.TRACE, Category.WARN, "IR Lamp Already Connected...");
                }
                else if (!bIRCOMExists)
                {
                    IRTransmitter.IsConnected = false;
                    _log.log(LogType.TRACE, Category.ERROR, "IR COM Port Selected Does Not Exist!");
                }
                else
                {
                    IRTransmitter.IsConnected = false;
                    _log.log(LogType.TRACE, Category.ERROR, "Could Not Establish Connection To IR-Lamp!");
                }
            }
        }

        private bool COMPortExists(string portName)
        {
            bool exists = false;
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
                if (port == portName || port.Contains(portName))
                    exists = true;

            return exists;
        }

        private void IRReconnect_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IRTransmitter.IsConnected)
            {
                IRTransmitter.IsConnected = true;
                IRReconnectAttempts = 0;
                _log.log(LogType.TRACE, Category.INFO, "Connection Established To IR-Lamp!");
            }
            else
            {
                IRReconnectAttempts += 1;
            }
        }

        public void SetupIRLamp()
        {
            if (MS.IRLampInstalled)
            {
                _log.log(LogType.TRACE, Category.INFO, "Connecting To IR-Lamp...");
                _bgwIRReconnect.RunWorkerAsync(); //Start Connect Worker
            }
        }

        #endregion

        #region Air Knife Operations

        public bool AirKnifeMainValveOpen => Outputs[3] == 1.0;
        public bool AirKnifeValveOpen => Outputs[18] == 1.0;
        public bool AirKnifeGasWarmupValveOpen => Outputs[17] == 1.0;
        public bool AirKnifeHeatEnabled => Outputs[19] == 1.0;

        public bool AirKknifeWarmupModeEnabled =>
            AirKnifeMainValveOpen && AirKnifeGasWarmupValveOpen && !AirKnifeValveOpen && AirKnifeHeatEnabled;
        public bool AirKnifeOn => AirKnifeMainValveOpen && (!MS.AirKnifeHeaterInstalled || (MS.AirKnifeHeaterInstalled && !AirKnifeGasWarmupValveOpen && AirKnifeValveOpen));


        internal void SetAirKnifeMainValve(bool state)
        {
            if (AirKnifeMainValveOpen == state)
            {
                return;
            }

            if (Connected)
            {
                RunCommand(state ? "SB3" : "CB3");
            }
        }

        internal void SetAirKnifeGasWarmupValve(bool state)
        {
            if (AirKnifeGasWarmupValveOpen == state)
            {
                return;
            }

            if (Connected)
            {
                RunCommand(state ? "SB17" : "CB17");
            }
        }

        internal void SetAirKnifeValve(bool state)
        {
            if (AirKnifeValveOpen == state)
            {
                return;
            }

            if (Connected)
            {
                RunCommand(state ? "SB18" : "CB18");
            }
        }

        internal void SetAirKnifeHeater(bool state)
        {
            if (AirKnifeHeatEnabled == state)
            {
                return;
            }

            if (Connected)
            {
                RunCommand(state ? "SB19" : "CB19");
            }
        }

        internal void SetAirKnife(bool state)
        {
            SetAirKnifeMainValve(state);

            if (MS.AirKnifeHeaterInstalled)
            {
                SetAirKnifeGasWarmupValve(false);
                SetAirKnifeValve(state);
                SetAirKnifeHeater(state);
            }
        }

        public bool SetGasKnifeWarmUpMode(bool state)
        {
            if (AirKknifeWarmupModeEnabled == state)
            {
                return true;
            }

            bool success = true;

            try
            {
                SetAirKnifeMainValve(state);
                SetAirKnifeGasWarmupValve(state);
                SetAirKnifeHeater(state);

                if (state)
                {
                    SetAirKnifeValve(false);
                }
            }
            catch (Exception Ex)
            {
                success = false;
                _log.log(LogType.TRACE, Category.ERROR, $"Exception Toggling Warm Up Mode. Exception: {Ex.Message}");
            }

            return success;
        }

        public void ToggleAirKnife()
        {
            SetAirKnife(AirKnifeMainValveOpen ? false : true);
        }

        #endregion

        #region Jog Operations

        public bool CarriageIsClear => MS.HasLoader ? LoaderPos <= MS.SafeCarriageLoc : true;  // Carriage is in safe place to enable Loader Move
        public bool LoaderIsClear => MS.HasLoader ? LoaderPos <= MS.SafeLoaderLoc : true;  // Loader is in safe place to enable Carriage Move
        public bool JogXActive { get { int nFlag = 0x8; return ((int)Memory[4] & nFlag) == nFlag; } }
        public bool JogZActive { get { int nFlag = 0x10; return ((int)Memory[4] & nFlag) == nFlag; } }
        public bool CanJogTo(double dNewLoc)
        {
            bool bRetVal = true;

            double[] dZoneZs = new double[5];

            dZoneZs[0] = MS.Zone1MaxZ;
            dZoneZs[1] = MS.Zone2MaxZ;
            dZoneZs[2] = MS.Zone3MaxZ;
            dZoneZs[3] = MS.Zone4MaxZ;
            dZoneZs[4] = MS.Zone5MaxZ;

            int nFirstZone = 4, nLastZone = 0;
            int nTemp;

            // figure out where we are
            if (XPos >= MS.Zone5Start) nFirstZone = 4;
            else if (XPos >= MS.Zone4Start) nFirstZone = 3;
            else if (XPos >= MS.Zone3Start) nFirstZone = 2;
            else if (XPos >= MS.Zone2Start) nFirstZone = 1;
            else nFirstZone = 0;

            // figure out where we're going
            if (dNewLoc >= MS.Zone5Start) nLastZone = 4;
            else if (dNewLoc >= MS.Zone4Start) nLastZone = 3;
            else if (dNewLoc >= MS.Zone3Start) nLastZone = 2;
            else if (dNewLoc >= MS.Zone2Start) nLastZone = 1;
            else nLastZone = 0;

            // swap them in they are going 'backwards'
            if (nFirstZone > nLastZone)
            {
                nTemp = nLastZone;
                nLastZone = nFirstZone;
                nFirstZone = nTemp;
            }

            //if (nLastZone != nFirstZone)
            //{
            // Loop thru all the zones checking the MaxZ...
            for (int nIdx = nFirstZone; nIdx<=nLastZone; nIdx++)
            {
                bRetVal &= (ZRPos <= dZoneZs[nIdx] && ZLPos <= dZoneZs[nIdx]);
            }
            //}
            //else
            //{
            //    bRetVal = (ZRPos <= dZoneZs[nFirstZone] && ZLPos <= dZoneZs[nFirstZone]);
            //}


            return bRetVal;
        }
        public void AbortJogX()
        {
            RunCommand("STA");
        }
        public bool JogX(double dSpeed, double dLocation, bool bWait = false)
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            ResetMaxPosError("X");
            string sCmd = string.Format("params[1]={0}", dSpeed * mmConv);
            RunCommand(sCmd);
            sCmd = string.Format("params[2]={0}", MS.DefaultXAccel * mmConv);
            RunCommand(sCmd);
            sCmd = string.Format("params[3]={0}", MS.DefaultXDecel * mmConv);
            RunCommand(sCmd);
            sCmd = string.Format("params[4]={0}", MS.DefaultXSCurve / 100.0);
            RunCommand(sCmd);
            sCmd = string.Format("params[5]={0}", (int)(dLocation * mmConv));
            RunCommand(sCmd);
            sCmd = string.Format("XQ #jogx, 3");
            RunCommand(sCmd);
            return bRetVal;
        }
        public bool JogTransfer(double dSpeed, double dLocation, bool bWait = false)
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            string sCmd = string.Format("params[1]={0}", dSpeed * mmConvTransfer);
            RunCommand(sCmd);
            sCmd = string.Format("params[2]={0}", dSpeed * mmConvTransfer);
            RunCommand(sCmd);
            sCmd = string.Format("params[3]={0}", dSpeed * mmConvTransfer);
            RunCommand(sCmd);
            sCmd = string.Format("params[4]={0}", MS.DefaultXSCurve / 100.0);
            RunCommand(sCmd);
            sCmd = string.Format("params[5]={0}", (int)(dLocation * mmConvTransfer));
            RunCommand(sCmd);
            sCmd = string.Format("XQ #jogf, 3");
            RunCommand(sCmd);
            return bRetVal;
        }
        public bool MoveConveyorToChuck()
        {
            if (!Connected)
                return false;
            bool bRetVal = true;
            DownloadTransferSettings();
            string sCmd = string.Format("XQ #cnv2chk, 3");
            RunCommand(sCmd);
            return bRetVal;
        }
        public bool MoveConeyorToStack()
        {
            if (!Connected)
                return false;
            bool bRetVal = true;
            DownloadTransferSettings();
            string sCmd = string.Format("XQ #cnv2ldr, 3");
            RunCommand(sCmd);
            return bRetVal;
        }
        public void AbortJogZ()
        {
            RunCommand("STBC");
        }
        public bool JogZLeft(double dSpeed, double dLocation, bool bWait = false)
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            ResetMaxPosError("LZ");
            string sCmd = string.Format("params[1]={0}", dSpeed * mmConv);
            RunCommand(sCmd);
            sCmd = string.Format("params[2]={0}", MS.DefaultZAccel * mmConv);
            RunCommand(sCmd);
            sCmd = string.Format("params[3]={0}", MS.DefaultZDecel * mmConv);
            RunCommand(sCmd);
            sCmd = string.Format("params[4]={0}", MS.DefaultZSCurve / 100.0);
            RunCommand(sCmd);
            sCmd = string.Format("params[5]={0}", (int)(dLocation * mmConv));
            RunCommand(sCmd);
            sCmd = string.Format("XQ #jogzL, 3");
            RunCommand(sCmd);
            return bRetVal;
        }
        public bool JogZRight(double dSpeed, double dLocation, bool bWait = false)
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            ResetMaxPosError("RZ");
            string sCmd = string.Format("params[1]={0}", dSpeed * mmConv);
            RunCommand(sCmd);
            sCmd = string.Format("params[2]={0}", MS.DefaultZAccel * mmConv);
            RunCommand(sCmd);
            sCmd = string.Format("params[3]={0}", MS.DefaultZDecel * mmConv);
            RunCommand(sCmd);
            sCmd = string.Format("params[4]={0}", MS.DefaultZSCurve / 100.0);
            RunCommand(sCmd);
            sCmd = string.Format("params[5]={0}", (int)(dLocation * mmConv));
            RunCommand(sCmd);
            sCmd = string.Format("XQ #jogzR, 3");
            RunCommand(sCmd);
            return bRetVal;
        }
        public bool JogZBoth(double dSpeed, double dLocationRight, double dLocationLeft, bool bWait = false)
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            ResetMaxPosError("Z");
            string sCmd = string.Format("params[1]={0}", dSpeed * mmConv);
            RunCommand(sCmd);
            sCmd = string.Format("params[2]={0}", MS.DefaultZAccel * mmConv);
            RunCommand(sCmd);
            sCmd = string.Format("params[3]={0}", MS.DefaultZDecel * mmConv);
            RunCommand(sCmd);
            sCmd = string.Format("params[4]={0}", MS.DefaultZSCurve / 100.0);
            RunCommand(sCmd);
            sCmd = string.Format("params[5]={0}", (int)(dLocationRight * mmConv));
            RunCommand(sCmd);
            sCmd = string.Format("params[6]={0}", (int)(dLocationLeft * mmConv));
            RunCommand(sCmd);
            sCmd = string.Format("XQ #jogz, 3");
            RunCommand(sCmd);
            return bRetVal;
        }

        #endregion Jog Operations

        #region Go-To Operations

        internal bool RunGotoMaintenance()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            try
            {
                string sCmd = string.Format("params[1]={0}", MS.XMaintLoc);
                RunCommand(sCmd);
                Thread.Sleep(5);
                sCmd = string.Format("params[2]={0}", MS.ZMaintLoc);
                RunCommand(sCmd);
                Thread.Sleep(5);
                sCmd = string.Format("params[3]={0}", MS.ZMaintLoc);
                RunCommand(sCmd);
                Thread.Sleep(5);
                sCmd = string.Format("XQ #go2mant, 3");
                RunCommand(sCmd);
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception During execution of #go2mant: " + ex.Message, "Error");
            }
            return bRetVal;
        }
        internal bool RunGotoDieLoad()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            try
            {
                string sCmd = string.Format("params[1]={0}", MS.XDieLoadLoc);
                RunCommand(sCmd);
                Thread.Sleep(5);
                sCmd = string.Format("params[2]={0}", MS.ZRDieLoadLoc);
                RunCommand(sCmd);
                Thread.Sleep(5);
                sCmd = string.Format("params[3]={0}", MS.ZLDieLoadLoc);
                Thread.Sleep(5);
                RunCommand(sCmd);
                sCmd = string.Format("XQ #go2mant, 3");
                RunCommand(sCmd);
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception During execution of #go2mant: " + ex.Message, "Error");
            }
            return bRetVal;
        }
        internal bool RunRecharge()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            try
            {
                string sCmd = string.Format("XQ #recharg, 2");
                RunCommand(sCmd);
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception During execution of #recharg: " + ex.Message, "Error");
            }
            return bRetVal;
        }
        internal bool RunGotoMeasurePosition()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            try
            {
                SetMemoryVal(20, MS.MeasureLoc);
                Thread.Sleep(5);
                SetMemoryVal(21, MS.ZLMeasurePos);
                Thread.Sleep(5);
                SetMemoryVal(22, MS.ZRMeasurePos);
                Thread.Sleep(5);
                _log.log(LogType.TRACE, Category.INFO, $"Running #Go2Meas - X: {MS.MeasureLoc}  ZL: {MS.ZLMeasurePos}  ZR: {MS.ZRMeasurePos}", "INFO");
                string sCmd = string.Format("XQ #go2Meas, 3");
                RunCommand(sCmd);
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception During execution of #go2Meas: " + ex.Message, "Error");
            }
            return bRetVal;
        }
        internal bool RunGotoVisionPosition()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            try
            {
                SetMemoryVal(23, MS.VisionXPos);
                Thread.Sleep(5);
                SetMemoryVal(24, MS.VisionZPos);
                Thread.Sleep(5);
                _log.log(LogType.TRACE, Category.INFO, $"Running #Go2Vis: X:{MS.VisionXPos} Z:{MS.VisionZPos}", "INFO");
                RunCommand("XQ #go2Vis, 3");
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception During execution of #go2Vis: " + ex.Message, "Error");
            }
            return bRetVal;
        }
        internal bool RunGotoHome()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            try
            {
                string sCmd = string.Format("XQ #go2home, 3");
                RunCommand(sCmd);
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception During execution of #go2home: " + ex.Message, "Error");
            }
            return bRetVal;
        }
        internal bool RunFullZUp()
        {
            bool bRetVal = true;
            if (!Connected)
                return false;
            try
            {
                string sCmd = string.Format("XQ #fullzup, 3");
                RunCommand(sCmd);
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception During execution of #fullzup: " + ex.Message, "Error");
            }
            return bRetVal;
        }

        #endregion

        #region Calibration and Leveling

        public string LevelFailReason { get; private set; } = "";
        public bool IsFindingPumpLimits => ((int)Memory[4] & 0x20) > 0;
        public bool IsPrimingMaxZSet => true;

        public void StartFindPumpLimits(int pumpID)
        {
            bool bContinue = true;

            if (!IsFindingPumpLimits)
            {
                _log.log(LogType.TRACE, Category.INFO, $"Selected Pump: {(pumpID == 0 ? $"{(SyringePumpDetected ? "Syringe" : "POH")}-A" : pumpID == 1 ? $"{(SyringePumpBDetected ? "Syringe" : "POH")}-B" : "Both")}");
                ChangePumpSetup(pumpID, pumpID == 0 ? 100 : 0, pumpID == 1 ? 100 : 0);

                Thread.Sleep(5);

                _log.log(LogType.TRACE, Category.INFO, "Executing #pmplmts From Controller.");

                try
                {
                    if(pumpID==0)
                        RunCommand("XQ #pmplmts, 4");
                    else
                        RunCommand("XQ #pmplmtB, 4");
                }
                catch (Exception ex)
                {
                    _log.log(LogType.TRACE, Category.INFO, "FindPumpLimits: Error While launching PumpLimits routine on controller - " + ex.Message);
                    bContinue = false;
                }

                if (bContinue)
                {
                    _log.log(LogType.TRACE, Category.INFO, "#pmplmts has started");
                }
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "Find Pump Limits was requested when it was already running", "Warning");
            }
        }

        public bool SetResponseTime(int respTime)
        {
            return _keyence.SetResponseTime(respTime);
        }

        public bool SetMaxZ(bool IsPriming, bool ResetValues)
        {
            bool bRetVal = true;

            // TODO Max Z work

            //if (IsPriming)
            //{
            //	if (ResetValues)
            //	{
            //		_log.log(LogType.TRACE, Category.INFO, "GW: Resetting Priming Plate Max Z values");
            //		Storage.ZRMaxZPriming = 0.0;
            //		Storage.ZLMaxZPriming = 0.0;
            //	}
            //	else
            //	{
            //		Storage.ZRMaxZPriming = ZRPos + MachineSettings.ShimSize / 1000.0;
            //		Storage.ZLMaxZPriming = ZLPos + MachineSettings.ShimSize / 1000.0;
            //		_log.log(LogType.TRACE, Category.INFO, $"GW: Setting Priming Plate Max Z values {{R,L}}: {{{Storage.ZRMaxZPriming.ToString("0.000")},{Storage.ZLMaxZPriming.ToString("0.000")}}}");
            //	}
            //}
            //else
            //{
            //	if (ResetValues)
            //	{
            //		_log.log(LogType.TRACE, Category.INFO, "GW: Resetting Chuck Max Z values");
            //		Storage.ZRMaxZChuck = 0.0;
            //		Storage.ZLMaxZChuck = 0.0;
            //	}
            //	else
            //	{
            //		Storage.ZRMaxZChuck = ZRPos + MachineSettings.ShimSize / 1000.0;
            //		Storage.ZLMaxZChuck = ZLPos + MachineSettings.ShimSize / 1000.0;
            //		_log.log(LogType.TRACE, Category.INFO, $"GW: Setting Chuck Max Z values {{R,L}}: {{{Storage.ZRMaxZChuck.ToString("0.000")},{Storage.ZLMaxZChuck.ToString("0.000")}}}");
            //	}
            //}
            //Storage.Save();
            return bRetVal;
        } // TODO - Finish Implementation

        private void PauseCalibration(string message, Func<bool> cancelRequested)
        {
            DateTime endTime = DateTime.Now.AddMinutes(15);

            CalibrationPaused = true;
            CalibrationMessage?.Invoke(message);

            while (CalibrationPaused && !cancelRequested() && DateTime.Now < endTime)
            {
                Thread.Sleep(50);
            }

            if (cancelRequested())
            {
                CalibrationPaused = false;
                _log.log(LogType.TRACE, Category.ERROR, $"Calibration was cancelled by the user.");
                throw new OperationCanceledException();
            }
            else if (CalibrationPaused)
            {
                CalibrationPaused = false;
                _log.log(LogType.TRACE, Category.ERROR, $"Calibration was left paused for too long -- cancelling.");
                throw new OperationCanceledException();
            }
        }

        public void ContinueCalibration()
        {
            if (!CalibrationPaused)
            {
                return;
            }

            _log.log(LogType.TRACE, Category.INFO, $"User has re-started the calibration routine from a pause.");
            CalibrationPaused = false;
        }

        private bool ZeroOutGT2s(bool remainExtended = false)
        {
            if (!GT2sExtended)
            {
                ExtendGT2s();
                Thread.Sleep(MS.GT2SettleDelay);
            }

            OpenKeyence();
            bool bContinue = ResetAllGT2PresetValues();
            bContinue &= PresetAll(MS.ShimSize, Storage.SelectedBankNo);

            if (!remainExtended)
            {
                RetractGT2s();
            }

            return bContinue;
        }

        private void UpdateGT2Data()
        {
            _log.log(LogType.TRACE, Category.INFO, $"Updating GT2 Data.");
            OpenKeyence();
            double leftGT2 = 0, rightGT2 = 0;
            KeyenceState ksLeft = KeyenceState.Unknown, ksRight = KeyenceState.Unknown;
            ReadKeyence(ref rightGT2, ref ksRight, ref leftGT2, ref ksLeft);
            LeftGT2Pos = leftGT2; RightGT2Pos = rightGT2; LeftGT2State = ksLeft; RightGT2State = ksRight;
        }

        public void SimpleDieCalibration(Func<bool> cancelRequested)
        {
            _log.log(LogType.TRACE, Category.INFO, $"Starting SimpleDieCalibration().");
            CalibratingDie = true;
            UpdateGT2Data();

            try
            {
                _log.log(LogType.TRACE, Category.INFO, $"SimpleDieCalibration() Zeroing Out GT2s.");

                if (!ZeroOutGT2s(true))
                {
                    PauseCalibration("Could not zero out the GT2s.  Cancelling calibration.", cancelRequested);
                    return;
                }

                _log.log(LogType.TRACE, Category.INFO, $"SimpleDieCalibration() GT2s Zeroed Successfully.");
                MS.ZLZeroEncPosForChuck = LZ_TP;
                MS.ZRZeroEncPosForChuck = RZ_TP;
                MS.Save();
                UpdateGT2Data();

                if (!MS.UsingKeyenceLaser)
                {
                    _log.log(LogType.TRACE, Category.INFO, $"No Lasers are installed, so calibration is complete with the GT2s.");
                    _log.log(LogType.TRACE, Category.INFO, $"Retacting the GT2s, and going full up.");
                    RetractGT2s();
                    RunFullZUp();
                    PauseCalibration("GT2 Calibration has completed successfully.", cancelRequested);
                    return;
                }

                PauseCalibration($"The die is at shim height. To calibrate the lasers, open LK-Navigator2, " + 
                    $"enter {MS.ShimSize:#0.000} into Display 1 for OUT1/OUT2. " +
                    $"Press OK when you are ready to move to the Upper calibration height.", cancelRequested);

                bool bContinue = LevelDieToGlass(false, null, null, isCalibration: true);

                if (bContinue)
                {
                    PauseCalibration($"The die is at the Upper calibration height. To calibrate the lasers, open LK-Navigator2, " +
                        $"enter {MS.LaserCalibrationUpperHeight/1000.0:#0.000} into Display 2 for OUT1/OUT2. " +
                        $"Press OK when you are done.", cancelRequested);
                }
                else
                {
                    PauseCalibration("There was an error moving the die up to the Display 2 height.  Calibration is cancelled.", cancelRequested);
                    return;
                }

                PauseCalibration("Calibration has completed successfully", cancelRequested);
            }
            catch (OperationCanceledException)
            {
                _log.log(LogType.TRACE, Category.WARN, "User cancelled calibration");
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.ERROR, $"Exception caught during calibration:  {ex}");
                _ = CheckForExceptionData(ex);
            }
            finally
            {
                if (GT2sExtended)
                {
                    _log.log(LogType.TRACE, Category.INFO, "Retracting GT2s at end of calibration");
                    RetractGT2s();
                }

                CalibratingDie = false;
            }
        }

        //TODO: Maybe go back to this form of calibration.  Someday.
        //public void RunDieCalibration()
        //{
        //    CalibrationPaused = false;
        //    CalibratingDie = false;
        //    bool bContinue = true;
        //    double dRight = 0, dLeft = 0;
        //    KeyenceState ksRight = KeyenceState.Unknown, ksLeft = KeyenceState.Unknown;

        //    double tpb1 = 0, tpb2 = 0, tpb3 = 0;
        //    double tpc1 = 0, tpc2 = 0, tpc3 = 0;
        //    double lZCal1 = 0, lZCal2 = 0;
        //    double rZCal1 = 0, rZCal2 = 0;

        //    try
        //    {
        //        SetMaxZ(false, false);
        //        SetMaxZ(true, true); // reset priming

        //        string laserStr = MachineSettings.UsingKeyenceLaser ? "1" : "0";
        //        _log.log(LogType.TRACE, Category.INFO, $"UsingKeyenceLaser = {(laserStr == "1" ? "True" : "False")}, Setting gMem[15]={laserStr}");
        //        RunCommand($"gMem[15]={laserStr}");

        //        DownloadDelays();
        //        RunCommand($"params[23]={0 * 1600}"); //right side return to height
        //        RunCommand($"params[24]={0 * 1600}"); //left side return to height
        //                                              //aSetMemoryVal(34, 0);
        //        RunCommand("gMem[4]=gMem[4]&@COM[$2000]");

        //        if (!WaitOnSignalBit(4, 0x2000, false, 1)) // wait on notication of thread running status  (1 sec timeout)
        //        {
        //            _log.log(LogType.TRACE, Category.INFO, "Calibrate: Timeout waiting on reset of calbrate started flag");
        //            SetMemoryVal(2, 1); // notfy controller thread to abort
        //            bContinue = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        bContinue = false;
        //        _log.log(LogType.TRACE, Category.INFO, "Calibrate: Exception while Waiting on Signal - " + ex.ToString());
        //        //nRadMessageBox.Show(this, "test test test", "test title", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }

        //    _log.log(LogType.TRACE, Category.INFO, " (1) launch the calibrate routine on controller");
        //    try { RunCommand("XQ #calbrat, 4"); }
        //    catch (Exception ex)
        //    {
        //        _log.log(LogType.TRACE, Category.INFO, "Calibrate: Error While launching calibrate routine on controller - " + ex.Message);
        //        bContinue = false;
        //    }
        //    // (2) wait for the notification that #calbrat is running
        //    if (bContinue && !WaitOnSignalBit(4, 0x2000, true, 10)) // wait on notication of thread start  (10 sec timeout)
        //    {
        //        _log.log(LogType.TRACE, Category.INFO, "Calibrate: Timeout waiting on waiting on calibration thread to start on controller");
        //        SetMemoryVal(2, 1); // notfy controller thread to abort
        //        bContinue = false;
        //    }
        //    _log.log(LogType.TRACE, Category.INFO, "#calbrat has started");

        //    _log.log(LogType.TRACE, Category.INFO, " (3) wait for the first signal to say that we are ready to measure");
        //    if (bContinue && !WaitOnSignal(3, 1, 20))  // wait on first signal from controller (20 sec timeout)
        //    {
        //        _log.log(LogType.TRACE, Category.INFO, "Calibrate: Timeout waiting on 'signal' to reset (3)");
        //        SetMemoryVal(2, 1); // notfy controller thread to abort
        //        bContinue = false;
        //    }

        //    if (bContinue)
        //    {
        //        if (!MachineSettings.UsingKeyenceLaser)
        //        {
        //            _log.log(LogType.TRACE, Category.INFO, " (4)  Preset the GT2s (use shim size) ");

        //            OpenKeyence();
        //            bContinue &= ResetAllGT2PresetValues();
        //            bContinue &= PresetAll(MachineSettings.ShimSize, Storage.SelectedBankNo);
        //            bContinue &= ReadKeyence(ref dRight, ref ksRight, ref dLeft, ref ksLeft);

        //            if (!bContinue)
        //            {
        //                _log.log(LogType.TRACE, Category.INFO, "Calibrate: Could not communicate with Keyence (4).  Aborting Calibrate.");
        //                SetMemoryVal(2, 1); // notfy controller thread to abort
        //                bContinue = false;
        //            }
        //            else
        //            {
        //                tpb1 = TPB;
        //                tpc1 = TPC;
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Right : {TPB} counts");
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Left  : {TPC} counts");
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Right : {ZRPos:0.000} mm");
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Left  : {ZLPos:0.000} mm");
        //                _log.log(LogType.TRACE, Category.INFO, $"Keyence Right : {dRight:0.000} (mm) : {ksRight}");
        //                _log.log(LogType.TRACE, Category.INFO, $"Keyence Left  : {dLeft:0.000} (mm) : {ksLeft}");
        //            }
        //        }
        //        else
        //        {
        //            if (!LaserMgr.GetLeftAndRightLaserValues(out double left, out double right, DateTime.Now))
        //            {
        //                _log.log(LogType.TRACE, Category.INFO, "Calibrate: Could not communicate with Keyence Lasers (4).  Aborting Calibrate.");
        //                SetMemoryVal(2, 1); // notfy controller thread to abort
        //                bContinue = false;
        //            }
        //            else
        //            {
        //                tpb3 = left;
        //                tpc3 = right;
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Right - {ZRPos} mm");
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Left - {ZLPos} mm");
        //                _log.log(LogType.TRACE, Category.INFO, $"Keyence Laser Right : {right:0.000} (mm)");
        //                _log.log(LogType.TRACE, Category.INFO, $"Keyence Laser Left  : {left:0.000} (mm)");
        //                _log.log(LogType.TRACE, Category.INFO, $"Setting DoneReading in Galil Controller.");
        //                SetMemoryVal(14, 1.0);
        //            }
        //        }
        //    }

        //    if (MachineSettings.UsingKeyenceLaser && !PauseCalibration($"Laser are measuring the low position"))
        //    {
        //        bContinue = false;
        //    }
        //    else
        //    {
        //        MachineSettings.ZLZeroEncPosForChuck = tpc1;
        //        MachineSettings.ZRZeroEncPosForChuck = tpb1;
        //    }

        //    if (bContinue)
        //    {
        //        _log.log(LogType.TRACE, Category.INFO, " (9)(10) reset waiting, set Continue ");
        //        SetMemoryVal(3, 0);

        //        if (!WaitOnSignal(3, 0, 1)) // 1 sec timeout
        //        {
        //            _log.log(LogType.TRACE, Category.INFO, "Calibrate: Timeout waiting on 'waiting' to reset (10)");
        //            SetMemoryVal(2, 1); // notfy controller thread to abort
        //            bContinue = false;
        //        }

        //        SetMemoryVal(1, 1);
        //    }

        //    if (bContinue)
        //    {
        //        _log.log(LogType.TRACE, Category.INFO, " (11) wait for the second signal to say that we are ready to measure");

        //        if (!WaitOnSignal(3, 1, 20))  // wait on first signal from controller (20 sec timeout)
        //        {
        //            _log.log(LogType.TRACE, Category.INFO, "Calibrate: Timeout waiting on 'signal' to reset (11)");
        //            SetMemoryVal(2, 1); // notfy controller thread to abort
        //            bContinue = false;
        //        }
        //    }


        //    if (bContinue)
        //    {
        //        if (!MachineSettings.UsingKeyenceLaser)
        //        {
        //            _log.log(LogType.TRACE, Category.INFO, " (12)  Read the GT2s and Left/Right Z locations   @ left high");
        //            bContinue = ReadKeyence(ref dRight, ref ksRight, ref dLeft, ref ksLeft);

        //            if (!bContinue)
        //            {
        //                _log.log(LogType.TRACE, Category.INFO, "Calibrate: Could not communicate with Keyence (12).  Aborting Calibrate.");
        //                SetMemoryVal(2, 1); // notfy controller thread to abort
        //                bContinue = false;
        //            }
        //            else
        //            {
        //                tpb2 = TPB;
        //                tpc2 = TPC;
        //                rZCal1 = dRight;
        //                lZCal1 = dLeft;
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Right - {tpb2} counts");
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Left - {tpc2} counts");
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Right - {ZRPos} mm");
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Left - {ZLPos} mm");
        //                _log.log(LogType.TRACE, Category.INFO, $"Keyence Right - {dRight} {ksRight}");
        //                _log.log(LogType.TRACE, Category.INFO, $"Keyence Left - {dLeft} {ksLeft}");
        //            }
        //        }
        //        else
        //        {
        //            _log.log(LogType.TRACE, Category.INFO, " (12)  Read the Lasers and Left/Right Z locations   @ left high");

        //            if (!LaserMgr.GetLeftAndRightLaserValues(out double left, out double right, DateTime.Now))
        //            {
        //                _log.log(LogType.TRACE, Category.INFO, "Calibrate: Could not communicate with Keyence Lasers (4).  Aborting Calibrate.");
        //                SetMemoryVal(2, 1); // notfy controller thread to abort
        //                bContinue = false;
        //            }
        //            else
        //            {
        //                dLeft = left;
        //                dRight = right;

        //                tpb3 = dRight;
        //                tpc3 = dLeft;
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Right - {ZRPos} mm");
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Left - {ZRPos} mm");
        //                _log.log(LogType.TRACE, Category.INFO, $"Keyence Laser Right : {dRight:0.000} (mm)");
        //                _log.log(LogType.TRACE, Category.INFO, $"Keyence Laser Left  : {dLeft:0.000} (mm)");
        //                _log.log(LogType.TRACE, Category.INFO, $"Setting DoneReading in Galil Controller.");
        //                SetMemoryVal(14, 1.0);
        //            }
        //        }
        //    }


        //    if (bContinue)
        //    {
        //        _log.log(LogType.TRACE, Category.INFO, " (13)(14) reset waiting, set Continue");
        //        SetMemoryVal(3, 0);
        //        if (!WaitOnSignal(3, 0, 1)) // 1 sec timeout
        //        {
        //            _log.log(LogType.TRACE, Category.INFO, "Calibrate: Timeout waiting on 'waiting' to reset (13)");
        //            SetMemoryVal(2, 1); // notfy controller thread to abort
        //            bContinue = false;
        //        }
        //        SetMemoryVal(1, 1);
        //    }

        //    if (bContinue)
        //    {
        //        _log.log(LogType.TRACE, Category.INFO, " (15) wait for the thrid signal to say that we are ready to measure (controller will drop the GT2s and wait for settle)");

        //        if (!WaitOnSignal(3, 1, 20))  // wait on first signal from controller (20 sec timeout)
        //        {
        //            _log.log(LogType.TRACE, Category.INFO, "Calibrate: Timeout waiting on 'signal' to reset (15)");
        //            SetMemoryVal(2, 1); // notfy controller thread to abort
        //            bContinue = false;
        //        }
        //    }

        //    if (bContinue)
        //    {
        //        if (!MachineSettings.UsingKeyenceLaser)
        //        {
        //            _log.log(LogType.TRACE, Category.INFO, " (16)  Read the GT2s and Left/Right Z locations @ right side high");
        //            bContinue = ReadKeyence(ref dRight, ref ksRight, ref dLeft, ref ksLeft);

        //            if (!bContinue)
        //            {
        //                _log.log(LogType.TRACE, Category.INFO, "Calibrate: Could not communicate with Keyence (16).  Aborting Calibrate.");
        //                SetMemoryVal(2, 1); // notfy controller thread to abort
        //                bContinue = false;
        //            }
        //            else
        //            {
        //                tpb3 = TPB;
        //                tpc3 = TPC;
        //                rZCal2 = dRight;
        //                lZCal2 = dLeft;

        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Right - {ZRPos}");
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Left - {ZLPos}");
        //                _log.log(LogType.TRACE, Category.INFO, $"Keyence Right - {dRight} {ksRight}");
        //                _log.log(LogType.TRACE, Category.INFO, $"Keyence Left - {dLeft} {ksLeft}");
        //            }
        //        }
        //        else
        //        {
        //            _log.log(LogType.TRACE, Category.INFO, " (16)  Read the GT2s and Left/Right Z locations @ right side high");

        //            if (!LaserMgr.GetLeftAndRightLaserValues(out double left, out double right, DateTime.Now))
        //            {
        //                _log.log(LogType.TRACE, Category.INFO, "Calibrate: Could not communicate with Keyence Lasers (16).  Aborting Calibrate.");
        //                SetMemoryVal(2, 1); // notfy controller thread to abort
        //                bContinue = false;
        //            }
        //            else
        //            {
        //                dLeft = left;
        //                dRight = right;

        //                rZCal2 = dRight;
        //                lZCal2 = dLeft;
        //                tpb3 = dRight;
        //                tpc3 = dLeft;
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Right - {ZRPos} mm");
        //                _log.log(LogType.TRACE, Category.INFO, $"Encoder Left - {ZLPos} mm");
        //                _log.log(LogType.TRACE, Category.INFO, $"Keyence Laser Right - {dRight} {ksRight}");
        //                _log.log(LogType.TRACE, Category.INFO, $"Keyence Laser Left - {dLeft} {ksLeft}");
        //                _log.log(LogType.TRACE, Category.INFO, $"Setting DoneReading in Galil Controller.");
        //                SetMemoryVal(14, 1.0);
        //            }
        //        }
        //    }

        //    if (bContinue)
        //    {
        //        _log.log(LogType.TRACE, Category.INFO, " (17)(18) reset waiting, set Continue");
        //        SetMemoryVal(3, 0);

        //        if (!WaitOnSignal(3, 0, 1)) // 1 sec timeout
        //        {
        //            _log.log(LogType.TRACE, Category.INFO, "Calibrate: Timeout waiting on 'waiting' to reset (17)");
        //            SetMemoryVal(2, 1); // notfy controller thread to abort
        //            bContinue = false;
        //        }

        //        SetMemoryVal(1, 1);
        //    }

        //    _log.log(LogType.TRACE, Category.INFO, " (19) wait for the notification that #calbrat is finished ");

        //    if (bContinue && !WaitOnSignalBit(4, 0x2000, false, 10)) // wait on notication of thread start  (10 sec timeout)
        //    {
        //        _log.log(LogType.TRACE, Category.INFO, "Calibrate: Timeout waiting on waiting on calibration thread to finish on controller");
        //        SetMemoryVal(2, 1); // notfy controller thread to abort
        //        bContinue = false;
        //    }
        //    else
        //    {
        //        _log.log(LogType.TRACE, Category.INFO, "Calibrate: Received calibration thread finished signal");
        //    }

        //    if (MachineSettings.UsingKeyenceLaser)
        //    {
        //        //now do the math...

        //        double dlLaserpos = 0.0, drLaserpos = 0.0;
        //        double dPivot = MachineSettings.CrossBarWidth;

        //        double zl1_1mm = 1, zr1_1mm = 1;
        //        double zl2_1mm = 1, zr2_1mm = 1;

        //        zl1_1mm = (tpc1 - tpc2) / 1600;
        //        zr1_1mm = (tpb2 - tpb1) / 1600;
        //        zl2_1mm = (tpc3 - tpc1) / 1600;
        //        zr2_1mm = (tpb2 - tpb3) / 1600;
        //        _log.log(LogType.TRACE, Category.INFO, $"zl1_1mm : {zl1_1mm}");
        //        _log.log(LogType.TRACE, Category.INFO, $"zr1_1mm : {zr1_1mm}");
        //        _log.log(LogType.TRACE, Category.INFO, $"zl2_1mm : {zl2_1mm}");
        //        _log.log(LogType.TRACE, Category.INFO, $"zr2_1mm : {zr2_1mm}");

        //        //dlGT2pos = (dPivot - (lZCal1 / (1 * zl1_1mm) * dPivot) + (rZCal1 / (1*zl1_1mm) * dPivot)) / 2;   // replace the 1s with actual encoder distances
        //        //drGT2pos = (dPivot - (lZCal2 / (1 * (zr2_1mm-zr1_1mm)) * dPivot) + (rZCal2 / (1*(zr2_1mm-zr1_1mm)) * dPivot)) / 2;   // replace the 1s with actual encoder distances
        //        dlLaserpos = (dPivot - (lZCal1 * dPivot) + (rZCal1 * dPivot)) / 2;   // replace the 1s with actual encoder distances
        //        drLaserpos = (dPivot - (lZCal2 * dPivot) + (rZCal2 * dPivot)) / 2;   // replace the 1s with actual encoder distances

        //        _log.log(LogType.TRACE, Category.INFO, $"Pivot Width : {dPivot}");
        //        _log.log(LogType.TRACE, Category.INFO, $"lZCal1 : {lZCal1}");
        //        _log.log(LogType.TRACE, Category.INFO, $"lZCal2 : {lZCal2}");
        //        _log.log(LogType.TRACE, Category.INFO, $"rZCal1 : {rZCal1}");
        //        _log.log(LogType.TRACE, Category.INFO, $"rZCal2 : {rZCal2}");
        //        _log.log(LogType.TRACE, Category.INFO, $"Keyence Position Right : {drLaserpos}");
        //        _log.log(LogType.TRACE, Category.INFO, $"Keyence Position Left : {dlLaserpos}");
        //        _log.log(LogType.TRACE, Category.INFO, $"Distance between : {drLaserpos - dlLaserpos}");
        //        _log.log(LogType.TRACE, Category.INFO, $"Keyence Position Right (from right) : {dPivot - drLaserpos}");
        //        _log.log(LogType.TRACE, Category.INFO, $"Keyence Position Left (from left) : {dPivot - dlLaserpos}");

        //        Storage.LeftGT2pos = dlLaserpos;
        //        Storage.RightGT2pos = drLaserpos;
        //        Storage.Save();
        //        MachineSettings.Save();
        //    }
        //    else
        //    {
        //        CloseKeyence();

        //        //now do the math...

        //        double dlGT2pos = 0.0, drGT2pos = 0.0;
        //        double dPivot = MachineSettings.CrossBarWidth;

        //        double zl1_1mm = (tpc1 - tpc2) / 1600;
        //        double zr1_1mm = (tpb2 - tpb1) / 1600;
        //        double zl2_1mm = (tpc3 - tpc1) / 1600;
        //        double zr2_1mm = (tpb2 - tpb3) / 1600;

        //        _log.log(LogType.TRACE, Category.INFO, $"zl1_1mm : {zl1_1mm}");
        //        _log.log(LogType.TRACE, Category.INFO, $"zr1_1mm : {zr1_1mm}");
        //        _log.log(LogType.TRACE, Category.INFO, $"zl2_1mm : {zl2_1mm}");
        //        _log.log(LogType.TRACE, Category.INFO, $"zr2_1mm : {zr2_1mm}");

        //        //dlGT2pos = (dPivot - (lZCal1 / (1 * zl1_1mm) * dPivot) + (rZCal1 / (1*zl1_1mm) * dPivot)) / 2;   // replace the 1s with actual encoder distances
        //        //drGT2pos = (dPivot - (lZCal2 / (1 * (zr2_1mm-zr1_1mm)) * dPivot) + (rZCal2 / (1*(zr2_1mm-zr1_1mm)) * dPivot)) / 2;   // replace the 1s with actual encoder distances
        //        dlGT2pos = (dPivot - (lZCal1 * dPivot) + (rZCal1 * dPivot)) / 2;   // replace the 1s with actual encoder distances
        //        drGT2pos = (dPivot - (lZCal2 * dPivot) + (rZCal2 * dPivot)) / 2;   // replace the 1s with actual encoder distances

        //        _log.log(LogType.TRACE, Category.INFO, $"Pivot Width : {dPivot}");
        //        _log.log(LogType.TRACE, Category.INFO, $"lZCal1 : {lZCal1}");
        //        _log.log(LogType.TRACE, Category.INFO, $"lZCal2 : {lZCal2}");
        //        _log.log(LogType.TRACE, Category.INFO, $"rZCal1 : {rZCal1}");
        //        _log.log(LogType.TRACE, Category.INFO, $"rZCal2 : {rZCal2}");
        //        _log.log(LogType.TRACE, Category.INFO, $"Keyence Position Right : {drGT2pos}");
        //        _log.log(LogType.TRACE, Category.INFO, $"Keyence Position Left : {dlGT2pos}");
        //        _log.log(LogType.TRACE, Category.INFO, $"Distance between : {drGT2pos - dlGT2pos}");
        //        _log.log(LogType.TRACE, Category.INFO, $"Keyence Position Right (from right) : {dPivot - drGT2pos}");
        //        _log.log(LogType.TRACE, Category.INFO, $"Keyence Position Left (from left) : {dPivot - dlGT2pos}");

        //        Storage.LeftGT2pos = dlGT2pos;
        //        Storage.RightGT2pos = drGT2pos;
        //        Storage.Save();
        //        MachineSettings.Save();
        //    }

        //    CalibratingDie = false;
        //}

        private bool LevelDieToGlass(bool bPriming, Recipe recipe, Recipe defaultRecipe, bool isCalibration)
        {
            LevelAttempt = 0;
            LevelingDie = true;
            bool bRetVal = false;
            bool bContinue = true;
            string sReturn = "";
            double dRight = 0, dLeft = 0;
            KeyenceState ksRight = KeyenceState.Unknown, ksLeft = KeyenceState.Unknown;

            DateTime readTimestamp = DateTime.MinValue;

            double lGT2 = 0;
            double rGT2 = 0;

            double dMaxZRight = 0, dMaxZLeft = 0;

            //double dSlope = 0, 
            double lZMove = 0, rZMove = 0, dGap = 100, lastLZMove = 0, lastRZMove = 0;
            RecipeParam paramTemp = null;
            LevelFailReason = "Untraced - See Logs";
            SubCategory subCategory = !isCalibration ? SubCategory.RECIPE_RUN : SubCategory.CALIBRATION;

            if (!isCalibration)
            {
                if (MS.UsingKeyenceLaser)
                {
                    if (!LaserMgr?.SetProgramNumber(recipe.KeyenceProgramNumber) ?? false)
                    {
                        _log.log(LogType.TRACE, Category.INFO, $"Could not set Keyence Program Number to the Priming Program Number!", "", SubCategory.RECIPE_RUN);
                    }
                    else
                    {
                        _log.log(LogType.TRACE, Category.INFO, $"Keyence Laser Program Number Set: {recipe.KeyenceProgramNumber}", "", SubCategory.RECIPE_RUN);
                    }
                }

                _log.log(LogType.TRACE, Category.INFO, $"Setting gMem[15] (using lasers): {(recipe.UsingKeyenceLaser ? 1.0 : 0.0)}", "", SubCategory.RECIPE_RUN);
                SetMemoryVal(15, recipe.UsingKeyenceLaser ? 1.0 : 0.0);//Tell Controller we are using lasers
            }

            string primingStr = isCalibration ? "Calibration" : (bPriming ? "Priming Area" : "Coating Area");
            double primingValue = bPriming && !isCalibration ? MS.PrimingMeasureLoc : MS.MeasureLoc;

            _log.log(LogType.TRACE, Category.INFO, $"LevelDieToGlass() - Begin '{primingStr}'", "", subCategory);
            _log.log(LogType.TRACE, Category.INFO, $"LevelDieToGlass() - Current X: '{XPos:0.000}'  Programmed Loc: '{primingValue}'", "", subCategory);

            Thread.Sleep(500);

            if (bPriming)
            {
                dMaxZRight = MaxZAtCurrentLoc(MS.PrimingMeasureLoc); // TODO Max Z work  Storage.ZRMaxZPriming;
                dMaxZLeft = MaxZAtCurrentLoc(MS.PrimingMeasureLoc); // TODO Max Z work Storage.ZLMaxZPriming;
                _log.log(LogType.TRACE, Category.INFO,
                    $"GW: Max Zs for Priming Plate leveling {{R,L}}: {{{dMaxZRight:0.000},{dMaxZLeft:0.000}}}", "", SubCategory.RECIPE_RUN);

                if (dMaxZLeft == 0.0 || dMaxZRight == 0.0)
                {
                    _log.log(LogType.TRACE, Category.ERROR, "Priming Max Z is not set.  Level Routine aborted!", "", SubCategory.RECIPE_RUN);
                    Exception ex1 = new Exception("Priming Max Z is not set.  Level Routine aborted!");
                    ex1.Data.Add("Reason", "Priming Max Z");
                    ex1.Data.Add("Actual", $"{dMaxZRight.ToString("0.000")},{dMaxZLeft.ToString("0.000")}");
                    throw ex1;  // note: we're done now
                }

                if (GetParam(100, recipe, defaultRecipe, out paramTemp))
                {
                    _log.log(LogType.TRACE, Category.INFO, "Priming Gap = " + paramTemp.Value, "", SubCategory.RECIPE_RUN);
                    dGap = double.Parse(paramTemp.Value) / 1000;
                    RunCommand(string.Format("params[8]=1"));
                    RunCommand(string.Format("params[5]={0}", MS.PrimingMeasureLoc));
                }
                else
                {
                    _log.log(LogType.TRACE, Category.ERROR, "Couldn't read Priming Gap from recipe", "", SubCategory.RECIPE_RUN);
                    bContinue = false;
                    LevelFailReason = "Couldn't read Priming Gap from recipe";
                }
            }
            else if (isCalibration)
            {
                dMaxZRight = MaxZAtCurrentLoc(MS.MeasureLoc);
                dMaxZLeft = MaxZAtCurrentLoc(MS.MeasureLoc);

                if (dMaxZLeft == 0.0 || dMaxZRight == 0.0)
                {
                    Exception ex1 = new Exception("Chuck Max Z is not set.  Level Routine aborted!");
                    ex1.Data.Add("Reason", "Measure Loc Max Z");
                    ex1.Data.Add("Actual", $"{dMaxZRight:0.000}, {dMaxZLeft:0.000}");
                    throw ex1;  // note: we're done now
                }

                _log.log(LogType.TRACE, Category.INFO, $"GW: Max Zs for Chuck leveling {{R,L}}: {{{dMaxZRight.ToString("0.000")},{dMaxZLeft.ToString("0.000")}}}", "", subCategory);

                dGap = MS.LaserCalibrationUpperHeight/1000;
                _log.log(LogType.TRACE, Category.INFO, $"Laser Calibration Upper Height = {dGap} µm", "", subCategory);
                RunCommand("params[8]=2");
                Thread.Sleep(5);
                RunCommand($"params[5]={MS.MeasureLoc}");
            }
            else // coating gap
            {
                dMaxZRight = MaxZAtCurrentLoc(MS.MeasureLoc); // TODO Max Z work  Storage.ZRMaxZChuck;
                dMaxZLeft = MaxZAtCurrentLoc(MS.MeasureLoc); // TODO Max Z work  Storage.ZLMaxZChuck;

                if (dMaxZLeft == 0.0 || dMaxZRight == 0.0)
                {
                    Exception ex1 = new Exception("Chuck Max Z is not set.  Level Routine aborted!");
                    ex1.Data.Add("Reason", "Measure Loc Max Z");
                    ex1.Data.Add("Actual", $"{dMaxZRight.ToString("0.000")},{dMaxZLeft.ToString("0.000")}");
                    throw ex1;  // note: we're done now
                }

                _log.log(LogType.TRACE, Category.INFO, $"GW: Max Zs for Chuck leveling {{R,L}}: {{{dMaxZRight:0.000},{dMaxZLeft:0.000}}}");

                if (GetParam(136, recipe, defaultRecipe, out paramTemp))
                {
                    _log.log(LogType.TRACE, Category.INFO, $"Coating Gap = {paramTemp.Value} µm", "", subCategory);
                    dGap = double.Parse(paramTemp.Value) / 1000;
                    RunCommand("params[8]=0");
                    Thread.Sleep(5);
                    RunCommand($"params[5]={MS.MeasureLoc}");
                }
                else
                {
                    _log.log(LogType.TRACE, Category.ERROR, "Couldn't read Coating Gap from recipe", "", subCategory);
                    bContinue = false;
                    LevelFailReason = "Couldn't read Coating Gap from recipe";
                }
            }

            // start the 'lvldie' routine 
            if (bContinue && !_abortingRecipe)
            {
                _log.log(LogType.TRACE, Category.INFO, "[2] Calling Leveling cycle", "", subCategory);

                if (Threads[4])
                {
                    _log.log(LogType.TRACE, Category.ERROR, "LevelDie: Unable to start, Thread already exists.", "", subCategory);
                    bContinue = false;
                    _log.log(LogType.TRACE, Category.INFO, "LevelDie: Stopping Existing Thread.", "", subCategory);
                    RunCommand("HX4");
                }

                RunCommand("XQ#lvldie,4");

                // (2) wait for the notification that #lvldie is running   "gMem[4] bit 0x4000"
                if (bContinue && !WaitOnSignalBit(4, 0x4000, true, 10)) // wait on notication of thread start  (10 sec timeout)
                {
                    _log.log(LogType.TRACE, Category.ERROR, "LevelDie: Timeout waiting on waiting on lvldie thread to start on controller", "", subCategory);
                    SetMemoryVal(2, 1); // notfy controller thread to abort
                    bContinue = false;
                    LevelFailReason = "Couldn't start Level Die routine";
                }
                else
                {
                    _log.log(LogType.TRACE, Category.INFO, "#lvldie has started", "", subCategory);
                }
            }

            if (bContinue && _abortingRecipe)
            {
                _log.log(LogType.TRACE, Category.ERROR, "LevelDieToGlass: Aborting Recipe Run - 1", "", subCategory);
                SetMemoryVal(2, 1); // notfy controller thread to abort
                bContinue = false;
                LevelFailReason = "Aborting";
            }

            _log.log(LogType.TRACE, Category.INFO, " [3] wait for the first signal to say that we are ready to measure (controller will drop the GT2s and wait for settle)");
            if (bContinue && !WaitOnSignal(3, 1, 20))  // wait on first signal from controller (20 sec timeout)
            {
                _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Timeout waiting on 'signal' to reset (3)");
                SetMemoryVal(2, 1); // notfy controller thread to abort
                bContinue = false;
                LevelFailReason = $"Did not Receive Signal Update: '#LvlDie Started'";
            }

            // Do the dance, the leveling dance
            double dNewLeft = 0, dNewRight = 0, dDeltaL = 0, dDeltaR = 0;
            int nRetryCount = 0;
            int LaserRetries = 0;
            bool bNeedRedo = true; // Just to enter the loop the first time
            int nMaxRetries = isCalibration ? 100 : MS.MaxLevelingRetries;

            while (bNeedRedo && bContinue) // continue moves until success or failure from retries
            {
                readTimestamp = DateTime.Now;

                //     ------   lvldie returns to HERE   --------
                _log.log(LogType.TRACE, Category.INFO, string.Format("LevelDieToGlass() - '{0}'  #{1}", bPriming ? "Priming Area" : "Coating Area", nRetryCount + 1));
                _log.log(LogType.TRACE, Category.INFO, string.Format("LevelDieToGlass() - Current X: '{0}'  Programmed Loc: '{1}' ", XPos.ToString("0.000"), bPriming ? MS.PrimingMeasureLoc : MS.MeasureLoc));

                if (_abortingRecipe)
                {
                    _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Aborting Recipe Run - 2");
                    SetMemoryVal(2, 1); // notfy controller thread to abort
                    bContinue = false;
                    LevelFailReason = "Aborting";
                }

                if (bContinue)
                {
                    if (!MS.UsingKeyenceLaser || isCalibration)
                    {
                        _log.log(LogType.TRACE, Category.INFO, " [4]  Read the GT2s");
                        OpenKeyence();
                        bContinue &= ReadKeyence(ref dRight, ref ksRight, ref dLeft, ref ksLeft);
                        if (!bContinue)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Could not communicate with Keyence [4].  Aborting Level Routine.");
                            SetMemoryVal(2, 1); // notfy controller thread to abort
                            bContinue = false;
                            LevelFailReason = "Communication failure with GT2s.  See logs";
                        }
                        else
                        {
                            rGT2 = dRight;
                            lGT2 = dLeft;
                            LeftGT2Pos = dLeft;
                            RightGT2Pos = dRight;
                            _log.log(LogType.TRACE, Category.INFO, $"Before adjustment move:");
                            _log.log(LogType.TRACE, Category.INFO, $"   X-Axis Loc : {XPos.ToString("0.000")} mm");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Right : {RZ_TP.ToString()} counts");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Left  : {LZ_TP.ToString()} counts");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Right : {ZRPos.ToString("0.000")} mm");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Left  : {ZLPos.ToString("0.000")} mm");
                            _log.log(LogType.TRACE, Category.INFO, $"Keyence Right : {dRight.ToString("0.0000")} (mm) : {ksRight.ToString()}");
                            _log.log(LogType.TRACE, Category.INFO, $"Keyence Left  : {dLeft.ToString("0.0000")} (mm) : {ksLeft.ToString()}");
                        }
                    }
                    else if (LaserMgr != null)
                    {
                        _log.log(LogType.TRACE, Category.INFO, " [4]  Read the Lasers");

                        bool lasersRead = false;

                        while (!lasersRead)//Loop While Waiting on Laser Read & Not Read Yet
                        {
                            lasersRead = LaserMgr.GetLeftAndRightLaserValues(out var left, out var right, readTimestamp);

                            if (lasersRead)
                            {
                                dLeft = left;
                                dRight = right;

                                _log.log(LogType.TRACE, Category.INFO, $"Lasers Read Successful! LEFT: {dLeft:#0.000} RIGHT: {dRight:#0.000}.", "INFO");
                                LaserRetries = 0;
                            }
                            else if (!lasersRead && LaserRetries < 50)
                            {
                                LaserRetries++;
                            }
                            else if (!lasersRead && LaserRetries > 50)
                            {
                                LaserRetries = 0;
                                _abortingRecipe = true;
                                LevelFailReason = "Could Not Read Keyence Laser Values";
                                _log.log(LogType.TRACE, Category.INFO, $"Laser Read Failed... Retry Attempts: {LaserRetries}!!! ABORTING!!!", "ERROR");
                            }
                        }

                        if (!lasersRead)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Could not communicate with Keyence [4].  Aborting Level Routine.");
                            SetMemoryVal(2, 1); // notfy controller thread to abort
                            bContinue = false;
                            LevelFailReason = "Communication failure with GT2s.  See logs";
                        }
                        else
                        {
                            rGT2 = dRight;
                            lGT2 = dLeft;

                            _log.log(LogType.TRACE, Category.INFO, $"Before adjustment move:");
                            _log.log(LogType.TRACE, Category.INFO, $"   X-Axis Loc : {XPos:0.000} mm");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Right : {RZ_TP} counts");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Left  : {LZ_TP} counts");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Right : {ZRPos:0.000} mm");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Left  : {ZLPos:0.000} mm");
                            _log.log(LogType.TRACE, Category.INFO, $"Keyence Laser Right : {dRight:0.0000} (mm)");
                            _log.log(LogType.TRACE, Category.INFO, $"Keyence Laser Left  : {dLeft:0.0000} (mm)");
                        }
                    }
                }

                // double check that X hasnt drifted. Since its not 'moving', position errors dont trip system (yet)
                if (bContinue && (X_MotorOff || !IsClose(XPos, bPriming ? MS.PrimingMeasureLoc : MS.MeasureLoc, 2.0)))
                {
                    _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Aborting Recipe Run - 3a");
                    SetMemoryVal(2, 1); // notfy controller thread to abort
                    bContinue = false;
                    Exception ex1 = new Exception("X Position Error or Motor Off.  Level Routine aborted!");
                    ex1.Data.Add("Reason", X_MotorOff ? "XMotor" : "XPos");
                    ex1.Data.Add("Actual", XPos);
                    ex1.Data.Add("Expected", bPriming ? MS.PrimingMeasureLoc : MS.MeasureLoc);
                    ex1.Data.Add("Motor", X_MotorOff ? "Off" : "On");
                    throw ex1;  // note: we're done now
                }

                if (_abortingRecipe)
                {
                    _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Aborting Recipe Run - 3b");
                    SetMemoryVal(2, 1); // notfy controller thread to abort
                    bContinue = false;
                    LevelFailReason = "Aborting";
                }

                //---------------------------------------------------------------------------------------------------------------
                // Calc Move Distances.
                if (bContinue)
                {
                    // old way.. abadon until math is corrected.
                    // [5] Calculate Slope based on current locations  (rGT2,lGT2)
                    //dSlope = (rGT2 - lGT2) / (MachineSettings.RightGT2pos - MachineSettings.LeftGT2pos);
                    //rZMove = (rGT2 - dGap) + (dSlope * (MachineSettings.CrossBarWidth - MachineSettings.RightGT2pos));
                    //lZMove = (lGT2 - dGap) - (dSlope * MachineSettings.LeftGT2pos);

                    // [5] Calculate moves based on % distances  (rGT2,lGT2)
                    _log.log(LogType.TRACE, Category.INFO, string.Format("Gap : {0}   Level % {1}", dGap.ToString("0.000"), MS.LevelingMovePercentage));
                    lastRZMove = rZMove;
                    lastLZMove = lZMove;
                    rZMove = ((rGT2 - dGap) * (MS.LevelingMovePercentage / 100f));
                    lZMove = ((lGT2 - dGap) * (MS.LevelingMovePercentage / 100f));


                    // Limit to max move for measures
                    if (rZMove > (MS.MaxMeasureMove / 1000.0))
                    {
                        _log.log(LogType.TRACE, Category.INFO, string.Format("Initial Right Z move greater than Max move. Reducing! - {0} mm vs {1} mm", rZMove.ToString("0.0000"), (MS.MaxMeasureMove / 1000.0).ToString("0.000")), "Warning");
                        rZMove = (MS.MaxMeasureMove / 1000.0);
                    }
                    if (lZMove > (MS.MaxMeasureMove / 1000.0))
                    {
                        _log.log(LogType.TRACE, Category.INFO, string.Format("Initial Left Z move greater than Max move. Reducing! - {0} mm vs {1} mm", lZMove.ToString("0.0000"), (MS.MaxMeasureMove / 1000.0).ToString("0.000")), "Warning");
                        lZMove = (MS.MaxMeasureMove / 1000.0);
                    }

                    _log.log(LogType.TRACE, Category.INFO, $"{{New, Last}} Right Z Move Request :{{ {rZMove:#0.0000}, {lastRZMove:#0.0000}}} mm");
                    _log.log(LogType.TRACE, Category.INFO, $"{{New, Last}} Left Z Move Request :{{ {lZMove:#0.0000}, {lastLZMove:#0.0000}}} mm");

                    //if (Math.Abs(lZMove) < 0.001)
                    //{
                    //    lZMove = 0;
                    //    _log.log(LogType.TRACE, Category.INFO, $"Resetting needed Left Z Move to zero.  Less than 1 µm");
                    //}
                    sReturn = RunCommand($"params[16]={lZMove:#0.0000}");
                    Thread.Sleep(15);

                    if (sReturn.ToLower().Contains("error"))
                    {
                        bContinue = false;
                        _log.log(LogType.TRACE, Category.INFO, $"Setting params[16] generated error: {sReturn}");
                        _log.log(LogType.TRACE, Category.INFO, "Aborted Run");
                    }

                    //if (Math.Abs(rZMove) < 0.001)
                    //{
                    //    rZMove = 0;
                    //    _log.log(LogType.TRACE, Category.INFO, $"Resetting needed Right Z Move to zero.  Less than 1 µm");
                    //}

                    sReturn = RunCommand($"params[17]={rZMove:#0.0000}");
                    Thread.Sleep(15);

                    if (sReturn.ToLower().Contains("error"))
                    {
                        bContinue = false;
                        _log.log(LogType.TRACE, Category.INFO, $"Setting params[17] generated error: {sReturn}");
                        _log.log(LogType.TRACE, Category.INFO, "Aborted Run");
                    }

                    dNewRight = ZRPos + rZMove;
                    dNewLeft = ZLPos + lZMove;
                    _log.log(LogType.TRACE, Category.INFO, $"Target Right Z Pos : {dNewRight:#0.0000} mm");
                    _log.log(LogType.TRACE, Category.INFO, $"Max Right Z Pos : {dMaxZRight:#0.0000} mm");
                    _log.log(LogType.TRACE, Category.INFO, $"Target Left Z Pos : {dNewLeft:#0.0000} mm");
                    _log.log(LogType.TRACE, Category.INFO, $"Max Left Z Pos : {dMaxZLeft:#0.0000} mm");

                    if (dNewRight > dMaxZRight || dNewLeft > dMaxZLeft)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: FAILED.. One or both moves would violate Max Z!", "ERROR");
                        SetMemoryVal(2, 1); // notfy controller thread to abort
                        bContinue = false;
                        LevelFailReason = $"Max Z Violation - See Logs at {DateTime.Now.ToLongTimeString()} ";
                    }

                    //if (lZMove == 0 && rZMove == 0)
                    //{
                    //    _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: FAILED.. both of the moves were 0!!!", "ERROR");
                    //    SetMemoryVal(2, 1); // notfy controller thread to abort
                    //    bContinue = false;
                    //    LevelFailReason = $"Both Moves were 0.0";
                    //}
                }

                if (_abortingRecipe)
                {
                    _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Aborting Recipe Run - 4");
                    SetMemoryVal(2, 1); // notfy controller thread to abort
                    bContinue = false;
                    LevelFailReason = $"Aborting";
                }

                if (bContinue)
                {
                    _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: [6] Reset waiting, Set Continue", "INFO");
                    SetMemoryVal(3, 0);
                    Thread.Sleep(5);
                    if (!WaitOnSignal(3, 0, 1))	// 1 sec timeout
                    {
                        _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Timeout waiting on 'waiting' to reset (13)", "WARNING");
                        SetMemoryVal(2, 1); // notfy controller thread to abort
                        bContinue = false;
                        LevelFailReason = $"Could not update signal #3";
                    }
                    else
                    {
                        _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Notifying controller to make adjustment moves.", "Info");
                    }

                    // now signal continue....
                    SetMemoryVal(1, 1); // this gets us past (7) in lvldie
                    //DelayDataUpdate(250);
                }

                // now #lvl die will
                // (8) retract GT2s
                // (10) make Z move
                // (11) extend gt2s
                // (12) waits on continue (gMem[1] to be 1 or 2)
                if (_abortingRecipe)
                {
                    _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Aborting Recipe Run - 5");
                    SetMemoryVal(2, 1); // notfy controller thread to abort
                    bContinue = false;
                    LevelFailReason = $"Aborting";
                }

                if (bContinue)
                {
                    _log.log(LogType.TRACE, Category.INFO, " [7] Wait for the signal to say that we are ready to measure (controller will drop the GT2s and wait for settle)");
                    if (!WaitOnSignal(3, 1, 20))  // wait on signal from controller (20 sec timeout)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Timeout waiting on 'signal' to reset");
                        SetMemoryVal(2, 1); // notfy controller thread to abort
                        bContinue = false;
                        LevelFailReason = $"Could not update signal #4";
                    }
                }

                if (bContinue)
                {
                    readTimestamp = DateTime.Now;

                    if (!MS.UsingKeyenceLaser || isCalibration)
                    {
                        _log.log(LogType.TRACE, Category.INFO, " [8] Read the GT2s ");
                        bContinue = ReadKeyence(ref dRight, ref ksRight, ref dLeft, ref ksLeft);

                        if (!bContinue)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Could not communicate with Keyence (4).  Aborting Level Routine.");
                            SetMemoryVal(2, 1); // notfy controller thread to abort
                            bContinue = false;
                        }
                        else
                        {
                            LeftGT2Pos = dLeft;
                            RightGT2Pos = dRight;
                            _log.log(LogType.TRACE, Category.INFO, $"After adjustment move:");
                            _log.log(LogType.TRACE, Category.INFO, $"   X-Axis Loc : {XPos:0.000} mm");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Right : {RZ_TP} counts");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Left  : {LZ_TP} counts");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Right : {ZRPos:0.000} mm");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Left  : {ZLPos:0.000} mm");
                            _log.log(LogType.TRACE, Category.INFO, $"Keyence Right : {dRight:0.0000} (mm) : {ksRight}");
                            _log.log(LogType.TRACE, Category.INFO, $"Keyence Left  : {dLeft:0.0000} (mm) : {ksLeft}");
                        }
                    }
                    else if (LaserMgr != null)
                    {
                        _log.log(LogType.TRACE, Category.INFO, " [4]  Read the Lasers");

                        bool lasersRead = false;
                        while (!lasersRead)//Loop While Waiting on Laser Read & Not Read Yet
                        {
                            lasersRead = LaserMgr.GetLeftAndRightLaserValues(out var left, out var right, readTimestamp);

                            if (lasersRead)
                            {
                                dLeft = left;
                                dRight = right;
                                _log.log(LogType.TRACE, Category.INFO, $"Lasers Read Successful! LEFT: {dLeft:#0.000} RIGHT: {dRight:#0.000}.", "INFO");
                                LaserRetries = 0;
                            }
                            else if (!lasersRead && LaserRetries < 50)
                            {
                                LaserRetries++;
                            }
                            else if (!lasersRead && LaserRetries > 50)
                            {
                                LaserRetries = 0;
                                _abortingRecipe = true;
                                LevelFailReason = "Could Not Read Keyence Laser Values";
                                _log.log(LogType.TRACE, Category.INFO, $"Laser Read Failed... Retry Attempts: {LaserRetries}!!! ABORTING!!!", "ERROR");
                            }
                        }

                        if (!lasersRead)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Could not communicate with Keyence [4].  Aborting Level Routine.");
                            SetMemoryVal(2, 1); // notfy controller thread to abort
                            bContinue = false;
                            LevelFailReason = "Communication failure with GT2s.  See logs";
                        }
                        else
                        {
                            rGT2 = dRight;
                            lGT2 = dLeft;
                            _log.log(LogType.TRACE, Category.INFO, $"Before adjustment move:");
                            _log.log(LogType.TRACE, Category.INFO, $"   X-Axis Loc : {XPos:0.000} mm");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Right : {RZ_TP} counts");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Left  : {LZ_TP} counts");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Right : {ZRPos:0.000} mm");
                            _log.log(LogType.TRACE, Category.INFO, $"Encoder Left  : {ZLPos:0.000} mm");
                            _log.log(LogType.TRACE, Category.INFO, $"Keyence Laser Right : {dRight:0.0000} (mm)");
                            _log.log(LogType.TRACE, Category.INFO, $"Keyence Laser Left  : {dLeft:0.0000} (mm)");
                        }
                    }

                    // [9] Verify Height
                    dDeltaL = Math.Abs(dGap - dLeft);
                    dDeltaR = Math.Abs(dGap - dRight);

                    _log.log(LogType.TRACE, Category.INFO, $"Target Height Delta Right - {dDeltaR:0.0000} mm");
                    _log.log(LogType.TRACE, Category.INFO, $"Target Height Delta Left - {dDeltaL:0.0000} mm");

                    int nTol = isCalibration ? 0 : MS.LevelingTolerance;
                    double dTol = isCalibration ? 0.0005 : (MS.LevelingTolerance / 1000f);
                    if (dDeltaL > dTol || dDeltaR > dTol)
                    {
                        _log.log(LogType.TRACE, Category.INFO, $"LevelDieToGlass: One or More Height are out of tolerance (+/- {nTol} µm) .. go again");
                        bNeedRedo = nRetryCount < nMaxRetries;
                    }
                    else
                    {
                        _log.log(LogType.TRACE, Category.INFO, $"LevelDieToGlass: Height Deltas are within tolerance (+/- {nTol} µm) .. continue to next step");
                        bNeedRedo = false;
                    }

                    if (_abortingRecipe)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Aborting Recipe Run - 6");
                        SetMemoryVal(2, 1); // notfy controller thread to abort
                        bContinue = false;
                        LevelFailReason = $"Aborting";
                    }

                    // now figure out what to tell lvldie to do... 
                    // gMem[1] = 1  -> continue and end
                    // gMem[1] = 2  -> redo the loop, jump to #lvlcmbk

                    // [10] Set Continue (retry flag or continue flag will be set...
                    if (bContinue)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: reset waiting, set Continue");
                        SetMemoryVal(3, 0);

                        Thread.Sleep(5);

                        if (!WaitOnSignal(3, 0, 1)) // 1 sec timeout
                        {
                            _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Timeout waiting on 'waiting' to reset (13)", "Info");
                            SetMemoryVal(2, 1); // notfy controller thread to abort
                            bContinue = false;
                            LevelFailReason = $"Could not update signal #5";
                        }
                        if (bNeedRedo)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Requesting 'redo' of #lvldie routine.", "Info");
                            // gMem[1] = 3  -> redo the loop, jump to #lvlcmbk     NOTE!   This used to be '2'...  nAble HAS to match DMC code.  Check #wtcont to be sure
                            SetMemoryVal(1, 3);
                        }
                        else
                        {
                            if (nRetryCount >= nMaxRetries)
                            {
                                _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Requesting Abort due to excessive Retries", "Warning");
                                SetMemoryVal(2, 1); // notfy controller thread to abort
                                LevelFailReason = "Could not reach level within retry limit.";
                            }
                            else
                            {
                                _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Requesting Continue of #lvldie routine", "Info");
                                // gMem[1] = 1  -> continue and end
                                SetMemoryVal(1, 1);
                            }
                        }
                    }

                    nRetryCount++;
                    LevelAttempt = nRetryCount;

                }

                if (_abortingRecipe)
                {
                    _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Aborting Recipe Run - 7");
                    SetMemoryVal(2, 1); // notfy controller thread to abort
                    bContinue = false;
                    LevelFailReason = $"Aborting";
                }

            }

            _log.log(LogType.TRACE, Category.INFO, " wait for the notification that #lvldie is finished ");

            if (!WaitOnSignalBit(4, 0x4000, false, 10, ignoreAborted: true)) // wait on notication of thread stop  (10 sec timeout)
            {
                _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Timeout waiting on lvldie thread to finish on controller");
                SetMemoryVal(2, 1); // notfy controller thread to abort
                bContinue = false;
                LevelFailReason = $"Did not Receive Signal Update: '#LvlDie Finished'";
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass: Received lvldie thread finished signal");
            }

            if (bContinue && (nRetryCount <= nMaxRetries))
            {
                ////////////////////////////////////////////
                // Save the values (Priming or Coating)
                bRetVal = true;
            }

            _log.log(LogType.TRACE, Category.INFO, "LevelDieToGlass(): End");
            LevelingDie = false;
            return bRetVal;
        }

        #endregion Calibration and Leveling

        #region Keyence GT2 Operations

        public double LeftGT2Pos { get; set; } = 0;
        public double RightGT2Pos { get; set; } = 0;

        public bool OpenKeyence()
        {
            bool bRetVal = true;
            if (!_keyence.IsOpen)
            {
                _keyence.PortName = MS.KeyenceCOMPort;
                bRetVal = _keyence.OpenPort(false);
            }
            return bRetVal;
        }

        public bool CloseKeyence()
        {
            bool bRetVal = true;
            if (_keyence.IsOpen)
            {
                //_log.log(LogType.TRACE, Category.INFO, "DEBUG: Keyence was NOT open! Opening Port...", "Debug");
                bRetVal = _keyence.ClosePort();
            }
            return bRetVal;
        }

        public bool ReadKeyence(ref double dValue0, ref KeyenceState eState0, ref double dValue1, ref KeyenceState eState1, int nNumRetries = 1)
        {
            bool bRetVal = false;
            if (!_keyence.IsOpen)
            {
                //_log.log(LogType.TRACE, Category.INFO, "DEBUG: Keyence was NOT open! Opening Port...", "Debug");
                bRetVal = _keyence.OpenPort(false);
            }

            if (_keyence.IsOpen)
            {
                bRetVal = _keyence.ReadAll(ref dValue0, ref eState0, ref dValue1, ref eState1, nNumRetries);
            }

            return bRetVal;
        }

        public bool ResetKeyence()
        {
            bool bRetVal = false;
            bRetVal = _keyence.PresetAll(MS.ShimSize, Storage.SelectedBankNo);
            return bRetVal;
        }

        public void ExtendAndReadKeyence()
        {
            ReadingKeyence = false;
            bool bContinue = true;
            double dRight = 0, dLeft = 0;
            int nPgmID = 31;
            KeyenceState ksRight = KeyenceState.Unknown, ksLeft = KeyenceState.Unknown;

            try
            {
                DownloadDelays();
                SetMemoryVal(nPgmID, 0);
                if (!WaitOnSignalBit(4, 0x1000, false, 1)) // 1 sec timeout
                {
                    _log.log(LogType.TRACE, Category.INFO, "ExtendAndReadKeyence: Timeout waiting on reset of calbrat extnred flag");
                    SetMemoryVal(2, 1); // notfy controller thread to abort
                    bContinue = false;
                }
            }
            catch (Exception ex)
            {
                bContinue = false;
                _log.log(LogType.TRACE, Category.INFO, "ExtendAndReadKeyence: Exception while Waiting on Signal - " + ex.ToString());
            }
            _log.log(LogType.TRACE, Category.INFO, " (1) launch the extnred routine on controller");
            try { RunCommand("XQ #extnred, 4"); }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "ExtendAndReadKeyence: Error While launching extnred routine on controller - " + ex.Message);
                bContinue = false;
            }
            // (2) wait for the notification that #extnred is running
            if (bContinue && !WaitOnSignalBit(4, 0x1000, true, 10)) // wait on notication of thread start  (10 sec timeout)
            {
                _log.log(LogType.TRACE, Category.INFO, "ExtendAndReadKeyence: Timeout waiting on waiting on calibration thread to start on controller");
                SetMemoryVal(2, 1); // notfy controller thread to abort
                bContinue = false;
            }
            _log.log(LogType.TRACE, Category.INFO, "#extnred has started");

            _log.log(LogType.TRACE, Category.INFO, " (3) wait for the first signal to say that we are ready to measure (controller will drop the GT2s and wait for settle)");
            if (bContinue && !WaitOnSignal(3, 1, 20))  // wait on first signal from controller (20 sec timeout)
            {
                _log.log(LogType.TRACE, Category.INFO, "ExtendAndReadKeyence: Timeout waiting on 'signal' to reset (3)");
                SetMemoryVal(2, 1); // notfy controller thread to abort
                bContinue = false;
            }

            if (bContinue)
            {
                _log.log(LogType.TRACE, Category.INFO, " (12)  Read the GT2s and Left/Right Z locations   @ left high");
                bContinue = ReadKeyence(ref dRight, ref ksRight, ref dLeft, ref ksLeft);
                if (!bContinue)
                {
                    _log.log(LogType.TRACE, Category.INFO, "ExtendAndReadKeyence: Could not communicate with Keyence (12).  Aborting Calibrate.");
                    SetMemoryVal(2, 1); // notfy controller thread to abort
                    bContinue = false;
                }
                _log.log(LogType.TRACE, Category.INFO, string.Format("Encoder Right - {0} mm", ZRPos.ToString()));
                _log.log(LogType.TRACE, Category.INFO, string.Format("Encoder Left - {0} mm", ZLPos.ToString()));
                _log.log(LogType.TRACE, Category.INFO, string.Format("Keyence Right - {0} {1}", dRight, ksRight.ToString()));
                _log.log(LogType.TRACE, Category.INFO, string.Format("Keyence Left - {0} {1}", dLeft, ksLeft.ToString()));
            }


            if (bContinue)
            {
                _log.log(LogType.TRACE, Category.INFO, " (13)(14) reset waiting, set Continue");
                SetMemoryVal(3, 0);
                if (!WaitOnSignal(3, 0, 1)) // 1 sec timeout
                {
                    _log.log(LogType.TRACE, Category.INFO, "ExtendAndReadKeyence: Timeout waiting on 'waiting' to reset (13)");
                    SetMemoryVal(2, 1); // notfy controller thread to abort
                    bContinue = false;
                }
                SetMemoryVal(1, 1);
            }

            _log.log(LogType.TRACE, Category.INFO, " (19) wait for the notification that #extnred is finished ");
            if (bContinue && !WaitOnSignalBit(4, 0x1000, false, 15)) // wait on notication of thread start  (15 sec timeout)
            {
                _log.log(LogType.TRACE, Category.INFO, "ExtendAndReadKeyence: Timeout waiting on waiting on extnred thread to finish on controller");
                SetMemoryVal(2, 1); // notfy controller thread to abort
                bContinue = false;
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "ExtendAndReadKeyence: Received calibration thread finished signal");
            }

            CloseKeyence();

            ReadingKeyence = false;
        }
        public bool ResetAllGT2PresetValues()
        {
            return _keyence.ResetAll();
        }
        public double ReadPresetValue(int bankNo)
        {
            return _keyence.ReadPresetValue(bankNo);
        }
        public bool PresetAll(double newPresetVal, int bankNo)
        {
            return _keyence.PresetAll(newPresetVal, bankNo);
        }
        public bool SetGT2Data(int dataID, int val)
        {
            return _keyence.SetDataValue(dataID, val);
        }
        public bool SetGT2Data(int dataID, double val)
        {
            return _keyence.SetDataValue(dataID, val);
        }
        public string ReadGT2Data(int dataID)
        {
            return _keyence.ReadDataValue(dataID);
        }
        internal bool SetBankNumber(int bankNo)
        {
            _log.log(LogType.TRACE, Category.INFO, $"Keyence: Setting Bank Number: {bankNo}", "INFO");
            bool bContinue = _keyence.ChangeBankNumber(bankNo);
            _log.log(LogType.TRACE, Category.INFO, $"Keyence: Bank Set Complete.", "INFO");
            if (bContinue)
            {
                Storage.SelectedBankNo = bankNo;
                Storage.Save();
                _log.log(LogType.TRACE, Category.INFO, $"Machine Storage Saved.", "INFO");
            }
            _log.log(LogType.TRACE, Category.INFO, $"Keyence: Returning Bank No: {bankNo}.", "INFO");
            return Storage.SelectedBankNo == bankNo;
        }
        internal int GetBankNumber()
        {
            int bankNo = _keyence.ReadCurrentBankNumber(GT2.Left);
            _log.log(LogType.TRACE, Category.INFO, $"Keyence: Current Bank Number = {bankNo}", "INFO");
            return bankNo;
        }

        #endregion Keyence Operations

        #region Keyence Laser Operations

        public ILaserManager LaserMgr { get; set; } = null;
        public bool ExclusiveNetworkUsage { get; private set; } = false;

        public void ConnectKeyenceLaser()
        {
            try
            {
                if (!MS.UsingKeyenceLaser)
                {
                    return;
                }

                if (LaserMgr is null)
                {
                    LaserMgr = LaserManagerFactory.CreateLaserManager(_log, MS, IsDemo);
                }

                if (LaserMgr is null)
                {
                    // If still null, then there is nothing we can do here.
                    return;
                }

                LaserMgr.EnableAutoConnect = true;
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.ALARM, $"Exception caught trying to initialize the Keyence Lasers:  {ex.Message}");
                LaserMgr = null;
                throw;
            }
        }

        #endregion

        #region Dual Pump Functions

        public int SelectedPump
        {
            get
            {
                int retVal = -1;
                if (IsDemo)
                    retVal = -1;
                else
                    if (Connected) { retVal = (int)_objGalil.commandValue("MGparams[110]"); }
                return retVal;
            }
        }
        public double PumpARatio
        {
            get
            {
                double retVal = -1;
                if (IsDemo)
                    retVal = -1;
                else
                    if (Connected) { retVal = _objGalil.commandValue("MGparams[111]"); }
                return retVal;
            }
        }
        public bool SetControllerPumpUsed(int Opmpa1pmpb2mix)
        {
            if (Opmpa1pmpb2mix > 2 || !Connected)
                return false;

            RunCommand($"params[110]={Opmpa1pmpb2mix}");
            return true;
        }

        #endregion

        #region RIO Automation
        public void SetReadyToLoad() => SetRIOOut(0, true);
        public void ResetReadyToLoad() => SetRIOOut(0, false);
        public void SetReadyToUnload() => SetRIOOut(1, true);
        public void ResetReadyToUnload() => SetRIOOut(1, false);
        public void SetOKToLDULD() => SetRIOOut(2, true);
        public void ResetOKToLDULD() => SetRIOOut(2, false);

        public bool ReadyToLoad => RIO_Outputs[0];
        public bool ReadyToUnload => RIO_Outputs[1];
        public bool OKToLDULD => RIO_Outputs[2];
        public bool UnloadRequest => RIO_Inputs[0];
        public bool UnloadComplete => RIO_Inputs[1];
        public bool LoadRequest => RIO_Inputs[2];
        public bool LoadComplete => RIO_Inputs[3];
        public bool AbortLDULDReq => RIO_Inputs[4];

        public bool RobotLoading = false;

        public bool RobotUnloading = false;

        public bool RobotLoadFailure = false;
        
        public bool RobotUnloadFailure = false;

        public void ClearLDULDFailure()
        {
            RobotLoadFailure = false;
            RobotUnloadFailure = false;
        }


        #endregion

        #region Galil Operations

        public void SetMachineSettings(MachineSettingsII machSettings, MachineStorage storage)
        {
            MS = machSettings;
            Storage = storage;
            if (!MS.UsingKeyenceLaser)
            {
                _keyence.PortName = MS.KeyenceCOMPort;
            }
            _DiePressureSampleValue = 0;
            _DiePressureSampleQty = Storage.PressureSmoothingSamples;
        }

        private bool InitializeGalil()
        {
            bool bRetVal = false;

            if (_objGalil == null)
            {
                try
                {
                    _objGalil = new Galil.Galil();
                    _objGalil.onInterrupt += new Events_onInterruptEventHandler(OnInterrupt);
                    _objGalil.onMessage += new Events_onMessageEventHandler(OnMessage);
                    _objGalil.onRecord += new Events_onRecordEventHandler(OnRecord);
                    bRetVal = true;
                }
                catch (COMException exCOM)
                {
                    _log.log(LogType.TRACE, Category.INFO, "ERROR: Could not create Galil Object!!!", "ERROR");
                    _log.log(LogType.TRACE, Category.INFO, exCOM.ToString());
                }
            }

            if (bRetVal)
            {
                _log.log(LogType.TRACE, Category.INFO, "New Galil Object created: " + _objGalil.libraryVersion(), "INFO");
            }
            return bRetVal;
        }
        public void StopDataRecord()
        {
            _objGalil.recordsStart(0);
            _log.log(LogType.TRACE, Category.INFO, "Galil Data Record Stopped");
            DataRecordRunning = false;
        }
        public void StartDataRecord()
        {
            _dtLastRecord = DateTime.Now;
            _objGalil.recordsStart(MS.RecordSendRate);
            _log.log(LogType.TRACE, Category.INFO, $"Galil Data Record Started: {MS.RecordSendRate} ms");
            DataRecordRunning = true;
        }
        public bool Connect(string sAddress)
        {
            bool bRetVal = false;
            string sAddr = "OFFLINE";
            if (IsDemo)
            {
                sAddr = "DEMO";
                //gVer = "4.02f"; coatVer = "5.08"; seg1Ver = "1.09"
                CodeVersion = "4.02g";
                _log.log(LogType.TRACE, Category.INFO, "nRad Firmware Version: " + CodeVersion);
                CoatVersion = "5.08";
                _log.log(LogType.TRACE, Category.INFO, "nRad Coating Version: " + CoatVersion);
                IsSetup = true;
                ConnectionInfo = MS.DualPumpInstalled ? MS.HasLoader ? "DMC4080, Rev 1.3a-SIN, 12345" : "DMC4070, Rev 1.3a-SIN, 12345" : MS.HasLoader ? "DMC4080, Rev 1.3a-SIN, 12345" : "DMC4050, Rev 1.3a-SIN, 12345";

                Memory[6] = 2;
                Outputs[8] = 1;
                Analogs[1] = 1.6;
                Analogs[2] = .07985; // main vac
                Analogs[3] = 1.25; //priming vac
                Analogs[4] = 1.25; //zone 1 vac
                Analogs[5] = 1.25; //zone 2 vac
                Analogs[6] = 1.25; //zone 3 vac
                Analogs[7] = 0; //resv
                Analogs[8] = 0; //Die Pressure Transducer

                //Inputs[8] = 0.0;
                //int nFlag = 0x200;
                //Memory[47] = (int)Memory[47] | nFlag;


                X_TP = 0;
                RZ_TP = 0;
                LZ_TP = -240000;
                PumpA_TP = 0;
                PumpB_TP = 0;
                Loader_TP = 0;
                X_TD = 0;
                RZ_TD = 0;
                LZ_TD = 0;
                PumpA_TD = 0;
                PumpB_TD = 0;
                Loader_TD = 0;
                _bDisconnected = false;
                _bConnecting = false;
                _bResetting = false;
                DataUpdate(true);
                IsSetup = true;
                FirstRecordReceived = true;
                return true;
            }
            try
            {
                FirstRecordReceived = false;
                _bDisconnected = true;
                _bConnecting = true;
                _bResetting = false;
                if (_objGalil == null)
                {
                    InitializeGalil();
                }
                //if (!nTactUtils.Ping(sAddress))
                //{
                //	_log.log(LogType.TRACE, Category.INFO, $"Could not Ping Galil Motion Controller @ addr: {sAddress}", "ERROR");
                //	_sConnectionInfo = "Not Connected";
                //	_bConnecting = false;
                //	bRetVal = false;
                //}
                //else if (_objGalil != null)
                if (_objGalil != null)
                {
                    if (_objGalil.address != "OFFLINE" && _objGalil.address != "")
                    {
                        FirstRecordReceived = false;
                        try { _objGalil.address = "OFFLINE"; }
                        catch (COMException) { }
                        ConnectionInfo = "Not Connected";
                        OnConnectionLost?.Invoke(this, new EventArgs());
                    }
                    try
                    {
                        _log.log(LogType.TRACE, Category.INFO, "Address currently specified is: " + sAddress, "INFO");
                        sAddr = sAddress;
                        _log.log(LogType.TRACE, Category.INFO, "Attempting connection to : " + sAddr, "INFO");
                        //_objGalil.timeout_ms = 10000;
                        bool bRetry = true;
                        int nTryCount = 0, nNumRetries = 5;
                        while (bRetry)
                        {
                            try
                            {
                                _objGalil.address = sAddr;
                                bRetry = false;
                            }
                            catch (COMException comex)
                            {
                                nTryCount++;
                                _log.log(LogType.TRACE, Category.INFO, "Connection Failed : " + comex.Message, "INFO");
                                if (comex.Message.Contains("1011 TIMEOUT ERROR"))
                                {
                                    bRetry = nTryCount < nNumRetries;
                                }
                                else
                                {
                                    bRetry = false;
                                }
                            }
                        }
                        _bConnecting = false;
                        _bDisconnected = _objGalil.address == "OFFLINE" || _objGalil.address == "";
                        ConnectionInfo = _objGalil.connection();
                        _log.log(LogType.TRACE, Category.INFO, "Connected to Galil 4000 - " + ConnectionInfo, "INFO");
                        RunCommand("EI 32768,128");
                        RunCommand($"setting[0]={MS.MinAirPressure}");
                        RunCommand($"setting[9]={MS.NumChuckAirVacZones}");
                        RunCommand(string.Format("devices[1]={0}", MS.HasLoader ? 1 : 0));
                        RunCommand(string.Format("devices[2]={0}", MS.UsingKeyenceLaser ? 1 : 0));
                        RunCommand(string.Format("devices[3]={0}", MS.AlignersEnabled ? 1 : 0));
                        RunCommand(string.Format("devices[4]={0}", (MS.LiftPinsEnabled || MS.HasLiftAndCenter) ? 1 : 0));
                        RunCommand(string.Format("devices[5]={0}", MS.AirKnifeInstalled ? 1 : 0));
                        RunCommand(string.Format("devices[6]={0}", MS.HasIRTransmitter ? 1 : 0));
                        RunCommand(string.Format("devices[7]={0}", MS.DualPumpInstalled ? 1 : 0));
                        RunCommand(string.Format("devices[8]={0}", MS.DualZoneLiftPinsEnabled ? 1 : 0));
                        RunCommand(string.Format("devices[9]={0}", MS.HasLiftAndCenter ? 1 : 0));

                        //Set Smart Valve Home Offset + Speed (PICO)
                        RunCommand($"gMem[38]={MS.ValveSpeed.ToString("#0000")}");
                        RunCommand($"gMem[39]={MS.ValveOffset.ToString("#0000")}");
                        //HomeSmartValve(true);

                        object objRecord = _objGalil.record("QR"); //get a record using QR
                        _dtLastRecord = DateTime.Now;
                        if (objRecord != null)
                        {
                            OnRecord(objRecord);
                            Inputs[5] = _objGalil.sourceValue(objRecord, "@IN[05]");
                            Inputs[8] = _objGalil.sourceValue(objRecord, "@IN[08]");
                            TA3 = _objGalil.sourceValue(objRecord, "_TA3");   // SHOULD be 0
                            if (_bHasAnalogInputs)
                            {
                                MainAirPressure = _objGalil.sourceValue(objRecord, "@AN[1]") * 50;  // Main Air Pressure
                                _log.log(LogType.TRACE, Category.INFO, string.Format("Air Pressure on connect - {0} PSI", MainAirPressure), "INFO");
                            }
                        }
                        else
                        {
                            Inputs[8] = _objGalil.commandValue("MG @IN[08]"); // SHOULD be 1
                            TA3 = _objGalil.commandValue("MG _TA3");   // SHOULD be 0
                            if (_bHasAnalogInputs)
                            {
                                MainAirPressure = _objGalil.commandValue("MG@AN[1]") * 50;  // Main Air Pressure
                                _log.log(LogType.TRACE, Category.INFO, string.Format("Polled Air Pressure on connect - {0} PSI", MainAirPressure), "INFO");
                            }
                        }
                        _bCommutating = false;

                        DataUpdate(true);

                        StopDataRecord(); //_objGalil.recordsStart(0);
                        System.Threading.Thread.Sleep(250);
                        StartDataRecord(); // _objGalil.recordsStart(_MachineSettings.RecordSendRate);
                        bRetVal = true;
                        try
                        {
                            CodeVersion = _objGalil.command("MGgVer{Sn}");
                            _log.log(LogType.TRACE, Category.INFO, "nRad Firmware Version: " + CodeVersion);
                            CoatVersion = _objGalil.command("MGcoatVer{Sn}");
                            _log.log(LogType.TRACE, Category.INFO, "nRad2 Coating Version: " + CoatVersion);
                            IsSetup = true;
                        }
                        catch (Exception ex)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "ERROR Could not query gVer variable - " + ex.Message, "ERROR");
                            IsSetup = false;
                        }
                        // Notify the form that the connection is up...
                        OnConnection?.Invoke(this, new EventArgs());
                    }
                    catch (Exception ex)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "ERROR during connect: " + ex.ToString(), "ERROR");

                        try { _objGalil.address = "OFFLINE"; }
                        catch (Exception) { }
                        ConnectionInfo = "Not Connected";
                        bRetVal = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "ERROR during connect: " + ex.ToString());
            }
            return bRetVal;
        }
        public bool ReConnect(string sAddress)
        {
            _bDisconnected = true;
            Reconnecting = true;
            bool bRetVal = true;
            string sAddr = "OFFLINE";
            if (_objGalil != null)
            {
                try
                {
                    _objGalil.address = "OFFLINE";
                }
                catch (Exception)
                {
                }

                try
                {
                    _log.log(LogType.TRACE, Category.INFO, "Address currently specified is: " + sAddress, "INFO");
                    sAddr = sAddress;
                    _log.log(LogType.TRACE, Category.INFO, "Attempting reconnection to : " + sAddr, "INFO");
                    bool bRetry = true;
                    int nTryCount = 0, nNumRetries = 5;
                    while (bRetry)
                    {
                        try
                        {
                            _objGalil.address = sAddr;
                            bRetry = false;
                        }
                        catch (COMException comex)
                        {
                            nTryCount++;
                            _log.log(LogType.TRACE, Category.INFO, "Reconnection (_objGalil.address = " + sAddr + ") Failed : " + comex.Message, "INFO");
                            if (comex.Message.Contains("1011 TIMEOUT ERROR"))
                            {
                                bRetry = nTryCount < nNumRetries;
                            }
                            else
                            {
                                bRetry = false;
                                bRetVal = false;
                            }
                        }
                    }
                    _bConnecting = false;
                    if (bRetVal)
                    {
                        try
                        {
                            ConnectionInfo = _objGalil.connection();
                        }
                        catch (Exception ex)
                        {
                            bRetVal = false;
                            _log.log(LogType.TRACE, Category.INFO, "Reconnection (_sConnectionInfo = _objGalil.connection()) Failed : " + ex.Message, "INFO");
                        }
                        System.Threading.Thread.Sleep(250);
                        try
                        {
                            StartDataRecord(); //_objGalil.recordsStart(_MachineSettings.RecordSendRate);
                        }
                        catch (Exception ex)
                        {
                            bRetVal = false;
                            _log.log(LogType.TRACE, Category.INFO, "Reconnection (_objGalil.recordsStart(_MachineSettings.RecordSendRate)) Failed : " + ex.Message, "INFO");
                        }
                    }
                }
                catch (Exception ex)
                {
                    bRetVal = false;
                    _log.log(LogType.TRACE, Category.INFO, "Reconnection Failed : " + ex.Message, "INFO");
                }
            }
            else
            {
                bRetVal = false;
            }
            if (bRetVal)
            {
                _log.log(LogType.TRACE, Category.INFO, "Reconnection Successful!", "INFO");
            }
            _bDisconnected = _objGalil.address == "OFFLINE" || _objGalil.address == "";
            Reconnecting = false;
            return bRetVal;
        }

        public bool Disconnect()
        {
            bool bRetval = false;

            if (_objGalil != null)
            {
                try
                {
                    _bDisconnected = true;
                    NotifyOffline();

                    if (Connected)
                    {
                        try
                        {
                            StopDataRecord(); //_objGalil.recordsStart(0); 
                        }
                        catch (Exception) { }

                        System.Threading.Thread.Sleep(250);
                        //_log.log(LogType.TRACE, Category.INFO, "Releasing Galil Connection.", "DEBUG");
                        try { _objGalil.address = "OFFLINE"; }

                        catch (Exception ex)
                        {
                            if (!ex.Message.StartsWith("5001"))
                            {
                                _log.log(LogType.TRACE, Category.ERROR, $"ERROR Did not receive expected result from disconnect - {ex.Message}");
                            }
                        }

                        _objGalil = null;
                        //_log.log(LogType.TRACE, Category.INFO, "Galil Connection removed.", "DEBUG");
                    }
                }
                catch (Exception) { }

                ConnectionInfo = "Not Connected";
            }

            InitializeGalil();
            return bRetval;
        }

        internal bool Reset()
        {
            bool bRetVal = false;
            if (Connected)
            {
                _bResetting = true;
                IsSetup = false;
                LastError = "";
                try
                {
                    RunCommand("HX1"); // stop monitor thread
                    StopDataRecord(); //_objGalil.recordsStart(0);	// shut down data collection;
                    Thread.Sleep(MS.RecordSendRate * 2);
                }
                catch (Exception) { }
                try
                {
                    RunCommand("OE 0,0,0,0,0");
                    RunCommand("HX;CB6;CB7;ST;MO;"); // stop all threads, set the Z brakes, Motor off all axis;
                    if (ELOLatched)
                        RunCommand("XQ#clrelo,7");
                    Thread.Sleep(500);
                    RunCommand("RS");
                }
                catch (Exception ex)
                {
                    _log.log(LogType.TRACE, Category.INFO, "Exception while performing PLC Reset: " + ex.Message);
                    LastError = ex.Message;
                }
                Disconnect();
                bRetVal = true;
            }
            else
            {
                LastError = "Not connected";
            }
            return bRetVal;
        }
        internal bool SoftReset()
        {
            bool bRetVal = false;
            try
            {
                RunCommand("XQ#reset,7");
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception while performing Motion Control Soft Reset: " + ex.Message);
                LastError = ex.Message;
            }
            return bRetVal;
        }
        public void DelayDataUpdate(int nMSecs)
        {
            _dtNextAllowedQU = DateTime.Now.AddMilliseconds(nMSecs);
        }
        public void DataUpdate(bool SkipTimeCheck = false, bool bForce = false)
        {
            _tsUpdate = DateTime.Now - _dtLastRecord;
            string dataUpdateStep = "";
            if (IsDemo)
            {
                Inputs[0] = 0; //   [0 == active  |  1 == Inactive]
                Inputs[1] = 1;
                Inputs[2] = 1;
                Inputs[3] = 0;
                Inputs[4] = 1;
                Inputs[5] = 0;
                Inputs[6] = 1;
                Inputs[7] = 1;
                Inputs[8] = 1;
                Inputs[9] = 0;
                Inputs[10] = 1;
                Inputs[11] = 0;
                Inputs[12] = 1;

                Homed[0] = 1.0;
                Homed[1] = 1.0;
                Homed[2] = 1.0;
                Homed[3] = 1.0;
                Homed[4] = 1.0;
                Homed[5] = 1.0;
                Homed[6] = 1.0;
                Homed[7] = 1.0;

                LimitFlag[0] = 0.0;
                LimitFlag[1] = 0.0;
                LimitFlag[2] = 0.0;
                LimitFlag[3] = 0.0;
                LimitFlag[4] = 0.0;
                LimitFlag[5] = 0.0;
                LimitFlag[6] = 0.0;
                LimitFlag[7] = 0.0;
            }
            else if (Connected && (!_bCommutating || bForce))
            {
                if (_tsUpdate.TotalSeconds > 10 && !SkipTimeCheck && !_bResetting && DataRecordRunning)
                {
                    _log.log(LogType.TRACE, Category.INFO, $"DataUpdate detected Disconnect.  Last Record received {_tsUpdate.TotalSeconds} seconds ago");
                    _bDisconnected = true;
                    try
                    {
                        _objGalil.address = "OFFLINE";
                    }
                    catch (COMException comex)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "Message from disconnect: " + comex.Message);
                    }
                    OnConnectionLost?.Invoke(this, new EventArgs());
                    ClearInputs();
                    return;
                }
                try
                {
                    if (!_bReadingArray && DateTime.Now > _dtNextAllowedQU)
                    {
                        Monitor.Enter(_CommandLock);

                        _bReadingArray = true;
                        int nIdx;
                        dataUpdateStep = "gMem";
                        Array arrayUp = (Array)_objGalil.arrayUpload("gMem");  //upload controller array "a" to local host array.
                        for (nIdx = 0; nIdx < arrayUp.Length; nIdx++)
                            Memory[nIdx] = (double)arrayUp.GetValue(nIdx);
                        AddDiePressureForSmoothing(DiePressurePSI, Analogs[8]); // +MachineSettings.DiePressureInputVoltageAdjust);

                        if(Settings.Default.HasRIOAutomation)
                        {
                            dataUpdateStep = "rioIn";
                            arrayUp = (Array)_objGalil.arrayUpload("rioIn");
                            for (nIdx = 0; nIdx < 2; nIdx++)
                                _dRIO_IO_Buffer[nIdx] = (double)arrayUp.GetValue(nIdx);
                            for (nIdx = 0; nIdx < 8; nIdx++)
                            {

                                RIO_Inputs[nIdx] = !(((int)_dRIO_IO_Buffer[0] | (int)Math.Pow(2, (nIdx))) == _dRIO_IO_Buffer[0]);
                                RIO_Inputs[nIdx + 8] = !(((int)_dRIO_IO_Buffer[1] | (int)Math.Pow(2, (nIdx))) == _dRIO_IO_Buffer[1]);
                            }

                            dataUpdateStep = "rioOut";
                            arrayUp = (Array)_objGalil.arrayUpload("rioOut");
                            for (nIdx = 0; nIdx < 2; nIdx++)
                                _dRIO_IO_Buffer[nIdx] = (double)arrayUp.GetValue(nIdx);
                            for (nIdx = 0; nIdx < 8; nIdx++)
                            {

                                RIO_Outputs[nIdx] = ((int)_dRIO_IO_Buffer[0] | (int)Math.Pow(2, (nIdx))) == _dRIO_IO_Buffer[0];
                                RIO_Outputs[nIdx + 8] = ((int)_dRIO_IO_Buffer[1] | (int)Math.Pow(2, (nIdx))) == _dRIO_IO_Buffer[1];
                            }
                        }
                        LastCommandErrorLine = ((int)Memory[10]).ToString();
                        LastCommandError = ErrorCodes.ContainsKey((int)Memory[11]) ? ErrorCodes[(int)Memory[11]].ToString() : $"Command Error: {((int)Memory[11]).ToString()}";
                        _bReadingArray = false;

                        Monitor.Exit(_CommandLock);
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Exit(_CommandLock);
                    if (!_bResetting)
                    {
                        _log.log(LogType.TRACE, Category.INFO, $"Error Reading {dataUpdateStep} array: " + ex.Message);
                        ReConnect(Address);
                    }
                    _bReadingArray = false;
                }
            }
        }
        internal bool StartRecords()
        {
            bool bRetVal = false;
            try
            {
                _log.log(LogType.TRACE, Category.INFO, "PLC: Starting Record Streaming", "INFO");
                StartDataRecord(); //_objGalil.recordsStart(MachineSettings.RecordSendRate);
                bRetVal = true;
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Error Issue StartRecords Command: " + ex.ToString(), "ERROR");
            }
            return bRetVal;
        }
        private void OpenDataLog()
        {
            if (CurrentRecipe == null)
            {
                _log.log(LogType.TRACE, Category.INFO, "Aborting Open of Data Logger() - No Recipe Selected", "ERROR");
                return;
            }
            if (_bDataLogActive)
            {
                Debug.WriteLine("Aborting Open of Data Logger() - already running.", "DEBUG");
                return;
            }

            _log.log(LogType.TRACE, Category.INFO, "Starting Data Logger ", "INFO");

            DateTime currentTime = DateTime.Now;
            _swDataLogStopwatch.Reset();
            Directory.CreateDirectory("DataLogs");
            string sLogFileName = $"{currentTime.ToString("yyyyMMdd HH-mm-ss(tt)")}.log";
            string sLogFilePath = Path.Combine("DataLogs", sLogFileName);

            DataLogFile = new StreamWriter(sLogFilePath);
            DataLogFile.WriteLine($"Recipe Run Date:, {currentTime.ToString("F")}, ,{currentTime.ToString("u")}");
            DataLogFile.WriteLine($"Recipe Name:, {CurrentRecipe.Name}");
            double dPumpVol = POHPumpDetected ? MS.POHVol : MS.SyringeVol;
            DataLogFile.WriteLine($"Dispense Rate:, {CurrentRecipe.DispenseRate}, µl/s, ,Conversion Factor:, {uLConv},, Pump Vol:,{dPumpVol.ToString("0.00")},ml");
            DataLogFile.WriteLine($"Coating Vel:, {CurrentRecipe.CoatingVel}, mm/s");
            DataLogFile.WriteLine($"Coat Gap:, {CurrentRecipe.CoatHeight}, µm");
            StringBuilder sb = new StringBuilder(128);
            sb.Append($"Time Stamp,");
            sb.Append($"Air Press.,");
            sb.Append($"X Position (mm),");
            sb.Append($"X Velocity (mm/s),");
            sb.Append($"X Position Error (mm),");
            sb.Append($"Z Right Position (mm),");
            sb.Append($"Z Right Velocity (mm/s),");
            sb.Append($"Z Right Position Error (mm),");
            sb.Append($"Z Left Position (mm),");
            sb.Append($"Z Left Velocity (mm/s),");
            sb.Append($"Z Left Position Error (mm),");
            sb.Append($"Pump Volume Dispensed (µl),");
            sb.Append($"Pump Velocity (µl/s),");
            sb.Append($"Pump Positon Error (µl),");
            sb.Append($"Pump Torque Load (VDC),");
            if (MS.ChuckTempControlEnabled)
                sb.Append($"Chuck Temp (°C),");
            if (MS.DieTempControlEnabled)
                sb.Append($"Die Temp (°C),");
            if (MS.ReservoirTempControlEnabled)
                sb.Append($"Resv. Temp (°C),");
            if (MS.HasDiePressureTransducer)
                sb.Append($"Die Pressure (PSI),");
            DataLogFile.WriteLine(sb.ToString());

            _swDataLogStopwatch.Start();

            _bDataLogActive = true;
        }
        private void CloseDataLog()
        {
            if (_bDataLogActive)
            {
                _log.log(LogType.TRACE, Category.INFO, "Closing Data Logger", "INFO");
            }
            _bDataLogActive = false;
            _swDataLogStopwatch.Stop();
            DataLogFile?.Close();
        }
        public string RunCommand(string sCommand)
        {
            string sRetVal = "";
            if (Connected)
            {
                lock (_CommandLock) // interlock for 
                {
                    try
                    {
                        _log.log(LogType.TRACE, Category.INFO, string.Format("PLC Running Command: '{0}'", sCommand));
                        sRetVal = _objGalil.command(sCommand);
                    }
                    catch (COMException comex)
                    {
                        sRetVal = "ERROR: " + comex.Message;
                        _log.log(LogType.TRACE, Category.INFO, "Could not run command: " + comex.Message, "ERROR");
                    }
                }
            }
            else
            {
                sRetVal = "Not Connected";
            }
            return sRetVal;
        }

        public void SetDigitalOut(int index, bool on)
        {
            double onVal = on ? 1.0 : 0;

            if (Connected && Outputs[index] != onVal)
            {
                string cmd = on ? $"SB{index}" : $"CB{index}";
                RunCommand(cmd);
            }
        }
        public void SetRIOOut(int index, bool state)
        {
            int rioAddr = 8000 + index;
            if (Connected && RIO_Outputs[index] != state)
            {
                string cmd = state ? $"SB{rioAddr}" : $"CB{rioAddr}";
                RunCommand(cmd);
            }
        }

        internal void ToggleDigitalOut(int index)
        {
            SetDigitalOut(index, Outputs[index] == 0);
        }

        internal void ToggleIO(int nNum)
        {
            if (Connected)
            {
                string sCmd;
                if (Connected)
                {
                    if (Outputs[nNum] == 1.0)
                        sCmd = "CB" + nNum.ToString();
                    else
                        sCmd = "SB" + nNum.ToString();
                    RunCommand(sCmd);
                }
            }
        }
        internal bool DownloadDelays()
        {
            bool downloaded = false;

            StringBuilder sb = new StringBuilder("");
            sb.Append($"delays[0]={MS.LiftPinsUpDelay};");
            sb.Append($"delays[1]={MS.LiftPinsDownDelay};");
            sb.Append($"delays[2]={MS.AlignersUpDelay};");
            sb.Append($"delays[3]={MS.AlignersDownDelay};");
            sb.Append($"delays[4]={MS.GT2SettleDelay};");
            sb.Append($"delays[5]={MS.AirPuffDelay};");
            sb.Append($"delays[7]={MS.PrimePostDelay};");
            sb.Append($"delays[8]={MS.RotaryValveMoveDelay};");

            try
            {
                RunCommand(sb.ToString());
                downloaded = true;
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception while updating Delays: " + ex.Message, "ERROR");
            }

            return downloaded;
        }

        internal bool DownloadTransferSettings()
        {
            bool bReturn = false;
            RunCommand(string.Format("params[40]={0}", MS.ConvHomeOffset));
            RunCommand(string.Format("params[41]={0}", MS.ConvCoaterPosition));
            RunCommand(string.Format("params[42]={0}", MS.ConvUnloadPosition));
            RunCommand(string.Format("params[43]={0}", MS.ConvVelocity));
            RunCommand(string.Format("params[44]={0}", MS.ConvAccDec));
            RunCommand(string.Format("params[45]={0}", MS.ConvSCurve / 100.0));
            return bReturn;
        }
        internal bool CallInitAll()
        {
            bool bRetVal = false;
            if (Connected)
            {
                LastError = "";
                try
                {
                    double cmdRetVal = _objGalil.commandValue("MG _HX2");
                    if (cmdRetVal == 1)
                    {
                        _objGalil.command("HX2");
                        _log.log(LogType.TRACE, Category.INFO, "ERROR while performing PLC Init request. Thread 2 already running");
                    }

                    //Inform Controller of Installed Devices
                    RunCommand($"devices[9]={(MS.HasLiftAndCenter ? 1 : 0)}");
                    RunCommand($"devices[8]={(MS.DualZoneLiftPinsEnabled ? 1 : 0)}");
                    RunCommand($"devices[7]={(MS.DualPumpInstalled ? 1 : 0)}");
                    RunCommand($"devices[6]={(MS.IRLampInstalled ? 1 : 0)}");
                    RunCommand($"devices[5]={(MS.UsingKeyenceLaser ? 1 : 0)}");
                    RunCommand($"devices[4]={((MS.LiftPinsEnabled || MS.HasLiftAndCenter) ? 1 : 0)}");
                    RunCommand($"devices[3]={(MS.AlignersEnabled ? 1 : 0)}");
                    RunCommand($"devices[2]={(MS.AirKnifeInstalled ? 1 : 0)}");
                    RunCommand($"devices[1]={(MS.HasLoader ? 1 : 0)}");

                    // Run Time tuning parameters. 
                    RunCommand($"tuning[0]={MS.X_KP}");
                    RunCommand($"tuning[1]={MS.X_KI}");
                    RunCommand($"tuning[2]={MS.X_KD}");
                    RunCommand($"tuning[3]={MS.X_FV}");
                    RunCommand($"tuning[4]={MS.X_FA}");
                    RunCommand($"tuning[5]={MS.X_PL}");
                    RunCommand($"tuning[6]={MS.ZR_KP}");
                    RunCommand($"tuning[7]={MS.ZR_KI}");
                    RunCommand($"tuning[8]={MS.ZR_KD}");
                    RunCommand($"tuning[9]={MS.ZR_FV}");
                    RunCommand($"tuning[10]={MS.ZR_FA}");
                    RunCommand($"tuning[11]={MS.ZR_PL}");
                    RunCommand($"tuning[12]={MS.ZL_KP}");
                    RunCommand($"tuning[13]={MS.ZL_KI}");
                    RunCommand($"tuning[14]={MS.ZL_KD}");
                    RunCommand($"tuning[15]={MS.ZL_FV}");
                    RunCommand($"tuning[16]={MS.ZL_FA}");
                    RunCommand($"tuning[17]={MS.ZL_PL}");
                    RunCommand($"tuning[18]={MS.SyringeA_KP}"); //Syringe-A
                    RunCommand($"tuning[19]={MS.SyringeA_KI}"); //Syringe-A
                    RunCommand($"tuning[20]={MS.SyringeA_KD}"); //Syringe-A
                    RunCommand($"tuning[21]={MS.SyringeA_FV}"); //Syringe-A
                    RunCommand($"tuning[22]={MS.SyringeA_FA}"); //Syringe-A
                    RunCommand($"tuning[23]={MS.SyringeA_PL}"); //Syringe-A
                    RunCommand($"tuning[24]={MS.POHA_KP}"); //POH-A
                    RunCommand($"tuning[25]={MS.POHA_KI}"); //POH-A
                    RunCommand($"tuning[26]={MS.POHA_KD}"); //POH-A
                    RunCommand($"tuning[27]={MS.POHA_FV}"); //POH-A
                    RunCommand($"tuning[28]={MS.POHA_FA}"); //POH-A
                    RunCommand($"tuning[29]={MS.POHA_PL}"); //POH-A
                    RunCommand($"tuning[30]={MS.SyringeB_KP}");
                    RunCommand($"tuning[31]={MS.SyringeB_KI}");
                    RunCommand($"tuning[32]={MS.SyringeB_KD}");
                    RunCommand($"tuning[33]={MS.SyringeB_FV}");
                    RunCommand($"tuning[34]={MS.SyringeB_FA}");
                    RunCommand($"tuning[35]={MS.SyringeB_PL}");
                    RunCommand($"tuning[36]={MS.POHB_KP}");
                    RunCommand($"tuning[37]={MS.POHB_KI}");
                    RunCommand($"tuning[38]={MS.POHB_KD}");
                    RunCommand($"tuning[39]={MS.POHB_FV}");
                    RunCommand($"tuning[40]={MS.POHB_FA}");
                    RunCommand($"tuning[41]={MS.POHB_PL}");
                    RunCommand($"tuning[42]={MS.Loader_KP}");
                    RunCommand($"tuning[43]={MS.Loader_KI}");
                    RunCommand($"tuning[44]={MS.Loader_KD}");
                    RunCommand($"tuning[45]={MS.Loader_FV}");
                    RunCommand($"tuning[46]={MS.Loader_FA}");
                    RunCommand($"tuning[47]={MS.Loader_PL}");

                    RunCommand($"setting[2]={MS.ValveOffset}");
                    RunCommand($"setting[6]={MS.ZROffset}");
                    RunCommand($"setting[7]={MS.ZLOffset}");

                    if (MS.PicoRotaryValveInstalled)
                    {
                        //Set Pico Rotary Valve Home Offset & Speed
                        RunCommand($"gMem[38]={MS.ValveSpeed.ToString("#0000")}");
                        RunCommand($"gMem[39]={MS.ValveOffset.ToString("#0000")}");
                    }

                    DownloadTransferSettings();
                    _objGalil.command("XQ#init,2");
                }
                catch (Exception ex)
                {
                    _log.log(LogType.TRACE, Category.INFO, "ERROR while performing PLC Init request: " + ex.Message);
                    LastError = ex.Message;
                }
                bRetVal = true;
            }
            else
            {
                LastError = "Not connected";
            }
            return bRetVal;
        }
        internal void ResetMaxPosError(string Axis)
        {
            switch (Axis)
            {
                case "X":
                    _dXMaxPosError = 0;
                    break;
                case "LZ":
                    _dLZMaxPosError = 0;
                    break;
                case "RZ":
                    _dRZMaxPosError = 0;
                    break;
                case "Z":
                    _dLZMaxPosError = 0;
                    _dRZMaxPosError = 0;
                    break;

            }
        }
        public bool ApplyTuning(string sAxis, int nKP, double dKI, int nKD, double nFV, double dFA, double nPL)
        {
            bool applied = false;

            try
            {
                RunCommand($"KP{sAxis}={nKP}");
                RunCommand($"KI{sAxis}={dKI}");
                RunCommand($"KD{sAxis}={nKD}");
                RunCommand($"FV{sAxis}={nFV}");
                RunCommand($"FA{sAxis}={dFA}");
                RunCommand($"PL{sAxis}={nPL}");

                applied = true;
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, $"Exception updateing Tuning Params: {ex.Message}");
            }

            return applied;
        }

        internal void NotifyOffline()
        {
            _bDisconnected = true;
            OnConnectionLost?.Invoke(this, new EventArgs());
        }

        public bool WaitOnSignal(int GMemID, double Watch4Value, int TimeoutSecs)
        {
            bool gotSignal = false;
            int timeout = 0;
            int numTries = TimeoutSecs * 20;

            while (!_abortingRecipe && numTries > timeout++)
            {
                if (!Connected || ErrorOccurred)
                {
                    gotSignal = false;
                    break;
                }
                else if (Watch4Value == Memory[GMemID])
                {
                    gotSignal = true;
                    break;
                }

                Thread.Sleep(50);
            }

            if (!gotSignal)
            {
                if (_abortingRecipe)
                {
                    _log.log(LogType.TRACE, Category.INFO, $"WaitSignal({GMemID},{Watch4Value}) - Recipe Aborted - Returning False", "ERROR");
                }
                else if (Connected)
                {
                    _log.log(LogType.TRACE, Category.INFO, $"WaitSignal({GMemID},{Watch4Value}) - Galil Disconnected - Returning False", "ERROR");
                }
                else if (ErrorOccurred)
                {
                    _log.log(LogType.TRACE, Category.INFO, $"WaitSignal({GMemID},{Watch4Value}) - Galil Error Occurred - Returning False", "ERROR");
                }
                else
                {
                    _log.log(LogType.TRACE, Category.INFO, $"WaitSignal({GMemID},{Watch4Value}) - Timeout Occurred - Returning False", "WARNING");
                }
            }

            return gotSignal;
        }

        public bool WaitOnSignalBit(int GMemID, int BitMask, bool Watch4State, int TimeoutSecs, bool ignoreAborted = false)
        {
            bool bRetVal = false;
            int nTimeout = 0;
            int nFlags = (int)Memory[GMemID];
            int nNumTries = TimeoutSecs * 20;

            _log.log(LogType.TRACE, Category.INFO, $"Comparing Flags: 0x{nFlags:X8} to Flag 0x{BitMask:X8}", "DEBUG");

            while ((!_abortingRecipe || ignoreAborted) && nNumTries > nTimeout++)
            {
                if (Connected)
                {
                    if (ErrorOccurred)
                    {
                        bRetVal = false;
                        break;
                    }
                    else
                    {
                        if (Watch4State && ((BitMask & nFlags) == BitMask)) // looking for a ON and the result of 'AND' is ON
                        {
                            bRetVal = true;
                            break;
                        }
                        else if (!Watch4State && ((BitMask & nFlags) != BitMask)) // looking for a OFF and the result of 'AND' is OFF
                        {
                            bRetVal = true;
                            break;
                        }

                        Thread.Sleep(50);

                        nFlags = (int)Memory[GMemID];
                    }
                }
                else
                {
                    bRetVal = false;
                    break;
                }
            }

            if (!bRetVal)
            {
                if (_abortingRecipe)
                {
                    _log.log(LogType.TRACE, Category.INFO, $"WaitSignal({GMemID}:0x{BitMask:X4} for '{Watch4State}) - Recipe Aborted - Returning False", "ERROR");
                }
                else if (Connected)
                {
                    _log.log(LogType.TRACE, Category.INFO, $"WaitSignal({GMemID}:0x{BitMask:X4} for '{Watch4State}') - Timeout Occurred - Returning False", "WARNING");
                    Debug.WriteLine($"Last Sample Flags: 0x{nFlags:X8} to Flag 0x{BitMask:X8}", "DEBUG");
                }
                else
                {
                    _log.log(LogType.TRACE, Category.INFO, $"WaitSignal({GMemID}:0x{BitMask:X4} for '{Watch4State}') - PLC Disconnected or Error - Returning False", "ERROR");
                    Debug.WriteLine($"Last Sample Flags: 0x{nFlags:X8} to Flag 0x{BitMask:X8}", "DEBUG");
                }
            }

            return bRetVal;
        }

        public bool SetMemoryVal(int nIndex, double dValue)
        {
            bool valSet = false;

            if (nIndex < Memory.Length)
            {
                string sCmd = string.Format("gMem[{0}]={1}", nIndex, dValue);

                try
                {
                    RunCommand(sCmd);
                    valSet = true;
                }
                catch (Exception ex)
                {
                    _log.log(LogType.TRACE, Category.INFO, "Error while running SetMemoryVal - " + ex.Message);
                }

                valSet = true;
            }

            return valSet;
        }

        private void ClearInputs()
        {
            for (int i = 0; i < Inputs.Length; Inputs[i] = 1, i++) ;
            XFLS = 1.0;
            XRLS = 1.0;
            ZRFLS = 1.0;
            ZRRLS = 1.0;
            ZLFLS = 1.0;
            ZLRLS = 1.0;
            PumpFLS = 1.0;
            PumpRLS = 1.0;
            PumpBFLS = 1.0;
            PumpBRLS = 1.0;
            HFLS = 1.0;
            HRLS = 1.0;
            Analogs[2] = 1.25;
            Analogs[3] = 1.25;
            Analogs[4] = 1.25;
            Analogs[5] = 1.25;
            Analogs[6] = 1.25;
        }
        public string GetCommandText(int StartLine, int EndLine = -1)
        {
            string sRetVal;
            if (EndLine == -1)
                sRetVal = RunCommand($"LS{StartLine},{StartLine}");
            else
                sRetVal = RunCommand($"LS{StartLine},{EndLine}");
            return sRetVal;
        }

        #region Galil Events

        private void OnRecord(object objRecord)
        {
            FirstRecordReceived = true;
            _dtLastRecord = DateTime.Now;
            _dtLastRecordReceived = DateTime.Now;
            if (_bReadingArray)
            {
                _log.log(LogType.TRACE, Category.INFO, "OnRecord Received while reading download array.", "WARNING");
            }

            try
            {
                SampleTime = (int)_objGalil.sourceValue(objRecord, "TIME");
            }
            catch (Exception)
            {
                return;
            }

            // Analog Inputs
            Inputs[1] = _objGalil.sourceValue(objRecord, "@IN[01]");   // 3-way valve home switch
            Inputs[2] = _objGalil.sourceValue(objRecord, "@IN[02]");   // Resvd.
            Inputs[3] = _objGalil.sourceValue(objRecord, "@IN[03]");   // Syringe Pump ID
            Inputs[4] = _objGalil.sourceValue(objRecord, "@IN[04]");   // POH Pump ID
            Inputs[5] = _objGalil.sourceValue(objRecord, "@IN[05]");   // Safety Guards 
            Inputs[6] = _objGalil.sourceValue(objRecord, "@IN[06]");   // Resvd.
            Inputs[7] = _objGalil.sourceValue(objRecord, "@IN[07]");   // Resvd.
            dLastEMOState = Inputs[8];
            Inputs[8] = _objGalil.sourceValue(objRecord, "@IN[08]");   // EMO depressed
            if (dLastEMOState != Inputs[8])
            {
                _log.log(LogType.TRACE, Category.INFO, string.Format("New EMO Button State: {0}", Inputs[8] == 0 ? "Clear" : "Active"), "Warning");
            }
            Inputs[9] = _objGalil.sourceValue(objRecord, "@IN[09]");   // Front Lift Down Status
            Inputs[10] = _objGalil.sourceValue(objRecord, "@IN[10]");  // Front Lift Up Status
            Inputs[11] = _objGalil.sourceValue(objRecord, "@IN[11]");  // Rear Lift Down Status
            Inputs[12] = _objGalil.sourceValue(objRecord, "@IN[12]");  // Rear Lift Up Status
            Inputs[13] = _objGalil.sourceValue(objRecord, "@IN[13]");  // Resvd.
            Inputs[14] = _objGalil.sourceValue(objRecord, "@IN[14]");  // Resvd.
            Inputs[15] = _objGalil.sourceValue(objRecord, "@IN[15]");  // Resvd.
            Inputs[16] = _objGalil.sourceValue(objRecord, "@IN[16]");  // Resvd.

            //Extended IO - Right Now Configured As Inputs w/ 'CO 0' on Controller
            Inputs[41] = _objGalil.sourceValue(objRecord, "@IN[41]");  // Main Air
            Inputs[42] = _objGalil.sourceValue(objRecord, "@IN[42]");  // Main Vac
            Inputs[43] = _objGalil.sourceValue(objRecord, "@IN[43]");  // Priming Vacuum
            Inputs[44] = _objGalil.sourceValue(objRecord, "@IN[44]");  // Zone 1 Vacuum
            Inputs[45] = _objGalil.sourceValue(objRecord, "@IN[45]");  // Zone 2 Vacuum
            Inputs[46] = _objGalil.sourceValue(objRecord, "@IN[46]");  // Zone 3 Vacuum
            Inputs[47] = _objGalil.sourceValue(objRecord, "@IN[47]");  // Reservoir Full
            Inputs[48] = _objGalil.sourceValue(objRecord, "@IN[48]");  // Reservoir Empty

            Outputs[1] = _objGalil.sourceValue(objRecord, "@OUT[01]");    // Alarm Buzzer
            Outputs[2] = _objGalil.sourceValue(objRecord, "@OUT[02]");    // Keyence GT2 Extend Valve
            Outputs[3] = _objGalil.sourceValue(objRecord, "@OUT[03]");    // Air Knife
            Outputs[4] = _objGalil.sourceValue(objRecord, "@OUT[04]");    // Resv
            Outputs[5] = _objGalil.sourceValue(objRecord, "@OUT[05]");    // Resv
            Outputs[6] = _objGalil.sourceValue(objRecord, "@OUT[06]");    // Right Brake Disengage
            Outputs[7] = _objGalil.sourceValue(objRecord, "@OUT[07]");    // Left Brake Disengage
            Outputs[8] = _objGalil.sourceValue(objRecord, "@OUT[08]");    // Main Air Valve

            Outputs[17] = _objGalil.sourceValue(objRecord, "@OUT[17]");   // Air Knife Warmup (Exhaust)
            Outputs[18] = _objGalil.sourceValue(objRecord, "@OUT[18]");   // Air Knife On
            Outputs[19] = _objGalil.sourceValue(objRecord, "@OUT[19]");   // Air Knife Heat
            Outputs[20] = _objGalil.sourceValue(objRecord, "@OUT[20]");   // Priming Air Valve
            Outputs[21] = _objGalil.sourceValue(objRecord, "@OUT[21]");   // Priming Vac Valve
            Outputs[22] = _objGalil.sourceValue(objRecord, "@OUT[22]");   // Zone 1 Vac
            Outputs[23] = _objGalil.sourceValue(objRecord, "@OUT[23]");   // Zone 2 Vac
            Outputs[24] = _objGalil.sourceValue(objRecord, "@OUT[24]");   // Zone 3 Vac
            Outputs[25] = _objGalil.sourceValue(objRecord, "@OUT[25]");   // Zone 1 Air
            Outputs[26] = _objGalil.sourceValue(objRecord, "@OUT[26]");   // Zone 2 Air
            Outputs[27] = _objGalil.sourceValue(objRecord, "@OUT[27]");   // Zone 3 Air
            Outputs[28] = _objGalil.sourceValue(objRecord, "@OUT[28]");   // Lift Pins UP
            Outputs[29] = _objGalil.sourceValue(objRecord, "@OUT[29]");   // Lift Pins Down
            Outputs[30] = _objGalil.sourceValue(objRecord, "@OUT[30]");   // Aligners Up
            Outputs[32] = _objGalil.sourceValue(objRecord, "@OUT[32]");   // Head Recharge Valve
            Outputs[33] = _objGalil.sourceValue(objRecord, "@OUT[33]");   // Die Vent Valve
            Outputs[34] = _objGalil.sourceValue(objRecord, "@OUT[34]");   // Head Dispense Valve
            Outputs[35] = _objGalil.sourceValue(objRecord, "@OUT[35]");   // Head Vent Valve
            Outputs[36] = _objGalil.sourceValue(objRecord, "@OUT[36]");   // Resv

            string info = ConnectionInfo;
            string[] temps = info.Split(new char[] { ',', '\n', '\r' });

            AxisCount = int.Parse(temps[1].Substring(6, 1));

            // TODO - UNCOMMENT ALL E/F/G AXIS REFERENCES AFTER 4070 TESTING!!!!!!!!!
            XFLS = _objGalil.sourceValue(objRecord, "_LF" + X_axis);
            XRLS = _objGalil.sourceValue(objRecord, "_LR" + X_axis);
            ZRFLS = _objGalil.sourceValue(objRecord, "_LF" + RZ_axis);
            ZRRLS = _objGalil.sourceValue(objRecord, "_LR" + RZ_axis);
            ZLFLS = _objGalil.sourceValue(objRecord, "_LF" + LZ_axis);
            ZLRLS = _objGalil.sourceValue(objRecord, "_LR" + LZ_axis);
            PumpFLS = _objGalil.sourceValue(objRecord, "_LF" + PumpA_axis);
            PumpRLS = _objGalil.sourceValue(objRecord, "_LR" + PumpA_axis);
            
            X_TP = _objGalil.sourceValue(objRecord, "_TP" + X_axis);
            RZ_TP = _objGalil.sourceValue(objRecord, "_TP" + RZ_axis);
            LZ_TP = _objGalil.sourceValue(objRecord, "_TP" + LZ_axis);
            PumpA_TP = _objGalil.sourceValue(objRecord, "_TP" + PumpA_axis);

            X_TV = _objGalil.sourceValue(objRecord, "_TV" + X_axis);
            RZ_TV = _objGalil.sourceValue(objRecord, "_TV" + RZ_axis);
            LZ_TV = _objGalil.sourceValue(objRecord, "_TV" + LZ_axis);
            PumpA_TV = _objGalil.sourceValue(objRecord, "_TV" + PumpA_axis);

            X_TD = _objGalil.sourceValue(objRecord, "_TD" + X_axis);
            RZ_TD = _objGalil.sourceValue(objRecord, "_TD" + RZ_axis);
            LZ_TD = _objGalil.sourceValue(objRecord, "_TD" + LZ_axis);
            PumpA_TD = _objGalil.sourceValue(objRecord, "_TD" + PumpA_axis);

            TorqueX = _objGalil.sourceValue(objRecord, "_TT" + X_axis);
            TorqueRZ = _objGalil.sourceValue(objRecord, "_TT" + RZ_axis);
            TorqueLZ = _objGalil.sourceValue(objRecord, "_TT" + LZ_axis);
            TorquePump = _objGalil.sourceValue(objRecord, "_TT" + PumpA_axis);

            PosErrX = _objGalil.sourceValue(objRecord, "_TE" + X_axis);
            PosErrRZ = _objGalil.sourceValue(objRecord, "_TE" + RZ_axis);
            PosErrLZ = _objGalil.sourceValue(objRecord, "_TE" + LZ_axis);
            PosErrPumpA = _objGalil.sourceValue(objRecord, "_TE" + PumpA_axis);

            _dQHA = _objGalil.sourceValue(objRecord, "_QHA");
            int prevX = XHomingStatus, prevZR = ZRHomingStatus, prevZL = ZLHomingStatus, prevP = PumpHomingStatus, prevV = ValveHomingStatus, prevPB = PumpBHomingStatus, prevVB = ValveBHomingStatus, prevL = LoaderHomingStatus;
            XHomingStatus = (int)_objGalil.sourceValue(objRecord, "_ZA" + X_axis);
            ZRHomingStatus = (int)_objGalil.sourceValue(objRecord, "_ZA" + RZ_axis);
            ZLHomingStatus = (int)_objGalil.sourceValue(objRecord, "_ZA" + LZ_axis);
            PumpHomingStatus = (int)_objGalil.sourceValue(objRecord, "_ZA" + PumpA_axis);
            ValveHomingStatus = (int)_objGalil.sourceValue(objRecord, "_ZA" + PumpA_axis);

            if (prevX != XHomingStatus)
                _log.log(LogType.TRACE, Category.INFO, $"New X Homing Status: {XHomingStatus}");
            if (prevZR != ZRHomingStatus)
                _log.log(LogType.TRACE, Category.INFO, $"New Z Right Status: {ZRHomingStatus}");
            if (prevZL != ZLHomingStatus)
                _log.log(LogType.TRACE, Category.INFO, $"New Z Left Status: {ZLHomingStatus}");
            if (prevP != PumpHomingStatus)
                _log.log(LogType.TRACE, Category.INFO, $"New Pmp-A Homing Status: {PumpHomingStatus}");
            if (prevV != ValveHomingStatus)
                _log.log(LogType.TRACE, Category.INFO, $"New Vlv-A Homing Status: {ValveHomingStatus}");

            if (MS.DualPumpInstalled)
            {
                PumpBFLS = _objGalil.sourceValue(objRecord, "_LF" + PumpB_axis);
                PumpBRLS = _objGalil.sourceValue(objRecord, "_LR" + PumpB_axis);
                PumpB_TP = _objGalil.sourceValue(objRecord, "_TP" + PumpB_axis);
                PumpB_TV = _objGalil.sourceValue(objRecord, "_TV" + PumpB_axis);
                PumpB_TD = _objGalil.sourceValue(objRecord, "_TD" + PumpB_axis);
                TorquePumpB = _objGalil.sourceValue(objRecord, "_TT" + PumpB_axis);
                PosErrPumpB = _objGalil.sourceValue(objRecord, "_TE" + PumpB_axis);
                PumpBHomingStatus = (int)_objGalil.sourceValue(objRecord, "_ZA" + PumpB_axis);
                ValveBHomingStatus = (int)_objGalil.sourceValue(objRecord, "_ZA" + PumpB_axis);
                if (prevPB != PumpBHomingStatus)
                    _log.log(LogType.TRACE, Category.INFO, $"New Pmp-B Homing Status: {PumpBHomingStatus}");
                if (prevVB != ValveBHomingStatus)
                    _log.log(LogType.TRACE, Category.INFO, $"New Vlv-B Homing Status: {ValveBHomingStatus}");
                if (Math.Abs(PosErrPumpB) > PumpBMaxPosErr)
                    PumpBMaxPosErr = Math.Abs(PosErrPumpB);
            }

            if (Math.Abs(PosErrX) > _dXMaxPosError)
                _dXMaxPosError = Math.Abs(PosErrX);
            if (Math.Abs(PosErrRZ) > _dRZMaxPosError)
                _dRZMaxPosError = Math.Abs(PosErrRZ);
            if (Math.Abs(PosErrLZ) > _dLZMaxPosError)
                _dLZMaxPosError = Math.Abs(PosErrLZ);
            if (Math.Abs(PosErrPumpA) > PumpMaxPosErr)
                PumpMaxPosErr = Math.Abs(PosErrPumpA);

            X_MotorOff = _objGalil.sourceValue(objRecord, "_MO" + X_axis) != 0.0;
            RZ_MotorOff = _objGalil.sourceValue(objRecord, "_MO" + RZ_axis) != 0.0;
            LZ_MotorOff = _objGalil.sourceValue(objRecord, "_MO" + LZ_axis) != 0.0;
            PumpA_MotorOff = _objGalil.sourceValue(objRecord, "_MO" + PumpA_axis) != 0.0;
            if(MS.DualPumpInstalled)
                PumpB_MotorOff = _objGalil.sourceValue(objRecord, "_MO" + PumpB_axis) != 0.0;
            XMoving = _objGalil.sourceValue(objRecord, "_BG" + X_axis) != 0.0;
            ZRightMoving = _objGalil.sourceValue(objRecord, "_BG" + RZ_axis) != 0.0;
            ZLeftMoving = _objGalil.sourceValue(objRecord, "_BG" + LZ_axis) != 0.0;
            PumpA_Moving = _objGalil.sourceValue(objRecord, "_BG" + PumpA_axis) != 0.0;
            if(MS.DualPumpInstalled)
                PumpB_Moving = _objGalil.sourceValue(objRecord, "_BG" + PumpB_axis) != 0.0;

            if (MS.HasLoader)
            {
                LoaderFLS = _objGalil.sourceValue(objRecord, "_LF" + Loader_axis);
                LoaderRLS = _objGalil.sourceValue(objRecord, "_LR" + Loader_axis);
                Loader_TP = _objGalil.sourceValue(objRecord, "_TP" + Loader_axis);
                Loader_TV = _objGalil.sourceValue(objRecord, "_TV" + Loader_axis);
                Loader_TD = _objGalil.sourceValue(objRecord, "_TD" + Loader_axis);
                TorqueLoader = _objGalil.sourceValue(objRecord, "_TT" + Loader_axis);
                PosErrLoader = _objGalil.sourceValue(objRecord, "_TE" + Loader_axis);
                LoaderHomingStatus = (int)_objGalil.sourceValue(objRecord, "_ZA" + Loader_axis);
                Loader_MotorOff = _objGalil.sourceValue(objRecord, "_MO" + Loader_axis) != 0.0;
                Loader_Moving = _objGalil.sourceValue(objRecord, "_BG" + Loader_axis) != 0.0;
            }

            bLastELOLatched = ELOLatched;
            ELOLatched = 1.0 == _objGalil.sourceValue(objRecord, "TA3AD");
            if (bLastELOLatched != ELOLatched)
                _log.log(LogType.TRACE, Category.INFO, string.Format("New ELO States: {0}", ELOLatched ? "Latched" : "Clear"), "Warning");
            dLastTA3 = TA3;
            TA3 = _objGalil.sourceValue(objRecord, "_TA3");
            if (bLastELOLatched != ELOLatched || dLastTA3 != TA3)
                _log.log(LogType.TRACE, Category.INFO, string.Format("New TA3 States: {0}", TA3.ToString("0")), "Warning");
            _TC = _objGalil.sourceValue(objRecord, "_TC");

            Threads[0] = _objGalil.sourceValue(objRecord, "NO0") == 1.0;
            Threads[1] = _objGalil.sourceValue(objRecord, "NO1") == 1.0;
            Threads[2] = _objGalil.sourceValue(objRecord, "NO2") == 1.0;
            Threads[3] = _objGalil.sourceValue(objRecord, "NO3") == 1.0;
            Threads[4] = _objGalil.sourceValue(objRecord, "NO4") == 1.0;
            Threads[5] = _objGalil.sourceValue(objRecord, "NO5") == 1.0;
            Threads[6] = _objGalil.sourceValue(objRecord, "NO6") == 1.0;
            Threads[7] = _objGalil.sourceValue(objRecord, "NO7") == 1.0;


            // Vac Zones
            if (VacuumDisabled)
            {
                Analogs[2] = .5;
                if (Outputs[4] == 1.0)
                {
                    Analogs[4] = .5;
                }
                else
                {
                    Analogs[4] = 1.25;

                }
                if (Outputs[3] == 1.0)
                {
                    Analogs[3] = .5;
                }
                else
                {
                    Analogs[3] = 1.25;
                }
                if (Outputs[6] == 1.0)
                {
                    Analogs[5] = .5;
                }
                else
                {
                    Analogs[5] = 1.25;
                }

            }
            else
            {
                if (_bHasAnalogInputs)
                {
                    Analogs[2] = _objGalil.sourceValue(objRecord, "@AN[2]");  // Main Vac
                    Analogs[3] = _objGalil.sourceValue(objRecord, "@AN[3]");  // Priming Vac
                    Analogs[4] = _objGalil.sourceValue(objRecord, "@AN[4]");  // Zone 1 Vac
                    Analogs[5] = _objGalil.sourceValue(objRecord, "@AN[5]");  // Zone 2 Vac

                    //Analogs 6-8  Move to DataUpdate() - value is correct but a little slower (maybe)
                    //Data record does not ship values unless axis is attached
                    //Analogs[6] = _objGalil.sourceValue(objRecord, "@AN[6]"); // Zone 3 Vac
                    //Analogs[6] = (Outputs[24] == 1.0) ? _objGalil.sourceValue(objRecord, "@AN[2]") : 0.0; // Zone 3 Vac
                }
            }
            if (_bHasAnalogInputs)
            {
                Analogs[1] = _objGalil.sourceValue(objRecord, "@AN[1]");  // Main Air 
                Analogs[6] = _objGalil.sourceValue(objRecord, "@AN[6]");
                Analogs[7] = _objGalil.sourceValue(objRecord, "@AN[7]");
                Analogs[8] = _objGalil.sourceValue(objRecord, "@AN[8]");
            }

            //if (!Reconnecting && !_bDisconneted && !_bCommutating)
            //{
            //    try
            //    {
            //        _objGalil.command("gAirPrs=" + MainAirPressure.ToString("#.#"));
            //        _nRetryCount = 0;
            //    }
            //    catch (Exception ex)
            //    {
            //        _log.log(LogType.TRACE, Category.INFO, "Error Sending gAirPrs to Controller: " + ex.Message, "INFO");
            //        _nRetryCount++;
            //        if (_nRetryCount > 10)
            //        {
            //            _nRetryCount = 0;
            //            ReConnect(Address);
            //        }
            //    }
            //}

            if (_bDataLogActive)
            {
                StringBuilder sb = new StringBuilder(128);
                sb.Append($"{_swDataLogStopwatch.ElapsedMilliseconds},");
                sb.Append($"{MainAirPressure.ToString("0.0")},");
                sb.Append($"{XPos.ToString("0.000")},");
                sb.Append($"{PosErrX.ToString("0.000")},");
                sb.Append($"{XVel.ToString("0.000")},");
                sb.Append($"{ZRPos.ToString("0.000")},");
                sb.Append($"{ZRVel.ToString("0.000")},");
                sb.Append($"{PosErrRZ.ToString("0.000")},");
                sb.Append($"{ZLPos.ToString("0.000")},");
                sb.Append($"{ZLVel.ToString("0.000")},");
                sb.Append($"{PosErrLZ.ToString("0.000")},");
                sb.Append($"{PumpVolLoc.ToString("0.000")},");
                sb.Append($"{PumpVel.ToString("0.000")},");
                sb.Append($"{PosErrPumpA.ToString("0.000")},");
                sb.Append($"{TorquePump.ToString("0.000")},");
                if (MS.DualPumpInstalled)
                {
                    sb.Append($"{PumpBVolLoc.ToString("0.000")},");
                    sb.Append($"{PumpBVel.ToString("0.000")},");
                    sb.Append($"{PosErrPumpB.ToString("0.000")},");
                    sb.Append($"{TorquePumpB.ToString("0.000")},");
                }
                if (MS.ChuckTempControlEnabled)
                    sb.Append($"{ChuckTemp},");
                if (MS.DieTempControlEnabled)
                    sb.Append($"{DieTemp},");
                if (MS.ReservoirTempControlEnabled)
                    sb.Append($"{ResvTemp},");
                if (MS.HasDiePressureTransducer)
                    sb.Append($"{DiePressurePSI.ToString("0.000")}");
                DataLogFile.WriteLine(sb.ToString());
                sb.Clear();
            }

        }
        private void OnMessage(string message)
        {
            int nStart = 0;
            int nEnd;
            _log.log(LogType.TRACE, Category.INFO, message.Replace("CTRLRMSG", ""));
            if (message.Contains("#VALPOS:"))
            {
                nStart = message.LastIndexOf("#VALPOS:");
                nEnd = message.IndexOf('#', nStart + 1);
                if (nEnd == -1)
                    nEnd = message.Length - 1;
                string sPosInfo = message.Substring(nStart + 8, nEnd - nStart - 8);
            }
            if (message.Contains("#HOMEMSG"))
            {
                nStart = message.LastIndexOf("#HOMEMSG:");
                nEnd = message.IndexOf('#', nStart + 1);
                if (nEnd == -1)
                    nEnd = message.Length - 1;
                HomingMessage = message.Substring(nStart + 9, nEnd - nStart - 9);
            }
            if (message.Contains("#COMMUTATION - BEGIN"))
            {
                _bCommutating = true;
                _log.log(LogType.TRACE, Category.INFO, "Received Commutation Begin Signal - Flagging Continue");
                StopDataRecord(); //_objGalil.recordsStart(0);
                Thread.Sleep(750); // 3 record cycles
                _dtLastRecord = DateTime.Now;
                DataUpdate(false, true);
                Thread.Sleep(50);
                RunCommand("gMem[1]=1");
            }
            if (message.Contains("#COMMUTATION - F"))
            {
                _log.log(LogType.TRACE, Category.INFO, "Received Commutation Finshed Signal - Restarting Messaging");
                RestartMessaging = true;
                _bCommutating = false;
            }
            if (message.Contains("BX failure"))
            {
                _log.log(LogType.TRACE, Category.INFO, "Initialization Failure - Restarting Messaging");
                RestartMessaging = true;
                _bCommutating = false;
            }
            if (message.Contains("#STATEMSG: ") && _readStateMsgs)
            {
                RecipeStateMsg = message.Substring(message.IndexOf("#STATEMSG: ") + 10).Trim();

                if (RecipeStateMsg.Contains("\n"))
                {
                    string[] saLines = RecipeStateMsg.Split('\n');
                    RecipeStateMsg = saLines[0].Trim();
                }
            }
            else if (!_readStateMsgs)
            {
                RecipeStateMsg = "";
            }

        }
        private void OnInterrupt(int status)
        {
            switch (status)
            {
                case 208:
                {
                    _log.log(LogType.TRACE, Category.INFO, "Notification: Axis A profiled motion complete _BGA = 0", "INFO");
                }
                break;
                case 209:
                {
                    _log.log(LogType.TRACE, Category.INFO, "Notification: Axis B profiled motion complete _BGA = 0", "INFO");
                }
                break;
                case 210:
                {
                    _log.log(LogType.TRACE, Category.INFO, "Notification: Axis C profiled motion complete _BGA = 0", "INFO");
                }
                break;
                case 211:
                {
                    _log.log(LogType.TRACE, Category.INFO, "Notification: Axis D profiled motion complete _BGA = 0", "INFO");
                }
                break;
                case 216:
                {
                    _log.log(LogType.TRACE, Category.INFO, "Notification: All axes profiled motion complete _BGI = 0", "INFO");
                }
                break;
                case 200:
                {
                    _log.log(LogType.TRACE, Category.INFO, "Notification: Excess position error _TEn >= _ERn*", "INFO");
                }
                break;
                case 219:  // Application Stopped
                {
                    _log.log(LogType.TRACE, Category.INFO, "Notification: Program Ended", "INFO");
                    //_eventPgmEnded.Set();
                    //if (RunStatusUpdate != null)
                    //{
                    //    StatusEventArgs sea = new StatusEventArgs(StatusEventArgType.RunStatus, -1, "Finished");
                    //    RunStatusUpdate(this, sea);
                    //}
                }
                break;
                case 225:  // Digital Input 1 is low @IN[1] = 0  -- Stepper wheel
                {
                    _log.log(LogType.TRACE, Category.INFO, "Notification: @IN[01] - Stepper Flag", "INFO");
                }
                break;
                case 232:  // Digital Input 8 is low @IN[8] = 0  -- ELO
                {
                    _log.log(LogType.TRACE, Category.INFO, "Notification: @IN[08] - ELO", "INFO");
                    //_bELOTripped = true;
                    OnELODetected?.Invoke(this, new EventArgs());
                }
                break;

                default:
                    Debug.WriteLine("Galil OnInterruption --- " + status.ToString(), "DEBUG");
                    break;
            }
        }

        #endregion Galil Events

        #region Error Codes

        private void SetupErrorCodes()
        {
            if (_htErrorCodes == null)
                _htErrorCodes = new Hashtable(168);
            ErrorCodes.Add(1, "Unrecognized command");
            ErrorCodes.Add(2, "Command only valid from program");
            ErrorCodes.Add(3, "Command not valid in program");
            ErrorCodes.Add(4, "Operand error");
            ErrorCodes.Add(5, "Input buffer full");
            ErrorCodes.Add(6, "Number out of range");
            ErrorCodes.Add(7, "Command not valid while running");
            ErrorCodes.Add(8, "Command not valid while not running");
            ErrorCodes.Add(9, "Variable error");
            ErrorCodes.Add(10, "Empty program line or undefined label");
            ErrorCodes.Add(11, "Invalid label or line number");
            ErrorCodes.Add(12, "Subroutine more than 16 deep");
            ErrorCodes.Add(13, "JG only valid when running in jog mode");
            ErrorCodes.Add(14, "EEPROM check sum error");
            ErrorCodes.Add(15, "EEPROM write error");
            ErrorCodes.Add(16, "IP incorrect sign during position move or IP given during forced deceleration");
            ErrorCodes.Add(17, "ED, BN and DL not valid while program running");
            ErrorCodes.Add(18, "Command not valid when contouring");
            ErrorCodes.Add(19, "Application strand already executing");
            ErrorCodes.Add(20, "Begin not valid with motor off");
            ErrorCodes.Add(21, "Begin not valid while running");
            ErrorCodes.Add(22, "Begin not possible due to Limit Switch");
            ErrorCodes.Add(24, "Begin not valid because no sequence defined");
            ErrorCodes.Add(25, "Variable not given in IN command");
            ErrorCodes.Add(28, "S operand not valid");
            ErrorCodes.Add(29, "Not valid during coordinated move");
            ErrorCodes.Add(30, "Sequenct Segment Too Short");
            ErrorCodes.Add(31, "Total move distance in a sequence > 2 billion");
            ErrorCodes.Add(32, "Segment buffer full");
            ErrorCodes.Add(33, "VP or CR commands cannot be mixed with LI commands");
            ErrorCodes.Add(39, "No time specified");
            ErrorCodes.Add(41, "Contouring record range error");
            ErrorCodes.Add(42, "Contour data being sent too slowly");
            ErrorCodes.Add(46, "Gear axis both master and follower");
            ErrorCodes.Add(50, "Not enough fields");
            ErrorCodes.Add(51, "Question mark not valid");
            ErrorCodes.Add(52, "Missing \" or string too long");
            ErrorCodes.Add(53, "Error in {}");
            ErrorCodes.Add(54, "Question mark part of string");
            ErrorCodes.Add(55, "Missing [ or []");
            ErrorCodes.Add(56, "Array index invalid or out of range");
            ErrorCodes.Add(57, "Bad function or array");
            ErrorCodes.Add(58, "Bad command response i.e._GNX");
            ErrorCodes.Add(59, "Mismatched parentheses");
            ErrorCodes.Add(60, "Download error - line too long or too many lines");
            ErrorCodes.Add(61, "Duplicate or bad label");
            ErrorCodes.Add(62, "Too many labels");
            ErrorCodes.Add(63, "IF statement without ENDIF");
            ErrorCodes.Add(65, "IN command must have a comma");
            ErrorCodes.Add(66, "Array space full");
            ErrorCodes.Add(67, "Too many arrays or variables");
            ErrorCodes.Add(71, "IN only valid in thread #0");
            ErrorCodes.Add(80, "Record mode already running");
            ErrorCodes.Add(81, "No array or source specified");
            ErrorCodes.Add(82, "Undefined Array");
            ErrorCodes.Add(83, "Not a valid number");
            ErrorCodes.Add(84, "Too many elements");
            ErrorCodes.Add(90, "Only A B C D valid operand");
            ErrorCodes.Add(97, "Bad Binary Command Format");
            ErrorCodes.Add(98, "Binary Commands not valid in application program");
            ErrorCodes.Add(99, "Bad binary command number");
            ErrorCodes.Add(100, "Not valid when running ECAM");
            ErrorCodes.Add(101, "Improper index into ET");
            ErrorCodes.Add(102, "No master axis defined for ECAM");
            ErrorCodes.Add(103, "Master axis modulus greater than 256 EP value");
            ErrorCodes.Add(104, "Not valid when axis performing ECAM");
            ErrorCodes.Add(105, "EB1 command must be given first");
            ErrorCodes.Add(106, "Privilege Violation");
            ErrorCodes.Add(110, "No hall effect sensors detected");
            ErrorCodes.Add(111, "Must be made brushless by BA command");
            ErrorCodes.Add(112, "BZ command timeout");
            ErrorCodes.Add(113, "No movement in BZ command");
            ErrorCodes.Add(114, "BZ command runaway");
            ErrorCodes.Add(118, "Controller has GL1600 not GL1800");
            ErrorCodes.Add(119, "Not valid for axis configured as stepper");
            ErrorCodes.Add(120, "Bad Ethernet transmit not valid for PCI");
            ErrorCodes.Add(121, "Bad Ethernet packet received not valid for PCI");
            ErrorCodes.Add(123, "TCP lost sync not valid for PCI");
            ErrorCodes.Add(124, "Ethernet handle already in use not valid for PCI");
            ErrorCodes.Add(125, "No ARP response from IP address not valid for PCI");
            ErrorCodes.Add(126, "Closed Ethernet handle not valid for PCI");
            ErrorCodes.Add(127, "Illegal Modbus function code not valid for PCI");
            ErrorCodes.Add(128, "IP address not valid not valid for PCI");
            ErrorCodes.Add(130, "Remote IO command error not valid for PCI");
            ErrorCodes.Add(131, "Serial Port Timeout not valid for PCI, See Remarks");
            ErrorCodes.Add(132, "Analog inputs not present");
            ErrorCodes.Add(133, "Command not valid when locked / Handle must be UDP not valid for PCI");
            ErrorCodes.Add(134, "All motors must be in MO for this command");
            ErrorCodes.Add(135, "Motor must be in MO");
            ErrorCodes.Add(136, "Invalid Password");
            ErrorCodes.Add(137, "Invalid lock setting");
            ErrorCodes.Add(138, "Passwords not identical");
            ErrorCodes.Add(140, "Serial encoder missing Valid for BiSS support");
            ErrorCodes.Add(141, "Incorrect ICM Configuration");
            ErrorCodes.Add(143, "TM timed out Valid on SER firmware (SSI and BiSS)");
            ErrorCodes.Add(160, "BX failure");
            ErrorCodes.Add(161, "Sine amp axis not initialized");
            ErrorCodes.Add(164, "Exceeded maximum sequence length, BGS or BGT is required");
            ErrorCodes.Add(166, "Unable to set analog output");
        }
        internal string GetErrorMessage(double nErrorCode)
        {
            string sRetVal = "";
            int nTemp = (int)nErrorCode;

            if (_htErrorCodes != null && _htErrorCodes.ContainsKey(nTemp))
            {
                sRetVal = (string)_htErrorCodes[nTemp];
            }

            return sRetVal;
        }

        #endregion Error Codes

        #endregion

        #region Recipe Execution

        private StreamWriter DataLogFile { set; get; } = null;
        private Stopwatch _swDataLogStopwatch = new Stopwatch();
        public bool OkToRelaseVacDuringRecipe => Memory[7] == 3;
        private volatile bool _bDataLogActive = false;
        private bool _abortingRecipe = false;
        public bool DownloadParams(Recipe defaultRecipe, Recipe recipe)
        {
            bool bRetVal = true;
            string sTemp = "";
            string gMem14 = $"gMem[14]=0";
            string params1 = "params[1]=16000";
            string params2 = "params[2]=16000";
            string params3 = "params[3]=16000";
            string params4 = "params[4]=16000";
            string params9 = $"params[9]={MS.MeasureHeight-MS.ShimSize}";
            string params10 = $"params[10]={MS.ZLZeroEncPosForChuck}";
            string params11 = $"params[11]={MS.ZRZeroEncPosForChuck}";
            string params12 = $"params[12]={MS.ZLMeasurePosForPriming}";
            string params13 = $"params[13]={MS.ZRMeasurePosForPriming}";
            string setting3 = $"setting[3]={uLConv}";
            string speedXAxis = "SPA=160000";
            string accelXAxis = "ACA=320000";
            string decelXAxis = "DCA=320000";

            if (recipe.UsingKeyenceLaser && LaserMgr != null)
            {
                LaserMgr.SetProgramNumber(recipe.KeyenceProgramNumber);
            }

            RunCommand($"{gMem14}");//Reset Laser Read Complete Flag
            Thread.Sleep(5);
            RunCommand($"{setting3}");
            Thread.Sleep(5);
            RunCommand($"{speedXAxis}"); // 100 mm/s | 200 mm/s² | 200 mm/s²
            Thread.Sleep(5);
            RunCommand($"{accelXAxis}");
            Thread.Sleep(5);
            RunCommand($"{decelXAxis}");
            Thread.Sleep(5);

            _log.log(LogType.TRACE, Category.INFO, "DownloadParams() - begin");
            DownloadDelays();

            RunCommand($"{params1}");
            Thread.Sleep(5);
            RunCommand($"{params2}");
            Thread.Sleep(5);
            RunCommand($"{params3}");
            Thread.Sleep(5);
            RunCommand($"{params4}");
            Thread.Sleep(5);
            RunCommand($"{params9}");
            Thread.Sleep(5);
            RunCommand($"{params10}");
            Thread.Sleep(5);
            RunCommand($"{params11}");
            Thread.Sleep(5);
            RunCommand($"{params12}");
            Thread.Sleep(5);
            RunCommand($"{params13}");
            Thread.Sleep(5);

            // dump the default params...
            double dMaxZSpeed = 20;
            List<int> zSpeedsLocs = new List<int> { 91, 101, 116, 132, 137, 161, 153 };
            foreach (RecipeParam defaultParam in defaultRecipe.RecipeParams)
            {
                sTemp = defaultParam.Value.Trim();
                if (sTemp != "")
                {
                    if (zSpeedsLocs.Contains(defaultParam.ArrayLocation))
                        sTemp = dMaxZSpeed.ToString();
                    RunCommand($"params[{defaultParam.ArrayLocation}]={sTemp}");
                    Thread.Sleep(5);
                }
            }

            // dump the receipe params...
            foreach (RecipeParam param in recipe.RecipeParams)
            {
                sTemp = param.Value.Trim();
                if (sTemp != "")
                {
                    if (zSpeedsLocs.Contains(param.ArrayLocation))
                        sTemp = dMaxZSpeed.ToString();
                    RunCommand(string.Format("params[{0}]={1}", param.ArrayLocation, sTemp));
                    Thread.Sleep(5);
                }
            }

            // dump the dispense profile params if using Freestyle profile...
            RecipeParam tempParam = null;
            int dispProfileType = 0;
            if (GetParam(126, recipe, defaultRecipe, out tempParam))
            {
                dispProfileType = int.Parse(tempParam.Value);
                _log.log(LogType.TRACE, Category.INFO, $"DispenseProfileType = {dispProfileType}", "", SubCategory.RECIPE_RUN);
            }
            else
            {
                _log.log(LogType.TRACE, Category.ERROR, $"Failed To Read Parameter 126, DispenseProfileType", "", SubCategory.RECIPE_RUN);
            }
            if (dispProfileType == 4)
            {
                foreach (DynamicDispenseProfileParam param in recipe.DispenseProfileParams)
                {
                    double xPos = param.XPos;
                    double dispRate = param.DispenseRate;
                    double zOffset = param.ZOffset;
                    int arrLoc = param.ArrayLocation;
                    string cmdStr = $"rtchgpos[{arrLoc}]={xPos};rtchgrt[{arrLoc}]={dispRate};rtchght[{arrLoc}]={zOffset};";
                    RunCommand(cmdStr);
                    Thread.Sleep(5);
                }
            }

            // If no priming plate, block any recipe priming
            if (!MS.HasPrimingPlate)
            {
                RunCommand("params[26]=0)");
                Thread.Sleep(5);
            }

            if (MS.AirKnifeInstalled)
            {
                RunCommand($"params[75]={MS.DieToAirKnifePitch:#0.0};airknf=1;");
                Thread.Sleep(5);

                bool heatOn = MS.AirKnifeHeaterInstalled;
                int time = heatOn ? MS.AirKnifeHeaterWarmup * 1000 : 0;
                RunCommand($"gMem[32]={time};akheat={(heatOn ? 1 : 0)};");
                _log.log(LogType.TRACE, Category.INFO, $"******* Warmup delay: {time}");
                Thread.Sleep(5);
            }
            else
            {
                RunCommand("params[78]=0;airknf=0;");
                Thread.Sleep(5);
            }

            if (MS.IRLampInstalled)
            {
                RunCommand($"params[195]={MS.DieToIRPitch:#0.0}");
                Thread.Sleep(5);
            }

            double jumpPos = Math.Min(MS.ZLMeasurePosForPriming, MS.ZRMeasurePosForPriming) - 5;
            RunCommand($"params[90]={jumpPos}");                   // jump height from end of priming measure to start of priming
            Thread.Sleep(5);

            RunCommand($"params[165]={MS.XMaintLoc}"); // return to location
            Thread.Sleep(5);

            return bRetVal;
        }

        public void DownloadDevicesUsed(Recipe defaultRecipe, Recipe recipe, bool recipeCompleted)
        {
            RecipeParam tempParam = null;
            if (!recipeCompleted)
            {
                _log.log(LogType.TRACE, Category.INFO, $"Handling Device: Loader");
                string loader = string.Format("devices[1]={0}", MS.HasLoader ? 1 : 0);
                _log.log(LogType.TRACE, Category.INFO, $"Handling Device: Laser");
                string laser = string.Format("devices[2]={0}", MS.UsingKeyenceLaser && recipe.UsingKeyenceLaser ? 1 : 0);
                _log.log(LogType.TRACE, Category.INFO, $"Handling Device: Aligners");
                string aligners = string.Format("devices[3]={0}", MS.AlignersEnabled ? 1 : 0);
                _log.log(LogType.TRACE, Category.INFO, $"Handling Device: LiftPins");
                string liftPins = string.Format("devices[4]={0}", (MS.LiftPinsEnabled || MS.HasLiftAndCenter) ? 1 : 0);
                _log.log(LogType.TRACE, Category.INFO, $"Handling Device: Airknife");
                string airKnife = string.Format("devices[5]={0}", MS.AirKnifeInstalled ? 1 : 0);
                _log.log(LogType.TRACE, Category.INFO, $"Handling Device: IR Lamp");
                string irLamp = string.Format("devices[6]={0}", MS.HasIRTransmitter && (recipe.UseIRDuringCoating || recipe.UseIROnReturn) ? 1 : 0);
                _log.log(LogType.TRACE, Category.INFO, $"Handling Device: Dual Pump");
                string dualPump = string.Format("devices[7]={0}", MS.DualPumpInstalled ? 1 : 0);
                _log.log(LogType.TRACE, Category.INFO, $"Handling Device: Dual Zone Lift Pins");
                string dualzoneliftPins = string.Format("devices[8]={0}", MS.DualZoneLiftPinsEnabled ? 1 : 0);
                _log.log(LogType.TRACE, Category.INFO, $"Handling Device: Lift and Center");
                string liftncenter = string.Format("devices[9]={0}", MS.HasLiftAndCenter ? 1 : 0);

                //Set Extra Variables For Installed Devices
                string.Format("gAirNif={0}", MS.AirKnifeInstalled ? 1 : 0);

                //Send True Only If Device Installed && Used In Recipe
                RunCommand($"{loader}");
                Thread.Sleep(5);
                RunCommand($"{aligners}");
                Thread.Sleep(5);
                RunCommand($"{liftPins}");
                Thread.Sleep(5);
                RunCommand($"{dualzoneliftPins}");
                Thread.Sleep(5);
                RunCommand($"{airKnife}");
                Thread.Sleep(5);
                RunCommand($"{irLamp}");
                Thread.Sleep(5);
                RunCommand($"{dualPump}");
                Thread.Sleep(5);
                RunCommand($"{liftncenter}");
            }
            else
            {
                string loader = string.Format("devices[1]={0}", MS.HasLoader ? 1 : 0);
                string laser = string.Format("devices[2]={0}", MS.UsingKeyenceLaser ? 1 : 0);
                string aligners = string.Format("devices[3]={0}", MS.AlignersEnabled ? 1 : 0);
                string liftPins = string.Format("devices[4]={0}", (MS.LiftPinsEnabled || MS.HasLiftAndCenter) ? 1 : 0);
                string airKnife = string.Format("devices[5]={0}", MS.AirKnifeInstalled ? 1 : 0);
                string irLamp = string.Format("devices[6]={0}", MS.HasIRTransmitter ? 1 : 0);
                string dualPump = string.Format("devices[7]={0}", MS.DualPumpInstalled ? 1 : 0);
                string dualzoneliftPins = string.Format("devices[8]={0}", MS.DualZoneLiftPinsEnabled ? 1 : 0);
                string liftncenter = string.Format("devices[9]={0}", MS.HasLiftAndCenter ? 1 : 0);

                string selPump = "params[110]=0";
                string pumpAVol = "params[111]=100";
                string pumpBVol = "params[112]=0";

                //Send True Only If Device Installed && Used In Recipe
                RunCommand($"{loader}");
                Thread.Sleep(5);
                RunCommand($"{aligners}");
                Thread.Sleep(5);
                RunCommand($"{liftPins}");
                Thread.Sleep(5);
                RunCommand($"{dualzoneliftPins}");
                Thread.Sleep(5);
                RunCommand($"{airKnife}");
                Thread.Sleep(5);
                RunCommand($"{irLamp}");
                Thread.Sleep(5);
                RunCommand($"{dualPump}");
                Thread.Sleep(5);
                RunCommand($"{liftncenter}");

                //Set Pump Selection Params To Pump-A if Dual-Pump Not Installed
                if (!MS.DualPumpInstalled)
                {
                    Thread.Sleep(50);
                    RunCommand($"{selPump};{pumpAVol};{pumpBVol};");
                }
            }
        }

        internal bool AbortRecipe()
        {
            bool bRetVal = false;

            if (RunningRecipe)
            {
                _abortingRecipe = true;

                if (Memory[7] != 0)
                {
                    RunCommand("SP0,0,0,0;HX4;HX2;HX3;HX5;HX6;ST");

                    if (MS.AirKnifeInstalled)
                    {
                        SetAirKnifeMainValve(false);
                    }

                    RunCommand("gMem[7]=0");

                    if (MS.UsingKeyenceLaser)
                    {
                        SetMemoryVal(14, 0.0);
                    }
                }
            }

            return bRetVal;
        }

        internal bool RunRecipe(Recipe defaultRecipe, Recipe recipe, clsCognex cognex)
        {
            bool recipeRan = false;
            bool bContinue = true;
            bool usingAirKnifeOnCoating = false;
            bool usingAirKnifeOnReturn = false;
            bool usingAirKnifeAfterCoat = false;

            RecipeStateMsg = "Starting Process";
            _readStateMsgs = true;
            RecipeRunErrorCode = 0;

            string RC = "", finishMsg = "Success";
            CurrentRecipe = recipe;

            _log.log(LogType.TRACE, Category.INFO, $"RunRecipe ({recipe.Name})", "", SubCategory.RECIPE_RUN);

            _abortingRecipe = false;
            double temp;
            RecipeParam tempParam = null;

            _log.log(LogType.TRACE, Category.INFO, $"Shim Size: {MS.ShimSize}", "", SubCategory.RECIPE_RUN);
            _log.log(LogType.TRACE, Category.INFO, $"Bank No: {Storage.SelectedBankNo}", "", SubCategory.RECIPE_RUN);

            if (RunningRecipe)
            {
                bContinue = false;
                _log.log(LogType.TRACE, Category.INFO, $"Shim Size: {MS.ShimSize}", "", SubCategory.RECIPE_RUN);
            }

            if (bContinue)
            {
                try
                {
                    RunningRecipe = true;

                    _log.log(LogType.TRACE, Category.INFO, $"DOWNLOADING params[]...", "", SubCategory.RECIPE_RUN);
                    DownloadParams(defaultRecipe, recipe);

                    _log.log(LogType.TRACE, Category.INFO, $"DOWNLOADING devices[]...", "", SubCategory.RECIPE_RUN);
                    DownloadDevicesUsed(defaultRecipe, recipe, false);

                    _log.log(LogType.TRACE, Category.INFO, "Running Leveling Routine for Coating Glass", "", SubCategory.RECIPE_RUN);
                    LevelingDie = true;

                    // Chuck Levelling
                    try
                    {
                        //Fiducial Verification If Using Cognex Cameras
                        if (MS.CognexCommunicationsUsed && !_abortingRecipe && recipe.VerifyFiducials)
                        {
                            bool goToOK = true;
                            bool continueCheck = false;
                            RecipeStateMsg = "Verifying Fiducial Alignment...";

                            if (RunGotoVisionPosition())
                            {
                                _log.log(LogType.TRACE, Category.INFO, $"#go2Meas Called...", "", SubCategory.RECIPE_RUN);

                                while (!XMoving && !continueCheck)
                                {
                                    if (_abortingRecipe)
                                    {
                                        goToOK = false;
                                    }

                                    if (IsClose(XPos, MS.MeasureLoc, 2))
                                    {
                                        goToOK = true;
                                        continueCheck = true;
                                    }
                                }

                                if (goToOK)
                                {
                                    //Check Right-Camera Fiducial Presence
                                    if (!(cognex.RightCamera.Results.Cells["E24"].ToString() == "1"))
                                    {
                                        _log.log(LogType.TRACE, Category.ERROR, "Cognex Right Camera: Failed To Locate Fiducials...Aborting Recipe!", "", SubCategory.RECIPE_RUN);
                                        bContinue = false;
                                    }
                                    else
                                    {
                                        _log.log(LogType.TRACE, Category.INFO, "Cognex Right Camera: Fiducial Found!", "", SubCategory.RECIPE_RUN);
                                    }

                                    //Check Left-Camera Fiducial Presence
                                    if (!(cognex.LeftCamera.Results.Cells["E24"].ToString() == "1"))
                                    {
                                        _log.log(LogType.TRACE, Category.ERROR, "Cognex Left Camera: Failed To Locate Fiducials...Aborting Recipe!", "", SubCategory.RECIPE_RUN);
                                        bContinue = false;
                                    }
                                    else
                                    {
                                        _log.log(LogType.TRACE, Category.INFO, "Cognex Left Camera: Fiducial Found!", "", SubCategory.RECIPE_RUN);
                                    }
                                }
                            }
                            else
                            {
                                _log.log(LogType.TRACE, Category.ERROR, "GoToVisionPos Failed To Execute...Aborting Recipe!!", "");
                                bContinue = false;
                            }
                        }

                        RecipeStateMsg = "Beginning Chuck Glass Measurement";

                        if (!_abortingRecipe && bContinue && LevelDieToGlass(false, recipe, defaultRecipe, false))
                        {
                            _log.log(LogType.TRACE, Category.INFO, "Leveling Routine for Coating Glass returned", "", SubCategory.RECIPE_RUN);

                            // if this succeeds, then the die is setting in its 'coating gap' location
                            // need to set some params now

                            string runCMD = string.Format("params[56]={0}", RZ_TP);
                            _log.log(LogType.TRACE, Category.INFO, $"Sending {runCMD}", "", SubCategory.RECIPE_RUN);
                            RC = RunCommand(runCMD);

                            Thread.Sleep(5);

                            runCMD = string.Format("params[57]={0}", LZ_TP);
                            _log.log(LogType.TRACE, Category.INFO, $"Sending {runCMD}", "", SubCategory.RECIPE_RUN);
                            RC = RunCommand(runCMD);

                            double LEO = 0;
                            double CoatingGap = 500;
                            double TEO = 0;

                            if (GetParam(131, recipe, defaultRecipe, out tempParam))
                            {
                                LEO = double.Parse(tempParam.Value);
                                _log.log(LogType.TRACE, Category.INFO, $"LEO = {LEO}", "", SubCategory.RECIPE_RUN);
                            }
                            else
                            {
                                _log.log(LogType.TRACE, Category.ERROR, $"Failed To Read Parameter 131, LEO", "", SubCategory.RECIPE_RUN);
                            }

                            if (GetParam(136, recipe, defaultRecipe, out tempParam))
                            {
                                CoatingGap = double.Parse(tempParam.Value);
                                _log.log(LogType.TRACE, Category.INFO, $"Coating Gap = {CoatingGap}", "", SubCategory.RECIPE_RUN);
                            }
                            else
                            {
                                _log.log(LogType.TRACE, Category.ERROR, $"Failed To Read Parameter 136, Coating Gap", "", SubCategory.RECIPE_RUN);
                            }

                            if (GetParam(152, recipe, defaultRecipe, out tempParam))
                            {
                                TEO = double.Parse(tempParam.Value);
                                _log.log(LogType.TRACE, Category.INFO, $"TEO = {TEO}", "", SubCategory.RECIPE_RUN);
                            }
                            else
                            {
                                _log.log(LogType.TRACE, Category.ERROR, $"Failed To Read Parameter 152, TEO", "", SubCategory.RECIPE_RUN);
                            }

                            temp = RZ_TP + (LEO * umConv);
                            runCMD = string.Format("params[54]={0}", temp);
                            _log.log(LogType.TRACE, Category.INFO, $"Sending {runCMD}...", "", SubCategory.RECIPE_RUN);
                            RunCommand(runCMD);

                            Thread.Sleep(5);

                            temp = LZ_TP + (LEO * umConv);
                            runCMD = string.Format("params[55]={0}", temp);
                            _log.log(LogType.TRACE, Category.INFO, $"Sending {runCMD}...", "", SubCategory.RECIPE_RUN);
                            RunCommand(string.Format("params[55]={0}", temp));

                            Thread.Sleep(5);

                            temp = RZ_TP + (TEO * umConv);
                            runCMD = string.Format("params[58]={0}", temp);
                            _log.log(LogType.TRACE, Category.INFO, $"Sending {runCMD}...", "", SubCategory.RECIPE_RUN);
                            RunCommand(string.Format("params[58]={0}", temp));

                            Thread.Sleep(5);

                            temp = LZ_TP + (TEO * umConv);
                            runCMD = string.Format("params[59]={0}", temp);
                            _log.log(LogType.TRACE, Category.INFO, $"Sending {runCMD}...", "", SubCategory.RECIPE_RUN);
                            RunCommand(string.Format("params[59]={0}", temp));

                            Thread.Sleep(5);
                        }
                        else if (!_abortingRecipe)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "Leveling Routine for Coating Glass  FAILED", "", SubCategory.RECIPE_RUN);
                            bContinue = false;

                            if (RecipeRunErrorCode == 0)
                            {
                                finishMsg = $"Coating Area Get Height Failed: {LevelFailReason}";
                                RecipeRunErrorCode = -100;
                            }
                        }
                        else
                        {
                            _log.log(LogType.TRACE, Category.INFO, "Detected aborting recipe after LvlDie call", "", SubCategory.RECIPE_RUN);
                            bContinue = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.log(LogType.TRACE, Category.ERROR, $"Error During Level Routine - {ex.Message} ", "", SubCategory.RECIPE_RUN);
                        finishMsg = CheckForExceptionData(ex);
                        bContinue = false;
                    }

                    //Handles Case Where GT2s Extended After Calibration
                    if (!MS.UsingKeyenceLaser && GT2sExtended)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "Detected GT2's Extended... Retracting GT2's", "", SubCategory.RECIPE_RUN);
                        RetractGT2s();

                        if (GT2sExtended)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "GT2s Still Reading Extended!!! Aborting Recipe!", "", SubCategory.RECIPE_RUN);
                            bContinue = false;
                        }
                    }

                    if (bContinue && recipe.IsSegmented)
                    {
                        double jumpHeight = 2;     //mm
                        double jumpAccel = 400;    //mm/s^2
                        double jumpDecel = 400;    //mm/s^2
                        double xVelocity = 50;     //mm/s

                        try
                        {
                            jumpHeight = GetParamValueDouble(73, "JumpHeight", recipe, defaultRecipe) / 1000;
                            xVelocity = GetParamValueDouble(144, "XVelocity", recipe, defaultRecipe);
                            jumpAccel = GetParamValueDouble(70, "JumpAccel", recipe, defaultRecipe);
                            jumpDecel = GetParamValueDouble(71, "JumpDecel", recipe, defaultRecipe);

                            double time = Math.Sqrt(jumpHeight / jumpAccel) + Math.Sqrt(jumpHeight / jumpDecel);
                            double leadIn = xVelocity * time;

                            RunCommand(string.Format("params[61]={0}", leadIn));
                            _log.log(LogType.TRACE, Category.INFO, string.Format("Setting params[61] to {0}", leadIn), "", SubCategory.RECIPE_RUN);
                        }
                        catch (Exception ex)
                        {
                            bContinue = false;
                            _log.log(LogType.TRACE, Category.ERROR, $"Caught an exception trying to set up segmented recipe params: {ex.Message}", "", SubCategory.RECIPE_RUN);
                        }
                    }

                    // Priming Plate Levelling
                    if (bContinue && recipe.HasPriming)
                    {
                        try
                        {
                            _log.log(LogType.TRACE, Category.INFO, "Running Leveling Routine for Priming Glass", "", SubCategory.RECIPE_RUN);
                            RecipeStateMsg = "Beginning Priming Plate Glass Measurement";

                            if (LevelDieToGlass(true, recipe, defaultRecipe, false))
                            {
                                // if this succeeds, then the die is setting in its 'priming gap' location
                                // need to set some params now
                                temp = RZ_TP;
                                _log.log(LogType.TRACE, Category.INFO, $"Setting params[50] to {temp}", "", SubCategory.RECIPE_RUN);
                                RunCommand($"params[50]={temp}");

                                Thread.Sleep(5);

                                temp = LZ_TP;
                                _log.log(LogType.TRACE, Category.INFO, $"Setting params[51] to {temp}", "", SubCategory.RECIPE_RUN);
                                RunCommand(string.Format("params[51]={0}", temp));

                                Thread.Sleep(5);

                                double primingGap = GetParamValueDouble(100, "PrimingGap", recipe, defaultRecipe);
                                _log.log(LogType.TRACE, Category.INFO, $"Priming Gap = {primingGap}", "", SubCategory.RECIPE_RUN);

                                double jumpGap = GetParamValueDouble(115, "JumpGap", recipe, defaultRecipe);
                                _log.log(LogType.TRACE, Category.INFO, $"Jump Gap = {jumpGap}", "", SubCategory.RECIPE_RUN);

                                temp = (primingGap - jumpGap) * umConv;
                                _log.log(LogType.TRACE, Category.INFO, $"Setting params[52] to {temp}", "", SubCategory.RECIPE_RUN);
                                RunCommand($"params[52]={temp}");

                                Thread.Sleep(5);

                                RunCommand($"params[53]={temp}");
                                _log.log(LogType.TRACE, Category.INFO, $"Setting params[53] to {temp}");
                            }
                            else if (!_abortingRecipe)
                            {
                                _log.log(LogType.TRACE, Category.ERROR, $"Leveling Routine for Priming Glass  FAILED. REASON:  {(RecipeRunErrorCode == 0 ? LevelFailReason : "UNKNOWN")}", "", SubCategory.RECIPE_RUN);
                                bContinue = false;

                                if (RecipeRunErrorCode == 0)
                                {
                                    finishMsg = $"Priming Area Get Height Failed: {LevelFailReason}";
                                    _log.log(LogType.TRACE, Category.INFO, $"RecipeRunErrorCode = -101 (Priming Area Get Height Failed).", "", SubCategory.RECIPE_RUN);
                                    RecipeRunErrorCode = -101;
                                }
                            }
                            else
                            {
                                _log.log(LogType.TRACE, Category.INFO, "Detected aborting recipe after LvlDie call.", "", SubCategory.RECIPE_RUN);
                                bContinue = false;
                            }
                        }
                        catch (ParamException pe)
                        {
                            _log.log(LogType.TRACE, Category.ERROR, $"Caught Exception reading params for Priming Chuck levelling:  {pe.Message}");
                            bContinue = false;
                        }
                        catch (Exception ex)
                        {
                            _log.log(LogType.TRACE, Category.INFO, $"Error During Level Routine - {ex.Message} ", "", SubCategory.RECIPE_RUN);

                            finishMsg = CheckForExceptionData(ex);

                            bContinue = false;
                        }
                    }

                    LevelingDie = false;

                    // Start Coating Cycle
                    if (bContinue)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "Calling Coating Cycle", "", SubCategory.RECIPE_RUN);
                        RecipeStateMsg = "Starting Coating Routine";

                        if (MS.DataLogging && MS.DataLoggingEnabled)
                        {
                            OpenDataLog();
                        }

                        if (bContinue && MS.IRLampInstalled && !IRTransmitter.IsConnected && recipe.UsesIR)
                        {
                            bContinue = IRTransmitter.Connect();
                            RecipeStateMsg = "Attempting Connection To IR Lamp...";
                            _log.log(LogType.TRACE, Category.WARN, "IR Lamp Not Connected, Connecting Now", "", SubCategory.RECIPE_RUN);
                        }

                        if (_abortingRecipe)
                        {
                            _log.log(LogType.TRACE, Category.ERROR, "Detected aborting recipe after IR Lamp Connection.", "", SubCategory.RECIPE_RUN);
                            bContinue = false;
                        }

                        if (bContinue)
                        {
                            // gMem[26] = Ready To Coat Signal From Controller
                            // gMem[27] = IR Lamp Ready To Coat Signal
                            // gMem[28] = Controller Ready To Return From Coating
                            // gMem[29] = IR Lamp Ready To Return
                            // gMem[30] = IR Lamp OK To Idle Signal

                            RecipeStateMsg = "Starting Galil Coat Program";
                            RunCommand(recipe.IsSegmented ? "XQ#segs1,4" : "XQ#coat,4");
                        }

                        _log.log(LogType.TRACE, Category.INFO, "Waiting For 'Ready To Coat' Signal From Controller...", "", SubCategory.RECIPE_RUN);
                        RecipeStateMsg = "Waiting For 'Ready To Coat' Signal...";

                        if (bContinue && !WaitOnSignal(7, 1, 15)) // wait on notication of thread start  (15 sec timeout)
                        {
                            _log.log(LogType.TRACE, Category.ERROR, "Timeout waiting on waiting on coat thread to start on controller", "", SubCategory.RECIPE_RUN);
                            SetMemoryVal(2, 1); // notfy controller thread to abort
                            bContinue = false;
                        }
                        else if (bContinue)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "Received coat thread started signal", "", SubCategory.RECIPE_RUN);
                        }

                        if (bContinue && MS.IRLampInstalled && recipe.UseIRDuringCoating)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "Waiting On 1st IR Signal", "", SubCategory.RECIPE_RUN);

                            if (!WaitOnSignal(26, 1, 20 * 300))
                            {
                                RecipeStateMsg = "ERROR: Timeout waiting on waiting on 1st IR Signal!!!";
                                _log.log(LogType.TRACE, Category.ERROR, "Timeout waiting on waiting on 1st IR Signal", "", SubCategory.RECIPE_RUN);
                                SetMemoryVal(2, 1); // notfy controller thread to abort
                                bContinue = false;
                            }
                            else
                            {
                                _log.log(LogType.TRACE, Category.INFO, "1st IR Signal Received! Continuing To Set Power Level For Coating...", "", SubCategory.RECIPE_RUN);

                                bContinue = IRTransmitter.WaitForPowerLevelChange(recipe.IRPowerLevelDuringCoating, 10, () => _abortingRecipe);
                                IRTransmitter.TurnOnIRTransmitter();

                                if (bContinue)
                                {
                                    string msg = $"IR Lamp - Power Level Set For Coating: {recipe.IRPowerLevelDuringCoating} %";
                                    RecipeStateMsg = msg;
                                    _log.log(LogType.TRACE, Category.INFO, msg, "", SubCategory.RECIPE_RUN);

                                    Thread.Sleep(50);

                                    //Set Signal Saying IR-Lamp Is Now Ready For Coating  |  Auto Flag OK If IR Not Installed
                                    RecipeStateMsg = "IR Lamp Ready, Signaling OK To Continue Coating.";
                                    _log.log(LogType.TRACE, Category.INFO, "IR Lamp Ready, Signaling OK To Continue Coating.",
                                        "", SubCategory.RECIPE_RUN);
                                    SetMemoryVal(27, 1);

                                    Thread.Sleep(10);
                                }
                                else
                                {
                                    SetMemoryVal(2, 1); // notfy controller thread to abort
                                    bContinue = false;
                                    AbortRecipe();
                                    _log.log(LogType.TRACE, Category.ERROR, "ERROR: Recipe Failed To Set IR Power Level During Coating.",
                                        "", SubCategory.RECIPE_RUN);
                                }
                            }
                        }

                        if (bContinue && MS.IRLampInstalled && recipe.UseIROnReturn)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "Waiting On 2nd IR Signal", "", SubCategory.RECIPE_RUN);

                            if (!WaitOnSignal(28, 1, 20 * 300))
                            {
                                string msg = "ERROR: Timeout waiting on waiting on 2nd IR Signal!!!";
                                RecipeStateMsg = msg;
                                _log.log(LogType.TRACE, Category.ERROR, msg, "", SubCategory.RECIPE_RUN);
                                SetMemoryVal(2, 1); // notfy controller thread to abort
                                bContinue = false;
                            }
                            else
                            {
                                _log.log(LogType.TRACE, Category.INFO, "2nd IR Signal Received! Continuing To Set Power Level For Return...", "", SubCategory.RECIPE_RUN);

                                bContinue = IRTransmitter.WaitForPowerLevelChange(recipe.IRPowerLevelOnReturn, 10, () => _abortingRecipe);
                                IRTransmitter.TurnOnIRTransmitter();

                                if (bContinue)
                                {
                                    string msg = $"IR Lamp Power Level Set For Return: {recipe.IRPowerLevelOnReturn} %";
                                    RecipeStateMsg = msg;
                                    _log.log(LogType.TRACE, Category.INFO, msg, "", SubCategory.RECIPE_RUN);

                                    Thread.Sleep(50);

                                    //Set Signal Saying IR-Lamp Is Now Ready For Coating  |  Auto Flag OK If IR Not Installed
                                    msg = "IR Lamp Ready, Signaling OK To Continue Coating.";
                                    RecipeStateMsg = msg;
                                    _log.log(LogType.TRACE, Category.INFO, msg, "", SubCategory.RECIPE_RUN);
                                    SetMemoryVal(29, 1);
                                    Thread.Sleep(10);
                                }
                                else
                                {
                                    SetMemoryVal(2, 1); // notfy controller thread to abort
                                    bContinue = false;
                                    AbortRecipe();
                                    _log.log(LogType.TRACE, Category.ERROR, "ERROR: Recipe Failed To Set IR Power Level for return.",
                                        "", SubCategory.RECIPE_RUN);
                                }
                            }
                        }

                        //Check If Releasing Vac @ EOC
                        if (!GetParam(185, recipe, defaultRecipe, out RecipeParam releaseVacAtEOC))
                        {
                            _log.log(LogType.TRACE, Category.ERROR, "Exception Getting Params[185], Release Vac At EOC ", "", SubCategory.RECIPE_RUN);
                            bContinue = false;
                        }
                        else if (int.Parse(releaseVacAtEOC.Value) == 1) // Release Vacuum @ EOC
                        {
                            _log.log(LogType.TRACE, Category.INFO, $"Waiting for EOC signal to release vac", "", SubCategory.RECIPE_RUN);

                            //Wait on Signal Bit To Wait For User To Give 'OK' To Return Home
                            if (bContinue && !WaitOnSignal(35, 1, MS.MaxCoatingTimeout)) // wait on notification of EOC
                            {
                                _log.log(LogType.TRACE, Category.ERROR, "Timeout waiting on controller to give signal that we are ready to release vac @ EOC.", "", SubCategory.RECIPE_RUN);
                                SetMemoryVal(2, 1); // notfy controller thread to abort
                                bContinue = false;
                            }
                            else
                            {
                                _log.log(LogType.TRACE, Category.INFO, "Received signal That we are ready to release vac @ EOC.", "", SubCategory.RECIPE_RUN);
                                ReleaseVacuum(Zones.AllChuck);
                                ReleaseVacuum(Zones.Priming);
                                _log.log(LogType.TRACE, Category.INFO, "Vacuum Has Been Released.", "", SubCategory.RECIPE_RUN);
                                SetMemoryVal(36, 1);
                                _log.log(LogType.TRACE, Category.INFO, "Set Signal That Vacuum Has Been Released.", "", SubCategory.RECIPE_RUN);
                            }
                        }

                        if (bContinue && MS.IRLampInstalled && recipe.UseIROnReturn)
                        {
                            bContinue = WaitOnIRIdle();
                        }

                        if (bContinue)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "Waiting for coat thread to complete", "", SubCategory.RECIPE_RUN);
                        }
                        else
                        {
                            _log.log(LogType.TRACE, Category.INFO, "Skipping wait for coat thread to complete", "", SubCategory.RECIPE_RUN);
                        }

                        if (bContinue && !WaitOnSignal(7, 0, MS.MaxCoatingTimeout)) // wait on notication of thread stop  (5 mins timeout)
                        {
                            if (Connected)
                            {
                                _log.log(LogType.TRACE, Category.INFO, "Timeout waiting on waiting on coat thread to finish on controller", "", SubCategory.RECIPE_RUN);
                                SetMemoryVal(2, 1); // notfy controller thread to abort
                            }
                            else
                            {
                                _log.log(LogType.TRACE, Category.INFO, "Controller Connection Lost", "", SubCategory.RECIPE_RUN);
                            }

                            bContinue = false;
                        }
                        else if (bContinue)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "Received coat thread finished signal", "", SubCategory.RECIPE_RUN);
                        }

                        //Release All Vacuum Zones After Coating If Recipe Calls For It
                        if (recipe.ReleaseVacuumOnCompletion)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "Releasing All Vacuum Zones Upon Completion...", "", SubCategory.RECIPE_RUN);
                            ReleaseVacuum(Zones.AllChuck);

                            if (MS.HasPrimingPlate)
                            {
                                ReleaseVacuum(Zones.Priming);
                            }
                        }
                    }

                    if (_abortingRecipe)
                    {
                        RecipeRunErrorCode = -999;
                        finishMsg = "Recipe Aborted";
                        _log.log(LogType.TRACE, Category.INFO, $"RecipeRunErrorCode = -999 ({finishMsg}).", "", SubCategory.RECIPE_RUN);
                    }

                    // Turn off the IR, whether we succeeded or not
                    if (MS.IRLampInstalled && recipe.UseIRDuringCoating)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "Turning Off IRTransmitter.", "", SubCategory.RECIPE_RUN);
                        IRTransmitter.TurnOffIRTransmitter();
                    }

                    // Unload Substrate
                    if(recipe.UnloadSubstrateOnCompletion)
                    {
                        _log.log(LogType.TRACE, Category.INFO, $"Unloading Substrate At End Of Run Cycle.", "", SubCategory.RECIPE_RUN);
                        //TODO  The actual code?
                    }

                    _log.log(LogType.TRACE, Category.INFO, $"RunningRecipe = FALSE.", "", SubCategory.RECIPE_RUN);
                    RunningRecipe = false;
                    _abortingRecipe = false;
                }
                catch (Exception ex)
                {
                    _log.log(LogType.TRACE, Category.INFO, $"Exception during Recipe Run: {ex.Message}", "", SubCategory.RECIPE_RUN);
                    RecipeRunErrorCode = -998;
                    finishMsg = "Exception Occurred See Log.";
                    _log.log(LogType.TRACE, Category.INFO, $"RecipeRunErrorCode = -998 (See Log Above...)", "", SubCategory.RECIPE_RUN);
                }

                RunningRecipe = false;
            }

            _log.log(LogType.TRACE, Category.INFO, $"Closing Data Log...", "", SubCategory.RECIPE_RUN);
            CloseDataLog();

            OnRecipeFinished?.Invoke(this, new RecipeEventArgs(RecipeRunErrorCode, finishMsg));
            _readStateMsgs = false;

            return recipeRan;
        }

        private bool WaitOnIRIdle()
        {
            _log.log(LogType.TRACE, Category.INFO, "Waiting On IR Idle Signal", "", SubCategory.RECIPE_RUN);

            if (!WaitOnSignal(30, 1, 20 * 300))
            {
                RecipeStateMsg = "ERROR: Timeout waiting on waiting on IR Idle Signal!!!";
                _log.log(LogType.TRACE, Category.ERROR, "Timeout waiting on waiting on IR Idle Signal", "", SubCategory.RECIPE_RUN);
                SetMemoryVal(2, 1); // notfy controller thread to abort
                return false;
            }

            _log.log(LogType.TRACE, Category.INFO, "IR Idle Signal Received! Turning IR Off", "", SubCategory.RECIPE_RUN);

            if (!IRTransmitter.WaitForPowerLevelChange(0, 10, () => _abortingRecipe))
            {
                SetMemoryVal(2, 1); // notfy controller thread to abort
                AbortRecipe();
                _log.log(LogType.TRACE, Category.ERROR, "ERROR: Recipe Failed To Turn Off IR Power",
                    "", SubCategory.RECIPE_RUN);
                return false;
            }

            string msg = $"IR Lamp - Power is off.";
            RecipeStateMsg = msg;
            _log.log(LogType.TRACE, Category.INFO, msg, "", SubCategory.RECIPE_RUN);
            return true;
        }

        private string CheckForExceptionData(Exception ex)
        {
            string reason = "";

            if (ex.Data.Contains("Reason"))
            {
                switch (ex.Data["Reason"].ToString())
                {
                    case "XPos":
                    {
                        reason = "X Position Error while Reading GT2s - Chuck";
                        _log.log(LogType.TRACE, Category.ERROR, $"Level Failure Reason:     X-Axis Position Error while Reading GT2s.", "", SubCategory.RECIPE_RUN);
                    }
                    break;

                    case "XMotor":
                    {
                        reason = "X Axis Motor Off while Reading GT2s - Chuck";
                        _log.log(LogType.TRACE, Category.ERROR, $"Level Failure Reason:     X-Axis Motor OFF while Reading GT2s.", "", SubCategory.RECIPE_RUN);
                    }
                    break;

                    default:
                    {
                        reason = "Get Height Error during routine - Chuck";
                        _log.log(LogType.TRACE, Category.ERROR, $"Level Failure Reason:     Get Height Error during routine.", "", SubCategory.RECIPE_RUN);
                    }
                    break;
                }

                _log.log(LogType.TRACE, Category.INFO, $"RecipeRunErrorCode = -103 (Error During Level Routine).", "", SubCategory.RECIPE_RUN);
                RecipeRunErrorCode = -103;
            }

            return reason;
        }


        public static bool GetParam(int paramNo, Recipe recipe, Recipe defaultRecipe, out RecipeParam tempParam)
        {
            bool gotParam = false;
            RecipeParam recipeParam = null;

            if (recipe.GetParam(paramNo, ref recipeParam))
            {
                gotParam = true;
            }
            else if (defaultRecipe.GetParam(paramNo, ref recipeParam))
            {
                gotParam = true;
            }

            tempParam = recipeParam;
            return gotParam;
        }

        public static double GetParamValueDouble(int paramNum, string name, Recipe recipe, Recipe defaultRecipe)
        {
            double val = 0;

            if (GetParam(paramNum, recipe, defaultRecipe, out RecipeParam tempParam))
            {
                val = double.Parse(tempParam.Value);
            }
            else
            {
                throw new ParamException($"Failed To Read Parameter {paramNum}, {name}");
            }

            return val;
        }

        #endregion Recipe Execution

        private bool CheckDemoMode()
        {
#if DEMOMODE
            return true;
#else
            return false;
#endif
        }
    }
}
