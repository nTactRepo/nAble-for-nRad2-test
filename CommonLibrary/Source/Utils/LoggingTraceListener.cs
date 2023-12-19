using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
//using nTactCellManager.Properties;

namespace CommonLibrary.Utils
{
    public enum TraceLogType { CellMgr, SecsGem, nAble, Service, Debug, Alarms, nTact }

    public class LoggingTraceListener : TraceListener
    {
        #region Constants

        private const string RelLogDirectory = "Logs";

        private static readonly Dictionary<TraceLogType, string> LogFileNames = new Dictionary<TraceLogType, string>()
        {
            { TraceLogType.CellMgr, "CellMgr.log" },
            { TraceLogType.SecsGem, "SECSGEM.log" },
            { TraceLogType.nAble, "nAble.log" },
            { TraceLogType.Service, "nTact_Service.log" },
            { TraceLogType.Debug, "Debug.log" },
            { TraceLogType.nTact, "nTact.log" },
            { TraceLogType.Alarms, "Alarms.log" }
        };

        #endregion

        #region Properties

        public int RunningDaysToKeepLogs { get; }

        #endregion

        #region Data Members

        private readonly object _logLock = new object();
        private DateTime _lastTime;
        private StreamWriter _logStream = null;
        private string _logFile;
        private string _logPath;
        private readonly TraceLogType _loggerID = TraceLogType.CellMgr;
        private readonly string _logName;
        private bool _isDebugLog = false;

        #endregion

        #region Functions

        #region Constructors

        public LoggingTraceListener(TraceLogType logType, int runningDaysToKeepLogs) : base()
        {
            _loggerID = logType;
            _isDebugLog = _loggerID == TraceLogType.Debug;
            RunningDaysToKeepLogs = runningDaysToKeepLogs;
            
            if (LogFileNames.ContainsKey(logType))
            {
                _logName = LogFileNames[logType];
                Name = logType.ToString();
            }
            else
            {
                throw new Exception($"Unknown Log type: {logType}");
            }

            Init();
        }

        #endregion

        #region TraceListener

        public override void Write(string message)
        {
            DateTime curTime = DateTime.Now;
            if (_isDebugLog)
            {
                if (curTime.Minute != _lastTime.Minute)
                {
                    SetupLogFile();
                }
            }
            else
            {
                if (curTime.DayOfYear != _lastTime.DayOfYear)
                {
                    SetupLogFile();
                }
            }

            lock (_logLock)
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
                            {
                                _logStream.Write(nextLine);
                            }

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
            if(_isDebugLog)
            {
                if(curTime.Minute !=_lastTime.Minute)
                {
                    SetupLogFile();
                }
            }
            else
            {
                if (curTime.DayOfYear != _lastTime.DayOfYear)
                {
                    SetupLogFile();
                }
            }

            lock (_logLock)
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

        #endregion

        #region Private Functions

        private void Init()
        {
            if(_isDebugLog)
            {
                string debugPath = Path.Combine(RelLogDirectory, "Debug");
                _logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), debugPath);
            }
            else
                _logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), RelLogDirectory);

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
            lock (_logLock)
            {
                _lastTime = DateTime.Now;
                if (_logStream != null)
                {
                    _logStream.Flush();
                    _logStream.Close();
                    _logStream = null;
                }
                if(_isDebugLog)
                    _logFile = Path.Combine(_logPath, DateTime.Now.ToString("yyyyMMdd-HHmm") + _logName);
                else
                    _logFile = Path.Combine(_logPath, DateTime.Now.ToString("yyyyMMdd") + _logName);
                _logStream = File.AppendText(_logFile);

                try
                {
                    Console.WriteLine($"LogListener[{Name}] Started on ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
                    int nDaysToKeep = RunningDaysToKeepLogs + 1; // one extra day to take care of partial days :P
                    DateTime dtOldestDay = DateTime.Now.Subtract(new TimeSpan(nDaysToKeep, 0, 0, 0));
                    dtOldestDay = dtOldestDay.Subtract(new TimeSpan(0, dtOldestDay.Hour, dtOldestDay.Minute, dtOldestDay.Second, dtOldestDay.Millisecond));
                    DirectoryInfo di = new DirectoryInfo(_logPath);
                    FileInfo[] logFiles = di.GetFiles($"*{_logName}", SearchOption.TopDirectoryOnly);
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
                                Trace.WriteLine(string.Format("Deleting Old Log: {0}", curFile.Name), "INFO");
                                curFile.Delete();
                            }
                        }
                        catch (Exception ex1)
                        {
                            Trace.WriteLine("Caught Execption while deleting old log file : " + ex1.Message, "ERROR");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Caught Execption while deleting old log files : " + ex.Message, "ERROR");
                }
            }
        }

        #endregion

        #endregion
    }
}
