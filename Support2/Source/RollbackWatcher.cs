using Support2.RegistryClasses;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Support2
{
    public class RollbackWatcher
    {
        #region Events

        #endregion

        #region Constants

        private const long FileMask = 0x728841FFB3921122;

        #endregion

        #region Properties

        public bool RollbackDetected { get; private set; } = false;
        public bool RollbackFileDeleted { get; private set; } = false;

        public bool IsOK => !RollbackDetected && !RollbackFileDeleted;

        public LicenseFailCode CurrentStatus
        {
            get
            {
                if (RollbackFileDeleted) return LicenseFailCode.ClockRollbackFileDeleted;
                if (RollbackDetected) return LicenseFailCode.DetectedClockRollback;
                return LicenseFailCode.Valid;
            }
        }

        public DateTime LatestTime { get; private set; } = DateTime.MinValue;

        #endregion

        #region Member Data

        private readonly NTactRegistry _reg = null;
        private readonly LogStringSource _logger = null;

        private string _filePath = "";

        #endregion

        #region Functions

        #region Constructors

        public RollbackWatcher(LogStringSource logger, NTactRegistry reg)
        {
            _reg = reg;
            _logger = logger;

            RollbackFileDeleted = _reg.Features.GetBit(FeatureMaskBits.RollbackFileDeleted);
            RollbackDetected = _reg.Features.GetBit(FeatureMaskBits.RollbackDetected);

            if (!IsOK)
            {
                _logger.SendLog($"Corruption found in license data.  Please contact nTact Support.");
            }

            _filePath = GetFileFullPath();
            CreateDirIfNeeded(Path.GetDirectoryName(_filePath));
        }

        #endregion

        #region Public Functions

        public void CheckForRollback()
        {
            if (!IsOK)
            {
                return;
            }

            CreateDirIfNeeded(Path.GetDirectoryName(_filePath));

            // If someone removed the file after we have once written it, fail out
            if (_reg.Other == 1 && !File.Exists(_filePath))
            {
                RollbackFileDeleted = true;
                _logger.SendLog("License file data has detected corruption");
                _reg.Features.SetBit(FeatureMaskBits.RollbackFileDeleted, true);
                _reg.ProductKey = new ProductKey();
                return;
            }

            CreateOrUpdateTimeSaveFile();
        }

        #endregion

        #region Private Functions

        private void CreateOrUpdateTimeSaveFile()
        {
            bool fileExists = File.Exists(_filePath);
            DateTime fileTime = DateTime.MinValue;

            if (fileExists)
            {
                fileTime = ReadFile();
            }

            // If current time is much smaller than last time, then flag a rollback
            // Note: The added day avoids issues like time zones, etc.
            if (DateTime.Now.AddDays(1) < fileTime)
            {
                RollbackDetected = true;
                return;
            }

            WriteFile(DateTime.Now);
        }

        private string GetFileFullPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                    "Data", "LastReading.dat");
        }

        private void CreateDirIfNeeded(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);

                var older = DateTime.Now - new TimeSpan(165, 165, 165, 0);

                // set directory date so it is indistinguishable from other folders and won't sort to top of a date list:
                Directory.SetCreationTime(dir, older);
                Directory.SetLastAccessTime(dir, older);
                Directory.SetLastWriteTime(dir, older);
            }
        }

        private DateTime ReadFile()
        {
            byte[] bytes = new byte[8];

            try
            {
                using (var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                {
                    stream.Read(bytes, 0, 8);
                }

                long newVal = BitConverter.ToInt64(bytes, 0) ^ FileMask;
                return DateTime.FromBinary(newVal);
            }
            catch (Exception)
            {
                _logger.SendLog("Problem reading time file.");
            }

            return DateTime.MinValue;
        }

        private void WriteFile(DateTime time)
        {
            try
            {
                var newVal = time.ToBinary() ^ FileMask;
                var bytes = BitConverter.GetBytes(newVal);

                using (var stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                _reg.Other = 1;

                var older = DateTime.Now - new TimeSpan(165, 165, 165, 0);

                // set directory date so it is indistinguishable from other folders and won't sort to top of a date list:
                File.SetCreationTime(_filePath, older);
                File.SetLastAccessTime(_filePath, older);
                File.SetLastWriteTime(_filePath, older);

                Thread.Sleep(10);

                var dir = Path.GetDirectoryName(_filePath);
                Directory.SetLastAccessTime(dir, older);
                Directory.SetLastWriteTime(dir, older);
            }
            catch (Exception ex)
            {
                _logger.SendLog($"Problem writing out the time file: {ex.Message}");
            }
        }

        #endregion

        #endregion
    }
}
