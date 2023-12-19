using CommonLibrary.nTactServer.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.nTactServer.Messages
{
    public class WriteModbusAllMessage : ModbusBase
    {
        #region Constants

        protected const string MsgTypeTag = "MODBUS-WRITE";

        #endregion

        #region Properties

        public List<ushort> HoldingRegisters { get; set; } = new List<ushort>();
        public List<bool> Coils { get; set; } = new List<bool>();

        public override string MessageType => MsgTypeTag;

        public override string MessageBody => MakeMessageString();

        public override string MessageReplyBody => "";

        #endregion

        #region Functions

        public WriteModbusAllMessage() : base() { }

        public override void ParseMessageBody(string messageBody)
        {
            ParseModbusMessageString(messageBody, Coils, HoldingRegisters);
        }

        public override void ParseReplyBody(string replyBody)
        {
            //Empty as the reply contains no extra data
        }

        private string MakeMessageString()
        {
            return MakeModbusString(Coils, HoldingRegisters);
        }

        #endregion
    }
}
