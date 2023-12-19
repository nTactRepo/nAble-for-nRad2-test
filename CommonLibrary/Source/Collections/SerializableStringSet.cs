using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace CommonLibrary.Collections
{
    public class SerializableStringSet
    {
        #region Properties

        public HashSet<string> StringSet { get; set; } = new HashSet<string>();

        [XmlIgnore]
        public string Filepath { get; set; } = "";

        [XmlIgnore]
        public string Filename { get; set; } = "";

        #endregion

        #region Data Members

        #endregion

        #region Functions

        #region Constructors

        public SerializableStringSet() { }

        public SerializableStringSet(string path, string filename)
        {
            Filepath = path;
            Filename = filename;
        }

        #endregion

        #region Public Functions

        public void AddString(string str)
        {
            StringSet.Add(str);
        }

        public void RemoveString(string str)
        {
            StringSet.Remove(str);
        }

        public void Clear()
        {
            StringSet.Clear();
        }

        #endregion

        #region Private Functions

        #endregion

        #region Serialization

        public void Load()
        {
            SerializableStringSet set = null;
            var fullPath = Path.Combine(Filepath, Filename);

            // Serialize the order to a file.
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SerializableStringSet));

                using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    set = (SerializableStringSet)serializer.Deserialize(fs);
                }

                foreach (string str in set.StringSet)
                {
                    StringSet.Add(str);
                }
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine($"ERROR: Could not read string list {fullPath}: {ex.Message}", "ERROR");
                throw ex;
            }
        }

        public virtual void Save()
        {
            string fullPath = Path.Combine(Filepath, Filename);

            try
            {
                // Serialize the order to a file.
                XmlSerializer serializer = new XmlSerializer(typeof(SerializableStringSet));

                using (var fs = new FileStream(fullPath, FileMode.Create))
                {
                    serializer.Serialize(fs, this);
                }

                Trace.Listeners["nTact"].WriteLine(fullPath + " Saved. ");
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine($"Could NOT save {fullPath}: {ex.Message}", "ERROR");
            }
        }

        #endregion

        #endregion
    }
}
