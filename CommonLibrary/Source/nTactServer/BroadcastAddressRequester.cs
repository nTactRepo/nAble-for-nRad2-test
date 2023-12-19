using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CommonLibrary.nTactServer
{
    public class BroadcastAddressRequester
    {
        #region Events

        public event Action<ResultState> Completed;

        #endregion

        #region Constants and Enums

        private const int DefaultBroadcastPort = 57258;
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan DefaultResendTimeout = TimeSpan.FromSeconds(5);
        private const int DefaultReciveTimeoutInMs = 250;

        private enum FetchState
        {
            Initializing,
            NeedSendMessage,
            WaitingForReply,
            Done
        }

        public enum ResultState
        {
            Running,
            Success,
            TimedOut,
            ServerError,
            InternalError
        }

        #endregion

        #region Properties

        public ResultState Result { get; private set; } = ResultState.Running;
        public bool Running => _state == FetchState.NeedSendMessage || _state == FetchState.WaitingForReply;
        public bool HasValidServerData => Result == ResultState.Success;
        public int BroadcastPort { get; set; } = DefaultBroadcastPort;
        public TimeSpan Timeout { get; set; } = DefaultTimeout;
        public TimeSpan ResendTimeout { get; set; } = DefaultResendTimeout;
        public string ServerName { get; set; } = "";
        public string ServerIP { get; private set; } = "";
        public int ServerPort { get; private set; } = 0;

        private bool CancelRequested { get; set; } = false;
        private string SendMessageStr => $"{IPServices.BroadcastAddressRequestStr} {ServerName} {IPServices.EndMessageTag}";
        private string RemovePart => $"{IPServices.BroadcastAddressRequestStr} {ServerName} ";

        #endregion

        #region Member Data

        private FetchState _state = FetchState.Initializing;

        private Thread _workerThread;
        private bool _disposed = false;
        private readonly object _lock = new object();

        // Thread vars
        private DateTime _nextIPCheck = DateTime.MinValue;
        private UdpClient _udp = null;
        private IPEndPoint _endpoint = null;
        private DateTime _nextResend = DateTime.MinValue;
        private DateTime _timeoutTime = DateTime.MaxValue;

        #endregion

        #region Functions

        #region Constructors

        public BroadcastAddressRequester(string serverName)
        {
            ServerName = serverName;
        }

        #endregion

        #region Public Functions

        public void Start()
        {
            _state = FetchState.NeedSendMessage;

            StartThread();
        }

        public void ReFetch()
        {
            if (Running && !HasValidServerData)
            {
                return;
            }

            if (Running)
            {
                CancelRequested = true;
                Thread.Sleep(DefaultReciveTimeoutInMs);

                if (_workerThread.IsAlive)
                {
                    _workerThread.Abort();
                }
            }

            StartThread();
        }

        #endregion

        #region Worker Thread

        private void StartThread()
        {
            _workerThread = new Thread(WorkerThread)
            {
                IsBackground = true,
                Name = "BroadcastAddressRequester Worker Thread"
            };

            _workerThread.Start();
        }

        private void WorkerThread()
        {
            _state = FetchState.NeedSendMessage;
            _timeoutTime = DateTime.Now.Add(Timeout);
            _nextResend = DateTime.MinValue;

            while (!CancelRequested && Running)
            {
                if (DateTime.Now > _timeoutTime)
                {
                    CompleteRun(ResultState.TimedOut);
                    break;
                }

                try
                {
                    switch (_state)
                    {
                        case FetchState.NeedSendMessage:
                            SendMessage();
                            _state = FetchState.WaitingForReply;
                            break;

                        case FetchState.WaitingForReply:
                            CheckForReply();
                            break;

                        default:
                            Trace.Listeners["nTact"].WriteLine($"BroadcastAddressRequester has unknown state: {_state}");
                            CompleteRun(ResultState.InternalError);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Trace.Listeners["nTact"].WriteLine($"Exception caught in BroadcastAddressRequester {ex.Message}");
                }
            }

            Completed?.Invoke(Result);
        }

        private void SendMessage()
        {
            try
            {
                _udp = new UdpClient();
                _udp.EnableBroadcast = true;
                _udp.DontFragment = true;

                _endpoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), BroadcastPort);

                var bytes = Encoding.ASCII.GetBytes(SendMessageStr);
                _udp.Send(bytes, bytes.Length, _endpoint);
                _state = FetchState.WaitingForReply;
                _nextResend = DateTime.Now.Add(ResendTimeout);
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine($"Exception caught in BroadcastAddressRequester.SendMessage:  {ex.Message}");
            }
        }

        private void CheckForReply()
        {
            // If good, set address and port and exit
            // If NAK received, fail out with error event(?)
            // If timed out waiting, resend

            byte[] receiveBytes;

            if (DateTime.Now > _nextResend)
            {
                // We have waiting long enough for the reply -- try again
                _state = FetchState.NeedSendMessage;
                return;
            }

            try
            {
                _udp.Client.ReceiveTimeout = DefaultReciveTimeoutInMs;
                receiveBytes = _udp.Receive(ref _endpoint);
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == (int)SocketError.TimedOut)
                {
                    // Timed out with no response, so we just keep waiting
                    return;
                }
                else
                {
                    Trace.Listeners["nTact"].WriteLine($"SocketException caught in BroadcastAddressRequester.CheckForReply:  {se.ErrorCode}: {se.Message}");
                    return;
                }
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine($"Exception caught in BroadcastAddressRequester.CheckForReply:  {ex.Message}");
                return;
            }

            var rawMessageString = Encoding.ASCII.GetString(receiveBytes, 0, receiveBytes.Length);
            var msgs = rawMessageString.Split(new string[] { IPServices.EndMessageTag }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var msg in msgs)
            {

                if (!msg.StartsWith(RemovePart))
                {
                    // This log line is sometimes useful, but potentially too spammy to be left in all of the time
                    //Trace.Listeners["nTact"].WriteLine($"BroadcastAddressRequester.CheckForReply received a response from an incorrect server: {msg}");
                    return;
                }
                else
                {
                    var newMsg = msg.Replace(RemovePart, "");

                    if (newMsg.StartsWith("NAK"))
                    {
                        Trace.Listeners["nTact"].WriteLine($"BroadcastAddressRequester.CheckForReply received a NAK -- trying again");
                    }
                    else
                    {
                        var parts = newMsg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        
                        if (parts.Length != 2)
                        {
                            Trace.Listeners["nTact"].WriteLine($"BroadcastAddressRequester.CheckForReply received a message with the wrong format: {newMsg}");
                        }
                        else
                        {
                            if (int.TryParse(parts[1], out int port))
                            {
                                ServerIP = parts[0];
                                ServerPort = port;

                                Trace.Listeners["nTact"].WriteLine($"BroadcastAddressRequester Successfully found ServerIP:  {ServerIP}:{ServerPort}");
                                CompleteRun(ResultState.Success);
                                return;
                            }
                            else
                            {
                                Trace.Listeners["nTact"].WriteLine($"BroadcastAddressRequester.CheckForReply received a message with a bad port number: {parts[1]}");
                            }
                        }
                    }
                }
            }
        }

        private void CompleteRun(ResultState state)
        {
            _state = FetchState.Done;
            Result = state;
        }

        #endregion

        #region Private Functions

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
