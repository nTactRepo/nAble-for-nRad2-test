using System;

namespace nAble.DataComm.KeyenceLasers
{
    public class LaserData : ICloneable
    {
        public const double MaxLaserValue = 99.998;

        public int HeadNumber { get; set; }

        public double Value { get; set; }

        public string ValueAsString => ValueToString();

        public bool Valid => Value != 0 && !Invalid;

        public bool Go { get; set; }
        public bool Hi { get; set; }
        public bool Lo { get; set; }

        public bool Alarm { get; set; }
        public bool Invalid { get; set; }
        public bool Waiting { get; set; }

        /// <summary>
        /// The time that this laser data was read
        /// </summary>
        public DateTime ReadTimestamp { get; set; }

        public object Clone()
        {
            var ld = new LaserData
            {
                HeadNumber = HeadNumber,
                Go = Go,
                Hi = Hi,
                Lo = Lo,
                Alarm = Alarm,
                Invalid = Invalid,
                Waiting = Waiting,
                Value = Value
            };

            return ld;
        }

        private string ValueToString()
        {
            bool outOfRange = Math.Abs(Value) > MaxLaserValue;
            return outOfRange ? "-FFFFFF" : $"{Value:#0.000}";
        }
    }
}
