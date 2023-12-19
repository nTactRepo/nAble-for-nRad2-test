using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CommonLibrary.nTactServer
{
    public class BroadcastAddressProvider : IDisposable
    {
        #region Events

        public event Action IPChanged;

        #endregion

        #region Constants and Enums

        private const string DefaultAdaptorName = "WAN";
        private const int DefaultBroadcastPort = 57258;
        private const int DefaultServicePort = 57257;

        private const int NumSecsBetweenIPChecks = 30;
        private const int ReceiveTimeoutInMs = 250;

        public enum AddressState
        {
            Stopped,
            NeedsIP,
            NeedsBinding,
            Listening
        }

        #endregion

        #region Properties

        public AddressState State { get; private set; } = AddressState.Stopped;

        private bool CancelRequested { get; set; } = false;
        public string AdaptorName { get; private set; } = DefaultAdaptorName;
        public int BroadcastPort { get; private set; } = DefaultBroadcastPort;
        public string ServerName { get; private set; } = "";
        public int ServicePort { get; private set; } = DefaultServicePort;
        public string LocalIP { get; private set; } = "";

        private string ValidStart => $"{IPServices.BroadcastAddressRequestStr} {ServerName}";

        #endregion

        #region Member Data

        private Thread _workerThread;
        private bool _disposed = false;

        // Thread vars
        private DateTime _nextIPCheck = DateTime.MinValue;
        private IPEndPoint _localEP = null;
        private IPEndPoint _remoteEP = null;
        private UdpClient _udpClient = null;

        #endregion

        #region Functions

        #region Constructors

        public BroadcastAddressProvider(string serverName, string adaptorName = DefaultAdaptorName, int broadcastPort = DefaultBroadcastPort, int servicePort = DefaultServicePort)
        {
            ServerName = serverName;
            AdaptorName = adaptorName;
            BroadcastPort = broadcastPort;
            ServicePort = servicePort;
        }

        #endregion

        #region Public Functions

        public void Start()
        {
            _workerThread = new Thread(WorkerThread)
            {
                IsBackground = true,
                Name = "BroadcastAddressProvider Worker Thread"
            };

            _workerThread.Start();
        }

        public void Stop()
        {
            CancelRequested = true;
            int waitTime = 0;

            while (_workerThread.IsAlive && waitTime < 500)
            {
                waitTime += 100;
                Thread.Sleep(100);
            }

            if (_workerThread.IsAlive)
            {
                _workerThread.Abort();
            }
        }

        #endregion

        #region Worker Thread

        private void WorkerThread()
        {
            State = AddressState.NeedsIP;
            Trace.Listeners["nTact"].WriteLine("Starting the BroadcastAddressProvider thread.");

            try
            {
                while (!CancelRequested)
                {
                    try
                    {
                        CheckLocalIP();

                        switch (State)
                        {
                            case AddressState.NeedsIP:
                                // Failed to find a valid IP, so wait, then try again
                                Thread.Sleep(200);
                                break;

                            case AddressState.NeedsBinding:
                                BindUdpClient();
                                break;

                            case AddressState.Listening:
                                Listen();
                                break;

                            default:
                                Trace.Listeners["nTact"].WriteLine("BroadcastAddressProvided is in an unknown state -- resetting", "ERROR");
                                State = AddressState.NeedsIP;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.Listeners["nTact"].WriteLine($"Exception caught: {ex.Message}");
                    }
                }
            }
            finally
            {
                _udpClient?.Close();
            }
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Need to check local IP constantly, in case of DHCP change of address
        /// </summary>
        private void CheckLocalIP()
        {
            if (DateTime.Now > _nextIPCheck)
            {
                _nextIPCheck = DateTime.Now.AddSeconds(NumSecsBetweenIPChecks);
                var ip = IPServices.GetLocalIP(AdaptorName);

                if (string.IsNullOrEmpty(ip))
                {
                    Trace.Listeners["nTact"].WriteLine($"Could not get local IP for adapter {AdaptorName}.  Retrying in {NumSecsBetweenIPChecks} seconds", "ERROR");
                }
                else if (ip != LocalIP)
                {
                    LocalIP = ip;
                    State = AddressState.NeedsBinding;
                    Trace.Listeners["nTact"].WriteLine($"BroadcastAddressProvider found a new valid local IP, and will rebind: {ip}", "INFO");
                }
            }
        }

        private void BindUdpClient()
        {
            // Local End-Point
            _localEP = new IPEndPoint(IPAddress.Any, BroadcastPort);
            _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            _udpClient = new UdpClient(_localEP);
            _udpClient.EnableBroadcast = true;
            _udpClient.DontFragment = true;
            State = AddressState.Listening;
        }

        private void Listen()
        {
            string replyMsg;
            byte[] receiveBytes;

            try
            {
                _udpClient.Client.ReceiveTimeout = ReceiveTimeoutInMs;
                receiveBytes = _udpClient.Receive(ref _remoteEP);
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == (int)SocketError.TimedOut)
                {
                    // Timed out with no message, so we just keep listening
                    return;
                }
                else
                {
                    Trace.Listeners["nTact"].WriteLine($"SocketException caught in BroadcastAddressProvider.Listen:  {se.ErrorCode}: {se.Message}");
                    return;
                }
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine($"Exception caught in BroadcastAddressProvider.Listen:  {ex.Message}");
                return;
            }

            //Trace.Listeners["nTact"].WriteLine($"New broadcast message in BroadcastAddressProvider");

            // If we get here, the receive succeeded, so parse the message
            var rawMessageString = Encoding.ASCII.GetString(receiveBytes);
            //Trace.Listeners["nTact"].WriteLine($"New broadcast message in BroadcastAddressProvider:  {rawMessageString}");
            var msgs = rawMessageString.Split(new string[] { IPServices.EndMessageTag }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var msg in msgs)
            {
                var trimmed = msg.Trim();

                if (trimmed == ValidStart)
                {
                    replyMsg = $"{ValidStart} {LocalIP} {ServicePort} {IPServices.EndMessageTag}";
                }
                else
                {
                    replyMsg = $"{ValidStart} NAK {IPServices.EndMessageTag}";
                }

                Trace.Listeners["nTact"].WriteLine($"Got message {msg}, replying with {replyMsg}");
                var packetBytes = Encoding.ASCII.GetBytes(replyMsg);
                _udpClient.Send(packetBytes, packetBytes.Length, _remoteEP);
            }
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                CancelRequested = true;
                _disposed = true;
            }
        }

        #endregion

        #endregion
    }
}
