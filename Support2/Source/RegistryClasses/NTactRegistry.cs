using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Support2.RegistryClasses
{
    public class NTactRegistry
    {
        #region Constants

        private const string BaseKeyStr = @"Software\nTact\nAble";
        private const string PrivateKey = "J*D6$________________Gwl";
        private const int SerialIndex = 5;
        private const string IVStatic = "pn@t9q________~+";
        private const int CreatedIndex = 6;

        private static readonly byte[] Diffuser = new byte[] { 1, 15, 4, 2, 11, 9, 8, 7 };

        private static readonly DateTime DefaultTime = new DateTime(2000, 1, 1);

        #endregion

        #region Properties

        public bool IsValid
        {
            get
            {
                var oldProductKey = ProductKey;
                var newProductKey = CreateProductKey();
                return oldProductKey.Equals(newProductKey);
            }
        }

        public ProductKey ProductKey
        {
            get => new ProductKey(_locations[RegName.ProductKey].Value as string);
            set => _locations[RegName.ProductKey].Value = value.ValueAsString;
        }

        public DateTime ExpirationDate
        {
            get => new DateTime(Convert.ToInt64(_locations[RegName.ExpirationDate].Value));
            set => _locations[RegName.ExpirationDate].Value = value.Ticks;
        }

        public DateTime CreationDate
        {
            get => new DateTime(Convert.ToInt64(_locations[RegName.CreationDate].Value));
            set => _locations[RegName.CreationDate].Value = value.Ticks;
        }

        public FeatureMask Features { get; private set; } = null;

        public int Other
        {
            get => Convert.ToInt32(_locations[RegName.Other].Value);
            set => _locations[RegName.Other].Value = value;
        }

        public string SerialNumber
        {
            get => (string)_locations[RegName.SerialNumber].Value;
            private set => _locations[RegName.SerialNumber].Value = value;
        }

        public string ProjectNumber
        {
            get => (string)_locations[RegName.ProjectNumber].Value;
            private set => _locations[RegName.ProjectNumber].Value = value;
        }

        private string HardwareId
        {
            get => (string)_locations[RegName.HWID].Value;
            set => _locations[RegName.HWID].Value = value;
        }

        public string DebugString => $"PK:\t{ProductKey}\nCreate:\t{CreationDate}\nExp:\t{ExpirationDate}\nPC:\t{Features.ProductCode}\nSerial:\t{SerialNumber}\nProj:\t{ProjectNumber}";

        #endregion

        #region Member Data

        private readonly Dictionary<RegName, RegistryLocation> _locations = null;
        private readonly RegistryKey _base = null;
        private readonly LogStringSource _logger = null;

        #endregion

        #region Functions

        #region Constructors

        public NTactRegistry(LogStringSource logger)
        {
            CheckKeySizes();
            _logger = logger;

            // Always open the registry in x86 mode, so that we are always seeing the same keys!!!
            RegistryKey start = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            _base = start.OpenSubKey(BaseKeyStr, writable: true);

            if (_base is null)
            {
                _base = start.CreateSubKey(BaseKeyStr, true) ?? throw new Exception("Unable to create base registry key!");
            }

            _locations = InitializeLocations();
            Features = new FeatureMask();
            object temp = _locations[RegName.FeatureMask].Value;
            Features.Value = Convert.ToInt64(_locations[RegName.FeatureMask].Value ?? 0L);
            Features.Changed += Features_Changed;
        }

        #endregion

        #region Public Functions

        public ProductKey CreateProductKey(long createTicks = -1, long expireTicks = -1, long features = -1, string serial = "")
        {
            Aes aes = Aes.Create();

            long create = createTicks == -1 ? Convert.ToInt64(_locations[RegName.CreationDate].Value) : createTicks;
            long expire = expireTicks == -1 ? Convert.ToInt64(_locations[RegName.ExpirationDate].Value) : expireTicks;
            long featureMask = features == -1 ? Convert.ToInt64(_locations[RegName.FeatureMask].Value) : features;
            string serialToUse = string.IsNullOrEmpty(serial) ? SerialNumber : serial;

            var iv = GetIV(create);
            var key = GetPrivateKey(serialToUse);
            var input = GetInput(expire, featureMask);

            MemoryStream outStream = new MemoryStream();
            CryptoStream cs = new CryptoStream(outStream, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);

            var cryptoBytes = outStream.ToArray();
            byte[] outBytes = new byte[8];

            // Get a smaller number of bytes for ease of user entry.  But do it in a weird fashion
            for (int i = 0; i < 8; i++)
            {
                int j = Diffuser[i];
                outBytes[i] = cryptoBytes[Diffuser[i]];
            }

            return new ProductKey(outBytes);
        }

        public void WriteNewHardwareId(string serialNum, string projectNum)
        {
            HardwareId = CreateHardwareId();
            SerialNumber = serialNum;
            ProjectNumber = projectNum;
        }

        public bool CheckHardwareId()
        {
            string verify = CreateHardwareId();
            bool matched = verify == HardwareId;

#if DEBUG
            _logger.SendLog($"HWID written:\n  {HardwareId}\nverify:\n  {verify}");
#endif

            return matched;
        }

        #endregion

        #region Private Functions

        private Dictionary<RegName, RegistryLocation> InitializeLocations()
        {
            Dictionary<RegName, RegistryLocation> dict = new Dictionary<RegName, RegistryLocation>()
            {
                { RegName.CreationDate, new EncryptedRegistryLocation(RegistryValueKind.QWord, "InitialState", _base) },
                { RegName.ExpirationDate, new EncryptedRegistryLocation(RegistryValueKind.QWord, "Expiry", _base) },
                { RegName.FeatureMask, new EncryptedRegistryLocation(RegistryValueKind.QWord, "Features", _base) },
                { RegName.ProductKey, new EncryptedRegistryLocation(RegistryValueKind.String, "Validator", _base) },
                { RegName.Other, new EncryptedRegistryLocation(RegistryValueKind.DWord, "Other", _base) },
                { RegName.HWID, new EncryptedRegistryLocation(RegistryValueKind.String, "HWID", _base) },
                { RegName.SerialNumber, new EncryptedRegistryLocation(RegistryValueKind.String, "SerialNo", _base) },
                { RegName.ProjectNumber, new EncryptedRegistryLocation(RegistryValueKind.String, "ProjectNo", _base) }
            };

            return dict;
        }

        private void CheckKeySizes()
        {
            if (IVStatic.Length != 16) throw new Exception("IV length should be 16!");
            if (PrivateKey.Length != 24) throw new Exception("Private key length should be 24!");
        }

        private byte[] GetIV(long create)
        {
            byte[] iv = Encoding.ASCII.GetBytes(IVStatic);
            long ticks = create;
            DateTime check = new DateTime(ticks);
            var createDate = BitConverter.GetBytes(ticks);
            Array.Copy(createDate, 0, iv, CreatedIndex, createDate.Length);
            return iv;
        }

        private byte[] GetInput(long expire, long featureMask)
        {
            byte[] input = new byte[16];
            
            DateTime check = new DateTime(expire);
            var expireDate = BitConverter.GetBytes(expire);
            Array.Copy(expireDate, 0, input, 0, 8);
            
            var featuresBytes = BitConverter.GetBytes(featureMask);
            Array.Copy(featuresBytes, 0, input, 8, 8);

            return input;
        }

        private byte[] GetPrivateKey(string serial)
        {
            byte[] privKey = Encoding.ASCII.GetBytes(PrivateKey);
            var serNumBytes = Encoding.ASCII.GetBytes(serial);
            int numToCopy = Math.Min(16, serial.Length);
            Array.Copy(serNumBytes, 0, privKey, SerialIndex, numToCopy);
            return privKey;
        }

        private void Features_Changed()
        {
            _locations[RegName.FeatureMask].Value = Features.Value;
        }

        private string CreateHardwareId()
        {
            string processorId = "";
            string systemId = string.Empty;

            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select ProcessorID From Win32_processor");

            foreach (ManagementBaseObject mo in mos.Get())
            {
                processorId = mo["ProcessorID"] as string;
            }

            mos = new ManagementObjectSearcher("Select UUID From Win32_ComputerSystemProduct");

            foreach (ManagementBaseObject mo in mos.Get())
            {
                systemId = mo["UUID"] as string;
            }

            systemId = systemId.Replace("-", "");

            return $"{processorId}{systemId}";
        }

        #endregion

        #endregion
    }
}
