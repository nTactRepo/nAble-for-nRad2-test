using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nAble.DataComm.AdvantechSerialServer.AdvantechComms
{
    public static class CommsFactory
    {
        public static ICommsBase CreateServerComms(CommsTypes type, LogEntry logger)
        {
            switch (type)
            {
                case CommsTypes.Adam6017: return new Adam6017(logger);

                default: throw new Exception($"Unrecognized type for CreateServerComms:  {type}");
            }
        }
    }
}
