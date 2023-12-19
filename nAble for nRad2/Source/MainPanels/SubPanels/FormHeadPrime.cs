using nAble.Data;
using nTact.DataComm;
using System;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormHeadPrime : Form, IUpdateableForm
    {
        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;

        private GalilWrapper2 MC => _frmMain.MC;
        private MachineSettingsII MS => _frmMain.MS;
        private MachineStorage Storage => _frmMain.Storage;

        public FormHeadPrime(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            InitializeComponent();
        }

        private void FormHeadPrime_Load(object sender, EventArgs e)
        {
            buttonFluidHeadPrimeCount.Text = Storage.HeadPrimingCount.ToString();
            buttonFluidHeadPrimeRate.Text = Storage.HeadPrimeRate.ToString();
            buttonFluidHeadPrimeRechargeRate.Text = Storage.HeadPrimeRechargeRate.ToString();
        }

        public void UpdateStatus()
        {
            if (!Visible)
            {
                return;
            }

            bool airOK = MC.SyringePumpDetected || (MC.POHPumpDetected && MC.MainAirOK);

            buttonFluidHeadPrimeCount.Enabled = !MC.PrimeRunning;
            buttonFluidHeadPrimeRate.Enabled = !MC.PrimeRunning;
            buttonFluidHeadPrimeRechargeRate.Enabled = !MC.PrimeRunning;
			buttonHeadPrimeStart.Enabled = !MC.PrimeRunning && !MC.RunningRecipe && airOK;
		}

        private void buttonFluidHeadPrimeRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Head Prime Rate (µl/s)", this, buttonFluidHeadPrimeRate, "#", 1, MS.MaxPumpRate);
        }

        private void buttonFluidHeadPrimeRechargeRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Head Prime Recharge Rate (µl/s)", this, buttonFluidHeadPrimeRechargeRate, "#", 1, MS.MaxPumpRate);
        }

        private void buttonFluidHeadPrimeCount_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Head Prime cycles", this, buttonFluidHeadPrimeCount, "#", 1, 250);
        }

        private void buttonHeadPrimeStart_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, string.Format("User started PumpPrime Counts: {0}  Speed: {1}", buttonFluidHeadPrimeCount.Text, buttonFluidHeadPrimeRate.Text), "INFO");
            string prompt = "Begin Head Prime?";

            if (MC.ELOPressed || MC.ELOWasTripped)
            {
                nRadMessageBox.Show(this, "Cannot proceed due to EMO Status!.", "Head Prime", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MC.PrimeRunning || _frmMain.Recirculating)
            {
                nRadMessageBox.Show(this, "Another prime operation or recirculation is already running.  Please stop the previous operation to continue.", "Head Prime", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (DialogResult.Yes == nRadMessageBox.Show(this, prompt, "Confirm Prime", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.INFO, "User confirmed Start Head Prime", "INFO");

                int counts = int.Parse(buttonFluidHeadPrimeCount.Text);
                double speed = double.Parse(buttonFluidHeadPrimeRate.Text);  // ul
                double rechargeRate = double.Parse(buttonFluidHeadPrimeRechargeRate.Text);  // ul
                double ulConv = MC.uLConv;
                
                if (counts != Storage.HeadPrimingCount || speed != Storage.HeadPrimeRate || rechargeRate != Storage.HeadPrimeRechargeRate)
                {
                    Storage.HeadPrimingCount = counts;
                    Storage.HeadPrimeRate = speed;
                    Storage.HeadPrimeRechargeRate = rechargeRate;
                    Storage.Save();
                }

                _log.log(LogType.TRACE, Category.INFO, "===========================");
                _log.log(LogType.TRACE, Category.INFO, "  Beginning Head Prime", "INFO");

                string pumpLabel = Storage.SelectedPump == 0 ? "A" : Storage.SelectedPump == 1 ? "B" : "Mixing";
                _log.log(LogType.TRACE, Category.INFO, $"Selected Pump: {pumpLabel}");

                if (Storage.SelectedPump == 2)
                {
                    double ratioA = Storage.PumpARatio;
                    _log.log(LogType.TRACE, Category.INFO, $"  Pump-A Ratio: {ratioA:#0.000}, Pump-B Ratio: {100 - ratioA:#0.000}");
                }

                _log.log(LogType.TRACE, Category.INFO, $"    # Cycles: {counts}");
                _log.log(LogType.TRACE, Category.INFO, $"    Dispense: {speed:#.###} µl/s");
                _log.log(LogType.TRACE, Category.INFO, $"    Recharge: {rechargeRate:#.###} µl/s");
                _log.log(LogType.TRACE, Category.INFO, $"      ulConv: {ulConv:#.###} µl/cnts");

                MC.StartPrime(PumpOp.Head, counts, speed, rechargeRate, ulConv, 4);
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Pressed No");
            }
        }
    }
}
