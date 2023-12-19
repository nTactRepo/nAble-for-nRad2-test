using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support2
{
    public class LogStringSource
    {
        private const string TAG = "Support: ";

        public event Action<string> NewLog;

        public void SendLog(string msg)
        {
            NewLog?.Invoke($"{TAG}{msg}");
        }
    }
}
