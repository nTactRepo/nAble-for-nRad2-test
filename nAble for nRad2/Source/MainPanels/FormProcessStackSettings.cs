using System;
using System.Drawing;
using System.Windows.Forms;
using nTact.DataComm;
using System.Diagnostics;
using nTact.PLC;
using nAble.Enums;

namespace nAble
{
    public partial class FormProcessStackSettings : Form, IUpdateableForm
    {
        private FormMain _frmMain = null;
		private readonly LogEntry _log = null;

		private GalilWrapper2 MC { get { return _frmMain.MC; } }
		private PLCWrapper PLC { get { return _frmMain.PLC; } }
		private Color _controlColor = Color.FromName("Control");
		private bool bMaintEnabled = false;
        private bool bUpdateOneShot = false;

        public FormProcessStackSettings(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain;
			_log = log;

            InitializeComponent();
        }

		private void FormProcessStackSettings_Load(object sender, EventArgs e)
        {
			ConfigMod1Layout();
			ConfigMod2Layout();
			ConfigMod3Layout();
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
				buttonMod1LiftPinsHomeOffset.Text = PLC.Mod1LiftPinHomeOffset.ToString("#0.000");
				buttonMod1LiftPinsLDULDHeight.Text = PLC.Mod1LiftPinLDULDHeight.ToString("#0.000");
				buttonMod1LiftPinsVelocity.Text = PLC.Mod1LiftPinVel.ToString("#0");
                buttonMod1LiftPinsAccDec.Text = PLC.Mod1LiftPinAccDcc.ToString("#0");

				buttonMod2LiftPinsHomeOffset.Text = PLC.Mod2LiftPinHomeOffset.ToString("#0.000");
				buttonMod2LiftPinsLDULDHeight.Text = PLC.Mod2LiftPinLDULDHeight.ToString("#0.000");
				buttonMod2LiftPinsVelocity.Text = PLC.Mod2LiftPinVel.ToString("#0");
				buttonMod2LiftPinsAccDec.Text = PLC.Mod2LiftPinAccDcc.ToString("#0");

				buttonMod3LiftPinsHomeOffset.Text = PLC.Mod3LiftPinHomeOffset.ToString("#0.000");
				buttonMod3LiftPinsLDULDHeight.Text = PLC.Mod3LiftPinLDULDHeight.ToString("#0.000");
				buttonMod3LiftPinsVelocity.Text = PLC.Mod3LiftPinVel.ToString("#0");
				buttonMod3LiftPinsAccDec.Text = PLC.Mod3LiftPinAccDcc.ToString("#0");

				buttonMod1DisableOpenDoorTime.Text = PLC.Mod1DisableDoorOpenTime.ToString("#0.0");
				buttonMod2DisableOpenDoorTime.Text = PLC.Mod2DisableDoorOpenTime.ToString("#0.0");
				buttonMod3DisableOpenDoorTime.Text = PLC.Mod3DisableDoorOpenTime.ToString("#0.0");

				buttonZLoadVelFast.Text = PLC.LoaderZLoadVelFast.ToString("#0");
                buttonZLoadVelSlow.Text = PLC.LoaderZLoadVelSlow.ToString("#0");
                buttonZUnloadVelFast.Text = PLC.LoaderZUnloadVelFast.ToString("#0");
                buttonZUnloadVelSlow.Text = PLC.LoaderZUnloadVelSlow.ToString("#0");
                buttonYVelNoSubstrate.Text = PLC.LoaderYVelNoSubstrate.ToString("#0");
                buttonYVelSubstrate.Text = PLC.LoaderYVelSubstrate.ToString("#0");
                bUpdateOneShot = false;
            }

