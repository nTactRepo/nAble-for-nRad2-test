using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nAble.Model.StatusMessages
{
    internal class StatusMessage
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public StatusMessageLevel Level { get; set; }
    }
}
