using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.nTactServer.Messages
{
    public class ReadModbusMessage : ModbusBase
    {
        #region Constants

        protected const string MsgTypeTag = "MODBUS-READ";

        #endregion

        #region Properties

        public List<ushort> InputRegisters { get; set; } = new List<ushort>();
        public List<bool> InputDiscretes { get; set; } = new List<bool>();

        public override string MessageType => MsgTypeTag;

        public override string MessageBody => "";

        public override string MessageReplyBody => MakeReplyString();

        #endregion

        #region Functions

        public ReadModbusMessage() : base() { }

        public override void ParseMessageBody(string messageBody)
        {
            // No data in the message body
        }

        public override void ParseReplyBody(string replyBody)
        {
            ParseModbusMessageString(replyBody, InputDiscretes, InputRegisters);
        }

        private string MakeReplyString()
        {
            return MakeModbusString(InputDiscretes, InputRegisters);
        }

        #endregion
    }
}
