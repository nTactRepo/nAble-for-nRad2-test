using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.nTactServer.Messages
{
    public class StatusMessage : MessageBase
    {
        #region Constants

        public const string MsgTypeTag = "STATUS";

        public enum RequiredMessageType { UserData, SubstrateTypes }

        #endregion

        #region Member Data

        #endregion

        #region Properties

        public List<RequiredMessageType> RequiredMessages { get; set; } = new List<RequiredMessageType>();

        public override string MessageType => MsgTypeTag;

        public override string MessageBody => "";

        public override string MessageReplyBody => MakeReplyString();

        #endregion

        #region Functions

        public StatusMessage() : base() { }

        public override void ParseMessageBody(string messageBody)
        {
        }

        public override void ParseReplyBody(string replyBody)
        {
            RequiredMessages.Clear();

            var parts = replyBody.Split(new char[] { Separator });
            
            foreach (var part in parts)
            {
                if (Enum.TryParse(part, out RequiredMessageType messageType))
                {
                    RequiredMessages.Add(messageType);
                }
            }
        }

        private string MakeReplyString()
        {
            string reply = "";
            bool first = true;

            foreach (var messageType in RequiredMessages)
            {
                reply += first ? $"{messageType}" : $"{Separator}{messageType}";
                first = false;
            }

            return reply;
        }

        #endregion
    }
}
