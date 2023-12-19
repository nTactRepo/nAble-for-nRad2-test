using CommonLibrary.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLibrary.Forms
{
    public partial class FormSplash : Form, ISplashScreen
    {
        #region Properties

        public string Header { get; set; } = "{{Header}}";

        public int MinimumDurationInMs { get; set; } = 2000;

        #endregion

        #region Member Data

        private DateTime _minEndTime;
        private bool _closeOK = false;

        #endregion

        #region Functions

        #region Constructors

        public FormSplash()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Functions

        public void AddMessage(string message)
        {
            _ = flowLayoutPanelMessages.BeginInvoke((MethodInvoker)delegate ()
            {
                var label = new Label()
                {
                    Text = message,
                    AutoSize = false,
                    Width = flowLayoutPanelMessages.Width,
                    Height = 26,
                    Font = flowLayoutPanelMessages.Font
                };

                flowLayoutPanelMessages.Controls.Add(label);
                flowLayoutPanelMessages.ScrollControlIntoView(label);
            });
        }

        public void CloseSplash()
        {
            if (Created)
            {
                _ = BeginInvoke((MethodInvoker)delegate ()
                {
                    Close();
                });
            }
        }

        public void UpdateStatus()
        {
            if (DateTime.Now > _minEndTime)
            {
                _closeOK = true;
            }
        }

        #endregion

        #region Control Event Handlers

        private void FormSplash_Load(object sender, EventArgs e)
        {
            labelHeader.Text = Header;
            _minEndTime = DateTime.Now.AddMilliseconds(MinimumDurationInMs);

            // This bit of voodoo blocks the horizontal scroll bar
            // https://stackoverflow.com/questions/5489273/how-do-i-disable-the-horizontal-scrollbar-in-a-panel
            _ = flowLayoutPanelMessages.BeginInvoke((MethodInvoker)delegate ()
            {
                flowLayoutPanelMessages.AutoScroll = false;
                flowLayoutPanelMessages.HorizontalScroll.Enabled = false;
                flowLayoutPanelMessages.HorizontalScroll.Visible = false;
                flowLayoutPanelMessages.HorizontalScroll.Maximum = 0;
                flowLayoutPanelMessages.AutoScroll = true;
            });
        }

        private void FormSplash_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_closeOK && DateTime.Now < _minEndTime)
            {
                e.Cancel = true;
                timerDelayedClose.Interval = Math.Max((int)(_minEndTime - DateTime.Now).TotalMilliseconds, 1);
                timerDelayedClose.Start();
            }
        }

        private void timerDelayedClose_Tick(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #endregion
    }
}
