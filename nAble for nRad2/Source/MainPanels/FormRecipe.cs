using nTact.Recipes;
using Support2;
using Support2.RegistryClasses;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormRecipe : Form, IUpdateableForm, ITracksRecipeList
    {
        #region Data Members

        private readonly FormMain _frmMain = null;
        private readonly IRecipeManager _recipeMgr = null;
        private readonly NRadLicensing2 _licMgr = null;

        private DataTable _recipeTable;
        private int _scrollLocation;
        private int _viewerRows = 10;
        private bool _requestRefresh = false;
        private bool _updatingRecipes = false;
        private int _updateRequests = 0;

        #endregion

        #region Functions

        #region Constructors

        public FormRecipe(FormMain formMain, NRadLicensing2 licMgr, IRecipeManager recipeManager)
        {
            InitializeComponent();

            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _recipeMgr = recipeManager ?? throw new ArgumentNullException(nameof(recipeManager));
            _licMgr = licMgr ?? throw new ArgumentNullException(nameof(licMgr));
        }

        #endregion

        #region Public Functions

        public void UpdateStatus()
        {
            if (_frmMain == null || !Visible)
            {
                return;
            }

            if (_requestRefresh)
            {
                _requestRefresh = false;
                LoadRecipes();
            }

            buttonRecipeExport.Enabled = dataGridViewRecipes.SelectedRows.Count != 0;
            buttonRecipeDelete.Enabled = dataGridViewRecipes.SelectedRows.Count != 0;
            buttonRecipeEdit.Enabled = dataGridViewRecipes.SelectedRows.Count != 0;

            buttonSegmentedDefaults.Visible = _licMgr.IsFeatureActive(LicensedFeautres.Feature1) || _frmMain.IsDemoMode || _frmMain.DebugMode;
        }

        #endregion

        #region Private Functions

        private void CreateDataTables()
        {
            //Create Alarm Log Datatable
            _recipeTable = new DataTable();
            // Define columns
            DataColumn dc2;
            dc2 = new DataColumn("No");
            dc2.DataType = typeof(int);
            _recipeTable.Columns.Add(dc2);
            dc2 = new DataColumn("Recipe_Name");
            _recipeTable.Columns.Add(dc2);
            _recipeTable.PrimaryKey = new DataColumn[] { _recipeTable.Columns["No"] };
        }

        private bool UpdateRecipeView()
        {
            string strSelect = $"No >= {_scrollLocation} AND No <= {_scrollLocation + _viewerRows}";

            try
            {
                if (_recipeTable.Rows.Count > 0)
                {
                    DataRow[] result = _recipeTable.Select(strSelect);
                    dataGridViewRecipes.DataSource = result.CopyToDataTable();
                    dataGridViewRecipes.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dataGridViewRecipes.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridViewRecipes.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridViewRecipes.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    dataGridViewRecipes.AllowUserToResizeColumns = true;
                }
                else
                {
                    dataGridViewRecipes.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return true;
        }

        private void LoadRecipes()
        {
            int nRow = 0;
            bool bContinue = true;
            if (!_updatingRecipes)
            {
                _updatingRecipes = true;

                while (bContinue)
                {
                    _recipeTable.Clear();
                    _recipeTable.BeginLoadData();
                    Recipe curRecipe = null;
                    DirectoryInfo di = new DirectoryInfo(@"Data\Recipes");

                    foreach (string recipeName in _recipeMgr.RecipeList)
                    {
                        if (recipeName != "defaults")
                        {
                            curRecipe = _recipeMgr.FetchRecipe(recipeName);

                            if (curRecipe != null)
                            {
                                nRow++;
                                _recipeTable.Rows.Add(nRow, curRecipe.Name);
                            }
                        }
                    }

                    if (_updateRequests > 0)
                    {
                        _updateRequests = 0;
                    }
                    else
                    {
                        bContinue = false;
                    }

                    _recipeTable.EndLoadData();
                }
                if (_recipeTable.Rows.Count > _viewerRows)
                {
                    splitContainerRecipeView.Panel2Collapsed = false;
                    vScrollBarRecipes.Maximum = _recipeTable.Rows.Count;
                    vScrollBarRecipes.LargeChange = _viewerRows;
                    vScrollBarRecipes.Value = vScrollBarRecipes.Maximum - _viewerRows;
                }
                else
                {
                    splitContainerRecipeView.Panel2Collapsed = true;
                }

                _recipeTable.DefaultView.Sort = "Recipe_Name asc";
                _recipeTable = _recipeTable.DefaultView.ToTable();
                int idx = 1;

                foreach (DataRow dr in _recipeTable.Rows)
                {
                    dr["No"] = idx;
                    idx++;
                }

                _scrollLocation = 1;
                UpdateRecipeView();
                _updatingRecipes = false;
                _requestRefresh = false;
            }
            else
            {
                _updateRequests++;
            }
        }

        #endregion

        #region Event Handlers

        private void FormRecipe_Load(object sender, EventArgs e)
        {
            splitContainerRecipeView.Panel2Collapsed = true;
            _scrollLocation = 1;
            CreateDataTables();
            _requestRefresh = true;

            if (_recipeTable.Rows.Count > _viewerRows)
            {
                splitContainerRecipeView.Panel2Collapsed = false;
                vScrollBarRecipes.Maximum = _recipeTable.Rows.Count;
                vScrollBarRecipes.LargeChange = _viewerRows;
                vScrollBarRecipes.Value = vScrollBarRecipes.Maximum - _viewerRows;
            }
            else
            {
                splitContainerRecipeView.Panel2Collapsed = true;
            }

            UpdateRecipeView();
        }

        public void HandleRecipeListChanged()
        {
            _requestRefresh = true;
        }

        private void buttonRecipeEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewRecipes.SelectedRows.Count != 1)
            {
                return;
            }

            string recipeName = dataGridViewRecipes.SelectedCells[1].Value.ToString();

            if (!_recipeMgr.HasRecipe(recipeName))
            {
                nRadMessageBox.Show(this, "ERROR:  Recipe no longer exists...  Please select a new recipe", "Recipe Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _requestRefresh = true;
                return;
            }

            Recipe curRecipe = _recipeMgr.FetchRecipe(recipeName);

            if (curRecipe.IsSegmented)
            {
                if (_frmMain.frmSegmentedRecipeEditor.LoadRecipe(recipeName))
                {
                    _frmMain.LoadSubForm(_frmMain.frmSegmentedRecipeEditor);
                }
            }
            //else if (curRecipe.IsMultiPanel)
            //{
            //    if (_frmMain.frmMultiPanelRecipeEditor.LoadRecipe(recipeName))
            //    {
            //        _frmMain.LoadSubForm(_frmMain.frmMultiPanelRecipeEditor);
            //    }
            //}
            else
            {
                if (_frmMain.frmRecipeEditor.LoadRecipe(recipeName))
                {
                    _frmMain.LoadSubForm(_frmMain.frmRecipeEditor);
                }
            }
        }

        private void buttonRecipeDelete_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;

            if (dataGridViewRecipes.SelectedRows.Count != 1)
            {
                return;
            }

            string recipeName = dataGridViewRecipes.SelectedCells[1].Value.ToString();

            if (DialogResult.Yes == nRadMessageBox.Show(this, $"Are you sure you wish to delete recipe:\r\n'{recipeName}'?", "Delete Recipe", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                try
                {
                    Recipe curRecipe = _recipeMgr.FetchRecipe(recipeName);
                    File.Delete(curRecipe.FileName);
                }
                catch (Exception)
                {
                    nRadMessageBox.Show(this, "Error while deleting recipe.  Please try again.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonRecipeAdd_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmRecipeAdd);
        }

        private void buttonRecipeExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Command Seperated (*.csv)|*.csv|Text file (*.txt)|*.txt|All files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Can use dialog.FileName
                    using (Stream stream = dialog.OpenFile())
                    {
                        if (dialog.FileName.ToLower().EndsWith(".csv"))
                        {
                        }
                    }
                }
            }
        }

        private void vScrollBarRecipes_ValueChanged(object sender, EventArgs e)
        {
            _scrollLocation = vScrollBarRecipes.Value;
            UpdateRecipeView();
        }

        private void dataGridViewRecipes_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewRecipes.SelectedRows.Count != 1)
            {
                return;
            }

            string recipeName = dataGridViewRecipes.SelectedCells[1].Value.ToString();

            if (!_recipeMgr.HasRecipe(recipeName))
            {
                nRadMessageBox.Show(this, "ERROR:  Recipe no longer exists...  Please select a new recipe", "Recipe Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _requestRefresh = true;
                return;
            }

            Recipe curRecipe = _recipeMgr.FetchRecipe(recipeName);
            Recipe DefaultTemplate = _recipeMgr.DefaultRecipe;

            if (curRecipe.IsSegmented)
            {
                if (_frmMain.frmSegmentedRecipeEditor.LoadRecipe(recipeName))
                {
                    _frmMain.LoadSubForm(_frmMain.frmSegmentedRecipeEditor);
                }
            }
            else
            {
                if (_frmMain.frmRecipeEditor.LoadRecipe(recipeName))
                {
                    _frmMain.LoadSubForm(_frmMain.frmRecipeEditor);
                }
            }
        }

        private void buttonEditTemplate_Click(object sender, EventArgs e)
        {
            Recipe DefaultTemplate = _recipeMgr.DefaultRecipe;

            if (_frmMain.frmRecipeEditor.LoadRecipe(DefaultTemplate.Name, isDefault: true))
            {
                _frmMain.LoadSubForm(_frmMain.frmRecipeEditor);
            }
        }

        private void buttonSegmentedDefaults_Click(object sender, EventArgs e)
        {
            Recipe DefaultTemplate = _recipeMgr.DefaultRecipe;

            if (_frmMain.frmSegmentedRecipeEditor.LoadRecipe(DefaultTemplate.Name, isDefault: true))
            {
                _frmMain.LoadSubForm(_frmMain.frmSegmentedRecipeEditor);
            }
        }

        #endregion

        #endregion
    }
}
