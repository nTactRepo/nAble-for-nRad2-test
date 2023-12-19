using System;
using System.Collections;
using System.Threading;
using System.IO;
using System.Data.SqlClient;
using System.Diagnostics;
using nTact;

namespace nAble
{
    public enum LogType
    {
        ACTIVITY,
        TRACE
    }

    public enum LogLevels
    {
        ALARM = 0,
        ERROR,
        WARN,
        INFO,
        DEBUG,
        ANY,
        ACTION
    }
    public enum Category
    {
        ALARM = 0,
        ERROR,
        WARN,
        INFO,
        DEBUG,
        ANY,
        ACTION,
        PURGE,
        SYSTEM
    }
    /// <summary>
    /// <para>NTACT (Used When nTact Personnel Should Be Notified Of A Log.)</para>
    /// <para>STARTUP (Used During Application Startup Logging.)</para>
    /// <para>RECIPE_RUN (Used During Recipe Run Routine For Logging.)</para>
    /// <para>EXIT (Used During Application Form_Leave Logging.)</para>
    /// <para>CONNECTION (Used Durring Any Device Connection Logging.)</para>
    /// </summary>
    public enum SubCategory
    {
        NONE = 0,
        NTACT = 1,
        STARTUP = 2,
        RECIPE_RUN = 3,
        EXIT = 4,
        CONNECTION = 5,
        ACTION = 6,
        CALIBRATION =7
    }

    /// <summary>
    /// Log Message Structure.
    /// </summary>
    public struct LogEntryMessage
    {
        public int level;
        public string level_desc;
        public Category tag;
        public LogType logType;
        public string message;
        public long time;
        public string user;
    }

    /// <summary>
    /// Event Handler for the Logger.
    /// </summary>
    public abstract class LogEntryEventHandler
    {
        private bool alive = false;
        private Queue q = null;
        private Thread dispatch = null;
        protected string[] logLevelDescriptors = null;
        private string _sName;
        public DateTime PrevDate;
        public string FilePath;
        public string LogName;
        public string newTag;
        public string newUserName;

        /// <summary>
        /// Name of logger and associated thread.
        /// </summary>
        public string Name
        {
            get { return _sName; }
            set
            {
                _sName = value;
                dispatch.Name = _sName;
            }
        }

        public LogEntryEventHandler()
        {
            q = Queue.Synchronized(new Queue(1000));
            start();
        }

        /// <summary>
        /// Start thread for logging messages to file.
        /// </summary>
        public void start()
        {
            if (alive) return;

            alive = true;
            dispatch = new Thread(new ThreadStart(dispatchMessages));
            dispatch.Start();
        }

        /// <summary>
        /// Shutdown logger and write all remaining queued messages.
        /// </summary>
        public void shutdown()
        {
            if (!alive) return;

            alive = false;
            Monitor.Enter(q);
            Monitor.PulseAll(q);
            Monitor.Exit(q);
        }

        /// <summary>
        /// Shutdown logger and discard all remaining queued messages.
        /// </summary>
        public void abort()
        {
            if (!alive) return;

            alive = false;
            dispatch.Abort();
        }

        /// <summary>
        /// Thread to queue and write messages.
        /// </summary>
        protected void dispatchMessages()
        {
            while (alive)
            {
                while ((q.Count != 0) && alive)
                {
                    log((LogEntryMessage)q.Dequeue());
                }

                if ((alive) && (q.Count == 0))
                {
                    Monitor.Enter(q);
                    if (q.Count == 0) Monitor.Wait(q);
                    Monitor.Exit(q);
                }
            }

            while (q.Count != 0)
            {
                log((LogEntryMessage)q.Dequeue());
            }

            onShutdown();

            dispatch = null;
        }

        /// <summary>
        /// Send log message to logging que.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="level"></param>
        /// <param name="level_desc"></param>
        /// <param name="message"></param>
        public void log(Category tag, LogType logType, string message, string user = "")
        {
            if (!alive) return;

            LogEntryMessage lm = new LogEntryMessage();
            lm.message = message;
            lm.tag = tag;
            newTag = tag.ToString();
            lm.logType = logType;
            lm.time = System.DateTime.Now.ToFileTime();
            lm.user = user;
            newUserName = user;

            q.Enqueue(lm);

            Monitor.Enter(q);
            Monitor.PulseAll(q);
            Monitor.Exit(q);
        }

        protected abstract void log(LogEntryMessage message);

        protected abstract void onShutdown();
    }

    public class BasicFileLogEntryEventHandler : LogEntryEventHandler
    {
        StreamWriter stream = null;
        bool append = true;
        private DatabaseWrapper _db = null;

