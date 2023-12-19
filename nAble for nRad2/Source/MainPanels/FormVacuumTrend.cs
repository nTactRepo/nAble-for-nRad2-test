using DevComponents.DotNetBar.Charts;
using DevComponents.DotNetBar.Charts.Style;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormVacuumTrend : Form, IUpdateableForm
    {
        #region Inner Classes

        public class VacDryRecipe
        {
            public double Ambient { get; set; } = 764;
            
            public int Step1Secs { get; set; } = 0;
            public int Step1Angle { get; set; } = 0;
            public double Step1SetPoint { get; set; } = 0;
            
            public int Step2Secs { get; set; } = 0;
            public int Step2Angle { get; set; } = 0;
            public double Step2SetPoint { get; set; } = 0;
            
            public int Step3Secs { get; set; } = 0;
            public double Step3SetPoint { get; set; } = 0;
            public int Step3Angle { get; set; } = 0;
            
            public int Step4Secs { get; set; } = 0;
            public double Step4SetPoint { get; set; } = 0;
            public int Step4Angle { get; set; } = 0;
            
            public int Step5DwellSecs { get; set; } = 0;
            public int Step5MaxSecs { get; set; } = 0;
            public double Step5SetPoint { get; set; } = 0;
            public int Step5Angle { get; set; } = 0;

            public int InitialVacReliefTime { get; set; } = 0;
            public double InitialVacReliefSetpoint { get; set; } = 0;
            public int FullVacReliefTime { get; set; } = 0;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                TimeSpan time = new TimeSpan(0, 0, Step1Secs);
                sb.AppendLine($"Step 1 Time: {time:hh:mm:ss}");
                sb.AppendLine($"Step 1 Angle: {Step1Angle:0.0}");
                sb.AppendLine($"Step 1 Setpoint {Step1SetPoint:0.0}");
                
                time = new TimeSpan(0, 0, Step2Secs);
                sb.AppendLine($"Step 2 Setpoint {time:hh:mm:ss}");
                sb.AppendLine($"Step 2 Angle: {Step2Angle:0.0}");
                sb.AppendLine($"Step 1 Setpoint {Step2SetPoint:0.0}");
                
                time = new TimeSpan(0, 0, Step3Secs);
                sb.AppendLine($"Step 3 Time: {time:hh:mm:ss}");
                sb.AppendLine($"Step 3 Setpoint: {Step3SetPoint:0.0}");
                sb.AppendLine($"Step 3 Angle: {Step3Angle:0.0}");
                
                time = new TimeSpan(0, 0, Step4Secs);
                sb.AppendLine($"Step 4 Time: {time:hh:mm:ss}");
                sb.AppendLine($"Step 4 Setpoint: {Step4SetPoint:0.0}");
                sb.AppendLine($"Step 4 Angle: {Step4Angle:0.0}");
                
                time = new TimeSpan(0, 0, Step5DwellSecs);
                sb.AppendLine($"Step 5 Dwell Time: {time:hh:mm:ss}");
                time = new TimeSpan(0, 0, Step5MaxSecs);
                sb.AppendLine($"Step 5 Max Time: {time:hh:mm:ss}");
                sb.AppendLine($"Step 5 Setpoint: {Step5SetPoint:0.0}");
                sb.AppendLine($"Step 5 Angle: {Step5Angle:0.0}");
                
                time = new TimeSpan(0, 0, InitialVacReliefTime);
                sb.AppendLine($"Initial Vac Relief Time: {time:hh:mm:ss}");

                return sb.ToString();
            }
        }

        #endregion

        #region Constants

        private const string BaseDirectory = @"Data\TrendingData";

        #endregion

        #region Fields

        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;

        private ChartControl _chartControl = null;

        // Data Recording Stuffs
        private System.Windows.Forms.Timer _timerRecorder = new System.Windows.Forms.Timer();
        private bool _recording = false;
        private bool _inProcess = false;
        private int _lastRecordedSecs;

        ChartSeries _chartSeriesExpected = null;
        ChartSeries _chartSeriesValveAngle = null;
        ChartSeries _chartSeriesRealtime = null;
        ChartSeries _chartSeriesFakeData = null;

#if USEFAKEVACDATA // using real data
        int _nLastFakeSecond = -1;
