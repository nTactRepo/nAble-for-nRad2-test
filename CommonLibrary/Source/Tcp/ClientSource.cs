using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Tcp
{
    public class ClientSource : IClientSource
    {
        public ITcpClient GetClient()
        {
            return new NTactTcpClient();
        }
    }
}
