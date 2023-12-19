using nAble.Data;
using nTact.DataComm;
using System;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormHeadPurge : Form, IUpdateableForm
    {
        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;

        private GalilWrapper2 MC => _frmMain.MC;
        private MachineSettingsII MS => _frmMain.MS;
        private MachineStorage Storage => _frmMain.Storage;

        public FormHeadPurge(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            InitializeComponent();
        }

        private void FormHeadPurge_Load(object sender, EventArgs e)
        {
            buttonFluidHeadPurgeVolume.Text = Storage.HeadPurgingVolume.ToString();
            buttonFluidHeadPurgeRate.Text = Storage.HeadPurgeRate.ToString();
            buttonFluidHeadPurgeRechargeRate.Text = Storage.HeadPurgeRechargeRate.ToString();

            FixPurgeVol();
        }

        public void UpdateStatus()
        {
            if (!Visible)
            {
                return;
            }

            bool bAirOK = MC.SyringePumpDetected || (MC.POHPumpDetected && MC.MainAirOK);

            buttonFluidHeadPurgeVolume.Enabled = !MC.PrimeRunning;
            buttonFluidHeadPurgeRate.Enabled = !MC.PrimeRunning;
            buttonFluidHeadPurgeRechargeRate.Enabled = !MC.PrimeRunning;
			buttonHeadPurgeStart.Enabled = !MS.DisableHeadPurge && !MC.PrimeRunning && !MC.RunningRecipe && bAirOK;
            buttonHeadPurgeStart.Text = MS.DisableHeadPurge ? "Head Purge Disabled" : "Start Head Purge";
        }

        private void buttonFluidHeadPurgeRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Head Purge Rate (µl/s)", this, buttonFluidHeadPurgeRate, "#", 1, MS.MaxPumpRate);
        }

        private void buttonFluidHeadPurgeRechargeRate_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Head Purge Recharge Rate (µl/s)", this, buttonFluidHeadPurgeRechargeRate, "#", 1, MS.MaxPumpRate);
        }

        private void buttonFluidHeadPurgeVolume_Click(object sender, EventArgs e)
        {
            double max = GetMaxVolume(Storage.SelectedPump);
            _frmMain.GotoNumScreen("Head Purge volume (µl)", this, buttonFluidHeadPurgeVolume, "#", 1, max);
        }

        private void buttonHeadPurgeStart_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, string.Format("User started HeadPurge Vol: {0}  Speed: {1}", buttonFluidHeadPurgeVolume.Text, buttonFluidHeadPurgeRate.Text), "INFO");
            _frmMain.LastClick = DateTime.Now;
            string prompt = "Begin Head Purge?";

            if (MC.ELOPressed || MC.ELOWasTripped)
            {
                nRadMessageBox.Show(this, "Cannot proceed due to EMO Status!.", "Head Purge", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MC.PrimeRunning || _frmMain.Recirculating)
            {
                nRadMessageBox.Show(this, "Another prime operation or recirculation is already running.  Please stop the previous operation to continue.", "Head Purge", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MS.DisableHeadPurge)
            {
                nRadMessageBox.Show(this, "Head Purge Is Disabled. \nPlease Visit The Setup/Options Page To Enable This Feature.", "Head Purge", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (Math.Abs(MC.XPos - MS.XMaintLoc) > 2)
			{
				_log.log(LogType.TRACE, Category.INFO, "Detected die lips not over trough", "WARNING");
				prompt = "Head does not appear to be over Maintenance positon.  Begin Head Purge anyway?";
			}

			if (DialogResult.Yes == nRadMessageBox.Show(this, prompt, "Confirm Purge", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.INFO, "User confirmed Start Head Purge", "INFO");

                double volume = double.Parse(buttonFluidHeadPurgeVolume.Text); // ul
                double speed = double.Parse(buttonFluidHeadPurgeRate.Text);  // ul/s
                double rechargeRate = double.Parse(buttonFluidHeadPurgeRechargeRate.Text);  // ul/s
                double ulConv = MC.uLConv;

                if (volume != Storage.HeadPurgingVolume || speed != Storage.HeadPurgeRate || rechargeRate != Storage.HeadPurgeRechargeRate)
                {
                    Storage.HeadPurgingVolume = volume;
                    Storage.HeadPurgeRate = speed;
                    Storage.HeadPurgeRechargeRate = rechargeRate;
                    Storage.Save();
                }

                _log.log(LogType.TRACE, Category.INFO, "===========================");
                _log.log(LogType.TRACE, Category.INFO, "  Beginning Head Purge");

                string pumpLabel = Storage.SelectedPump == 0 ? "A" : Storage.SelectedPump == 1 ? "B" : "Mixing";
                _log.log(LogType.TRACE, Category.INFO, $"  Selected Pump: {pumpLabel}");

                if (Storage.SelectedPump == 2)
                {
                    double ratioA = Storage.PumpARatio;
                    _log.log(LogType.TRACE, Category.INFO, $"  Pump-A Ratio: {ratioA:#0.000}, Pump-B Ratio: {100 - ratioA:#0.000}");
                }

                _log.log(LogType.TRACE, Category.INFO, $"      Volume: {volume:#.###}");
                _log.log(LogType.TRACE, Category.INFO, $"    Dispense: {speed:#.###} µl/s");
                _log.log(LogType.TRACE, Category.INFO, $"    Recharge: {rechargeRate:#.###} µl/s");
                _log.log(LogType.TRACE, Category.INFO, $"      ulConv: {ulConv:#.###} µl/cnts");

                MC.StartPurge(PumpOp.Head, volume, speed, rechargeRate, ulConv, 4);
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Pressed No");
            }
        }

        private void FormHeadPurge_Enter(object sender, EventArgs e)
        {
            FixPurgeVol();
        }

        private void FixPurgeVol()
        {
            if (!double.TryParse(buttonFluidHeadPurgeVolume.Text, out double volume))
            {
                volume = double.MaxValue;
            }

            double max = GetMaxVolume(Storage.SelectedPump);
            volume = Math.Min(volume, max);

            buttonFluidHeadPurgeVolume.Text = $"{volume:#}";
        }

        private void FormHeadPurge_VisibleChanged(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "Head Purge Visible Changed");

            if (Visible)
            {
                FixPurgeVol();
            }
        }

        private double GetMaxVolume(int pumpSelected)
        {
            double max = 0;

            switch (pumpSelected)
            {
                case 0:
                case 2:
                    if (MC.SyringePumpDetected)
                    {
                        max = MS.SyringeVol * 1000;
                    }
                    else if (MC.POHPumpDetected)
                    {
                        max = MS.POHVol * 1000;
                    }
                    break;

                case 1:
                    if (MC.SyringePumpBDetected)
                    {
                        max = MS.SyringeBVol * 1000;
                    }
                    else if (MC.POHPumpBDetected)
                    {
                        max = MS.POHBVol * 1000;
                    }
                    break;

                default:
                    _log.log(LogType.TRACE, Category.WARN, $"Unknown pump selection value:  {pumpSelected}");
                    break;
            }
            if (Storage.SelectedPump == 0)
            {
            }
            else
            {
                if (MC.SyringePumpBDetected)
                {
                    max = MS.SyringeBVol * 1000;
                }
                else if (MC.POHPumpBDetected)
                {
                    max = MS.POHBVol * 1000;
                }
            }

            return max;
        }
    }
}
