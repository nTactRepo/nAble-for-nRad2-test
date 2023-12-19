using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using nTact.PLC;
using nAble.Enums;
using static nAble.Enums.NAbleEnums;

namespace nAble.Data
{
    [XmlRoot("MachineSettingsII")]
    public class MachineSettingsII
    {
        #region Enums

        public enum DMC_ControllerType { DMCx4040 = 0, DMCx4050 = 1 };

        #endregion

        #region Constants

        public const int ZAxisEncoderCountsPerMM = 1600;

        #endregion

        #region Internal Settings

        public DateTime LastUpdated { set; get; }
        public int RecordSendRate { set; get; } = 200;
        public int DataUpdateRate { get; set; } = 150;
        public bool BypassSafetyGuards { get; set; } = false;
        public bool PicoRotaryValveInstalled { get; set; } = false;
        public bool HasPumpFlowMeters { get; set; } = false;
        public bool EnableLooperMode { get; set; } = false;
        public static LogEntry LogEntry { get; set; } = null;

        public bool ProcessLogsEnabled { get; set; } = false;

        #endregion

        #region Tuning Parameters

        #region X Axis - A

        public int X_KP { set; get; } = 300;
        public double X_KI { set; get; } = 1.143;
        public int X_KD { get; set; } = 500;
        public double X_PL { set; get; } = 500;
        public double X_FV { get; set; } = 0;
        public double X_FA { get; set; } = 100;

        #endregion

        #region Z Axis Right - B

        public int ZR_KP { set; get; } = 100;
        public double ZR_KI { set; get; } = 0.1;
        public int ZR_KD { set; get; } = 800;
        public double ZR_PL { set; get; } = 0;
        public double ZR_FA { set; get; } = 0;
        public double ZR_FV { set; get; } = 0;

        #endregion

        #region Z Axis Left - C

        public int ZL_KP { set; get; } = 100;
        public double ZL_KI { set; get; } = 0.1;
        public int ZL_KD { set; get; } = 800;
        public double ZL_PL { set; get; } = 0;
        public double ZL_FA { set; get; } = 0;
        public double ZL_FV { set; get; } = 0;

        #endregion

        #region Pump Axis - D Syringe

        public int SyringeA_KP { set; get; } = 400;
        public double SyringeA_KI { set; get; } = 0.1;
        public int SyringeA_KD { set; get; } = 600;
        public double SyringeA_PL { set; get; } = 0;
        public double SyringeA_FA { set; get; } = 0;
        public double SyringeA_FV { set; get; } = 0;

        #endregion

        #region Pump Axis - D POH

        public int POHA_KP { set; get; } = 400;
        public double POHA_KI { set; get; } = 0.1;
        public int POHA_KD { set; get; } = 600;
        public double POHA_PL { set; get; } = 0;
        public double POHA_FA { set; get; } = 0;
        public double POHA_FV { set; get; } = 0;

        #endregion

        #region Pump Axis (Syringe-B) - F

        public int SyringeB_KP { set; get; } = 400;
        public double SyringeB_KI { set; get; } = 0.1;
        public int SyringeB_KD { set; get; } = 600;
        public double SyringeB_PL { set; get; } = 0;
        public double SyringeB_FA { set; get; } = 0;
        public double SyringeB_FV { set; get; } = 0;

        #endregion

        #region Pump Axis (POH-B) - F

        public int POHB_KP { set; get; } = 400;
        public double POHB_KI { set; get; } = 0.1;
        public int POHB_KD { set; get; } = 600;
        public double POHB_PL { set; get; } = 0;
        public double POHB_FA { set; get; } = 0;
        public double POHB_FV { set; get; } = 0;

        #endregion

        #region Loader Conveyor Axis - H

        public int Loader_KP { set; get; } = 300;
        public double Loader_KI { set; get; } = .1;
        public int Loader_KD { get; set; } = 1200;
        public double Loader_PL { set; get; } = 300;
        public double Loader_FV { get; set; } = 0;
        public double Loader_FA { get; set; } = 0;

        #endregion

        #endregion

        #region Safety Zones

