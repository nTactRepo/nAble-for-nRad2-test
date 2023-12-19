using Support2;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Support2.Forms
{

    public partial class UCTrialStatus : UserControl
    {
        #region Properties

        public NRadLicensing2 LicMgr { get; set; } = null;

        public int FirstWarningDays { get; set; } = 30;
        public int SeriousWarningDays { get; set; } = 7;

        #endregion

        #region Functions

        #region Constructors

        public UCTrialStatus()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Functions

        public void UpdateStatus()
        {
            if (!Visible || (LicMgr?.IsActivated ?? true))
            {
                panelTrialMode.Visible = false;
                return;
            }

            panelTrialMode.Visible = true;
            labelExpireDate.Text = LicMgr.ExpireDate.ToShortDateString();

            // Otherwise, set warning colors if the license expires soon
            labelTrialMode.ForeColor = Color.FromKnownColor(KnownColor.ControlText);

            if (LicMgr.ExpiresWithinXDays(SeriousWarningDays))
            {
                labelExpireDate.BackColor = Color.Red;
                labelTrialMode.ForeColor = Color.Red;
            }
            else if (LicMgr.ExpiresWithinXDays(FirstWarningDays))
            {
                labelExpireDate.BackColor = Color.Yellow;
            }
            else
            {
                labelExpireDate.BackColor = Color.Transparent;
            }
        }

        #endregion

        #region Control Event Handlers

        #endregion

        #region Private Functions

        #endregion

        #endregion
    }
}
