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
    public partial class FormTimeInput : Form, IUpdateableForm
    {
        private FormMain _formMain = null;
        private Form _returnPage = null;
        private Control _returnControlItem = null;

		public string NewValue { get; set; } = "00:00:00";
		public bool HoursMinsOnly { get; set; } = false;

        public FormTimeInput(FormMain formMain)
        {
            _formMain = formMain;
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
			Button curButton = (Button)sender;
			textBoxTimeInput.Text = GenerateNewDisplayString(curButton.Text);
			EnableNumbers(textBoxTimeInput.Text.Length < 9);
		}

		private void EnableNumbers(bool bEnable)
		{
			button0.Enabled = bEnable;
			button1.Enabled = bEnable;
			button2.Enabled = bEnable;
			button3.Enabled = bEnable;
			button4.Enabled = bEnable;
			button5.Enabled = bEnable;
			button6.Enabled = bEnable;
			button7.Enabled = bEnable;
			button8.Enabled = bEnable;
			button9.Enabled = bEnable;
		}

		private string _sNums = "";
		private string GenerateNewDisplayString(string nextVal)
		{
			string sTemp, sRetVal = NewValue;

			// leading zeros are ignored
			if (_sNums.Length == 0 && nextVal == "0")
				return sRetVal;

			// not a leading zero so append it
			if (HoursMinsOnly && _sNums.Length == 4)
				_sNums = _sNums.Substring(1, 3);
			_sNums += nextVal;

			// make it look purdy
			if (_sNums.Length > 4)
			{
				sTemp = _sNums;
				sTemp = sTemp.Insert(_sNums.Length - 2, ":");
				sTemp = sTemp.Insert(sTemp.Length - 5, ":");
				sRetVal = sTemp.PadLeft(8, '0');
			}
			else if (_sNums.Length > 2)
			{
				sTemp = _sNums;
				sTemp = sTemp.Insert(_sNums.Length - 2, ":");
				if (HoursMinsOnly)
					sRetVal = sTemp.PadLeft(5, '0');
				else
					sRetVal = "00:" + sTemp.PadLeft(5, '0');
			}
			else
			{
				if (HoursMinsOnly)
					sRetVal = "00:" + _sNums.PadLeft(2, '0');
				else
					sRetVal = "00:00:" + _sNums.PadLeft(2, '0');
			}

			return sRetVal;
		}


        private void buttonClear_Click(object sender, EventArgs e)
        {
			_formMain.LastClick = DateTime.Now;

			textBoxTimeInput.Text = "00:00:00";
			_sNums = "";
			EnableNumbers(true);
			buttonEnter.Enabled = true;
            labelErrNumInput.Text = "";
        }

        private void buttonEnter_Click(object sender, EventArgs e)
        {
            _formMain.LastClick = DateTime.Now;
			_returnControlItem.Text = textBoxTimeInput.Text;
			_returnControlItem.Refresh();
            _formMain.ShowLastForm();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _formMain.LastClick = DateTime.Now;
            _formMain.ShowLastForm();
        }

        private void buttonPeriod_Click(object sender, EventArgs e)
        {
            _formMain.LastClick = DateTime.Now;
            textBoxTimeInput.Text += ((Button)sender).Tag.ToString();
        }

        public void SetupPage(string sTitle, Form tabSourceForm, Control controlReturnFocus)
        {
            labelTimeTitle.Text = sTitle;
			labelPrevious.Text = $"Previous Value: {controlReturnFocus.Text}";
            textBoxTimeInput.Text = "00:00:00";
			_sNums = "";
			textBoxTimeInput.Tag = 0.0;
            labelErrNumInput.Text = "";
            _returnPage = tabSourceForm;
            _returnControlItem = controlReturnFocus;
            textBoxTimeInput.Focus();
        }

		private void textBoxTimeInput_TextChanged(object sender, EventArgs e)
		{
			if (HoursMinsOnly)
			{
				if (Convert.ToInt16(textBoxTimeInput.Text.Substring(0, 2)) < 24 && Convert.ToInt16(textBoxTimeInput.Text.Substring(3, 2)) < 60)
				{
					labelErrNumInput.Text = "";
					NewValue = textBoxTimeInput.Text;
					buttonEnter.Enabled = true;
				}
				else
				{
					labelErrNumInput.Text = "Selected Time Is Invalid. Valid times are in the range of 00:00 to 23:59.";
					buttonEnter.Enabled = false;
				}
			}
			else
			{
				if (Convert.ToInt16(textBoxTimeInput.Text.Substring(0, 2)) < 24 && Convert.ToInt16(textBoxTimeInput.Text.Substring(3, 2)) < 60 && Convert.ToInt16(textBoxTimeInput.Text.Substring(6, 2)) < 60)
				{
					labelErrNumInput.Text = "";
					NewValue = textBoxTimeInput.Text;
					buttonEnter.Enabled = true;
				}
				else
				{
					labelErrNumInput.Text = "Selected Time Is Invalid. Valid times are in the range of 00:00:00 to 23:59:59.";
					buttonEnter.Enabled = false;
				}
			}

		}
	}
}
