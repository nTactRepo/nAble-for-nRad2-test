using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace nTact.PLC
{
    public static class StatusBits
    {
        public static Boolean InAutoMode = false;
        public static Boolean SystemInReset = false;
        public static Boolean RobotLoading = false;
        public static Boolean SubstrateLoading = false;
        public static Boolean SubstrateUnloading = false;
        public static Boolean SubstrateChucked = false;
        public static Boolean SubstrateOnChuck = false;
        public static Boolean SubstrateCoated = false;
        public static Boolean MainAirOK = false;
        public static Boolean MainVacOK = false;
        public static Boolean ChuckZone1VacOK = false;
        public static Boolean ChuckZone2VacOK = false;
        public static Boolean ChuckZone3VacOK = false;
        public static Boolean AlignersAreDown = false;
        public static Boolean OKToMoveCarriage = false;
        public static Boolean ReservoirNotEmpty = false;
        public static Boolean DisableModeChange = false;
        public static Boolean PDNotRunning = false;
        public static Boolean XInSafeArea = false;

    }

    class GlobalData
    {
		/// <summary>Size of Dword int[] array</summary>
		public static int DwordSize = 10000;
		/// <summary>Size of Wword int[] array</summary>
		public static int WwordSize = 10000;
		/// <summary>Size of SDword int[] array</summary>
		public static int SDwordSize = 10000;
		/// <summary>Size of Tword int[] array</summary>
		public static int TwordSize = 500;
		/// <summary>Size of Mbit bool[] array</summary>
		public static int MbitSize = 5000;
		/// <summary>Size of Xbit bool[] array</summary>
		public static int XbitSize = 5000;
		/// <summary>Size of Ybit bool[] array</summary>
		public static int YbitSize = 5000;
		/// <summary>Size of Bbit bool[] array</summary>
		public static int BbitSize = 5000;
		/// <summary>Size of Lbit bool[] array</summary>
		public static int LbitSize = 5000;

		/// <summary>D vals (int addrs)</summary>
		public static int[] Dword = new int[DwordSize];
		/// <summary>W vals (hex addrs)</summary>
		public static int[] Wword = new int[WwordSize];
		/// <summary>SD vals (int addrs)</summary>
		public static int[] SDword = new int[SDwordSize];
		/// <summary>T(imers) vals (int addrs)</summary>
		public static int[] Tword = new int[TwordSize];
		/// <summary>M bits (int addrs)</summary>
		public static bool[] Mbit = new bool[MbitSize];
		/// <summary>X bits (hex addrs)</summary>
		public static bool[] Xbit = new bool[XbitSize];
		/// <summary>Y bits (hex addrs)</summary>
		public static bool[] Ybit = new bool[YbitSize];
		/// <summary>B bits (hex addrs)</summary>
		public static bool[] Bbit = new bool[BbitSize];
		/// <summary>L bits (int addrs)</summary>
		public static bool[] Lbit = new bool[LbitSize];
        public static int[] arrBinValue = new int[16];
        public static TimeSpan PollingTime = new TimeSpan();
    }

}
