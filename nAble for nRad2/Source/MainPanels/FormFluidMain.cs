using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormFluidMain : Form, IUpdateableForm
    {
        private FormMain _frmMain = null;
        public FormFluidMain(FormMain formMain)
        {
            _frmMain = formMain;
            InitializeComponent();
        }

        public void UpdateStatus()
        {
            if (_frmMain == null || !Visible)
                return;
#if !DEBUG
            buttonFluidFlow.Enabled = _frmMain.MC.Connected;
#endif
        }

        private void buttonFluidFlow_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.LoadSubForm(_frmMain.frmFluidFlow);
        }

        private void buttonFluidTemp_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.LoadSubForm(_frmMain.frmFluidTemp);
        }

        private void FormFluidMain_Load(object sender, EventArgs e)
        {
#if DEBUG
            buttonFluidFlow.Enabled = true;
#endif
        }
    }
}
