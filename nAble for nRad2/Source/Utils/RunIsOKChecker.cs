using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nAble.Utils
{
    public class RunBlocker
    {
        #region Properties

        public Func<bool> Condition { get; } = null;
        public string Message { get; } = string.Empty;

        #endregion

        #region Functions

        public RunBlocker() { }

        public RunBlocker(Func<bool> condition, string message)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        #endregion
    }

    public class RunIsOKChecker
    {
        #region Properties

        public List<RunBlocker> RunBlockers { get; set; } = null;

        public bool OKToRun => !RunBlockers.Any(rb => rb.Condition());

        public string StatusString
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (var rb in RunBlockers)
                {
                    try
                    {
                        if (rb.Condition())
                        {
                            sb.Append($"{rb.Message}, ");
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.log(LogType.TRACE, Category.ERROR, 
                            $"Exception caught trying to check if it is OK to run: {ex.Message}");
                    }
                }

                return sb.Length == 0 ? "" : sb.ToString().TrimEnd(new char[] { ' ', ',' });
            }
        }

        #endregion

        #region Data Members

        private readonly LogEntry _log = null;

        #endregion

        #region Functions

        public RunIsOKChecker(LogEntry log, IEnumerable<RunBlocker> runBlockers)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));

            var rbs = runBlockers ?? throw new ArgumentNullException(nameof(runBlockers));
            RunBlockers = new List<RunBlocker>(rbs);
        }

        #endregion    
    }
}