			bool bConnected = (MC != null && MC.Connected && MC.IsHomed);
            bool bMoving = MC.Moving;
			bMaintEnabled = PLC.StackInMaintMode;
			bool bMod1Online = PLC.Mod1CommEnabled;
			bool bMod2Online = PLC.Mod2CommEnabled;
			bool bMod3Online = PLC.Mod3CommEnabled;
			if (_frmMain.MS.StackMod1Installed)
			{
				buttonMod1Power.BackColor = PLC.Mod1EnablePower ? Color.LimeGreen : Color.Red;
				buttonMod1Power.Text = PLC.Mod1EnablePower ? $"{_frmMain.MS.StackMod1Name}\r Power On" : $"{_frmMain.MS.StackMod1Name}\rPower Off";
				buttonMod1OnlineState.BackColor = bMod1Online ? Color.LimeGreen : Color.Red;
				buttonMod1OnlineState.Text = bMod1Online ? $"{_frmMain.MS.StackMod1Name}\r Online" : $"{_frmMain.MS.StackMod1Name}\rOffline";
			}
			else
			{
				buttonMod1Power.BackColor = Color.DarkGray;
				buttonMod1Power.Text = "Not Installed";
				buttonMod1OnlineState.BackColor = Color.DarkGray;
				buttonMod1OnlineState.Text = "Not Installed";
			}
			if (_frmMain.MS.StackMod2Installed)
			{
				buttonMod2Power.BackColor = PLC.Mod2EnablePower ? Color.LimeGreen : Color.Red;
				buttonMod2Power.Text = PLC.Mod2EnablePower ? $"{_frmMain.MS.StackMod2Name}\r Power On" : $"{_frmMain.MS.StackMod2Name}\rPower Off";
				buttonMod2OnlineState.BackColor = bMod2Online ? Color.LimeGreen : Color.Red;
				buttonMod2OnlineState.Text = bMod2Online ? $"{_frmMain.MS.StackMod2Name}\rEnabled" : $"{_frmMain.MS.StackMod2Name}\rDisabled";
			}
			else
			{
				buttonMod2Power.BackColor = Color.DarkGray;
				buttonMod2Power.Text = "Not Installed";
				buttonMod2OnlineState.BackColor = Color.DarkGray;
				buttonMod2OnlineState.Text = "Not Installed";
			}
			if (_frmMain.MS.StackMod3Installed)
			{
				buttonMod3Power.BackColor = PLC.Mod3EnablePower ? Color.LimeGreen : Color.Red;
				buttonMod3Power.Text = PLC.Mod3EnablePower ? $"{_frmMain.MS.StackMod3Name}\r Power On" : $"{_frmMain.MS.StackMod3Name}\rPower Off";
				buttonMod3OnlineState.BackColor = bMod3Online ? Color.LimeGreen : Color.Red;
				buttonMod3OnlineState.Text = bMod3Online ? $"{_frmMain.MS.StackMod3Name}\rEnabled" : $"{_frmMain.MS.StackMod3Name}\rDisabled";
			}
			else
			{
				buttonMod3Power.BackColor = Color.DarkGray;
				buttonMod3Power.Text = "Not Installed";
				buttonMod3OnlineState.BackColor = Color.DarkGray;
				buttonMod3OnlineState.Text = "Not Installed";
			}
		}
		private void buttonGoToStack_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmProcessStack);
        }
		private void buttonGoToStackMaint_Click(object sender, EventArgs e)
        {
            if (_frmMain._prevForm != null)
                _frmMain.LoadSubForm(_frmMain._prevForm);
            else
                _frmMain.LoadSubForm(_frmMain.frmProcessStackMaint);
        }

		#region Module 1

		private void ConfigMod1Layout()
		{
			groupBoxModule1.Enabled = _frmMain.MS.StackMod1Installed;
			bool bShowDoorStuff = NAbleEnums.ModuleHasVac((NAbleEnums.StackModuleType)_frmMain.MS.StackMod1Type);
			labelMod1DisableOpenDoorTime.Visible = bShowDoorStuff;
			buttonMod1DisableOpenDoorTime.Visible = bShowDoorStuff;
		}
		private void buttonMod1LiftPinsHomeOffset_Click(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			_frmMain.GotoNumScreen("Module 1 Lift Pins Home Offset (0-5)", this, buttonSender, "#0.000", 0, 5);
		}

		private void buttonMod1LiftPinsHomeOffset_TextChanged(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			if (!bUpdateOneShot)
			{
				PLC.Mod1LiftPinHomeOffset = Convert.ToSingle(buttonSender.Text); // PLC.SetDevice("W30", (int)nValue);
			}
		}


		private void buttonMod1LiftPinsLDULDHeight_Click(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            _frmMain.GotoNumScreen("Module 1 Lift Pins LD/ULD Height (1-25)", this, buttonSender, "#0.000", 1, 25);
        }

        private void buttonMod1LiftPinsLDULDHeight_TextChanged(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (!bUpdateOneShot)
            {
				PLC.Mod1LiftPinLDULDHeight = Convert.ToSingle(buttonSender.Text);
			}
        }

        private void buttonMod1LiftPinsVelocity_Click(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            _frmMain.GotoNumScreen("Module 1 Lift Pins Velocity (1-100)", this, buttonSender, "#0", 1, 100,"",false,false);
        }

        private void buttonMod1LiftPinsVelocity_TextChanged(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (!bUpdateOneShot)
            {
				PLC.Mod1LiftPinVel = Convert.ToInt32(buttonSender.Text);
            }
        }

        private void buttonMod1LiftPinsAccDec_Click(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            _frmMain.GotoNumScreen("Module 1 Lift Pins Acc/Dec (1-98)", this, buttonSender, "#0", 1, 98, "", false, false);
        }

        private void buttonLiftPinsAccDec_TextChanged(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (!bUpdateOneShot)
            {
				PLC.Mod1LiftPinAccDcc = Convert.ToInt32(buttonSender.Text);
			}
        }

		private void buttonMod1DisableOpenDoorTime_Click(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			_frmMain.GotoNumScreen($"Disable {_frmMain.MS.StackMod1Name} Open Door Time (1.0-60.0)", this, buttonSender, "#0.0", 1, 60);
		}
		private void buttonMod1DisableOpenDoorTime_TextChanged(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			if (!bUpdateOneShot)
			{
				PLC.Mod1DisableDoorOpenTime = Convert.ToSingle(buttonSender.Text);
			}
		}

		private void buttonMod1OnlineState_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, string.Format("User Toggled Mod 1 Online state to {0}", PLC.Mod1CommEnabled ? "Off" : "On"), "Action");
			PLC.Mod1CommEnabled = !PLC.Mod1CommEnabled;
        }
		private void buttonMod1Power_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, string.Format("User Toggled Mod 1 Power state to {0}", PLC.Mod1EnablePower ? "Off" : "On"), "Action");
			PLC.Mod1EnablePower = !PLC.Mod1EnablePower;
		}


		#endregion Module 1

		#region Module 2
		private void ConfigMod2Layout()
		{
			groupBoxModule2.Enabled = _frmMain.MS.StackMod2Installed;
			bool bShowDoorStuff = NAbleEnums.ModuleHasVac((NAbleEnums.StackModuleType)_frmMain.MS.StackMod2Type);
			labelMod2DisableOpenDoorTime.Visible = bShowDoorStuff;
			buttonMod2DisableOpenDoorTime.Visible = bShowDoorStuff;
		}

		private void buttonMod2LiftPinsLDULDHeight_Click(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			_frmMain.GotoNumScreen("Module 2 Lift Pins LD/ULD Height (1-25)", this, buttonSender, "#0.000", 1, 25);
		}

		private void buttonMod2LiftPinsLDULDHeight_TextChanged(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			if (!bUpdateOneShot)
			{
				PLC.Mod2LiftPinLDULDHeight = Convert.ToSingle(buttonSender.Text);
			}
		}

		private void buttonMod2LiftPinsVelocity_Click(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			_frmMain.GotoNumScreen("Module 2 Lift Pins Velocity (1-100)", this, buttonSender, "#0", 1, 100, "", false, false);
		}

		private void buttonMod2LiftPinsVelocity_TextChanged(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			if (!bUpdateOneShot)
			{
				PLC.Mod2LiftPinVel = Convert.ToInt32(buttonSender.Text);
			}
		}

		private void buttonMod2LiftPinsAccDec_Click(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			_frmMain.GotoNumScreen("Module 2 Lift Pins Acc/Dec (1-98)", this, buttonSender, "#0", 1, 98, "", false, false);
		}

		private void buttonMod2LiftPinsAccDec_TextChanged(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			if (!bUpdateOneShot)
			{
				PLC.Mod2LiftPinAccDcc = Convert.ToInt32(buttonSender.Text);
			}
		}

		private void buttonMod2LiftPinsHomeOffset_Click(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			_frmMain.GotoNumScreen("Module 2 Lift Pins Home Offset (0-5)", this, buttonSender, "#0.000", 0, 5);
		}

		private void buttonMod2LiftPinsHomeOffset_TextChanged(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			if (!bUpdateOneShot)
			{
				PLC.Mod2LiftPinHomeOffset = Convert.ToSingle(buttonSender.Text); // PLC.SetDevice("W30", (int)nValue);
			}
		}

		private void buttonMod2DisableOpenDoorTime_Click(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			_frmMain.GotoNumScreen($"Disable {_frmMain.MS.StackMod2Name} Open Door Time (1.0-60.0)", this, buttonSender, "#0.0", 1, 60);
		}
		private void buttonMod2DisableOpenDoorTime_TextChanged(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			if (!bUpdateOneShot)
			{
				PLC.Mod2DisableDoorOpenTime = Convert.ToSingle(buttonSender.Text);
			}
		}

		private void buttonMod2OnlineState_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, string.Format("User Toggled Mod 2 Online state to {0}", PLC.Mod2CommEnabled?"Off":"On"),"Action");
			PLC.Mod2CommEnabled = !PLC.Mod2CommEnabled;
        }

		private void buttonMod2Power_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, string.Format("User Toggled Mod 2 Power state to {0}", PLC.Mod2EnablePower ? "Off" : "On"), "Action");
			PLC.Mod2EnablePower = !PLC.Mod2EnablePower;
		}

		#endregion Module 2

		#region Module 3
		private void ConfigMod3Layout()
		{
			groupBoxModule3.Enabled = _frmMain.MS.StackMod3Installed;
			bool bShowDoorStuff = NAbleEnums.ModuleHasVac((NAbleEnums.StackModuleType)_frmMain.MS.StackMod3Type);
			labelMod3DisableOpenDoorTime.Visible = bShowDoorStuff;
			buttonMod3DisableOpenDoorTime.Visible = bShowDoorStuff;
		}

		private void buttonMod3LiftPinsLDULDHeight_Click(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			_frmMain.GotoNumScreen("Module 3 Lift Pins LD/ULD Height (1-25)", this, buttonSender, "#0.000", 1, 25);
		}

		private void buttonMod3LiftPinsLDULDHeight_TextChanged(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			if (!bUpdateOneShot)
			{
				PLC.Mod3LiftPinLDULDHeight = Convert.ToSingle(buttonSender.Text);
			}
		}

		private void buttonMod3LiftPinsVelocity_Click(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			_frmMain.GotoNumScreen("Module 3 Lift Pins Velocity (1-100)", this, buttonSender, "#0", 1, 100, "", false, false);
		}

		private void buttonMod3LiftPinsVelocity_TextChanged(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			if (!bUpdateOneShot)
			{
				PLC.Mod3LiftPinVel = Convert.ToInt32(buttonSender.Text);
			}
		}

		private void buttonMod3LiftPinsAccDec_Click(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			_frmMain.GotoNumScreen("Module 3 Lift Pins Acc/Dec (1-98)", this, buttonSender, "#0", 1, 98, "", false, false);
		}
		private void buttonMod3LiftPinsAccDec_TextChanged(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			if (!bUpdateOneShot)
			{
				PLC.Mod3LiftPinAccDcc = Convert.ToInt32(buttonSender.Text);
			}
		}

		private void buttonMod3LiftPinsHomeOffset_Click(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			_frmMain.GotoNumScreen("Module 3 Lift Pins Home Offset (0-5)", this, buttonSender, "#0.000", 0, 5);
		}

		private void buttonMod3LiftPinsHomeOffset_TextChanged(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			if (!bUpdateOneShot)
			{
				PLC.Mod3LiftPinHomeOffset = Convert.ToSingle(buttonSender.Text); // PLC.SetDevice("W30", (int)nValue);
			}
		}

		private void buttonMod3DisableOpenDoorTime_Click(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			_frmMain.GotoNumScreen($"Disable {_frmMain.MS.StackMod3Name} Open Door Time (1.0-60.0)", this, buttonSender, "#0.0", 1, 60);
		}
		private void buttonMod3DisableOpenDoorTime_TextChanged(object sender, EventArgs e)
		{
			Button buttonSender = (Button)sender;
			if (!bUpdateOneShot)
			{
				PLC.Mod3DisableDoorOpenTime = Convert.ToSingle(buttonSender.Text);
			}
		}

		private void buttonMod3OnlineState_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, string.Format("User Toggled Mod 3 Online state to {0}", PLC.Mod3CommEnabled ? "Off" : "On"), "Action");
			PLC.Mod3CommEnabled = !PLC.Mod3CommEnabled;
		}
		#endregion Module 3

		#region Loader Settings

		private void buttonZLoadVelFast_Click(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            _frmMain.GotoNumScreen("Load Z Velocity Fast (1-100)", this, buttonSender, "#0", 1, 100,"",false,false);
        }

        private void buttonZLoadVelFast_TextChanged(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (!bUpdateOneShot)
            {
                //PLC.SetDevice("D400", Convert.ToInt32(buttonSender.Text));
				PLC.LoaderZLoadVelFast = Convert.ToInt32(buttonSender.Text);
			}
        }

        private void buttonZLoadVelSlow_Click(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            _frmMain.GotoNumScreen("Load Z Velocity Slow (1-100)", this, buttonSender, "#0", 1, 100, "", false, false);
        }

        private void buttonZLoadVelSlow_TextChanged(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (!bUpdateOneShot)
            {
                //PLC.SetDevice("D401", Convert.ToInt32(buttonSender.Text));
				PLC.LoaderZLoadVelSlow = Convert.ToInt32(buttonSender.Text);
			}
		}

        private void buttonZUnloadVelFast_Click(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            _frmMain.GotoNumScreen("Unload Z Velocity Fast (1-100)", this, buttonSender, "#0", 1, 100, "", false, false);
        }

        private void buttonZUnloadVelFast_TextChanged(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (!bUpdateOneShot)
            {
                //PLC.SetDevice("D402", Convert.ToInt32(buttonSender.Text));
				PLC.LoaderZUnloadVelFast = Convert.ToInt32(buttonSender.Text);
			}
        }

        private void buttonZUnloadVelSlow_Click(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            _frmMain.GotoNumScreen("Unload Z Velocity Slow (1-100)", this, buttonSender, "#0", 1, 100, "", false, false);
        }

        private void buttonZUnloadVelSlow_TextChanged(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (!bUpdateOneShot)
            {
                //PLC.SetDevice("D403", Convert.ToInt32(buttonSender.Text));
				PLC.LoaderZUnloadVelSlow = Convert.ToInt32(buttonSender.Text);
			}
		}

        private void buttonYVelNoSubstrate_Click(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            _frmMain.GotoNumScreen("Y Velocity w/o Substrate (1-100)", this, buttonSender, "#0", 1, 100, "", false, false);
        }

        private void buttonYVelNoSubstrate_TextChanged(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (!bUpdateOneShot)
            {
                //PLC.SetDevice("D404", Convert.ToInt32(buttonSender.Text));
				PLC.LoaderYVelNoSubstrate = Convert.ToInt32(buttonSender.Text);
			}
		}

        private void buttonYVelSubstrate_Click(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            _frmMain.GotoNumScreen("Y Velocity w/ Substrate (1-100)", this, buttonSender, "#0", 1, 100, "", false, false);
        }

        private void buttonYVelSubstrate_TextChanged(object sender, EventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (!bUpdateOneShot)
            {
                //PLC.SetDevice("D405", Convert.ToInt32(buttonSender.Text));
				PLC.LoaderYVelSubstrate = Convert.ToInt32(buttonSender.Text);
			}
		}

		#endregion Loader Settings



		private void buttonMod3Power_Click(object sender, EventArgs e)
		{

		}
	}
}
