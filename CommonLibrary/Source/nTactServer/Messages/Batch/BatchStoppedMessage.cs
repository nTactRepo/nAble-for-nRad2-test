using CommonLibrary.nTactServer.Messages;

namespace CommonLibrary.Source.nTactServer.Messages.Batch
{
    public class BatchStoppedMessage : MessageBase
    {
        #region Constants

        protected const string MsgTypeTag = "BATCH_STOPPED";

        #endregion

        #region Properties

        public override string MessageType => MsgTypeTag;

        public override string MessageBody => "";

        public override string MessageReplyBody => "";

        #endregion

        #region Functions

        #region Constructors

        public BatchStoppedMessage() { }

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
