using System;

namespace nAble.DataComm.AdvantechSerialServer
{
    public class AnalogInput
    {
        #region Events

        public event Action<double> NewValue;

        #endregion

        #region Properties

        // Data set on construction

        public int AIONumber { get; set; }

        public string Name { get; set; }

        public string Unit { get; set; }

        public ushort MinRaw { get; set; } = 0;

        public ushort MaxRaw { get; set; } = 65535;

        public double MinConvertedInput { get; set; }

        public double MaxConvertedInput { get; set; }

        // Updateable values

        public bool Enabled { get; set; }

        public double Value { get; private set; } = 0;

        public ushort RawInput { get; private set; } = 0;

        public ErrorState ErrorState { get; private set; } = new ErrorState();

        #endregion

        #region Member Data

        private object _lock = new object();

        private double _A = 0;
        private double _B = 0;

        #endregion

        #region Functions

        #region Constructors

        public AnalogInput(int aIONumber, string name, string unit, double minInput, double maxInput, ushort minRaw = 0, ushort maxRaw = 65535) 
        { 
            if (minInput >= maxInput)
            {
                throw new ArgumentOutOfRangeException(nameof(minInput), 
                    $"For input range, min input ({minInput}) needs to be less than max input ({maxInput})");
            }

            if (minRaw >= maxRaw)
            {
                throw new Exception($"minRaw is bigger than or equal to maxRaw.  This is not allowed.");
            }

            AIONumber = aIONumber;
            
            MinRaw = minRaw; 
            MaxRaw = maxRaw;
            
            MinConvertedInput = minInput;
            MaxConvertedInput = maxInput;

            Unit = unit ?? throw new ArgumentNullException(nameof(unit));
            Name = name ?? throw new ArgumentNullException(nameof(name));

            CalculateConversionConstants();

            Enabled = false;
            RawInput = MinRaw;
            Value = ConvertInput(MinRaw);
        }

        #endregion

        #region Public Functions

        public void SetRawInput(ushort rawInput, int rawErrorState, bool enabled)
        {
            RawInput = rawInput;
            Value = ConvertInput(RawInput);
            ErrorState.RawState = rawErrorState;
            Enabled = enabled;
        }

        #endregion

        #region Private Functions

        private double ConvertInput(ushort raw)
        {
            // For details, see CalculateConversionConstants()

            return _A * (raw - MinRaw) + _B;
        }

        private void CalculateConversionConstants()
        {
            // With a generic equation _A * (raw - RawMin) + _B = Output value,
            // you can create the following two equations
            //
            // _A * (RawMin - RawMin) + _B = Vmin
            // _A * (RawMax - RawMin) + _B = Vmax
            //
            // where Vmin is min output value, and Vmax is max output value.
            // Do some math, and we get:
            
            _A = (MaxConvertedInput - MinConvertedInput) / (MaxRaw - MinRaw);

            _B = MinConvertedInput;
        }

        #endregion

        #endregion    
    }
}
