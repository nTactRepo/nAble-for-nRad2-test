using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

namespace CommonLibrary.nTactServer
{
    [XmlRoot("Recipe")]
    public class RecipeData : IEquatable<RecipeData>
    {
        #region Constants

        public const char Separator = '|';

        #endregion

        #region Properties

        public string Filename { get; set; } = "";
        public string RecipeID { get; set; } = "";
        public string CoaterID { get; set; } = "";
        public string CarriageID { get; set; } = "";
        public string Name { get; set; } = "";

        [XmlIgnore]
        public bool HasData => !string.IsNullOrEmpty(Filename) && !string.IsNullOrEmpty(RecipeID) && !string.IsNullOrEmpty(CarriageID) & !string.IsNullOrEmpty(Name) ;

        [XmlIgnore]
        public int CarriageIDAsInt => ConvertToInt(CarriageID);
        [XmlIgnore]
        public int CoaterIDAsInt => ConvertToInt(CoaterID);
        [XmlIgnore]
        public int RecipeIDAsInt => ConvertToInt(RecipeID);

        #endregion

        #region Functions

        #region Constructors

        public RecipeData() { }

        public RecipeData(string filename,string id, string carriageID,string name)
        {
            Filename = filename;
            RecipeID = id;
            CarriageID = carriageID;
            Name = name;
        }

        #endregion Constructors

        #region Public Functions

        public void WriteFile(string folderName)
        {
            if (!HasData)
            {
                return;
            }

            SaveAs(Path.Combine(folderName, Filename));
        }

        static public RecipeData ParseFromFileDataString(string rdStr)
        {
            RecipeData rd = new RecipeData();
            var parts = rdStr.Split(new char[1] { Separator });

            if (parts.Length != 3)
            {
                return rd;
            }

            rd.RecipeID = parts[0];
            rd.CarriageID = parts[1];
            rd.Name = parts[2];
            rd.Filename = $"Recipe_{rd.RecipeID}.xml";

            return rd;
        }

        public int ConvertToInt(string str)
        {
            int val = 0;

            if (int.TryParse(str, out int i))
            {
                val = i;
            }

            return val;
        }

        public bool Save()
        {
            bool saved = false;

            try
            {
                if (!string.IsNullOrEmpty(Filename))
                {
                    // Serialize the order to a file.
                    XmlSerializer serializer = new XmlSerializer(typeof(RecipeData));

                    using (var fs = new FileStream(Filename, FileMode.Create))
                    {
                        serializer.Serialize(fs, this);
                    }

                    saved = true;
                }
            }
            catch (Exception)
            {
            }

            return saved;
        }

        public bool SaveAs(string fileName)
        {
            bool saved = false;

            try
            {
                if (fileName != "")
                {
                    Filename = fileName;
                }

                // Serialize the order to a file.
                XmlSerializer serializer = new XmlSerializer(typeof(RecipeData));

                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    serializer.Serialize(fs, this);
                }

                saved = true;
            }
            catch (Exception)
            {
            }

            return saved;
        }

        public static RecipeData Load(string filename)
        {
            RecipeData rd = null;

            // Serialize the order to a file.
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(RecipeData));

                using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    rd = (RecipeData)serializer.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine($"Could not read the recipe data file:  {ex}", "ERROR");
            }

            return rd;
        }

        public override string ToString() => $"{RecipeID}{Separator}{CarriageID}{Separator}{Name}";

        public bool Equals(RecipeData other)
        {
            return RecipeID == other.RecipeID &&
                   CoaterID == other.CoaterID &&
                   CarriageID == other.CarriageID &&
                   Name == other.Name;
    }

        #endregion

        #endregion
    }
}
