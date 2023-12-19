using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Collections
{
    public class EnumElement<T>
    {
        public T Element { get; set; }

        public EnumElement(T element)
        {
            Element = element;
        }

        public override string ToString() => Element.ToString();
    }

    /// <summary>
    /// Class to create a list of EnumElements from an enum
    /// </summary>
    /// <remarks>Adapted from https://stackoverflow.com/questions/5638639/how-to-bind-an-enumeration-to-combobox </remarks>
    public class EnumList
    {
        public static IEnumerable<EnumElement<T>> Of<T>()
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(p => new EnumElement<T>(p));
        }
    }
}
