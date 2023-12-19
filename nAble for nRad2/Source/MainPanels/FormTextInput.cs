using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace nAble
{
    public partial class FormTextInput : Form, IUpdateableForm
    {
        #region Fields

        private readonly FormMain _formMain = null;

        private Form _returnPage = null;
        private Control _returnControlItem = null;
        private int _maxLength = 0;

        private List<Button> _alphaButtons = new List<Button>(30);
        private DateTime _lastClick = DateTime.Now;

        private bool _shifted = true;
        private bool _capsLocked = false;

        #endregion

        #region Functions

        #region Constructors

        public FormTextInput(FormMain formMain)
        {
            InitializeComponent();

            var buttons = new List<Button>();

            _formMain = formMain ?? throw new NullReferenceException(nameof(formMain));

            _alphaButtons
                .AddRange(Controls.OfType<Button>()
                .Where(button => IsLetterButton(button)));
        }

        #endregion

        #region Public Functions

        public void UpdateStatus()
        {
            if (!Visible)
            {
                return;
            }

            buttonEnter.Enabled = labelInput.Text.Length > 0;
            buttonClear.Enabled = labelInput.Text.Length > 0;
        }

        public void SetupPage(string title, Form tabSourceForm, Control controlReturnFocus, int maxCharacters)
        {
            labelTitle.Text = title;
            labelInput.Text = controlReturnFocus.Text;
            _maxLength = maxCharacters;
            _returnPage = tabSourceForm;
            _returnControlItem = controlReturnFocus;
        }

        #endregion

        #region Control Event Handlers

        private void ButtonShift_Click(object sender, EventArgs e)
        {
            TimeSpan timeSinceLastClick = DateTime.Now - _lastClick;
            _lastClick = DateTime.Now;

            if (timeSinceLastClick.TotalMilliseconds > 250)
            {
                Debug.WriteLine("Shift Button  SINGLE CLICK");
                _capsLocked = false;

                if (_shifted)
                {
                    _shifted = false;
                    ChangeCase(false);

                    buttonShift.Text = "Shift";
                    buttonShift.BackColor = System.Drawing.SystemColors.ControlLight;
                    buttonShift.ForeColor = System.Drawing.Color.Black;
                }
                else
                {
                    _shifted = true;
                    ChangeCase(true);

                    buttonShift.Text = "Caps";
                    buttonShift.BackColor = System.Drawing.SystemColors.Control;
                    buttonShift.ForeColor = System.Drawing.Color.Black;
                }
            }
            else
            {
                ButtonShift_DoubleClick(sender, e);
            }
        }

        private void ButtonShift_DoubleClick(object sender, EventArgs e)
        {
            Debug.WriteLine("Shift Button  DOUBLE CLICK");

            _shifted = true;
            _capsLocked = true;
            ChangeCase(true);

            buttonShift.Text = "Caps Lock";
            buttonShift.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            buttonShift.ForeColor = System.Drawing.Color.White;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _formMain.LastClick = DateTime.Now;
            _formMain.ShowLastForm();
        }

        private void buttonEnter_Click(object sender, EventArgs e)
        {
            _formMain.LastClick = DateTime.Now;

            _returnControlItem.Text = labelInput.Text;
            _returnControlItem.Refresh();

            _formMain.ShowLastForm();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            _formMain.LastClick = DateTime.Now;
            labelInput.Text = "";
        }

        private void buttonAlpha_Click(object sender, EventArgs e)
        {
            _formMain.LastClick = DateTime.Now;

            if (labelInput.Text.Length == _maxLength)
            {
                nRadMessageBox.Show(this, $"Maximum of {_maxLength} Characters", "Too Many Characters", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (((Button)sender).Text == "SPACE")
            {
                labelInput.Text += " ";
            }
            else
            {
                labelInput.Text += ((Button)sender).Text;

                if (!_capsLocked)
                {
                    buttonShift.Text = "Shift";
                    _shifted = false;
                    ChangeCase(false);
                }
            }
        }

        private void buttonBackspace_Click(object sender, EventArgs e)
        {
            if (labelInput.Text.Length > 0)
            {
                labelInput.Text = labelInput.Text.Substring(0, labelInput.Text.Length - 1);
            }
        }

        #endregion

        #region Private Functions

        private void ChangeCase(bool toUpper = false)
        {
            foreach (Button button in _alphaButtons)
            {
                button.Text = toUpper ? button.Text.ToUpper() : button.Text.ToLower();
            }
        }

        private bool IsLetterButton(Button button)
        {
            return button.Text.Length == 1 && char.IsLetter(button.Text[0]);
        }

        #endregion

        #endregion
    }
}