        public double Zone1Start { get; set; } = -999.999;
        public double Zone1MaxZ { get; set; } = 47;
        public double Zone2Start { get; set; } = 125;
        public double Zone2MaxZ { get; set; } = 47;
        public double Zone3Start { get; set; } = 255;
        public double Zone3MaxZ { get; set; } = 47;
        public double Zone4Start { get; set; } = 285;
        public double Zone4MaxZ { get; set; } = 47;
        public double Zone5Start { get; set; } = 750;
        public double Zone5MaxZ { get; set; } = 47;
        public double MeasureZonePrimingStart { get; set; } = 65;
        public double MeasureZonePrimingEnd { get; set; } = 206;
        public double MeasureZoneChuckStart { get; set; } = 217;
        public double MeasureZoneChuckEnd { get; set; } = 678;
        public int MaxMeasureMove { get; set; } = 2000;	// 2 mm - mAX dURING lEVELING rOUTINE

        #endregion

        #region General Zones

        public double PrimingPlateStart { get; set; } = 133.0;
        public double PrimingPlateEnd { get; set; } = 279.0;
        public double ChuckStart { get; set; } = 287.0;
        public double ChuckEnd { get; set; } = 752.000;

        #endregion

        #region General Settings

        public int ControllerType { get; set; } = 0;
        public bool HasAnalogInputs { get; set; } = false;
        
        public string OperatorPW { get; set; } = "000000";
        public string EditorPW { get; set; } = "0000";
        public string AdminPW { get; set; } = "999999999";

        public string VacuumUnits { get; set; } = "inHg";
        public string DisplayedTimeUnits { get; set; } = "sec";
        public int TimeOut { get; set; } = 60;
        public int MaxCoatingTimeout { get; set; } = 300;
        public bool DataLogging { get; set; } = false;

        public bool HasDiePressureTransducer { get; set; } = false;
        public double DiePressureInputVoltageAdjust { get; set; } = 0.0; // (Offset (in PSI) for matching adjustment of)
        public double DiePressureOffset { get; set; } = -0.06;
        public bool HasReservoirSensors { get; set; } = false;
        public bool HasPrimingPlate { get; set; } = true;
        public int NumChuckAirVacZones { get; set; } = 1;
        public bool UsesSelectiveAirVacZones { set; get; } = false; // (Denotes that zones are selectable on the chuck screen.)

        public int SelectiveZones { get; set; } = 0x01; // (Bitmask for selectable zones. 0x1-Zone1;0x2-Zone2;0x4-Zone3;)
        public bool SelectiveZonesConfigOK => !UsesSelectiveAirVacZones || SelectiveZones != 0;
        public bool SelectiveZone1Enabled => (SelectiveZones & 0x1) == 0x1;
        public bool SelectiveZone2Enabled => (SelectiveZones & 0x2) == 0x2;
        public bool SelectiveZone3Enabled => (SelectiveZones & 0x4) == 0x4;

        #region Stack Settings

        public bool HasLoader { set; get; } = false;
        public bool HasStack { set; get; } = false;
        public string PLCAddress { set; get; } = "10.10.0.100";

        public bool StackMod1Installed { set; get; } = false;
        public int StackMod1Type { set; get; } = 0;
        public string StackMod1Name { set; get; } = "";
        public bool StackMod2Installed { set; get; } = false;
        public int StackMod2Type { set; get; } = 0;
        public string StackMod2Name { set; get; } = "";
        public bool StackMod3Installed { set; get; } = false;
        public int StackMod3Type { set; get; } = 0;
        public string StackMod3Name { set; get; } = "";

        public bool StackIncludesAdvancedVac =>
            (StackMod1Installed && 
                (((StackModuleType)StackMod1Type == StackModuleType.VacuBakePlus) ||                                     
                ((StackModuleType)StackMod1Type == StackModuleType.VacuDryPlus))) ||
            (StackMod2Installed && 
                (((StackModuleType)StackMod2Type == StackModuleType.VacuBakePlus) || 
                ((StackModuleType)StackMod2Type == StackModuleType.VacuDryPlus))) ||
            (StackMod3Installed && 
                (((StackModuleType)StackMod3Type == StackModuleType.VacuBakePlus) || 
                ((StackModuleType)StackMod3Type == StackModuleType.VacuDryPlus)));

        #endregion

        public double ZROffset { set; get; } = 0;
        public double ZLOffset { set; get; } = 0.0;
        public double CrossBarWidth { set; get; } = 739.089;
        public int PumpEncoderResolution { set; get; } = 4000;
        public bool HideHeadPurgeOnRun { set; get; } = false;
        public bool HideHeadPurgeOnFluidControl { set; get; } = false;
        public bool HideRecirculateOnFluidControl { set; get; } = false;

