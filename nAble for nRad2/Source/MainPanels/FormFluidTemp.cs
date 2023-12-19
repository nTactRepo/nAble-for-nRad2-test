using nAble.Data;
using nAble.Model;
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
    public partial class FormFluidTemp : Form, IUpdateableForm
    {
        #region Properties

        private MachineSettingsII MS => _frmMain.MS;
        private Omega485Controller Heaters => _frmMain.Omega485Controller;

        #endregion

        #region Data Members

        private readonly FormMain _frmMain = null;

        private bool _applyingDieSetpoint = false;
        private bool _applyingResvSetpoint = false;
        private bool _applyingResvBSetpoint = false;

        #endregion

        #region Functions

        #region Constructors

        public FormFluidTemp(FormMain formMain)
        {
            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));

            InitializeComponent();
        }

        #endregion

        #region Public Functions

        public void UpdateStatus()
        {
            if (!Visible)
            {
                return;
            }

            if (MS.DieTempControlEnabled)
            {
                labelDieTempOutput.Text = Heaters.Temp(MS.DieCOMID);
                labelDieTempOutput.ForeColor = Heaters.TempColor(MS.DieCOMID);

                if (buttonDieTempSetPoint.Text != $"{Heaters.SetPoint(MS.DieCOMID):0.0}")
                {
                    if (_applyingDieSetpoint)
                    {
                        buttonDieSetPointApply.Enabled = false;
                        labelDieSetPointChanges.Text = "Changes are being applied.";
                        labelDieSetPointChanges.Visible = true;
                    }
                    else
                    {
                        buttonDieSetPointApply.Enabled = true;
                        labelDieSetPointChanges.Text = "Changes have not been applied.";
                        labelDieSetPointChanges.Visible = true;
                    }
                }
                else
                {
                    _applyingDieSetpoint = false;
                    buttonDieSetPointApply.Enabled = false;
                    labelDieSetPointChanges.Visible = false;
                }
            }

            if (MS.ReservoirTempControlEnabled)
            {
                labelResvTempOutput.Text = Heaters.Temp(MS.ResvCOMID);
                labelResvTempOutput.ForeColor = Heaters.TempColor(MS.ResvCOMID);

                if (buttonResvSetPoint.Text != $"{Heaters.SetPoint(MS.ResvCOMID):0.0}")
                {
                    if (_applyingResvSetpoint)
                    {
                        buttonResvSetPointApply.Enabled = false;
                        labelResvSetPointChanges.Text = "Changes are being applied.";
                        labelResvSetPointChanges.Visible = true;
                    }
                    else
                    {
                        buttonResvSetPointApply.Enabled = true;
                        labelResvSetPointChanges.Text = "Changes have not been applied.";
                        labelResvSetPointChanges.Visible = true;
                    }
                }
                else
                {
                    _applyingResvSetpoint = false;
                    buttonResvSetPointApply.Enabled = false;
                    labelResvSetPointChanges.Visible = false;
                }

                if (MS.DualPumpInstalled)
                {
                    labelResvBTempOutput.Text = Heaters.Temp(MS.ResvBCOMID);
                    labelResvBTempOutput.ForeColor = Heaters.TempColor(MS.ResvBCOMID);

                    if (buttonResvBSetPoint.Text != $"{Heaters.SetPoint(MS.ResvBCOMID):0.0}")
                    {
                        if (_applyingResvBSetpoint)
                        {
                            buttonResvBSetPointApply.Enabled = false;
                            labelResvBSetPointChanges.Text = "Changes are being applied.";
                            labelResvBSetPointChanges.Visible = true;
                        }
                        else
                        {
                            buttonResvBSetPointApply.Enabled = true;
                            labelResvBSetPointChanges.Text = "Changes have not been applied.";
                            labelResvBSetPointChanges.Visible = true;
                        }
                    }
                    else
                    {
                        _applyingResvBSetpoint = false;
                        buttonResvBSetPointApply.Enabled = false;
                        labelResvBSetPointChanges.Visible = false;
                    }
                }
            }
        }

        #endregion

        #region Control Event Handlers

        private void FormFluidTemp_Load(object sender, EventArgs e)
        {
            groupBoxDieTemp.Visible = MS.DieTempControlEnabled;
            groupBoxReservoirTemp.Visible = MS.ReservoirTempControlEnabled;
            groupBoxReservoirBTemp.Visible = MS.ReservoirTempControlEnabled && MS.DualPumpInstalled;
            buttonResvAdvance.Visible = MS.ReservoirLimitControlEnabled;

            if (MS.DieTempControlEnabled)
            {
                buttonDieTempSetPoint.Text = $"{Heaters.SetPoint(MS.DieCOMID):0.0}";
            }

            if (MS.ReservoirTempControlEnabled)
            {
                buttonResvSetPoint.Text = $"{Heaters.SetPoint(MS.ResvCOMID):0.0}";

                if (MS.DualPumpInstalled)
                {
                    buttonResvBSetPoint.Text = $"{Heaters.SetPoint(MS.ResvBCOMID):0.0}";
                }
            }
        }

        private void buttonResvAdvance_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmFluidTempAdv);
        }

        private void buttonDieTempSetPoint_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Die Temperature Controller Setpoint", this, buttonDieTempSetPoint, "0.0", 15, MS.DieMaxTemp);
        }
        private void buttonResvSetPoint_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Reservoir-A Temperature Controller Setpoint", this, buttonResvSetPoint, "0.0", 15, MS.ReservoirMaxTemp);
        }

        private void buttonDieSetPointApply_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _applyingDieSetpoint = true;
            Heaters.ChangeTempSetPoint(MS.DieCOMID, double.Parse(buttonDieTempSetPoint.Text));
        }

        private void buttonResvSetPointApply_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _applyingResvSetpoint = true;
            Heaters.ChangeTempSetPoint(MS.ResvCOMID, double.Parse(buttonResvSetPoint.Text));
        }

        private void buttonResvBSetPointApply_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _applyingResvBSetpoint = true;
            Heaters.ChangeTempSetPoint(MS.ResvBCOMID, double.Parse(buttonResvBSetPoint.Text));
        }

        private void buttonResvBSetPoint_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Reservoir-B Temperature Controller Setpoint", this, buttonResvBSetPoint, "0.0", 15, MS.ReservoirMaxTemp);
        }

        private void buttonResvBAdvance_Click(object sender, EventArgs e)
        {
            // TODO
            //_frmMain.LoadSubForm(_frmMain.frmFluidTempAdv);
        }

        #endregion

        #endregion
    }
}
