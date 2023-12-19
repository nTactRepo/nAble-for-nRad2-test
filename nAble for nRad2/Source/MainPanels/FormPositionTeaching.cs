using System;
using System.Drawing;
using System.Windows.Forms;
using nTact.DataComm;
using System.Diagnostics;
using nTact.PLC;

namespace nAble
{
    public partial class FormPositionTeaching : Form, IUpdateableForm
    {
        private FormMain _frmMain = null;
		private GalilWrapper2 MCPU { get { return _frmMain.MC; } }
		private PLCWrapper PLC { get { return _frmMain.PLC; } }
		private Color _controlColor = Color.FromName("Control");
        private int nCurZPosRaw = 0;
        private int nCurYPosRaw = 0;
        private bool bOneShot = false;
        public FormPositionTeaching(FormMain formMain)
        {
            _frmMain = formMain;
            InitializeComponent();
        }

		private void FormPositionTeaching_Load(object sender, EventArgs e)
        {
		}

		public void UpdateStatus()
        {
			PLC.RobotReadPositions = Visible;
            if (_frmMain == null || !Visible)
            {
                if (PLC != null)
                {
					PLC.SetRobotManualFlag(false);
                }
                bOneShot = true;
                return;
            }
            if(bOneShot && PLC != null)
            {
				PLC.SetRobotManualFlag(true);
                bOneShot = false;
            }
            bool bConnected = (MCPU != null && MCPU.Connected && MCPU.IsHomed);
            bool bMoving = MCPU.Moving;
			bool bZCmdActive = PLC.LoaderZCmdActive;
			bool bYCmdActive = PLC.LoaderYCmdActive;
			bool bManMovesEnabled = PLC.ManMovesEnabled;
            bool bEnableManMoves = bManMovesEnabled && !bZCmdActive && !bYCmdActive;
			bool bEnableZJog = PLC.ZAxisManualMode;
            bool bEnableYJog = PLC.YAxisManualMode;
            nCurZPosRaw = -999;
            nCurYPosRaw = -999;
            int[] ldata = new int[4];

			nCurZPosRaw = PLC.RobotZPositionRaw;
			nCurYPosRaw = PLC.RobotYPositionRaw;

			panelZCmdActive.Visible = bZCmdActive;
            panelYCmdActive.Visible = bYCmdActive;


            if (nCurZPosRaw != -999)
                labelCurZPos.Text = (nCurZPosRaw / 100.00).ToString("#0.00");
            else
                labelCurZPos.Text = "Err";
            if (nCurYPosRaw != -999)
                labelCurYPos.Text = (nCurYPosRaw / 100.00).ToString("#0.00");
            else
                labelCurYPos.Text = "Err";

			// Z Pos Settings
			labelMod1ZHigh.Text = PLC.Mod1ZHighPos.ToString("#0.00");
			labelMod1ZLow.Text = PLC.Mod1ZLowPos.ToString("#0.00");
			labelMod2ZHigh.Text = PLC.Mod2ZHighPos.ToString("#0.00");
			labelMod2ZLow.Text = PLC.Mod2ZLowPos.ToString("#0.00");
			labelMod3ZHigh.Text = PLC.Mod3ZHighPos.ToString("#0.00"); 
			labelMod3ZLow.Text = PLC.Mod3ZLowPos.ToString("#0.00");
			labelZLDULDPos.Text = PLC.ZLDULDPos.ToString("#0.00");
			labelZDockingPos.Text = PLC.ZDockingPos.ToString("#0.00");
			labelZClearPos.Text = PLC.ZClearPos.ToString("#0.00");

			// Y Pos Settings
			labelMod1YPos.Text = PLC.Mod1YPos.ToString("#0.00");
			labelMod2YPos.Text = PLC.Mod2YPos.ToString("#0.00");
			labelMod3YPos.Text = PLC.Mod3YPos.ToString("#0.00");
			labelYLDULDPos.Text = PLC.YLDULDPos.ToString("#0.00");
			labelYDockingPos.Text = PLC.YDockingPos.ToString("#0.00");
			labelYClearPos.Text = PLC.YClearPos.ToString("#0.00");

			labelMod1.Enabled = _frmMain.MS.StackMod1Installed;
			labelMod1ZHigh.Enabled = _frmMain.MS.StackMod1Installed;
			labelMod1ZLow.Enabled = _frmMain.MS.StackMod1Installed;
			buttonTeachZMod1High.Enabled = nCurZPosRaw != -999 && _frmMain.MS.StackMod1Installed;
			buttonTeachZMod1Low.Enabled = nCurZPosRaw != -999 && _frmMain.MS.StackMod1Installed;
			labelMod2.Enabled = _frmMain.MS.StackMod2Installed;
			labelMod2ZHigh.Enabled = _frmMain.MS.StackMod2Installed;
			labelMod2ZLow.Enabled = _frmMain.MS.StackMod2Installed;
			buttonTeachZMod2High.Enabled = nCurZPosRaw != -999 && _frmMain.MS.StackMod2Installed;
			buttonTeachZMod2Low.Enabled = nCurZPosRaw != -999 && _frmMain.MS.StackMod2Installed;
			labelMod3.Enabled = _frmMain.MS.StackMod3Installed;
			labelMod3ZHigh.Enabled = _frmMain.MS.StackMod3Installed;
			labelMod3ZLow.Enabled = _frmMain.MS.StackMod3Installed;
			buttonTeachZMod3High.Enabled = nCurZPosRaw != -999 && _frmMain.MS.StackMod3Installed;
			buttonTeachZMod3Low.Enabled = nCurZPosRaw != -999 && _frmMain.MS.StackMod3Installed;

			labelMod1YPosTitle.Enabled = _frmMain.MS.StackMod1Installed;
			labelMod1YPos.Enabled = _frmMain.MS.StackMod1Installed;
			buttonTeachYMod1Pos.Enabled = nCurYPosRaw != -999 && _frmMain.MS.StackMod1Installed;
			labelMod2YPosTitle.Enabled = _frmMain.MS.StackMod2Installed;
			labelMod2YPos.Enabled = _frmMain.MS.StackMod2Installed;
			buttonTeachYMod2Pos.Enabled = nCurYPosRaw != -999 && _frmMain.MS.StackMod2Installed;
			labelMod3YPosTitle.Enabled = _frmMain.MS.StackMod3Installed;
			labelMod3YPos.Enabled = _frmMain.MS.StackMod3Installed;
			buttonTeachYMod3Pos.Enabled = nCurYPosRaw != -999 && _frmMain.MS.StackMod3Installed;
			buttonTeachZLDULDPos.Enabled = nCurZPosRaw != -999;
			buttonTeachZDockingPos.Enabled = nCurZPosRaw != -999;
			buttonTeachZClearPos.Enabled = nCurZPosRaw != -999;
			buttonTeachYLDULDPos.Enabled = nCurYPosRaw != -999;
			buttonTeachYDockingPos.Enabled = nCurYPosRaw != -999;
			buttonTeachYClearPos.Enabled = nCurYPosRaw != -999;

        }

