namespace CommonLibrary.nTactServer.Messages
{
    public class SubstrateTypesMessage : MessageBase
    {
        #region Constants

        protected const string MsgTypeTag = "SUBS_TYPES";

        #endregion

        #region Member Data

        #endregion

        #region Properties

        public FileData SubstrateTypesData { get; set; } = new FileData();

        public override string MessageBody => SubstrateTypesData.HasData ? $"{SubstrateTypesData}" : "";

        public override string MessageReplyBody => "";

        public override string MessageType => MsgTypeTag;

        #endregion

        #region Functions

        public SubstrateTypesMessage() : base() { }

        /// <summary>
        /// Creates a user data message based on the filename given.  Throws in error conditions.
        /// </summary>
        /// <param name="userFileFullPath"></param>
        public SubstrateTypesMessage(string userFileFullPath) : base()
        {
            SubstrateTypesData.SetFile(userFileFullPath);
        }

        public override void ParseMessageBody(string messageBody)
        {
            SubstrateTypesData.Clear();
            SubstrateTypesData = FileData.ParseFromFileDataString(messageBody);
        }

        public override void ParseReplyBody(string replyBody)
        {
            // reply carries no new data
        }

        #endregion
    }
}
