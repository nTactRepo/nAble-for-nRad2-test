using DevComponents.DotNetBar.Schedule;
using DevComponents.Schedule.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormVacTrendCalendar : Form, IUpdateableForm
    {
        #region Constants

        private const string BaseDirectory = @"Data\TrendingData";

        #endregion

        #region Inner Classes

        private class TrendDataMonth
        {
            public bool Loaded { get; set; } = false;
            public List<string> TrendDataFilenames { get; set; } = new List<string>();
        }

        private class TrendDataDate
        {
            public string Name { get; set; } = "";
            public string Filename { get; set; } = "";

            public TrendDataDate(string name, string filename)
            {
                Name = name;
                Filename = filename;
            }

            public override string ToString() => Name;
        }

        #endregion

        #region Fields

        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;

        private Appointment _apptToday = null;
        private Appointment _apptCurSelected = null;

        private Dictionary<string, TrendDataMonth> _months = new Dictionary<string, TrendDataMonth>(3);

        #endregion

        #region Functions

        #region Constructors

        public FormVacTrendCalendar(FormMain formMain, LogEntry log)
        {
            InitializeComponent();

            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            panelExDateNav.Text = $"{calendarViewPicker.MonthViewStartDate:MMMM yyyy}";

            LoadSavedTrendingData();
            UpdateCalendarView(DateTime.Now); // populate the current month on the calendar

            panelExSelectedDay.Text = $"{DateTime.Now:MMMM dd, yyyy}";
        }

        #endregion

        #region Public Functions

        public void UpdateStatus()
        {
            if (!Visible)
            {
                return;
            }
        }

        #endregion

        #region Control Event Handlers

        private void buttonGotoPrevScreen_Click(object sender, EventArgs e)
        {
            _frmMain.ShowPrevForm();
        }

        private void buttonXPrevMonth_Click(object sender, EventArgs e)
        {
            calendarViewPicker.MonthViewStartDate = calendarViewPicker.MonthViewStartDate.AddMonths(-1);
            calendarViewPicker.MonthViewEndDate = calendarViewPicker.MonthViewEndDate.AddMonths(-1);
        }

        private void buttonXNextMonth_Click(object sender, EventArgs e)
        {
            calendarViewPicker.MonthViewEndDate = calendarViewPicker.MonthViewEndDate.AddMonths(1);
            calendarViewPicker.MonthViewStartDate = calendarViewPicker.MonthViewStartDate.AddMonths(1);
        }

        private void calendarViewPicker_MonthViewStartDateChanged(object sender, DevComponents.DotNetBar.Schedule.DateChangeEventArgs e)
        {
            panelExDateNav.Text = $"{calendarViewPicker.MonthViewStartDate:MMMM yyyy}";
            UpdateCalendarView(calendarViewPicker.MonthViewStartDate);
        }

        private void calendarViewPicker_ItemClick(object sender, EventArgs e)
        {
            DateTime selectedDate = new DateTime();
            Appointment curAppt = null;

            if (sender is AppointmentMonthView apptMonthView)
            {
                selectedDate = apptMonthView.StartTime;
                panelExSelectedDay.Text = selectedDate.ToString("MMMM dd, yyyy");

                curAppt = apptMonthView.Appointment;
                _apptCurSelected = apptMonthView.Appointment;
            }
            else if (sender is MonthView monthView)
            {
                listBoxAdvCharts.Items.Clear();
                _apptCurSelected = null;

                if (monthView?.DateSelectionStart.Value != null)
                {
                    selectedDate = (DateTime)monthView.DateSelectionStart;
                    panelExSelectedDay.Text = selectedDate.ToString("MMMM dd, yyyy");
                    listBoxAdvCharts.Items.Clear();

                    foreach (AppointmentMonthView tmpAppt in monthView.SubItems)
                    {
                        if (tmpAppt.StartTime == monthView.DateSelectionStart.Value)
                        {
                            selectedDate = tmpAppt.StartTime;
                            curAppt = tmpAppt.Appointment;
                            _apptCurSelected = tmpAppt.Appointment;
                        }
                    }
                }
            }

            if (selectedDate.Year > 1 && curAppt != null)
            {
                listBoxAdvCharts.Items.Clear();

                if (_apptCurSelected?.Subject == "Today")
                {
                    UpdateToday();
                }
                else
                {
                    foreach (string file in (List<string>)curAppt.Tag)
                    {
                        listBoxAdvCharts.Items.Add(new TrendDataDate(TimeStringFromFilename(file), file));
                    }
                }
            }
        }

        private void LoadSelectedChart()
        {
            if (listBoxAdvCharts.SelectedIndex != -1)
            {
                TrendDataDate tdd = (TrendDataDate)listBoxAdvCharts.SelectedItem;

                if (tdd != null)
                {
                    string monthDir = tdd.Filename.Substring(0, 6);
                    string newChart = Path.Combine(BaseDirectory, monthDir, tdd.Filename);

                    _log.log(LogType.TRACE, Category.INFO, $"Requesting Chart: {newChart}", "Info");
                    
                    _frmMain.ShowTrendingChart(newChart);
                }
            }
        }

        private void listBoxAdvCharts_ItemDoubleClick(object sender, MouseEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "User Double Clicked Selection List", "Action");
            LoadSelectedChart();
        }

        private void buttonViewSelectedChart_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "User Pressed View Selected Chart Button", "Action");
            LoadSelectedChart();
        }

        #endregion

        #region Private Functions

        private void UpdateCalendarView(DateTime selectedMonth, bool needsRefresh = false)
        {
            if (needsRefresh)
            {
                LoadSavedTrendingData();
                calendarViewPicker.CalendarModel.Appointments.Clear();
            }

            string monthString = $"{selectedMonth.Year}{selectedMonth.Month:0#}";

            // see if the month has been loaded into the calendar, if not.. load it
            if (_months.ContainsKey(monthString) && !_months[monthString].Loaded)
            {
                Appointment newAppt = null;
                var trendData = _months[monthString];
                int curDaysCount = 0;
                string curDay = "", prevDay = "";

                trendData.Loaded = true;
                string today = DateTime.Now.ToString("yyyyMMdd");

                foreach (string file in trendData.TrendDataFilenames)
                {
                    // see if we have a new day;
                    curDay = file.Substring(0, 8);

                    if (curDay != prevDay)
                    {
                        // on new day update previous Appointment and then post it (if exists)
                        if (newAppt != null)
                        {
                            if (prevDay != today)
                            {
                                newAppt.Subject = curDaysCount.ToString();
                                newAppt.Tooltip = $"{curDaysCount} Charts Available";
                                calendarViewPicker.CalendarModel.Appointments.Add(newAppt);
                            }
                            else
                            {
                                newAppt.Subject = "Today";
                                newAppt.Tooltip = "Click for Available Charts";
                                calendarViewPicker.CalendarModel.Appointments.Add(newAppt);
                                _apptToday = newAppt;
                            }
                        }

                        DateTime newDate = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
                        newDate = newDate.AddDays(int.Parse(curDay.Substring(6, 2)) - 1);

                        // start a new appointment
                        curDaysCount = 1;
                        prevDay = curDay;

                        newAppt = new Appointment
                        {
                            StartTime = newDate,
                            EndTime = newDate.AddDays(1),
                            CategoryColor = Appointment.CategoryYellow
                        };

                        var fileNames = new List<string> { file };
                        newAppt.Tag = fileNames;
                    }
                    else // same day so just count it
                    {
                        curDaysCount++;
                        ((List<string>)newAppt.Tag).Add(file);
                    }
                }

                // on new day update previous Appointment and then post it (if exists)
                if (newAppt != null)
                {
                    if (curDay != today)
                    {
                        newAppt.Subject = curDaysCount.ToString();
                        newAppt.Tooltip = string.Format("{0} Charts Available", curDaysCount);
                        calendarViewPicker.CalendarModel.Appointments.Add(newAppt);
                    }
                    else
                    {
                        newAppt.Subject = "Today";
                        newAppt.Tooltip = string.Format("Click for Available Charts", curDaysCount);
                        calendarViewPicker.CalendarModel.Appointments.Add(newAppt);
                        _apptToday = newAppt;
                    }
                }

                if (_apptToday == null)
                {
                    DateTime newDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                    // start a new appointment
                    prevDay = curDay;

                    newAppt = new Appointment
                    {
                        StartTime = newDate,
                        EndTime = newDate.AddDays(1),
                        CategoryColor = Appointment.CategoryYellow,
                        Subject = "Today",
                        Tooltip = "Click for Available Charts"
                    };

                    calendarViewPicker.CalendarModel.Appointments.Add(newAppt);
                    _apptToday = newAppt;
                }
            }
        }

        private void LoadSavedTrendingData()
        {
            DirectoryInfo di = new DirectoryInfo(BaseDirectory);
            Regex reg = new Regex(@"^20[0-9]{2}([0][1-9]|1[0-2])$");

            int totalFound = 0;

            try
            {
                // Create TrendingData if it does not Exist
                if (!di.Exists)
                {
                    Trace.Listeners[1].WriteLine("Trending Data Directory not found, creating;", "INFO");
                    di.Create();
                }

                // Read all of the subdirectories
                Debug.Write($"Dir Names: ");

                var filenames = new List<string>();

                foreach (DirectoryInfo subdir in di.GetDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    Debug.Write($"{subdir.Name}, ");

                    // Only process the match of YYYYMM  (starting at year 2000)
                    if (reg.IsMatch(subdir.Name))
                    {
                        foreach (FileInfo fi in subdir.GetFiles("20*.xml"))
                        {
                            filenames.Add(fi.Name);
                            totalFound++;
                        }

                        filenames.Sort();

                        _months[subdir.Name].Loaded = false;
                        _months[subdir.Name].TrendDataFilenames = filenames;
                    }
                }

                Debug.WriteLine("");
                Trace.Listeners[1].WriteLine($"Finished processing TrendingData. {totalFound} data files found", "INFO");
                string temp = DateTime.Now.ToString("yyyyMM");

                if (!_months.ContainsKey(temp))
                {
                    _months[temp].TrendDataFilenames = new List<string>();
                    _months[temp].Loaded = false;
                    Directory.CreateDirectory($@"{BaseDirectory}\{temp}");
                }
            }
            catch (IOException ioe)
            {
                Trace.Listeners[1].WriteLine("Error processing TrendingData directory.", "ERROR");
                Trace.Listeners[1].WriteLine($"ErrorInfo: {ioe.Message}", "ERROR");
            }
        }

        private void UpdateToday()
        {
            string timeString;
            string today = DateTime.Now.ToString("yyyyMMdd");
            string dirName = DateTime.Now.ToString("yyyyMM");
            string newDir = Path.Combine(BaseDirectory, dirName);

            listBoxAdvCharts.Items.Clear();
            Debug.WriteLine($"UpdateToday()");

            if (Directory.Exists(newDir))
            {
                DirectoryInfo di = new DirectoryInfo(newDir);
                Debug.WriteLine($"Dir Name: {di.Name}");

                foreach (FileInfo fi in di.GetFiles($"{today}*.xml"))
                {
                    string filename = fi.Name;
                    timeString = TimeStringFromFilename(filename);
                    listBoxAdvCharts.Items.Add(new TrendDataDate(timeString, filename));
                }
            }
        }

        private string TimeStringFromFilename(string filename)
        {
            // file Ex:  20170207_091259_100030.xml
            //                    ^ ^ ^
            return $"{filename.Substring(9, 2)}:{filename.Substring(11, 2)}:{filename.Substring(13, 2)}";
        }

        #endregion

        #endregion
    }
}
