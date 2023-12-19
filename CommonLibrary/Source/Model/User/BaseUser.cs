using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace CommonLibrary.Model.User
{
    public class BaseUser : IComparable<BaseUser>, IEquatable<BaseUser>
    {
        #region Constants

        private static readonly byte[] Secret = Encoding.ASCII.GetBytes("nT@ctFhG");

        public const string NotLoggedOnName = "Not Logged On";

        #endregion

        #region Data Members

        private string _password = "";
        private string _encryptedPassword = "";

        #endregion

        #region Properties

        [XmlAttribute]
        public string Name { get; set; } = "";

        [XmlIgnore]
        public string Password
        {
            get => _password;

            set
            {
                _password = value;
                _encryptedPassword = EncryptPassword(value);
            }
        }

        public string EncryptedPassword
        {
            get => _encryptedPassword;

            set
            {
                _encryptedPassword = value;
                _password = DecryptPassword(value);
            }
        }

        #endregion

        #region Functions

        #region Constructors

        public BaseUser() { }

        public BaseUser(string sName)
        {
            Name = sName;
        }

        #endregion

        #region Public Functions

        public override string ToString() => Name;

        public bool Validate(string name, string password)
        {
            return Name.ToLower() == name.ToLower() && Password == password;
        }

        public virtual void FillInNTactUser() { }

        #endregion

        #region Private Functions

        private string EncryptPassword(string originalString)
        {
            if (string.IsNullOrEmpty(originalString))
            {
                return "";
            }
            else
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(Secret, Secret), CryptoStreamMode.Write);
                StreamWriter writer = new StreamWriter(cryptoStream);
                writer.Write(originalString);
                writer.Flush();
                cryptoStream.FlushFinalBlock();
                writer.Flush();
                return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            }
        }

        private string DecryptPassword(string cryptedString)
        {
            if (string.IsNullOrEmpty(cryptedString))
            {
                return "";
            }
            else
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString));
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(Secret, Secret), CryptoStreamMode.Read);
                StreamReader reader = new StreamReader(cryptoStream);
                return reader.ReadToEnd();
            }
        }

        #endregion

        #region IComparable

        public virtual int CompareTo(BaseUser other)
        {
            return Name.CompareTo(other?.Name);
        }

        #endregion

        #region IEquitable

        public virtual bool Equals(BaseUser other)
        {
            return Name.Equals(other?.Name) && Password.Equals(other?.Password);
        }

        #endregion

        #endregion
    }
}
