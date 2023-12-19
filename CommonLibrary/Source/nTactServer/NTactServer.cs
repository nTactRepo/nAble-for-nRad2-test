using CommonLibrary.nTactServer.Messages;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonLibrary.Source.nTactServer.Messages.Batch;
using CommonLibrary.Source.Model.CellStatus;

namespace CommonLibrary.nTactServer
{
    public class NTactServer
    {
        #region Events

        public event Action<string> UserDataChanged;
        public event Action SubstrateTypesChanged;
        public event Action<BatchStartedMessage> CellBatchStarted;
        public event Action CellBatchStopped;
        public event Action CellBatchNewLoop;
        public event Action CellBatchSubstrateIncoming;

        #endregion

        #region Constants

        public const string DefaultRecipeFolder = "Recipes";

        #endregion

        #region Properties

        public bool CancelRequested { get; set; } = false;

        public bool IsPaused { get; private set; } = false;

        public IPAddress IpAddress { get; private set; }

        public int Port { get; private set; }

        public string UserFolder
        {
            get => _userFolder;

            set
            {
                _userFolder = value;
                Directory.CreateDirectory(value);
            }
        }

        public string RecipeFolder { get; set; }

        public string SubstrateTypesFolder { get; set; }

        public bool NeedNewUserData { get; set; } = true;

        public bool NeedNewSubstrateTypes { get; set; } = true;

        public List<RecipeData> Recipes { get; private set; } = new List<RecipeData>();

        public string SubstrateID { get; set; } = "";

        public bool DebugEnabled { get; set; } = false;

        #endregion

        #region Data Members

        private string _userFolder = "";

        private Thread _serverThread;
        private TcpListener _server;
        private object _syncLock = new object();

        #endregion

        #region Functions

        #region Constructors

        public NTactServer(string ipAddress, int port)
        {
            Port = port;

            bool validIPAddr = IPServices.IsIpAddressValid(ipAddress);

            if (!validIPAddr || string.IsNullOrEmpty(ipAddress))
            {
                Trace.Listeners["nTact"].WriteLine($"NTactServer: Invalid IP Address! {ipAddress}:{port}");
            }

            if (validIPAddr)
            {
                IpAddress = IPAddress.Parse(ipAddress);
                Trace.Listeners["nTact"].WriteLine($"Created nTact server: {ipAddress}:{port}");
            }
            else
            {
                IpAddress = IPAddress.Parse("127.0.0.1");
                Trace.Listeners["nTact"].WriteLine($"Created nTact server: {ipAddress}:{port}  (Could not start given address)");
            }

            _serverThread = new Thread(ServerThread)
            {
                Name = $"NTact Server Thread ({IpAddress}:{Port})",
                IsBackground = true
            };

            _serverThread.Start();
        }

        #endregion

        #region Public Functions

        public void Start()
        {
            lock (_syncLock)
            {
                _server = new TcpListener(IpAddress, Port);
            }

            CancelRequested = false;
            Trace.Listeners["nTact"].WriteLine($"Starting nTact Server.");
        }

