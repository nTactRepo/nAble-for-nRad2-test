using nAble.Data;
using nAble.DataComm;
using nTact.DataComm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormVisionView : Form, IUpdateableForm
    {
        private FormMain _frmMain = null;
        private readonly LogEntry _log = null;

        bool bChangingCamera = false;
        private GalilWrapper2 MC { get { return _frmMain.MC; } }
        private MachineSettingsII MS { get { return _frmMain.MS; } }
        private MachineStorage SV { get { return _frmMain.Storage; } }

        private clsCognex CognexVision { get { return _frmMain.CognexVision; } }
        public AlignmentResults AlignmentResult;
        public BackgroundWorker bgWorker;
        public BackgroundWorker bgwGoTo;
        private bool bGotoFail = false;
        public AlignmentResults PrevAlignmentResult;
        public bool WaitingOnResults = false;
        private bool InitImageAcqOK = false;
        private bool abort = false;
        public bool Abort { get { return abort; } set { abort = value; } }

        private bool bVisibleOneShot = true;
        public FormVisionView(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain;
            _log = log;

            AlignmentResult = new AlignmentResults();
            clsCognex.ResultsAcquired += ClsCognex_ResultsAcquired;
            InitializeComponent();
        }

        private void ClsCognex_ResultsAcquired(object o, EventArgs e)
        {
            pbLeft.Image = CognexVision.AlignmentResult.LeftImage;
            pbRight.Image = CognexVision.AlignmentResult.RightImage;
        }

        public void UpdateStatus()
        {
            if (_frmMain == null || !Visible)
                return;
            if (bVisibleOneShot)
            {
                bVisibleOneShot = false;
            }
            bool cameraConnected = _frmMain.CognexVision.IsConnected;
            bool bConnected = (_frmMain.MC != null && _frmMain.MC.Connected);
            pbLeft.Image = CognexVision.LeftCamera != null && CognexVision.LeftCamera.Results != null ? CognexVision.LeftCamera.Results.Image.ToBitmap() : null;
            pbRight.Image = CognexVision.RightCamera != null && CognexVision.RightCamera.Results != null ? CognexVision.RightCamera.Results.Image.ToBitmap() : null;
            gpLeftCamJobPass.Style.BackColor = CognexVision.LeftCamera.Results.Cells["E24"].ToString() == "1" ? Color.FromArgb(0, 255, 0) : Color.Red;
            gpLeftCamJobPass.Style.BackColor2 = CognexVision.LeftCamera.Results.Cells["E24"].ToString() == "1" ? Color.FromArgb(0, 195, 0) : Color.FromArgb(198, 0, 0);
            gpRightCamJobPass.Style.BackColor = CognexVision.RightCamera.Results.Cells["E24"].ToString() == "1" ? Color.FromArgb(0, 255, 0) : Color.Red;
            gpRightCamJobPass.Style.BackColor2 = CognexVision.RightCamera.Results.Cells["E24"].ToString() == "1" ? Color.FromArgb(0, 195, 0) : Color.FromArgb(198, 0, 0);
            pbLeftJobPass.Image = CognexVision.LeftCamera.Results.Cells["E24"].ToString() == "1" ? Resources.checkmark_48px : Resources.delete_100px;
            pbRightJobPass.Image = CognexVision.RightCamera.Results.Cells["E24"].ToString() == "1" ? Resources.checkmark_48px : Resources.delete_100px;
        }

        private void FormVisionControl_Load(object sender, EventArgs e)
        {

        }

        private void buttonXPrevScreen_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain._prevForm);
            _frmMain._prevForm = null;
            _log.log(LogType.TRACE, Category.INFO, "FormVisionControl: User Pressed Prev Screen Button.", "ACTION");
        }

        private void buttonXTrigger_Click(object sender, EventArgs e)
        {

        }


        private bool ConfirmAction()
        {
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Are You Sure?", "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                return true;
            }
            else
                return false;
        }

        private bool AcquireImages(string desc)
        {
            bool bContinue = true;
            //Acquire Images and Results
            _log.log(LogType.TRACE, Category.INFO, $"AcquireImages() Acquiring {desc} Images and Results", "INFO");
            CognexVision.AcquireResults();
            int timeout = 0;
            int maxtimeout = 5000 / 50;
            while (CognexVision.AcquiringResults && timeout < maxtimeout)
            {
                Thread.Sleep(50);
            }
            if (timeout >= maxtimeout)
            {
                bContinue = false;
                _log.log(LogType.TRACE, Category.INFO, $"AcquireImages() Timeout Waiting on Results", "INFO");
            }
            else
            {

            }
            return bContinue;
        }

        private void buttonTrigger_Click(object sender, EventArgs e)
        {
            AcquireImages("Cognex Camera");
            DisplayResults();
        }
        private void DisplayResults()
        {
            if (pbLeft.InvokeRequired)
            {
                pbLeft.Invoke(new MethodInvoker(delegate () { pbLeft.Image = CognexVision.AlignmentResult.LeftImage; }));
            }
            else
                pbLeft.Image = CognexVision.AlignmentResult.LeftImage;

            if (pbRight.InvokeRequired)
            {
                pbRight.Invoke(new MethodInvoker(delegate () { pbRight.Image = CognexVision.AlignmentResult.RightImage; }));
            }
            else
                pbRight.Image = CognexVision.AlignmentResult.RightImage;
        }

        private void FormVisionView_Load(object sender, EventArgs e)
        {

        }

        private void buttonPrevScreen_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmRun);
        }
    }
}
