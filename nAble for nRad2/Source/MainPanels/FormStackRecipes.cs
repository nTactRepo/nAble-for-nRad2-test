using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nTact.DataComm;
using nTact.PLC;
using System.Diagnostics;
using nAble.Enums;

namespace nAble
{
    public partial class FormStackRecipes : Form, IUpdateableForm
    {
        private FormMain _frmMain = null;
		private readonly LogEntry _log = null;

		private GalilWrapper2 MCPU { get { return _frmMain.MC; } }
        private PLCWrapper PLC { get { return _frmMain.PLC; } }
        private Color _controlColor = Color.FromName("Control");
        private bool bOneShot = false;
        private bool bRecipeNoChg = false;

        public FormStackRecipes(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain;
			_log = log;

            InitializeComponent();
			SetupTabPages();
		}

        private void FormStackRecipes_Load(object sender, EventArgs e)
        {
		}

		private void SetupTabPages()
		{
			ConfigMod1Layout();
			ConfigMod2Layout();
			ConfigMod3Layout();
			//TODO Uncomment to use Advanced Vacuum when ready
            //buttonVacuumProfile.Visible = _frmMain.MS.StackIncludesAdvancedVac;
		}

		private void ConfigMod1Layout()
		{
			if (_frmMain.MS.StackMod1Installed)
			{
				NAbleEnums.StackModuleType modType = (NAbleEnums.StackModuleType)_frmMain.MS.StackMod1Type;
				tabPageModule1.Text = $"Mod 1 {NAbleEnums.ModuleTypeName(modType, true)}";
				switch (modType)
				{
					case NAbleEnums.StackModuleType.Hotplate:
					{
						groupBoxMod1HP.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod1Name}";
						groupBoxMod1HP.Visible = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuBake:
					case NAbleEnums.StackModuleType.VacuBakePlus:
					{
						groupBoxMod1VB.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod1Name}";
						groupBoxMod1VB.Visible = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuDry:
					case NAbleEnums.StackModuleType.VacuDryPlus:
					{
						groupBoxMod1VD.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod1Name}";
						groupBoxMod1VD.Visible = true;
					}
					break;
					case NAbleEnums.StackModuleType.ChillPlate:
					{
					}
					break;
				}
			}
			else
			{
				tabControlModules.TabPages.Remove(tabPageModule1);
			}

		}

