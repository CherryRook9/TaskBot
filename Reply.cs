#nullable enable

namespace SimpleTgBot
{
    internal class Reply
    {
        public Reply(TextMessageResult textMessage)
        {
            TextMessage = textMessage;
        }

        public TextMessageResult? TextMessage { get; }
    }
}