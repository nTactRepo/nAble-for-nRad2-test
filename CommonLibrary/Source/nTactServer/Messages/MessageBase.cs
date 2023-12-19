using CommonLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.nTactServer.Messages
{
    public abstract class MessageBase
    {
        #region Constants

        public const string MsgEndTag = "{END}";

        public const string Success = "ACK";
        public const string Failure = "NACK";

        public const char Separator = '^';

        #endregion

        #region Member Data

        private readonly string _type;
        private static readonly Dictionary<string, ConstructorInfo> _constructors = new Dictionary<string, ConstructorInfo>();

        #endregion

        #region Properties

        public bool Succeeded { get; set; } = true;

        public string StatusString => Succeeded ? Success : Failure;

        public abstract string MessageType { get; }

        public abstract string MessageBody { get; }

        public abstract string MessageReplyBody { get; }

        #endregion

        #region Functions

        /// <summary>
        /// Static constructor to set up the factory dictionary
        /// </summary>
        static MessageBase()
        {
            var types = typeof(MessageBase).GetAllDerivedTypes(includeAbstract: false).ToList();

            foreach (var type in types)
            {
                var ctor = type.GetConstructor(new Type[] { });

                if (ctor != null)
                {
                    var obj = (MessageBase)ctor.Invoke(new Type[] { });

                    if (obj != null)
                    {
                        _constructors[obj.MessageType] = ctor;
                    }
                }
            }
        }

        public MessageBase()
        {
            _type = MessageType;
        }

        public string ToMessageString()
        {
            var body = MessageBody;
            var bodyStr = string.IsNullOrEmpty(body) ? "" : $"{Separator}{body}";
            return $"{_type}{bodyStr}{Separator}{MsgEndTag}";
        }

        public string ToReplyString()
        {
            var body = MessageReplyBody;
            var bodyStr = string.IsNullOrEmpty(body) ? "" : $"{Separator}{body}";
            return $"{_type}{Separator}{StatusString}{bodyStr}{Separator}{MsgEndTag}";
        }

        public void ParseFromMessageString(string messageString)
        {
            var body = ValidateAndReturnBody(messageString, isReply: false);
            ParseMessageBody(body);
        }

        public void ParseFromReplyString(string replyString)
        {
            var body = ValidateAndReturnBody(replyString, isReply: true);
            ParseReplyBody(body);
        }

        static public MessageBase ConstructMessageFromRawString(string rawString)
        {
            var tag = GetMessageTypeFromRawString(rawString);

            if (!_constructors.ContainsKey(tag))
            {
                throw new InvalidOperationException($"Could not construct a message with message type tag {tag}");
            }

            var msg = (MessageBase)_constructors[tag].Invoke(new object[] { });
            return msg;
        }

        public abstract void ParseMessageBody(string messageBody);

        public abstract void ParseReplyBody(string replyBody);

        static private string GetMessageTypeFromRawString(string rawMessage)
        {
            int firstIndex = 0;

            firstIndex = rawMessage.IndexOf(Separator);

            if (firstIndex < 0)
            {
                return "";
            }

            return rawMessage.Substring(0, firstIndex);
        }

        private string ValidateAndReturnBody(string msg, bool isReply)
        {
            int firstIndex, secondIndex = 0;

            firstIndex = msg.IndexOf(Separator);

            if (firstIndex < 0 || firstIndex + 1 >= msg.Length)
            {
                throw new Exception("Invalid message format");
            }

            var typeStr = msg.Substring(0, firstIndex);

            if (typeStr != MessageType)
            {
                throw new Exception($"Invalid message format. Expected {MessageType}, got {typeStr}");
            }

            if (isReply)
            {
                secondIndex = msg.IndexOf(Separator, firstIndex + 1);

                if (secondIndex < 0)
                {
                    throw new Exception("Invalid message format");
                }

                Succeeded = msg.Substring(firstIndex + 1, secondIndex - firstIndex - 1) == Success;
            }

            var body = msg.Substring(isReply ? secondIndex + 1 : firstIndex + 1);

            if (!body.Contains("{END}"))
            {
                throw new Exception("End tag is missing!");
            }

            body = body.Replace($"{MsgEndTag}", "");

            if (body.EndsWith($"{Separator}"))
            {
                body = body.Remove(body.Length - 1);
            }
                    
            return body;
        }

        #endregion
    }
}
