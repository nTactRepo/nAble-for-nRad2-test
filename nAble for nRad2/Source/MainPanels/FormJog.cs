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
    public partial class FormJog : Form, IUpdateableForm
    {
        private FormMain _frmMain = null;
        public FormJog(FormMain formMain)
        {
            _frmMain = formMain;
            InitializeComponent();
        }

        private void FormJog_Load(object sender, EventArgs e)
        {
			if (_frmMain.MS.HasLoader)
			{
				buttonJogTransfer.Visible = true;
				buttonJogXAxis.Size = new Size(276, 152);
				buttonJogXAxis.Location = new Point(726, 22);
				buttonJogZAxis.Size = new Size(276, 152);
				buttonJogZAxis.Location = new Point(726, 181);
			}
			else
			{
				buttonJogTransfer.Visible = false;
				buttonJogXAxis.Size = new Size(276, 227);
				buttonJogXAxis.Location = new Point(726, 22);
				buttonJogZAxis.Size = new Size(276, 227);
				buttonJogZAxis.Location = new Point(726, 267);
			}
		}

        public void UpdateStatus()
        {
            if (_frmMain == null || !Visible)
                return;
            bool bConnected = (_frmMain.MC != null && _frmMain.MC.Connected);
            bool bNotMoving = !_frmMain.MC.Moving || (_frmMain.MC.Moving && _frmMain.MC.PumpPrimeRunning);
            bool bAxisMoving = _frmMain.MC.XMoving || _frmMain.MC.ZMoving;
#if !DEBUG
            buttonJogXAxis.Enabled = bConnected;
            buttonJogZAxis.Enabled = bConnected;
            buttonDieLoadUnloadPos.Enabled = bConnected && bNotMoving && !bAxisMoving && (_frmMain.MS.BypassSafetyGuards || !_frmMain.MC.SafetyGuardsActive);
            buttonGotoMaint.Enabled = bConnected && bNotMoving && !bAxisMoving && (_frmMain.MS.BypassSafetyGuards || !_frmMain.MC.SafetyGuardsActive);
            buttonGotoHome.Enabled = bConnected && bNotMoving && !bAxisMoving && (_frmMain.MS.BypassSafetyGuards || !_frmMain.MC.SafetyGuardsActive);
            buttonFullZUp.Enabled = bConnected && bNotMoving && !bAxisMoving && (_frmMain.MS.BypassSafetyGuards || !_frmMain.MC.SafetyGuardsActive);
            buttonGoToVisionLoc.Enabled = bConnected && bNotMoving && !bAxisMoving && (_frmMain.MS.BypassSafetyGuards || !_frmMain.MC.SafetyGuardsActive);
            buttonGoToVisionLoc.Visible = _frmMain.MS.CognexCommunicationsUsed;
            buttonGoToMeasureLoc.Enabled = bConnected && bNotMoving && !bAxisMoving && (_frmMain.MS.BypassSafetyGuards || !_frmMain.MC.SafetyGuardsActive);
            buttonGoToMeasureLoc.Visible = false;
#endif
        }

        private void buttonJogXAxis_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmJogX);
        }

        private void buttonJogZAxis_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmJogZ);
        }

		private void buttonJogTransfer_Click(object sender, EventArgs e)
		{
			_frmMain.LoadSubForm(_frmMain.frmJogTransfer);
		}

		private void buttonGotoMaint_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;

            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready To Move Die To Maintenance/Drip Tray Position?", "Move Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _frmMain.LastClick = DateTime.Now;
                _frmMain.MC.RunGotoMaintenance();
            }
        }
		private void buttonDieLoadUnloadPos_Click(object sender, EventArgs e)
		{
			_frmMain.LastClick = DateTime.Now;

			if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready To Move Shuttle To Load/Unload Position?", "Move Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
			{
				if (DialogResult.Yes == nRadMessageBox.Show(this, "Is the Lip Guard Attached?", "Move Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
				{
					_frmMain.LastClick = DateTime.Now;
					_frmMain.MC.RunGotoDieLoad();
				}
			}
		}

		private void buttonGotoHome_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready To Move Shuttle To Home Position?", "Move Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _frmMain.LastClick = DateTime.Now;
                _frmMain.MC.RunGotoHome();
            }
        }

        private void buttonFullZUp_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready To Move Z To The Full Height Position?", "Move Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _frmMain.LastClick = DateTime.Now;
                _frmMain.MC.RunFullZUp();
            }
        }

        private void buttonGoToVisionLoc_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready To Move To Cognex Vision Position?", "Move Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _frmMain.LastClick = DateTime.Now;
                _frmMain.MC.RunGotoVisionPosition();
            }
        }

        private void buttonGoToMeasureLoc_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready To Move To Calibration Measure Position?", "Move Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _frmMain.LastClick = DateTime.Now;
                _frmMain.MC.RunGotoMeasurePosition();
            }
        }
    }
}
