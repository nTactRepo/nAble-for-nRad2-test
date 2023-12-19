using System;
using System.Drawing;
using System.Windows.Forms;
using nTact.DataComm;
using System.Diagnostics;
using nTact.PLC;

namespace nAble
{
    public partial class FormLoaderOperation : Form, IUpdateableForm
    {
        private FormMain _frmMain = null;
		private readonly LogEntry _log = null;
		private PLCWrapper PLC { get { return _frmMain.PLC; } }
		private GalilWrapper2 MCPU { get { return _frmMain.MC; } }
        private Color _controlColor = Color.FromName("Control");
        private bool bUpdateOneShot = false;
        public FormLoaderOperation(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain;
			_log = log;

            InitializeComponent();
        }

		private void FormLoaderOperation_Load(object sender, EventArgs e)
		{
			labelModule1.Text = $"{_frmMain.MS.StackMod1Name}:";
			labelModule2.Text = $"{_frmMain.MS.StackMod2Name}:";
			labelModule2.Text = $"{_frmMain.MS.StackMod2Name}:";
			buttonMoveYToModule1Chuck.Text = $"{_frmMain.MS.StackMod1Name} Chuck Pos";
			buttonMoveYToModule2Chuck.Text = $"{_frmMain.MS.StackMod2Name} Chuck Pos";
			buttonMoveYToModule3Chuck.Text = $"{_frmMain.MS.StackMod2Name} Chuck Pos";
		}


