using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CommonLibrary.Source.Utils
{
    public static class CompareUtils
    {
        private static readonly Color ControlColor = Color.FromName("Control");

        public static bool CompareArrays<T>(T[] array1, T[] array2) where T : IEquatable<T>
        {
            if (array1 == array2) // If there are the exact same list (including both null), then we are good
            {
                return true;
            }

            if (array1 is null || array2 is null)
            {
                return false;
            }

            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                bool areEqual = array1[i]?.Equals(array2[i]) ?? false;
                bool bothNull = array1[i] == null && array2[i] == null;

                if (!areEqual && !bothNull)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CompareLists<T>(List<T> list1, List<T> list2) where T : IEquatable<T>
        {
            if (list1 == list2) // If there are the exact same list (including both null), then we are good
            {
                return true;
            }

            if (list1 is null || list2 is null)
            {
                return false;
            }

            if (list1.Count != list2.Count)
            {
                return false;
            }

            for (int i = 0; i < list1.Count; i++)
            {
                bool areEqual = list1[i]?.Equals(list2[i]) ?? false;
                bool bothNull = list1[i] == null && list2[i] == null;

                if (!areEqual && !bothNull)
                {
                    return false;
                }
            }

            return true;
        }

        public static void CheckIfChanged<T>(this Control control, T setting) where T : IConvertible
        {
            control.BackColor = ConvertValue<T>(control).Equals(setting) ? ControlColor : Color.Yellow;
        }

        public static T ConvertValue<T>(Control control) where T : IConvertible
        {
            return (T)Convert.ChangeType(control.Text, typeof(T));
        }
    }
}
