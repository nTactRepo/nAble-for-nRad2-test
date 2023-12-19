using System;
using System.IO;
using System.Threading.Tasks;

namespace CommonLibrary.Tcp
{
    public interface ITcpClient : IDisposable
    {
        int SendTimeout { get; set; }
        int ReceiveTimeout { get; set; }
        bool Connected { get; set; }

        Task ConnectAsync(string host, int port);
        Stream GetStream();
    }
}
