using CommonLibrary.nTactServer.Messages;
using System.ComponentModel;
using System.Text;

namespace CommonLibrary.Source.nTactServer.Messages.Batch
{
    public class BatchStartedMessage : MessageBase
    {
        #region Constants

        protected const char CommandSeparator = '=';

        protected const string MsgTypeTag = "BATCH_START";

        protected const string PSEnabledType = "PSEnabled";

        protected const string PSIdleTimeType = "PSIdleTime";

        //protected const string ERCEnabledType = "ERCEnabled";

        //protected const string ERCIdleTimeType = "ERCIdleTime";

        #endregion

        #region Properties

        public bool UsePurgeScrub { get; set; } = false;
        public int PurgeScrubIdleTime { get; set; } = 60;

        //public bool UseExtraRingClean { get; set; } = false;
        //public int ExtraRingCleanIdleTime { get; set; } = 10;


        public override string MessageType => MsgTypeTag;

        public override string MessageBody => MakeBodyString();

        public override string MessageReplyBody => "";

        #endregion

        #region Functions

        #region Constructors

        public BatchStartedMessage() { }

        #endregion

        #region Public Functions

        public override void ParseMessageBody(string messageBody)
        {
            var parts = messageBody.Split(new char[] { Separator });

            foreach (var part in parts)
            {
                if (!ParseCommand(part))
                {
                    return;
                }
            }
        }

        public override void ParseReplyBody(string replyBody)
        {
            // Nothing to do -- no data in reply for this message
        }

        #endregion

        #region Private Functions

        private string MakeBodyString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{PSEnabledType}{CommandSeparator}{UsePurgeScrub}");
            builder.Append(Separator);
            builder.Append($"{PSIdleTimeType}{CommandSeparator}{PurgeScrubIdleTime}");
            //builder.Append(Separator);
            //builder.Append($"{ERCEnabledType}{CommandSeparator}{UseExtraRingClean}");
            //builder.Append(Separator);
            //builder.Append($"{ERCIdleTimeType}{CommandSeparator}{ExtraRingCleanIdleTime}");

            return builder.ToString();
        }

        private bool ParseCommand(string command)
        {
            var parts = command.Split(new char[] { CommandSeparator });

            if (parts.Length != 2)
            {
                return false;
            }

            switch (parts[0])
            {
                case PSEnabledType:
                    if (bool.TryParse(parts[1], out var usePurgeScrub))
                    {
                        UsePurgeScrub = usePurgeScrub;
                    }
                    break;

                case PSIdleTimeType:
                    if (int.TryParse(parts[1], out var psIdle))
                    {
                        PurgeScrubIdleTime = psIdle;
                    }
                    break;

                //case ERCEnabledType:
                //    if (bool.TryParse(parts[1], out var useErc))
                //    {
                //        UseExtraRingClean = useErc;
                //    }
                //    break;

                //case ERCIdleTimeType:
                //    if (int.TryParse(parts[1], out var ercIdle))
                //    {
                //        ExtraRingCleanIdleTime = ercIdle;
                //    }
                //    break;

                default:
                    return false;
            }

            return true;
        }

        #endregion

        #endregion
    }
}
