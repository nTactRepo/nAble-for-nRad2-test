using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using nTact.DataComm;
using nAble.Data;
using nAble.DataComm.AdvantechSerialServer;

namespace nAble
{
    public partial class FormFluidFlow : Form, IUpdateableForm
    {
        private FormPumpPrime frmPumpPrime = null;
        private FormHeadPrime frmHeadPrime = null;
        private FormHeadPurge frmHeadPurge = null;
        private FormPumpPurge frmPumpPurge = null;
        private Color _colorControl = Color.FromName("Control");
        private double _pumpARatio = 0;
        private double _pumpBRatio = 0;
        private bool _visibleOneShot = true;

        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;

        private GalilWrapper2 MC => _frmMain.MC;
        private MachineSettingsII MS => _frmMain.MS;
        private MachineStorage Storage => _frmMain.Storage;
        private AdvantechServer AdvServer => _frmMain.AdvServer;

        public FormFluidFlow(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            InitializeComponent();

            frmPumpPrime = new FormPumpPrime(_frmMain, _log);
            frmHeadPrime = new FormHeadPrime(_frmMain, _log);
            frmHeadPurge = new FormHeadPurge(_frmMain, _log);
            frmPumpPurge = new FormPumpPurge(_frmMain, _log);
        }

        private void FormFluidFlow_Load(object sender, EventArgs e)
        {
            LoadSubForm(frmPumpPrime);
            buttonRecircTime.Text = Storage.RecirculationInterval.ToString();
            buttonRecircCount.Text = Storage.RecirculationCount.ToString();
            comboBoxSelectedPump.Visible = MS.DualPumpInstalled;
            labelSelectedPumpTitle.Visible = MS.DualPumpInstalled;
            buttonExpandRatioPanel.Visible = MS.DualPumpInstalled;
            comboBoxSelectedPump.SelectedIndex = MS.DualPumpInstalled ? Storage.SelectedPump : 0;
            buttonPumpARatio.Text = $"{Storage.PumpARatio:#0.000}";
        }

        private void CheckSettings()
        {
            buttonFluidHeadPurge.Visible = !MS.HideHeadPurgeOnFluidControl;
            panelRecirc.Visible = !MS.HideRecirculateOnFluidControl;
        }

        internal void LoadSubForm(Form newForm)
        {
            newForm.TopLevel = false;
            panelFluidParams.Controls.Clear();
            panelFluidParams.Controls.Add(newForm);
            newForm.Dock = DockStyle.Fill;
            newForm.Show();
        }

        private void buttonFluidPumpPrime_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            LoadSubForm(frmPumpPrime);
        }

