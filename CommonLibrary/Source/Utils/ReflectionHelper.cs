using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CommonLibrary.Utils
{
    public static class ReflectionHelper
    {

        public static string GetDescription<T>(this T enumerationValue)
            where T : struct
        {
            Type type = enumerationValue.GetType();

            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());

            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        public static IEnumerable<Type> GetAllDerivedTypes(this Type baseType, bool includeAbstract)
        {
            return Assembly.GetAssembly(baseType).GetAllDerivedTypes(baseType, includeAbstract);
        }

        public static IEnumerable<Type> GetAllDerivedTypes(this Assembly assembly, Type baseType, bool includeAbstract)
        {
            TypeInfo baseTypeInfo = baseType.GetTypeInfo();
            bool isClass = baseTypeInfo.IsClass;
            bool isInterface = baseTypeInfo.IsInterface;

            return assembly
                .GetTypes()
                .Where(t => t != baseType && baseType.IsAssignableFrom(t) && !(t.IsAbstract && !includeAbstract));
        }

    }
}
