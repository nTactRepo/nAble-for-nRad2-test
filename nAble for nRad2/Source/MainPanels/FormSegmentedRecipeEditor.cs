using nAble.Data;
using nAble.Model.Recipes;
using nAble.Utils;
using nTact.DataComm;
using nTact.Recipes;
using Support2.RegistryClasses;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormSegmentedRecipeEditor : Form, IUpdateableForm
    {
        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;
        private readonly IRecipeManager _recipeMgr = null;
        
        private bool _oneShot = false;
        private bool _loading = false;
        private bool _editingDefault = false;
        private bool _hasChanges = false;

        private Recipe _currentRecipe = null;
        private Recipe _defaultRecipe = null;

        private TabPage _tabAirKnife = null;
        private TabPage _tabIR = null;
        private TabPage _tabPageDefaults = null;
        private TabPage _tabPriming = null;

        private RecipeAdvancedEditParams _primingShuttleSpeedParams;
        private RecipeAdvancedEditParams _dispenseRateParams;
        private RecipeAdvancedEditParams _coatingVelocityParams;
        private RecipeAdvancedEditParams _zStageVelocityParams;
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

        #region Properties

        private MachineSettingsII MS => _frmMain.MS;
        private GalilWrapper2 MC => _frmMain.MC;
        private bool HasIR => MS.IRLampInstalled;
        private bool HasAirKnife => MS.AirKnifeInstalled;

        #endregion

        public FormSegmentedRecipeEditor(FormMain formMain, LogEntry log, IRecipeManager recipeManager)
        {
            InitializeComponent();

            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _log = log;
            _recipeMgr = recipeManager ?? throw new ArgumentNullException(nameof(recipeManager));

            _tabPageDefaults = tabPageDefaults;
            _tabAirKnife = tabPageAirKnife;
            _tabIR = tabPageIR;
            _tabPriming = tabPagePriming;
        }

        private void FormSegmentedRecipeEditor_Load(object sender, EventArgs e)
        {
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
            checkBoxAirKnifeDuringReturn.Visible = MS.AirKnifeInstalled;
            checkBoxAirKnifeDuringCoat.Visible = MS.AirKnifeInstalled;
            checkBoxUnloadSubstrateOnCompletion.Visible = MS.AutoLDULD || MS.HasLiftAndCenter || MS.LiftPinsEnabled;
            panelKeyence.Visible = MS.UsingKeyenceLaser;
            checkBoxUseLasers.Visible = MS.UsingKeyenceLaser;
            checkBoxVerifyVision.Visible = MS.CognexCommunicationsUsed;

            labelPrimingDelayUnits.Text = MS.DisplayedTimeUnits;
            PostRechargeDelayUnits.Text = MS.DisplayedTimeUnits;
            labelCoatingOffsetDelayUnits.Text = MS.DisplayedTimeUnits;
            labelAKDelayBeforeFinalPassUnits.Text = MS.DisplayedTimeUnits;
            labelIRDelayBeforeReturnUnits.Text = MS.DisplayedTimeUnits;

            groupBoxIRCoatingPowerLevel.Enabled = checkBoxUseIRDuringCoating.Checked;
            groupBoxIRReturnPowerLevel.Enabled = checkBoxUseIRDuringReturn.Checked;

            bool dispProfileEnabled = _frmMain.LicMgr.IsFeatureActive(LicensedFeautres.Feature2) || _frmMain.IsDemoMode || _frmMain.DebugMode;
            panelDispinseProfileType.Visible = dispProfileEnabled;
            if (dispProfileEnabled)
                buttonEditDispenseProfile.Visible = comboBoxDispenseProfileType.SelectedIndex > 0;
        }

        private void LoadAdvancedParams(int accelIndex, int decelIndex, int sCurveIndex, RecipeAdvancedEditParams advParams)
        {
            double temp = 0;
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

        private bool LoadTimeParam(int nIdx, Button button)
        {
            bool bReturn = false;
            double dTemp = 0;
            bReturn = LoadParam(nIdx, out dTemp);
            if(bReturn)
            {
                if (MS.DisplayedTimeUnits == "sec")
                {
                    button.Text = (dTemp / 1000).ToString("#0.000");
                }
                else
                    button.Text = dTemp.ToString("#0");

            }
            return bReturn;
        }

        private bool LoadTimeParam(int nIdx, out double timeVal)
        {
            bool bReturn = false;
            double dTemp = 0;
            bReturn = LoadParam(nIdx, out dTemp);
            timeVal = MS.DisplayedTimeUnits=="sec" ? dTemp / 1000 : dTemp;
            return bReturn;
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

        private bool LoadParam(int nIdx, Button button)
        {
            bool bRetVal = true;
            RecipeParam tempParam = null;
            bRetVal = _currentRecipe.GetParam(nIdx, ref tempParam);
            if (bRetVal)
            {
                try
                {
                    double dTemp = double.Parse(tempParam.Value);
                    button.Text = dTemp.ToString();
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
                        double dTemp = double.Parse(tempParam.Value);
                        button.Text = dTemp.ToString();
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

        private bool LoadParam(int nIdx, Control control, bool isCombobox = false)
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

        public bool LoadRecipe(string recipeName, bool isDefault = false)
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
                _loading = false;
                return false;
            }

            _loading = true;

            _currentRecipe = isDefault ? _defaultRecipe : _recipeMgr.FetchRecipe(recipeName);

            if (_currentRecipe != null)
            {
                comboBoxKeyenceProgramNumber.SelectedIndex = CommonUtils.ProgramNumberIsValid(_currentRecipe.KeyenceProgramNumber) ? 
                    _currentRecipe.KeyenceProgramNumber : 0;
                checkBoxUseLasers.Checked = _currentRecipe.UsingKeyenceLaser;
                checkBoxReleaseVacOnCompletion.Checked = _currentRecipe.ReleaseVacuumOnCompletion;
                checkBoxUnloadSubstrateOnCompletion.Checked = _currentRecipe.UnloadSubstrateOnCompletion;
                textBoxRecipeName.Text = _currentRecipe.Name;

                LoadParam(25, buttonPLE);

                if (MS.HasPrimingArea || MS.HasPrimingPlate)
                    LoadParam(26, buttonPrimingRate);
                else
                    buttonPrimingRate.Text = "0";

                LoadTimeParam(27, buttonPrimingDelay);

                LoadParam(28, buttonPrimingVelocity);
                LoadParam(29, buttonPrimingDispenseOff);
                LoadParam(30, buttonCLE);
                LoadParam(32, buttonCoatingDispenseRate);

                LoadTimeParam(37, buttonLEODelay);

                LoadParam(62, buttonSegmentCount);
                LoadParam(63, buttonSegmentLength);
                LoadParam(64, buttonSegmentGapLength);
                LoadParam(65, buttonDispenseOnOffset);
                LoadParam(66, buttonDispenseOffOffset);
                LoadParam(69, buttonZVelocity);
                LoadAdvancedParams(70, 71, 72, _zStageVelocityParams);
                LoadParam(73, buttonSegmentGapHeight);
                LoadParam(74, buttonGapVelocity);

                LoadParam(76, buttonAirKnifeCoatingStartOffset);
                LoadParam(77, buttonAirKnifePostCoatDistance);
                LoadParam(78, out airKnife);
                checkBoxAirKnifeDuringReturn.Checked = ((int)airKnife & 0x1) == 0x1;
                checkBoxAirKnifeDuringCoat.Checked = ((int)airKnife & 0x2) == 0x2;
                LoadTimeParam(79, buttonAirKnifePostCoatDelay);
                LoadParam(80, buttonAirKnifeShuttleVel);
                LoadParam(81, checkBoxAirKnifeAfterCoating);

                LoadAdvancedParams(85, 86, 87, _dispenseRateParams);
                LoadParam(100, buttonPrimingGap);
                LoadParam(106, buttonEOP);
                LoadAdvancedParams(107, 108, 109, _primingShuttleSpeedParams);
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

                LoadTimeParam(158, buttonPostRechargeDelay);

                LoadParam(185, checkBoxReleaseVacAtEOC);
                LoadParam(166, buttonReturnVelocity);

                if (HasIR)
                {
                    LoadParam(196, buttonIRCoatingStartOffset);
                    LoadParam(197, buttonIRPostCoatDistance);
                    double irUsage = 0;
                    LoadParam(198, out irUsage);
                    checkBoxUseIRDuringCoating.Checked = 0x1 == ((int)irUsage & 0x1);
                    checkBoxUseIRDuringReturn.Checked = 0x2 == ((int)irUsage & 0x2);
                    buttonIRPowerLevelCoating.Text = $"{_currentRecipe.IRPowerLevelDuringCoating:#0}";
                    buttonIRPowerLevelReturn.Text = $"{_currentRecipe.IRPowerLevelOnReturn:#0}";
                    LoadTimeParam(199, buttonIRDelayOnReturn);
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

                LoadParam(126, comboBoxDispenseProfileType, true);
                _dispenseProfileEditorParams = new DispenseProfileEditorParams();
                LoadDispProfileParams(127, 128, 129, _dispenseProfileEditorParams);
                _dispenseProfileParamList = new DynamicDispenseProfileParamList();
                if (_currentRecipe.DispenseProfileParams != null)
                {
                    foreach (DynamicDispenseProfileParam param in _currentRecipe.DispenseProfileParams)
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
            ShowOrHideTab(_editingDefault, _tabPageDefaults, index: 0);
            ShowOrHideTab(MS.AirKnifeInstalled, _tabAirKnife);
            ShowOrHideTab(MS.IRLampInstalled, _tabIR);
            ShowOrHideTab((MS.HasPrimingArea || MS.HasPrimingPlate), _tabPriming);
        }

        /// <summary>
        /// Shows or hides a tab correctly
        /// </summary>
        /// <param name="show">If true, shows the tab, else hides it.</param>
        /// <param name="tab">The tab in question</param>
        /// <param name="index">If -1, add at the end, else insert at the index.</param>
        private void ShowOrHideTab(bool show, TabPage tab, int index = -1)
        {
            try
            {
                if (show)
                {
                    if (!tabControlRecipe.TabPages.Contains(tab))
                    {
                        if (index == -1)
                        {
                            tabControlRecipe.TabPages.Add(tab);
                        }
                        else
                        {
                            tabControlRecipe.TabPages.Insert(index, tab);
                        }
                    }
                }
                else
                {
                    if (tabControlRecipe.TabPages.Contains(tab))
                    {
                        tabControlRecipe.TabPages.Remove(tab);
                    }
                }
            }
            catch (ArgumentOutOfRangeException aoor)
            {
                nRadMessageBox.Show(this, $"Caught an exception while modifying tab pages:  {aoor.Message}.  Please contact nTact.",
                    $"Fatal Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateAdvancedParams(int startIndex, RecipeAdvancedEditParams advParams)
        {
            UpdateParam(startIndex, advParams.Accel);
            UpdateParam(startIndex + 1, advParams.Decel);
            UpdateParam(startIndex + 2, advParams.SCurve);
        }

        private bool UpdateParam(int nIdx, bool bNewValue)
        {
            return UpdateParam(nIdx, bNewValue ? 1 : 0);
        }

        private bool UpdateTimeParam(int nIdx, double dNewValue)
        {
            bool bRetVal = true;

            double dVal = MS.DisplayedTimeUnits == "sec" ? dNewValue * 1000 : dNewValue;
            bRetVal = UpdateParam(nIdx, dVal);

            return bRetVal;
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
            bool saved = true;

            if (_currentRecipe != null)
            {
                _currentRecipe.Name = textBoxRecipeName.Text;
                _currentRecipe.LastUpdate = DateTime.Now;
                _currentRecipe.LastUpdatedBy = string.Format("{0}/{1} on machine {2}", Environment.UserDomainName, Environment.UserName, Environment.MachineName);
                _currentRecipe.UsingKeyenceLaser = !MS.UsingKeyenceLaser && checkBoxUseLasers.Checked ? false : checkBoxUseLasers.Checked;
                _currentRecipe.KeyenceProgramNumber = MS.UsingKeyenceLaser ? comboBoxKeyenceProgramNumber.SelectedIndex : 0;
                _currentRecipe.VerifyFiducials = MS.CognexCommunicationsUsed ? checkBoxVerifyVision.Checked : false;
                _currentRecipe.ReleaseVacuumOnCompletion = checkBoxReleaseVacOnCompletion.Checked;
                _currentRecipe.UnloadSubstrateOnCompletion = (MS.LiftPinsEnabled || MS.HasLiftAndCenter) ? checkBoxUnloadSubstrateOnCompletion.Checked : false;
                
                try
                {
                    UpdateParam(25, double.Parse(buttonPLE.Text));
                    UpdateParam(26, double.Parse(buttonPrimingRate.Text));
                    UpdateTimeParam(27, double.Parse(buttonPrimingDelay.Text));
                    UpdateParam(28, double.Parse(buttonPrimingVelocity.Text));
                    UpdateParam(29, double.Parse(buttonPrimingDispenseOff.Text));
                    UpdateParam(30, double.Parse(buttonCLE.Text));
                    UpdateParam(32, double.Parse(buttonCoatingDispenseRate.Text));
                    UpdateTimeParam(37, double.Parse(buttonLEODelay.Text));


                    UpdateParam(62, double.Parse(buttonSegmentCount.Text));
                    UpdateParam(63, double.Parse(buttonSegmentLength.Text));
                    UpdateParam(64, double.Parse(buttonSegmentGapLength.Text));
                    UpdateParam(65, double.Parse(buttonDispenseOnOffset.Text));
                    UpdateParam(66, double.Parse(buttonDispenseOffOffset.Text));
                    UpdateParam(69, double.Parse(buttonZVelocity.Text));
                    UpdateAdvancedParams(70, _zStageVelocityParams);
                    UpdateParam(73, double.Parse(buttonSegmentGapHeight.Text));
                    UpdateParam(74, double.Parse(buttonGapVelocity.Text));

                    //Air Knife
                    UpdateParam(76, double.Parse(buttonAirKnifeCoatingStartOffset.Text));
                    UpdateParam(77, double.Parse(buttonAirKnifePostCoatDistance.Text));

                    double dAirKnife = 0;
                    dAirKnife += checkBoxAirKnifeDuringReturn.Checked ? 1 : 0;
                    dAirKnife += checkBoxAirKnifeDuringCoat.Checked ? 2 : 0;
                    UpdateParam(78, dAirKnife);

                    UpdateTimeParam(79, double.Parse(buttonAirKnifePostCoatDelay.Text));
                    UpdateParam(80, double.Parse(buttonAirKnifeShuttleVel.Text));
                    UpdateParam(81, checkBoxAirKnifeAfterCoating.Checked);

                    UpdateAdvancedParams(85, _dispenseRateParams);
                    UpdateParam(100, double.Parse(buttonPrimingGap.Text));
                    UpdateParam(106, double.Parse(buttonEOP.Text));
                    UpdateAdvancedParams(107, _primingShuttleSpeedParams);
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
                    UpdateTimeParam(158, double.Parse(buttonPostRechargeDelay.Text));
                    UpdateParam(185, checkBoxReleaseVacAtEOC.Checked);
                    UpdateParam(166, double.Parse(buttonReturnVelocity.Text));

                    saved &= UpdateParam(193, double.Parse(buttonEOP.Text) - double.Parse(buttonPrimingDispenseOff.Text)); //End of Prime Position

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
                        UpdateTimeParam(199, double.Parse(buttonIRDelayOnReturn.Text));
                    }

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

                    saved &= _recipeMgr.SaveRecipe(_currentRecipe, _editingDefault);
                }
                catch (Exception ex)
                {
                    _log.log(LogType.TRACE, Category.ERROR, $"Exception Saving params[] To Recipe: {_currentRecipe.Name}", "", SubCategory.NONE);
                    _log.log(LogType.TRACE, Category.ERROR, $"Exception: {ex.Message}", "", SubCategory.NONE);
                    saved = false;
                }
            }

            return saved;
        }

        private void buttonConfigSave_Click(object sender, EventArgs e)
        {
            if (RecipeHasErrors(true))
            {
                _log.log(LogType.TRACE, Category.INFO, "Recipe had errors.  Save aborted");
                return;
            }

            if (DialogResult.No == nRadMessageBox.Show(this, string.Format("Save Changes to Recipe '{0}'?", _currentRecipe.Name), "Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                return;
            }

            if (Save())
            {
                nRadMessageBox.Show(this, string.Format("Recipe '{0}' successfully saved", _currentRecipe.Name), "Save Recipe", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            _frmMain.CancelConfirmLeave();
            _frmMain.ShowPrevForm();
        }

        private bool RecipeHasErrors(bool bShowErrors = false)
        {
            bool bRetVal = false;

            int nNumSegs = int.Parse(buttonSegmentCount.Text);
            double dGap = double.Parse(buttonSegmentGapLength.Text);
            double dCoating = double.Parse(buttonSegmentLength.Text);
            double dCoatingStart = double.Parse(buttonCLE.Text);
            double dPeriod = dGap + dCoating;
            double dPrimingStart = MS.PrimingPlateStart;
            double dPrimingEnd = MS.PrimingPlateEnd;
            double dChuckStart = MS.ChuckStart;
            double dChuckEnd = MS.ChuckEnd;
            double dPrimePLE = double.Parse(buttonPLE.Text);
            double dPrimeEOP = double.Parse(buttonEOP.Text);
            double dPrimeOff = double.Parse(buttonPrimingDispenseOff.Text);
            double endOfIR = double.Parse(buttonIRPostCoatDistance.Text);
            double airKnifePostDist = double.Parse(buttonAirKnifePostCoatDistance.Text);
            bool usingAirKnifeCoat = MS.AirKnifeInstalled && checkBoxAirKnifeDuringCoat.Checked;
            bool usingAirKnifeAfter = MS.AirKnifeInstalled && checkBoxAirKnifeAfterCoating.Checked;
            bool usingIR = checkBoxUseIRDuringCoating.Checked || checkBoxUseIRDuringReturn.Checked;
            bool vacOffAtEOC = checkBoxReleaseVacAtEOC.Checked;
            bool IRDuringReturn = checkBoxUseIRDuringReturn.Checked;

            if ((MS.HasPrimingArea || MS.HasPrimingPlate) && buttonPrimingRate.Text != "0")
            {
                // priming should start on the priming plate
                if (!bRetVal && !(dPrimingStart <= dPrimePLE && dPrimePLE <= dPrimingEnd))
                {
                    if (bShowErrors)
                        nRadMessageBox.Show(this, "The specified priming start position is not on the defined priming plate.", "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bRetVal = true;
                }
                // priming should end on the priming plate
                if (!bRetVal && !(dPrimingStart <= dPrimeEOP && dPrimeEOP <= dPrimingEnd))
                {
                    if (bShowErrors)
                        nRadMessageBox.Show(this, "The specified priming end position is not on the defined priming plate.", "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bRetVal = true;
                }
                // priming position order
                if (!bRetVal && (dPrimingStart >= dPrimeEOP))
                {
                    if (bShowErrors)
                        nRadMessageBox.Show(this, "The specified priming start/stop positions are out of order.", "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bRetVal = true;
                }
            }
            else
            {
                if (!bRetVal && dCoatingStart <= MS.MeasureLoc)
                {
                    if (bShowErrors)
                        nRadMessageBox.Show(this, "The specified coating start position is before the Chuck Measure Location.", "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bRetVal = true;
                }
            }
            // coating should start on the chuck
            if (!bRetVal && !(dChuckStart <= dCoatingStart && dCoatingStart <= dChuckEnd))
            {
                if (bShowErrors)
                    nRadMessageBox.Show(this, "The specified coating start position is not in the defined coating area.", "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bRetVal = true;
            }

            // make sure the segments fit on the chuck
            if (!bRetVal && (dCoatingStart + (nNumSegs * dPeriod) - dGap > dChuckEnd))
            {
                if (bShowErrors)
                    nRadMessageBox.Show(this, "The specified size and number of segments will not fit on the chuck.", "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bRetVal = true;
            }

            // make sure that the rise and decent of the chuck fits in the gap  (leadin * 2)
            double dJumpHeight = double.Parse(buttonSegmentGapHeight.Text) / 1000;
            double dXVelocity = double.Parse(buttonGapVelocity.Text);     //mm/s
            double dTime = Math.Sqrt(dJumpHeight / _zStageVelocityParams.Accel) + Math.Sqrt(dJumpHeight / _zStageVelocityParams.Decel);
            double dLeadIn = dXVelocity * dTime;

            // make sure the segments fit on the chuck
            if (!bRetVal && ((dLeadIn * 2) >= dGap))
            {
                if (bShowErrors)
                {
                    string msg = "Not enough time for the segment gap jump.  Either lower the gap " +
                        "velocity, lower the gap height, increase the gap length, or increase " +
                        "the Z stage vel/acc/dec.";
                    nRadMessageBox.Show(this, msg, "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                bRetVal = true;
            }

            return bRetVal;
        }

        private void buttonConfigCancel_Click(object sender, EventArgs e)
        {
            _frmMain.ShowPrevForm();
        }

        private void FormSegmentedRecipeEditor_Enter(object sender, EventArgs e)
        {
            checkBoxAirKnifeDuringReturn.Visible = MS.AirKnifeInstalled;
            checkBoxAirKnifeDuringCoat.Visible = MS.AirKnifeInstalled;
            panelKeyence.Visible = MS.UsingKeyenceLaser || _currentRecipe.UsingKeyenceLaser;
            checkBoxUseLasers.Visible = MS.UsingKeyenceLaser || _currentRecipe.UsingKeyenceLaser;
        }

        private bool IsChangedAdvanced(RecipeAdvancedEditParams advParams, Button button)
        {
            bool changed = advParams.IsChanged;

            if (changed)
            {
                button.BackColor = Color.Yellow;
            }
            else
            {
                button.BackColor = SystemColors.ButtonFace;
                button.UseVisualStyleBackColor = true;
            }

            return changed;
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

        private bool IsTimeChanged(Button control, int paramNo)
        {
            bool bIsChanged = false;
            double dTemp = 0;
            LoadTimeParam(paramNo, out dTemp);
            if (dTemp != double.Parse(control.Text))
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
        private bool IsChanged(Button control, double paramNo, bool isParam = true, int multiplier = 1)
        {
            bool bIsChanged = false;
            double dTemp = 0;
            if (isParam)
                LoadParam((int)paramNo, out dTemp);
            else
                dTemp = (double)paramNo;
            if (dTemp != (double.Parse(control.Text) * multiplier))
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
            switch (bitNo)
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
                bIsChanged = true;
            }
            else
            {
                control.Visible = false;
            }
            return bIsChanged;
        }
        private void ChangesExist()
        {
            if (_loading)
            {
                return;
            }

            double dTemp = 0;
            double akOpt = 0;
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
            bIsDirty |= IsTimeChanged(buttonPrimingDelay, 27); //Priming Dispense Delay

            //Transition Parameter Checks
            bIsDirty |= IsChanged(buttonJumpHeight, 115); //Transition Jump Height
            bIsDirty |= IsChanged(buttonJumpVelocity, 121); //Transition Shuttle Velocity
            bIsDirty |= IsChanged(buttonTransitionDispenseRate, 125); //Transition Dispense Rate
            bIsDirty |= IsChangedAdvanced(_transitionJumpVelocityParams, buttonTranJumpVelAdv);

            if (MS.KeyenceLaserInstalled)
            {
                bIsDirty |= IsChanged(labelLaserProgramChanged, _currentRecipe.KeyenceProgramNumber.ToString("#0"), comboBoxKeyenceProgramNumber.Text);
            }

            if (MS.CognexCommunicationsUsed)
            {
                bIsDirty |= IsChanged(checkBoxVerifyVision, _currentRecipe.VerifyFiducials);
            }

            //Coating Parameters Parameter Checks
            bIsDirty |= IsChanged(buttonCLE, 30); //Coating Leading Edge Position
            bIsDirty |= IsChanged(buttonDispenseOffOffset, 66); //Coating Dispense Stop Offset
            bIsDirty |= IsChanged(buttonCoatingGap, 136); //Coating Gap
            bIsDirty |= IsChanged(buttonCoatingVelocity, 144); //Coating Shuttle Velocity
            bIsDirty |= IsChangedAdvanced(_coatingVelocityParams, buttonCoatingVelocityAdv); //Coating Shuttle Velocity Advanced

            //Coating Dispensing Parameter Checks
            bIsDirty |= IsChanged(buttonCoatingDispenseRate, 32); //Coating Dispnese Rate
            bIsDirty |= IsChangedAdvanced(_dispenseRateParams, buttonCoatingDispenseRateAdv); //Coating Dispense Rate Advanced
            bIsDirty |= IsChanged(buttonDispenseOnOffset, 65); //Coating Dispnese On Offset
            bIsDirty |= IsChanged(buttonRechargeRate, 157); //Coating Recharge Rate
            bIsDirty |= IsTimeChanged(buttonPostRechargeDelay, 158); //Coating Post Recharge Delay

            //Coating Segment Parameter Checks
            bIsDirty |= IsChanged(buttonSegmentCount, 62); //Segment Count
            bIsDirty |= IsChanged(buttonSegmentLength, 63); //Segment Length

            //Coating Segment Gap Parameter Checks
            bIsDirty |= IsChanged(buttonSegmentGapLength, 64); //Segment Gap Length
            bIsDirty |= IsChanged(buttonGapVelocity, 74); //Segment Gap Velocity
            bIsDirty |= IsChanged(buttonSegmentGapHeight, 73); //Segment Gap Height
            bIsDirty |= IsChanged(buttonZVelocity, 69); //Segment Gap Z Velocity
            bIsDirty |= IsChangedAdvanced(_zStageVelocityParams, buttonZVelocityAdv); //Segment Gap Z Velocity Advanced

            //Coating Offsets Parameter Checks
            bIsDirty |= IsChanged(checkBoxCoatingOffset, 130); //Coating Leading Edge Offset Enabled
            bIsDirty |= IsChanged(buttonLEOGap, 131); //Coating Leading Edge Offset Height
            bIsDirty |= IsTimeChanged(buttonLEODelay,37); //Coating Leading Edge Offset Delay
            bIsDirty |= IsChanged(checkBoxTEO, 150); //Coating Trailing Edge Offset Enabled
            bIsDirty |= IsChanged(buttonTEOGap, 152); //Coating Trailing Edge Offset Height
            bIsDirty |= IsChanged(buttonTEOOffset, 151); //Coating Trailing Edge Offset Distance From EOC

            if (MS.AirKnifeInstalled)
            {
                bIsDirty |= IsChanged(buttonAirKnifeCoatingStartOffset, 76); //Air Knife Start Offset Distance From Coat Start
                bIsDirty |= IsChanged(buttonAirKnifePostCoatDistance, 77); //Air Knife Post Coat Distance After EOC
                bIsDirty |= IsTimeChanged(buttonAirKnifePostCoatDelay, 79);
                bIsDirty |= IsChanged(buttonAirKnifeShuttleVel, 80);
                bIsDirty |= IsChanged(checkBoxAirKnifeAfterCoating, 81);

                LoadParam(78, out akOpt);
                bIsDirty |= IsChanged(checkBoxAirKnifeDuringReturn, 0x1 == ((int)akOpt & 0x1));
                bIsDirty |= IsChanged(checkBoxAirKnifeDuringCoat, 0x2 == ((int)akOpt & 0x2));
            }

            if (HasIR)
            {
                bIsDirty |= IsChanged(buttonIRCoatingStartOffset, 196);
                bIsDirty |= IsChanged(buttonIRPostCoatDistance, 197);

                LoadParam(198, out dTemp);
                bIsDirty |= IsChanged(checkBoxUseIRDuringCoating, 0x1 == ((int)dTemp & 0x1));
                bIsDirty |= IsChanged(checkBoxUseIRDuringReturn, 0x2 == ((int)dTemp & 0x2));
                bIsDirty |= IsChanged(checkBoxIROnEndOfPrime, 0x4 == ((int)dTemp & 0x4));

                bIsDirty |= IsTimeChanged(buttonIRDelayOnReturn, 199);
                bIsDirty |= IsChanged(buttonIRPowerLevelCoating, _currentRecipe.IRPowerLevelDuringCoating, false);
                bIsDirty |= IsChanged(buttonIRPowerLevelReturn, _currentRecipe.IRPowerLevelOnReturn, false);
            }

            //Post Parameter Checks
            bIsDirty |= IsChanged(buttonReturnVelocity, 166); //Post Return Shuttle Velocity
            bIsDirty |= IsChanged(checkBoxReleaseVacOnCompletion, _currentRecipe.ReleaseVacuumOnCompletion);
            bIsDirty |= IsChanged(checkBoxUnloadSubstrateOnCompletion, _currentRecipe.UnloadSubstrateOnCompletion);
            bIsDirty |= IsChanged(checkBoxReleaseVacAtEOC, 185);

            if (panelDispinseProfileType.Visible)
            {
                bIsDirty |= IsChanged(labelDispenseProfileChanged, 126, comboBoxDispenseProfileType.SelectedIndex);
                switch (comboBoxDispenseProfileType.SelectedIndex)
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
                        bIsDirty |= IsChangedDispProfile(_dispenseProfileEditorParams, buttonEditDispenseProfile, true);
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
        }

        #region Buttons
        private void buttonPLE_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priming Leading Edge (mm)", this, buttonPLE, "#0.0", MS.PrimingPlateStart, MS.PrimingPlateEnd);
        }
        private void buttonPrimingGap_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priming Gap (µm)", this, buttonPrimingGap, "#0.000", MS.MinimumGap, 5000);
        }
        private void buttonPrimingVelocity_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priming Shuttle Velocity (mm/s)", this, buttonPrimingVelocity, "#0.0", 0, 100);
        }
        private void buttonPrimingRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priming Dispense Rate (µL/s)", this, buttonPrimingRate, "#0.000", 0, MS.MaxPumpRate);
        }
        private void buttonPrimingDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen($"Priming Dispense Delay ({(MS.DisplayedTimeUnits == "sec" ? "sec" : "msec")})", this, buttonPrimingDelay, MS.DisplayedTimeUnits == "sec" ? "#0.000" : "#0", 0, Int16.MaxValue, "", MS.DisplayedTimeUnits == "sec");
        }
        private void buttonPrimingDispenseOff_Click(object sender, EventArgs e)
        {
            double EOP = double.Parse(buttonEOP.Text);
            _frmMain.GotoNumScreen("Priming Dispense Stop Offset (mm)", this, buttonPrimingDispenseOff, "#0.0", 0, EOP);
        }
        private void buttonEOP_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("End of Prime Position (mm)", this, buttonEOP, "#0.0", MS.PrimingPlateStart, MS.PrimingPlateEnd);
        }
        //===== Coating Segments ================================================================================//
        private void buttonCoatingGap_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Coating Gap (µm)", this, buttonCoatingGap, "0", MS.MinimumGap, 5000, "", false);
        }
        private void buttonCoatingVelocity_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Coating Velocity (mm/s)", this, buttonCoatingVelocity, "#0.0", 0, 100);
        }
        private void buttonCoatingVelocityAdv_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _coatingVelocityParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }
        private void buttonCoatingDispenseRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Coating Dispense Rate (µL/s)", this, buttonCoatingDispenseRate, "0.###", 0, MS.MaxPumpRate);
        }
        private void buttonCoatingDispenseRateAdv_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _dispenseRateParams, MC.PumpMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }

        private void buttonSegmentCount_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Number of segments", this, buttonSegmentCount, "0", 1, 100, "", false);
        }
        private void buttonSegmentLength_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Segment Length (mm)", this, buttonSegmentLength, "0.###", 10, MS.MaxXTravel);
        }
        private void buttonCLE_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Coating Leading Edge Position (mm)", this, buttonCLE, "0.###", MS.ChuckStart, MS.ChuckEnd);
        }
        private void buttonSegmentGapLength_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Segment Gap (Length) (mm)", this, buttonSegmentGapLength, "0.###", 1, MS.MaxXTravel);
        }
        private void buttonSegmentGapHeight_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Segment Gap Height (µm)", this, buttonSegmentGapHeight, "0", 1, 5000, "", false);
        }
        private void buttonZVelocity_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Z Stage Velocity (mm/s)", this, buttonZVelocity, "0.###", 1, 300);
        }
        private void buttonZVelocityAdv_Click(object sender, EventArgs e)
        {
            var advForm = new FormRecipeEditAdvanced(_frmMain, this, _zStageVelocityParams, MC.XMotionProfile);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(advForm);
        }
        private void buttonGapVelocity_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Segment Gap Shuttle Velocity (mm/s)", this, buttonGapVelocity, "0.###", .5, 100);
        }
        private void buttonDispenseOnOffset_Click(object sender, EventArgs e)
        {
            double dMaxOn = double.Parse(buttonSegmentGapLength.Text);
            double dMinOn = double.Parse(buttonSegmentLength.Text);
            _frmMain.GotoNumScreen("Dispense On Offset (mm)", this, buttonDispenseOnOffset, "0.###", -20, dMaxOn);
        }
        private void buttonDispenseOffOffset_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Dispense Off Offset (mm)", this, buttonDispenseOffOffset, "0.###", 0, 20);
        }
        private void buttonRechargeRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Recharge Rate (µL)", this, buttonRechargeRate, "0.###", 0, MS.MaxPumpRate);
        }

        #endregion Buttons

        private void labelError_Click(object sender, EventArgs e)
        {
            RecipeHasErrors(true);
        }

        private void labelPrimingRate_Click(object sender, EventArgs e)
        {

        }

        private void labelPrimingDelay_Click(object sender, EventArgs e)
        {

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

        private void buttonJumpHeight_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Jump Height (µm)", this, buttonJumpHeight, "0", 0, MS.MaxZTravel * 1000, "", false);
        }

        private void buttonPostRechargeDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Post Recharge Delay", this, buttonPostRechargeDelay, "0", 0, 180000, "", false);
        }

        private void buttonReturnVelocity_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Carriage Return Velocity (mm/s)", this, buttonReturnVelocity, "0.###", 0.1, 100);
        }

        private void buttonPostRechargeDelay_Click_1(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen($"Post Recharge Delay ({(MS.DisplayedTimeUnits == "sec" ? "sec" : "msec")})", this, buttonPostRechargeDelay, MS.DisplayedTimeUnits == "sec" ? "#0.000" : "#0", 0, Int16.MaxValue, "", MS.DisplayedTimeUnits == "sec");
        }

        private void buttonKeyboard4Name_Click(object sender, EventArgs e)
        {
            _frmMain.GotoTextScreen("Recipe Name", this, textBoxRecipeName, 128);
        }

        private void buttonTransitionDispenseRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Transition Dispense Rate (µL/s)", this, buttonTransitionDispenseRate, "0.###", 0, MS.MaxPumpRate);
        }

        private void checkBoxReleaseVacOnCompletion_CheckedChanged(object sender, EventArgs e)
        {

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
                Title = "Dispense Rate",
                Units = "µl/s²",
                MinVal = 1 * _frmMain.MC.MinPumpAccel,
                MaxVal = 1000 * _frmMain.MC.MinPumpAccel
            };

            _coatingVelocityParams = new RecipeAdvancedEditParams()
            {
                Title = "Coating Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
            };

            _zStageVelocityParams = new RecipeAdvancedEditParams()
            {
                Title = "Z Stage Velocity",
                Units = "mm/s²",
                MinVal = 18.20,
                MaxVal = 18200
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

        private void buttonIRCoatingStartOffset_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("IR Coating Start Offset (mm):", this, buttonIRCoatingStartOffset, "0.0", MS.DieToIRPitch*-1, 100, "", true);
        }

        private void buttonIRPostCoatDistance_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("IR Post-Coat Distance (mm):", this, buttonIRPostCoatDistance, "0.0", 0.0, 250.0, "", true);
        }

        private void buttonIRPowerLevelCoating_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("IR Power Level During Coating (%):", this, buttonIRPowerLevelCoating, "#0", 0, 100, "", false);
        }

        private void buttonIRPowerLevelReturn_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("IR Power Level During Return (%):", this, buttonIRPowerLevelReturn, "#0", 0, 100, "", false);
        }

        private void buttonIRDelayOnReturn_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen($"IR Delay At Return ({(MS.DisplayedTimeUnits == "sec" ? "sec" : "msec")})", this, buttonIRDelayOnReturn, MS.DisplayedTimeUnits == "sec" ? "#0.000" : "#0", 0, Int16.MaxValue, "", MS.DisplayedTimeUnits == "sec");
        }

        private void buttonAirKnifeCoatingStartOffset_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Air-Knife Start Offset (mm)", this, buttonAirKnifeCoatingStartOffset, "#0.0", MS.DieToAirKnifePitch * -1, 100);
        }

        private void buttonAirKnifePostCoatDistance_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Air-Knife Post-Coat Distance (mm)", this, buttonAirKnifePostCoatDistance, "#0.0", 0, 250);
        }

        private void buttonAirKnifeShuttleVel_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Air Knife Shuttle Velocity (mm/sec)", this, buttonAirKnifeShuttleVel, "0.0", 0.0, 250.0, "", true);
        }

        private void buttonAirKnifePostCoatDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen($"Air Knife Post-Coat Delay ({(MS.DisplayedTimeUnits == "sec" ? "sec" : "msec")})", this, buttonAirKnifePostCoatDelay, MS.DisplayedTimeUnits == "sec" ? "#0.000" : "#0", 0, Int16.MaxValue, "", MS.DisplayedTimeUnits == "sec");
        }

        private void checkBoxCoatingOffset_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxCoatingOffset.Enabled = checkBoxCoatingOffset.Checked;
        }

        private void checkBoxTEO_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxTEO.Enabled = checkBoxTEO.Checked;
        }

        private void buttonLEOGap_Click(object sender, EventArgs e)
        {
            int max = int.Parse(buttonCoatingGap.Text);
            _frmMain.GotoNumScreen("Leading Edge Offset Gap (µm)", this, buttonLEOGap, "0", 0, max, "", false);
        }

        private void buttonLEODelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen($"Leading Edge Offset Delay ({(MS.DisplayedTimeUnits == "sec" ? "sec" : "msec")})", this, buttonLEODelay, MS.DisplayedTimeUnits == "sec" ? "#0.000" : "#0", 0, Int16.MaxValue, "", MS.DisplayedTimeUnits == "sec");
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

        private void buttonEditDispenseProfile_Click(object sender, EventArgs e)
        {
            double xmax = double.Parse(buttonSegmentLength.Text);
            var subForm = new FormDispenseProfileEditor(_frmMain, this, comboBoxDispenseProfileType.SelectedIndex, _dispenseProfileParamList, double.Parse(buttonCoatingDispenseRate.Text), xmax, _dispenseProfileEditorParams);
            _frmMain.CancelConfirmLeave();
            _frmMain.LoadSubForm(subForm);
        }
    }
}
