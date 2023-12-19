using nAble.Data;
using nTact.DataComm;
using System;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormPumpPrime : Form, IUpdateableForm
    {
        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;

        private GalilWrapper2 MC => _frmMain.MC;
        private MachineSettingsII MS => _frmMain.MS;
        private MachineStorage Storage => _frmMain.Storage;

        public FormPumpPrime(FormMain formMain, LogEntry log)
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

            buttonFluidSyringePrimeCount.Enabled = !MC.PrimeRunning;
            buttonFluidSyringePrimeRate.Enabled = !MC.PrimeRunning;
            buttonFluidSyringePrimeRechargeRate.Enabled = !MC.PrimeRunning;
			buttonSyringPrimeStart.Enabled = !MC.PrimeRunning && !MC.RunningRecipe && airOK;
        }

        private void FormPumpPrime_Load(object sender, EventArgs e)
        {
            buttonFluidSyringePrimeCount.Text = Storage.SyringePrimingCount.ToString();
            buttonFluidSyringePrimeRate.Text = Storage.SyringePrimingRate.ToString();
            buttonFluidSyringePrimeRechargeRate.Text = Storage.SyringePrimingRechargeRate.ToString();
        }

        private void buttonFluidSyringePrimeRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Pump Prime Rate (µl/s)", this, buttonFluidSyringePrimeRate, "#", 1, MS.MaxPumpRate);
        }

        private void buttonFluidSyringePrimeRechargeRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Pump Prime Recharge Rate (µl/s)", this, buttonFluidSyringePrimeRechargeRate, "#", 1, MS.MaxPumpRate);
        }

        private void buttonFluidSyringePrimeCount_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Pump Prime cycles", this, buttonFluidSyringePrimeCount, "#", 1, 250);
        }

        private void buttonSyringPrimeStart_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, string.Format("User started PumpPrime Counts: {0}  Speed: {1}", buttonFluidSyringePrimeCount.Text, buttonFluidSyringePrimeRate.Text), "INFO");
            string sPrompt = "Begin Pump Prime?";

            if (MC.ELOPressed || MC.ELOWasTripped)
            {
                nRadMessageBox.Show(this, "Cannot proceed due to EMO Status!.", "Pump Prime", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MC.PrimeRunning || _frmMain.Recirculating)
            {
                nRadMessageBox.Show(this, "Another prime operation or recirculation is already running.  Please stop the previous operation to continue.", "Pump Prime", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (DialogResult.Yes == nRadMessageBox.Show(this, sPrompt, "Confirm Prime", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.INFO, "User confirmed Start Pump Prime", "INFO");

                int counts = int.Parse(buttonFluidSyringePrimeCount.Text);
                double speed = double.Parse(buttonFluidSyringePrimeRate.Text);  // ul
                double rechargeRate = double.Parse(buttonFluidSyringePrimeRechargeRate.Text);  // ul
                double ulConv = MC.uLConv;
                
                if (counts != Storage.SyringePrimingCount || speed != Storage.SyringePrimingRate ||
                    rechargeRate != Storage.SyringePrimingRechargeRate)
                {
                    Storage.SyringePrimingCount = counts;
                    Storage.SyringePrimingRate = speed;
                    Storage.SyringePrimingRechargeRate = rechargeRate;
                    Storage.Save();
                }

                _log.log(LogType.TRACE, Category.INFO, "===========================");
                _log.log(LogType.TRACE, Category.INFO, "Beginning Syringe/Pump Prime");

                string pumpLabel = Storage.SelectedPump == 0 ? "A" : Storage.SelectedPump == 1 ? "B" : "Mixing";
                _log.log(LogType.TRACE, Category.INFO, $"  Selected Pump: {pumpLabel}", "INFO");

                if (Storage.SelectedPump == 2)
                {
                    double ratioA = Storage.PumpARatio;
                    _log.log(LogType.TRACE, Category.INFO, $"  Pump-A Ratio: {ratioA:#0.000}, Pump-B Ratio: {100 - ratioA:#0.000}");
                }

                _log.log(LogType.TRACE, Category.INFO, $"    # Cycles: {counts}");
                _log.log(LogType.TRACE, Category.INFO, $"    Dispense: {speed:#.###} µl/s");
                _log.log(LogType.TRACE, Category.INFO, $"    Recharge: {rechargeRate:#.###} µl/s");
                _log.log(LogType.TRACE, Category.INFO, $"      ulConv: {ulConv:#.###} µl/cnts");

                MC.StartPrime(PumpOp.Pump, counts, speed, rechargeRate, ulConv, 4);
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Pressed No");
            }
        }
    }
}
