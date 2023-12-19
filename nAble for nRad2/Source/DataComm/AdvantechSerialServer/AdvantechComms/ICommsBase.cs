namespace nAble.DataComm.AdvantechSerialServer.AdvantechComms
{
    public interface ICommsBase
    {
        #region Properties

        string ServerName { get; }

        int MinAINumber { get; set; }
        int MaxAINumber { get; set; }

        int Total { get; }
                
        string IPAddress { get; }
        int Port { get; }

        bool Connected { get; }

        #endregion

        #region Functions

        bool Connect(string ip, int port);

        void Disconnect();

        bool GetValues(out ushort[] values);

        bool GetStatus(out int[] states);

        bool GetEnabled(out bool[] enabled);

        #endregion
    }
}