        public double SyringeVol { set; get; } = 250; // Syringe Volume in µl
        public double SyringeBVol { set; get; } = 250; // Syringe-B Volume in µl
        public double PumpHomingRate { set; get; } = 50; // µl/sec
        public int PrimePreDelay { get; set; } = 0; // Delay before 'dispense' action of pump to allow valves time to open and pressure to stabalize (msecs)
        public int PrimePostDelay { get; set; } = 0; // Delay before 'recharge' action of pump to allow valves time to open and pressure to stabalize (msecs)
        public double POHVol { set; get; } = 17.9;   //ml
        public double POHDiam { set; get; } = 20;   //ml
        public double POHScrewPitch { set; get; } = 1.0; // mm
        public int POHMotorGearTeeth { set; get; } = 20;
        public int POHPistonGearTeeth { get; set; } = 20;
        public double POHGearRatio => (double)POHMotorGearTeeth / POHPistonGearTeeth;
        public double POHHomingRate { set; get; } = 100;
        public double POHBVol { set; get; } = 16;   //ml
        public double POHBDiam { set; get; } = 20;   //ml
        public double POHBScrewPitch { set; get; } = 1.0; // mm
        public int POHBMotorGearTeeth { set; get; } = 20;
        public int POHBPistonGearTeeth { get; set; } = 20;
        public double POHBGearRatio => (double)POHBMotorGearTeeth / POHBPistonGearTeeth;
        public double POHBHomingRate { set; get; } = 100;
        
        public double MeasureSpeed { set; get; } = 100;   // mm/s
        public double MeasureHeight { set; get; } = 1;   // mm
        public double MeasureLeftZPos { set; get; } = 43.000;   // mm
        public double MeasureRightZPos { set; get; } = 43.000;   // mm

        //////////////////////////////////////////////////  Leveling 
        public int MaxLevelingRetries { set; get; } = 15;
        public double PrimingMeasureLoc { set; get; } = 126;  // mm
        public double MeasureLoc { set; get; } = 283;  // mm
        public double ShimSize { set; get; } = 0.127;    // Calibration Shim size in µm

        public double ZLZeroEncPosForChuck { set; get; } = 45.5 * ZAxisEncoderCountsPerMM;    // counts (from top)
        public double ZRZeroEncPosForChuck { set; get; } = 45.5 * ZAxisEncoderCountsPerMM;    // counts (from top)
        public double ZLMeasurePosForPriming { set; get; } = 42;    // mm (from top)
        public double ZRMeasurePosForPriming { set; get; } = 42;    // mm (from top)
        public double ZLMeasurePos { set; get; } = 42;    // mm (from top)
        public double ZRMeasurePos { set; get; } = 42;    // mm (from top)

        /////////////////////////////////////////////////  Connectivitity
        public string IPAddress { set; get; } = "10.10.1.112";
        public string KeyenceCOMPort { set; get; } = "COM5"; // Keyence GT2 Keyence DL-RS1A Serial Module
        public string ChuckCOMPort { set; get; } = "COM3"; // Omega CNi - Chuck heater
        public int ChuckCOMID { set; get; } = 2;  // Omega CNi3233 - Chuck Temp Controller
        public string DieCOMPort { set; get; } = "COM3";  // Omega CNi3233 - Die Temp
        public int DieCOMID { set; get; } = 3; // Omega CNi3233 - Die Temp
        public string ResvCOMPort { set; get; } = "COM3";  // Omega CNi3233 - Reservoir Temp Reading
        public int ResvCOMID { set; get; } = 4; // Omega CNi3233 - Resv Temp Reader
        public int ResvBCOMID { set; get; } = 6; // Omega CNi3233 - Resv Temp Reader
        public string ResvHeaterCOMPort { set; get; } = "COM3"; // Omega CNi3233 - Reservoir Temp Control
        public int ResvHeaterCOMID { set; get; } = 5; // Omega CNi3233 - Resv Temp Reader
        public int ResvBHeaterCOMID { set; get; } = 7; // Omega CNi3233 - Resv Temp Reader
        public string AirKnifeHeaterCOMPort { set; get; } = "COM3"; // Omega CNi3233 - AirKnife Temp Control
        public int AirKnifeHeaterCOMID { get; set; } = 5;
        public int GasHeaterCOMID { get; set; } = 6;
        public int GasHeaterReaderCOMID { get; set; } = 7;

        public bool ChuckTempControlEnabled { set; get; } = false;
        public float ChuckTempControlSetPoint { set; get; } = 0;
        public float ChuckMaxTemp { set; get; } = 110;
        public bool DieTempControlEnabled { get; set; } = false;
        public float DieTempControlSetPoint { set; get; } = 0;
        public float DieMaxTemp { set; get; } = 110;

