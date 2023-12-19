using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using nTact.PLC;

namespace nAble.Data
{
	[XmlRoot("MachineStorage")]
	public class MachineStorage
	{
        #region GT2 Storage

        [XmlElement] public double LeftGT2pos { get; set; } = 246.3;
		[XmlElement] public double RightGT2pos { get; set; } = 246.3;
		[XmlElement] public int SelectedBankNo { get; set; } = 0;

        #endregion

        #region Recirculation Storage

        [XmlElement] public int RecirculationCount { set; get; } = 1;
		[XmlElement] public int RecirculationInterval { set; get; } = 5; // mins

        #endregion

        #region Pump Priming Storage

        [XmlElement] public double SyringePrimingRate { set; get; } = 25; // µl/s
		[XmlElement] public double SyringePrimingRechargeRate { set; get; } = 25; // µl/s
		[XmlElement] public double SyringeBPrimingRate { set; get; } = 25; // µl/s
		[XmlElement] public double SyringeBPrimingRechargeRate { set; get; } = 25; // µl/s
		[XmlElement] public int SyringePrimingCount { set; get; } = 3;
		[XmlElement] public int SyringeBPrimingCount { set; get; } = 3;

        #endregion

        #region Head Prime Storage

        [XmlElement] public double HeadPrimeRate { set; get; } = 25; // µl/s
		[XmlElement] public double HeadPrimeRechargeRate { set; get; } = 25; // µl/s
		[XmlElement] public int HeadPrimingCount { set; get; } = 5;

        #endregion

        #region Head Purge Storage

        [XmlElement] public double HeadPurgeRate { set; get; } = 25; // µl/s
		[XmlElement] public double HeadPurgeRechargeRate { set; get; } = 25; // µl/s
		[XmlElement] public double HeadPurgingVolume { set; get; } = 250; // µl

		#endregion

		#region Counters

		[XmlElement] public double CycleCount { set; get; } = 0;
		[XmlElement] public double LifetimeCount { set; get; } = 0;

		#endregion

        #region Pump Selection Storage

        [XmlElement] public int SelectedPump { set; get; } = 0; //0=Pump A, 1=Pump B, 2=Mix
		[XmlElement] public double PumpARatio { set; get; } = 100; // % Total Dispense

        #endregion

        #region Motor Pulses

        [XmlElement] public int MaxPOHPulses { set; get; } = 500000;
		[XmlElement] public int MaxSyringePulses { set; get; } = 240000;
		[XmlElement] public int MaxPOHBPulses { set; get; } = 500000;
		[XmlElement] public int MaxSyringeBPulses { set; get; } = 240000;

		#endregion

		#region Misc Storage Properties

		[XmlIgnore] public static LogEntry LogEntry = null;
		[XmlElement] public int PressureSmoothingSamples { set; get; } = 10;

		#endregion

		#region File Management

		[XmlIgnore] public string LastError = "";
		[XmlIgnore] public string FileName { get; private set; }
		public MachineStorage() { }
		public static MachineStorage Load(string sFileName)
		{
			MachineStorage oRetVal = null;

			// Serialize the order to a file.
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(MachineStorage));
				FileStream fs = new FileStream(sFileName, FileMode.Open);
				oRetVal = (MachineStorage)serializer.Deserialize(fs);
				fs.Close();
				oRetVal.FileName = sFileName;
				oRetVal.LastError = "";
			}
			catch (Exception ex)
			{
				LogEntry.log(LogType.TRACE, Category.INFO, "ERROR: Could not read MachineSettings File - }" + ex.Message, "ERROR");
				oRetVal = null;
				throw ex;
			}

			return oRetVal;
		}
		public bool Save(string sFileName = "MachineStorage.xml")
		{
			bool bRetVal = false;
			try
			{
				// Serialize the order to a file.
				XmlSerializer serializer = new XmlSerializer(typeof(MachineStorage));
				FileStream fs = new FileStream(sFileName, FileMode.Create);
				serializer.Serialize(fs, this);
				fs.Close();
				bRetVal = true;
				LogEntry.log(LogType.TRACE, Category.INFO, "MachineStorage.xml Saved. ");
				LastError = "";
			}
			catch (Exception ex)
			{
				LastError = ex.Message;
				LogEntry.log(LogType.TRACE, Category.INFO, "Could NOT save MachineStorage.xml - " + ex.Message, "ERROR");
			}
			return bRetVal;
		}

		#endregion File Management
	}
}
