using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Support2.RegistryClasses
{
    public class EncryptedRegistryLocation : RegistryLocation
    {
        #region Constants

        private static readonly byte[] Entropy = new byte[] { 231, 18, 42, 69, 111, 111, 3 };

        #endregion

        public EncryptedRegistryLocation(RegistryValueKind kind, string name, RegistryKey baseKey, string subDirKeyStr = null)
            : base(kind, name, baseKey, subDirKeyStr) { }

        protected override object GetValue()
        {
            var objValue = BaseRegistryKey.GetValue(Name, "");

            if (objValue is string str && !string.IsNullOrEmpty(str))
            {
                return UnProtect((string)objValue, Kind);
            }
            else
            {
                return GetDefault(Kind);
            }
        }

        protected override void SetValue(object value)
        {
            var encrStr = Protect(value, Kind);
            BaseRegistryKey.SetValue(Name, encrStr, RegistryValueKind.String);
        }

        private string Protect(object value, RegistryValueKind kind)
        {
            switch (kind)
            {
                case RegistryValueKind.String:
                {
                    var input = Encoding.ASCII.GetBytes((string)value);
                    var answer = ProtectedData.Protect(input, Entropy, DataProtectionScope.LocalMachine);
                    return Convert.ToBase64String(answer);
                }

                case RegistryValueKind.DWord:
                {
                    var input = BitConverter.GetBytes(Convert.ToUInt32(value));
                    var answer = ProtectedData.Protect(input, Entropy, DataProtectionScope.LocalMachine);
                    return Convert.ToBase64String(answer);
                }

                case RegistryValueKind.QWord:
                {
                    var input = BitConverter.GetBytes(Convert.ToUInt64(value));
                    var answer = ProtectedData.Protect(input, Entropy, DataProtectionScope.LocalMachine);
                    return Convert.ToBase64String(answer);
                }

                default:
                    Debug.Assert(false, "Unknown registry kind in Protect()");
                    return "";
            }
        }

        private object UnProtect(string value, RegistryValueKind kind)
        {
            switch (kind)
            {
                case RegistryValueKind.String:
                {
                    var input = Convert.FromBase64String(value);
                    var answer = ProtectedData.Unprotect(input, Entropy, DataProtectionScope.LocalMachine);
                    return Encoding.ASCII.GetString(answer);
                }

                case RegistryValueKind.DWord:
                {
                    var input = Convert.FromBase64String(value);
                    var answer = ProtectedData.Unprotect(input, Entropy, DataProtectionScope.LocalMachine);
                    return BitConverter.ToUInt32(answer, 0);
                }

                case RegistryValueKind.QWord:
                {
                    var input = Convert.FromBase64String(value);
                    var answer = ProtectedData.Unprotect(input, Entropy, DataProtectionScope.LocalMachine);
                    return BitConverter.ToUInt64(answer, 0);
                }

                default:
                    Debug.Assert(false, "Unknown registry kind in Unprotect()");
                    return null;
            }
        }

    }

}