        public bool ReservoirTempControlEnabled { set; get; } = false;
        public bool ReservoirLimitControlEnabled { get; set; }

        public float ReservoirTempControlSetPoint { set; get; } = 0;
        public float ReservoirMaxTemp { set; get; } = 100;
        public float ReservoirHeaterMaxTemp { set; get; } = 200;

        public double AirKnifeHeaterTemperature { get; set; } = 35.0;
        public double AirKnifeHeaterSetPoint { get; set; } = 35.0;
        public double AirKnifeHeaterMaxTemperature { get; set; } = 160.0;
        public int AirKnifeHeaterWarmup { get; set; } = 90;

        public double GasHeaterTemperature { get; set; } = 35.0;
        public double GasHeaterSetPoint { get; set; } = 35.0;

        public bool AnyHeaterEnabled => ReservoirTempControlEnabled || ChuckTempControlEnabled || DieTempControlEnabled ||
            ReservoirLimitControlEnabled || AirKnifeHeaterInstalled;

        public bool DualPumpInstalled { set; get; } = false;
        public bool HasIRTransmitter { set; get; } = false;
        public bool HasDoorInterlocks { set; get; } = false;
        public bool AllowPumpMixing { set; get; } = false;
        public bool DisableHeadPurge { set; get; } = false;
        public bool HidePumpMixing { set; get; } = false;
        public bool DataLoggingEnabled { set; get; } = false;
        
        public string HeaterComPort
        {
            get
            {
                if (ChuckTempControlEnabled)
                {
                    return ChuckCOMPort;
                }
                else if (DieTempControlEnabled)
                {
                    return DieCOMPort;
                }
                else if (ReservoirTempControlEnabled)
                {
                    return ResvCOMPort;
                }
                else if (AirKnifeHeaterInstalled)
                {
                    return AirKnifeHeaterCOMPort;
                }
                else
                {
                    return "COM3";
                }
            }
        }

        #region Install Options

        public bool HasRaspberryPiRotary { get; set; } = false;
        public bool AirKnifeInstalled { get; set; } = false;
        public bool AirKnifeHeaterInstalled { get; set; } = false;
        public bool KeyenceLaserInstalled { get; set; } = false;
        public bool IRLampInstalled { get; set; } = false;
        public bool AdvantechServerInstalled { get; set; } = false;

        #endregion

        #region Keyence Lasers

        public bool UsingKeyenceLaser { get; set; } = false;

        public string KeyenceLaserIP { get; set; } = "192.168.1.20";

        public int KeyenceLaserProgramNumber { get; set; } = 0;
        public int NumberOfLasers { get; set; } = 0;

        #endregion

        #region IR Lamp

        public double IRIdlePower { get; set; } = 0.0;
        public int IRSlaveAddress { get; set; } = 11;
        public string IRCOMPort { get; set; } = "COM3";
        public double IRPostCoatDistance { set; get; } = 50.0;
        public double DieToIRPitch { get; set; } = 150;

        #endregion

        #region Locations

        public double VisionXPos { get; set; } = 300.000;    //mm
        public double VisionZPos { get; set; } = 49.000;    //mm

        #endregion

        #region Cognex Vision

        public bool CognexCommunicationsUsed { get; set; } = false;
        public string LeftCameraIPAddr { get; set; } = "";
        public string RightCameraIPAddr { get; set; } = "";
        public string VisionUserName { get; set; } = "admin";
        public string VisionPassword { get; set; } = "admin";

        #endregion

        #region Advantech Server

        public string AdvantechServerIPAddr { get; set; } = "10.10.1.120";
        public int AdvantechServerPort { get; set; } = 502;

        #endregion

