using CommonLibrary.Utils;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLibrary.Utils
{
    public class ThreadedLogger : ILogger
    {
        #region Properties

        public string LogFolder { get; set; } = "";

        public string LogfileTag { get; set; } = "";

        public string LoggerName
        {
            get { return _loggerName; }

            set
            {
                _loggerName = value;
                _thread.Name = LoggerName;
            }
        }

        public bool Closed { get; private set; } = false;

        #region ILogger

        // Not STRICTLY necessary -- just makes testing easier
        public int NumQueuedLogs => _queue.Count;

        // Written ONLY by the thread!!
        public string CurrentLogfilePath { get; private set; } = "";

        #endregion

        #endregion

        #region Member Data

        private readonly ConcurrentQueue<LoggerMessage> _queue = new ConcurrentQueue<LoggerMessage>();
        private readonly IDateTimeSource _nowSource = null;
        private bool _closing = false;
        private string _loggerName = "";
        private readonly Thread _thread = null;

        #endregion

        #region Functions

        #region Constructors

        public ThreadedLogger(string path, string logfileTag, string loggerName, IDateTimeSource nowSource)
        {
            _nowSource = nowSource;
            _thread = new Thread(new ThreadStart(LoggerThread));

            LogFolder = path;
            LogfileTag = logfileTag;
            LoggerName = loggerName;

            CreateLogDirectoryIfNeeded();

            _thread.Start();
        }

        #endregion

        #region ILogger

        public void Log(LogLevel level, string tag, string message)
        {
            if (_closing)
            {
                return;
            }

            LoggerMessage logMessage = new LoggerMessage()
            {
                Message = message,
                Tag = tag,
                Level = level,
                Time = _nowSource.Now
            };

            _queue.Enqueue(logMessage);
        }

        public bool PurgeLogs(int numDaysToPurge)
        {
            bool succeeded = true;

            try
            {
                DateTime oldestDay = _nowSource.Now.Subtract(new TimeSpan(numDaysToPurge, 0, 0, 0));
                oldestDay = oldestDay.Subtract(new TimeSpan(0, oldestDay.Hour, oldestDay.Minute, oldestDay.Second, oldestDay.Millisecond));
                DirectoryInfo di = new DirectoryInfo(LogFolder);
                var logFiles = di.GetFiles("*" + LogfileTag + ".log", SearchOption.TopDirectoryOnly);

                foreach (FileInfo file in logFiles)
                {
                    try
                    {
                        DateTime fileTime = ParseFileDate(file.Name);

                        if (fileTime < oldestDay)
                        {
                            Log(LogLevel.ERROR, "PURGE", String.Format("Deleting Log: {0}", file.Name));
                            file.Delete();
                        }
                    }
                    catch (Exception ex)
                    {
                        succeeded = false;
                        Log(LogLevel.ERROR, "PURGE", "Caught Execption while deleting log files : " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                succeeded = false;
                Log(LogLevel.ERROR, "PURGE", "Caught Execption while listing log files for deletion: " + ex.Message);
            }

            return succeeded;
        }

        public void Shutdown()
        {
            bool needAbort = true;

            // First, try cancelling and waiting for a "clean" exit
            // 50 waits of 10ms each = 0.5 seconds
            _closing = true;

            for (int i = 0; i < 50; ++i)
            {
                if (Closed)
                {
                    needAbort = false;
                    break;
                }

                Thread.Sleep(10);
            }

            if (needAbort)
            {
                _thread.Abort();
            }
        }

        #endregion

        #region Thread Functions

        private void LoggerThread()
        {
            while (!_closing)
            {
                while (!_queue.IsEmpty)
                {
                    if (_queue.TryDequeue(out var message))
                    {
                        WriteLogToFile(message).Wait();
                    }
                }

                Thread.Sleep(2);
            }

            EmptyQueue().Wait();
            Closed = true;
        }

        #endregion

        #region Private Functions

        private async Task WriteLogToFile(LoggerMessage message)
        {
            try
            {
                string filename = message.Time.Year.ToString() + message.Time.Month.ToString("00") + message.Time.Day.ToString("00") + LogfileTag + ".log";
                CurrentLogfilePath = Path.Combine(LogFolder, filename);

                using (FileStream fs = new FileStream(CurrentLogfilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    using (StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8, 4096))
                    {
                        await writer.WriteLineAsync(message.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine($"Exception caught trying to write to the {LoggerName} logfile: {ex.Message}");
            }
        }

        private async Task EmptyQueue()
        {
            while (!_queue.IsEmpty)
            {
                if (_queue.TryDequeue(out var message))
                {
                    await WriteLogToFile(message);
                }
            }
        }

        /// <summary>
        /// Function to create the log directory.        
        /// Throws if there are any problems, which can take down the constructor
        /// </summary>
        private void CreateLogDirectoryIfNeeded()
        {
            if (!Directory.Exists(LogFolder))
            {
                Directory.CreateDirectory(LogFolder);
            }
        }

        private DateTime ParseFileDate(string name)
        {
            DateTime date = DateTime.Now;

            try
            {
                int year = int.Parse(name.Substring(0, 4));
                int month = int.Parse(name.Substring(4, 2));
                int day = int.Parse(name.Substring(6, 2));
                date = new DateTime(year, month, day);
            }
            catch (Exception)
            {
                // Nothing, let default now return
            }

            return date;
        }

        #endregion

        #endregion
    }
}
