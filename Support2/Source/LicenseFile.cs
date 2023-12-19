using CommonLibrary.Source.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using CommonLibrary.Source.Enums;
using System.Text;

namespace Support2
{
    public class LicenseFile : ILoadSave
    {
        #region Properties

        public string ProjectNumber { get; set; } = "";

        public CoaterModels CoaterModel { get; set; } = CoaterModels.None;

        public string SerialNumber { get; set; } = "";

        [XmlIgnore]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [XmlElement("CreationDate")]
        public string CreationDateString
        {
            get => CreationDate.ToShortDateString();
            set => CreationDate = DateTime.Parse(value);
        }

        [XmlIgnore]
        public DateTime ExpirationDate { get; set; } = DateTime.Now;

        [XmlElement("ExpirationDate")]
        public string ExpirationDateString
        {
            get => ExpirationDate.ToShortDateString();
            set => ExpirationDate = DateTime.Parse(value);
        }

        [XmlElement("ProductCode")]
        public int Features { get; set; } = 0;

        public ProductKey ProductKey { get; set; } = new ProductKey("");

        public string Filename { get; set; }

        public string OutputFilename => GetOutputFilename();

        public bool Activated => Features > 65535;

        #endregion

        #region Data Members

        private string _filePath = "";

        #endregion

        #region Functions

        public LicenseFile() { }

        public void SaveAsText(string newPath = "")
        {
            string path = string.IsNullOrEmpty(newPath) ? _filePath : newPath;
            string dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var strings = new List<string>()
            {
                $"License Information for {CoaterModel}, Project:  {ProjectNumber}, SN# {SerialNumber}",
                $"--------------------------------------------------------",
                $"",
                $"Product Code:     {Features}",
                $"Product Key:      {ProductKey}",
                $"Creation Date:    {CreationDateString}",
                $"Expiration Date:  {ExpirationDateString}",
            };

            File.WriteAllLines(path, strings);
        }

        private string GetOutputFilename()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.IsNullOrEmpty(ProjectNumber) ? "XXXXX" : ProjectNumber);
            sb.Append('-');
            sb.Append(CoaterModel);
            sb.Append('-');
            sb.Append(Activated ? "Permanent" : "Trial");
            sb.Append(" License");

            if (!Activated)
            {
                sb.Append($"-[{ExpirationDate.Month:00}-{ExpirationDate.Day:00}-{ExpirationDate.Year:0000}]");
            }

            sb.Append(".txt");
            
            return Path.Combine(Path.GetDirectoryName(Filename) ?? "", sb.ToString());
        }

        #endregion
    }
}
