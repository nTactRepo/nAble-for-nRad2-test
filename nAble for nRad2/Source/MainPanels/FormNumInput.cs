using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace nAble
{
    public partial class FormNumInput : Form, IUpdateableForm
    {
        private FormMain _formMain = null;
        private readonly LogEntry _log = null;
        
        private Form _returnPage = null;
        private Control _returnControlItem = null;
        private string _numFormat;
        private string _altText;
        private string _returnValue;
        private double _dMaxNumInput, _dMinNumInput;

        public FormNumInput(FormMain formMain, LogEntry log)
        {
            _formMain = formMain;
            _log = log;

            InitializeComponent();
        }

        public void UpdateStatus()
        {
            if (_formMain == null)
                return;
        }

        private void buttonNum_Click(object sender, EventArgs e)
        {
            _formMain.LastClick = DateTime.Now;
            textBoxNumInput.Text += ((Button)sender).Tag.ToString();
        }

        private void textBoxNumInput_TextChanged(object sender, EventArgs e)
        {
            _formMain.LastClick = DateTime.Now;
            Double dTemp = 0.0;
            try
            {
                textBoxNumInput.Tag = 0.0;
                if (textBoxNumInput.Text.Trim() != "" && textBoxNumInput.Text.Trim() != "." && textBoxNumInput.Text.Trim() != "-")
                {
                    try
                    {
						dTemp = Convert.ToDouble(textBoxNumInput.Text);
						labelErrNumInput.Text = "";
						buttonEnter.Enabled = (dTemp == _dMinNumInput) || (dTemp == _dMaxNumInput) || (dTemp >= _dMinNumInput && dTemp <= _dMaxNumInput);
						if (buttonEnter.Enabled)
						{
							textBoxNumInput.Tag = dTemp;
						}
						else
						{
							labelErrNumInput.Text = String.Format("Number must be between {0} and {1}", _dMinNumInput.ToString(_numFormat), _dMaxNumInput.ToString(_numFormat));
						}
					}
					catch (FormatException)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "Format Exception... not a number '" + textBoxNumInput.Text + "'", "ERROR");
                        labelErrNumInput.Text = "Only Numbers Allowed";
                        buttonEnter.Enabled = false;
                    }
                }
                else
                {
                    buttonEnter.Enabled = false;
                }
            }
            catch (FormatException)
            {
                labelErrNumInput.Text = "Invalid Number Format";
                buttonEnter.Enabled = false;
            }
            if (_numFormat == "S")
                _returnValue = textBoxNumInput.Text;
            else
                _returnValue = dTemp.ToString(_numFormat);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            _formMain.LastClick = DateTime.Now;
            buttonEnter.Enabled = false;
            textBoxNumInput.Text = "";
            labelErrNumInput.Text = "";
        }

        private void buttonEnter_Click(object sender, EventArgs e)
        {
            _formMain.LastClick = DateTime.Now;

            if (_altText == "S")
            {
                _returnControlItem.Text = textBoxNumInput.Text;
            }
            else if (_altText == "")
            {
                _returnControlItem.Text = ((double)textBoxNumInput.Tag).ToString(_numFormat);
            }
            else
            {
                _returnControlItem.Text = string.Format(_altText, ((double)textBoxNumInput.Tag).ToString(_numFormat));
            }

            _returnControlItem.Refresh();
            _formMain.ShowLastForm(confirmExit: false);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _formMain.LastClick = DateTime.Now;
            _formMain.ShowLastForm(confirmExit: false);
        }

        private void buttonPeriod_Click(object sender, EventArgs e)
        {
            _formMain.LastClick = DateTime.Now;
            textBoxNumInput.Text += ((Button)sender).Tag.ToString();
        }

        public void SetupPage(String sTitle, Form tabSourceForm, Control controlReturnFocus, String sNumFormat, Double dMin, Double dMax, String sAltText = "", Boolean bEnablePeriod = true, Boolean bIsPassword = false)
        {
            labelNumTitle.Text = sTitle;
            textBoxNumInput.UseSystemPasswordChar = bIsPassword;
            textBoxNumInput.Text = "";
            textBoxNumInput.Tag = 0.0;
            labelErrNumInput.Text = "";
            _returnPage = tabSourceForm;
            _returnControlItem = controlReturnFocus;
            _numFormat = sNumFormat;
            _altText = sAltText;
            _dMaxNumInput = dMax;
            _dMinNumInput = dMin;
            labelInputRange.Text = String.Format("Min: {0}, Max: {1}", _dMinNumInput.ToString(_numFormat), _dMaxNumInput.ToString(_numFormat));
            if (_dMinNumInput < 0)
                buttonPlusMinus.Enabled = true;    
            buttonPeriod.Enabled = bEnablePeriod;
            textBoxNumInput.Focus();
        }

        private void FormNumInput_Load(object sender, EventArgs e)
        {

        }

        private void buttonPlusMinus_Click(object sender, EventArgs e)
        {
            if (textBoxNumInput.Text.StartsWith("-"))
                textBoxNumInput.Text = textBoxNumInput.Text.Remove(0, 1);
            else
                textBoxNumInput.Text = "-" + textBoxNumInput.Text;
        }
    }
}
