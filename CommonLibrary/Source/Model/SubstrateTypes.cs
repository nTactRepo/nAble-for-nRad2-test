using CommonLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CommonLibrary.Model
{
    public class SubstrateType : IClone<SubstrateType>, IEquatable<SubstrateType>
    {
        #region Properties

        [XmlAttribute]
        public string Description { get; set; } = "";
        [XmlAttribute]
        public int GemBitPosition { get; set; } = 0;
        public float Thickness { get; set; } = 0;
        public int Speed { get; set; } = 100;

        #endregion

        #region Functions

        public SubstrateType() { }

        public SubstrateType(string description, float thickness, int speed = 100, int gemPos = 0)
        {
            Description = description;
            Thickness = thickness;
            Speed = speed;
            GemBitPosition = gemPos;
        }

        public override string ToString()
        {
            return Description;
        }

        public string FullString()
        {
            return $"{Description} [{Thickness}mm]";
        }

        public SubstrateType Clone()
        {
            return new SubstrateType()
            {
                Description = (string)Description.Clone(),
                GemBitPosition = GemBitPosition,
                Thickness = Thickness,
                Speed = Speed
            };
        }

        public bool Equals(SubstrateType other)
        {
            if (other is null)
            {
                return false;
            }

            return other.Description.Equals(Description) &&
                   other.GemBitPosition.Equals(GemBitPosition) &&
                   other.Speed.Equals(Speed) &&
                   other.Thickness.Equals(Thickness);
        }

        #endregion
    }

    public class SubstrateTypes
    {
        #region Events

        static public event Action<SubstrateTypes> Changed;

        #endregion

        #region Constants

        public static readonly string SubstrateTypesPath = Path.Combine(System.Windows.Forms.Application.StartupPath, @"Data\SubstrateTypes.xml");

        #endregion

        #region Properties

        public List<SubstrateType> Types = new List<SubstrateType>();

        #endregion

        #region Functions

        public SubstrateTypes() { }

        public void Add(SubstrateType newType)
        {
            Types.Add(newType);
        }

        public SubstrateType FindTypeFromDescription(string description)
        {
            return Types.Find(t => t.Description == description);
        }

        #region Serialization

        public static SubstrateTypes Load(string filename = null)
        {
            SubstrateTypes types;
            filename = filename ?? SubstrateTypesPath;

            try
            {
                using (var reader = new StreamReader(filename))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SubstrateTypes));
                    types = (SubstrateTypes)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine("ERROR: Could not read User File - }" + ex.ToString(), "ERROR");

                // Return default if Load fails
                types = new SubstrateTypes();
            }

            return types;
        }

        public bool Save(string filename = null)
        {
            bool succeeded = false;
            filename = filename ?? SubstrateTypesPath;

            try
            {
                using (var writer = new StreamWriter(filename))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SubstrateTypes));
                    serializer.Serialize(writer, this);
                    succeeded = true;
                    Changed?.Invoke(this);
                }
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine("ERROR: Could not save file - }" + ex.ToString(), "ERROR");
            }

            return succeeded;
        }

        #endregion

        #endregion
    }
}