        private void buttonReturnToPrev_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmLoaderOperation);
        }

        private void buttonTeachZMod1High_Click(object sender, EventArgs e)
        {
			PLC.TeachZPos(StackPosID.MOD1Hi);
        }

        private void buttonTeachZMod1Low_Click(object sender, EventArgs e)
        {
			PLC.TeachZPos(StackPosID.MOD1Low);
        }

        private void buttonTeachZMod2High_Click(object sender, EventArgs e)
        {
			PLC.TeachZPos(StackPosID.MOD2Hi);
        }

        private void buttonTeachZMod2Low_Click(object sender, EventArgs e)
        {
			PLC.TeachZPos(StackPosID.MOD2Low);
		}
		private void buttonTeachZMod3High_Click(object sender, EventArgs e)
		{
			PLC.TeachZPos(StackPosID.MOD3Hi);
		}

		private void buttonTeachZMod3Low_Click(object sender, EventArgs e)
		{
			PLC.TeachZPos(StackPosID.MOD3Low);
		}


		private void buttonTeachZLDULDPos_Click(object sender, EventArgs e)
        {
			PLC.TeachZPos(StackPosID.LDULD);
        }

        private void buttonTeachYMod1Pos_Click(object sender, EventArgs e)
        {
			PLC.TeachYPos(StackPosID.MOD1);
        }

        private void buttonTeachYMod2Pos_Click(object sender, EventArgs e)
        {
			PLC.TeachYPos(StackPosID.MOD2);
		}
		private void buttonTeachYMod3Pos_Click(object sender, EventArgs e)
		{
			PLC.TeachYPos(StackPosID.MOD3);
		}

		private void buttonTeachYLDULDPos_Click(object sender, EventArgs e)
        {
			PLC.TeachYPos(StackPosID.LDULD);
        }

        private void buttonTeachZDockingPos_Click(object sender, EventArgs e)
        {
			PLC.TeachZPos(StackPosID.Docking);
        }

        private void buttonTeachYDockingPos_Click(object sender, EventArgs e)
        {
			PLC.TeachYPos(StackPosID.Docking);
        }

        private void buttonTeachYClearPos_Click(object sender, EventArgs e)
        {
			PLC.TeachYPos(StackPosID.Clear);
        }

        private void buttonTeachZClearPos_Click(object sender, EventArgs e)
        {
			PLC.TeachZPos(StackPosID.Clear);
        }
	}
}
