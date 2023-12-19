using nAble.Data;
using nAble.Model.Recipes;
using nAble.Utils;
using nTact.DataComm;
using nTact.Recipes;
using Support2.RegistryClasses;

namespace nAble
{
    public partial class FormRecipeEditor : Form, IUpdateableForm
    {
        #region Data Members

        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;
        private readonly IRecipeManager _recipeMgr = null;

        private bool _loading = false;
        private bool _editingDefault = false;
        private bool _hasChanges = false;

        private Recipe _currentRecipe;
        private Recipe _defaultRecipe;

        //private TabPage _tabPageMain = null;
        //private TabPage _tabAirKnife = null;
        //private TabPage _tabIR = null;

        private RecipeAdvancedEditParams _primingShuttleSpeedParams;
        private RecipeAdvancedEditParams _dispenseRateParams;
        private RecipeAdvancedEditParams _coatingVelocityParams;
        private RecipeAdvancedEditParams _suckbackRateParams;
        private RecipeAdvancedEditParams _transitionJumpVelocityParams;
        private RecipeAdvancedEditParams _raiseZSpeedParams;
        private RecipeAdvancedEditParams _XMoveToPLESpeedParams;
        private RecipeAdvancedEditParams _ZMoveToPrimingGapHeightSpeedParams;
        private RecipeAdvancedEditParams _ZMoveForJumpParams;
        private RecipeAdvancedEditParams _ZMoveToCoatingOffsetParams;
        private RecipeAdvancedEditParams _ZMoveToCoatingGapParams;
        private RecipeAdvancedEditParams _ZMoveToTrailingOffsetGapParams;
        private RecipeAdvancedEditParams _endOfCoatSpeedParams;
        private RecipeAdvancedEditParams _XHomePositionParams;
        private DynamicDispenseProfileParamList _dispenseProfileParamList;
        private DispenseProfileEditorParams _dispenseProfileEditorParams;

        #endregion

        #region Properties

        private MachineSettingsII MS => _frmMain.MS;
        private GalilWrapper2 MC => _frmMain.MC;
        private bool HasIR => MS.IRLampInstalled;
        private bool HasAirKnife => MS.AirKnifeInstalled;

        #endregion

        public FormRecipeEditor(FormMain formMain, LogEntry log, IRecipeManager recipeManager)
        {
            InitializeComponent();

            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _log = log;
            _recipeMgr = recipeManager;

            //_tabPageMain = tabPageDefaults;
            //_tabAirKnife = tabPageAirKnife;
            //_tabIR = tabPageIR;
        }

        public void UpdateStatus()
        {
            if (!Visible)
            {
                return;
            }

            if (!_loading)
            {
                ChangesExist();
            }

            labelPrimingDispenseRateNote.BackColor = buttonPrimingRate.Text == "0" ? Color.Yellow : Color.Transparent;
            labelError.Visible = RecipeHasErrors(false);
            checkBoxUnloadSubstrateOnCompletion.Visible = MS.AutoLDULD || MS.HasLiftAndCenter || MS.LiftPinsEnabled;
            checkBoxAirKnifeDuringReturn.Visible = MS.AirKnifeInstalled;
            checkBoxAirKnifeDuringCoat.Visible = MS.AirKnifeInstalled;
            panelKeyence.Visible = MS.KeyenceLaserInstalled && MS.UsingKeyenceLaser;
            checkBoxUseLasers.Visible = MS.KeyenceLaserInstalled && MS.UsingKeyenceLaser;
            checkBoxVerifyVision.Visible = MS.CognexCommunicationsUsed;
            labelPrimingDelayUnits.Text = MS.DisplayedTimeUnits;
            labelCoatingDelayUnits.Text = MS.DisplayedTimeUnits;
            labelCoatingOffsetDelayUnits.Text = MS.DisplayedTimeUnits;
            PostRechargeDelayUnits.Text = MS.DisplayedTimeUnits;

            groupBoxIRCoatingPowerLevel.Enabled = checkBoxUseIRDuringCoating.Checked;
            groupBoxIRReturnPowerLevel.Enabled = checkBoxUseIRDuringReturn.Checked;

            if (MS.HidePumpMixing && _currentRecipe.SelectedPump == "MIXING")
            {
                _currentRecipe.UpdateMixingRatios(0, 1.000, 0.000);
            }

            bool dispProfileEnabled = _frmMain.LicMgr.IsFeatureActive(LicensedFeautres.Feature2) || _frmMain.IsDemoMode || _frmMain.DebugMode;
            panelDispinseProfileType.Visible = dispProfileEnabled;
            if(dispProfileEnabled)
                buttonEditDispenseProfile.Visible = comboBoxDispenseProfileType.SelectedIndex>0;
        }

        private void LoadAdvancedParams(int accelIndex, int decelIndex, int sCurveIndex, RecipeAdvancedEditParams advParams)
        {
            double temp;
            LoadParam(accelIndex, out temp);
            advParams.Accel = temp;
            advParams.RecipeAccel = temp;
            LoadParam(decelIndex, out temp);
            advParams.Decel = temp;
            advParams.RecipeDecel = temp;
            LoadParam(sCurveIndex, out temp);
            advParams.SCurve = temp;
            advParams.RecipeSCurve = temp;
        }

        private void LoadDispProfileParams(int param1Index, int param2Index, int param3Index, DispenseProfileEditorParams dispProfileParams)
        {
            double temp;
            LoadParam(param1Index, out temp);
            dispProfileParams.Param1 = temp;
            dispProfileParams.RecipeParam1 = temp;
            LoadParam(param2Index, out temp);
            dispProfileParams.Param2 = temp;
            dispProfileParams.RecipeParam2 = temp;
            LoadParam(param3Index, out temp);
            dispProfileParams.Param3 = temp;
            dispProfileParams.RecipeParam3 = temp;
        }
        private bool LoadParam(int nIdx, out double dTempVar)
        {
            bool bRetVal = true;
            dTempVar = 0;
            RecipeParam tempParam = null;
            bRetVal = _currentRecipe.GetParam(nIdx, ref tempParam);
            if (bRetVal)
            {
                try
                {
                    dTempVar = double.Parse(tempParam.Value);
                }
                catch (FormatException)
                {
                    bRetVal = false;
                }
            }
            else
            {
                tempParam = null;
                bRetVal = _defaultRecipe.GetParam(nIdx, ref tempParam);
                if (bRetVal)
                {
                    try
                    {
                        dTempVar = double.Parse(tempParam.Value);
                    }
                    catch (FormatException)
                    {
                        bRetVal = false;
                    }
                }
            }
            tempParam = null;
            return bRetVal;
        }

        private bool LoadParam(int nIdx, CheckBox checkBox)
        {
            bool bRetVal = true;
            RecipeParam tempParam = null;
            bRetVal = _currentRecipe.GetParam(nIdx, ref tempParam);
            if (bRetVal)
            {
                checkBox.Checked = (tempParam.Value == "1");

            }
            else
            {
                tempParam = null;
                bRetVal = _defaultRecipe.GetParam(nIdx, ref tempParam);
                if (bRetVal)
                    checkBox.Checked = (tempParam.Value == "1");
            }
            tempParam = null;
            return bRetVal;
        }

        private bool LoadParam(int nIdx, Control control,bool isCombobox = false)
        {
            bool bRetVal = true;
            RecipeParam tempParam = null;
            bRetVal = _currentRecipe.GetParam(nIdx, ref tempParam);
            if (bRetVal)
            {
                try
                {
                    if (isCombobox)
                        ((ComboBox)control).SelectedIndex = int.Parse(tempParam.Value);
                    else
                        control.Text = tempParam.Value;
                }
                catch (FormatException)
                {
                    bRetVal = false;
                }
            }
            else
            {
                tempParam = null;
                bRetVal = _defaultRecipe.GetParam(nIdx, ref tempParam);
                if (bRetVal)
                {
                    try
                    {
                        if (isCombobox)
                            ((ComboBox)control).SelectedIndex = int.Parse(tempParam.Value);
                        else
                            control.Text = tempParam.Value;
                    }
                    catch (FormatException)
                    {
                        bRetVal = false;
                    }
                }
            }
            tempParam = null;
            return bRetVal;
        }

