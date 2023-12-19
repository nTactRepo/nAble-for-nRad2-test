using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Utils
{
    public enum LogLevel
    {
        ALARM = 0,
        ERROR,
        WARN,
        INFO,
        DEBUG,
        ANY
    }

    /// <summary>
    /// Log Message Structure.
    /// </summary>
    public struct LoggerMessage
    {
        private const string TimeFormat = "G"; //  mm/dd/yyyy h:mm:ss AM/PM

        public LogLevel Level;
        public string Tag;
        public string Message;
        public DateTime Time;

        public override string ToString()
        {
            var tagStr = string.IsNullOrEmpty(Tag) ? "" : $"[{Tag}] ";
            return $"{Time.ToString(TimeFormat)} [{Level}] {tagStr}{Message}";
        }
    }

    public interface ILogger
    {
        #region Properties

        string LogFolder { get; set; }

        string LogfileTag { get; set; }

        string LoggerName { get; set; }

        int NumQueuedLogs { get; }

        string CurrentLogfilePath { get; }

        bool Closed { get; }

        #endregion

        #region Functions

        void Log(LogLevel level, string tag, string message);

        void Shutdown();

        bool PurgeLogs(int numDaysToPurge);

        #endregion
    }
}
