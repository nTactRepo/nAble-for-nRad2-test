using CommonLibrary.Source.Utils;
using CommonLibrary.Utils;
using nTact.Recipes;
using System;
using System.IO;

namespace nAble.Managers.ProcessLogCollectors
{
    public class ProcessLogCollector
    {
        #region Properties

        public string ProcessLogDirectory { get; set; } = @"C:\Temp\ProcessLogs";
        public ProcessLog ProcessLog { get; private set; } = null;
        public bool Enabled => _isEnabled();
        public bool CollectingLog { get; set; } = false;

        private LogEntry Logger { get; set; } = null;

        #endregion

        #region Member Data

        private Recipe _recipe = null;
        private Func<bool> _isEnabled = () => false;

        private IDateTimeSource _timeSource = null;

        #endregion

        #region Functions

        #region Constructors

        public ProcessLogCollector(string logDir, LogEntry logger, Func<bool> enabled, IDateTimeSource timeSource)
        {
            ProcessLogDirectory = logDir ?? throw new ArgumentNullException(nameof(logDir));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _isEnabled = enabled ?? throw new ArgumentNullException(nameof(enabled));
            _timeSource = timeSource ?? throw new ArgumentNullException(nameof(timeSource));

            try
            {
                Directory.CreateDirectory(ProcessLogDirectory);
            }
            catch (Exception ex)
            {
                logger.log(LogType.TRACE, Category.ERROR, $"Caught an error trying to create the directory '{ProcessLogDirectory}' for " +
                    $"process logging:  {ex.Message}");
                _isEnabled = () => false;
            }
        }

        #endregion

        #region Public Functions

        public bool RecipeStarted(Recipe recipe)
        {
            _recipe = recipe ?? throw new ArgumentNullException(nameof(recipe));

            if (!Enabled)
            {
                Logger.log(LogType.TRACE, Category.WARN, $"Process Logging is not currently enabled -- start request denied.");
                return false;
            }

            if (CollectingLog)
            {
                Logger.log(LogType.TRACE, Category.WARN, $"Tried to start collecting a process log when one was already running -- start denied.");
                return false;
            }

            ProcessLog = MakeProcessLogFromRecipe(_recipe);
            CollectingLog = true;

            return true;
        }

        public bool RecipeStopped(bool aborted, int errorCode)
        {
            if (!Enabled)
            {
                Logger.log(LogType.TRACE, Category.WARN, $"Process Logging is not currently enabled -- Stop request denied.");
                return false;
            }

            if (!CollectingLog)
            {
                Logger.log(LogType.TRACE, Category.WARN, $"Tried to finalize a process log when no log was currently running.");
                return false;
            }

            ProcessLog.RecipeEnd = _timeSource.Now;
            ProcessLog.RecipeAborted = aborted;
            ProcessLog.RecipeSucceeded = errorCode == 0;
            ProcessLog.ErrorCode = errorCode;

            try
            {
                LoaderSaver<ProcessLog>.Save(ProcessLog, $"ProcessLog_{ProcessLog.RecipeStart:MM.dd.yyyy_HH.mm.ss}.xml");
            }
            catch (Exception ex)
            {
                Logger.log(LogType.TRACE, Category.ERROR, $"Caught exception trying to save a process log:  {ex.Message}");
            }

            CollectingLog = false;

            return true;
        }

        #endregion

        #region Private Functions

        private ProcessLog MakeProcessLogFromRecipe(Recipe recipe)
        {
            ProcessLog log = new ProcessLog
            {
                RecipeName = recipe.Name,
                RecipeStart = _timeSource.Now
            };

            foreach (RecipeParam param in recipe.RecipeParams)
            {
                log.RecipeParams.Add(param);
            }

            return log;
        }

        #endregion

        #endregion
    }
}
