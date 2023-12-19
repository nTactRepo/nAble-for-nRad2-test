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
    public partial class FormFluidTempAdv : Form, IUpdateableForm
    {
        private readonly FormMain _frmMain = null;
        private Omega485Controller Heaters => _frmMain.Omega485Controller;
        private MachineSettingsII MS => _frmMain.MS;

        private bool _applyingSetpoint = false;

        public FormFluidTempAdv(FormMain formMain)
        {
            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));

            InitializeComponent();
        }

        private void FormFluidTempAdv_Load(object sender, EventArgs e)
        {
            buttonResvHeaterSetPoint.Text = $"{Heaters.SetPoint(MS.ResvHeaterCOMID):0.0}";
        }

        public void UpdateStatus()
        {
            if (!Visible)
            {
                return;
            }

            labelResvHeaterTempOutput.Text = Heaters.Temp(MS.ResvHeaterCOMID);
            labelResvHeaterTempOutput.ForeColor = Heaters.TempColor(MS.ResvHeaterCOMID);

            if (buttonResvHeaterSetPoint.Text != $"{Heaters.SetPoint(MS.ResvHeaterCOMID):0.0}")
            {
                if (_applyingSetpoint)
                {
                    buttonResvHeaterSetPointApply.Enabled = false;
                    labelResvHeaterSetPointChanges.Text = "Changes are being applied.";
                    labelResvHeaterSetPointChanges.Visible = true;
                }
                else
                {
                    buttonResvHeaterSetPointApply.Enabled = true;
                    labelResvHeaterSetPointChanges.Text = "Changes have not been applied.";
                    labelResvHeaterSetPointChanges.Visible = true;
                }
            }
            else
            {
                _applyingSetpoint = false;
                buttonResvHeaterSetPointApply.Enabled = false;
                labelResvHeaterSetPointChanges.Visible = false;
            }
        }

        private void buttonResvTempAdvDone_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.LoadSubForm(_frmMain.frmFluidTemp);
        }

        private void buttonResvHeaterSetPoint_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Reservoir Temperature Controller Setpoint", this, buttonResvHeaterSetPoint, "0.0", 15, _frmMain.MS.ReservoirHeaterMaxTemp);
        }

        private void buttonResvHeaterSetPointApply_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _applyingSetpoint = true;
            Heaters.ChangeTempSetPoint(MS.ResvHeaterCOMID, double.Parse(buttonResvHeaterSetPoint.Text));
        }
    }
}
