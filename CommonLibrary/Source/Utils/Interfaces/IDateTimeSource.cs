using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Utils
{
    public interface IDateTimeSource
    {
        DateTime MaxValue { get; }
        DateTime MinValue { get; }
        DateTime Now { get; }
    }
}
