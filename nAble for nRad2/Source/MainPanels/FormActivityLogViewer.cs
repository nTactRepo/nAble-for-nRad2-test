using nAble.Enums;
using nTact;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormActivityLogViewer : Form, IUpdateableForm
    {
        #region Properties

        public LogTypes TableType { get; set; } = LogTypes.Trace;

        #endregion

        #region Data Members

        private readonly FormMain _formMain = null;
        private readonly DatabaseWrapper _db;
        private readonly LogEntry _log = null;

        private DataTable _activityLog;
        private ToolTip _toolTip = new ToolTip();

        private bool _waiting = false;
        private bool _wasInvisible = false;
        private bool _findingFirstOccurance = false;
        private bool _findingLastOccurance = false;
        private bool _findingNextOccurance = false;
        private bool _findingPrevOccurance = false;

        private string _cellSearchString = "";

        #endregion

        #region Functions

        #region Constructors

        public FormActivityLogViewer(FormMain formMain, DatabaseWrapper db, LogEntry log)
        {
            try
            {
                InitializeComponent();

                _formMain = formMain;
                _db = db;
                _log = log;

                _toolTip.AutoPopDelay = 5000;
                _toolTip.InitialDelay = 1000;
                _toolTip.ReshowDelay = 500;
                _toolTip.ShowAlways = false;

                LoadPLCAddresses();
                LoadLangStrings();

#if DEBUG

                _log.log(LogType.TRACE, Category.INFO, "FormActivityLogViewer Loaded Successfully", "NULL");
#endif
            }
            catch(Exception ex)
            {
                _log.log(LogType.TRACE, Category.ERROR, $"FormActivityLogViewer Failed To Load: {ex.Message}", "NULL");
            }
        }

        #endregion

        #region Public Functions

        public bool LoadLangStrings(bool suspendLayout = true)
        {
            bool loaded = false;

            if (suspendLayout)
            {
                SuspendLayout();
            }

            try
            {
                loaded = true;
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.ERROR, $"FormActivityLogViewer Failed To Change Language - {ex.Message}", "NULL");
            }

            if (suspendLayout)
            {
                ResumeLayout();
            }

            return loaded;
        }

        public void UpdateStatus()
        {
            if (Visible)
            {
                //Update log dates if form was hidden
                if (_wasInvisible)
                {
                    _wasInvisible = false;

                    dateTimeInputStartDate.Value = DateTime.Now;
                    dateTimeInputEndDate.Value = DateTime.Now;
                    StartCreateDataTables();
                }

                expandablePanelSearchControls.TitleText = $"Log Viewer - {TableType} Log";
                panelControls.Enabled = !_waiting;

                bool doingExact = checkBoxXFindOcc.Checked;
                buttonXFindFirst.Enabled = !doingExact;
                buttonXFindLast.Enabled = !doingExact;
                buttonXFindNext.Enabled = !doingExact;
                buttonXFindPrev.Enabled = !doingExact;
            }
            else
            {
                _wasInvisible = true;
            }
        }

        #endregion

        #region Private Functions

        private bool LoadPLCAddresses()
        {
            bool loaded = true;
            return loaded;
        }

        private void StartCreateDataTables()
        {
            Task.Run(CreateDataTablesAsync);
        }

        private async Task CreateDataTablesAsync()
        {
            // If already running, do not try to re-entrantly start
            if (_waiting)
            {
                return;
            }

            _waiting = true;
            UseWaitCursor = true;

            try
            {
                _activityLog = new DataTable();

                string tableName;
                string startDate = $"{dateTimeInputStartDate.Value.ToShortDateString()} 12:00:00 AM";
                string endDate = $"{dateTimeInputEndDate.Value.ToShortDateString()} 11:59:59 PM";

                tableName = TableType == LogTypes.Activity ? "dbo.ActivityLog" : "dbo.TraceLog";

                string sqlStatement = string.Format("SELECT * FROM {0} WHERE TimeStamp BETWEEN '{1}' AND '{2}' ORDER BY TimeStamp DESC", tableName, startDate, endDate);

                using (SqlConnection con = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlStatement, con))
                    {
                        await con.OpenAsync();
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        _activityLog.Load(reader);

                        if (dataGridViewActivityHistory.InvokeRequired)
                        {
                            dataGridViewActivityHistory.Invoke((MethodInvoker)delegate { UpdateUIAfterTableLoad(); });
                        }
                        else
                        {
                            UpdateUIAfterTableLoad();
                        }
                    }
#if DEBUG
                    _log.log(LogType.TRACE, Category.INFO, $"FormActivityLogViewer Successfully Created DataTables - {tableName}");
#endif
                }
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.ERROR, $"FormActivityLogViewer Failed To Create DataTables - {ex.Message}");
            }
            finally
            {
                _waiting = false;
                UseWaitCursor = false;
            }
        }

        private void UpdateUIAfterTableLoad()
        {
            // Setting DataGridView Properties
            if (dataGridViewActivityHistory.CurrentCell != null)
            {
                dataGridViewActivityHistory.CurrentCell.Selected = false;
            }

            dataGridViewActivityHistory.ClearSelection();
            dataGridViewActivityHistory.AllowUserToResizeColumns = true;
            dataGridViewActivityHistory.AutoGenerateColumns = false;

            if (dataGridViewActivityHistory.Columns.Count == 0)
            {
                InitializeColumns(dataGridViewActivityHistory);
            }

            dataGridViewActivityHistory.DataSource = _activityLog;

            vScrollBarAdv1.Value = 0;
            vScrollBarAdv1.Maximum = dataGridViewActivityHistory.RowCount;
            vScrollBarAdv1.LargeChange = dataGridViewActivityHistory.DisplayedRowCount(true);
            vScrollBarAdv1.SmallChange = 1;
        }

        private void InitializeColumns(DataGridView view)
        {
            view.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle()
            {
                Font = new System.Drawing.Font("Tahoma", 14),
                Alignment = DataGridViewContentAlignment.BottomCenter
            };

            // Add Column 1
            var timeStampColumn = new DataGridViewTextBoxColumn()
            {
                HeaderText = "TimeStamp",
                DataPropertyName = "TimeStamp",
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Format = "MM/dd/yyyy hh:mm:ss.ff tt",
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },
                Width = 200
            };

            view.Columns.Add(timeStampColumn);

            // Add Column 2
            var logLevelColumn = new DataGridViewTextBoxColumn()
            {
                HeaderText = "LogLevel",
                DataPropertyName = "LogLevel",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };

            view.Columns.Add(logLevelColumn);

            // Add Column 3
            var descriptionColumn = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Description",
                DataPropertyName = "Description",
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 0, 0, 0)
                },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            view.Columns.Add(descriptionColumn);
        }

        private void FindFirstOccurance()
        {
            DataGridView parentDataGrid = dataGridViewActivityHistory;

            if (textBoxXSearch.Text == "") return;

            int matches = 0;

            if (_findingFirstOccurance == true)
            {
                for (int row = 0; row < parentDataGrid.Rows.Count-1; row++)
                {
                    for (int col = 0; col < parentDataGrid.Columns.Count; col++)
                    {
                        if (parentDataGrid.Rows[row].Cells[col].Value.ToString().Contains(textBoxXSearch.Text))
                        {
                            parentDataGrid.Rows[row].Cells[col].Selected = true;

                            matches++;
                        }
                        if (matches == 1)
                        {
                            _findingFirstOccurance = false;
                            break;
                        }
                    }
                }
            }
        }

        private void FindNext()
        {
            DataGridView parentDataGrid = dataGridViewActivityHistory;

            if (textBoxXSearch.Text == "") return;

            int matches = 0;

            if (_findingNextOccurance == true)
                for (int row = parentDataGrid.CurrentRow.Index + 1; row < parentDataGrid.Rows.Count; row++)
                {
                    for (int col = 0; col < parentDataGrid.Columns.Count; col++)
                    {
                        if (parentDataGrid.Rows[row].Cells[col].Value.ToString().Contains(textBoxXSearch.Text))
                        {
                            parentDataGrid.Rows[row].Cells[col].Selected = true; 
                            matches++;
                        }
                        if (matches > 0)
                        {
                            _findingNextOccurance = false;
                            break;
                        }
                    }
                }
        }

        private void FindPrev()
        {
            DataGridView parentDataGrid = dataGridViewActivityHistory;

            if (textBoxXSearch.Text == "") return;

            int matches = 0;

            if (_findingPrevOccurance == true)
            {
                for (int row = parentDataGrid.CurrentRow.Index - 1; row >= 0; row--)
                {
                    for (int col = 0; col < parentDataGrid.Columns.Count; col++)
                    {
                        if (parentDataGrid.Rows[row].Cells[col].Value.ToString().Contains(textBoxXSearch.Text))
                        {
                            parentDataGrid.Rows[row].Cells[col].Selected = true;
                            matches++;
                        }
                        if (matches == 1)
                        {
                            _findingPrevOccurance = false;
                            break;
                        }
                    }
                }
            }
        }

        private void FindLast()
        {
            DataGridView parentDataGrid = dataGridViewActivityHistory;

            if (textBoxXSearch.Text == "") return;

            int matches = 0;

            if (_findingLastOccurance == true)
                for (int row = parentDataGrid.Rows.Count-1; row > 0; row--)
                {
                    for (int col = 0; col < parentDataGrid.Columns.Count; col++)
                    {
                        if (parentDataGrid.Rows[row].Cells[col].Value.ToString().Contains(textBoxXSearch.Text))
                        {
                            parentDataGrid.Rows[row].Cells[col].Selected = true;
                            matches++;
                        }
                        if (matches == 1)
                        {
                            _findingLastOccurance = false;
                            break;
                        }
                    }
                }
        }

        private DataTable GetDataTableFromDGV(DataGridView dgv)
        {
            var dt = new DataTable();
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if (column.Visible)
                {
                    dt.Columns.Add();
                }
            }

            object[] cellValues = new object[dgv.Columns.Count];
            foreach (DataGridViewRow row in dgv.Rows)
            {
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    cellValues[i] = row.Cells[i].Value;
                }
                dt.Rows.Add(cellValues);
            }

            return dt;
        }

        #endregion

        #region Control Event Handlers

        private void FormActivityLogViewer_Load(object sender, EventArgs e)
        {
            dateTimeInputStartDate.Value = DateTime.Now;
            dateTimeInputEndDate.Value = DateTime.Now;
            dateTimeInputEndDate.MinDate = dateTimeInputStartDate.Value;

            textBoxXSearch.Focus();
            textBoxXSearch.Text = "";
        }

        private void ButtonXExportXML_Click(object sender, EventArgs e)
        {
            try
            {
                string newFileName = "";

                DataTable dT = GetDataTableFromDGV(dataGridViewActivityHistory);
                DataSet dS = new DataSet();
                dS.Tables.Add(dT);

                SaveFileDialog folderBrowser = new SaveFileDialog();
                folderBrowser.InitialDirectory = @"C:\nTact";
                folderBrowser.ValidateNames = false;
                folderBrowser.CheckFileExists = false;
                folderBrowser.CheckPathExists = true;

                newFileName = "Trace_Log.xml";

                folderBrowser.FileName = newFileName;
                

                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
                    string newFullPath = Path.Combine(folderPath, newFileName);
                    dS.WriteXml(newFullPath);

                    _log.log(LogType.TRACE, Category.INFO, $"XML Log Export Completed Successfully: {newFullPath}");
                }
            }
            catch(Exception ex)
            {
                _log.log(LogType.TRACE, Category.ERROR, $"XML Log Export Failed - {ex.Message}");
            }
        }

        private void ButtonXFindNext_Click(object sender, EventArgs e)
        {
            if (!checkBoxXFindOcc.Checked && textBoxXSearch.Text != "")
            {
                _findingNextOccurance = true;
                FindNext();
            }
        }

        private void ButtonXFindLast_Click(object sender, EventArgs e)
        {
            if (!checkBoxXFindOcc.Checked && textBoxXSearch.Text != "")
            {
                _findingFirstOccurance = true;
                FindFirstOccurance();
            }
        }

        private void VScrollBarAdv1_Scroll(object sender, ScrollEventArgs e)
        {
            if (dataGridViewActivityHistory.Rows.Count >= 1)
            {
                dataGridViewActivityHistory.FirstDisplayedScrollingRowIndex = e.NewValue;
            }
        }

        private void DateTimeInputStartDate_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimeInputStartDate.Value > dateTimeInputEndDate.Value)
            {
                dateTimeInputEndDate.Value = dateTimeInputStartDate.Value;
            }

            dateTimeInputEndDate.MinDate = dateTimeInputStartDate.Value;
        }

        private async void ButtonXGo_Click(object sender, EventArgs e)
        {
            try
            {
                _activityLog.Clear();
                await CreateDataTablesAsync();

                if (checkBoxXFindOcc.Checked)
                {
                    string extra = TableType == LogTypes.Activity ? " + userid" : "";
                    _activityLog.DefaultView.RowFilter = $"description + loglevel{extra} like '%{textBoxXSearch.Text.Replace("'", "''")}%'";

                    dataGridViewActivityHistory.DataSource = _activityLog.DefaultView.ToTable();

                    vScrollBarAdv1.Value = 0;
                    vScrollBarAdv1.Maximum = dataGridViewActivityHistory.RowCount;
                    vScrollBarAdv1.LargeChange = dataGridViewActivityHistory.DisplayedRowCount(true);
                    vScrollBarAdv1.SmallChange = 1;

                    dataGridViewActivityHistory.ClearSelection();
                }
                else if (!checkBoxXFindOcc.Checked && textBoxXSearch.Text != "")
                {
                    _findingFirstOccurance = true;
                    FindFirstOccurance();
                }
                else if (!checkBoxXFindOcc.Checked)
                {
                    textBoxXSearch.Text = "";
                }
            }
            catch(Exception ex)
            {
                _log.log(LogType.TRACE, Category.ERROR, $"FormActivityLogViewer | Failed To Perform Search: {ex.Message}");
            }
        }

        private void DataGridViewActivityHistory_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                vScrollBarAdv1.Value = e.NewValue;
            }
        }

        private void ButtonXFindFirst_Click(object sender, EventArgs e)
        {
            // Using FindLast As FindFirst So it Finds Earliest Date Item As First
            if (!checkBoxXFindOcc.Checked && textBoxXSearch.Text != "")
            {
                _findingLastOccurance = true;
                FindLast();
            }
        }

        private void ButtonXFindPrev_Click(object sender, EventArgs e)
        {
            if (!checkBoxXFindOcc.Checked && textBoxXSearch.Text != "")
            {
                _findingPrevOccurance = true;
                FindPrev();
            }
        }

        private void DataGridViewActivityHistory_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridViewActivityHistory.ClearSelection();
        }

        private void FormActivityLogViewer_Leave(object sender, EventArgs e)
        {
            textBoxXSearch.Text = "";
            _activityLog.Clear();
        }

        private void dataGridViewActivityHistory_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.ColumnIndex != 2 || e.RowIndex == -1)
            {
                return;
            }

            e.ContextMenuStrip = contextMenuStripResults;
            _cellSearchString = dataGridViewActivityHistory[e.ColumnIndex, e.RowIndex].Value.ToString();
        }

        private void contextMenuStripResults_Click(object sender, EventArgs e)
        {
            textBoxXSearch.Text = _cellSearchString;
            buttonXGo.PerformClick();
        }

        private void checkBoxXFindOcc_CheckedChanged(object sender, EventArgs e)
        {
            buttonXGo.PerformClick();
        }

        #endregion

        #endregion
    }
}
