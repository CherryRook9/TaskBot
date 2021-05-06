#nullable enable
using Telegram.Bot.Types.ReplyMarkups;

namespace SimpleTgBot
{
    internal class TextMessageResult
    {
        public TextMessageResult(string text)
        {
            Text = text;
            ReplyMarkup = null;
        }

        public TextMessageResult(string text, IReplyMarkup replyMarkup)
        {
            Text = text;
            ReplyMarkup = replyMarkup;
        }

        public string Text { get; }
        public IReplyMarkup? ReplyMarkup { get; }

    }
}