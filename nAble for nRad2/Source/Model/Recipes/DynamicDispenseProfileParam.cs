using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections;
using System.Security.Cryptography;

namespace nTact.Recipes
{
    [Serializable]
    public class DynamicDispenseProfileParam
    {
        [XmlElement("X-Position")]
        public double XPos { get; set; }
        
        [XmlElement("DispenseRate")]
        public double DispenseRate { get; set; }
        
        [XmlElement("Z-Offset")]
        public double ZOffset { get; set; }
        
        [XmlElement("Location")]
        public int ArrayLocation { get; set; } = -1;
        
        public DynamicDispenseProfileParam() { }

        public DynamicDispenseProfileParam(double xPos, double dispenseRate, double zOffset,int arrayLocation)
        {
            XPos = xPos;
            DispenseRate = dispenseRate;
            ZOffset = zOffset;
            ArrayLocation = arrayLocation;
        }

        public void Update(DynamicDispenseProfileParam newParam)
        {
            XPos = newParam.XPos;
            DispenseRate = newParam.DispenseRate;
            ZOffset = newParam.ZOffset;
            ArrayLocation = newParam.ArrayLocation;
        }

        public override string ToString() => $"Dispense Profile Dataset {ArrayLocation + 1}";

        public DynamicDispenseProfileParam Clone()
        {
            return new DynamicDispenseProfileParam(XPos,DispenseRate,ZOffset,ArrayLocation);
        }

        public bool IsChanged(DynamicDispenseProfileParam param)
        {
            bool bRetVal = false;
            bRetVal = XPos != param.XPos;
            bRetVal |= DispenseRate != param.DispenseRate;
            bRetVal |= ZOffset != param.ZOffset;
            bRetVal |= ArrayLocation != param.ArrayLocation;
            return bRetVal;
        }
    }
    public class DispenseProfileEditorParams
    {
        public double Param1 { get; set; }
        public double Param2 { get; set; }
        public double Param3 { get; set; }
        public double RecipeParam1 { get; set; }
        public double RecipeParam2 { get; set; }
        public double RecipeParam3 { get; set; }
        public bool Param1IsChanged => Param1 != RecipeParam1;
        public bool Param2IsChanged => Param2 != RecipeParam2;
        public bool Param3IsChanged => Param3 != RecipeParam3;
        public bool IsChanged => Param1IsChanged || Param2IsChanged || Param3IsChanged;
    }
}
