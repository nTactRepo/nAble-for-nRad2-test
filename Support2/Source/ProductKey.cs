using System;
using System.Text;
using System.Xml.Serialization;

namespace Support2
{
    public class ProductKey : IEquatable<ProductKey>
    {
        #region Properties

        [XmlIgnore]
        public byte[] ValueAsBytes
        {
            get => GetBytes();
            set => SetFromBytes(value);
        }

        public string ValueAsString
        {
            get => GetString();
            set => SetFromString(value);
        }

        #endregion

        #region Data Members

        private string _stringVal = "0000000000000000";

        #endregion

        #region Functions

        #region Constructors

        public ProductKey() { }

        public ProductKey(byte[] bytes) => SetFromBytes(bytes);

        public ProductKey(string key) => SetFromString(key);

        #endregion

        #region Public Functions

        public override string ToString() => ValueAsString;

        #endregion

        #region IEquitable

        public bool Equals(ProductKey other) => _stringVal == other?._stringVal;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                var data = (ProductKey)obj;
                return Equals(data);
            }
        }

        public override int GetHashCode() => _stringVal.GetHashCode();

        #endregion

        #region Private Functions

        private byte[] GetBytes()
        {
            byte[] bytes = new byte[8];

            for (int i = 0; i < 8; i++)
            {
                bytes[i] = Convert.ToByte(_stringVal.Substring(i * 2, 2), 8);
            }

            return bytes;
        }

        private void SetFromBytes(byte[] value)
        {
            SetFromString(BitConverter.ToString(value));
        }

        private string GetString() => FormatString(_stringVal);

        private void SetFromString(string value)
        {
            var newStr = StripString(value);

            if (newStr.Length != 16)
            {
                return;
            }

            _stringVal = newStr;
        }

        private string StripString(string str) => str.Replace("-", "");

        private string FormatString(string str)
        {
            StringBuilder sb = new StringBuilder();

            // XXXX-XXXX-XXXX-XXXX
            for (int i = 0; i < 4; i++)
            {
                sb.Append($"{str.Substring(i*4, 4)}-");
            }

            var newStr = sb.ToString();
            return newStr.Substring(0, newStr.Length - 1);
        }

        #endregion

        #endregion
    }
}
