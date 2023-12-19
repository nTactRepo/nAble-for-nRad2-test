using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CommonLibrary.Utils
{
    static public class UIUtils
    {
        #region Constants

        public const string NoTimeInfoString = "--:--:--";

        #endregion

        #region Functions

        #region Public Functions

        static public List<T> FindAllChildControls<T>(this Control control) where T : Control
        {
            var controls = control.Controls.Cast<Control>();
            return controls.OfType<T>().Concat(controls.SelectMany(ctrl => FindAllChildControls<T>(ctrl))).ToList();
        }

        static public int Distance(Point p1, Point p2)
        {
            int xDiff = p1.X - p2.X;
            int yDiff = p1.Y - p2.Y;
            return (int)Math.Round(Math.Sqrt(xDiff * xDiff + yDiff * yDiff), 0);
        }

        static public string GetTimeAsString(TimeSpan span)
        {
            string time = NoTimeInfoString;

            if (span.Hours + span.Minutes + span.Seconds > 0)
            {
                time = $"{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}";
            }

            return time;
        }

        static public bool IsApproximatelyEqualTo(this double initialValue, double value)
        {
            return IsApproximatelyEqualTo(initialValue, value, 0.00001);
        }

        static public bool IsApproximatelyEqualTo(this double initialValue, double value, double maximumDifferenceAllowed)
        {
            // Handle comparisons of floating point values that may not be exactly the same
            return (Math.Abs(initialValue - value) < maximumDifferenceAllowed);
        }

        static public bool IsApproximatelyEqualTo(this float initialValue, float value)
        {
            return IsApproximatelyEqualTo(initialValue, value, 0.00001f);
        }

        static public bool IsApproximatelyEqualTo(this float initialValue, float value, float maximumDifferenceAllowed)
        {
            // Handle comparisons of floating point values that may not be exactly the same
            return (Math.Abs(initialValue - value) < maximumDifferenceAllowed);
        }

        #endregion

        #endregion
    }
}