		public void UpdateStatus()
        {
            if (_frmMain == null || !Visible)
            {
                bUpdateOneShot = true;
                return;
            }
            if(bUpdateOneShot)
            {
				buttonZVelocity.Text = PLC.LoaderZVel.ToString("#0");
                buttonYVelocity.Text = PLC.LoaderYVel.ToString("#0");
				bUpdateOneShot = false;
            }
            bool bConnected = (MCPU != null && MCPU.Connected && MCPU.IsHomed);
            bool bMoving = _frmMain.MC.Moving;
			bool bZCmdActive = PLC.LoaderZCmdActive;
			bool bYCmdActive = PLC.LoaderYCmdActive;
			bool bManMovesEnabled = PLC.ManMovesEnabled;
            bool bEnableManMoves = bManMovesEnabled && !bZCmdActive && !bYCmdActive;
			bool bEndEffectorSubstrate = PLC.IsEndEffectorOccupied;
			bool bEndEffectorDocked = PLC.IsLoadArmDocked;
			labelManualMovesDisabled.Visible = !bManMovesEnabled;
            panelZCmdActive.Visible = bZCmdActive;
            panelYCmdActive.Visible = bYCmdActive;

			labelModule1.Visible = _frmMain.MS.StackMod1Installed;
			buttonMoveZToMod1PosHigh.Visible = _frmMain.MS.StackMod1Installed;
			buttonMoveZToMod1PosLow.Visible = _frmMain.MS.StackMod1Installed;
			buttonMoveZToMod1PosHigh.Enabled = bEnableManMoves;
			buttonMoveZToMod1PosLow.Enabled = bEnableManMoves;

			labelModule2.Visible = _frmMain.MS.StackMod2Installed;
			buttonMoveZToMod2PosHigh.Visible = _frmMain.MS.StackMod2Installed;
			buttonMoveZToMod2PosLow.Visible = _frmMain.MS.StackMod2Installed;
			buttonMoveZToMod2PosHigh.Enabled = bEnableManMoves;
            buttonMoveZToMod2PosLow.Enabled = bEnableManMoves;

			labelModule3.Visible = _frmMain.MS.StackMod3Installed;
			buttonMoveZToMod3PosHigh.Visible = _frmMain.MS.StackMod3Installed;
			buttonMoveZToMod3PosLow.Visible = _frmMain.MS.StackMod3Installed;
			buttonMoveZToMod3PosHigh.Enabled = bEnableManMoves;
            buttonMoveZToMod3PosLow.Enabled = bEnableManMoves;
            buttonMoveZToMod3PosLow.Enabled = bEnableManMoves;
            buttonMoveZToLDULD.Enabled = bEnableManMoves;
            buttonMoveZToDockingPos.Enabled = bEnableManMoves;
            buttonMoveZToClear.Enabled = bEnableManMoves;

			buttonMoveYToModule1Chuck.Visible = _frmMain.MS.StackMod1Installed;
            buttonMoveYToModule1Chuck.Enabled = bEnableManMoves && PLC.Mod1CommEnabled;
			buttonMoveYToModule2Chuck.Visible = _frmMain.MS.StackMod2Installed;
			buttonMoveYToModule2Chuck.Enabled = bEnableManMoves && PLC.Mod2CommEnabled;
			buttonMoveYToModule3Chuck.Visible = _frmMain.MS.StackMod3Installed;
			buttonMoveYToModule3Chuck.Enabled = bEnableManMoves && PLC.Mod3CommEnabled;
            buttonMoveYToLDULD.Enabled = bEnableManMoves;
            buttonMoveYToDockingPos.Enabled = bEnableManMoves;
            buttonMoveYToClear.Enabled = bEnableManMoves;
            buttonHomeLoader.Enabled = !bZCmdActive && !bYCmdActive;

			labelZMoving.Visible = PLC.LoaderZMoving;
			labelYMoving.Visible = PLC.LoaderYMoving;

            buttonEnableStackMaint.BackColor = PLC.StackInMaintMode ? Color.Orange : SystemColors.Control;
            buttonMoveZToMod1PosHigh.BackColor = PLC.ZAtMod1PosHigh ? Color.Orange : SystemColors.Control;
			buttonMoveZToMod1PosLow.BackColor = PLC.ZAtMod1PosLow ? Color.Orange : SystemColors.Control;
			buttonMoveZToMod2PosHigh.BackColor = PLC.ZAtMod2PosHigh ? Color.Orange : SystemColors.Control;
			buttonMoveZToMod2PosLow.BackColor = PLC.ZAtMod2PosLow ? Color.Orange : SystemColors.Control;
			buttonMoveZToMod3PosHigh.BackColor = PLC.ZAtMod3PosHigh ? Color.Orange : SystemColors.Control;
			buttonMoveZToMod3PosLow.BackColor = PLC.ZAtMod3PosLow ? Color.Orange : SystemColors.Control;
			buttonMoveZToLDULD.BackColor = PLC.ZAtLDULDPos ? Color.Orange : SystemColors.Control;
            buttonMoveZToDockingPos.BackColor = PLC.ZAtDockingPos ? Color.Orange : SystemColors.Control;
            buttonMoveZToClear.BackColor = PLC.ZAtClearPos ? Color.Orange : SystemColors.Control;



			buttonMoveYToModule1Chuck.BackColor = PLC.YAtMod1ChuckPos ? Color.Orange : SystemColors.Control;
			buttonMoveYToModule2Chuck.BackColor = PLC.YAtMod2ChuckPos ? Color.Orange : SystemColors.Control;
			buttonMoveYToModule3Chuck.BackColor = PLC.YAtMod3ChuckPos ? Color.Orange : SystemColors.Control;
            buttonMoveYToLDULD.BackColor = PLC.YAtLDULDPos ? Color.Orange : SystemColors.Control;
            buttonMoveYToDockingPos.BackColor = PLC.YAtDockPos ? Color.Orange : SystemColors.Control;
            buttonMoveYToClear.BackColor = PLC.YAtClearPos ? Color.Orange : SystemColors.Control;

            buttonHomeLoader.BackColor = PLC.HomeLoader ? Color.Orange : SystemColors.Control;
            buttonDockEndEffector.Enabled = PLC.StackInMaintMode;
            buttonDockEndEffector.BackColor = PLC.LockQuickChange ? Color.Yellow : SystemColors.Control;
            buttonDockEndEffector.Text = PLC.LockQuickChange ? "Rls End Efector" : "Dock End Effector";

            labelVacuumSensor.BackColor = PLC.EndEffectorVacuum ? Color.LimeGreen : Color.DarkGray;
            labelLoaderSubstrate.BackColor = bEndEffectorSubstrate ? Color.LimeGreen : Color.DarkGray;
            buttonEndEffectorVacuum.Enabled = PLC.StackInMaintMode;
            buttonEndEffectorVacuum.BackColor = PLC.EndEffectorVac ? Color.LimeGreen : SystemColors.Control;
            buttonEndEffectorVacuum.Text = PLC.EndEffectorVac ? "Disable Vacuum" : "Enable Vacuum";

			bool ZAxisError = !PLC.ZAxisNoError;
			bool YAxisError = !PLC.YAxisNoError;
            buttonResetError.Visible = ZAxisError || YAxisError;
            labelZError.Visible = ZAxisError;
            labelYError.Visible = YAxisError;

            if (PLC.EndEffectorPresent)
            {
                if(PLC.LockQuickChange)
                {
                    labelDockState.Text = "Docked";
                    labelDockState.BackColor = Color.LimeGreen;
                    labelDockState.ForeColor = Color.White;
                }
                else
                {
                    labelDockState.Text = "Not Docked";
                    labelDockState.BackColor = Color.Yellow;
                    labelDockState.ForeColor = Color.Black;
                }
            }
            else
            {
                labelDockState.Text = "Not Ready";
                labelDockState.BackColor = Color.DarkGray;
                labelDockState.ForeColor = Color.White;
            }
        }

