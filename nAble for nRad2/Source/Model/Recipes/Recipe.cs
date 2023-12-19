using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Text;

namespace nTact.Recipes
{
    [XmlRoot("Recipe")]
    public class Recipe
    {
        public string Name;
        public string TemplateVersion;
        public string Description;
        public DateTime LastUpdate;
        public string LastUpdatedBy;
        public bool IsSegmented = false;
        public bool IsMultiPanel = false;
        public bool VerifyFiducials = false;
        public bool ReleaseVacuumOnCompletion = false;
        public bool UnloadSubstrateOnCompletion = false;
        public int KeyenceProgramNumber = 0;
        public bool UsingKeyenceLaser = false;
        public double IRPowerLevelDuringCoating = 0.0;
        public double IRPowerLevelOnReturn = 0.0;
        public bool UseIRDuringCoating = false;
        public bool UseIROnReturn = false;
        public string ProgramName;
        public bool UsingPanel1 = false;
        public bool UsingPanel2 = false;
        public bool UsingPanel3 = false;

        public bool UsesIR => UseIRDuringCoating || UseIROnReturn;

        [XmlArray("RecipeParams")]
        [XmlArrayItem(typeof(RecipeParam))]
        public RecipeParam[] _RecipeParams;

        [XmlArray("DispenseProfileParams")]
        [XmlArrayItem(typeof(DynamicDispenseProfileParam))]
        public DynamicDispenseProfileParam[] _DispenseProfileParams;

        [XmlIgnore]
        public RecipeParamList RecipeParams { get; set; } = null;
        [XmlIgnore]
        public DynamicDispenseProfileParamList DispenseProfileParams { get; set; } = null;
        [XmlIgnore]
        public Hashtable ArrayNames = new Hashtable();

        [XmlIgnore]
        public string FileName { get; set; } = "";

        public bool HasPriming
        {
            get
            {
                RecipeParam paramPrimingRate = null;

                if (GetParam(26, ref paramPrimingRate))
                {
                    return 0 != decimal.Parse(paramPrimingRate.Value);
                }

                return false;
            }
        }

        public int CoatHeight  // param 136	mm
        {
            get
            {
                int coatHeight = -1;
                RecipeParam paramWorkHeight = null;

                if (GetParam(136, ref paramWorkHeight))
                {
                    coatHeight = int.Parse(paramWorkHeight.Value);
                }

                return coatHeight;
            }
        }

        public double CoatingVel  // param 144  mm/s
        {
            get
            {
                double coatingVel = -1;
                RecipeParam paramCoatingVel = null;

                if (GetParam(144, ref paramCoatingVel))
                {
                    coatingVel = double.Parse(paramCoatingVel.Value);
                }

                return coatingVel;
            }
        }

        public double DispenseRate  // param 32  µL/s
        {
            get
            {
                double dispenseRate = -1;
                RecipeParam paramDispenseRate = null;

                if (GetParam(32, ref paramDispenseRate))
                {
                    dispenseRate = double.Parse(paramDispenseRate.Value);
                }

                return dispenseRate;
            }
        }

        public string SelectedPump
        {
            get
            {
                string selectedPump = "NONE";
                double paramValue;
                RecipeParam paramSelectedPump = null;

                if (GetParam(110, ref paramSelectedPump))
                {
                    paramValue = double.Parse(paramSelectedPump.Value);

                    switch ((int)paramValue)
                    {
                        case 0:
                            selectedPump = "A";
                            break;
                        case 1:
                            selectedPump = "B";
                            break;
                        case 2:
                            selectedPump = "MIXING";
                            break;
                    }
                }

                return selectedPump;
            }
        }

        public Recipe()
        {
            Name = "";
            IsSegmented = false;
            Description = "";

            FileName = "";
            RecipeParams = new RecipeParamList();
            DispenseProfileParams = new DynamicDispenseProfileParamList();
        }

        public bool UpdateMixingRatios(int selectedPump, double pumpA, double pumpB)
        {
            bool updated = true;
            RecipeParam paramSelectedPump = new RecipeParam();

            updated |= GetParam(110, ref paramSelectedPump);
            paramSelectedPump.Value = selectedPump.ToString();
            updated |= GetParam(111, ref paramSelectedPump);
            paramSelectedPump.Value = pumpA.ToString();
            updated |= GetParam(112, ref paramSelectedPump);
            paramSelectedPump.Value = pumpB.ToString();

            return updated;
        }

        public int AddParam(RecipeParam recipeParam)
        {
            if (RecipeParams == null)
            {
                RecipeParams = new RecipeParamList(25);
            }

            //System.Array;
            return RecipeParams.Add(recipeParam);
        }

        public bool GetParam(int nAddress, ref RecipeParam recipeParam)
        {
            return RecipeParams?.Get(nAddress, ref recipeParam) ?? false;
        }

        public bool UpdateParam(RecipeParam recipeParam)
        {
            return RecipeParams?.Update(recipeParam) ?? false;
        }

        #region Serialization

        public void PrepareForSave()
        {
            _RecipeParams = (RecipeParam[])RecipeParams.ToArray(typeof(RecipeParam));
            _DispenseProfileParams = (DynamicDispenseProfileParam[])DispenseProfileParams.ToArray(typeof(DynamicDispenseProfileParam));
        }

        public void InitializeAfterLoad(string filename)
        {
            FileName = filename;
            RecipeParams.Clear();

            if (_RecipeParams != null)
            {
                RecipeParams.AddRange(_RecipeParams);
            }
            DispenseProfileParams.Clear();

            if (_DispenseProfileParams != null)
            {
                DispenseProfileParams.AddRange(_DispenseProfileParams);
            }

            ArrayNames.Clear();

            foreach (RecipeParam param in RecipeParams)
            {
                if (ArrayNames.ContainsKey(param.ArrayName))
                {
                    if (param.ArrayLocation > (int)ArrayNames[param.ArrayName])
                    {
                        ArrayNames[param.ArrayName] = param.ArrayLocation + 1;
                    }
                }
                else
                {
                    ArrayNames.Add(param.ArrayName, param.ArrayLocation + 1);
                }
            }
        }

        #endregion
    }
}
