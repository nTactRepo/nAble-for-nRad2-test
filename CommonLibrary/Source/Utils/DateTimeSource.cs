using System;

namespace CommonLibrary.Utils
{
    public class DateTimeSource : IDateTimeSource
    {
        public DateTime MaxValue => DateTime.MaxValue;
        public DateTime MinValue => DateTime.MinValue;
        public DateTime Now => DateTime.Now;
    }
}
