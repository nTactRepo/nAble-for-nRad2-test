using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTact.Recipes
{
    public class RecipeParamList : ArrayList
    {
        public RecipeParamList() : base() { }
        public RecipeParamList(int capacity) : base(capacity) { }
        public virtual int Add(RecipeParam value)
        {
            return base.Add(value);
        }
        public virtual RecipeParam Item(int Index)
        {
            return (RecipeParam)base[Index];
        }

        public bool Get(int nAddress, ref RecipeParam recipeParam)
        {
            bool bRetVal = false;

            if (Count > 0)
            {
                foreach (RecipeParam curParam in this)
                {
                    if (curParam.ArrayLocation == nAddress)
                    {
                        bRetVal = true;
                        recipeParam = curParam;
                        break;
                    }
                }
            }
            return bRetVal;
        }
        public bool Update(RecipeParam recipeParam)
        {
            bool bRetVal = false;

            if (Count > 0)
            {
                foreach (RecipeParam curParam in this)
                {
                    if (curParam.ArrayLocation == recipeParam.ArrayLocation)
                    {
                        bRetVal = true;
                        curParam.Update(recipeParam);
                        break;
                    }
                }
                if (!bRetVal)
                {
                    Add(recipeParam);
                    bRetVal = true;
                }
            }
            else
            {
                Add(recipeParam);
                bRetVal = true;
            }
            return bRetVal;
        }
        public string GetParamProgramListing()
        {
            string sRetVal = "";
            // find the array names
            Hashtable htArrays = new Hashtable(60);
            StringBuilder sbTemp = new StringBuilder();
            foreach (RecipeParam recipeParam in this)
            {
                sbTemp.AppendLine(String.Format("{0}[{1}]={2}", recipeParam.ArrayName, recipeParam.ArrayLocation, recipeParam.Value.ToString()));
                if (htArrays.ContainsKey(recipeParam.ArrayName))
                {
                    if (recipeParam.ArrayLocation > (int)htArrays[recipeParam.ArrayName])
                    {
                        htArrays[recipeParam.ArrayName] = recipeParam.ArrayLocation;
                    }
                }
                else
                {
                    htArrays.Add(recipeParam.ArrayName, recipeParam.ArrayLocation);
                }
            }
            StringBuilder sbHeader = new StringBuilder();
            foreach (String sKey in htArrays.Keys)
            {
                sbHeader.AppendLine(String.Format("DA {0}[0]", sKey));
                sbHeader.AppendLine(String.Format("DM {0}[{1}]", sKey, htArrays[sKey]));
            }
            sRetVal = sbHeader.ToString() + sbTemp.ToString();
            return sRetVal;
        }
    }
}
