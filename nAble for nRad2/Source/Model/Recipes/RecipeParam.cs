using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections;

namespace nTact.Recipes
{
    [Serializable]
    public class RecipeParam
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Units { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
        public int Order { get; set; } = -1;

        [XmlElement("Min")]
        public string MinValue { get; set; }
        
        [XmlElement("Max")]
        public string MaxValue { get; set; }
        
        [XmlElement("Array")]
        public string ArrayName { get; set; }
        
        [XmlElement("Location")]
        public int ArrayLocation { get; set; } = -1;
        
        public string Formatting { get; set; }
        public bool IsEditable { get; set; } = false;

        public RecipeParam() { }

        public RecipeParam(string name, string desc, string units, string dataType, string val, int nOrder, 
                           string minValue, string maxValue, string arrayName, int arrayLocation, string formatting, bool isEditable)
        {
            Name = name;
            Description = desc;
            Units = units;
            DataType = dataType;
            Value = val;
            Order = nOrder;
            MinValue = minValue;
            MaxValue = maxValue;
            ArrayName = arrayName;
            ArrayLocation = arrayLocation;
            Formatting = formatting;
            IsEditable = isEditable;
        }

        public void Update(RecipeParam newParam)
        {
            Name = newParam.Name;
            Description = newParam.Description;
            Units = newParam.Units;
            DataType = newParam.DataType;
            Value = newParam.Value;
            Order = newParam.Order;
            MinValue = newParam.MinValue;
            MaxValue = newParam.MaxValue;
            ArrayName = newParam.ArrayName;
            ArrayLocation = newParam.ArrayLocation;
            Formatting= newParam.Formatting;
            IsEditable = newParam.IsEditable;
        }

        public override string ToString() => Name;
    }
}
