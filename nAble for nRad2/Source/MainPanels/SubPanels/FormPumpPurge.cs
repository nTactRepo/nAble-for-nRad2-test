using nAble.Data;
using nTact.DataComm;
using System;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormPumpPurge : Form, IUpdateableForm
    {
        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;

        private GalilWrapper2 MC => _frmMain.MC;
        private MachineSettingsII MS => _frmMain.MS;
        private MachineStorage Storage => _frmMain.Storage;

        public FormPumpPurge(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            InitializeComponent();
        }

        public void UpdateStatus()
        {
            if (!Visible)
            {
                return;
            }

            bool airOK = MC.SyringePumpDetected || (MC.POHPumpDetected && MC.MainAirOK);

            buttonFluidSyringePurgeRate.Enabled = !MC.PrimeRunning;
            buttonFluidSyringePurgeRechargeRate.Enabled = !MC.PrimeRunning;
			buttonSyringPurgeStart.Enabled = !MC.PrimeRunning && !MC.RunningRecipe && airOK;
        }

        private void FormPumpPurge_Load(object sender, EventArgs e)
        {
            buttonFluidSyringePurgeRate.Text = Storage.SyringePrimingRate.ToString();
            buttonFluidSyringePurgeRechargeRate.Text = Storage.SyringePrimingRechargeRate.ToString();
        }

        private void buttonFluidSyringePurgeRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Pump Purge Rate (µl/s)", this, buttonFluidSyringePurgeRate, "#", 1, MS.MaxPumpRate);
        }

        private void buttonFluidSyringePurgeRechargeRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Pump Purge Recharge Rate (µl/s)", this, buttonFluidSyringePurgeRechargeRate, "#", 1, MS.MaxPumpRate);
        }

        private void buttonSyringPurgeStart_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, string.Format("User started PumpPurge Speed: {0}", buttonFluidSyringePurgeRate.Text), "INFO");
            string sPrompt = "Begin Pump Purge?";

            if (MC.ELOPressed || MC.ELOWasTripped)
            {
                nRadMessageBox.Show(this, "Cannot proceed due to EMO Status!.", "Pump Purge", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MC.PrimeRunning || _frmMain.Recirculating)
            {
                nRadMessageBox.Show(this, "Another prime operation or recirculation is already running.  Please stop the previous operation to continue.", "Pump Purge", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (DialogResult.Yes == nRadMessageBox.Show(this, sPrompt, "Confirm Purge", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.INFO, "User confirmed Start Pump Purge", "INFO");

                double speed = double.Parse(buttonFluidSyringePurgeRate.Text);  // ul
                double rechargeRate = double.Parse(buttonFluidSyringePurgeRechargeRate.Text);  // ul
                double ulConv = MC.uLConv;
                double vol = 1000 * (MC.SyringePumpDetected ? MS.SyringeVol : MS.POHVol);

                _log.log(LogType.TRACE, Category.INFO, "===========================", "INFO");
                _log.log(LogType.TRACE, Category.INFO, "  Beginning Syring/Pump Purge", "INFO");

                string pumpLabel = Storage.SelectedPump == 0 ? "A" : Storage.SelectedPump == 1 ? "B" : "Mixing";
                _log.log(LogType.TRACE, Category.INFO, $"  Selected Pump: {pumpLabel}", "INFO");

                if (Storage.SelectedPump == 2)
                {
                    double ratioA = Storage.PumpARatio;
                    _log.log(LogType.TRACE, Category.INFO, $"  Pump-A Ratio: {ratioA:#0.000}, Pump-B Ratio: {100 - ratioA:#0.000}", "INFO");
                }

                _log.log(LogType.TRACE, Category.INFO, $"    Dispense: {speed:#.###} µl/s", "INFO");
                _log.log(LogType.TRACE, Category.INFO, $"    Recharge: {rechargeRate:#.###} µl/s", "INFO");
                _log.log(LogType.TRACE, Category.INFO, $"      ulConv: {ulConv:#.###} µl/cnts", "INFO");

                MC.StartPurge(PumpOp.Pump, vol, speed, rechargeRate, ulConv, 4);
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Pressed No");
            }
        }
    }
}