        public BasicFileLogEntryEventHandler(string filePath, string fileName, string sName, DatabaseWrapper db)
        {
            FilePath = filePath;
            LogName = fileName;
            Name = sName;
            _db = db;
            string logFile = $"{DateTime.Now.Year}{DateTime.Now.Month:00}{DateTime.Now.Day:00}{fileName}.log";

            if (logFile != null)
            {
                FileMode fm = append ? FileMode.Append : FileMode.Create;

                FileStream fs = new FileStream(Path.Combine(filePath, logFile), fm, FileAccess.Write, FileShare.Read);
                PrevDate = DateTime.Now;
                stream = new StreamWriter(fs, System.Text.Encoding.UTF8, 4096);
                stream.AutoFlush = true;

            }
        }

        private void RollLog()
        {
            stream.Flush();
            stream.Close();

            string logFile = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + this.LogName + ".log";
            if (logFile != null)
            {
                FileMode fm;

                if (append) fm = FileMode.Append;
                else fm = FileMode.Create;

                FileStream fs = new FileStream(Path.Combine(this.FilePath, logFile), fm, FileAccess.Write, FileShare.Read);
                this.PrevDate = DateTime.Now;
                stream = new StreamWriter(fs, System.Text.Encoding.UTF8, 4096);
                stream.AutoFlush = true;
            }
        }

