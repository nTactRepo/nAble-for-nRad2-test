namespace nAble.DataComm.AdvantechSerialServer.AdvantechComms
{
    public class Adam6017 : CommsBase
    {
        #region Properties

        public override string ServerName => "Adam6017";

        public override int MinAINumber { get; set; } = 0;
        public override int MaxAINumber { get; set; } = 7;

        #endregion

        #region Functions

        #region Constructors

        public Adam6017(LogEntry log) : base(log) { }

        public override bool GetValues(out ushort[] values)
        {
            int[] rawData = new int[Total];
            values = new ushort[Total];

            if (_socket.Modbus().ReadInputRegs(MinAINumber + 1, Total, out rawData))
            {
                for (int i = 0; i < Total; i++)
                {
                    values[i] = (ushort)rawData[i];
                }

                return true;
            }

            _log.log(LogType.TRACE, Category.ERROR, $"Could not retrieve analog input values for Adam6017");
            return false;
        }

        public override bool GetStatus(out int[] states)
        {
            int aiStatusStart = 101;

            if (_socket.Modbus().ReadInputRegs(aiStatusStart, Total, out states))
            {
                return true;
            }

            _log.log(LogType.TRACE, Category.ERROR, $"Could not retrieve status data for Adam6017");
            return false;
        }

        public override bool GetEnabled(out bool[] enabled)
        {
            if (_socket.AnalogInput().GetChannelEnabled(Total, out enabled))
            {
                return true;
            }

            _log.log(LogType.TRACE, Category.ERROR, $"Could not retrieve channel enabled data for Adam6017");
            return false;
        }

        #endregion

        #endregion
    }
}
