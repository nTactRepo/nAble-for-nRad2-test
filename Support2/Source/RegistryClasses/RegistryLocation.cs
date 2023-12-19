using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Support2.RegistryClasses
{
    public class RegistryLocation
    {
        #region Properties

        public object Value
        {
            get => GetValue();

            set => SetValue(value);
        }

        public RegistryValueKind Kind { get; }

        public string Name { get; }

        public RegistryKey BaseRegistryKey { get; }

        #endregion

        #region Functions

        #region Constructors

        public RegistryLocation(RegistryValueKind kind, string name, RegistryKey baseKey, string subDirKeyStr = null)
        {
            Kind = kind;
            Name = name;
            BaseRegistryKey = baseKey ?? throw new ArgumentNullException("baseKey");

            if (!string.IsNullOrEmpty(subDirKeyStr))
            {
                BaseRegistryKey = BaseRegistryKey.OpenSubKey(subDirKeyStr, true) ?? throw new Exception("subDirKeyStr did not point at a valid registry key");
            }
        }

        #endregion

        #region Public Functions

        #endregion

        #region Private Functions

        protected virtual object GetValue()
        {
            return BaseRegistryKey.GetValue(Name, GetDefault(Kind));
        }

        protected virtual void SetValue(object value)
        {
            BaseRegistryKey.SetValue(Name, value, Kind);
        }

        protected object GetDefault(RegistryValueKind kind)
        {
            switch (kind)
            {
                case RegistryValueKind.DWord: return 0;
                case RegistryValueKind.QWord: return 0;
                case RegistryValueKind.String: return "";
                default: return null;
            }
        }

        #endregion

        #endregion
    }
}
