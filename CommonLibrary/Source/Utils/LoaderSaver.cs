using CommonLibrary.Source.Utils.Interfaces;
using System;
using System.IO;
using System.Xml.Serialization;

namespace CommonLibrary.Source.Utils
{
    public static class LoaderSaver<T> where T : ILoadSave
    {
        #region Functions

        public static T Load(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            T loadObject;

            // Serialize the object to a file.
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                loadObject = (T)serializer.Deserialize(fs);
            }

            loadObject.Filename = filename;

            return loadObject;
        }

        public static bool Save(T saveObject, string filename = "")
        {
            string savedName = saveObject.Filename;
            string name = !string.IsNullOrEmpty(filename) ? filename :
                !string.IsNullOrEmpty(savedName) ? savedName : throw new ArgumentException("No valid name to save with!", nameof(filename));

            saveObject.Filename = name;
            string dir = Path.GetDirectoryName(name);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // Serialize the order to a file.
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (FileStream fs = new FileStream(name, FileMode.Create))
            {
                serializer.Serialize(fs, saveObject);
            }

            return true;
        }

        #endregion
    }
}
