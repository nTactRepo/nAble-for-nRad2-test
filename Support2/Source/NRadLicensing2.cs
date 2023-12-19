using Support2.RegistryClasses;
using System;

namespace Support2
{
    public class NRadLicensing2
    {
        #region Properties

        public LicenseFailCode CurrentLicenseState { get; private set; } = LicenseFailCode.Uninitialized;
        public bool IsExpired => CheckIfExpired();
        public bool IsActivated => _reg.Features.Activated;
        public ProductKey ProductKey => _reg.ProductKey;

        public string SerialNumber => _reg.SerialNumber;
        public string ProjectNo => _reg.ProjectNumber;

        public DateTime ExpireDate => _reg.ExpirationDate;

        public string DebugString => _reg.DebugString;

        public RollbackWatcher RollbackWatcher { get; private set; } = null;

        public LogStringSource Logger { get; } = null;
        internal DateTime lastCheckValidityLogTimeStamp { get; private set; }
        internal int lastCheckValidityCode { get; private set; } = 99;

        #endregion

        #region Member Data

        private readonly NTactRegistry _reg = null;

        #endregion

        #region Functions

        #region Constructors

        public NRadLicensing2()
        {
            Logger = new LogStringSource();
            _reg = new NTactRegistry(Logger);
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Check License Validity.
        /// </summary>
        /// <param name="logIsValid"></param>
        /// <param name="logFreqency"></param>
        /// <param name="forceLogEntry"></param>
        /// <returns></returns>
        public LicenseFailCode CheckLicenseValidity(bool logIsValid = false, int logFreqency = 60, bool forceLogEntry = false)
        {
            LicenseFailCode code = LicenseFailCode.Valid;
            int validityCode = 0;
            string logMsg = "";

            if (_reg.IsValid)
            {
                RollbackWatcher = new RollbackWatcher(Logger, _reg);
                RollbackWatcher.CheckForRollback();
                code = RollbackWatcher.CurrentStatus;

                if (!RollbackWatcher.IsOK)
                {
                    validityCode = 1;
                    logMsg="License Manager detected clock variation";
                }
                else if (code == LicenseFailCode.Valid && IsExpired)
                {
                    validityCode = 2;
                    logMsg = "License is expired";
                    code = LicenseFailCode.Expired;
                }
                else if (code == LicenseFailCode.Valid && !CheckHardwareId())
                {
                    validityCode = 3;
                    logMsg = "HWID Mismatch";
                    code = LicenseFailCode.HWIDMismatch;
                }
                else if (code == LicenseFailCode.Valid)
                {
                    validityCode = 5;
                    logMsg = "License is valid";
                }
            }
            else
            {
                validityCode = 4;
                code = GetValidationFailedStatus();
                logMsg = "License validation failed";
            }

            if(validityCode>0)
            {
                bool bLogEntry = forceLogEntry || validityCode != lastCheckValidityCode || ((DateTime.Now.Subtract(lastCheckValidityLogTimeStamp).TotalSeconds > logFreqency) && ((validityCode == 5 && logIsValid) || validityCode < 5));
                if (bLogEntry)
                {
                    Logger.SendLog(logMsg);
                    lastCheckValidityCode = validityCode;
                    lastCheckValidityLogTimeStamp = DateTime.Now;
                }
            }
            CurrentLicenseState = code;
            return code;
        }

        public ProductKey CreateProductKey(DateTime created, DateTime expired, FeatureMask mask, string serialNum)
        {
            return _reg.CreateProductKey(MakeDateOnly(created).Ticks, MakeDateOnly(expired).Ticks, mask.Value, serialNum);
        }

        public bool UpdateLicenseData(DateTime created, DateTime expired, long mask, ProductKey key)
        {
            try
            {
                _reg.CreationDate = created;
                _reg.ExpirationDate = expired;
                _reg.Features.Value = mask;
                _reg.ProductKey = key;
                _reg.Other = 0;
                Logger.SendLog($"License data updated: Creation:{created.ToShortDateString()}, Expire:{expired.ToShortDateString()}, Mask:{mask}, Validator:{key}");
                CheckLicenseValidity();
                return CurrentLicenseState == LicenseFailCode.Valid;
            }
            catch (Exception ex)
            {
                Logger.SendLog($"Failure caught during license update: {ex.Message}");
                return false;
            }
        }

        public void CreateHWID(string serialNum, string projectNum)
        {
            _reg.WriteNewHardwareId(serialNum, projectNum);
        }

        public bool CheckHardwareId()
        {
            return _reg.CheckHardwareId();
        }

        public static DateTime MakeDateOnly(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day);
        }

        public bool IsFeatureActive(LicensedFeautres feature) => _reg.Features.GetBit((int)feature);

        public bool ExpiresWithinXDays(int numDays)
        {
            // If the license isn't currently ok, then it has already expired
            if (CurrentLicenseState != LicenseFailCode.Valid)
            {
                return true;
            }

            // If the coater is Activated, then it will not expire
            if (IsActivated)
            {
                return false;
            }

            // Otherwise, check the number of days
            var difference = _reg.ExpirationDate - DateTime.Now;

            return difference.Days < numDays;
        }

        #endregion

        #region Private Functions

        private bool CheckIfExpired()
        {
            if (IsActivated)
            {
                return false;
            }

            bool expired = DateTime.Now > MakeDateOnly(_reg.ExpirationDate);

            if (expired)
            {
                Logger.SendLog("License is expired!");
                CurrentLicenseState = LicenseFailCode.Expired;
            }

            return expired;
        }

        private LicenseFailCode GetValidationFailedStatus()
        {
            if (_reg.Features.GetBit(FeatureMaskBits.RollbackDetected))
            {
                return LicenseFailCode.DetectedClockRollback;
            }
            else if (_reg.Features.GetBit(FeatureMaskBits.RollbackFileDeleted))
            {
                return LicenseFailCode.ClockRollbackFileDeleted;
            }
            else
            {
                return LicenseFailCode.ValidationFailed;
            }
        }

        #endregion

        #endregion
    }
}
