using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support2.RegistryClasses
{
    public enum FeatureMaskBits
    {
        Feature1 = 0,
        Feature2 = 1,
        Feature3 = 2,
        Feature4 = 3,
        Feature5 = 4,
        Feature6 = 5,
        Feature7 = 6,
        Feature8 = 7,
        Feature9 = 8,
        Feature10 = 9,
        Feature11 = 10,
        Feature12 = 11,
        Feature13 = 12,
        Feature14 = 13,
        Feature15 = 14,
        Feature16 = 15,
        Activated = 16,
        RollbackDetected = 17,
        RollbackFileDeleted = 18,
        FixedOff1 = 24,
        FixedOn1 = 25,
        FixedOff2 = 26,
        FixedOn2 = 27,
        FixedOff3 = 28,
        FixedOn3 = 29,
        FixedOn4 = 30,
        FixedOn5 = 31,
        // Bits 32-63 open for future expansion
        MaxBit = 63
    }

    /// <summary>
    /// Class to keep track of the feature mask.
    /// </summary>
    /// <remarks>
    /// Product Code is a shorter version of the feature mask.  Setting Value with the Product Code
    /// is appropriate.
    /// Bits 24-31 are included in a specific, checkable pattern to allow validation of the mask.
    /// </remarks>
    public class FeatureMask
    {
        private BitArray _bits = null;

        public event Action Changed;

        public long Value
        {
            get
            {
                byte[] bytes = new byte[8];
                _bits.CopyTo(bytes, 0);
                return BitConverter.ToInt64(bytes, 0);
            }

            set
            {
                _bits = SetValueFromLong(value);
                Changed?.Invoke();
            }
        }

        // Masks off just the low 17 bits, for easier user entry
        public int ProductCode
        {
            get
            {
                return (int)(Value & 0x1ffff);
            }
        }

        public bool Activated => GetBit(FeatureMaskBits.Activated);

        public FeatureMask()
        {
            _bits = new BitArray((int)FeatureMaskBits.MaxBit + 1);
        }

        public bool GetBit(FeatureMaskBits bit) => _bits[(int)bit];

        public bool GetBit(int bitNum) => _bits[bitNum];

        public void SetBit(FeatureMaskBits bit, bool val)
        {
            _bits[(int)bit] = val;
            SetFixedBits(_bits);
            Changed?.Invoke();
        }

        public void Clear()
        {
            _bits = SetValueFromLong(0);
            Changed?.Invoke();
        }

        public static bool ValidateMask(long val)
        {
            var newBits = SetValueFromLong(val, setFixedBits: false);

            return newBits[(int)FeatureMaskBits.FixedOn1] &&
                   newBits[(int)FeatureMaskBits.FixedOn2] &&
                   newBits[(int)FeatureMaskBits.FixedOn3] &&
                   newBits[(int)FeatureMaskBits.FixedOn4] &&
                   newBits[(int)FeatureMaskBits.FixedOn5] &&
                   !newBits[(int)FeatureMaskBits.FixedOff1] &&
                   !newBits[(int)FeatureMaskBits.FixedOff2] &&
                   !newBits[(int)FeatureMaskBits.FixedOff3];
        }

        private static BitArray SetValueFromLong(long val, bool setFixedBits = true)
        {
            var bytes = BitConverter.GetBytes(val);
            var newBits = new BitArray(bytes);

            if (setFixedBits)
            {
                SetFixedBits(newBits);
            }

            return newBits;
        }

        private static void SetFixedBits(BitArray bits)
        {
            bits[(int)FeatureMaskBits.FixedOn1] = true;
            bits[(int)FeatureMaskBits.FixedOn2] = true;
            bits[(int)FeatureMaskBits.FixedOn3] = true;
            bits[(int)FeatureMaskBits.FixedOn4] = true;
            bits[(int)FeatureMaskBits.FixedOn5] = true;
            bits[(int)FeatureMaskBits.FixedOff1] = false;
            bits[(int)FeatureMaskBits.FixedOff2] = false;
            bits[(int)FeatureMaskBits.FixedOff3] = false;
        }
    }
}
