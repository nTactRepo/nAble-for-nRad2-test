using nTact.Recipes;
using Support2;
using Support2.RegistryClasses;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormRecipeAdd : Form, IUpdateableForm
    {
        private readonly FormMain _frmMain = null;
        private readonly IRecipeManager _recipeMgr = null;
        private readonly NRadLicensing2 _licMgr = null;

        private Color _colorWindow;

        public FormRecipeAdd(FormMain frmMain, NRadLicensing2 licMgr, IRecipeManager recipeManager)
        {
            InitializeComponent();

            _colorWindow = Color.FromName("Window");
            _frmMain = frmMain;
            _recipeMgr = recipeManager;
            _licMgr = licMgr;
        }

        private void FormRecipeAdd_Load(object sender, EventArgs e)
        {
            textBoxNewRecipeName.Text = "";
            LoadRecipes();
            radioButtonBlank.Checked = true;
            checkBoxSegmented.Visible = _licMgr.IsFeatureActive(LicensedFeautres.Feature1) || _frmMain.IsDemoMode || _frmMain.DebugMode;
        }

        private void FormRecipeAdd_Enter(object sender, EventArgs e)
        {
            LoadRecipes();
            radioButtonBlank.Checked = true;
            checkBoxSegmented.Visible = _licMgr.IsFeatureActive(LicensedFeautres.Feature1) || _frmMain.IsDemoMode || _frmMain.DebugMode;
        }

        public void UpdateStatus()
        {
            bool hasRecipe = _recipeMgr.HasRecipe(textBoxNewRecipeName.Text);
            bool hasNewName = textBoxNewRecipeName.Text.Length >= 1 && !hasRecipe;
            bool isValid = radioButtonBlank.Checked || (radioButtonStartFrom.Checked && comboBoxCopyFrom.Text != "");

            buttonOK.Enabled = isValid && hasNewName;
            Color colorNew = hasRecipe ? Color.LightSalmon : _colorWindow;

            textBoxNewRecipeName.BackColor = colorNew;
            comboBoxCopyFrom.Enabled = radioButtonStartFrom.Checked;
            checkBoxSegmented.Enabled = radioButtonBlank.Checked;
        }

        private bool _bUpdatingRecipes = false;
        private int _nUpdateRequests = 0;
        private void LoadRecipes()
        {
            bool bContinue = true;

            if (!_bUpdatingRecipes)
            {
                _bUpdatingRecipes = true;

                while (bContinue)
                {
                    comboBoxCopyFrom.Items.Clear();

                    foreach (string recipeName in _recipeMgr.RecipeList)
                    {
                        comboBoxCopyFrom.Items.Add(recipeName);
                    }

                    if (_nUpdateRequests > 0)
                    {
                        _nUpdateRequests = 0;
                    }
                    else
                    {
                        bContinue = false;
                    }
                }

                _bUpdatingRecipes = false;
            }
            else
            {
                _nUpdateRequests++;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string newFileName = GetNewRecipeFileName();

            if (radioButtonBlank.Checked)
            {
                CreateNewFromDefaults(textBoxNewRecipeName.Text.Trim(), newFileName);
            }
            else
            {
                Recipe copyFromRecipe = _recipeMgr.FetchRecipe(comboBoxCopyFrom.Text);
                Recipe newRecipe = _recipeMgr.LoadRecipe(copyFromRecipe.FileName);

                if (newRecipe != null)
                {
                    newRecipe.Name = textBoxNewRecipeName.Text.Trim();

                    if (_recipeMgr.SaveRecipeAs(newRecipe, newFileName, true))
                    {
                        nRadMessageBox.Show(this, "Recipe \r\n'" + textBoxNewRecipeName.Text.Trim() + "'\r\n Created Successfully.", "New Recipe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        nRadMessageBox.Show(this, "Error while saving new recipe.  Please try again.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    nRadMessageBox.Show(this, "Error while loading base recipe.  Please try again.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            _frmMain.LoadSubForm(_frmMain.frmRecipe);
            textBoxNewRecipeName.Text = "";
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmRecipe);
            textBoxNewRecipeName.Text = "";
            LoadRecipes();
            radioButtonBlank.Checked = true;
        }

        public string GetNewRecipeFileName()
        {
            string sRetVal = "";
            string sNewFileName;

            //_sDataPath
            do
            {
                DirectoryInfo di = new DirectoryInfo(@"Data\Recipes");
                sNewFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                sRetVal = Path.Combine(di.FullName, "Recipe" + sNewFileName + ".xml");
            } while (File.Exists(sRetVal));

            return sRetVal;
        }
        private void CreateNewFromDefaults(string sRecipeName, string sNewFileName)
        {
            Recipe newRecipe = _recipeMgr.DefaultRecipe;

            if (newRecipe != null)
            {
                newRecipe.Name = sRecipeName;
                newRecipe.IsSegmented = checkBoxSegmented.Checked;

                if (_recipeMgr.SaveRecipeAs(newRecipe, sNewFileName, true))
                {
                    nRadMessageBox.Show(this, $"Recipe \r\n'{textBoxNewRecipeName.Text.Trim()}'\r\n Created Successfully.", "New Recipe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    nRadMessageBox.Show(this, "Error while saving new recipe.  Please try again.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                nRadMessageBox.Show(this, "Error while loading base recipe.  Please try again.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonKeyboard4Name_Click(object sender, EventArgs e)
        {
            _frmMain.GotoTextScreen("New Recipe Name", this, textBoxNewRecipeName, 128);
        }
    }
}