        public void Stop()
        {
            try
            {
                lock (_syncLock)
                {
                    _server.Stop();
                    _server = null;
                    Trace.Listeners["nTact"].WriteLine("NTactServer is stopped");
                }
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine($"Caught exception trying to stop the nTactServer:  {ex}");
                _server = null;
            }
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public void SetNewIPAddress(string ipAddress, int port = -1)
        {
            if (!IPServices.IsIpAddressValid(ipAddress))
            {
                Trace.Listeners["nTact"].WriteLine($"nTactServer was given a new IP address ({ipAddress}) that is not valid.  Ignoring...");
                return;
            }

            if (port < -1 || port > 65535)
            {
                Trace.Listeners["nTact"].WriteLine($"nTactServer was given a new port ({port}) that is not valid.  Ignoring...");
                return;
            }

            Stop();
            IpAddress = IPAddress.Parse(ipAddress);
            
            if (port != -1)
            {
                Port = port;
            }

            Trace.Listeners["nTact"].WriteLine($"nTactServer received a new IP Address:  {IpAddress}:{Port}");

            Start();
        }

        public void SetRecipes(List<RecipeData> recipes)
        {
            lock (_syncLock)
            {
                Recipes = recipes;
            }
        }

        #endregion

        #region Private Functions

        private void ServerThread()
        {
            // Enter the listening loop.
            while (true)
            {
                try
                {
                    Thread.Sleep(50);

                    if (CancelRequested)
                    {
                        Trace.Listeners["nTact"].WriteLine("NTactServer has been requested to stop.");
                        _server?.Stop();
                        return;
                    }

                    lock (_syncLock)
                    {
                        if (_server == null)
                        {
                            Start();
                            continue;
                        }

                        if (!_server.Server.IsBound)
                        {
                            // Start listening for client requests.
                            try
                            {
                                _server.Start();
                            }
                            catch (SocketException se)
                            {
                                Trace.Listeners["nTact"].WriteLine($"NTactServer got a socket error on starting to listen:  {se.Message}");
                                continue;
                            }
                        }

                        if (DebugEnabled)
                        {
                            Trace.Listeners["Debug"].WriteLine($"NTactServer has started listening for messages.");
                        }

                        if (!IsPaused && (_server?.Pending() ?? false))
                        {
                            using (var client = _server.AcceptTcpClient())
                            {
                                if (DebugEnabled)
                                {
                                    Trace.Listeners["Debug"].WriteLine($"NTactServer: Connected to a client: {client.Client.RemoteEndPoint}");
                                }

                                HandleClientRequest(client);

                                if (DebugEnabled)
                                {
                                    Trace.Listeners["Debug"].WriteLine("NTactServer: Client reply sent");
                                }
                            }
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    // Need to break to get out of the while loop
                    Trace.Listeners["nTact"].WriteLine($"NTactServer thread got hard aborted.", "ERROR");
                    break;
                }
                catch (Exception ex)
                {
                    Trace.Listeners["nTact"].WriteLine($"NTactServer got exception talking to a client:  {ex.Message}", "ERROR");
                }
            }
        }

        private void HandleClientRequest(TcpClient client)
        {
            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;
            var msgBytes = new byte[1024];
            string rawMessageString = "";

            // Loop to receive all the data sent by the client.
            do
            {
                i = stream.Read(msgBytes, 0, msgBytes.Length);
                // Translate data bytes to a ASCII string.
                rawMessageString += System.Text.Encoding.ASCII.GetString(msgBytes, 0, i);
            } while (!rawMessageString.Contains(MessageBase.MsgEndTag) && i != 0);

            if (DebugEnabled)
            {
                Trace.Listeners["Debug"].WriteLine($"NTactServer: Message Received: {rawMessageString}");
            }

            var message = GetMessageFromRawString(rawMessageString);

            HandleMessage(message);

            rawMessageString = message.ToReplyString();

            if (DebugEnabled)
            {
                Trace.Listeners["Debug"].WriteLine($"NTactServer: Message To Send: {rawMessageString}");
            }

            msgBytes = System.Text.Encoding.ASCII.GetBytes(rawMessageString);

            // Send back a response.
            stream.Write(msgBytes, 0, msgBytes.Length);
        }

        private MessageBase GetMessageFromRawString(string rawMessageString)
        {
            var message = MessageBase.ConstructMessageFromRawString(rawMessageString);
            message.ParseFromMessageString(rawMessageString);

            return message;
        }

        virtual protected bool HandleMessage(MessageBase message)
        {
            message.Succeeded = false;
            bool handled = false;

            if (message is TestMessage)
            {
                handled = true;
                message.Succeeded = true;
            }
            else if (message is UserDataMessage udm)
            {
                handled = true;
                message.Succeeded = HandleUserData(udm);
            }
            else if (message is FetchRecipesMessage frm)
            {
                handled = true;
                message.Succeeded = HandleFetchRecipes(frm);
            }
            else if (message is StatusMessage sm)
            {
                handled = true;
                message.Succeeded = HandleStatus(sm);
            }
            else if (message is CoaterDataMessage cdm)
            {
                handled = true;
                message.Succeeded = HandleCoaterDataMessage(cdm);
            }
            else if (message is SubstrateTypesMessage stm)
            {
                handled = true;
                message.Succeeded = HandleSubstrateTypesMessage(stm);
            }
            else if (message is BatchStartedMessage bsm)
            {
                handled = true;
                message.Succeeded = HandleBatchStartedMessage(bsm);
            }
            else if (message is BatchStoppedMessage bstm)
            {
                handled = true;
                message.Succeeded = HandleBatchStoppedMessage(bstm);
            }
            else if (message is NewLoopStartedMessage nlsm)
            {
                handled = true;
                message.Succeeded = HandleNewLoopStartedMessage(nlsm);
            }
            else if (message is SubstrateMovingToCoaterMessage smtc)
            {
                handled = true;
                message.Succeeded = HandleSubstrateMovingToCoaterMessage(smtc);
            }

            return handled;
        }

        private bool HandleUserData(UserDataMessage udm)
        {
            bool handled = false;

            try
            {
                udm.UserData.WriteFile(UserFolder);
                UserDataChanged?.Invoke(Path.Combine(UserFolder, udm.UserData.Filename));
                handled = true;
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine($"Caught an exception trying to write the user data file:  {ex.Message}");
            }

            return handled;
        }

        private bool HandleFetchRecipes(FetchRecipesMessage frm)
        {
            bool handled = true;

            lock (_syncLock)
            {
                foreach (RecipeData recipeData in Recipes)
                {
                    frm.AddFile(recipeData);
                }
            }

            return handled;
        }

        private bool HandleStatus(StatusMessage sm)
        {
            if (NeedNewUserData)
            {
                sm.RequiredMessages.Add(StatusMessage.RequiredMessageType.UserData);
                NeedNewUserData = false;
            }

            if (NeedNewSubstrateTypes)
            {
                sm.RequiredMessages.Add(StatusMessage.RequiredMessageType.SubstrateTypes);
                NeedNewSubstrateTypes = false;
            }

            return true;
        }

        private bool HandleCoaterDataMessage(CoaterDataMessage cdm)
        {
            SubstrateID = cdm.SubstrateID;
            return true;
        }

        private bool HandleSubstrateTypesMessage(SubstrateTypesMessage stm)
        {
            bool handled = false;

            try
            {
                stm.SubstrateTypesData.WriteFile(SubstrateTypesFolder);
                SubstrateTypesChanged?.Invoke();
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine($"Caught an exception trying to write the substrate types data file:  {ex.Message}");
            }

            return handled;
        }

        private bool HandleBatchStartedMessage(BatchStartedMessage bsm)
        {
            CellBatchStarted?.Invoke(bsm);
            return true;
        }

        private bool HandleBatchStoppedMessage(BatchStoppedMessage bstm)
        {
            CellBatchStopped?.Invoke();
            return true;
        }

        private bool HandleNewLoopStartedMessage(NewLoopStartedMessage nlsm)
        {
            CellBatchNewLoop?.Invoke();
            return true;
        }

        private bool HandleSubstrateMovingToCoaterMessage(SubstrateMovingToCoaterMessage smtc)
        {
            CellBatchSubstrateIncoming?.Invoke();
            return true;
        }

        #endregion

        #endregion
    }
}
