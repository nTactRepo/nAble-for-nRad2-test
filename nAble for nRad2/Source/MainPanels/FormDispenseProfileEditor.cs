using nAble.Model.Recipes;
using nTact.DataComm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nAble.DataComm;
using nTact.Recipes;
using DevComponents.DotNetBar;
using System.Runtime.InteropServices;

namespace nAble
{
    public partial class FormDispenseProfileEditor : Form, IUpdateableForm
    {
        #region Constants

        #endregion

        #region Properties 
        public DynamicDispenseProfileParamList ParamList;
        public DispenseProfileEditorParams DispProfileParams;
        #endregion

        #region Data Members

        private readonly FormMain _frmMain = null;
        private readonly Form _returnForm = null;
        private int _dispenseProfileType;
        private bool newRateChange = false;
        private int rateChangeNo = 0;
        private double baseRate = 0;
        private double maxX = 0;

        #endregion

        #region Functions

        #region Constructors

        public FormDispenseProfileEditor(FormMain frmMain, Form returnForm, int disptype, DynamicDispenseProfileParamList dispParams, double baserate, double maxpos, DispenseProfileEditorParams profileEditorParams)
        {
            InitializeComponent();

            _frmMain = frmMain ?? throw new ArgumentNullException(nameof(frmMain));
            _returnForm = returnForm ?? throw new ArgumentNullException(nameof(returnForm));
            _dispenseProfileType = disptype;
            baseRate = baserate;
            maxX = maxpos;
            ParamList = dispParams ?? throw new ArgumentNullException(nameof(dispParams));
            DispProfileParams = profileEditorParams;
            InitializeFieldsFromParams();
            labelBaseRate.Text = baserate.ToString("#0.00");
        }
        #endregion

        #region Public Functions

        public void UpdateStatus()
        {
            buttonSaveRateChange.Enabled = newRateChange || rateChangeNo > 0;
            buttonSaveRateChange.Enabled = rateChangeNo > 0;
            buttonAddRateChange.Enabled = !newRateChange;
            buttonDeleteRateChange.Enabled = !newRateChange && rateChangeNo > 0;
            buttonFreestyleRate.Enabled = newRateChange || rateChangeNo > 0;
            buttonFreestylePos.Enabled = newRateChange || rateChangeNo > 0;
        }

        #endregion

        #region Private Functions
        private void InitializeFieldsFromParams()
        {
            switch (_dispenseProfileType)
            {
                case 1:
                    panelGradientProfile.Visible = true;
                    panelTriangleProfile.Visible = false;
                    panelTrapezoidalProfile.Visible = false;
                    panelFreestyleProfile.Visible = false;
                    buttonGradientFinalRate.Text = DispProfileParams.Param1.ToString("#0.0");
                    break;
                case 2:
                    panelGradientProfile.Visible = false;
                    panelTriangleProfile.Visible = true;
                    panelTrapezoidalProfile.Visible = false;
                    panelFreestyleProfile.Visible = false;
                    buttonTrianglePeakRate.Text = DispProfileParams.Param1.ToString("#0.0");
                    break;
                case 3:
                    panelGradientProfile.Visible = false;
                    panelTriangleProfile.Visible = false;
                    panelTrapezoidalProfile.Visible = true;
                    panelFreestyleProfile.Visible = false;
                    buttonTrapPeakRate.Text = DispProfileParams.Param1.ToString("#0.0");
                    buttonTrapAccDecPct.Text = DispProfileParams.Param2.ToString("#0");
                    break;
                case 4:
                    panelGradientProfile.Visible = false;
                    panelTriangleProfile.Visible = false;
                    panelTrapezoidalProfile.Visible = false;
                    panelFreestyleProfile.Visible = true;
                    foreach(DynamicDispenseProfileParam param in ParamList)
                    {
                        dataGridViewFreestyleProfile.Rows.Add(param.ArrayLocation+1,param.XPos.ToString("#0.000"),param.DispenseRate.ToString("#0.00"));
                    }
                    UpdateChart();
                    break;
            }
        }

        private void UpdateChart()
        {
            double lastpos = 0;
            double lastrate = 0;
            try
            {
                chartDispProfile.Series[0].Points.Clear();
                chartDispProfile.Series[0].Points.AddXY(0, baseRate);
                for (int i = 0; i < dataGridViewFreestyleProfile.Rows.Count; i++)
                {
                    double xvalue = double.Parse(dataGridViewFreestyleProfile.Rows[i].Cells[1].Value.ToString());
                    double yvalue = double.Parse(dataGridViewFreestyleProfile.Rows[i].Cells[2].Value.ToString());
                    chartDispProfile.Series[0].Points.AddXY(xvalue, yvalue);
                    lastpos = xvalue;
                    lastrate = yvalue;
                }
                if (lastpos < maxX)
                {
                    chartDispProfile.Series[0].Points.AddXY(maxX, lastrate);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating chart. Ex: " + ex.ToString());
            }
        }

        private void ExitForm()
        {
            _frmMain.LoadSubForm(_returnForm);
        }

        #endregion

        #endregion

        private void buttonOK_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            UpdateParamList();
            ExitForm();
        }

