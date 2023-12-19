namespace CommonLibrary.nTactServer.Messages
{
    public class UserDataMessage : MessageBase
    {
        #region Constants

        protected const string MsgTypeTag = "USER";

        #endregion

        #region Member Data

        #endregion

        #region Properties

        public FileData UserData { get; set; } = new FileData();

        public override string MessageBody => UserData.HasData ? $"{UserData}" : "";

        public override string MessageReplyBody => "";

        public override string MessageType => MsgTypeTag;

        #endregion

        #region Functions

        public UserDataMessage() : base() { }

        /// <summary>
        /// Creates a user data message based on the filename given.  Throws in error conditions.
        /// </summary>
        /// <param name="userFileFullPath"></param>
        public UserDataMessage(string userFileFullPath) : base()
        {
            UserData.SetFile(userFileFullPath);
        }

        public override void ParseMessageBody(string messageBody)
        {
            UserData.Clear();
            UserData = FileData.ParseFromFileDataString(messageBody);
        }

        public override void ParseReplyBody(string replyBody)
        {
            // reply carries no new data
        }

        #endregion
    }
}
