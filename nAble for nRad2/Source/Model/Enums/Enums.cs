using System;

namespace nTact.PLC
{
    public static class Enums
    {
        /// <summary>
        /// Recipe section.
        /// </summary>
        public enum RecipeType : int
        {
            /// <summary>
            /// PreProcess Recipe.
            /// </summary>
            PreProcess = 0,

            /// <summary>
            /// Coating Recipe.
            /// </summary>
            Coating = 1,

            /// <summary>
            /// PostProcess Recipe.
            /// </summary>
            PostProcess = 2,

            /// <summary>
            /// Master Recipe.
            /// </summary>
            Master = 3
        }

        public enum PLCDataRegister : int
        {
            /// <summary>
            /// Data Register [D]
            /// </summary>
            Data=0,

            /// <summary>
            /// Link Register [W]
            /// </summary>
            Link = 1,

            /// <summary>
            /// Link Special Register [LW]
            /// </summary>
            LinkSpecial =2,

            /// <summary>
            /// Special Register [SD]
            /// </summary>
            SpecialRegister = 3
        }

		public enum StackModuleType : int
		{
			/// <summary>
			/// None (Not Defined or Installed)
			/// </summary>
			None = 0,

			/// <summary>
			/// Hotplate (HP)
			/// </summary>
			Hotplate = 1,

			/// <summary>
			/// VacuBake (VB)
			/// </summary>
			VacuBake = 2,

			/// <summary>
			/// VacuBake+ (VB+)
			/// </summary>
			VacuBakePlus = 3,

			/// <summary>
			/// VacuDry (VD)
			/// </summary>
			VacuDry = 4,

			/// <summary>
			/// VacuDry+ (VD+)
			/// </summary>
			VacuDryPlus = 5,

			/// <summary>
			/// ChillPlate (CP)
			/// </summary>
			ChillPlate = 6
		}

		public static bool ModuleHasVac(StackModuleType modType)
		{
			bool bRetVal = false;
			bRetVal |= modType == StackModuleType.VacuBake;
			bRetVal |= modType == StackModuleType.VacuBakePlus;
			bRetVal |= modType == StackModuleType.VacuDry;
			bRetVal |= modType == StackModuleType.VacuDryPlus;
			return bRetVal;
		}

		public static string ModuleTypeName(int imt, bool bShortName = false)
		{
			return ModuleTypeName((Enums.StackModuleType)imt, bShortName);
		}
		public static string ModuleTypeName(StackModuleType smt, bool bShortName = false)
		{
			string sRetVal = "Unknown";
			switch (smt)
			{
				case StackModuleType.Hotplate: { sRetVal = bShortName ? "(HP)" : "Hotplate"; } break;
				case StackModuleType.VacuBake: { sRetVal = bShortName ? "(VB)" : "VacuBake"; } break;
				case StackModuleType.VacuBakePlus: { sRetVal = bShortName ? "(VB+)" : "VacuBake+"; } break;
				case StackModuleType.VacuDry: { sRetVal = bShortName ? "(VD)" : "VacuDry"; } break;
				case StackModuleType.VacuDryPlus: { sRetVal = bShortName ? "(VD+)" : "VacuDry+"; } break;
				case StackModuleType.ChillPlate: { sRetVal = bShortName ? "(CP)" : "ChillPlate"; } break;
				default: { } break;
			}
			return sRetVal;
		}
	}
}