        private bool UpdateParamList()
        {
            bool result = true;
            try
            {
                switch (_dispenseProfileType)
                {
                    case 1:
                        DispProfileParams.Param1 = double.Parse(buttonGradientFinalRate.Text);
                        break;
                    case 2:
                        DispProfileParams.Param1 = double.Parse(buttonTrianglePeakRate.Text);
                        break;
                    case 3:
                        DispProfileParams.Param1 = double.Parse(buttonTrapPeakRate.Text);
                        DispProfileParams.Param2 = double.Parse(buttonTrapAccDecPct.Text);
                        break;
                    case 4:
                        DispProfileParams.Param1 = dataGridViewFreestyleProfile.Rows.Count;
                        ParamList.Clear();
                        for (int i = 0; i < dataGridViewFreestyleProfile.Rows.Count; i++)
                        {
                            double xPos = double.Parse(dataGridViewFreestyleProfile.Rows[i].Cells[1].Value.ToString());
                            double xRate = double.Parse(dataGridViewFreestyleProfile.Rows[i].Cells[2].Value.ToString());
                            DynamicDispenseProfileParam newParm = new DynamicDispenseProfileParam(xPos, xRate, 0, i);
                            ParamList.Add(newParm);
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                result = false;
                MessageBox.Show("Error Updating Params. Ex: " + ex.ToString());
            }
            return result;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            ExitForm();
        }

        private void buttonFreestyleRate_Click(object sender, EventArgs e)
        {
            Button thisbutton = (Button)sender;
            _frmMain.GotoNumScreen("New Dispense Rate (µL/s)", this, thisbutton, "#0.00", 0.01,_frmMain.MS.MaxPumpRate);
        }

        private void buttonFreestylePos_Click(object sender, EventArgs e)
        {
            Button thisbutton = (Button)sender;
            _frmMain.GotoNumScreen("X-Position (from Coat Start)", this, thisbutton, "0.000", 0, maxX, "");
        }

        private void buttonAddRateChange_Click(object sender, EventArgs e)
        {
            buttonFreestyleRate.Text = "0.0";
            buttonFreestylePos.Text = "0.000";
            newRateChange = true;
            groupBoxRateChange.Text = "Rate Change No: New";
            rateChangeNo = dataGridViewFreestyleProfile.Rows.Count + 1;
            groupBoxRateChange.Enabled = true;
        }

        private void buttonSaveRateChange_Click(object sender, EventArgs e)
        {
            string stepMsg = "";
            try
            {
                double xpos = double.Parse(buttonFreestylePos.Text);
                string rate = buttonFreestyleRate.Text;
                bool rowInserted = false;
                bool duplicate = false;
                int rowCount = dataGridViewFreestyleProfile.Rows.Count;
                if (newRateChange)
                {
                    if (rowCount == 0)
                    {
                        stepMsg = "Adding First Rate Change";
                        dataGridViewFreestyleProfile.Rows.Add();
                        dataGridViewFreestyleProfile.Rows[0].Cells[0].Value = 1;
                        dataGridViewFreestyleProfile.Rows[0].Cells[1].Value = xpos.ToString("#0.000");
                        dataGridViewFreestyleProfile.Rows[0].Cells[2].Value = rate;
                    }
                    else
                    {
                        stepMsg = "Cloning New Row";
                        DataGridViewRow newRate = (DataGridViewRow)dataGridViewFreestyleProfile.Rows[0].Clone();
                        newRate.Cells[0].Value = rowCount + 1;
                        newRate.Cells[1].Value = xpos.ToString("#0.000");
                        newRate.Cells[2].Value = rate;
                        int totrows = rowCount;
                        for (int i = 0; i < totrows; i++)
                        {
                            if (!rowInserted && xpos < double.Parse(dataGridViewFreestyleProfile.Rows[i].Cells[1].Value.ToString()))
                            {
                                newRate.Cells[0].Value = i + 1;
                                stepMsg = "Inserting New Rate Change";
                                dataGridViewFreestyleProfile.Rows.Insert(i, newRate);
                                totrows++;
                                rowInserted = true;
                            }
                            else if(!rowInserted && xpos == double.Parse(dataGridViewFreestyleProfile.Rows[i].Cells[1].Value.ToString()))
                            {
                                duplicate = true; break;
                            }
                            if (rowInserted)
                            {
                                stepMsg = "Updating Rate Change No";
                                dataGridViewFreestyleProfile.Rows[i].Cells[0].Value = i + 1;
                            }
                        }
                        if (!rowInserted && rowCount > 0)
                        {
                            stepMsg = "Adding New Rate Change";
                            dataGridViewFreestyleProfile.Rows.Add(newRate);
                        }
                    }
                    if (duplicate)
                        MessageBox.Show("Duplicate X-Pos! Rate Change Not added.");
                    newRateChange = false;
                }
                else
                {
                    dataGridViewFreestyleProfile.Rows[rateChangeNo - 1].Cells[1].Value = buttonFreestylePos.Text;
                    dataGridViewFreestyleProfile.Rows[rateChangeNo - 1].Cells[2].Value = buttonFreestyleRate.Text;
                }
                buttonFreestyleRate.Text = "0.0";
                buttonFreestylePos.Text = "0.000";
                groupBoxRateChange.Text = "Rate Change No:";
                rateChangeNo = 0;
                SortByXPos();
                UpdateChart();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error Saving while {stepMsg}! Ex: " + ex.ToString());
            }
        }

        private bool SortByXPos()
        {
            bool result = true;
            int iCnt = dataGridViewFreestyleProfile.Rows.Count;
            int i;
            for (i = 0; i < iCnt; i++)
            {
                for (int j = 0; j < iCnt - 1; j++)
                {
                    double xPos1 = double.Parse(dataGridViewFreestyleProfile.Rows[j].Cells[1].Value.ToString());
                    double xPos2 = double.Parse(dataGridViewFreestyleProfile.Rows[j+1].Cells[1].Value.ToString());
                    double rate1 = double.Parse(dataGridViewFreestyleProfile.Rows[j].Cells[2].Value.ToString());
                    double rate2 = double.Parse(dataGridViewFreestyleProfile.Rows[j + 1].Cells[2].Value.ToString());
                    if (xPos1 > xPos2)
                    {
                        dataGridViewFreestyleProfile.Rows[j].Cells[1].Value = xPos2.ToString("#0.000");
                        dataGridViewFreestyleProfile.Rows[j].Cells[2].Value = rate2.ToString("#0.00");
                        dataGridViewFreestyleProfile.Rows[j+1].Cells[1].Value = xPos1.ToString("#0.000");
                        dataGridViewFreestyleProfile.Rows[j+1].Cells[2].Value = rate1.ToString("#0.00");
                    }
                }
            }
            return result;
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void dataGridViewFreestyleProfile_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewFreestyleProfile.SelectedRows.Count == 0) return;
            int iRow = dataGridViewFreestyleProfile.SelectedRows[0].Index;
            if (iRow >= 0)
            {
                rateChangeNo = int.Parse(dataGridViewFreestyleProfile.Rows[iRow].Cells[0].Value.ToString());
                buttonFreestylePos.Text = dataGridViewFreestyleProfile.Rows[iRow].Cells[1].Value.ToString();
                buttonFreestyleRate.Text = dataGridViewFreestyleProfile.Rows[iRow].Cells[2].Value.ToString();
                groupBoxRateChange.Text = "Rate Change No: " + rateChangeNo.ToString();
            }
        }

        private void buttonDeleteRateChange_Click(object sender, EventArgs e)
        {
            dataGridViewFreestyleProfile.Rows.RemoveAt(rateChangeNo - 1);
            RenumberRows();
            UpdateChart();
        }

        private void RenumberRows()
        {
            for(int i = 0;i<dataGridViewFreestyleProfile.Rows.Count;i++)
            {
                dataGridViewFreestyleProfile.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void buttonGradientFinalRate_Click(object sender, EventArgs e)
        {
            Button thisbutton = (Button)sender;
            _frmMain.GotoNumScreen("Final Dispense Rate (µL/s)", this, thisbutton, "#0.00", 0.01, _frmMain.MS.MaxPumpRate);
        }

        private void buttonTrapPeakRate_Click(object sender, EventArgs e)
        {
            Button thisbutton = (Button)sender;
            _frmMain.GotoNumScreen("Peak Dispense Rate (µL/s)", this, thisbutton, "#0.00", 0.01, _frmMain.MS.MaxPumpRate);
        }

        private void buttonTrapAccDecPct_Click(object sender, EventArgs e)
        {
            Button thisbutton = (Button)sender;
            _frmMain.GotoNumScreen("Accel/Decel % Of Profile", this, thisbutton, "#0", 1, 49);
        }

        private void buttonTrianglePeakRate_Click(object sender, EventArgs e)
        {
            Button thisbutton = (Button)sender;
            _frmMain.GotoNumScreen("Peak Dispense Rate (µL/s)", this, thisbutton, "#0.00", 0.01, _frmMain.MS.MaxPumpRate);
        }
    }
}