        public double MaxPumpRate { get; set; } = 25000;    // ul/s
        public double ZMaintLoc { get; set; } = 40.000;
        public double XMaintLoc { get; set; } = 86.000;
        public double ZRDieLoadLoc { get; set; } = 36.001;
        public double ZLDieLoadLoc { get; set; } = 36.001;
        public double XDieLoadLoc { get; set; } = 0.0;
        public double MinVacuum { get; set; } = 16; // inHg
        public double MinAirPressure { get; set; } = 85.0;   // PSI
        public double MachineMaxXTravel { get; set; } = 780; // mm
        public double MaxXTravel { get; set; } = 765;   // mm
        public double MachineMaxZTravel { get; set; } = 60;    // mm
        public double MaxZTravel { get; set; } = 56;   // mm
        public double SafeCarriageLoc { get; set; } = 100;
        public double SafeLoaderLoc { get; set; } = 100;
        public double DefaultXAccel { get; set; } = 200;   // mm/s^2
        public double DefaultXDecel { get; set; } = 200;   // mm/s^2
        public double DefaultZAccel { get; set; } = 100;   // mm/s^2
        public double DefaultZDecel { get; set; } = 100;   // mm/s^2
        public int DefaultXSCurve { get; set; } = 100;   // %
        public int DefaultZSCurve { get; set; } = 100;   // %
        public int ValveSpeed { set; get; } = 200;
        public double ValveOffset { get; set; } = 0;
        public double ValveBOffset { get; set; } = 0;
        public bool HasLiftAndCenter { get; set; } = false;
        public bool LiftPinsEnabled { get; set; } = false;
        public bool DualZoneLiftPinsEnabled { get; set; } = false;
        public bool AutoLDULD { get; set; } = false;
        public bool HasPrimingArea {  get; set; } = false;
        public bool LiftPinsDuringPrep2Load { get; set; } = false;
        public int LiftPinsUpDelay { get; set; } = 2000;
        public int LiftPinsDownDelay { get; set; } = 2000;
        public bool AlignersEnabled { get; set; } = false;
        public int AlignersUpDelay { get; set; } = 2000;
        public int AlignersDownDelay { get; set; } = 500;
        public int GT2SettleDelay { get; set; } = 2000;
        public int AirPuffDelay { get; set; } = 250;
        public int RotaryValveMoveDelay { get; set; } = 1000;
        public double DieToAirKnifePitch { get; set; } = 100;
        public double AirKnifePostCoatDistance { get; set; } = 25.0;
        public int MinimumGap { get; set; } = 20;
        public int LevelingTolerance { set; get; } = 5;
        public int LevelingMovePercentage { set; get; } = 100;
        public double ConvHomeOffset { set; get; } = 0.0;
        public double ConvCoaterPosition { set; get; } = 0.0;
        public double ConvUnloadPosition { set; get; } = 0.0;
        public double ConvVelocity { set; get; } = 100;
        public double ConvAccDec { set; get; } = 100;
        public double ConvSCurve { set; get; } = 100;
        public double MaxLoaderTravel { set; get; } = 1100;

        public double LaserCalibrationUpperHeight { get; set; } = 1.127;

        #endregion

        #region Helper Functions

        public string GetActiveSelectiveZonesText()
        {
            string zoneText = "";

            if (SelectiveZone1Enabled)
            {
                zoneText += "1,";                               // Add Zone 1,
            }
            
            if (SelectiveZone2Enabled)
            {
                zoneText += "2,";                               // Add Zone 2,
            }

            if (SelectiveZone3Enabled)
            {
                zoneText += "3,";                               // Add Zone 3,
            }

            if (zoneText.Length == 0)
            {
                zoneText = "None";
            }
            else
            {
                zoneText = zoneText.Substring(0, zoneText.Length - 1); // strip the trailing comma
            }

            return zoneText;
        }

        #endregion

        #region File Management

        public string FileName { get; set; }
        [XmlIgnore]
        public string LastError = "";

        public MachineSettingsII() { }

        public static MachineSettingsII Load(string fileName)
        {
            MachineSettingsII ms = null;

            // Serialize the order to a file.
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MachineSettingsII));

                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    ms = (MachineSettingsII)serializer.Deserialize(fs);
                }
                
                ms.FileName = fileName;
                ms.LastError = "";
            }
            catch (Exception ex)
            {
                LogEntry.log(LogType.TRACE, Category.INFO, $"ERROR: Could not read MachineSettings File - {ex.Message}", "ERROR");
                throw ex;
            }

            return ms;
        }

        public bool Save(string sFileName = "MachineSettings.xml")
        {
            bool saved = false;

            try
            {
                // Serialize the order to a file.
                XmlSerializer serializer = new XmlSerializer(typeof(MachineSettingsII));

                using (FileStream fs = new FileStream(sFileName, FileMode.Create))
                {
                    serializer.Serialize(fs, this);
                }
                
                saved = true;
                LogEntry.log(LogType.TRACE, Category.INFO, "MachineSettings.xml Saved. ");
                LastError = "";
                LastUpdated = DateTime.Now;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                LogEntry.log(LogType.TRACE, Category.INFO, $"Could NOT save MachineSettings.xml - {ex.Message}", "ERROR");
            }

            return saved;
        }

        #endregion
    }
}
