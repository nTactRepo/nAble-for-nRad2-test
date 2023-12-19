using System;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using nTact.PLC;
using System.Collections.Generic;
using nTact.DataComm;

namespace nAble
{
    public partial class FormAlarms : Form, IUpdateableForm
    {
        private GalilWrapper2 MC => _frmMain.MC;

        private readonly FormMain _frmMain = null;
        private readonly Dictionary<int, string> _errors = null;

        private DataTable _activityLog;
        int _lastErrors = 0;
        int _lastStackErrorCount = 0;

        public FormAlarms(FormMain formMain)
        {
            InitializeComponent();

            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));

            _errors = new Dictionary<int, string>
            {
                { 0, "General - Reset Required" },
                { 1, "System Stopped Due to Main Air Loss" },
                { 2, "Init Failed - X Axis " },
                { 3, "Init Failed - ZR Axis " },
                { 4, "Init Failed - ZL Axis" },
                { 5, "Init Failed - Pump" },
                { 6, "Init Failed - Valve" },
                { 7, "Init Failed - Loader" },
                { 8, "Light Curtain Tripped" },
                { 9, "EMO Pressed" },
                { 10, "AMP ERROR" },
                { 11, "Command Error" },
                { 12, "X Motor was off when required" },
                { 13, "Z Motor(s) were off when required" },
                { 14, "Pump Motor was off when required" },
                { 16, "Recipe Failed - Vac Loss" },
                { 17, "Initialization Failure" },

                { 20, "X - Axis Pos Error" },
                { 21, "RZ - Axis Pos Error" },
                { 22, "LZ - Axis Pos Error" },
                { 23, "Pump Pos Error" },
                { 24, "Loader Pos Error" }
            };

            buttonResetAlarms.Visible = _frmMain.MS.HasStack;
        }

        public void UpdateStatus()
        {
            if (!Visible)
            {
                return;
            }

            // TODO  Make this smarter on stack errors.  Only based on count, not on individual alarms.
            bool displayAlarms = _lastErrors != (int)MC.Memory[0] || _lastStackErrorCount != _frmMain.PLC.StackAlarmCount;
            _lastErrors = (int)MC.Memory[0];
            _lastStackErrorCount = _frmMain.PLC.StackAlarmCount;

            if (displayAlarms)
            {
                DisplayErrors();
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            _frmMain.ShowLastForm();
        }

        private void FormAlarms_Load(object sender, EventArgs e)
        {
            CreateDataTables();
        }
        private void CreateDataTables()
        {
            _activityLog = new DataTable();
            _activityLog.Columns.Add("No.", typeof(int));
            _activityLog.Columns.Add("Cat.");
            _activityLog.Columns.Add("Description");

            dataGridViewActivityHistory.DataSource = _activityLog;
            dataGridViewActivityHistory.Columns[0].Width = 100;
            dataGridViewActivityHistory.Columns[1].Width = 100;
            dataGridViewActivityHistory.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewActivityHistory.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewActivityHistory.AllowUserToResizeColumns = true;
        }

        private void DisplayErrors()
        {
            _activityLog.Clear();

            if (_lastErrors == 0)
            {
                _activityLog.Rows.Add(-1, "Coater", "No Coater Errors");
            }
            else
            {
                // Motion Controller Errors
                for (int row = 0; row < 32; row++)
                {
                    int flag = 0x1 << row;

                    if ((_lastErrors & flag) == flag)
                    {
                        if (row == 11)
                        {
                            string error = MC.LastCommandError.Trim();
                            string extra = error.Length > 0 ? "'{MC.LastCommandError}'" : "";
                            _activityLog.Rows.Add(row, "Coater", $"{_errors[row]} - Line #{MC.CommandErrorLineNum}{extra}");
                            
                            string lineData = MC.GetCommandText(MC.CommandErrorLineNum);
                            _activityLog.Rows.Add(row, "Coater", $"Line #{lineData}");
                        }
                        else
                        {
                            _activityLog.Rows.Add(row, "Coater", _errors[row]);
                        }
                    }
                }

            }

            //PLC Errors....
            if (_frmMain.MS.HasStack)
            {
                if (_lastStackErrorCount == 0)
                {
                    _activityLog.Rows.Add(-1, "Stack", "No Stack Alarms");
                }
                else
                {
                    for (int row = 0; row < _frmMain.PLC.MaxAlarmNumber; row++)
                    {
                        if (_frmMain.PLC.AlarmExists(row))
                        {
                            Alarm curAlarm = _frmMain.PLC.GetAlarm(row);

                            _activityLog.Rows.Add(row+1, "Stack", curAlarm?.Message);
                        }
                    }
                }
            }
        }

        private void buttonResetAlarms_Click(object sender, EventArgs e)
        {
            _frmMain.PLC.ResetStackError();
        }
    }
}
