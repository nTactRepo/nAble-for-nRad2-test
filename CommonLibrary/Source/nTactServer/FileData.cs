using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace CommonLibrary.nTactServer
{
    public class FileData : IEquatable<FileData>
    {
        #region Constants

        public const char Separator = '|';

        #endregion

        #region Properties

        public string Filename { get; set; } = "";
        public string Contents { get; set; } = "";
        public bool HasData => !string.IsNullOrEmpty(Filename) && !string.IsNullOrEmpty(Contents);

        #endregion

        #region Functions

        #region Constructors

        public FileData() { }

        public FileData(string filename, string contents)
        {
            Filename = filename;
            Contents = contents;
        }

        #endregion

        #region Public Functions

        public void Clear()
        {
            Filename = "";
            Contents = "";
        }

        /// <summary>
        /// Sets data from the filename given.  Throws on error cases
        /// </summary>
        /// <param name="fullPath"></param>
        public void SetFile(string fullPath)
        {
            Filename = Path.GetFileName(fullPath);

            using (StreamReader reader = new StreamReader(fullPath))
            {
                Contents = reader.ReadToEnd();
            }
        }

        public async Task SetFileAsync(string fullPath)
        {
            Filename = Path.GetFileName(fullPath);

            using (StreamReader reader = new StreamReader(fullPath))
            {
                Contents = await reader.ReadToEndAsync();
            }
        }

        static public FileData ParseFromFileDataString(string fdStr)
        {
            var parts = fdStr.Split(new char[1] { Separator });

            if (parts.Length != 2)
            {
                return new FileData();
            }

            return new FileData(parts[0], parts[1]);
        }

        public void WriteFile(string folderName)
        {
            if (!HasData)
            {
                return;
            }

            string filename = Path.GetFileName(Filename);  // In case someone put in the full path
            string newPath = Path.Combine(folderName, filename);

            using (var fileStream = new FileStream(newPath, FileMode.Create))
            {
                using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(Contents), writable: false))
                {
                    memStream.WriteTo(fileStream);
                    fileStream.Flush();
                }
            }
        }

        public override string ToString() => $"{Filename}{Separator}{Contents}";

        public bool Equals(FileData other)
        {
            return Filename == other.Filename &&
                   Contents == other.Contents;
        }

        #endregion

        #endregion
    }
}