        private void buttonFluidHeadPrime_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            LoadSubForm(frmHeadPrime);
        }

        private void buttonFluidHeadPurge_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            LoadSubForm(frmHeadPurge);
        }

        private void buttonFluidStopPrime_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            MC.StopPrime();
        }

        public void UpdateStatus()
        {
            frmHeadPrime.UpdateStatus();
            frmHeadPurge.UpdateStatus();
            frmPumpPrime.UpdateStatus();
            frmPumpPurge.UpdateStatus();

            if (!Visible)
            {
                _visibleOneShot = true;
                return;
            }

            if (_visibleOneShot)
            {
                comboBoxSelectedPump.SelectedIndex = MC.SelectedPump < 0 ? 0 : MC.SelectedPump;
                double pumpARatio = MC.PumpARatio;
                buttonPumpARatio.Text = $"{pumpARatio*100.000:#0.000}";
                labelPumpBRatio.Text = $"{(1.000-pumpARatio)*100.000:#0.000}";
                _visibleOneShot = false;
            }

            CheckSettings();

            labelPumpSpec.Text = MC.SyringePumpDetected ? "Detected Pump: Syringe" :
                MC.POHPumpDetected ? "Detected Pump: POH" :
                "No Pump Detected";

            labelPumpAVolDispensed.Text = (MC.PumpA_TP / MC.uLConv / 1000).ToString("#0.000") + "ml";
            labelPumpBVolDispensed.Text = (MC.PumpB_TP / MC.uLConv / 1000).ToString("#0.000") + "ml";
            buttonFluidStopPrime.Enabled = MC.HeadPrimeRunning || MC.HeadPurgeRunning || MC.PumpPrimeRunning || MC.PumpPurgeRunning;

            panelValveStatus.Visible = MC.POHPumpDetected || MC.POHPumpBDetected;
            panelValvePosition.Visible = MC.SyringePumpDetected || MC.SyringePumpBDetected;

            if (panelValvePosition.Visible)
            {
                labelValveAPosition.Text = MC.ValveStatusName;
                labelValveBPosition.Text = MC.ValveBStatusName;
            }

            if (MC.PumpPrimeRunning)
            {
                buttonFluidPumpPrime.BackColor = Color.Yellow;
                labelValveStatus.Text = string.Format("Performing Pump Prime\r\nCycle {0} of {1}", MC.PrimingCounter + 1, MC.PrimingCounts);
            }
            else if (MC.HeadPrimeRunning)
            {
                buttonFluidHeadPrime.BackColor = Color.Yellow;
                labelValveStatus.Text = string.Format("Performing Head Prime\r\nCycle {0} of {1}", MC.PrimingCounter + 1, MC.PrimingCounts);
            }
            else if (MC.HeadPurgeRunning)
            {
                buttonFluidHeadPurge.BackColor = Color.Yellow;
                labelValveStatus.Text = $"Performing Head Purge\r\n{MC.HeadPurgingVolume} µl";
            }
            else if (MC.PumpPurgeRunning)
            {
                buttonFluidPumpPurge.BackColor = Color.Yellow;
                labelValveStatus.Text = "Performing Pump Purge";
            }
            else if (MC.StoppingPrime)
            {
                buttonFluidStopPrime.BackColor = Color.Yellow;
                labelValveStatus.Text = "Stopping pump action";
            }
            else if (MC.AbortingPumpOperation)
            {
                buttonFluidStopPrime.BackColor = Color.Yellow;
                labelValveStatus.Text = "Aborting Pump Operation";
            }
            else
            {
                buttonFluidPumpPrime.UseVisualStyleBackColor = true;
                buttonFluidPumpPurge.UseVisualStyleBackColor = true;
                buttonFluidHeadPrime.UseVisualStyleBackColor = true;
                buttonFluidHeadPurge.UseVisualStyleBackColor = true;
                buttonFluidStopPrime.UseVisualStyleBackColor = true;
                labelValveStatus.Text = "Ready";
            }

            buttonRecircTime.BackColor = int.Parse(buttonRecircTime.Text) == Storage.RecirculationInterval ? _colorControl : Color.Yellow;
            if (buttonRecircTime.BackColor != Color.Yellow)
                buttonRecircTime.UseVisualStyleBackColor = true;
            buttonRecircCount.BackColor = int.Parse(buttonRecircCount.Text) == Storage.RecirculationCount ? _colorControl : Color.Yellow;
            if (buttonRecircCount.BackColor != Color.Yellow)
                buttonRecircCount.UseVisualStyleBackColor = true;

            labelSaveWarning.Visible = (buttonRecircCount.BackColor == Color.Yellow || buttonRecircTime.BackColor == Color.Yellow);
            buttonRecirc.Enabled = !MC.PrimeRunning || _frmMain.Recirculating;
            buttonRecirc.Text = _frmMain.Recirculating ? "Disable Recirculation" : "Enable Recirculation";
            buttonRecirc.BackColor = _frmMain.Recirculating ? Color.Lime : _colorControl;
            if (buttonRecirc.BackColor != Color.Lime)
                buttonRecirc.UseVisualStyleBackColor = true;
            buttonFluidPumpPrime.Enabled = !MC.PrimeRunning; // || _frmMain.Recirculating);
            buttonFluidPumpPurge.Enabled = !MC.PrimeRunning; // || _frmMain.Recirculating);
            buttonFluidHeadPrime.Enabled = !MC.PrimeRunning; // || _frmMain.Recirculating);
            buttonFluidHeadPurge.Enabled = !MC.PrimeRunning; // || _frmMain.Recirculating);
            buttonFluidHeadPurge.Visible = !(MS.HideHeadPurgeOnFluidControl);
            buttonRecirc.Visible = !(MS.HideRecirculateOnFluidControl);

            pictureBoxValveDieVent.BackColor = !MC.DieVentOpen ? Color.Red : MC.MainAirOK ? Color.Lime : Color.Yellow;
            pictureBoxValveDispense.BackColor = MC.SyringePumpDetected ? _colorControl : !MC.DispenseVlvOpen ? Color.Red : MC.MainAirOK ? Color.Lime : Color.Yellow;
            pictureBoxValveRecharge.BackColor = MC.SyringePumpDetected ? _colorControl : !MC.RechargeVlvOpen ? Color.Red : MC.MainAirOK ? Color.Lime : Color.Yellow;
            pictureBoxValveHeadVent.BackColor = MC.SyringePumpDetected ? _colorControl : !MC.HeadVentVlvOpen ? Color.Red : MC.MainAirOK ? Color.Lime : Color.Yellow;

            labelRecircCountUnits.Visible = !MS.HideRecirculateOnFluidControl;

            groupBoxDiePressure.Visible = MS.HasDiePressureTransducer || MS.AdvantechServerInstalled;

            if (MS.AdvantechServerInstalled)
            {
                var inputPressure = AdvServer.GetAnalogInputByName("Die Pressure");
                groupBoxDiePressure.Text = "Die Press";
                labelDiePressure.Text = $"{inputPressure?.Value ?? 0:#0.##}";
                labelDiePressureUnits.Text = $"{inputPressure.Unit}";
            }
            else if (MS.HasDiePressureTransducer)
            {
                switch (PressDisplayType)
                {
                    case 0:
                    {
                        groupBoxDiePressure.Text = "Die Pressure Avg.";
                        labelDiePressure.Text = $"{MC.DiePressurePSISmoothed:0.00}";
                        labelDiePressureUnits.Text = "PSI";
                        break;
                    }

                    case 1:
                    {
                        groupBoxDiePressure.Text = "Die Pressure Avg.";
                        labelDiePressure.Text = $"{MC.DiePressureVDCSmoothed:0.000}";
                        labelDiePressureUnits.Text = "VDC";
                        break;
                    }

                    case 2:
                    {
                        groupBoxDiePressure.Text = "Die Pressure";
                        labelDiePressure.Text = $"{MC.DiePressurePSI:0.00}";
                        labelDiePressureUnits.Text = "PSI";
                        break;
                    }

                    case 3:
                    {
                        groupBoxDiePressure.Text = "Die Pressure";
                        labelDiePressure.Text = $"{MC.Analogs[8]:0.000}";
                        labelDiePressureUnits.Text = "VDC";
                        break;
                    }
                }
            }

            groupBoxDispenseRate.Visible = MS.AdvantechServerInstalled;

            if (MS.AdvantechServerInstalled)
            {
                var inputA = AdvServer.GetAnalogInputByName("Pump A Flow");
                labelDispenseRateA.Text = $"A:{inputA?.Value ?? 0:#0.##} {inputA.Unit}";

                if (labelDispenseRateB.Visible)
                {
                    var inputB = AdvServer.GetAnalogInputByName("Pump B Flow");
                    labelDispenseRateB.Text = $"B:{inputB?.Value ?? 0:#0.##} {inputB.Unit}";
                }
            }
        }

        private void FormFluidFlow_Enter(object sender, EventArgs e)
        {
        }

        private void buttonRecirc_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Recirculation Button", "INFO");
            string prompt;
            int recircTime = 0, recircCount = 0;

            string tag = _frmMain.Recirculating ? "Disable" : "Enable";
            prompt = $"{tag} Pump Recirculation Sequence?";

            if (MC.ELOPressed || MC.ELOWasTripped)
            {
                nRadMessageBox.Show(this, "Cannot proceed due to EMO Status!.", "Recirculate Request", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MC.PrimeRunning)
            {
                nRadMessageBox.Show(this, "Another prime operation or recirculation is already running.  Please stop the previous operation to continue.", "Pump Prime", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            recircTime = int.Parse(buttonRecircTime.Text);
            recircCount = int.Parse(buttonRecircCount.Text);

            if (!_frmMain.Recirculating && (recircCount > recircTime))
            {
                nRadMessageBox.Show(this, "Recirculation is limited to once per minute. Please adjust the Time or Count.", "Pump Prime", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (DialogResult.Yes == nRadMessageBox.Show(this, prompt, "Recirculation Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                if (_frmMain.Recirculating)
                    _frmMain.StopRecirc();
                else
                {
                    Storage.RecirculationInterval = recircTime;
                    Storage.RecirculationCount = recircCount;
                    _frmMain.StartRecirc();
                }
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Pressed No");
            }

        }

        private void buttonPumpPurge_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            LoadSubForm(frmPumpPurge);
        }

        private void buttonRecircTime_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen("Recirculation Interval (mins)", this, buttonRecircTime, "#", 1, 3599, "", false);
        }

        private void buttonRecircCount_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen("Recirculation Stroke Count", this, buttonRecircCount, "#", 1, 25, "", false);
        }

        private int PressDisplayType { set; get; } = 0;
        private void labelDiePressure_Click(object sender, EventArgs e)
        {
            PressDisplayType++;
            if (PressDisplayType == 4) PressDisplayType = 0;
        }

        private void buttonExpandRatioPanel_Click(object sender, EventArgs e)
        {
            if (!panelRatioMixing.Visible)
            {
                panelRatioMixing.Visible = true;
                panelRatioMixing.BringToFront();
            }
            else
            {
                panelRatioMixing.Visible = false;
            }
        }

        private void buttonPumpARatio_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Pump-A Dispense Ratio % (0.001 - 99.999)", this, buttonPumpARatio, "#0.000", .001, 99.999);
        }

        private void buttonPumpARatio_TextChanged(object sender, EventArgs e)
        {
            _pumpARatio = Convert.ToDouble(buttonPumpARatio.Text);
            _pumpBRatio = 100.000 - _pumpARatio;
            Storage.PumpARatio = _pumpARatio;
            Storage.Save();
            labelPumpBRatio.Text = _pumpBRatio.ToString("#0.000");
            panelRatioMixing.Visible = false;

            if (!_visibleOneShot)
            {
                MC.ChangePumpSetup(comboBoxSelectedPump.SelectedIndex, _pumpARatio, _pumpBRatio);
            }
        }

        private void comboBoxSelectedPump_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelRatioMixing.Visible = false;
            buttonExpandRatioPanel.Visible = comboBoxSelectedPump.SelectedIndex == 2;
            bool noChange = Storage.SelectedPump == comboBoxSelectedPump.SelectedIndex && Storage.PumpARatio == Convert.ToDouble(buttonPumpARatio.Text);
            ShowVolumesAndStatus(comboBoxSelectedPump.SelectedItem);

            if (noChange)
            {
                return;
            }

            Storage.SelectedPump = comboBoxSelectedPump.SelectedIndex;
            Storage.Save();
            _pumpARatio = Convert.ToDouble(buttonPumpARatio.Text);
            _pumpBRatio = 100.000 - _pumpARatio;
            MC.ChangePumpSetup(comboBoxSelectedPump.SelectedIndex, _pumpARatio, _pumpBRatio);
        }

        private void ShowVolumesAndStatus(object item)
        {
            string type = (string)item;

            switch (type)
            {
                case "Pump-A":
                {
                    labelPumpAVolDispensed.Visible = true;
                    labelPumpBVolDispensed.Visible = false;
                    labelValveAPosition.Visible = true;
                    labelValveBPosition.Visible = false;
                    labelDispenseRateA.Visible = true;
                    labelDispenseRateB.Visible = false;
                    break;
                }

                case "Pump-B":
                {
                    labelPumpAVolDispensed.Visible = false;
                    labelPumpBVolDispensed.Visible = true;
                    labelValveAPosition.Visible = false;
                    labelValveBPosition.Visible = true;
                    labelDispenseRateA.Visible = false;
                    labelDispenseRateB.Visible = true;
                    break;
                }

                case "Mixing":
                {
                    labelPumpAVolDispensed.Visible = true;
                    labelPumpBVolDispensed.Visible = true;
                    labelValveAPosition.Visible = true;
                    labelValveBPosition.Visible = true;
                    labelDispenseRateA.Visible = true;
                    labelDispenseRateB.Visible = true;
                    break;
                }

                default: throw new ArgumentException($"ShowVolumesAndStatus:  Unknown pump type = {type}");
            }
        }
    }
}
