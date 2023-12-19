using CommonLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CommonLibrary.Utils
{
    public class LogLimiter
    {
        #region Constants

        private static readonly TimeSpan DefaultTimeBetweenRepeats = new TimeSpan(hours: 0, minutes: 2, seconds: 0);

        #endregion

        #region Data Members

        private readonly Dictionary<string, DateTime> _logTimes = new Dictionary<string, DateTime>();

        #endregion

        #region Properties

        public TimeSpan TimeBetweenRepeatLogs { get; set; } = DefaultTimeBetweenRepeats;

        /// <summary>
        /// If set, the logger the output will go to, otherwise to Trace.Listeners["nTact"].WriteLine
        /// </summary>
        public ILogger Logger { get; set; } = null;

        public IDateTimeSource DateSource { get; } = null;

        #endregion

        #region Functions

        #region Constructors

        public LogLimiter() : this(null, new DateTimeSource()) { }

        public LogLimiter(ILogger logger = null, IDateTimeSource source = null)
        {
            Logger = logger;
            DateSource = source ?? new DateTimeSource();
        }

        #endregion

        public void Log(LogLevel level, string tag, string msg)
        {
            bool sendLog = false;
            DateTime now = DateSource.Now;

            // If in the dictionary
            if (_logTimes.ContainsKey(msg))
            {
                // If it has been longer than the repeat time since last sent, send again
                if (now >= _logTimes[msg] + TimeBetweenRepeatLogs)
                {
                    sendLog = true;
                    _logTimes[msg] = now;
                }
            }
            else // not in the dictionary.  Add it, and send the log
            {
                _logTimes[msg] = now;
                sendLog = true;
            }

            if (sendLog)
            {
                if (Logger is null)
                {
                    Trace.Listeners["nTact"].WriteLine(msg, tag);
                }
                else
                {
                    Logger.Log(level, tag, msg);
                }
            }
        }

        public void Clear()
        {
            _logTimes.Clear();
        }

        #endregion
    }
}
