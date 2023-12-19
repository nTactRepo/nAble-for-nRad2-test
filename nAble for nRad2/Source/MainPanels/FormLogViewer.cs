using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace nAble
{
    public partial class FormLogViewer : Form, IUpdateableForm
    {
        private DataTable dtActivityLog;
        private FormMain _frmMain = null;
		private Int32 nScrollLocation;
        private int nViewerRows = 18;
        public Stopwatch stopWatch = new Stopwatch();
        static BackgroundWorker _bwLoadLogFile;
        private System.Windows.Forms.Timer displayTimer = new System.Windows.Forms.Timer();
        private string searchString = null;
        private int[] searchResults;
        private int nSearchIndex = 0;
        private string prevFilter = "NONE";

        public FormLogViewer(FormMain formMain)
        {
            _bwLoadLogFile = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _bwLoadLogFile.DoWork += new DoWorkEventHandler(_bwLoadLogFile_DoWork);
            _bwLoadLogFile.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bwLoadLogFile_RunWorkerCompleted);
            _bwLoadLogFile.ProgressChanged += new ProgressChangedEventHandler(_bwLoadLogFile_ProgressChanged);
            displayTimer.Tick += new EventHandler(displayTimer_Tick);
            _frmMain = formMain;
            InitializeComponent();
        }
        void _bwLoadLogFile_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            labelLoadProgress.Text = "Loading Log: " + e.ProgressPercentage.ToString() + "%";
        }

        void _bwLoadLogFile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            stopWatch.Stop();
            labelLoadProgress.Text = "Load Time: " + stopWatch.Elapsed.TotalSeconds.ToString("#0.000") + Environment.NewLine + "Total Log Entries: " + dtActivityLog.Rows.Count.ToString();
            stopWatch.Reset();
            displayTimer.Interval = 2000;
            displayTimer.Start();
            nScrollLocation = 1;
            if (dtActivityLog.Rows.Count > nViewerRows)
            {
                vScrollBarLog.Visible = true;
                vScrollBarLog.Maximum = dtActivityLog.Rows.Count;
                vScrollBarLog.LargeChange = nViewerRows;
                vScrollBarLog.Value = vScrollBarLog.Maximum - nViewerRows;
            }
            else
            {
                vScrollBarLog.Visible = false;
            }
            UpdateLogView();
        }

        void _bwLoadLogFile_DoWork(object sender, DoWorkEventArgs e)
        {
            double totalBytes = 0;
            double totalBytesRead = 0;
            int loadProgressTest = 10;
            string fileName = e.Argument.ToString();
            FileInfo f = new FileInfo(fileName);
            totalBytes = f.Length;
            stopWatch.Start();
            string sLine;
            int nRow = 0;
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            dtActivityLog.Clear();
            dtActivityLog.BeginLoadData();
            while (!sr.EndOfStream)
            {
                sLine = sr.ReadLine();
                totalBytesRead = totalBytesRead + sLine.Length;

                if (((totalBytesRead / totalBytes) * 100) > loadProgressTest)
                {
                    loadProgressTest = loadProgressTest + 10;
                    _bwLoadLogFile.ReportProgress((int)((totalBytesRead / totalBytes) * 100));
                }
                nRow++;
                dtActivityLog.Rows.Add(nRow, sLine);
            }
            sr.Close();
            dtActivityLog.EndLoadData();
        }
        void displayTimer_Tick(object sender, EventArgs e)
        {
            displayTimer.Stop();
            labelLoadProgress.Visible = false;
        }
        private void FormLogViewer_Load(object sender, EventArgs e)
        {
            nScrollLocation = 1;
            CreateDataTables();
            //gpFilter.Visible = true;
            rbNone.Checked = true;
            prevFilter = "NONE";
        }
        private void CreateDataTables()
        {
            //Create Alarm Log Datatable
            dtActivityLog = new DataTable();
            // Define columns
            DataColumn dc2;
            dc2 = new DataColumn("No");
            dc2.DataType = typeof(Int32);
            dtActivityLog.Columns.Add(dc2);
            dc2 = new DataColumn("Log Entry");
            dtActivityLog.Columns.Add(dc2);
            dtActivityLog.PrimaryKey = new DataColumn[] { dtActivityLog.Columns["No"] };
        }

        public void UpdateStatus()
        {
        }
		public FileInfo LogFile { get; set; } = null;

		private void FormLogViewer_Enter(object sender, EventArgs e)
        {
            vScrollBarLog.Visible = false;
            labelLoadProgress.Text = "Loading Log: 0%";
            labelLoadProgress.Visible = true;
            _bwLoadLogFile.RunWorkerAsync(LogFile.FullName);
        }

        private bool UpdateLogView()
        {
            string strSelect = "No >= " + nScrollLocation.ToString() + " AND No <= " + (nScrollLocation + nViewerRows).ToString();
            try
            {
                DataRow[] result = dtActivityLog.Select(strSelect);
                dataGridViewActivityHistory.DataSource = result.CopyToDataTable();
                dataGridViewActivityHistory.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridViewActivityHistory.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridViewActivityHistory.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridViewActivityHistory.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewActivityHistory.AllowUserToResizeColumns = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return true;
        }
        private void buttonOpenIn_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;

        }

        private void dataGridViewActivityHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show(dataGridViewActivityHistory.Rows[e.RowIndex].Cells[1].Value.ToString());
        }

        private void vScrollBarLog_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void vScrollBarLog_ValueChanged(object sender, EventArgs e)
        {
            nScrollLocation = vScrollBarLog.Value;
            UpdateLogView();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormFind dlgFind = new FormFind();
            string finalSearchString;
            if (dlgFind.ShowDialog(this) == DialogResult.OK)
            {
                searchString = dlgFind.SearchString;
                StringBuilder sb = new StringBuilder();
                for (int t = 0; t < searchString.Length; t++)
                {
                    char c = searchString[t];
                    if (c == '*' || c == '%' || c == '[' || c == ']')
                        sb.Append("[").Append(c).Append("]");
                    else
                        sb.Append(c);
                }
                finalSearchString = sb.ToString();
                string strSelect = "[Log Entry] LIKE '*" + finalSearchString + "*'";
                DataRow[] drSearch = dtActivityLog.Select(strSelect);
                searchResults = new int[drSearch.Count()];
                int i = 0;
                foreach (DataRow dtRow in drSearch)
                {
                    searchResults[i] = int.Parse(dtRow["No"].ToString());
                    i++;
                }
                if (drSearch.Count() > 0)
                {
                    vScrollBarLog.Value = searchResults[0];
                    nSearchIndex = 0;
                }
            }

        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nSearchIndex < searchResults.Count() - 1)
            {
                nSearchIndex++;
                vScrollBarLog.Value = searchResults[nSearchIndex];
            }
        }

        private void findPrevToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nSearchIndex > 0)
            {
                nSearchIndex--;
                vScrollBarLog.Value = searchResults[nSearchIndex];
            }
        }

        private void dataGridViewActivityHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridViewActivityHistory_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                FormFind dlgFind = new FormFind();
                dlgFind.ShowDialog(dataGridViewActivityHistory);
            }
        }

        private void FilterLogs(string FilterByCategory)
        {
            prevFilter = FilterByCategory;

            DataTable table = dtActivityLog;
            bool complete = false;
            bool resetLoop = false;
            int count = 0;

            do
            {
                do
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (!row.ItemArray[1].ToString().Contains(FilterByCategory))
                            table.Rows.Remove(row);
                        complete = row.Table.Rows.IndexOf(row) == table.Rows.Count;
                        resetLoop = row.Table.Rows.IndexOf(row) != table.Rows.Count;
                    }
                } while (!resetLoop && !complete);
            } while (!complete);
        }

        private void rbInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (prevFilter != "INFO")
            {
                FilterLogs("INFO");
            }
        }

        private void rbErr_CheckedChanged(object sender, EventArgs e)
        {
            if (prevFilter != "ERROR")
            {
                FilterLogs("ERROR");
            }
        }

        private void rbAction_CheckedChanged(object sender, EventArgs e)
        {
            if (prevFilter != "ACTION")
            {
                FilterLogs("ACTION");
            }
        }

        private void rbNone_CheckedChanged(object sender, EventArgs e)
        {
            if (prevFilter != "NONE")
            {
                FilterLogs("NONE");
            }
        }
    }
}
