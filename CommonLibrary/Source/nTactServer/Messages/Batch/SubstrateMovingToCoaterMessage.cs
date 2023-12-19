using CommonLibrary.nTactServer.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Source.nTactServer.Messages.Batch
{
    public class SubstrateMovingToCoaterMessage : MessageBase
    {
        #region Constants

        protected const string MsgTypeTag = "SUB_MOVING_TO_COATER";

        #endregion

        #region Properties

        public override string MessageType => MsgTypeTag;

        public override string MessageBody => "";

        public override string MessageReplyBody => "";

        #endregion

        #region Functions

        #region Constructors

        public SubstrateMovingToCoaterMessage() { }

        #endregion

        #region Public Functions

        public override void ParseMessageBody(string messageBody)
        {
            // Nothing to do -- no data in body for this message
        }

        public override void ParseReplyBody(string replyBody)
        {
            // Nothing to do -- no data in reply for this message
        }

        #endregion

        #endregion
    }
}
