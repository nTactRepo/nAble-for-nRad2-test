using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
namespace nTact.Logging
{
    class LoggingTraceListener : TraceListener
    {
        private Object logLock = new Object();
        private DateTime _lastTime;
        private StreamWriter _logStream = null;
        private string _logFile;
        private string _logPath;
        private int _nDaysToKeep = 30;
        public int DaysToKeep
        {
            get { return _nDaysToKeep; }
            set { _nDaysToKeep = value; }
        }

        public LoggingTraceListener()
            : base()
        {
            Init();
        }

        public LoggingTraceListener(int nDaysToKeep)
            : base()
        {
            _nDaysToKeep = nDaysToKeep;
            Init();
        }

        private void Init()
        {
            _logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs");
            try
            {
                Directory.CreateDirectory(_logPath);
            }
            catch (Exception) { }
            if (!Directory.Exists(_logPath)) // couldnt create or something bad wrong... FAIL
            {
                throw new NullReferenceException("Couldn't access log file directory : " + _logPath);
            }

            SetupLogFile();
        }

        private void SetupLogFile()
        {
            _lastTime = DateTime.Now;
            lock (logLock)
            {
                if (_logStream != null)
                {
                    _logStream.Flush();
                    _logStream.Close();
                    _logStream = null;
                }
                _logFile = Path.Combine(_logPath, DateTime.Now.ToString("yyyyMMdd") + "nAble.log");
                _logStream = File.AppendText(_logFile);
            }

            try
            {
                int nDaysToKeep = _nDaysToKeep + 1; // one extra day to take care of partial days :P
                DateTime dtOldestDay = DateTime.Now.Subtract(new TimeSpan(nDaysToKeep, 0, 0, 0));
                dtOldestDay = dtOldestDay.Subtract(new TimeSpan(0, dtOldestDay.Hour, dtOldestDay.Minute, dtOldestDay.Second, dtOldestDay.Millisecond));
                DirectoryInfo di = new DirectoryInfo(_logPath);
                FileInfo[] logFiles = di.GetFiles("????????nAble.log", SearchOption.TopDirectoryOnly);
                foreach (FileInfo curFile in logFiles)
                {
                    try
                    {
                        int nYear = int.Parse(curFile.Name.Substring(0, 4));
                        int nMonth = int.Parse(curFile.Name.Substring(4, 2));
                        int nDay = int.Parse(curFile.Name.Substring(6, 2));
                        DateTime dtLogFileTime = new DateTime(nYear, nMonth, nDay);
                        if (dtLogFileTime < dtOldestDay)
                        {
                            //_log.log(LogType.TRACE, Category.INFO, String.Format("Deleting Old Log: {0}", curFile.Name));
                            curFile.Delete();
                        }
                    }
                    catch (Exception ex1)
                    {
                        //_log.log(LogType.TRACE, Category.INFO, "Caught Execption while deleting old log file : " + ex1.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                //_log.log(LogType.TRACE, Category.INFO, "Caught Execption while deleting old log files : " + ex.Message);
            }
        }

        public override void Write(string message)
        {
            DateTime curTime = DateTime.Now;
            if (curTime.DayOfYear != _lastTime.DayOfYear)
            {
                SetupLogFile();
            }

            lock (logLock)
            {
                try
                {
                    string timeString = DateTime.Now.ToString("hh:mm:ss:fff tt");
                    if (message.Contains("\n"))
                    {
                        string[] lines = message.Split('\n');
                        string nextLine;
                        if (lines.Length > 1)
                        {
                            int i;
                            nextLine = lines[0];
                            for (i = 0; i < lines.Length - 1; i++)
                            {
                                if (nextLine.Length > 0 || (nextLine.Length == 0 && i != lines.Length - 1))
                                {
                                    _logStream.WriteLine(timeString + "   " + nextLine);
                                }
                                nextLine = lines[i].TrimEnd();
                            }
                            if (nextLine.Length > 0)
                                _logStream.Write(nextLine);
                            lines = null;
                        }

                        _logStream.Write(message);
                    }
                    else
                    {
                        _logStream.Write(message);
                    }
                    _logStream.Flush();
                }
                catch (Exception) { }
            }

        }
        public override void WriteLine(string message)
        {
            DateTime curTime = DateTime.Now;
            if (curTime.DayOfYear != _lastTime.DayOfYear)
            {
                SetupLogFile();
            }

            lock (logLock)
            {
                try
                {
                    string[] lines = message.Split('\n');
                    string nextLine;
                    string timeString = DateTime.Now.ToString("hh:mm:ss:fff tt");
                    for (int i = 0; i < lines.Length; i++)
                    {
                        nextLine = lines[i].TrimEnd();

                        if (nextLine.Length > 0 || (nextLine.Length == 0 && i != lines.Length - 1))
                        {
                            _logStream.WriteLine(timeString + "   " + nextLine);
                        }
                    }
                    lines = null;
                    _logStream.Flush();
                }
                catch (Exception) { }
            }
        }
        public override void Write(string message, string category)
        {
            base.Write(message, category);
        }
        public override void WriteLine(string message, string category)
        {
            base.WriteLine(message, category);
        }
    }
}
