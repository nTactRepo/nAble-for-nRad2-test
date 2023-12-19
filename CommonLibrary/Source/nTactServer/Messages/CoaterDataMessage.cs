namespace CommonLibrary.nTactServer.Messages
{
    public class CoaterDataMessage : MessageBase
    {
        #region Constants

        protected const string MsgTypeTag = "COATER-DATA";

        #endregion

        #region Properties

        public string SubstrateID { get; set;  } = "";

        public override string MessageType => MsgTypeTag;

        public override string MessageBody => MakeMessageString();

        public override string MessageReplyBody => "";

        #endregion

        #region Functions

        public CoaterDataMessage() : base() { }

        public override void ParseMessageBody(string messageBody)
        {
            SubstrateID = messageBody;
        }

        public override void ParseReplyBody(string replyBody)
        {
        }

        private string MakeMessageString()
        {
            return SubstrateID;
        }

        #endregion
    }
}