        public bool LoadRecipe(string sRecipeFileName, bool isDefault = false)
        {
            bool loaded = false;
            double airKnife = 0;
            double tempParam = 0;
            _editingDefault = isDefault;

            if (_loading)
            {
                return loaded;
            }

            HideAndShowTabsAsRequired();
            SetupAdvancedParamSets();

            _defaultRecipe = _recipeMgr.DefaultRecipe;

            if (_defaultRecipe == null)
            {
                nRadMessageBox.Show(this, "The default parameter file can not be read.\r\n      This a fatal error.\r\nPlease contact nTact support.", "File Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return loaded;
            }

            _loading = true;

            _currentRecipe = null;
            _currentRecipe = _editingDefault ? _defaultRecipe : _recipeMgr.FetchRecipe(sRecipeFileName);

            if (_currentRecipe != null)
            {
                comboBoxKeyenceProgramNumber.SelectedIndex = CommonUtils.ProgramNumberIsValid(_currentRecipe.KeyenceProgramNumber) ? 
                    _currentRecipe.KeyenceProgramNumber : 0;
                checkBoxUseLasers.Checked = _currentRecipe.UsingKeyenceLaser;
                checkBoxReleaseVacOnCompletion.Checked = _currentRecipe.ReleaseVacuumOnCompletion;
                textBoxRecipeName.Text = _currentRecipe.Name;
                checkBoxUnloadSubstrateOnCompletion.Checked = _currentRecipe.UnloadSubstrateOnCompletion;

                LoadParam(25, buttonPLE);
                if (MS.HasPrimingArea || MS.HasPrimingPlate)
                    LoadParam(26, buttonPrimingRate);
                else
                    buttonPrimingRate.Text = "0";

                if (MS.DisplayedTimeUnits == "sec")
                {
                    LoadParam(27, out tempParam);
                    tempParam = tempParam / 1000;
                    buttonPrimingDelay.Text = tempParam.ToString();
                }
                else
                {
                    LoadParam(27, buttonPrimingDelay);
                }

                LoadParam(28, buttonPrimingVelocity);
                LoadAdvancedParams(107, 108, 109, _primingShuttleSpeedParams);

                LoadParam(29, buttonPrimingDispenseOff);
                LoadParam(30, buttonCLE);
                LoadParam(32, buttonCoatingRate);

                if (MS.DisplayedTimeUnits == "sec")
                {
                    LoadParam(33, out tempParam);
                    tempParam = tempParam / 1000;
                    buttonCoatingDelay.Text = tempParam.ToString();
                }
                else
                {
                    LoadParam(33, buttonCoatingDelay);
                }

                LoadParam(34, buttonEOC);
                LoadParam(35, buttonCoatingDispenseOff);

                if (MS.DisplayedTimeUnits == "sec")
                {
                    LoadParam(37, out tempParam);
                    tempParam = tempParam / 1000;
                    buttonLEODelay.Text = tempParam.ToString();
                }
                else
                {
                    LoadParam(37, buttonLEODelay);
                }

                LoadAdvancedParams(85, 86, 87, _dispenseRateParams);
                LoadParam(100, buttonPrimingGap);

                LoadParam(106, buttonEOP);

                double dTemp;
                LoadParam(110, out dTemp);
                int nVal = (int)dTemp;
                comboBoxSelectedPump.SelectedIndex = MS.DualPumpInstalled ? nVal : 0;

                LoadParam(111, out dTemp);
                if (!MS.DualPumpInstalled)
                    dTemp = 1;
                buttonPumpARatio.Text = (dTemp*100.0).ToString("#0.000");

                LoadParam(112, out dTemp);
                if (!MS.DualPumpInstalled)
                    dTemp = 0;
                buttonPumpBRatio.Text = (dTemp * 100.0).ToString("#0.000");

                LoadParam(115, buttonJumpHeight);

                LoadParam(121, buttonJumpVelocity);
                LoadAdvancedParams(122, 123, 124, _transitionJumpVelocityParams);
                LoadParam(125, buttonTransitionDispenseRate);

                LoadParam(130, checkBoxCoatingOffset);
                groupBoxCoatingOffset.Enabled = checkBoxCoatingOffset.Checked;
                LoadParam(131, buttonLEOGap);
                LoadParam(136, buttonCoatingGap);

                LoadParam(144, buttonCoatingVelocity);
                LoadAdvancedParams(145, 146, 147, _coatingVelocityParams);

                LoadParam(150, checkBoxTEO);
                groupBoxTEO.Enabled = checkBoxTEO.Checked;
                LoadParam(151, buttonTEOOffset);
                LoadParam(152, buttonTEOGap);
                LoadParam(157, buttonRechargeRate);

                if (MS.DisplayedTimeUnits == "sec")
                {
                    LoadParam(158, out tempParam);
                    tempParam = tempParam / 1000;
                    buttonPostRechargeDelay.Text = tempParam.ToString();
                }
                else
                {
                    LoadParam(158, buttonPostRechargeDelay);
                }

                LoadParam(76, buttonAirKnifeCoatingStartOffset);
                LoadParam(77, buttonAirKnifePostCoatDistance);
                LoadParam(78, out airKnife);
                checkBoxAirKnifeDuringReturn.Checked = MS.AirKnifeInstalled ? ((int)airKnife & 0x1) == 0x1 : false;
                checkBoxAirKnifeDuringCoat.Checked = MS.AirKnifeInstalled ? ((int)airKnife & 0x2) == 0x2 : false;
                LoadParam(79, buttonAirKnifePostCoatDelay);
                LoadParam(80, buttonAirKnifeShuttleVel);
                LoadParam(81, checkBoxAirKnifeAfterCoating);

                if (HasIR)
                {
                    LoadParam(196, buttonIRCoatingStartOffset);
                    LoadParam(197, buttonIRPostCoatDistance);
                    double irUsage = 0;
                    LoadParam(198, out irUsage);
                    checkBoxUseIRDuringCoating.Checked = 0x1 == ((int)irUsage & 0x1);
                    checkBoxUseIRDuringReturn.Checked = 0x2 == ((int)irUsage & 0x2);
                    buttonIRPowerLevelCoating.Text = _currentRecipe.IRPowerLevelDuringCoating.ToString("#0");
                    buttonIRPowerLevelReturn.Text = _currentRecipe.IRPowerLevelOnReturn.ToString("#0");
                    LoadParam(199, buttonIRDelayOnReturn);
                }
                else
                {
                    checkBoxUseIRDuringCoating.Checked = false;
                    checkBoxUseIRDuringReturn.Checked = false;
                }

                LoadParam(166, buttonReturnVelocity);
                LoadParam(170, checkBoxSuckback);
                groupBoxSuckback.Enabled = checkBoxSuckback.Checked;
                LoadParam(171, buttonSuckbackOffset);
                LoadParam(172, buttonSuckbackRate);
                LoadAdvancedParams(173, 174, 175, _suckbackRateParams);
                LoadParam(176, buttonSuckbackVol);

                LoadParam(126,comboBoxDispenseProfileType,true);
                _dispenseProfileEditorParams = new DispenseProfileEditorParams();
                LoadDispProfileParams(127, 128, 129,_dispenseProfileEditorParams);
                _dispenseProfileParamList = new DynamicDispenseProfileParamList();
                if(_currentRecipe.DispenseProfileParams != null)
                {
                    foreach(DynamicDispenseProfileParam param in _currentRecipe.DispenseProfileParams)
                    {
                        _dispenseProfileParamList.Add(param.Clone());
                    }
                }

                if (_editingDefault)
                {
                    LoadParam(90, buttonRaiseZHeight);
                    LoadParam(91, buttonRaiseZSpeed);
                    LoadAdvancedParams(92, 93, 94, _raiseZSpeedParams);
                    LoadParam(96, buttonXMoveToPLE);
                    LoadAdvancedParams(97, 98, 99, _XMoveToPLESpeedParams);
                    LoadParam(101, buttonZMoveToPrimingGapHeightSpeed);
                    LoadAdvancedParams(102, 103, 104, _ZMoveToPrimingGapHeightSpeedParams);
                    LoadParam(116, buttonZMoveForJump);
                    LoadAdvancedParams(117, 118, 119, _ZMoveForJumpParams);
                    LoadParam(132, buttonZMoveToCoatingOffset);                    
                    LoadAdvancedParams(133, 134, 135, _ZMoveToCoatingOffsetParams);
                    LoadParam(137, buttonZMoveToCoatingGap);
                    LoadAdvancedParams(138, 139, 140, _ZMoveToCoatingGapParams);
                    LoadParam(153, buttonZMoveToTrailingOffsetGap);
                    LoadAdvancedParams(154, 155, 156, _ZMoveToTrailingOffsetGapParams);
                    LoadParam(160, buttonEndOfCoatHeight);
                    LoadParam(161, buttonEndOfCoatSpeed);
                    LoadAdvancedParams(162, 163, 164, _endOfCoatSpeedParams);
                    LoadParam(165, buttonXHomePosition);
                    LoadAdvancedParams(167, 168, 169, _XHomePositionParams);
                    LoadParam(177, buttonAirKnifeDistancePastEOC);
                }

                LoadParam(185, checkBoxReleaseVacAtEOC);

                loaded = true;
            }
            else
            {
                nRadMessageBox.Show(this, "ERROR:  Could not load recipe file", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _loading = false;
            return loaded;
        }

        private void HideAndShowTabsAsRequired()
        {
            tabControlRecipe.ShowOrHideTab(tabPageDefaults, _editingDefault, index: 0);
            tabControlRecipe.ShowOrHideTab(tabPagePumpMixing, MS.DualPumpInstalled);
            tabControlRecipe.ShowOrHideTab(tabPageAirKnife, MS.AirKnifeInstalled);
            tabControlRecipe.ShowOrHideTab(tabPageIR, MS.IRLampInstalled);
            tabControlRecipe.ShowOrHideTab(tabPagePriming, (MS.HasPrimingPlate || MS.HasPrimingArea));
        }

        private void UpdateAdvancedParams(int startIndex, RecipeAdvancedEditParams advParams)
        {
            UpdateParam(startIndex, advParams.Accel);
            UpdateParam(startIndex+1, advParams.Decel);
            UpdateParam(startIndex+2, advParams.SCurve);
        }

        private bool UpdateParam(int nIdx, bool bNewValue)
        {
            return UpdateParam(nIdx, bNewValue ? 1 : 0);
        }

        private bool UpdateParam(int nIdx, double dNewValue)
        {
            bool bRetVal = true;
            RecipeParam tempParam = null;
            bRetVal = _currentRecipe.GetParam(nIdx, ref tempParam);
            if (!bRetVal)
            {
                bRetVal = _defaultRecipe.GetParam(nIdx, ref tempParam);
            }
            if (bRetVal)
            {
                if (tempParam.Value != dNewValue.ToString())
                {
                    tempParam.Value = dNewValue.ToString();
                    _currentRecipe.UpdateParam(tempParam);
                }
            }
            tempParam = null;
            return bRetVal;
        }

        public bool Save()
        {
            bool saved = false;
            if (_currentRecipe != null)
            {
                try
                {
                    _currentRecipe.Name = textBoxRecipeName.Text;
                    _currentRecipe.LastUpdate = DateTime.Now;
                    _currentRecipe.LastUpdatedBy = string.Format("{0}/{1} on machine {2}",
                        Environment.UserDomainName, Environment.UserName, Environment.MachineName);
                    _currentRecipe.UsingKeyenceLaser = !MS.KeyenceLaserInstalled || !MS.UsingKeyenceLaser ? false : checkBoxUseLasers.Checked;
                    _currentRecipe.KeyenceProgramNumber = !MS.KeyenceLaserInstalled || !MS.UsingKeyenceLaser ?
                        0 : comboBoxKeyenceProgramNumber.SelectedIndex;
                    _currentRecipe.VerifyFiducials = MS.CognexCommunicationsUsed ? checkBoxVerifyVision.Checked : false;
                    _currentRecipe.ReleaseVacuumOnCompletion = checkBoxReleaseVacOnCompletion.Checked;
                    _currentRecipe.UnloadSubstrateOnCompletion = (MS.LiftPinsEnabled || MS.HasLiftAndCenter) ? checkBoxUnloadSubstrateOnCompletion.Checked : false;
                    UpdateParam(25, double.Parse(buttonPLE.Text));
                    UpdateParam(26, double.Parse(buttonPrimingRate.Text));

                    //Save Delays In ms ALWAYS - Detect if value is > 1000, If So, Store the Button Text As Value
                    if (MS.DisplayedTimeUnits == "sec" && double.Parse(buttonPrimingDelay.Text) < 1000)
                    {
                        UpdateParam(27, double.Parse(buttonPrimingDelay.Text) * 1000);
                    }
                    else
                    {
                        UpdateParam(27, double.Parse(buttonPrimingDelay.Text));
                    }

                    UpdateParam(28, double.Parse(buttonPrimingVelocity.Text));
                    UpdateParam(29, double.Parse(buttonPrimingDispenseOff.Text));
                    UpdateParam(30, double.Parse(buttonCLE.Text));
                    UpdateParam(32, double.Parse(buttonCoatingRate.Text));

                    if (MS.DisplayedTimeUnits == "sec" && double.Parse(buttonCoatingDelay.Text) < 1000)
                    {
                        UpdateParam(33, double.Parse(buttonCoatingDelay.Text) * 1000);
                    }
                    else
                    {
                        UpdateParam(33, double.Parse(buttonCoatingDelay.Text));
                    }

                    UpdateParam(34, double.Parse(buttonEOC.Text));
                    UpdateParam(35, double.Parse(buttonCoatingDispenseOff.Text));

                    if (MS.DisplayedTimeUnits == "sec" && double.Parse(buttonLEODelay.Text) < 1000)
                    {
                        UpdateParam(37, double.Parse(buttonLEODelay.Text) * 1000);
                    }
                    else
                    {
                        UpdateParam(37, double.Parse(buttonLEODelay.Text));
                    }

                    UpdateAdvancedParams(85, _dispenseRateParams);
                    UpdateParam(100, double.Parse(buttonPrimingGap.Text));

                    UpdateParam(106, double.Parse(buttonEOP.Text));
                    UpdateAdvancedParams(107, _primingShuttleSpeedParams);

                    UpdateParam(110, MS.DualPumpInstalled ? comboBoxSelectedPump.SelectedIndex : 0);
                    double dTemp = double.Parse(buttonPumpARatio.Text);
                    dTemp = dTemp / 100.00;
                    UpdateParam(111, MS.DualPumpInstalled ? dTemp : 1);
                    dTemp = double.Parse(buttonPumpBRatio.Text);
                    dTemp = dTemp / 100.00;
                    UpdateParam(112, MS.DualPumpInstalled ? dTemp : 0);
                    UpdateParam(115, double.Parse(buttonJumpHeight.Text));

                    UpdateParam(121, double.Parse(buttonJumpVelocity.Text));
                    UpdateAdvancedParams(122, _transitionJumpVelocityParams);
                    UpdateParam(125, double.Parse(buttonTransitionDispenseRate.Text));

                    UpdateParam(130, checkBoxCoatingOffset.Checked);
                    UpdateParam(131, double.Parse(buttonLEOGap.Text));
                    UpdateParam(136, double.Parse(buttonCoatingGap.Text));
                    UpdateParam(144, double.Parse(buttonCoatingVelocity.Text));
                    UpdateAdvancedParams(145, _coatingVelocityParams);
                    UpdateParam(150, checkBoxTEO.Checked);
                    UpdateParam(151, double.Parse(buttonTEOOffset.Text));
                    UpdateParam(152, double.Parse(buttonTEOGap.Text));
                    UpdateParam(157, double.Parse(buttonRechargeRate.Text));

                    if (MS.DisplayedTimeUnits == "sec" && double.Parse(buttonPostRechargeDelay.Text) < 1000)
                    {
                        UpdateParam(158, double.Parse(buttonPostRechargeDelay.Text) * 1000);
                    }
                    else
                    {
                        UpdateParam(158, double.Parse(buttonPostRechargeDelay.Text));
                    }

                    UpdateParam(185, checkBoxReleaseVacAtEOC.Checked);

                    //Air Knife
                    UpdateParam(76, double.Parse(buttonAirKnifeCoatingStartOffset.Text));
                    UpdateParam(77, double.Parse(buttonAirKnifePostCoatDistance.Text));

                    double dAirKnife = 0;
                    dAirKnife += checkBoxAirKnifeDuringReturn.Checked ? 1 : 0;
                    dAirKnife += checkBoxAirKnifeDuringCoat.Checked ? 2 : 0;
                    UpdateParam(78, dAirKnife);

                    UpdateParam(79, double.Parse(buttonAirKnifePostCoatDelay.Text));
                    UpdateParam(80, double.Parse(buttonAirKnifeShuttleVel.Text));
                    UpdateParam(81, checkBoxAirKnifeAfterCoating.Checked);

                    //IR
                    if (HasIR)
                    {
                        UpdateParam(196, double.Parse(buttonIRCoatingStartOffset.Text));
                        UpdateParam(197, double.Parse(buttonIRPostCoatDistance.Text));
                        //double irUsage = (checkBoxUseIRDuringCoating.Checked ? 1 : 0) + (checkBoxUseIRDuringReturn.Checked ? 2 : 0) + (checkBoxIROnEndOfPrime.Checked ? 4 : 0);
                        double irUsage = (checkBoxUseIRDuringCoating.Checked ? 1 : 0) + (checkBoxUseIRDuringReturn.Checked ? 2 : 0);
                        UpdateParam(198, irUsage);
                        _currentRecipe.UseIRDuringCoating = checkBoxUseIRDuringCoating.Checked;
                        _currentRecipe.UseIROnReturn = checkBoxUseIRDuringReturn.Checked;
                        //_currentRecipe.IROnEndOfPriming
                        _currentRecipe.IRPowerLevelDuringCoating = double.Parse(buttonIRPowerLevelCoating.Text);
                        _currentRecipe.IRPowerLevelOnReturn = double.Parse(buttonIRPowerLevelReturn.Text);
                        UpdateParam(199, double.Parse(buttonIRDelayOnReturn.Text));
                    }
                    else
                    {
                        UpdateParam(196, 0);
                        UpdateParam(197, 0);
                        UpdateParam(198, 0);
                        _currentRecipe.UseIRDuringCoating = false;
                        _currentRecipe.UseIROnReturn = false;
                        _currentRecipe.IRPowerLevelDuringCoating = 0;
                        _currentRecipe.IRPowerLevelOnReturn = 0;
                        UpdateParam(199, 0);
                    }

                    UpdateParam(166, double.Parse(buttonReturnVelocity.Text));

                    //Suckback
                    UpdateParam(170, checkBoxSuckback.Checked);
                    UpdateParam(171, double.Parse(buttonSuckbackOffset.Text));
                    UpdateParam(172, double.Parse(buttonSuckbackRate.Text));
                    UpdateAdvancedParams(173, _suckbackRateParams);
                    UpdateParam(176, double.Parse(buttonSuckbackVol.Text));

                    if (panelDispinseProfileType.Visible)
                    {
                        int profileType = comboBoxDispenseProfileType.SelectedIndex;
                        UpdateParam(126, profileType);
                        UpdateParam(127, profileType > 0 ? _dispenseProfileEditorParams.Param1 : 0);
                        UpdateParam(128, profileType == 3 ? _dispenseProfileEditorParams.Param2 : 0);
                        UpdateParam(129, profileType > 4 ? _dispenseProfileEditorParams.Param3 : 0);
                        if (comboBoxDispenseProfileType.SelectedIndex == 4)
                            _currentRecipe.DispenseProfileParams = _dispenseProfileParamList;
                        else
                            _currentRecipe.DispenseProfileParams.Clear();
                    }
                    else
                    {
                        UpdateParam(126, 0);
                        UpdateParam(127, 0);
                        UpdateParam(128, 0);
                        UpdateParam(129, 0);
                        _currentRecipe.DispenseProfileParams.Clear();
                    }

                    if (_editingDefault)
                    {
                        UpdateParam(90, double.Parse(buttonRaiseZHeight.Text));
                        UpdateParam(91, double.Parse(buttonRaiseZSpeed.Text));
                        UpdateAdvancedParams(92, _raiseZSpeedParams);
                        UpdateParam(96, double.Parse(buttonXMoveToPLE.Text));
                        UpdateAdvancedParams(97, _XMoveToPLESpeedParams);
                        UpdateParam(101, double.Parse(buttonZMoveToPrimingGapHeightSpeed.Text));
                        UpdateAdvancedParams(102, _ZMoveToPrimingGapHeightSpeedParams);
                        UpdateParam(116, double.Parse(buttonZMoveForJump.Text));
                        UpdateAdvancedParams(117, _ZMoveForJumpParams);
                        UpdateParam(132, double.Parse(buttonZMoveToCoatingOffset.Text));
                        UpdateAdvancedParams(133, _ZMoveToCoatingOffsetParams);
                        UpdateParam(137, double.Parse(buttonZMoveToCoatingGap.Text));
                        UpdateAdvancedParams(138, _ZMoveToCoatingGapParams);
                        UpdateParam(153, double.Parse(buttonZMoveToTrailingOffsetGap.Text));
                        UpdateAdvancedParams(154, _ZMoveToTrailingOffsetGapParams);
                        UpdateParam(160, double.Parse(buttonEndOfCoatHeight.Text));
                        UpdateParam(161, double.Parse(buttonEndOfCoatSpeed.Text));
                        UpdateAdvancedParams(162, _endOfCoatSpeedParams);
                        UpdateParam(165, double.Parse(buttonXHomePosition.Text));
                        UpdateAdvancedParams(167, _XHomePositionParams);
                        UpdateParam(177, double.Parse(buttonAirKnifeDistancePastEOC.Text));
                    }

                    saved = _recipeMgr.SaveRecipe(_currentRecipe, _editingDefault);

                    if (saved)
                    {
                        Text = $"Recipe - {_currentRecipe.Name}";
                    }
                }
                catch (Exception ex)
                {
                    _log.log(LogType.TRACE, Category.ERROR, "Error Saving Recipes. Error:" + ex.ToString());
                }
            }

            return saved;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (RecipeHasErrors(true))
            {
                _log.log(LogType.TRACE, Category.INFO, "Recipe had errors.  Save aborted");
                return;
            }

            if (DialogResult.No == nRadMessageBox.Show(this, string.Format("Save Changes to Recipe '{0}'?", _currentRecipe.Name), "Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                _log.log(LogType.TRACE, Category.INFO, "User Cancelled Recipe Save", "ACTION");
                return;
            }

            if (Save())
            {
                nRadMessageBox.Show(this, string.Format("Recipe '{0}' successfully saved", _currentRecipe.Name), "Save Recipe", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            _frmMain.CancelConfirmLeave();
            _frmMain.ShowPrevForm();
        }

        private bool RecipeHasErrors(bool showErrors = false)
        {
            int coatGap = int.Parse(buttonCoatingGap.Text);
            int LEOGap = int.Parse(buttonLEOGap.Text);
            int TEOGap = int.Parse(buttonTEOGap.Text);

            double coatingStart = double.Parse(buttonCLE.Text);
            double TEO = double.Parse(buttonTEOOffset.Text);
            double coatingEnd = double.Parse(buttonEOC.Text);
            double coatingLength = coatingEnd - coatingStart;
            double primingPlateStart = MS.PrimingPlateStart;
            double primingPlateEnd = MS.PrimingPlateEnd;
            double chuckStart = MS.ChuckStart;
            double chuckEnd = MS.ChuckEnd;
            double irPitch = MS.DieToIRPitch;
            double akPitch = MS.DieToAirKnifePitch;
            double primeLEP = double.Parse(buttonPLE.Text);
            double primeEOP = double.Parse(buttonEOP.Text);
            double primeOff = double.Parse(buttonPrimingDispenseOff.Text);
            double endOfIR = double.Parse(buttonIRPostCoatDistance.Text);
            double airKnifePostDist = double.Parse(buttonAirKnifePostCoatDistance.Text);
            double coatOff = double.Parse(buttonCoatingDispenseOff.Text);
            double suckback = double.Parse(buttonSuckbackOffset.Text);
            double coatSpeed = double.Parse(buttonCoatingVelocity.Text);
            double coatDelay = double.Parse(buttonCoatingDelay.Text);
            double coatingRate = double.Parse(buttonCoatingRate.Text);
            double suckbackVol = double.Parse(buttonSuckbackVol.Text);
            double dispenseVol = ((coatDelay / 1000.0) + (coatingEnd - coatingStart) / coatSpeed) * coatingRate;

            bool usingAirKnifeCoat = MS.AirKnifeInstalled && checkBoxAirKnifeDuringCoat.Checked;
            bool usingAirKnifeAfter = MS.AirKnifeInstalled && checkBoxAirKnifeAfterCoating.Checked;
            bool usingIR = checkBoxUseIRDuringCoating.Checked || checkBoxUseIRDuringReturn.Checked;
            bool vacOffAtEOC = checkBoxReleaseVacAtEOC.Checked;
            bool IRDuringReturn = checkBoxUseIRDuringReturn.Checked;

            if ((MS.HasPrimingArea || MS.HasPrimingPlate) && buttonPrimingRate.Text != "0")
            {
                // priming should start on the priming plate
                if (primeLEP < primingPlateStart || primeLEP > primingPlateEnd)
                {
                    nRadMessageBox.ShowIf(showErrors, this, "The specified priming start position is not on the defined priming plate.", 
                        "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }

                // priming should end on the priming plate
                if (primeEOP < primingPlateStart || primeEOP > primingPlateEnd)
                {
                    nRadMessageBox.ShowIf(showErrors, this, "The specified priming end position is not on the defined priming plate.", 
                        "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }

                // priming stop dispense should happen AFTER priming start
                if ((primeEOP - primeOff) <= primeLEP)
                {
                    nRadMessageBox.ShowIf(showErrors, this, "The specified priming dispense offset is too large.  It puts dispense off before priming start.",
                        "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }

            // coating should start on the chuck
            if (!(chuckStart <= coatingStart && coatingStart < chuckEnd))
            {
                nRadMessageBox.ShowIf(showErrors, this, "The specified coating start position is not on the defined chuck.", 
                    "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            // make sure the coating ends on the chuck
            if (!(chuckStart < coatingEnd && coatingEnd <= chuckEnd))
            {
                nRadMessageBox.ShowIf(showErrors, this, "The specified coating end position is not on the defined chuck.", 
                    "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            // coating position order
            if (coatingStart >= coatingEnd)
            {
                nRadMessageBox.ShowIf(showErrors, this, "The specified coating start and stop positions are out of order.",
                    "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            // LEO < Coat Gap
            if (coatGap <= LEOGap)
            {
                nRadMessageBox.ShowIf(showErrors, this, "The Leading Edge Offset from Coat Height is larger than the Coat Gap.", 
                    "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            // TEO < Coat Gap
            if (coatGap <= TEOGap)
            {
                nRadMessageBox.ShowIf(showErrors, this, "The Trailing Edge Offset from Coat Height is larger than the Coat Gap.", 
                    "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            // TEO, DispenseOFF and Suckback offsets fit in coated area
            if (coatingLength <= TEO || coatingLength <= coatOff || (checkBoxSuckback.Checked && coatingLength <= suckback))
            {
                nRadMessageBox.ShowIf(showErrors, this, "One or more offsets do not fit within the specified coating area.", 
                    "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            // DispenseOFF must be greater than Suckback offset
            if (checkBoxSuckback.Checked && suckback >= coatOff)
            {
                nRadMessageBox.ShowIf(showErrors, this, "The Suckback Offset must be less than the Coating Dispense Stop Offset.", 
                    "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            // IR cannot force past end of stroke
            if (checkBoxUseIRDuringCoating.Checked && ((endOfIR + irPitch + coatingEnd) > MS.MaxXTravel))
            {
                nRadMessageBox.ShowIf(showErrors, this, "The specified IR Lamp Post Coat Distance Will Cause X-Axis To Move Beyond Limit.\n" + 
                    "Please Adjust either EOC or IR Post Coat Distance.", "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            // Air Knife cannot force past end of stroke
            if (usingAirKnifeCoat && ((airKnifePostDist + akPitch + coatingEnd) > MS.MaxXTravel))
            {
                nRadMessageBox.ShowIf(showErrors, this, "The specified Air Knife Post Coat Distance Will Cause X-Axis To Move Beyond Limit.\n" +
                    "Please Adjust either EOC or Air Knife Post Coat Distance.", "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            // Air Knife Velocity Set If Used
            if (usingAirKnifeAfter && decimal.Parse(buttonAirKnifeShuttleVel.Text) == 0)
            {
                nRadMessageBox.ShowIf(showErrors, this, "No Velocity Set For Air-Knife Post Coat Routine.", 
                    "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            // Cannot use vac off at EOC
            if (vacOffAtEOC && IRDuringReturn)
            {
                nRadMessageBox.ShowIf(showErrors, this, "Cannot use IR On Return and also Release Vac at EOC at the same time.", 
                    "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            // Too much suck back request.
            if (checkBoxSuckback.Checked && (suckbackVol > dispenseVol))
            {
                nRadMessageBox.ShowIf(showErrors, this, $"The Suckback Volume must be less than the dispensed volume. ({dispenseVol:0.000} µl)", 
                    "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            if (checkBoxSuckback.Checked && (checkBoxAirKnifeDuringCoat.Checked || checkBoxAirKnifeDuringReturn.Checked))
            {
                nRadMessageBox.ShowIf(showErrors, this, "Suckback cannot be used with airknife during coating or on return selected.",
                    "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            if (checkBoxSuckback.Checked && (checkBoxUseIRDuringCoating.Checked || checkBoxUseIRDuringReturn.Checked))
            {
                nRadMessageBox.ShowIf(showErrors, this, "Suckback cannot be used with IR during coating or IR on return selected.",
                    "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            //TODO:  These seemed to break normal recipes.  Check later to see if they SHOULD be here.

            //if (!AreAdvancedInRange(Usage.JumpVelocity))
            //{
            //    nRadMessageBox.ShowIf(showErrors, this, $"One or more Transition Shuttle Velocity Advanced parameters is out of range.", 
            //        "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return true;
            //}

            //if (!AreAdvancedInRange(Usage.CoatingVelocity))
            //{
            //    nRadMessageBox.ShowIf(showErrors, this, $"One or more Coating Shuttle Velocity Advanced parameters is out of range.",
            //        "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return true;
            //}

            //if (!AreAdvancedInRange(Usage.DispenseRate))
            //{
            //    nRadMessageBox.ShowIf(showErrors, this, $"One or more Coating Dispense Rate Advanced parameters is out of range.", 
            //        "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return true;
            //}

            //if (checkBoxSuckback.Checked && !AreAdvancedInRange(Usage.SuckbackRate))
            //{
            //    nRadMessageBox.ShowIf(showErrors, this, $"One or more Suckback Rate Advanced parameters is out of range.", 
            //        "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return true;
            //}

            // If we get down here, there were no problems!  :-)
            return false;
        }

        private void buttonConfigCancel_Click(object sender, EventArgs e)
        {
            _frmMain.ShowPrevForm();
        }

        private void FormRecipeEditor_Enter(object sender, EventArgs e)
        {
            checkBoxAirKnifeDuringReturn.Visible = MS.AirKnifeInstalled;
            checkBoxAirKnifeDuringCoat.Visible = MS.AirKnifeInstalled;
        }

        private bool IsChangedAdvanced(RecipeAdvancedEditParams advParams, Button advParamsButton)
        {
            bool isChanged = advParams.IsChanged;

            if (isChanged)
            {
                advParamsButton.BackColor = Color.Yellow;
            }
            else
            {
                advParamsButton.BackColor = SystemColors.ButtonFace;
                advParamsButton.UseVisualStyleBackColor = true;
            }

            return isChanged;
        }

        private bool IsChangedDispProfile(DispenseProfileEditorParams profileParams, Button paramsButton, bool dynamicProfile = false)
        {
            bool isChanged = profileParams.IsChanged;
            if (dynamicProfile)
                isChanged |= _dispenseProfileParamList.IsChanged(_currentRecipe.DispenseProfileParams);

            if (isChanged)
            {
                paramsButton.BackColor = Color.Yellow;
            }
            else
            {
                paramsButton.BackColor = SystemColors.ButtonFace;
                paramsButton.UseVisualStyleBackColor = true;
            }

            return isChanged;
        }
        private bool IsChanged(Button control, double paramNo, bool isParam = true, int multiplier = 1, bool divisor = false)
        {
            bool bIsChanged = false;
            double dTemp = 0, dVal = 0;
            if (isParam)
                LoadParam((int)paramNo, out dTemp);
            else
                dTemp = (double)paramNo;
            if (!divisor)
                dVal = double.Parse(control.Text) * multiplier;
            else
                dVal = double.Parse(control.Text) / multiplier;
            if (dTemp != dVal)
            {
                control.BackColor = Color.Yellow;
                bIsChanged = true;
            }
            else
            {
                control.BackColor = SystemColors.ButtonFace;
                control.UseVisualStyleBackColor = true;
            }
            return bIsChanged;
        }
        private bool IsChanged(Label control, int paramNo)
        {
            bool bIsChanged = false;
            double dTemp = 0;
            LoadParam(paramNo, out dTemp);
            if (dTemp != double.Parse(control.Text))
            {
                control.BackColor = Color.Yellow;
                bIsChanged = true;
            }
            else
            {
                control.BackColor = Color.Transparent;
            }
            return bIsChanged;
        }

        private bool IsChanged(Button control, double originalValue, double newValue)
        {
            bool bIsChanged = false;
            if (originalValue != newValue)
            {
                control.BackColor = Color.Yellow;
                bIsChanged = true;
            }
            else
            {
                control.BackColor = SystemColors.ButtonFace;
                control.UseVisualStyleBackColor = true;
            }
            return bIsChanged;
        }

        private bool IsChanged(CheckBox control, bool originalValue, bool newValue)
        {
            bool bIsChanged = false;
            if (originalValue != newValue)
            {
                control.BackColor = Color.Yellow;
                bIsChanged = true;
            }
            else
            {
                control.BackColor = Color.White;
            }
            return bIsChanged;
        }

        private bool IsChanged(CheckBox control, int paramNo)
        {
            bool bIsChanged = false;
            double dTemp = 0;
            LoadParam(paramNo, out dTemp);
            if (dTemp != (control.Checked ? 1 : 0))
            {
                control.BackColor = Color.Yellow;
                bIsChanged = true;
            }
            else
            {
                control.BackColor = Color.Transparent;
            }
            return bIsChanged;
        }

        private bool IsChanged(CheckBox control, int paramNo, int bitNo)
        {
            bool bIsChanged = false;
            double dTemp = 0;
            LoadParam(paramNo, out dTemp);
            bool state = false;
            switch(bitNo)
            {
                case 1:
                    state = ((int)dTemp & 0x1) == 0x1;
                    break;
                case 2:
                    state = ((int)dTemp & 0x2) == 0x2;
                    break;
                case 3:
                    state = ((int)dTemp & 0x4) == 0x4;
                    break;
                case 4:
                    state = ((int)dTemp & 0x8) == 0x8;
                    break;
                case 5:
                    state = ((int)dTemp & 0x10) == 0x10;
                    break;
                case 6:
                    state = ((int)dTemp & 0x20) == 0x20;
                    break;
                case 7:
                    state = ((int)dTemp & 0x40) == 0x40;
                    break;
                case 8:
                    state = ((int)dTemp & 0x80) == 0x80;
                    break;
                case 9:
                    state = ((int)dTemp & 0x100) == 0x100;
                    break;
                case 10:
                    state = ((int)dTemp & 0x200) == 0x200;
                    break;
                case 11:
                    state = ((int)dTemp & 0x400) == 0x400;
                    break;
                case 12:
                    state = ((int)dTemp & 0x800) == 0x800;
                    break;
                case 13:
                    state = ((int)dTemp & 0x1000) == 0x1000;
                    break;
                case 14:
                    state = ((int)dTemp & 0x2000) == 0x2000;
                    break;
                case 15:
                    state = ((int)dTemp & 0x4000) == 0x4000;
                    break;
                case 16:
                    state = ((int)dTemp & 0x8000) == 0x8000;
                    break;
            }

            if (state != control.Checked)
            {
                control.BackColor = Color.Yellow;
                bIsChanged = true;
            }
            else
            {
                control.BackColor = Color.Transparent;
            }
            return bIsChanged;
        }

        private bool IsChanged(CheckBox control, bool origState)
        {
            bool bIsChanged = false;
            double dTemp = 0;
            if (origState != control.Checked)
            {
                control.BackColor = Color.Yellow;
                bIsChanged = true;
            }
            else
            {
                control.BackColor = Color.Transparent;
            }

            return bIsChanged;
        }

        private bool IsChanged(Control control, int paramNo, double newValue)
        {
            bool bIsChanged = false;
            double dTemp = 0;
            LoadParam(paramNo, out dTemp);

            if (dTemp != newValue)
            {
                control.Visible = true;
                bIsChanged = true;
            }
            else
            {
                control.Visible = false;
            }

            return bIsChanged;
        }

        /// <summary>
        /// Compare original value and new value. Highlight control if differenct
        /// </summary>
        /// <param name="control"></param>
        /// <param name="origValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private bool IsChanged(Control control, string origValue, string newValue)
        {
            bool bIsChanged = false;

            if (origValue != newValue)
            {
                control.Visible = true;
                //control.BackColor = Color.Yellow;
                bIsChanged = true;
            }
            else
            {
                //control.BackColor = Color.Transparent;
                control.Visible = false;
            }

            return bIsChanged;
        }

        private bool ChangesExist()
        {
            if (_loading)
            {
                return false;
            }

            double dTemp = 0;
            bool bIsDirty = false;
            int timeMult = MS.DisplayedTimeUnits == "sec" ? 1000 : 1;

            //Priming Parameters Parameter Checks
            bIsDirty |= IsChanged(buttonPLE, 25); //Leading Edge Position
            bIsDirty |= IsChanged(buttonPrimingDispenseOff, 29); //Dispense Stop Position
            bIsDirty |= IsChanged(buttonEOP, 106); //End of Prime Position
            bIsDirty |= IsChanged(buttonPrimingGap, 100); //Priming Gap
            bIsDirty |= IsChanged(buttonPrimingVelocity, 28); //Priming Shuttle Velocity
            bIsDirty |= IsChangedAdvanced(_primingShuttleSpeedParams, buttonPrimingVelAdv);

            //Priming Dispensing Parameter Checks
            bIsDirty |= IsChanged(buttonPrimingRate, 26); //Priming Dispense Rate
            bIsDirty |= IsChanged(buttonPrimingDelay, 27, true, timeMult); //Priming Dispense Delay

            //Transition Parameter Checks
            bIsDirty |= IsChanged(buttonJumpHeight, 115); //Transition Jump Height
            bIsDirty |= IsChanged(buttonJumpVelocity, 121); //Transition Shuttle Velocity
            bIsDirty |= IsChanged(buttonTransitionDispenseRate, 125); //Transition Dispense Rate
            bIsDirty |= IsChangedAdvanced(_transitionJumpVelocityParams, buttonTranJumpVelAdv);

            //Coating Parameters Parameter Checks
            bIsDirty |= IsChanged(buttonCLE, 30); //Coating Leading Edge Position
            bIsDirty |= IsChanged(buttonCoatingDispenseOff, 35); //Coating Dispense Stop Offset
            bIsDirty |= IsChanged(buttonEOC, 34); //Coating Leading Edge Position
            bIsDirty |= IsChanged(buttonCoatingGap, 136); //Coating Gap
            bIsDirty |= IsChanged(buttonCoatingVelocity, 144); //Coating Shuttle Velocity
            bIsDirty |= IsChangedAdvanced(_coatingVelocityParams, buttonCoatingVelocityAdv); //Coating Shuttle Velocity Advanced

            if (MS.KeyenceLaserInstalled && MS.UsingKeyenceLaser)
            {
                bIsDirty |= IsChanged(labelLaserProgramChanged, _currentRecipe.KeyenceProgramNumber.ToString("#0"), comboBoxKeyenceProgramNumber.Text);
                bIsDirty |= IsChanged(checkBoxUseLasers, _currentRecipe.UsingKeyenceLaser);
            }

            if (MS.CognexCommunicationsUsed)
            {
                bIsDirty |= IsChanged(checkBoxVerifyVision, _currentRecipe.VerifyFiducials);
            }

            //Coating Dispensing Parameter Checks
            bIsDirty |= IsChanged(buttonCoatingRate, 32); //Coating Dispnese Rate
            bIsDirty |= IsChangedAdvanced(_dispenseRateParams, buttonDispenseAdv); //Coating Dispense Rate Advanced
            bIsDirty |= IsChanged(buttonCoatingDelay, 33, true, timeMult); //Coating Dispnese Delay
            bIsDirty |= IsChanged(buttonRechargeRate, 157); //Coating Recharge Rate
            bIsDirty |= IsChanged(buttonPostRechargeDelay, 158, true, timeMult); //Coating Post Recharge Delay

            //Coating Suckback Parameter Checks
            bIsDirty |= IsChanged(checkBoxSuckback, 170); //Coating Suckback Enabled
            bIsDirty |= IsChanged(buttonSuckbackOffset, 171); //Coating Suckback Offset
            bIsDirty |= IsChanged(buttonSuckbackVol, 176); //Coating Suckback Volume
            bIsDirty |= IsChanged(buttonSuckbackRate, 172); //Coating Suckback Rate
            bIsDirty |= IsChangedAdvanced(_suckbackRateParams, buttonSuckbackRateAdv); //Coating Suckback Rate Advanced

            //Coating Offsets Parameter Checks
            bIsDirty |= IsChanged(checkBoxCoatingOffset, 130); //Coating Leading Edge Offset Enabled
            bIsDirty |= IsChanged(buttonLEOGap, 131); //Coating Leading Edge Offset Height
            bIsDirty |= IsChanged(buttonLEODelay, 37, true, timeMult); //Coating Leading Edge Offset Delay
            bIsDirty |= IsChanged(checkBoxTEO, 150); //Coating Trailing Edge Offset Enabled
            bIsDirty |= IsChanged(buttonTEOGap, 152); //Coating Trailing Edge Offset Height
            bIsDirty |= IsChanged(buttonTEOOffset, 151); //Coating Trailing Edge Offset Distance From EOC

            if (MS.AirKnifeInstalled)
            {
                bIsDirty |= IsChanged(buttonAirKnifeCoatingStartOffset, 76); //Air Knife Start Offset Distance From Coat Start
                bIsDirty |= IsChanged(buttonAirKnifePostCoatDistance, 77); //Air Knife Post Coat Distance After EOC
                bIsDirty |= IsChanged(buttonAirKnifePostCoatDelay, 79);
                bIsDirty |= IsChanged(buttonAirKnifeShuttleVel, 80);
                bIsDirty |= IsChanged(checkBoxAirKnifeAfterCoating, 81);

                LoadParam(78, out var AKOpt);
                bIsDirty |= IsChanged(checkBoxAirKnifeDuringReturn, 0x1 == ((int)AKOpt & 0x1));
                bIsDirty |= IsChanged(checkBoxAirKnifeDuringCoat, 0x2 == ((int)AKOpt & 0x2));
            }

            if (HasIR)
            {
                bIsDirty |= IsChanged(buttonIRCoatingStartOffset, 196);
                bIsDirty |= IsChanged(buttonIRPostCoatDistance, 197);

                LoadParam(198, out dTemp);
                bIsDirty |= IsChanged(checkBoxUseIRDuringCoating, 0x1 == ((int)dTemp & 0x1));
                bIsDirty |= IsChanged(checkBoxUseIRDuringReturn, 0x2 == ((int)dTemp & 0x2));
                bIsDirty |= IsChanged(checkBoxIROnEndOfPrime, 0x4 == ((int)dTemp & 0x4));
                
                bIsDirty |= IsChanged(buttonIRDelayOnReturn, 199);
                bIsDirty |= IsChanged(buttonIRPowerLevelCoating, _currentRecipe.IRPowerLevelDuringCoating, false);
                bIsDirty |= IsChanged(buttonIRPowerLevelReturn, _currentRecipe.IRPowerLevelOnReturn, false);
            }

            //Post Parameter Checks
            bIsDirty |= IsChanged(buttonReturnVelocity, 166); //Post Return Shuttle Velocity
            bIsDirty |= IsChanged(checkBoxReleaseVacOnCompletion, _currentRecipe.ReleaseVacuumOnCompletion);
            bIsDirty |= IsChanged(checkBoxUnloadSubstrateOnCompletion, _currentRecipe.UnloadSubstrateOnCompletion);
            bIsDirty |= IsChanged(checkBoxReleaseVacAtEOC, 185);

            //Mixing Parameter Checks
            if(MS.DualPumpInstalled)
            {
                bIsDirty |= IsChanged(labelSelectedPump, 110, comboBoxSelectedPump.SelectedIndex);
                bIsDirty |= IsChanged(buttonPumpARatio, 111, true, 100,true);
                bIsDirty |= IsChanged(buttonPumpBRatio, 112, true, 100,true);
            }

            if (panelDispinseProfileType.Visible)
            {
                bIsDirty |= IsChanged(labelDispenseProfileChanged, 126, comboBoxDispenseProfileType.SelectedIndex);
                switch(comboBoxDispenseProfileType.SelectedIndex)
                {
                    case 0:
                        break;
                    case 1:
                        bIsDirty |= IsChangedDispProfile(_dispenseProfileEditorParams, buttonEditDispenseProfile);
                        break;
                    case 2:
                        bIsDirty |= IsChangedDispProfile(_dispenseProfileEditorParams, buttonEditDispenseProfile);
                        break;
                    case 3:
                        bIsDirty |= IsChangedDispProfile(_dispenseProfileEditorParams, buttonEditDispenseProfile);
                        break;
                    case 4:
                        bIsDirty |= IsChangedDispProfile(_dispenseProfileEditorParams, buttonEditDispenseProfile,true);
                        break;
                }
            }

            if (_hasChanges != bIsDirty)
            {
                _hasChanges = bIsDirty;

                if (bIsDirty)
                {
                    _frmMain.SetConfirmLeave("Are you sure you wish to exit?  Edits have not been saved.", "Recipe Exits");
                }
                else
                {
                    _frmMain.CancelConfirmLeave();
                }
            }

            return bIsDirty;
        }

        private void buttonPLE_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priming Leading Edge (mm)", this, buttonPLE, "0.###", MS.PrimingPlateStart, MS.PrimingPlateEnd);
        }

        private void buttonPrimingGap_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priming Gap (µm)", this, buttonPrimingGap, "0.###", MS.MinimumGap, 5000);
        }

        private void buttonPrimingVelocity_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priming Shuttle Velocity (mm/s)", this, buttonPrimingVelocity, "0.###", 0, 100);
        }

        private void buttonJumpHeight_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Jump Height (µm)", this, buttonJumpHeight, "0", 0, MS.MaxZTravel * 1000, "", false);
        }

        private void buttonPrimingRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priming Dispense Rate (µL/s)", this, buttonPrimingRate, "0.###", 0, MS.MaxPumpRate);
        }

        private void buttonPrimingDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen($"Priming Dispense Delay ({(MS.DisplayedTimeUnits == "ms" ? "msec" : "sec")})", this, buttonPrimingDelay, "0", 0, Int16.MaxValue, "", MS.DisplayedTimeUnits == "sec");
        }

        private void buttonPrimingDispenseOff_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priming Dispense Stop Offset (mm)", this, buttonPrimingDispenseOff, "0.###", 0, MS.PrimingPlateEnd);
        }

        private void buttonEOP_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("End of Prime Position (mm)", this, buttonEOP, "0.###", MS.PrimingPlateStart, MS.PrimingPlateEnd);
        }

        private void buttonCLE_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Coating Leading Edge Position (mm)", this, buttonCLE, "0.###", MS.ChuckStart, MS.ChuckEnd);
        }

        private void checkBoxCoatingOffset_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxCoatingOffset.Enabled = checkBoxCoatingOffset.Checked;
        }

        private void buttonLEOGap_Click(object sender, EventArgs e)
        {
            int max = int.Parse(buttonCoatingGap.Text);
            _frmMain.GotoNumScreen("Leading Edge Offset Gap (µm)", this, buttonLEOGap, "0", 0, max, "", false);
        }

        private void buttonLEODelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen($"Leading Edge Offset Delay ({(MS.DisplayedTimeUnits == "ms" ? "msec" : "sec")})", this, buttonLEODelay, "0", 0, double.MaxValue, "", MS.DisplayedTimeUnits == "sec");
        }

        private void buttonCoatingGap_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Coating Gap (µm)", this, buttonCoatingGap, "0", MS.MinimumGap, 5000, "", false);
        }

        private void buttonCoatingRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Coating Dispense Rate (µL/s)", this, buttonCoatingRate, "0.###", 0.01, MS.MaxPumpRate);
        }

        private void buttonDispenseAdv_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _dispenseRateParams, MC.PumpMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void buttonCoatingDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen($"Coating Dispense Delay ({(MS.DisplayedTimeUnits == "ms" ? "msec" : "sec")})", this, buttonCoatingDelay, "0", 0, Int16.MaxValue, "", MS.DisplayedTimeUnits == "sec");
        }

        private void buttonCoatingVelocity_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Coating Velocity (mm/s)", this, buttonCoatingVelocity, "0.###", 0, 100);
        }

        private void buttonCoatingVelocityAdv_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _coatingVelocityParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void checkBoxTEO_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxTEO.Enabled = checkBoxTEO.Checked;
        }

        private void buttonTEOGap_Click(object sender, EventArgs e)
        {
            int max = int.Parse(buttonCoatingGap.Text);
            _frmMain.GotoNumScreen("Trailing Edge Offset Gap (µm)", this, buttonTEOGap, "0", 0, max, "", false);
        }

        private void buttonTEOOffset_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Trailing Edge Offset (mm)", this, buttonTEOOffset, "0.###", 0, MS.MaxXTravel);
        }

        private void buttonCoatingDispenseOff_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Coating Dispense Off Offset (mm)", this, buttonCoatingDispenseOff, "0.###", 0, MS.MaxXTravel);
        }

        private void buttonEOC_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("End of Coating Position (mm)", this, buttonEOC, "0.###", MS.ChuckStart, MS.ChuckEnd);
        }

        private void checkBoxSuckback_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxSuckback.Enabled = checkBoxSuckback.Checked;
        }

        private void buttonSuckbackOffset_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Coating Dispense Off Offset (mm)", this, buttonSuckbackOffset, "0.###", 0, MS.MaxXTravel);
        }

        private void buttonSuckbackVol_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Suckback Volume (µL)", this, buttonSuckbackVol, "0.###", 0, MS.SyringeVol * 1000);
        }

        private void buttonSuckbackRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Suckback Rate (µL/s)", this, buttonSuckbackRate, "0.###", 0, MS.MaxPumpRate);
        }

        private void buttonSuckbackRateAdv_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _suckbackRateParams, MC.PumpMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void buttonRechargeRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Recharge Rate (µL)", this, buttonRechargeRate, "0.###", 0, MS.MaxPumpRate);
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
        }

        private void labelError_Click(object sender, EventArgs e)
        {
            RecipeHasErrors(true);
        }

        private void buttonPrimingVelAdv_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _primingShuttleSpeedParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void buttonTranJumpVelAdv_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _transitionJumpVelocityParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void buttonJumpVelocity_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Transition Jump Velocity (mm/s)", this, buttonJumpVelocity, "0.###", 0, 500);
        }

        private void buttonPostRechargeDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen($"Post Recharge Delay ({(MS.DisplayedTimeUnits == "ms" ? "msec" : "sec")})", this, buttonPostRechargeDelay, "0", 0, 180000, "", MS.DisplayedTimeUnits == "sec");
        }

        private void buttonReturnVelocity_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Carriage Return Velocity (mm/s)", this, buttonReturnVelocity, "0.###", 0.1, 100);
        }

        private void buttonKeyboard4Name_Click(object sender, EventArgs e)
        {
            _frmMain.GotoTextScreen("Recipe Name", this, textBoxRecipeName, 128);
        }

        private void buttonTransitionDispenseRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Transition Dispense Rate (µL/s)", this, buttonTransitionDispenseRate, "0.###", 0, MS.MaxPumpRate);
        }

        private void buttonPumpARatio_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Pump-A Dispense Ratio % (0.001 - 99.999)", this, buttonPumpARatio, "#0.000", 0.000, 100.000);
        }

        private void buttonPumpARatio_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            double dPumpARatio = Convert.ToDouble(buttonPumpARatio.Text);
            buttonPumpBRatio.Text = $"{100.000 - dPumpARatio:#0.000}";
        }

        private void comboBoxSelectedPump_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSelectedPump.SelectedIndex == 0)
            {
                buttonPumpARatio.Enabled = false;
                buttonPumpARatio.Text = "100.000";
                buttonPumpBRatio.Text = "0.000";
            }
            else if (comboBoxSelectedPump.SelectedIndex == 1)
            {
                buttonPumpARatio.Enabled = false;
                buttonPumpARatio.Text = "0.000";
                buttonPumpBRatio.Text = "100.000";
            }
            else if (comboBoxSelectedPump.SelectedIndex == 2)
            {
                buttonPumpARatio.Enabled = true;
                buttonPumpARatio.Text = "50.000";
                buttonPumpBRatio.Text = "50.000";
            }
        }

        private void checkBoxReleaseVacOnCompletion_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxReleaseVacOnCompletion.BackColor = checkBoxReleaseVacOnCompletion.Checked != _currentRecipe.ReleaseVacuumOnCompletion ? Color.Yellow : Color.FromName("Control");
        }

        private void buttonRaiseZHeight_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Raise Z Height (mm)", this, buttonRaiseZHeight, "0.###", 0, MS.MaxZTravel);
        }

        private void buttonRaiseZSpeed_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Raise Z Velocity (mm/s)", this, buttonRaiseZSpeed, "0.###", 0, 500);
        }

        private void buttonXMoveToPLE_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priming Leading Edge Velocity (mm/s)", this, buttonXMoveToPLE, "0.###", 1, 100);
        }

        private void buttonZMoveToPrimingGapHeightSpeed_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priming Gap Height Velocity (mm/s)", this, buttonZMoveToPrimingGapHeightSpeed, "0.###", 0, 500);
        }

        private void buttonAdvRaiseZSpeed_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _raiseZSpeedParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void buttonAdvXMoveToPLE_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _XMoveToPLESpeedParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void buttonAdvZMoveToPrimingGapHeightSpeed_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _ZMoveToPrimingGapHeightSpeedParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void buttonZMoveForJump_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Z Jump Velocity (mm/s)", this, buttonZMoveForJump, "0.###", 0, 500);
        }

        private void buttonZMoveToCoatingOffset_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Z Coating Offset Velocity (mm/s)", this, buttonZMoveToCoatingOffset, "0.###", 0, 500);
        }

        private void buttonZMoveToCoatingGap_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Z Coating Gap Velocity (mm/s)", this, buttonZMoveToCoatingGap, "0.###", 0, 500);
        }

        private void buttonZMoveToTrailingOffsetGap_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Z Trailing Offset Gap Velocity (mm/s)", this, buttonZMoveToTrailingOffsetGap, "0.###", 0, 500);
        }

