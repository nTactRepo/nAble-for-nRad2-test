using CommonLibrary.nTactServer.Messages;
using CommonLibrary.Tcp;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.nTactServer
{
    public class NTactClient
    {
        #region Constants

        public const int DefaultClientTimeout = 10 * 1000;

        #endregion

        #region Properties

        public string Name { get; }
        public string IpAddress { get; private set; }
        public int Port { get; private set;}
        public bool Provisioned { get; private set; } = false;

        public IClientSource ClientSource { get; private set; } = new ClientSource();
        public int ClientTimeout { get; set; } = DefaultClientTimeout;
        public DateTime LastMessage { get; private set; } = DateTime.MinValue;

        #endregion

        #region Member Data

        private readonly BroadcastAddressRequester _requester = null;
        private int _numConsecutiveTimeouts = 0;

        #endregion

        #region Functions

        public NTactClient(string name, BroadcastAddressRequester requester)
        {
            Name = name;
            _requester = requester;
            requester.Completed += Requester_Completed;
            requester.Start();
        }

        public NTactClient(string name, string ip, int port)
        {
            Name = name;
            IpAddress = ip;
            Port = port;
            Provisioned = true;

            Trace.Listeners["nTact"].WriteLine($"Created an nTact Client: {name}, {ip}:{port}");
        }

        /// <summary>
        /// Synchronously send a message to this client.  The results will be in the altered message
        /// </summary>
        /// <param name="message">The message to send, and receive results with</param>
        /// <returns>True if the message was sent, false otherwise</returns>
        public bool SendMessage(MessageBase message)
        {
            if (!Provisioned)
            {
                return false;
            }
            else
            {
                return Task.Run(() => SendMessageAsync(message)).Result;
            }
        }

        /// <summary>
        /// Asynchronously send a message to this client.  The results will be in the altered message
        /// </summary>
        /// <param name="message">The message to send, and receive results with</param>
        /// <returns>True if the message was sent, false otherwise</returns>
        public async Task<bool> SendMessageAsync(MessageBase message)
        {
            if (!Provisioned)
            {
                return false;
            }

            bool succeeded = false;

            try
            {
                using (var client = ClientSource.GetClient())
                {
                    client.ReceiveTimeout = ClientTimeout;
                    client.SendTimeout = ClientTimeout;

                    var connectTask = client.ConnectAsync(IpAddress, Port);
                    var cancelTask = Task.Delay(ClientTimeout);

                    await Task.WhenAny(connectTask, cancelTask);

                    if (cancelTask.IsCompleted)
                    {
                        _numConsecutiveTimeouts++;
                        throw new Exception("TcpClient connect timed out");
                    }

                    if (!client.Connected)
                    {
                        Trace.Listeners["nTact"].WriteLine("Exception Sending Msg To Server, Not Connected To Server!", "ERROR");
                        throw new Exception("TcpClient did not connect correctly.  Check the address?");
                    }

                    // If we get here, we connected correctly
                    _numConsecutiveTimeouts = 0;

                    // Translate the passed message into ASCII and store it as a Byte array.
                    byte[] data = Encoding.ASCII.GetBytes(message.ToMessageString());

                    // Get a client stream for reading and writing.
                    using (var stream = client.GetStream())
                    {

                        // Send the message to the connected TcpServer.
                        await stream.WriteAsync(data, 0, data.Length);

                        // Receive the TcpServer.response.

                        // Buffer to store the response bytes.
                        data = new byte[1024];
                        string msgStr = "";
                        int bytes;

                        do
                        {
                            bytes = await stream.ReadAsync(data, 0, data.Length);
                            msgStr += Encoding.ASCII.GetString(data, 0, bytes);
                        } while (!msgStr.Contains(MessageBase.MsgEndTag) && bytes != 0);

                        message.ParseFromReplyString(msgStr);
                        succeeded = true;
                        LastMessage = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine($"Exception caught sending a message ({message.MessageType}) to the nTactServer: {ex.Message}");
            }

            return succeeded;
        }

        [Conditional("UnitTestsActive")]
        public void SetNewClientSource(IClientSource source)
        {
            ClientSource = source;
        }

        private void Requester_Completed(BroadcastAddressRequester.ResultState obj)
        {
            if (_requester.Result == BroadcastAddressRequester.ResultState.Success)
            {
                IpAddress = _requester.ServerIP;
                Port = _requester.ServerPort;
                Provisioned = true;

                Trace.Listeners["nTact"].WriteLine($"Received Broadcast Address for nTact Server {Name}:  {IpAddress}:{Port}");
            }
            else
            {
                _requester.ReFetch();
            }
        }

        #endregion
    }
}
