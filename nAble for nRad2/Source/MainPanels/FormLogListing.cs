using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace nAble
{
    public partial class FormLogListing : Form, IUpdateableForm
    {
        private FormMain _frmMain = null;
        public FormLogListing(FormMain formMain)
        {
            _frmMain = formMain;
            InitializeComponent();
        }

        private void FormLogListing_Enter(object sender, EventArgs e)
        {
            listBoxLogFiles.Items.Clear();
            DirectoryInfo di = new DirectoryInfo(@"Data\Logs");
            foreach (FileInfo fi in di.GetFiles("*.log"))
            {
                listBoxLogFiles.Items.Insert(0,fi);
            }
        }

        public void UpdateStatus()
        {
        }

        private void listBoxLogFiles_DoubleClick(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (listBoxLogFiles.SelectedItem != null)
            {
                UseWaitCursor = true;

                if (Settings.Default.UseLogDB)
                {
                    _frmMain.LoadSubForm(_frmMain.frmActivityLogViewer);
                }
                else
                {
                    _frmMain.frmLogViewer.LogFile = (FileInfo)listBoxLogFiles.SelectedItem;
                    _frmMain.LoadSubForm(_frmMain.frmLogViewer);
                }

                UseWaitCursor = false;
            }
        }

        private void FormLogListing_Load(object sender, EventArgs e)
        {
            listBoxLogFiles.Sorted = false;
        }

        private void listBoxLogFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
