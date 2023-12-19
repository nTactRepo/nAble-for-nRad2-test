using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CommonLibrary.Tcp
{
    public class NTactTcpClient : ITcpClient
    {
        public int SendTimeout { get => _client.SendTimeout; set => _client.SendTimeout = value; }
        public int ReceiveTimeout { get => _client.ReceiveTimeout; set => _client.ReceiveTimeout = value; }
        public bool Connected { get => _client.Connected; set => _b = value; }

        private TcpClient _client = null;
        private bool _b;

        public NTactTcpClient()
        {
            _client = new TcpClient();
        }

        public Task ConnectAsync(string host, int port)
        {
            return _client.ConnectAsync(host, port);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public Stream GetStream()
        {
            return _client.GetStream();
        }
    }
}
