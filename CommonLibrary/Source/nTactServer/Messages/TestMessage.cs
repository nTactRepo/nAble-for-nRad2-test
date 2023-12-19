using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.nTactServer.Messages
{
    public class TestMessage : MessageBase
    {
        #region Constants

        public const string MsgTypeTag = "TEST";
        public const string ReplyTag = "Echo: ";

        #endregion

        #region Member Data

        #endregion

        #region Properties

        public string SentString { get; set; }

        public override string MessageType => MsgTypeTag;

        public override string MessageBody => $"{SentString}";

        public override string MessageReplyBody => $"{ReplyTag}{SentString}";

        #endregion

        #region Functions

        public TestMessage() : base() { }

        public TestMessage(string msgString) : base()
        {
            SentString = msgString;
        }

        public override void ParseMessageBody(string messageBody)
        {
            SentString = messageBody;
        }

        public override void ParseReplyBody(string replyBody)
        {
            SentString = replyBody.Replace(ReplyTag, "");
        }

        #endregion
    }
}