#endif
        int _nFakeSecondCounter = -1;

        DateTime _processStartTime;

        #endregion

        #region Functions

        public FormVacuumTrend(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain;
            _log = log;

            SuspendLayout();  // pause screen draw (prevents flashes)

            InitializeComponent();

            SetupChart();  // no param sets up the 'example' chart

            Controls.Add(_chartControl);

            _chartControl.SendToBack();

            ResumeLayout(false); // resume screen draw 

            _timerRecorder.Tick += _timerRecorder_Tick;
            _timerRecorder.Enabled = false;

        }

        internal void StartTimer(int nInterval = 250)
        {
            _timerRecorder.Interval = nInterval;
            _timerRecorder.Enabled = true;
        }

        private void _timerRecorder_Tick(object sender, EventArgs e)
        {
            double dPressure = 0.0, dValveAngle = 0.0;
            int nProcessSecs = _nFakeSecondCounter;

            // figure out if using real or fake data
#if !USEFAKEVACDATA // using real data
            {
                dPressure = _frmMain.PLC.Mod2CurVacLevel; //plcFunctions.TwosCompD(_sVDVacLevel) / 1000.0;
                _inProcess = _frmMain.PLC.Mod2VacDryInProcess; //plcFunctions.GetState(_sVDInProcess);
                nProcessSecs = _frmMain.PLC.Mod2VacDryProcessTimeout; //plcFunctions.GetWord(_sVacDryProcessTimeSeconds);
                dValveAngle = _frmMain.PLC.Mod2VacValveAngle; //plcFunctions.GetWord(_sVDVacAngle) / _valveAngleMulti;
            }
#else       // using fake data
            {
                if (_bRecording)
                {
                    if (_nLastFakeSecond != DateTime.Now.Second)
                    {
                        _nLastFakeSecond = DateTime.Now.Second;
                        _nFakeSecondCounter++;
                        nProcessSecs = _nFakeSecondCounter;
                        checkBoxFakeProcessActive.Checked = _bInProcess;
                        if (nProcessSecs < ChartSeriesFakeData.SeriesPoints.Count)
                            dPressure = (double)ChartSeriesFakeData.SeriesPoints[nProcessSecs].ValueY[0];
                        else if (nProcessSecs >= ChartSeriesFakeData.SeriesPoints.Count)
                            dPressure = (double)ChartSeriesFakeData.SeriesPoints[ChartSeriesFakeData.SeriesPoints.Count-1].ValueY[0];
                        if (nProcessSecs == ChartSeriesFakeData.SeriesPoints.Count)
                        {
                            _bInProcess = false;
                            checkBoxFakeProcessActive.Checked = false;
                        }
                    }
                }
                else
                {
                    _nLastFakeSecond = -1;
                    _nFakeSecondCounter = -1;
                    if (ChartSeriesFakeData != null)
                        dPressure = (double)ChartSeriesFakeData.SeriesPoints[0].ValueY[0];
                    else
                        dPressure = 764;
                }
            }
#endif

            // determine what to do w/ data
            if (_recording) // we are/were recording
            {
                if (_inProcess)  // we are recording and should continue to record
                {
                    if (_lastRecordedSecs != nProcessSecs)
                    {
                        if (nProcessSecs == 0)
                        {
                            Debug.WriteLine("Start of new Realtime Series!");
                        }
                        SeriesPoint newPoint = new SeriesPoint(nProcessSecs, dPressure);
                        newPoint.Tag = dValveAngle;
                        _chartSeriesRealtime.SeriesPoints.Add(newPoint);
                        newPoint = new SeriesPoint(nProcessSecs, dValveAngle);
                        _chartSeriesValveAngle.SeriesPoints.Add(newPoint);
                        _lastRecordedSecs = nProcessSecs;
                        Debug.WriteLine(string.Format("Recording: {0} : {1}", nProcessSecs, dPressure.ToString("#.##")));
                    }
                }
                else  // we are recording but now should stop
                {
                    if (_lastRecordedSecs != nProcessSecs)
                    {
                        SeriesPoint newPoint = new SeriesPoint(nProcessSecs, dPressure);
                        _chartSeriesRealtime.SeriesPoints.Add(newPoint);
                        newPoint.Tag = dValveAngle;
                        newPoint = new SeriesPoint(nProcessSecs, dValveAngle);
                        _chartSeriesValveAngle.SeriesPoints.Add(newPoint);
                    }
                    WriteProcessTrendingData(_processStartTime);
                    _recording = false;
                    Trace.Listeners[1].WriteLine("New VacDataRecording ended at " + DateTime.Now.ToString());
                }
            }
            else // we are not recording
            {
                if (_inProcess)  // we are not recording but should start
                {
                    _lastRecordedSecs = -1;
                    _processStartTime = DateTime.Now;

                    StartNewRealtimeSeries();

                    SeriesPoint newPoint = new SeriesPoint(nProcessSecs, dPressure);
                    _chartSeriesRealtime.SeriesPoints.Add(newPoint);
                    newPoint.Tag = dValveAngle;
                    newPoint = new SeriesPoint(nProcessSecs, dValveAngle);
                    _chartSeriesValveAngle.SeriesPoints.Add(newPoint);

                    _recording = true;

                    SetupExpectedChartData();
                    Trace.Listeners[1].WriteLine("New VacDataRecording started at " + _processStartTime.ToString());
                    Debug.WriteLine(string.Format("Recording: {0} : {1}", nProcessSecs, dPressure.ToString("#.##")));
                    ChartXy chartXy = (ChartXy)_chartControl.ChartPanel.ChartContainers[0];
                    chartXy.ChartSeries.Add(_chartSeriesExpected);
                    AddRealTimeActualSeries(chartXy);
                }
            }
        }

        internal void StartNewRealtimeSeries()
        {
            if (_chartSeriesRealtime != null)
            {
                _chartSeriesRealtime.SeriesPoints.Clear();
                _chartSeriesRealtime = null;

                ChartXy chartXy = (ChartXy)_chartControl.ChartPanel.ChartContainers[0];
                if (chartXy.ChartSeries.Count == 2)
                {
                    chartXy.ChartSeries.RemoveAt(1);
                    Debug.WriteLine("Removed Existing Realtime chart series");
                }
            }
            if (_chartSeriesValveAngle != null)
            {
                _chartSeriesValveAngle.SeriesPoints.Clear();
                _chartSeriesValveAngle = null;
            }

            _chartSeriesRealtime = new ChartSeries("Actual", SeriesType.Line);
            _chartSeriesRealtime.ChartSeriesVisualStyle.LineStyle.LineWidth = 3;
            _chartSeriesRealtime.ChartSeriesVisualStyle.LineStyle.LinePattern = LinePattern.Solid;
            _chartSeriesRealtime.DataLabelVisualStyle.Background = new Background(Color.FromArgb(150, Color.LightGreen));
            _chartSeriesRealtime.DataLabelVisualStyle.TextAlignment = LineAlignment.Center;
            _chartSeriesRealtime.DataLabelVisualStyle.HighlightBackground = new Background(Color.Yellow);
            PointMarkerVisualStyle pstyle = _chartSeriesRealtime.ChartSeriesVisualStyle.MarkerVisualStyle;
            pstyle.Background = new Background(Color.Red);
            pstyle.BorderColor = Color.Black;
            pstyle.Type = PointMarkerType.Ellipse;
            pstyle.Size = new Size(9, 9);

            _chartSeriesValveAngle = new ChartSeries("Valve Angle", SeriesType.Line);

            SetupExpectedChartData();
            //AddRealTimeActualSeries();

        }

        internal void WriteProcessTrendingData(DateTime ProcessStartTime)
        {
            DataTable newChart = new DataTable("");
            DateTime st = ProcessStartTime;
            string dirName = $@"{BaseDirectory}\{st:yyyyMM}";
            string fileName = $@"{dirName}\{st:yyyyMMdd}_{st:HHmmss}.xml";

            try
            {
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }

                DataTable dtWriter = new DataTable("VacDryData");
                dtWriter.ReadXmlSchema(@"Data\VacDryDataSchema.xml");
                //dtWriter.ExtendedProperties.Add("Recipe", _curVacDryRecipe.ToString());

                TimeSpan tsDiplay = new TimeSpan(0, 0, _curVacDryRecipe.Step1Secs);
                dtWriter.ExtendedProperties.Add("Step1Time", tsDiplay.ToString("c"));
                dtWriter.ExtendedProperties.Add("Step1Angle", _curVacDryRecipe.Step1Angle.ToString("0.0"));
                dtWriter.ExtendedProperties.Add("Step1Setpoint", _curVacDryRecipe.Step1SetPoint.ToString("0.0"));
                tsDiplay = new TimeSpan(0, 0, _curVacDryRecipe.Step2Secs);
                dtWriter.ExtendedProperties.Add("Step2Time", tsDiplay.ToString("c"));
                dtWriter.ExtendedProperties.Add("Step2Angle", _curVacDryRecipe.Step2Angle.ToString("0.0"));
                dtWriter.ExtendedProperties.Add("Step2Setpoint", _curVacDryRecipe.Step2SetPoint.ToString("0.0"));
                tsDiplay = new TimeSpan(0, 0, _curVacDryRecipe.Step3Secs);
                dtWriter.ExtendedProperties.Add("Step3Time", tsDiplay.ToString("c"));
                dtWriter.ExtendedProperties.Add("Step3Setpoint", _curVacDryRecipe.Step3SetPoint.ToString("0.0"));
                dtWriter.ExtendedProperties.Add("Step3Angle", _curVacDryRecipe.Step3Angle.ToString("0.0"));
                tsDiplay = new TimeSpan(0, 0, _curVacDryRecipe.Step4Secs);
                dtWriter.ExtendedProperties.Add("Step4Time", tsDiplay.ToString("c"));
                dtWriter.ExtendedProperties.Add("Step4Setpoint", _curVacDryRecipe.Step4SetPoint.ToString("0.0"));
                dtWriter.ExtendedProperties.Add("Step4Angle", _curVacDryRecipe.Step4Angle.ToString("0.0"));
                tsDiplay = new TimeSpan(0, 0, _curVacDryRecipe.Step5DwellSecs);
                dtWriter.ExtendedProperties.Add("Step5DwellTime", tsDiplay.ToString("c"));
                tsDiplay = new TimeSpan(0, 0, _curVacDryRecipe.Step5MaxSecs);
                dtWriter.ExtendedProperties.Add("Step5MaxTime", tsDiplay.ToString("c"));
                dtWriter.ExtendedProperties.Add("Step5Setpoint", _curVacDryRecipe.Step5SetPoint.ToString("0.0"));
                dtWriter.ExtendedProperties.Add("Step5Angle", _curVacDryRecipe.Step5Angle.ToString("0.0"));
                tsDiplay = new TimeSpan(0, 0, _curVacDryRecipe.InitialVacReliefTime);
                dtWriter.ExtendedProperties.Add("InitialVacReliefTime", tsDiplay.ToString("c"));


                int nMaxRow = _chartSeriesRealtime.SeriesPoints.Count > _chartSeriesExpected.SeriesPoints.Count ? _chartSeriesRealtime.SeriesPoints.Count : _chartSeriesExpected.SeriesPoints.Count;
                DataRow drNewRow;
                double dLastExpected = 0.0, dLastActual = 0.0, dLastValveAngle = 0.0;

                for (int i = 0; i <= nMaxRow; i++)
                {
                    drNewRow = dtWriter.NewRow();
                    drNewRow["Time"] = i;
                    if (i < _chartSeriesExpected.SeriesPoints.Count)
                    {
                        drNewRow["Expected"] = (double)_chartSeriesExpected.SeriesPoints[i].ValueY[0];
                        dLastExpected = (double)_chartSeriesExpected.SeriesPoints[i].ValueY[0];
                    }
                    else
                    {
                        drNewRow["Expected"] = dLastExpected;
                    }
                    if (i < _chartSeriesRealtime.SeriesPoints.Count)
                    {
                        drNewRow["Actual"] = (double)_chartSeriesRealtime.SeriesPoints[i].ValueY[0];
                        dLastActual = (double)_chartSeriesRealtime.SeriesPoints[i].ValueY[0];
                    }
                    else
                    {
                        drNewRow["Actual"] = dLastActual;
                    }
                    if (i < _chartSeriesValveAngle.SeriesPoints.Count)
                    {
                        drNewRow["ValveAngle"] = (double)_chartSeriesValveAngle.SeriesPoints[i].ValueY[0];
                        dLastValveAngle = (double)_chartSeriesValveAngle.SeriesPoints[i].ValueY[0];
                    }
                    else
                    {
                        drNewRow["ValveAngle"] = dLastValveAngle;
                    }
                    dtWriter.Rows.Add(drNewRow);
                }

                dtWriter.WriteXml(fileName, XmlWriteMode.WriteSchema, true);
                dtWriter = null;

                //if (_apptCurSelected?.Subject == "Today")
                //{
                //	UpdateToday();
                //}

            }
            catch (Exception ex)
            {
                Trace.Listeners[1].WriteLine("Could not save Trending Data File. " + ex.Message, "ERROR");
            }
        }

        private void RefreshExpected()
        {
            SetupExpectedChartData();
            AddRealTimeChart();
        }

        private void getLineMB(double X1, double Y1, double X2, double Y2, ref double m, ref double b)
        {
            if (X1 == X2) return;
            m = (Y2 - Y1) / (X2 - X1);
            b = Y1 - m * X1;
        }

        VacDryRecipe _curVacDryRecipe;
        private void SetupExpectedChartData()
        {
            bool bHasAtLeastOne = true;
            double m = 0, b = 0, dPressure = 0, dLastSetpoint = 0;
            int nCurSec = 0, nEndOfPullDown = 0;
            //getMB(0, 0, 1, 1, ref m, ref b); 

            VacDryRecipe curVacDryRecipe = new VacDryRecipe();
            curVacDryRecipe = GetCurVacDryRecipe();
            _curVacDryRecipe = curVacDryRecipe;
            if (_recording)
            {
                curVacDryRecipe.Ambient = (double)_chartSeriesRealtime.SeriesPoints[0].ValueY[0];
            }
            _chartSeriesExpected = new ChartSeries("Expected", SeriesType.Line);
            _chartSeriesExpected.DataPropertyNameX = "Time";
            _chartSeriesExpected.ChartSeriesVisualStyle.LineStyle.LineWidth = 3;
            _chartSeriesExpected.DataLabelVisualStyle.Background = new Background(Color.FromArgb(150, Color.CornflowerBlue));
            _chartSeriesExpected.DataLabelVisualStyle.TextAlignment = LineAlignment.Center;
            _chartSeriesExpected.DataLabelVisualStyle.HighlightBackground = new Background(Color.Yellow);


            dLastSetpoint = curVacDryRecipe.Ambient;

            //step 1 values
            if (curVacDryRecipe.Step1Secs != 0 && curVacDryRecipe.Step1SetPoint != 0.0 && curVacDryRecipe.Step1Angle != 0)
            {
                bHasAtLeastOne = true;
                // get slope and y intercept
                getLineMB(0, dLastSetpoint, curVacDryRecipe.Step1Secs, curVacDryRecipe.Step1SetPoint, ref m, ref b);
                // fill in series values
                for (int X = 0; X < curVacDryRecipe.Step1Secs; X++)
                {
                    dPressure = m * X + b;
                    SeriesPoint newPoint = new SeriesPoint(X, dPressure);
                    _chartSeriesExpected.SeriesPoints.Add(newPoint);
                }
                nCurSec += curVacDryRecipe.Step1Secs;
                dLastSetpoint = curVacDryRecipe.Step1SetPoint;

            }
            //step 2 values
            if (curVacDryRecipe.Step2Secs != 0 && curVacDryRecipe.Step2SetPoint != 0.0 && curVacDryRecipe.Step2Angle != 0)
            {
                bHasAtLeastOne = true;
                // get slope and y intercept
                getLineMB(nCurSec, dLastSetpoint, curVacDryRecipe.Step2Secs + nCurSec, curVacDryRecipe.Step2SetPoint, ref m, ref b);
                // fill in series values
                for (int X = nCurSec; X < curVacDryRecipe.Step2Secs + nCurSec; X++)
                {
                    dPressure = m * X + b;
                    SeriesPoint newPoint = new SeriesPoint(X, dPressure);
                    _chartSeriesExpected.SeriesPoints.Add(newPoint);
                }
                nCurSec += curVacDryRecipe.Step2Secs;
                dLastSetpoint = curVacDryRecipe.Step2SetPoint;
            }
            //step 3 values
            if (curVacDryRecipe.Step3Secs != 0 && curVacDryRecipe.Step3SetPoint != 0.0 && curVacDryRecipe.Step3Angle != 0)
            {
                bHasAtLeastOne = true;
                // get slope and y intercept
                getLineMB(nCurSec, dLastSetpoint, curVacDryRecipe.Step3Secs + nCurSec, curVacDryRecipe.Step3SetPoint, ref m, ref b);
                // fill in series values
                for (int X = nCurSec; X < curVacDryRecipe.Step3Secs + nCurSec; X++)
                {
                    dPressure = m * X + b;
                    SeriesPoint newPoint = new SeriesPoint(X, dPressure);
                    _chartSeriesExpected.SeriesPoints.Add(newPoint);
                }
                nCurSec += curVacDryRecipe.Step3Secs;
                dLastSetpoint = curVacDryRecipe.Step3SetPoint;
            }
            //step 4 values
            if (curVacDryRecipe.Step4Secs != 0 && curVacDryRecipe.Step4SetPoint != 0.0 && curVacDryRecipe.Step4Angle != 0)
            {
                bHasAtLeastOne = true;
                // get slope and y intercept
                getLineMB(nCurSec, dLastSetpoint, curVacDryRecipe.Step4Secs + nCurSec, curVacDryRecipe.Step4SetPoint, ref m, ref b);
                // fill in series values
                for (int X = nCurSec; X < curVacDryRecipe.Step4Secs + nCurSec; X++)
                {
                    dPressure = m * X + b;
                    SeriesPoint newPoint = new SeriesPoint(X, dPressure);
                    _chartSeriesExpected.SeriesPoints.Add(newPoint);
                }
                nCurSec += curVacDryRecipe.Step4Secs;
                dLastSetpoint = curVacDryRecipe.Step4SetPoint;
            }
            //step 5 values
            if (curVacDryRecipe.Step5DwellSecs != 0 && curVacDryRecipe.Step5SetPoint != 0.0 && curVacDryRecipe.Step5Angle != 0)
            {
                bHasAtLeastOne = true;
                // get slope and y intercept
                getLineMB(nCurSec, dLastSetpoint, curVacDryRecipe.Step5DwellSecs + nCurSec, curVacDryRecipe.Step5SetPoint, ref m, ref b);
                // fill in series values
                for (int X = nCurSec; X < curVacDryRecipe.Step5DwellSecs + nCurSec; X++)
                {
                    dPressure = m * X + b;
                    SeriesPoint newPoint = new SeriesPoint(X, dPressure);
                    _chartSeriesExpected.SeriesPoints.Add(newPoint);
                }
                nCurSec += curVacDryRecipe.Step5DwellSecs;
                dLastSetpoint = curVacDryRecipe.Step5SetPoint;
                nEndOfPullDown = nCurSec;
            }

            // Vac Release
            // slow relief
            if (bHasAtLeastOne && curVacDryRecipe.InitialVacReliefTime != 0)
            {
                // get slope and y intercept
                getLineMB(nCurSec, dLastSetpoint, curVacDryRecipe.InitialVacReliefTime + nCurSec, curVacDryRecipe.InitialVacReliefSetpoint, ref m, ref b);
                // fill in series values
                for (int X = nCurSec; X < curVacDryRecipe.InitialVacReliefTime + nCurSec; X++)
                {
                    dPressure = m * X + b;
                    SeriesPoint newPoint = new SeriesPoint(X, dPressure);
                    _chartSeriesExpected.SeriesPoints.Add(newPoint);
                }
                nCurSec += curVacDryRecipe.InitialVacReliefTime;
                dLastSetpoint = curVacDryRecipe.InitialVacReliefSetpoint;
            }

            // full relief
            if (bHasAtLeastOne)
            {
                // get slope and y intercept
                getLineMB(nCurSec, dLastSetpoint, curVacDryRecipe.FullVacReliefTime + nCurSec, curVacDryRecipe.Ambient, ref m, ref b);
                // fill in series values
                for (int X = nCurSec; X <= curVacDryRecipe.FullVacReliefTime + nCurSec; X++)
                {
                    dPressure = m * X + b;
                    SeriesPoint newPoint = new SeriesPoint(X, dPressure);
                    _chartSeriesExpected.SeriesPoints.Add(newPoint);
                }
                nCurSec += curVacDryRecipe.FullVacReliefTime;
                dLastSetpoint = curVacDryRecipe.InitialVacReliefSetpoint;
            }

            //generate the fake data (straight line start to finish)
            if (_chartSeriesFakeData != null)
            {
                _chartSeriesFakeData.SeriesPoints.Clear();
                _chartSeriesFakeData = null;
            }
            _chartSeriesFakeData = new ChartSeries();

            if (bHasAtLeastOne)
            {
                //pull down
                getLineMB(0, curVacDryRecipe.Ambient, nEndOfPullDown, curVacDryRecipe.Step5SetPoint, ref m, ref b);
                for (int X = 0; X < nEndOfPullDown; X++)
                {
                    dPressure = m * X + b;
                    SeriesPoint newPoint = new SeriesPoint(X, dPressure);
                    _chartSeriesFakeData.SeriesPoints.Add(newPoint);
                }
                // vac relief
                getLineMB(nEndOfPullDown, curVacDryRecipe.Step5SetPoint, _chartSeriesExpected.SeriesPoints.Count, curVacDryRecipe.Ambient, ref m, ref b);
                for (int X = nEndOfPullDown; X <= _chartSeriesExpected.SeriesPoints.Count; X++)
                {
                    dPressure = m * X + b;
                    SeriesPoint newPoint = new SeriesPoint(X, dPressure);
                    _chartSeriesFakeData.SeriesPoints.Add(newPoint);
                }
            }
        }

        public void UpdateStatus()
        {
            if (_frmMain == null || !Visible)
                return;
            bool bConnected = (_frmMain.MC != null && _frmMain.MC.Connected);
        }

        private void buttonShowRealTimeChart_Click(object sender, EventArgs e)
        {
            ;
        }

        /// <summary>
        /// Initializes the control chart.
        /// </summary>
        private void SetupChart(string sChartName = "")
        {
            _chartControl = new ChartControl();

            _chartControl.Name = "VacuumDryProfile";
            _chartControl.Location = new Point(8, 8);
            _chartControl.Size = new Size(750, 500);
            _chartControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            _chartControl.MinimumSize = new Size(200, 200);

            SetupScrollBarStyles();

            AddChart(sChartName);

            //_ChartControl.PointLabelUpdate += ChartControl_PointLabelUpdate;
        }

        /// <summary>
        /// Sets up the scrollbar styles.
        /// </summary>
        private void SetupScrollBarStyles()
        {
            ScrollBarVisualStyle moStyle =
                _chartControl.DefaultVisualStyles.HScrollBarVisualStyles.MouseOver;

            moStyle.ArrowBackground = new Background(Color.AliceBlue);
            moStyle.ThumbBackground = new Background(Color.AliceBlue);

            ScrollBarVisualStyle smoStyle =
                _chartControl.DefaultVisualStyles.HScrollBarVisualStyles.SelectedMouseOver;

            smoStyle.ArrowBackground = new Background(Color.White);
            smoStyle.ThumbBackground = new Background(Color.White);

            moStyle = _chartControl.DefaultVisualStyles.VScrollBarVisualStyles.MouseOver;

            moStyle.ArrowBackground = new Background(Color.AliceBlue);
            moStyle.ThumbBackground = new Background(Color.AliceBlue);

            smoStyle = _chartControl.DefaultVisualStyles.VScrollBarVisualStyles.SelectedMouseOver;

            smoStyle.ArrowBackground = new Background(Color.White);
            smoStyle.ThumbBackground = new Background(Color.White);
        }

        #region AddChart

        /// <summary>
        /// Adds a new chart to the ChartControl
        /// </summary>
        internal void AddChart(string sChartName = "")
        {
            DateTime dtStartDate = DateTime.Now;

            if (sChartName == "")
            {
                sChartName = @"Templates\VacDryDataExample.xml";
            }
            else
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                string sTemp = sChartName.Substring(20, 15);
                dtStartDate = DateTime.ParseExact(sTemp, "yyyyMMdd_HHmmss", provider);
            }

            // Create a new ChartXy.
            ChartXy chartXy = new ChartXy();

            chartXy.Name = "VacuumDryChart";
            chartXy.MinContentSize = new Size(500, 300);
            chartXy.ChartLineDisplayMode = ChartLineDisplayMode.DisplayLine;

            // Create our DataTable to use for the chart, and initialize it.

            DataTable dt = new DataTable("VacDryData");

            dt.ReadXml(sChartName);   // pass in filename of xml to load                  "

            //dt.WriteXml("DUPChart.xml",XmlWriteMode.WriteSchema,true);

            chartXy.DataSource = dt;

            // Setup our Crosshair display.

            ChartCrosshair ch = chartXy.ChartCrosshair;

            ch.AxisOrientation = AxisOrientation.X;

            ch.ShowValueXLine = true;
            ch.ShowValueYLine = true;
            ch.ShowValueXLabels = true;
            ch.ShowValueYLabels = true;

            ch.HighlightPoints = true;

            ch.ShowCrosshairLabels = false;
            ch.CrosshairLabelMode = CrosshairLabelMode.NearestSeries;

            ch.CrosshairVisualStyle.Background = new Background(Color.White);
            ch.CrosshairLabelVisualStyle.DropShadow.Enabled = Tbool.True;

            // Setup various styles for the chart...

            SetupChartStyle(chartXy);
            SetupContainerStyle(chartXy);
            SetupChartAxes(chartXy);
            SetupChartLegend(chartXy);

            // Add a chart title and associated series.

            AddChartTitle(chartXy, dtStartDate);

            AddExpectedSeries(chartXy);
            AddActualSeries(chartXy);

            // And finally, add the chart to the ChartContainers
            // collection of chart elements.
            _chartControl.ChartPanel.ChartContainers.Clear();
            _chartControl.ChartPanel.ChartContainers.Add(chartXy);
            _chartControl.Refresh();
        }

        private void AddRealTimeChart()
        {
            // Create a new ChartXy.
            ChartXy chartXy = new ChartXy();

            chartXy.Name = "VacuumDryChart";
            chartXy.MinContentSize = new Size(500, 300);
            chartXy.ChartLineDisplayMode = ChartLineDisplayMode.DisplayLine;

            // Setup our Crosshair display.
            ChartCrosshair ch = chartXy.ChartCrosshair;
            ch.AxisOrientation = AxisOrientation.X;

            ch.ShowValueXLine = true;
            ch.ShowValueYLine = true;
            ch.ShowValueXLabels = true;
            ch.ShowValueYLabels = true;
            ch.HighlightPoints = true;
            ch.ShowCrosshairLabels = false;
            ch.CrosshairLabelMode = CrosshairLabelMode.NearestSeries;

            ch.CrosshairVisualStyle.Background = new Background(Color.White);
            ch.CrosshairLabelVisualStyle.DropShadow.Enabled = Tbool.True;

            // Setup various styles for the chart...

            SetupChartStyle(chartXy);
            SetupContainerStyle(chartXy);
            SetupChartAxes(chartXy);
            SetupChartLegend(chartXy);

            // Add a chart title and associated series.
            if (_processStartTime == DateTime.MinValue)
                _processStartTime = DateTime.Now;
            AddChartTitle(chartXy, _processStartTime);


            chartXy.ChartSeries.Add(_chartSeriesExpected); //AddExpectedSeries(chartXy);  
            AddRealTimeActualSeries(chartXy);

            // And finally, add the chart to the ChartContainers
            // collection of chart elements.
            _chartControl.ChartPanel.ChartContainers.Clear();
            _chartControl.ChartPanel.ChartContainers.Add(chartXy);
            _chartControl.Refresh();

        }

        /// <summary>
        /// Sets up the chart axes.
        /// </summary>
        /// <param name="chartXy"></param>
        private void SetupChartAxes(ChartXy chartXy)
        {
            // X Axis

            ChartAxis axis = chartXy.AxisX;

            axis.GridSpacing = 5;

            axis.AxisMargins = 10;
            axis.MinGridInterval = 50;

            axis.MinorTickmarks.TickmarkCount = 0;

            axis.MajorGridLines.GridLinesVisualStyle.LineColor = Color.Gainsboro;
            axis.MinorGridLines.GridLinesVisualStyle.LineColor = Color.WhiteSmoke;

            axis.ChartAxisVisualStyle.AlternateBackground = new Background(Color.FromArgb(253, 253, 253));

            axis.CrosshairLabelVisualStyle.TextFormat = "#";
            axis.CrosshairLabelVisualStyle.Background = new Background(Color.Gainsboro);

            axis.UseAlternateBackground = true;

            // Y Axis

            axis = chartXy.AxisY;

            axis.AxisMargins = 10;
            axis.GridSpacing = 10;

            axis.MinValue = 0;
            axis.MaxValue = 800;

            Color color = Color.FromArgb(38, 93, 171);

            axis.AxisAlignment = AxisAlignment.Far;

            axis.CrosshairLabelVisualStyle.TextColor = color;
            axis.CrosshairLabelVisualStyle.Background = new Background(Color.White);

            axis.MinorTickmarks.TickmarkCount = 0;
            axis.MajorTickmarks.LabelVisualStyle.TextFormat = "0";

            // Let's add a title associated with the axis.

            axis.Title.Text = "Expected Vacuum Profile";

            ChartTitleVisualStyle tstyle = axis.Title.ChartTitleVisualStyle;

            tstyle.Font = new Font("Georgia", 15);
            tstyle.TextColor = color;
            tstyle.Alignment = Alignment.MiddleCenter;
            tstyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(4, 0, 4, 0);

            axis.ChartAxisVisualStyle.AxisColor = color;

            axis.MajorGridLines.GridLinesVisualStyle.LineColor = ControlPaint.LightLight(color);
            axis.MajorTickmarks.LabelVisualStyle.TextColor = color;

            // We are adding an Ancillary Y Axis for the "Glaciers Measured"
            // series to utilize.

            axis = new ChartAxisY("ActualAxisY");

            axis.AxisMargins = 20;
            axis.GridSpacing = 5;

            axis.AxisMargins = 10;
            axis.GridSpacing = 10;

            axis.MinValue = 0;
            axis.MaxValue = 1100;

            axis.MinValue = 0;
            axis.MaxValue = 800;

            color = Color.FromArgb(223, 92, 36);

            axis.AxisAlignment = AxisAlignment.Near;

            axis.CrosshairLabelVisualStyle.TextFormat = "0";
            axis.CrosshairLabelVisualStyle.TextColor = color;
            axis.CrosshairLabelVisualStyle.Background = new Background(Color.White);

            // Let's add a title associated with the axis.

            axis.Title.Text = "Actual Vacuum Profile";

            tstyle = axis.Title.ChartTitleVisualStyle;

            tstyle.Font = new Font("Georgia", 15);
            tstyle.TextColor = color;
            tstyle.Alignment = Alignment.MiddleCenter;
            tstyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(4, 0, 4, 0);

            axis.MajorGridLines.Visible = false;
            axis.MinorTickmarks.TickmarkCount = 1;

            axis.ChartAxisVisualStyle.AxisColor = color;
            axis.MinorGridLines.GridLinesVisualStyle.LineColor = ControlPaint.LightLight(color);

            axis.MajorTickmarks.ChartTickmarkVisualStyle.TickmarkColor = color;
            axis.MajorTickmarks.LabelVisualStyle.TextColor = color;
            axis.MinorTickmarks.ChartTickmarkVisualStyle.TickmarkColor = color;

            chartXy.AncillaryAxesY.Add(axis);
        }

        /// <summary>
        /// Sets up the chart style.
        /// </summary>
        /// <param name="chartXy"></param>
        private void SetupChartStyle(ChartXy chartXy)
        {
            ChartXyVisualStyle xystyle = chartXy.ChartVisualStyle;

            xystyle.Background = new Background(Color.White);
            xystyle.BorderThickness = new Thickness(1);
            xystyle.BorderColor = new BorderColor(Color.Black);

            xystyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(6);

            ChartSeriesVisualStyle cstyle = chartXy.ChartSeriesVisualStyle;
            PointMarkerVisualStyle pstyle = cstyle.MarkerHighlightVisualStyle;

            pstyle.Background = new Background(Color.Yellow);
            pstyle.Type = PointMarkerType.Ellipse;
            pstyle.Size = new Size(15, 15);
        }

        /// <summary>
        /// Sets up the chart's container style.
        /// </summary>
        /// <param name="chartXy"></param>
        private void SetupContainerStyle(ChartXy chartXy)
        {
            ContainerVisualStyle dstyle = chartXy.ContainerVisualStyles.Default;

            dstyle.Background = new Background(Color.White);
            dstyle.BorderColor = new BorderColor(Color.DimGray);
            dstyle.BorderThickness = new Thickness(1);

            dstyle.DropShadow.Enabled = Tbool.True;
            dstyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(6);
        }

        /// <summary>
        /// Sets up the Legend style.
        /// </summary>
        /// <param name="chartXy"></param>
        private void SetupChartLegend(ChartXy chartXy)
        {
            ChartLegend legend = chartXy.Legend;

            legend.ShowCheckBoxes = true;

            legend.Placement = Placement.Inside;
            legend.Alignment = Alignment.BottomLeft;
            legend.Direction = Direction.LeftToRight;

            legend.AlignVerticalItems = true;

            // Only let the Legend occupy 50 of the
            // available horizontal space.

            legend.MaxHorizontalPct = 50;

            ChartLegendVisualStyle lstyle = legend.ChartLegendVisualStyles.Default;

            lstyle.BorderThickness = new Thickness(1);
            lstyle.BorderColor = new BorderColor(Color.Crimson);

            lstyle.Margin = new DevComponents.DotNetBar.Charts.Style.Padding(8);
            lstyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(4);

            lstyle.Background = new Background(Color.FromArgb(200, Color.White));
        }

        /// <summary>
        /// Sets up the chart title style.
        /// </summary>
        /// <param name="chartXy"></param>
        private void AddChartTitle(ChartXy chartXy, DateTime dtStartTime)
        {
            ChartTitle title = new ChartTitle();

            title.Text = string.Format("Vacuum Dry Profile Trend\n{0}", dtStartTime.ToString());
            title.XyAlignment = XyAlignment.Top;

            ChartTitleVisualStyle tstyle = title.ChartTitleVisualStyle;

            tstyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(12, 12, 12, 6);
            tstyle.Font = new Font("Georgia", 16);
            tstyle.TextColor = Color.DarkGreen;
            tstyle.Alignment = Alignment.MiddleCenter;

            chartXy.Titles.Clear();
            chartXy.Titles.Add(title);
        }

        /// <summary>
        /// Adds a "Expected" series to the given chart.
        /// </summary>
        /// <param name="chartXy"></param>
        private void AddExpectedSeries(ChartXy chartXy)
        {
            ChartSeries series = new ChartSeries("Expected", SeriesType.Line);

            // The series utilizes the DataSource set at the chart level, and
            // sets the PropertyNames accordingly for the Year and MassBalance.

            series.DataPropertyNameX = "Time";
            series.DataPropertyNamesY.AddRange(new string[] { "Expected" });

            series.ChartSeriesVisualStyle.LineStyle.LineWidth = 3;

            series.DataLabelVisualStyle.Background = new Background(Color.FromArgb(150, Color.CornflowerBlue));
            series.DataLabelVisualStyle.TextAlignment = LineAlignment.Center;
            series.DataLabelVisualStyle.HighlightBackground = new Background(Color.Yellow);

            PointMarkerVisualStyle pstyle = series.ChartSeriesVisualStyle.MarkerVisualStyle;

            pstyle.Background = new Background(Color.Aqua);
            pstyle.BorderColor = Color.Black;
            pstyle.Type = PointMarkerType.Ellipse;
            pstyle.Size = new Size(9, 9);

            chartXy.ChartSeries.Add(series);
        }

        private void AddRealTimeActualSeries(ChartXy chartXy = null)
        {
            if (chartXy == null)
            {
                chartXy = (ChartXy)_chartControl.ChartPanel.ChartContainers[0];
            }
            Debug.WriteLine("we are here - AddRealTimeActualSeries");

            if (_chartSeriesRealtime != null)
            {
                if (chartXy.ChartSeries.Count < 2)
                {
                    _chartSeriesRealtime.AxisY = chartXy.AncillaryAxesY["RealTimeAxisY"];
                    chartXy.ChartSeries.Add(_chartSeriesRealtime);
                    Debug.WriteLine("Added Realtime chart series");
                }
                AddChartTitle(chartXy, _processStartTime);
            }
        }

        /// <summary>
        /// Adds a "Actual" series to the given chart.
        /// </summary>
        /// <param name="chartXy"></param>
        private void AddActualSeries(ChartXy chartXy)
        {
            ChartSeries series = new ChartSeries("Actual", SeriesType.Line);

            // The series utilizes the DataSource set at the chart level, and
            // sets the PropertyNames accordingly for the Year and Readings.

            series.DataPropertyNameX = "Time";
            series.DataPropertyNamesY.AddRange(new string[] { "Actual" });

            series.AxisY = chartXy.AncillaryAxesY["ActualAxisY"];

            series.ChartSeriesVisualStyle.LineStyle.LineWidth = 3;
            series.ChartSeriesVisualStyle.LineStyle.LinePattern = LinePattern.Dash;

            series.DataLabelVisualStyle.Background = new Background(Color.FromArgb(150, Color.LightGreen));
            series.DataLabelVisualStyle.TextAlignment = LineAlignment.Center;
            series.DataLabelVisualStyle.HighlightBackground = new Background(Color.Yellow);

            PointMarkerVisualStyle pstyle = series.ChartSeriesVisualStyle.MarkerVisualStyle;

            pstyle.Background = new Background(Color.Red);
            pstyle.BorderColor = Color.Black;
            pstyle.Type = PointMarkerType.Ellipse;
            pstyle.Size = new Size(9, 9);

            chartXy.ChartSeries.Add(series);
        }

        #endregion AddChart

        private void buttonTrendCalendar_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Trend Calendar Button", "Action");
            _frmMain.LoadSubForm(_frmMain.frmVacTrendCalendar);
        }

        private VacDryRecipe GetCurVacDryRecipe()
        {
            VacDryRecipe oRetVal = new VacDryRecipe();

            return oRetVal;
        }

        #endregion
    }
}