		#region Z Moves
		private void buttonMoveZToMod1PosHigh_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Robot Arm Z (up/dwn) move to High Pos of Mod 1:  '{_frmMain.MS.StackMod1Type} - {_frmMain.MS.StackMod1Name}'", "Action");
			PLC.MoveZToStackMod1High();
		}

		private void buttonMoveZToMod1PosLow_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Robot Arm Z (up/dwn) move to Low Pos of Mod 1:  '{_frmMain.MS.StackMod1Type} - {_frmMain.MS.StackMod1Name}'", "Action");
			PLC.MoveZToStackMod1Low();
		}

		private void buttonMoveZToMod2PosHigh_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Robot Arm Z (up/dwn) move to High Pos of Mod 2:  '{_frmMain.MS.StackMod2Type} - {_frmMain.MS.StackMod2Name}'", "Action");
			PLC.MoveZToStackMod2High();
			_log.log(LogType.TRACE, Category.INFO, "User Requested move to ", "Action");
		}

		private void buttonMoveZToMod2PosLow_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Robot Arm Z (up/dwn) move to Low Pos of Mod 2:  '{_frmMain.MS.StackMod2Type} - {_frmMain.MS.StackMod2Name}'", "Action");
			PLC.MoveZToStackMod2Low();
		}

		private void buttonMoveZToMod3PosHigh_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Robot Arm Z (up/dwn) move to High Pos of Mod 3:  '{_frmMain.MS.StackMod3Type} - {_frmMain.MS.StackMod3Name}'", "Action");
			PLC.MoveZToStackMod3High();
		}

		private void buttonMoveZToMod3PosLow_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Robot Arm Z (up/dwn) move to Low Pos of Mod 3:  '{_frmMain.MS.StackMod3Type} - {_frmMain.MS.StackMod3Name}'", "Action");
			PLC.MoveZToStackMod3Low();
		}

		private void buttonMoveZToClear_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Robot Arm Z (up/dwn) move to Clear Position", "Action");
			PLC.MoveZToClearPos();
		}

		private void buttonMoveZToLDULD_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Robot Arm Z (up/dwn) move to Load/Unload Position", "Action");
			PLC.MoveZToLDULDPos();
		}

		private void buttonMoveZToDockingPos_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"FrmLoaderOperation: User Requested Robot Arm Z (up/dwn) move to Docking Position", "Action");
			PLC.MoveZToDockingPos();
		}

		private void buttonZVelocity_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen("Z-Axis Move Velocity (1-100)", this, buttonZVelocity, "#0", 1, 100, "", false, false);
		}

		private void buttonZVelocity_TextChanged(object sender, EventArgs e)
		{
			if (!bUpdateOneShot)
			{
				_log.log(LogType.TRACE, Category.INFO, $"User Changed Robot Arm Z velocity to '{buttonZVelocity.Text}'", "Action");
				PLC.SetRobotArmZVel(Convert.ToInt32(buttonYVelocity.Text));
			}
		}


		#endregion Z Moves


		#region Y Moves

		private void buttonMoveYToModule1Chuck_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Robot Arm Y (in/out) move to Mod 1  '{_frmMain.MS.StackMod1Type} - {_frmMain.MS.StackMod1Name}'", "Action");
			PLC.MoveYToStackMod1();
		}

		private void buttonMoveYToModule2Chuck_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Robot Arm Y (in/out) move to Mod 2  '{_frmMain.MS.StackMod2Type} - {_frmMain.MS.StackMod2Name}'", "Action");
			PLC.MoveYToStackMod2();
		}

		private void buttonMoveYToModule3Chuck_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Robot Arm Y (in/out) move to Mod 3  '{_frmMain.MS.StackMod3Type} - {_frmMain.MS.StackMod3Name}'", "Action");
			PLC.MoveYToStackMod3();
		}

		private void buttonMoveYToClear_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Robot Arm Y (in/out) move to Clear Position", "Action");
			PLC.MoveYToClearPos();
		}

		private void buttonMoveYToLDULD_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Robot Arm Y (in/out) move to Load/Unload Position", "Action");
			PLC.MoveYToLDULDPos();
		}

		private void buttonMoveYToDockingPos_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"FrmLoaderOperation: User Requested Robot Arm Y (in/out) move to Docking Position", "Action");
			PLC.MoveYToDockingPos();
		}

		private void buttonYVelocity_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen("Y-Axis Move Velocity (1-100)", this, buttonYVelocity, "#0", 1, 100, "", false, false);
		}
		private void buttonYVelocity_TextChanged(object sender, EventArgs e)
		{
			if (!bUpdateOneShot)
			{
				_log.log(LogType.TRACE, Category.INFO, $"User Changed Robot Arm Y velocity to '{buttonYVelocity.Text}'", "Action");
				PLC.SetRobotArmYVel(Convert.ToInt32(buttonYVelocity.Text));
			}
		}

		#endregion Y Moves


		#region Misc Loader Ops
		private void buttonHomeLoader_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested to Home the Loader", "Action");
			PLC.ReqHomeLoader();
		}

		private void buttonDockEndEffector_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested to Dock the End Effector", "Action");
			PLC.DockEndEffector();
		}

		private void buttonEndEffectorVacuum_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested to Toggle EndEffector Vacuum", "Action");
			PLC.EnableEndEffectorVac(-1);
		}

		#endregion Misc Loader Ops

		#region Other Loader Ops

		private void buttonEnableStackMaint_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, string.Format("Loader Ops: User Toggled Stack Maint Mode to '{0}'", PLC.StackInMaintMode ? "Off" : "On"), "Action");
			PLC.EnableStackMaintMode(-1); 
		}

		private void buttonResetError_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested reset of Stack Error", "Action");
			PLC.ResetStackError();
		}

		private void buttonGoToPositionTeaching_Click(object sender, EventArgs e)
		{
			_frmMain.LoadSubForm(_frmMain.frmPositionTeaching);
		}

		private void buttonReturnToPrev_Click(object sender, EventArgs e)
		{
			if (_frmMain._prevForm != null)
				_frmMain.LoadSubForm(_frmMain._prevForm);
			else
				_frmMain.LoadSubForm(_frmMain.frmProcessStackMaint);
		}
		#endregion Other Loader Ops
	}
}
