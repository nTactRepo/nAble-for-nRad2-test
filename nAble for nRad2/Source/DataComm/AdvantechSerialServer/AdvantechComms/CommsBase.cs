using Advantech.Adam;
using System;
using System.Net.Sockets;

namespace nAble.DataComm.AdvantechSerialServer.AdvantechComms
{
    public abstract class CommsBase : ICommsBase, IDisposable
    {
        #region Properties

        public abstract string ServerName { get; }

        public virtual int MinAINumber { get; set; } = 0;
        public virtual int MaxAINumber { get; set; } = 0;

        public virtual int Total => MaxAINumber - MinAINumber + 1;

        public string IPAddress { get; private set; } = "";

        public int Port { get; private set; } = 0;

        public bool Connected { get; private set; } = false;

        #endregion

        #region Member Data

        protected AdamSocket _socket = new AdamSocket();

        protected LogEntry _log = null;

        private bool _disposed = false;

        #endregion

        #region Functions

        #region Constructors

        public CommsBase(LogEntry log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        ~CommsBase() => Dispose(false);

        #endregion

        #region Public Functions

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual bool Connect(string ip, int port)
        {
            IPAddress = ip ?? throw new ArgumentNullException(nameof(ip));
            Port = port;

            Connected = _socket.Connect(ip, ProtocolType.Tcp, port);

            if (!Connected)
            {
                throw new Exception($"Could not connect to an {ServerName} using IP: {ip}, Port: {port}");
            }

            return Connected;
        }

        public virtual void Disconnect()
        {
            _socket?.Disconnect();
        }

        public abstract bool GetValues(out ushort[] values);

        public abstract bool GetStatus(out int[] states);

        public abstract bool GetEnabled(out bool[] enabled);

        #endregion

        #region Protected Functions

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose of managed objects here
                Disconnect();
            }

            // Dispose of unmanaged objects here

            _disposed = true;
        }

        protected ushort GetChannelRangeUshortFormat(int index)
        {
            if (_socket.AnalogInput().GetInputRange(index, out ushort usRange))
            {
                return usRange;
            }

            return 0;
        }

        #endregion

        #endregion
    }
}
