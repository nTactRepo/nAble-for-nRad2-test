using nTact.Recipes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace nAble.Model.Recipes
{
    [Serializable]
    public class ProcessRecipe
    {
        public string Name;
        public int ID = -1;
        public string Description;
        public int Revision;
        public DateTime LastUpdate;
        public string LastUpdatedBy;
        public int SelectedCarriage;
        public List<RecipeParam> RecipeParams;

        public string Filename { get; set; } = "";

        public ProcessRecipe()
        {
            Name = "";
            Description = "";
            SelectedCarriage = 1;
            Filename = "";
        }

        public int AddParam(RecipeParam recipeParam)
        {
            int nRetVal = 0;

            RecipeParams.Add(recipeParam);
            return nRetVal;
        }

        public bool GetParam(int paramID, ref RecipeParam recipeParam)
        {
            bool bRetVal = false;

            if (RecipeParams != null)
            {
                foreach (RecipeParam thisParam in RecipeParams)
                {
                    if (paramID == thisParam.ID)
                    {
                        recipeParam = thisParam;
                        bRetVal = true;
                        break;
                    }
                }
            }

            return bRetVal;
        }

        public bool GetParam(string paramName, ref RecipeParam recipeParam)
        {
            bool bRetVal = false;

            if (RecipeParams != null)
            {
                foreach (RecipeParam thisParam in RecipeParams)
                {
                    if (paramName == thisParam.Name)
                    {
                        recipeParam = thisParam;
                        bRetVal = true;
                        break;
                    }
                }
            }

            return bRetVal;
        }
        public bool UpdateParam(RecipeParam recipeParam)
        {
            bool bRetVal = false;

            if (RecipeParams != null)
            {

            }

            return bRetVal;
        }

        public bool GetParamValue(string paramName, out double paramValue)
        {
            bool bRetVal = false;
            paramValue = 0;
            foreach (RecipeParam recipeParam in RecipeParams)
            {
                if (paramName == recipeParam.Name && double.TryParse(recipeParam.Value, out var num))
                {
                    paramValue = num;
                    bRetVal = true;
                    break;
                }
            }
            return bRetVal;
        }

        public static ProcessRecipe Load(string sFileName)
        {
            ProcessRecipe oRetVal = null;

            // Serialize the order to a file.
            FileStream fs = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ProcessRecipe));
                fs = new FileStream(sFileName, FileMode.Open);
                oRetVal = (ProcessRecipe)serializer.Deserialize(fs);
                fs.Close();
            }
            catch (Exception ex)
            {
                Trace.Listeners[1].WriteLine("Could not read Receipe File : " + ex.Message, "ERROR");
                oRetVal = null;
                if (fs != null)
                    fs.Close();
            }

            return oRetVal;
        }

        public bool Save()
        {
            bool bRetVal = false;
            try
            {
                if (Filename != "")
                {
                    // Serialize the order to a file.
                    XmlSerializer serializer = new XmlSerializer(typeof(ProcessRecipe));
                    FileStream fs = new FileStream(Filename, FileMode.Create);
                    serializer.Serialize(fs, this);
                    fs.Close();
                    bRetVal = true;
                }
            }
            catch (Exception)
            {
            }

            return bRetVal;
        }
        public bool SaveAs(string sFileName, bool bOverWrite)
        {
            bool bRetVal = false;
            try
            {
                if (sFileName != "")
                    Filename = sFileName;
                // Serialize the order to a file.
                XmlSerializer serializer = new XmlSerializer(typeof(ProcessRecipe));
                FileStream fs = new FileStream(sFileName, FileMode.Create);
                serializer.Serialize(fs, this);
                fs.Close();
                bRetVal = true;
            }
            catch (Exception)
            {
            }

            return bRetVal;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