        private void buttonAdvZMoveForJump_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _ZMoveForJumpParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void buttonAdvZMoveToCoatingOffset_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _ZMoveToCoatingOffsetParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void buttonAdvZMoveToCoatingGap_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _ZMoveToCoatingGapParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void buttonAdvZMoveToTrailingOffsetGap_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _ZMoveToTrailingOffsetGapParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void buttonEndOfCoatHeight_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("End of Coat Height (mm)", this, buttonEndOfCoatHeight, "0.###", 0, MS.MaxZTravel);
        }

        private void buttonEndOfCoatSpeed_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("End Of Coat Velocity (mm/s)", this, buttonEndOfCoatSpeed, "0.###", 0, 500);
        }

        private void buttonXHomePosition_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("X Home Position (mm)", this, buttonXHomePosition, "0.###", 0, MS.MaxXTravel);
        }

        private void buttonAirKnifeDistancePastEOC_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Air Knife Distance Past End Of Coat (mm)", this, buttonAirKnifeDistancePastEOC, "0.###", 0, 100);
        }

        private void buttonAdvEndOfCoatSpeed_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _endOfCoatSpeedParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void buttonAdvXHomePosition_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _XHomePositionParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void SetupAdvancedParamSets()
        {
            if (_primingShuttleSpeedParams != null)
            {
                return;
            }

            //TODO: Get these from the actual recipe or template, not hardcoded...

            _primingShuttleSpeedParams = new RecipeAdvancedEditParams()
            {
                Title = "Priming Shuttle Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
            };

            _dispenseRateParams = new RecipeAdvancedEditParams()
            {
                Title = "Coating Dispense Rate",
                Units = "µl/s²",
                MinVal = 1 * MC.MinPumpAccel,
                MaxVal = 1000 * MC.MinPumpAccel
            };

            _coatingVelocityParams = new RecipeAdvancedEditParams()
            {
                Title = "Coating Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
            };

            _suckbackRateParams = new RecipeAdvancedEditParams()
            {
                Title = "Suckback Rate",
                Units = "µl/s²",
                MinVal = 1 * MC.MinPumpAccel,
                MaxVal = 1000 * MC.MinPumpAccel
            };

            _transitionJumpVelocityParams = new RecipeAdvancedEditParams()
            {
                Title = "Transition Jump Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
            };

            _raiseZSpeedParams = new RecipeAdvancedEditParams()
            {
                Title = "Raise Z Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
            };

            _XMoveToPLESpeedParams = new RecipeAdvancedEditParams()
            {
                Title = "X Move to PLE Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
            };

            _ZMoveToPrimingGapHeightSpeedParams = new RecipeAdvancedEditParams()
            {
                Title = "Z Move to Priming Gap Height Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
            };

            _ZMoveForJumpParams = new RecipeAdvancedEditParams()
            {
                Title = "Z Move For Jump Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
            };

            _ZMoveToCoatingOffsetParams = new RecipeAdvancedEditParams()
            {
                Title = "Move to Coating Offset Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
            };

            _ZMoveToCoatingGapParams = new RecipeAdvancedEditParams()
            {
                Title = "Move to Coating Gap Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
            };

            _ZMoveToTrailingOffsetGapParams = new RecipeAdvancedEditParams()
            {
                Title = "Z Move to Trailing Offset Gap Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
            };

            _endOfCoatSpeedParams = new RecipeAdvancedEditParams()
            {
                Title = "End of Coat Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
            };

            _XHomePositionParams = new RecipeAdvancedEditParams()
            {
                Title = "X Move Return To Home Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
            };
        }

        private void buttonAirKnifeCoatingStartOffset_Click(object sender, EventArgs e)
        {
            double CLE = double.Parse(buttonCLE.Text);
            double EOC = double.Parse(buttonEOC.Text);
            double maxAllowedOffset = EOC - CLE;
            _frmMain.GotoNumScreen("Air-Knife Start Offset (mm)", this, buttonAirKnifeCoatingStartOffset, "#0.0", MS.DieToAirKnifePitch * -1, 100);
        }

        private void buttonAirKnifePostCoatDistance_Click(object sender, EventArgs e)
        {
            double EOC = double.Parse(buttonEOC.Text);
            double maxAllowedOffset = MS.MaxXTravel - EOC - MS.DieToAirKnifePitch;
            _frmMain.GotoNumScreen("Air-Knife Post-Coat Distance (mm)", this, buttonAirKnifePostCoatDistance, "#0.0", 0,maxAllowedOffset );
        }

        private void buttonAirKnifePostCoatDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Air Knife Post-Coat Delay (ms):", this, buttonAirKnifePostCoatDelay, "#0", 0, 10000, "", false);
        }

        private void buttonAirKnifeShuttleVel_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Air Knife Shuttle Velocity (mm/sec)", this, buttonAirKnifeShuttleVel, "0.0", 0.0, 250.0, "", true);
        }

        private void buttonIRPowerLevelCoating_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("IR Power Level During Coating (%):", this, buttonIRPowerLevelCoating, "#0", 0, 100, "", false);
        }

        private void buttonIRPowerLevelReturn_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("IR Power Level During Return (%):", this, buttonIRPowerLevelReturn, "#0", 0, 100, "", false);
        }

        private void buttonIRCoatingStartOffset_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("IR Coating Start Offset (mm):", this, buttonIRCoatingStartOffset, "0.0", MS.DieToIRPitch*-1, 100, "", true);
        }

        private void buttonIRPostCoatDistance_Click(object sender, EventArgs e)
        {
            double EOC = double.Parse(buttonEOC.Text);
            double maxAllowedOffset = MS.MaxXTravel - EOC - MS.DieToIRPitch;
            _frmMain.GotoNumScreen("IR Post-Coat Distance (mm):", this, buttonIRPostCoatDistance, "0.0", 0.0, maxAllowedOffset, "", true);
        }

        private void buttonIRDelayOnReturn_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("IR Delay At Return", this, buttonIRDelayOnReturn, "0", 0, 30000);
        }

        private void buttonEditDispenseProfile_Click(object sender, EventArgs e)
        {
            double xmax = double.Parse(buttonEOC.Text) - double.Parse(buttonCLE.Text);
            var subForm = new FormDispenseProfileEditor(_frmMain, this, comboBoxDispenseProfileType.SelectedIndex,_dispenseProfileParamList, double.Parse(buttonCoatingRate.Text),xmax, _dispenseProfileEditorParams);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(subForm);
        }
    }
}

