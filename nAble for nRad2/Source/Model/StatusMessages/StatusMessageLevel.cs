using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nAble.Model.StatusMessages
{
    public enum StatusMessageLevel
    {
        Error,
        Warning,
        Status,
        Info
    }

    public static class StatusMessageColors
    {
        public static Color ForeColor(StatusMessageLevel level)
        {
            switch (level)
            {
                case StatusMessageLevel.Error: return Color.Red;
                case StatusMessageLevel.Warning: return Color.Yellow;
                case StatusMessageLevel.Status: return Color.Fuchsia;
                case StatusMessageLevel.Info: return Color.Blue;

                default: throw new NotImplementedException($"Unknown Status Message Level: {level}");
            }
        }
        public static Color BackColor(StatusMessageLevel level)
        {
            switch (level)
            {
                case StatusMessageLevel.Error: return Color.Black;
                case StatusMessageLevel.Warning: return Color.Black;
                case StatusMessageLevel.Status: return Color.Black;
                case StatusMessageLevel.Info: return Color.Yellow;

                default: throw new NotImplementedException($"Unknown Status Message Level: {level}");
            }
        }
    }
}