        override protected void log(LogEntryMessage message)
        {
            DateTime curDate = DateTime.Now;
            if (stream == null) return;
            string time = System.DateTime.FromFileTime(message.time).ToString("MM/dd/yyyy hh:mm:ss.ff tt");
            if (!PrevDate.Date.Equals(curDate.Date))
            {
                RollLog();
            }

            if (Settings.Default.UseLogDB)
            {
                try
                {
                    if (message.logType == LogType.TRACE)
                    {
                        // Trace Logging DB
                        using (SqlConnection conn = new SqlConnection(_db.ConnectionString))
                        {
                            conn.Open();
                            int recordsUpdated = 0;
                            string cmdString = "INSERT INTO dbo.TraceLog (TimeStamp,LogLevel,Description)";
                            cmdString += " VALUES (@val1,@val2,@val3)";

                            SqlCommand newCmd = new SqlCommand(cmdString, conn);
                            newCmd.Parameters.AddWithValue("@val1", time);
                            newCmd.Parameters.AddWithValue("@val2", newTag);
                            newCmd.Parameters.AddWithValue("@val3", message.message);
                            //newCmd.Parameters.AddWithValue("@val4", newUserName);
                            recordsUpdated = newCmd.ExecuteNonQuery();
                        }
                    }
                    else if (message.logType == LogType.ACTIVITY)
                    {
                        // Trace Logging DB
                        using (SqlConnection conn = new SqlConnection(_db.ConnectionString))
                        {
                            conn.Open();
                            int recordsUpdated = 0;
                            string cmdString = "INSERT INTO dbo.ActivityLog (TimeStamp,LogLevel,Description,UserID)";
                            cmdString += " VALUES (@val1,@val2,@val3,@val4)";

                            SqlCommand newCmd = new SqlCommand(cmdString, conn);
                            newCmd.Parameters.AddWithValue("@val1", time);
                            newCmd.Parameters.AddWithValue("@val2", newTag);
                            newCmd.Parameters.AddWithValue("@val3", message.message);
                            newCmd.Parameters.AddWithValue("@val4", newUserName);
                            recordsUpdated = newCmd.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                }
            }
            else
            {
                string sMessage = time + " [" + message.tag + "] " + message.user + " " + message.message + "\r\n";
                stream.Write(sMessage);
            }
        }

        protected override void onShutdown()
        {
            if (stream != null)
            {
                stream.Flush();
                stream.Close();
            }
        }

        public void setAppend(bool flag)
        {
            this.append = flag;
        }

        /// <summary>
        /// Get's append flag
        /// </summary>
        /// <returns></returns>
        public bool getAppend()
        {
            return append;
        }
    }

    /// <summary>
    /// Logger class.
    /// </summary>
    public class LogEntry
    {
        private static LogEntry logger;
        protected static string[] logLevelDesc = null;
        public string Name = "";
        //public FormMain _formMain = new FormMain();

        protected LogEntryEventHandler[][] leh;
        protected uint max = 0;
        protected uint levels = 0;
        protected LogEntryEventHandler defaultHandler = null;
        public string logtype;

        public LogEntry(uint levels, LogEntryEventHandler defaultHandler)
        {
            init(levels, defaultHandler);
        }

        /// <summary>
        /// Opens up a logger with the specified number of log levels.
        /// </summary>
        /// <param name="levels"></param>
        /// <param name="filename"></param>
        /// <param name="sName"></param>
        public LogEntry(uint levels, string filePath, string fileName, string sName, DatabaseWrapper db)
        {
            this.Name = sName;
            init(levels, new BasicFileLogEntryEventHandler(filePath, fileName, sName, db));
        }

        public LogEntry(uint levels)
        {
            init(levels, null);
        }

        public LogEntry promoteToStatic()
        {
            logger = this;
            return logger;
        }

        /// <summary>
        /// Create instance of logger.
        /// </summary>
        /// <param name="sName"></param>
        /// <returns></returns>
        public static LogEntry singleton(string sPath, string sLogName, string sThreadName, DatabaseWrapper db)
        {
            if (logger == null)
            {
                logger = new LogEntry(6, sPath, sLogName, sThreadName, db);
                logLevelDesc = new string[6];
                logLevelDesc[0] = "ALARM";
                logLevelDesc[1] = "ERROR";
                logLevelDesc[2] = "WARN";
                logLevelDesc[3] = "INFO";
                logLevelDesc[4] = "DEBUG";
                logLevelDesc[5] = "ANY";
            }

            return logger;
        }

        private void init(uint levels, LogEntryEventHandler defaultHandler)
        {
            this.levels = levels;
            this.defaultHandler = defaultHandler;
            this.max = levels - 1;

            leh = new LogEntryEventHandler[levels][];

            LogEntryEventHandler[] handler = new LogEntryEventHandler[1];

            handler[0] = defaultHandler;

            for (int i = 0; i < levels; i++)
            {
                leh[i] = handler;
            }
        }


        public void setMaximumLogLevel(uint max)
        {
            this.max = max;
        }

        public uint getMaximumLogLevel()
        {
            return max;
        }

        public LogEntryEventHandler getDefaultLogEntryEventHandler()
        {
            return this.defaultHandler;
        }

        public void addSpecialLoggerToAllLevels(LogEntryEventHandler handler)
        {
            if (handler == null) return;

            for (int level = 0; level < this.levels; level++)
            {
                addSpecialLogEntry(level, handler);
            }
        }

        public void addSpecialLogEntry(int level, LogEntryEventHandler handler)
        {
            if (level < levels)
            {
                if (leh[level] != null)
                {
                    int size = leh[level].Length + 1;
                    LogEntryEventHandler[] temp = new LogEntryEventHandler[size];

                    for (int i = 0; i < leh[level].Length; i++)
                    {
                        temp[i] = leh[level][i];
                    }

                    temp[size - 1] = handler;

                    leh[level] = temp;
                }
                else
                {
                    leh[level] = new LogEntryEventHandler[1];
                    leh[level][0] = handler;
                }
            }
        }

        public void addSpecialLogger(int level, string filePath, string fileName, string sName, DatabaseWrapper db)
        {
            addSpecialLogEntry(level, new BasicFileLogEntryEventHandler(filePath, fileName, sName, db));
        }

        public virtual void log(LogType logType, Category tag, string message, string user = "", SubCategory subCategory = SubCategory.NONE)
        {
            String subCatString = GetSubCategoryString(subCategory);
            leh[1][0].log(tag, logType, (subCatString + message), user);
        }

        private String GetSubCategoryString(SubCategory subCategory)
        {
            String sRetVal = "";
            switch (((int)subCategory))
            {
                case 0: return ""; 
                case 1: return "[NTACT] - ";
                case 2: return "[STARTUP] - "; 
                case 3: return "[RECIPE_RUN] - ";
                case 4: return "[EXIT] - ";
                case 5: return "[CONNECTION] - ";
                case 6: return "[ACTION] - ";
            }
            return sRetVal;
        }

        public void shutdown()
        {
            for (int level = 0; level < leh.Length; level++)
            {
                for (int i = 0; i < leh[level].Length; i++)
                {
                    if (leh[level][i] != null) leh[level][i].shutdown();
                }
            }
        }

        /// <summary>
        /// Purge log files older than nDays. Returns true if successful, false if error occurs.
        /// </summary>
        /// <param name="nDays"></param>
        /// <returns></returns>
        public bool PurgeLogs(int nDays)
        {
            bool bReturn = true;

            try
            {
                DateTime dtOldestDay = DateTime.Now.Subtract(new TimeSpan(nDays, 0, 0, 0));
                dtOldestDay = dtOldestDay.Subtract(new TimeSpan(0, dtOldestDay.Hour, dtOldestDay.Minute, dtOldestDay.Second, dtOldestDay.Millisecond));
                DirectoryInfo di = new DirectoryInfo(this.defaultHandler.FilePath);
                FileInfo[] logFiles = di.GetFiles("*" + this.defaultHandler.LogName + ".log", SearchOption.TopDirectoryOnly);
                foreach (FileInfo curFile in logFiles)
                {
                    try
                    {
                        DateTime dtLogFileTime = curFile.CreationTime;
                        if (dtLogFileTime < dtOldestDay)
                        {
                            this.log(LogType.TRACE, Category.PURGE, String.Format("Deleting Log: {0}", curFile.Name), "Null");
                            curFile.Delete();
                        }
                    }
                    catch (Exception ex1)
                    {
                        bReturn = false;
                        this.log(LogType.TRACE, Category.PURGE, "Caught Execption while deleting log files : " + ex1.Message, "Null");
                    }
                }
            }
            catch (Exception ex)
            {
                bReturn = false;
                this.log(LogType.TRACE, Category.PURGE, "Caught Execption while deleting log files : " + ex.Message, "Null");
            }

            return bReturn;
        }
    }
}