		private void ConfigMod2Layout()
		{
			if (_frmMain.MS.StackMod2Installed)
			{
				NAbleEnums.StackModuleType modType = (NAbleEnums.StackModuleType)_frmMain.MS.StackMod2Type;
				tabPageModule2.Text = $"Mod 2 {NAbleEnums.ModuleTypeName(modType, true)}";
				switch (modType)
				{
					case NAbleEnums.StackModuleType.Hotplate:
					{
						groupBoxMod2HP.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod2Name}";
						groupBoxMod2HP.Visible = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuBake:
					case NAbleEnums.StackModuleType.VacuBakePlus:
					{
						groupBoxMod2VB.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod2Name}";
						groupBoxMod2VB.Visible = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuDry:
					case NAbleEnums.StackModuleType.VacuDryPlus:
					{
						groupBoxMod2VD.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod2Name}";
						groupBoxMod2VD.Visible = true;
					}
					break;
					case NAbleEnums.StackModuleType.ChillPlate:
					{
					}
					break;
				}
			}
			else
			{
				tabControlModules.TabPages.Remove(tabPageModule2);
			}
		}

		private void ConfigMod3Layout()
		{
			if (_frmMain.MS.StackMod3Installed)
			{
				NAbleEnums.StackModuleType modType = (NAbleEnums.StackModuleType)_frmMain.MS.StackMod3Type;
				tabPageModule3.Text = $"Mod 3 {NAbleEnums.ModuleTypeName(modType,true)}";
				switch (modType)
				{
					case NAbleEnums.StackModuleType.Hotplate:
					{
						groupBoxMod3HP.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod3Name}";
						groupBoxMod3HP.Visible = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuBake:
					case NAbleEnums.StackModuleType.VacuBakePlus:
					{
						groupBoxMod3VB.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod3Name}";
						groupBoxMod3VB.Visible = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuDry:
					case NAbleEnums.StackModuleType.VacuDryPlus:
					{
						groupBoxMod3VD.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod3Name}";
						groupBoxMod3VD.Visible = true;
					}
					break;
					case NAbleEnums.StackModuleType.ChillPlate:
					{
					}
					break;
				}
			}
			else
			{
				tabControlModules.TabPages.Remove(tabPageModule3);
			}
		}

		public void UpdateStatus()
        {

			try
			{
				PLC.ReadRecipes = Visible;
				if (_frmMain == null || !Visible)
				{
					bOneShot = true;
					return;
				}
				if (bOneShot)
				{
					UpdateRecipe();
					bOneShot = false;
				}

				labelRecipeNo.Text = PLC.EditRecipeNumber.ToString("#0");
				labelMod1ChgReqActive.Visible = PLC.Mod1ChgReqActive;
				labelMod2ChgReqActive.Visible = PLC.Mod2ChgReqActive;
				labelMod3ChgReqActive.Visible = PLC.Mod3ChgReqActive;
				buttonSaveRecipe.BackColor = PLC.RecipeHasChanged ? Color.Magenta : SystemColors.Control;

			}
			catch (Exception ex)
			{
				Debug.WriteLine($"FormStackRecipes:: Exception during UpdateStatus - {ex.Message}", "ERROR");
			}
        }

        private void buttonReturnToPrev_Click(object sender, EventArgs e)
        {
            if (_frmMain._prevForm != null)
                _frmMain.LoadSubForm(_frmMain._prevForm);
            else
                _frmMain.LoadSubForm(_frmMain.frmProcessStack);
        }

        private void buttonNextRecipe_Click(object sender, EventArgs e)
        {
			if (PLC.EditRecipeNumber < 10)
				PLC.EditRecipeNumber += 1;
			else
				PLC.EditRecipeNumber = 1;
			PLC.SelectedRecipeChanged = true;

        }

        private void buttonPrevRecipe_Click(object sender, EventArgs e)
        {
			if (PLC.EditRecipeNumber > 1)
				PLC.EditRecipeNumber -= 1;
			else
				PLC.EditRecipeNumber = 10;
			PLC.SelectedRecipeChanged = true;
		}

		private void buttonDownloadRecipe_Click(object sender, EventArgs e)
        {
			PLC.DownloadRecipe = true;
        }

        private void buttonSaveRecipe_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"User Saved Recipe Recipe [{labelRecipeNo.Text}] - '{buttonRecipeName.Text}'", "Action");
			PLC.SaveRecipe = true;
        }

        private void buttonRecipeName_Click(object sender, EventArgs e)
        {
            _frmMain.GotoTextScreen("Process Stack Recipe Name", this, buttonRecipeName,20);
        }

		private void buttonEditRecipeName_Click(object sender, EventArgs e)
		{
			_frmMain.GotoTextScreen("Process Stack Recipe Name", this, buttonRecipeName, 20);
		}

		private void labelRecipeNo_TextChanged(object sender, EventArgs e)
        {
            bRecipeNoChg = true;
            UpdateRecipe();
            bRecipeNoChg = false;
        }

        private void ParameterChanged(object sender, EventArgs e)
        {
            if (bRecipeNoChg || bOneShot)
                return;
            Button buttonSender = (Button)sender;
            char delimiter = ',';
            string[] paramData = (buttonSender.Tag.ToString()).Split(delimiter);
            if (paramData.Count() == 1)
                PLC.SetRecipeParam(paramData[0], int.Parse(buttonSender.Text));
            else
            {
                double dValue = double.Parse(buttonSender.Text);
                double dMult = double.Parse(paramData[1]);
                int nData = (int)(dValue * dMult);
                PLC.SetRecipeParam(paramData[0], nData);
            }
        }
		private void NewParameterChanged(object sender, EventArgs e)
		{
			if (bRecipeNoChg || bOneShot)
				return;
			Button buttonSender = (Button)sender;
			PLC.SetRecipeParam(buttonSender.Tag.ToString(), double.Parse(buttonSender.Text));
		}
		private void TimeParameterChanged(object sender, EventArgs e)
		{
			if (bRecipeNoChg || bOneShot)
				return;
			Button buttonSender = (Button)sender;
			PLC.SetRecipeParam(buttonSender.Tag.ToString(), TimeSpan.Parse(buttonSender.Text));
		}

		private void UpdateRecipe()
        {
            //UPdate Module 1 Recipe Info
            buttonRecipeName.Text = PLC.EditRecipeName;
			//Temps	(HP/VB/(CP??))
			buttonMod1HPTemp.Text = PLC.EditRecipeMod1Temp.ToString("#0");
			buttonMod1VBTemp.Text = PLC.EditRecipeMod1Temp.ToString("#0");
			// Temp Offsets 
			buttonMod1HPTempOffset.Text = PLC.EditRecipeMod1TempOffset.ToString("#0");
			buttonMod1VBTempOffset.Text = PLC.EditRecipeMod1TempOffset.ToString("#0");
			// Proximity Bake Height (HP/VB)
			buttonMod1HPProxBakeHt.Text = (PLC.EditRecipeMod1ProxBakeHt / 1000.000).ToString("#0.0");
			buttonMod1VBProxBakeHt.Text = (PLC.EditRecipeMod1ProxBakeHt / 1000.000).ToString("#0.0");
			// Proximity Bake Time (HP/VB)
			buttonMod1HPProxBakeHrs.Text = PLC.EditRecipeMod1ProxBakeTimeHrs.ToString("00");
			buttonMod1VBProxBakeHrs.Text = PLC.EditRecipeMod1ProxBakeTimeHrs.ToString("00");
			buttonMod1HPProxBakeMins.Text = PLC.EditRecipeMod1ProxBakeTimeMins.ToString("00");
			buttonMod1VBProxBakeMins.Text = PLC.EditRecipeMod1ProxBakeTimeMins.ToString("00");
            buttonMod1HPProxBakeSecs.Text = PLC.EditRecipeMod1ProxBakeTimeSecs.ToString("00");
			buttonMod1VBProxBakeSecs.Text = PLC.EditRecipeMod1ProxBakeTimeSecs.ToString("00");
			// Contact Bake Time (HP/VB)
			buttonMod1HPContBakeHrs.Text =  PLC.EditRecipeMod1ContBakeTimeHrs.ToString("00");
			buttonMod1VBContBakeHrs.Text =  PLC.EditRecipeMod1ContBakeTimeHrs.ToString("00");
			buttonMod1HPContBakeMins.Text = PLC.EditRecipeMod1ContBakeTimeMins.ToString("00");
			buttonMod1VBContBakeMins.Text = PLC.EditRecipeMod1ContBakeTimeMins.ToString("00");
            buttonMod1HPContBakeSecs.Text = PLC.EditRecipeMod1ContBakeTimeSecs.ToString("00");
			buttonMod1VBContBakeSecs.Text = PLC.EditRecipeMod1ContBakeTimeSecs.ToString("00");
			// Vacuum (bake) Time (HP/VB)
			buttonMod1VBVacBakeHrs.Text =   PLC.EditRecipeMod1VacTimeHrs.ToString("00");
			buttonMod1VDTimeHrs.Text =      PLC.EditRecipeMod1VacTimeHrs.ToString("00");
			buttonMod1VBVacBakeMins.Text =  PLC.EditRecipeMod1VacTimeMins.ToString("00");
			buttonMod1VDTimeMins.Text =     PLC.EditRecipeMod1VacTimeMins.ToString("00");
			buttonMod1VBVacBakeSecs.Text =  PLC.EditRecipeMod1VacTimeSecs.ToString("00");
			buttonMod1VDTimeSecs.Text =     PLC.EditRecipeMod1VacTimeSecs.ToString("00");
			// Cooldown/Chill time (HP/VB/CP)
			buttonMod1HPCoolTimeHrs.Text =  PLC.EditRecipeMod1CoolTimeHrs.ToString("00");
			buttonMod1VBCoolTimeHrs.Text =  PLC.EditRecipeMod1CoolTimeHrs.ToString("00");
			buttonMod1HPCoolTimeMins.Text = PLC.EditRecipeMod1CoolTimeMins.ToString("00");
			buttonMod1VBCoolTimeMins.Text = PLC.EditRecipeMod1CoolTimeMins.ToString("00");
			buttonMod1HPCoolTimeSecs.Text = PLC.EditRecipeMod1CoolTimeSecs.ToString("00");
			buttonMod1VBCoolTimeSecs.Text = PLC.EditRecipeMod1CoolTimeSecs.ToString("00");

            //Update Module 2 Recipe Info
			//Temps	(HP/VB/(CP??))
			buttonMod2HPTemp.Text = PLC.EditRecipeMod2Temp.ToString("#0");
			buttonMod2VBTemp.Text = PLC.EditRecipeMod2Temp.ToString("#0");
			// Temp Offsets 
			buttonMod2HPTempOffset.Text = PLC.EditRecipeMod2TempOffset.ToString("#0");
			buttonMod2VBTempOffset.Text = PLC.EditRecipeMod2TempOffset.ToString("#0");
			// Proximity Bake Height (HP/VB)
			buttonMod2HPProxBakeHt.Text = (PLC.EditRecipeMod2ProxBakeHt / 1000.000).ToString("#0.0");
			buttonMod2VBProxBakeHt.Text = (PLC.EditRecipeMod2ProxBakeHt / 1000.000).ToString("#0.0");
			// Proximity Bake Time (HP/VB)
			buttonMod2HPProxBakeHrs.Text = PLC.EditRecipeMod2ProxBakeTimeHrs.ToString("00");
			buttonMod2VBProxBakeHrs.Text = PLC.EditRecipeMod2ProxBakeTimeHrs.ToString("00");
			buttonMod2HPProxBakeMins.Text = PLC.EditRecipeMod2ProxBakeTimeMins.ToString("00");
			buttonMod2VBProxBakeMins.Text = PLC.EditRecipeMod2ProxBakeTimeMins.ToString("00");
			buttonMod2HPProxBakeSecs.Text = PLC.EditRecipeMod2ProxBakeTimeSecs.ToString("00");
			buttonMod2VBProxBakeSecs.Text = PLC.EditRecipeMod2ProxBakeTimeSecs.ToString("00");
			// Contact Bake Time (HP/VB)
			buttonMod2HPContBakeHrs.Text = PLC.EditRecipeMod2ContBakeTimeHrs.ToString("00");
			buttonMod2VBContBakeHrs.Text = PLC.EditRecipeMod2ContBakeTimeHrs.ToString("00");
			buttonMod2HPContBakeMins.Text = PLC.EditRecipeMod2ContBakeTimeMins.ToString("00");
			buttonMod2VBContBakeMins.Text = PLC.EditRecipeMod2ContBakeTimeMins.ToString("00");
			buttonMod2HPContBakeSecs.Text = PLC.EditRecipeMod2ContBakeTimeSecs.ToString("00");
			buttonMod2VBContBakeSecs.Text = PLC.EditRecipeMod2ContBakeTimeSecs.ToString("00");
			// Vacuum (bake) Time (HP/VB)
			buttonMod2VBVacBakeHrs.Text = PLC.EditRecipeMod2VacTimeHrs.ToString("00");
			buttonMod2VDTimeHrs.Text = PLC.EditRecipeMod2VacTimeHrs.ToString("00");
			buttonMod2VBVacBakeMins.Text = PLC.EditRecipeMod2VacTimeMins.ToString("00");
			buttonMod2VDTimeMins.Text = PLC.EditRecipeMod2VacTimeMins.ToString("00");
			buttonMod2VBVacBakeSecs.Text = PLC.EditRecipeMod2VacTimeSecs.ToString("00");
			buttonMod2VDTimeSecs.Text = PLC.EditRecipeMod2VacTimeSecs.ToString("00");
			// Cooldown/Chill time (HP/VB/CP)
			buttonMod2HPCoolTimeHrs.Text = PLC.EditRecipeMod2CoolTimeHrs.ToString("00");
			buttonMod2VBCoolTimeHrs.Text = PLC.EditRecipeMod2CoolTimeHrs.ToString("00");
			buttonMod2HPCoolTimeMins.Text = PLC.EditRecipeMod2CoolTimeMins.ToString("00");
			buttonMod2VBCoolTimeMins.Text = PLC.EditRecipeMod2CoolTimeMins.ToString("00");
			buttonMod2HPCoolTimeSecs.Text = PLC.EditRecipeMod2CoolTimeSecs.ToString("00");
			buttonMod2VBCoolTimeSecs.Text = PLC.EditRecipeMod2CoolTimeSecs.ToString("00");

			//Temps	(HP/VB/(CP??))
			buttonMod3HPTemp.Text = PLC.EditRecipeMod3Temp.ToString("#0");
			buttonMod3VBTemp.Text = PLC.EditRecipeMod3Temp.ToString("#0");
			// Temp Offsets 
			buttonMod3HPTempOffset.Text = PLC.EditRecipeMod3TempOffset.ToString("#0");
			buttonMod3VBTempOffset.Text = PLC.EditRecipeMod3TempOffset.ToString("#0");
			// Proximity Bake Height (HP/VB)
			buttonMod3HPProxBakeHt.Text = (PLC.EditRecipeMod3ProxBakeHt / 1000.000).ToString("#0.0");
			buttonMod3VBProxBakeHt.Text = (PLC.EditRecipeMod3ProxBakeHt / 1000.000).ToString("#0.0");
			// Proximity Bake Time (HP/VB)
			buttonMod3HPProxBakeHrs.Text = PLC.EditRecipeMod3ProxBakeTimeHrs.ToString("00");
			buttonMod3VBProxBakeHrs.Text = PLC.EditRecipeMod3ProxBakeTimeHrs.ToString("00");
			buttonMod3HPProxBakeMins.Text = PLC.EditRecipeMod3ProxBakeTimeMins.ToString("00");
			buttonMod3VBProxBakeMins.Text = PLC.EditRecipeMod3ProxBakeTimeMins.ToString("00");
			buttonMod3HPProxBakeSecs.Text = PLC.EditRecipeMod3ProxBakeTimeSecs.ToString("00");
			buttonMod3VBProxBakeSecs.Text = PLC.EditRecipeMod3ProxBakeTimeSecs.ToString("00");
			// Contact Bake Time (HP/VB)
			buttonMod3HPContBakeHrs.Text = PLC.EditRecipeMod3ContBakeTimeHrs.ToString("00");
			buttonMod3VBContBakeHrs.Text = PLC.EditRecipeMod3ContBakeTimeHrs.ToString("00");
			buttonMod3HPContBakeMins.Text = PLC.EditRecipeMod3ContBakeTimeMins.ToString("00");
			buttonMod3VBContBakeMins.Text = PLC.EditRecipeMod3ContBakeTimeMins.ToString("00");
			buttonMod3HPContBakeSecs.Text = PLC.EditRecipeMod3ContBakeTimeSecs.ToString("00");
			buttonMod3VBContBakeSecs.Text = PLC.EditRecipeMod3ContBakeTimeSecs.ToString("00");
			// Vacuum (bake) Time (HP/VB)
			buttonMod3VBVacBakeHrs.Text = PLC.EditRecipeMod3VacTimeHrs.ToString("00");
			buttonMod3VDTimeHrs.Text = PLC.EditRecipeMod3VacTimeHrs.ToString("00");
			buttonMod3VBVacBakeMins.Text = PLC.EditRecipeMod3VacTimeMins.ToString("00");
			buttonMod3VDTimeMins.Text = PLC.EditRecipeMod3VacTimeMins.ToString("00");
			buttonMod3VBVacBakeSecs.Text = PLC.EditRecipeMod3VacTimeSecs.ToString("00");
			buttonMod3VDTimeSecs.Text = PLC.EditRecipeMod3VacTimeSecs.ToString("00");
			// Cooldown/Chill time (HP/VB/CP)
			buttonMod3HPCoolTimeHrs.Text = PLC.EditRecipeMod3CoolTimeHrs.ToString("00");
			buttonMod3VBCoolTimeHrs.Text = PLC.EditRecipeMod3CoolTimeHrs.ToString("00");
			buttonMod3HPCoolTimeMins.Text = PLC.EditRecipeMod3CoolTimeMins.ToString("00");
			buttonMod3VBCoolTimeMins.Text = PLC.EditRecipeMod3CoolTimeMins.ToString("00");
			buttonMod3HPCoolTimeSecs.Text = PLC.EditRecipeMod3CoolTimeSecs.ToString("00");
			buttonMod3VBCoolTimeSecs.Text = PLC.EditRecipeMod3CoolTimeSecs.ToString("00");

			// Load Advanced Vacuum profile if included.
			if (_frmMain.MS.StackIncludesAdvancedVac)
			{
				buttonStep1ValveAngle.Text = PLC.EditRecipeStep1ValveAngle.ToString("0.0");
				buttonStep1PresSetpoint.Text = PLC.EditRecipeStep1PresSetpoint.ToString("0.0");
				buttonStep1Time.Text = PLC.EditRecipeStep1Time.ToString("c");
				buttonStep2ValveAngle.Text = PLC.EditRecipeStep2ValveAngle.ToString("0.0");
				buttonStep2PresSetpoint.Text = PLC.EditRecipeStep2PresSetpoint.ToString("0.0");
				buttonStep2Time.Text = PLC.EditRecipeStep2Time.ToString("c");
				buttonStep3ValveAngle.Text = PLC.EditRecipeStep3ValveAngle.ToString("0.0");
				buttonStep3PresSetpoint.Text = PLC.EditRecipeStep3PresSetpoint.ToString("0.0");
				buttonStep3Time.Text = PLC.EditRecipeStep3Time.ToString("c");
				buttonStep4ValveAngle.Text = PLC.EditRecipeStep4ValveAngle.ToString("0.0");
				buttonStep4PresSetpoint.Text = PLC.EditRecipeStep4PresSetpoint.ToString("0.0");
				buttonStep4Time.Text = PLC.EditRecipeStep4Time.ToString("c");
				buttonStepDwellHoldValveAngle.Text = PLC.EditRecipeStepDwellHoldValveAngle.ToString("0.0");
				buttonStepDwellHoldPresSetpoint.Text = PLC.EditRecipeStepDwellHoldPresSetpoint.ToString("0.0");
				buttonDwellHoldTime.Text = PLC.EditRecipeDwellHoldTime.ToString("c");
				buttonMaxVacTime.Text = PLC.EditRecipeMaxVacTime.ToString("c");
				buttonVacReliefTime.Text = PLC.EditRecipeVacReliefTime.ToString("c");
			}

		}

		private void buttonRecipeName_TextChanged(object sender, EventArgs e)
        {
            if (bRecipeNoChg || bOneShot)
                return;
			PLC.EditRecipeName = buttonRecipeName.Text;
        }

		private void buttonVacuumProfile_Click(object sender, EventArgs e)
		{
			if (panelVacProfile.Visible)
			{
				panelVacProfile.Visible = false;
				tabControlModules.Visible = true;
				buttonVacuumProfile.Text = "Vacuum\rProfile";
			}
			else
			{
				panelVacProfile.Visible = true;
				tabControlModules.Visible = false;
				buttonVacuumProfile.Text = "Recipe\rParams";
			}
		}

		#region Module 1 Buttons

		private void buttonMod1HPTemp_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Temperature Setpoint", this, buttonMod1HPTemp, "#0", 0, 180, "", false, false);
		}

		private void buttonMod1HPAtTempOffset_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} At Temperature Offset", this, buttonMod1HPTempOffset, "#0", 0, PLC.EditRecipeMod1Temp, "", false, false);
		}
		private void buttonMod1HPProxBakeHt_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Proximity Bake Height", this, buttonMod1HPProxBakeHt, "#0.0", 0, 22, "", true, false);
		}

		private void buttonMod1HPProxBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Proximity Bake Hours", this, buttonMod1HPProxBakeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod1HPProxBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Proximity Bake Minutes", this, buttonMod1HPProxBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod1HPProxBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Proximity Bake Seconds", this, buttonMod1HPProxBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod1HPContBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Contact Bake Hours", this, buttonMod1HPContBakeHrs, "00", 0, 8, "", false, false);
		}
		private void buttonMod1HPContBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Contact Bake Minutes", this, buttonMod1HPContBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod1HPContBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Contact Bake Seconds", this, buttonMod1HPContBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod1HPCoolTimeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Cool Down Hours", this, buttonMod1HPCoolTimeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod1HPCoolTimeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Cool Down Minutes", this, buttonMod1HPCoolTimeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod1HPCoolTimeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Cool Down Seconds", this, buttonMod1HPCoolTimeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod1VBTemp_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Temperature Setpoint", this, buttonMod1VBTemp, "#0", 0, 180, "", false, false);
		}

		private void buttonMod1VBTempOffset_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} At Temperature Offset", this, buttonMod1VBTempOffset, "#0", 0, PLC.EditRecipeMod1Temp, "", false, false);
		}

		private void buttonMod1VBProxBakeHt_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Proximity Bake Height", this, buttonMod1VBProxBakeHt, "#0.0", 0, 22, "", true, false);
		}

		private void buttonMod1VBProxBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Proximity Bake Hours", this, buttonMod1VBProxBakeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod1VBProxBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Proximity Bake Minutes", this, buttonMod1VBProxBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod1VBProxBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Proximity Bake Seconds", this, buttonMod1VBProxBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod1VBContBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Contact Bake Hours", this, buttonMod1VBContBakeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod1VBContBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Contact Bake Minutes", this, buttonMod1VBContBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod1VBContBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Contact Bake Seconds", this, buttonMod1VBContBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod1VBVacBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Vacuum Bake Hours", this, buttonMod1VBVacBakeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod1VBVacBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Vacuum Bake Minutes", this, buttonMod1VBVacBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod1VBVacBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Vacuum Bake Seconds", this, buttonMod1VBVacBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod1VBCoolTimeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Cool Down Hours", this, buttonMod1VBCoolTimeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod1VBCoolTimeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Cool Down Minutes", this, buttonMod1VBCoolTimeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod1VBCoolTimeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Cool Down Seconds", this, buttonMod1VBCoolTimeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod1VDTimeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Vacuum Dry Hours", this, buttonMod1VDTimeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod1VDTimeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Vacuum Dry Minutes", this, buttonMod1VDTimeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod1VDTimeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Vacuum Dry Seconds", this, buttonMod1VDTimeSecs, "00", 0, 59, "", false, false);
		}

		#endregion Module 1 Buttons

		#region Module 2 Buttons

		private void buttonMod2HPTemp_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Temperature Setpoint", this, buttonMod2HPTemp, "#0", 0, 180, "", false, false);
		}

		private void buttonMod2HPTempOffset_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} At Temperature Offset", this, buttonMod2HPTempOffset, "#0", 0, 25, "", false, false);
		}
		private void buttonMod2HPProxBakeHt_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Proximity Bake Height", this, buttonMod2HPProxBakeHt, "#0.0", 0, 22, "", true, false);
		}

		private void buttonMod2HPProxBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Proximity Bake Hours", this, buttonMod2HPProxBakeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod2HPProxBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Proximity Bake Minutes", this, buttonMod2HPProxBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod2HPProxBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Proximity Bake Seconds", this, buttonMod2HPProxBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod2HPContBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Contact Bake Hours", this, buttonMod2HPContBakeHrs, "00", 0, 8, "", false, false);
		}
		private void buttonMod2HPContBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Contact Bake Minutes", this, buttonMod2HPContBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod2HPContBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Contact Bake Seconds", this, buttonMod2HPContBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod2HPCoolTimeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Cool Down Hours", this, buttonMod2HPCoolTimeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod2HPCoolTimeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Cool Down Minutes", this, buttonMod2HPCoolTimeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod2HPCoolTimeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Cool Down Seconds", this, buttonMod2HPCoolTimeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod2VBTemp_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Temperature Setpoint", this, buttonMod2VBTemp, "#0", 0, 180, "", false, false);
		}

		private void buttonMod2VBTempOffset_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} At Temperature Offset", this, buttonMod2VBTempOffset, "#0", 0, 25, "", false, false);
		}

		private void buttonMod2VBProxBakeHt_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Proximity Bake Height", this, buttonMod2VBProxBakeHt, "#0.0", 0, 22, "", true, false);
		}

		private void buttonMod2VBProxBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Proximity Bake Hours", this, buttonMod2VBProxBakeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod2VBProxBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Proximity Bake Minutes", this, buttonMod2VBProxBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod2VBProxBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Proximity Bake Seconds", this, buttonMod2VBProxBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod2VBContBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Contact Bake Hours", this, buttonMod2VBContBakeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod2VBContBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Contact Bake Minutes", this, buttonMod2VBContBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod2VBContBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Contact Bake Seconds", this, buttonMod2VBContBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod2VBVacBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Vacuum Bake Hours", this, buttonMod2VBVacBakeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod2VBVacBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Vacuum Bake Minutes", this, buttonMod2VBVacBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod2VBVacBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Vacuum Bake Seconds", this, buttonMod2VBVacBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod2VBCoolTimeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Cool Down Hours", this, buttonMod2VBCoolTimeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod2VBCoolTimeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Cool Down Minutes", this, buttonMod2VBCoolTimeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod2VBCoolTimeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Cool Down Seconds", this, buttonMod2VBCoolTimeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod2VDTimeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Vacuum Dry Hours", this, buttonMod2VDTimeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod2VDTimeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Vacuum Dry Minutes", this, buttonMod2VDTimeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod2VDTimeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Vacuum Dry Seconds", this, buttonMod2VDTimeSecs, "00", 0, 59, "", false, false);
		}

		#endregion Module 2 Buttons

		#region Module 3 Buttons

		private void buttonMod3HPTemp_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Temperature Setpoint", this, buttonMod3HPTemp, "#0", 0, 180, "", false, false);
		}

		private void buttonMod3HPTempOffset_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} At Temperature Offset", this, buttonMod3HPTempOffset, "#0", 0, 25, "", false, false);
		}
		private void buttonMod3HPProxBakeHt_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Proximity Bake Height", this, buttonMod3HPProxBakeHt, "#0.0", 0, 22, "", true, false);
		}

		private void buttonMod3HPProxBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Proximity Bake Hours", this, buttonMod3HPProxBakeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod3HPProxBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Proximity Bake Minutes", this, buttonMod3HPProxBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod3HPProxBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Proximity Bake Seconds", this, buttonMod3HPProxBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod3HPContBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Contact Bake Hours", this, buttonMod3HPContBakeHrs, "00", 0, 8, "", false, false);
		}
		private void buttonMod3HPContBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Contact Bake Minutes", this, buttonMod3HPContBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod3HPContBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Contact Bake Seconds", this, buttonMod3HPContBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod3HPCoolTimeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Cool Down Hours", this, buttonMod3HPCoolTimeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod3HPCoolTimeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Cool Down Minutes", this, buttonMod3HPCoolTimeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod3HPCoolTimeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Cool Down Seconds", this, buttonMod3HPCoolTimeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod3VBTemp_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Temperature Setpoint", this, buttonMod3VBTemp, "#0", 0, 180, "", false, false);
		}

		private void buttonMod3VBTempOffset_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} At Temperature Offset", this, buttonMod3VBTempOffset, "#0", 0, 25, "", false, false);
		}

		private void buttonMod3VBProxBakeHt_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Proximity Bake Height", this, buttonMod3VBProxBakeHt, "#0.0", 0, 22, "", true, false);
		}

		private void buttonMod3VBProxBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Proximity Bake Hours", this, buttonMod3VBProxBakeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod3VBProxBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Proximity Bake Minutes", this, buttonMod3VBProxBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod3VBProxBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Proximity Bake Seconds", this, buttonMod3VBProxBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod3VBContBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Contact Bake Hours", this, buttonMod3VBContBakeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod3VBContBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Contact Bake Minutes", this, buttonMod3VBContBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod3VBContBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Contact Bake Seconds", this, buttonMod3VBContBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod3VBVacBakeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Vacuum Bake Hours", this, buttonMod3VBVacBakeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod3VBVacBakeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Vacuum Bake Minutes", this, buttonMod3VBVacBakeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod3VBVacBakeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Vacuum Bake Seconds", this, buttonMod3VBVacBakeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod3VBCoolTimeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Cool Down Hours", this, buttonMod3VBCoolTimeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod3VBCoolTimeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Cool Down Minutes", this, buttonMod3VBCoolTimeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod3VBCoolTimeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Cool Down Seconds", this, buttonMod3VBCoolTimeSecs, "00", 0, 59, "", false, false);
		}

		private void buttonMod3VDTimeHrs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Vacuum Dry Hours", this, buttonMod3VDTimeHrs, "00", 0, 8, "", false, false);
		}

		private void buttonMod3VDTimeMins_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Vacuum Dry Minutes", this, buttonMod3VDTimeMins, "00", 0, 59, "", false, false);
		}

		private void buttonMod3VDTimeSecs_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod3Name} Vacuum Dry Seconds", this, buttonMod3VDTimeSecs, "00", 0, 59, "", false, false);
		}

		#endregion Module 3 Buttons


		private void buttonStep1ValveAngle_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"Advanced Vacuum Step 1 Valve Angle", this, buttonStep1ValveAngle, "#0.0", 0, 90, "", true, false);
		}

		private void buttonStep1PresSetpoint_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"Advanced Vacuum Step 1 Pressure Setpoint", this, buttonStep1PresSetpoint, "#0.0", 0.1, 764, "", true, false);
		}

		private void buttonStep1Time_Click(object sender, EventArgs e)
		{
			_frmMain.GotoTimeScreen($"Advanced Vacuum Step 1 Time Setpoint", this, buttonStep1Time);
		}

		private void buttonStep2ValveAngle_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"Advanced Vacuum Step 2 Valve Angle", this, buttonStep2ValveAngle, "#0.0", 0, 90, "", true, false);
		}

		private void buttonStep2PresSetpoint_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"Advanced Vacuum Step 2 Pressure Setpoint", this, buttonStep2PresSetpoint, "#0.0", 0.1, 764, "", true, false);
		}

		private void buttonStep2Time_Click(object sender, EventArgs e)
		{
			_frmMain.GotoTimeScreen($"Advanced Vacuum Step 2 Time Setpoint", this, buttonStep2Time);
		}

		private void buttonStep3ValveAngle_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"Advanced Vacuum Step 3 Valve Angle", this, buttonStep3ValveAngle, "#0.0", 0, 90, "", true, false);
		}

		private void buttonStep3PresSetpoint_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"Advanced Vacuum Step 3 Pressure Setpoint", this, buttonStep3PresSetpoint, "#0.0", 0.1, 764, "", true, false);
		}

		private void buttonStep3Time_Click(object sender, EventArgs e)
		{
			_frmMain.GotoTimeScreen($"Advanced Vacuum Step 3 Time", this, buttonStep3Time);
		}

		private void buttonStep4ValveAngle_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"Advanced Vacuum Step 4 Valve Angle", this, buttonStep4ValveAngle, "#0.0", 0, 90, "", true, false);
		}

		private void buttonStep4PresSetpoint_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"Advanced Vacuum Step 4 Pressure Setpoint", this, buttonStep4PresSetpoint, "#0.0", 0.1, 764, "", true, false);
		}

		private void buttonStep4Time_Click(object sender, EventArgs e)
		{
			_frmMain.GotoTimeScreen($"Advanced Vacuum Step 4 Time Setpoint", this, buttonStep4Time);
		}

		private void buttonStepDwellHoldValveAngle_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"Advanced Vacuum Dwell/Hold Valve Angle", this, buttonStepDwellHoldValveAngle, "#0.0", 0, 90, "", true, false);
		}

		private void buttonStepDwellHoldPresSetpoint_Click(object sender, EventArgs e)
		{
			_frmMain.GotoNumScreen($"Advanced Vacuum Dwell/Hold Pressure Setpoint", this, buttonStepDwellHoldPresSetpoint, "#0.0", 0.1, 764, "", true, false);
		}

		private void buttonDwellHoldTime_Click(object sender, EventArgs e)
		{
			_frmMain.GotoTimeScreen($"Advanced Vacuum Dwell/Hold Time", this, buttonDwellHoldTime);
		}

		private void buttonMaxVacTime_Click(object sender, EventArgs e)
		{
			_frmMain.GotoTimeScreen($"Advanced Vacuum Maximum Vacuum Time", this, buttonMaxVacTime);
		}

		private void buttonVacReliefTime_Click(object sender, EventArgs e)
		{
			_frmMain.GotoTimeScreen($"Advanced Vacuum Relief Time", this, buttonVacReliefTime);
		}
	}
}
