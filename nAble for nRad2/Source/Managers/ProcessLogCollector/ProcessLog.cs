using CommonLibrary.Source.Utils.Interfaces;
using nTact.Recipes;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace nAble.Managers.ProcessLogCollectors
{
    public class ProcessLog : ILoadSave
    {
        #region Properties

        public string RecipeName { get; set; }

        [XmlIgnore]
        public DateTime RecipeStart { get; set; }
        
        [XmlElement("RecipeStart")]
        public string RecipeStartString
        {
            get => RecipeStart.ToShortDateString();
            set => RecipeStart = DateTime.Parse(value);
        }

        [XmlIgnore]
        public DateTime RecipeEnd { get; set; }

        [XmlElement("RecipeEnd")]
        public string RecipeEndString
        {
            get => RecipeEnd.ToShortDateString();
            set => RecipeEnd = DateTime.Parse(value);
        }

        public bool RecipeSucceeded { get; set; }
        public bool RecipeAborted { get; set; }
        public int ErrorCode { get; set; }

        public List<RecipeParam> RecipeParams { get; set; } = new List<RecipeParam>();

        public string Filename { get; set; }

        #endregion
    }
}
