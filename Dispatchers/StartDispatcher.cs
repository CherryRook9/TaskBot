using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using System.Linq;
using Telegram.Bot.Args;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;

namespace SimpleTgBot.Dispatchers
{
    internal class StartDispatcher : IUpdateDispatcher
    {
        public bool IsApplicable(State state, Update update)
        {
            return state == null && update.Type == UpdateType.Message && update.Message.Text == "/start";
        }

        public (Reply, State) Dispatch(State state, Update update)
        {
            var chatId = update.Message.Chat.Id;
            return (new Reply(new TextMessageResult("Бот включен")), new State(chatId));
        }
    }
}