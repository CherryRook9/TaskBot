using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using SimpleTgBot;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using System.Linq;
using Telegram.Bot.Args;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;

namespace SimpleTgBot.Dispatchers
{
    internal class EditTaskDispatcher : IUpdateDispatcher
    {
        public bool IsApplicable(State state, Update update)
        {
            return state != null && update.Type == UpdateType.Message && update.Message.Text == "/count";
        }

        public (Reply, State) Dispatch(State state, Update update)
        {
            state.MessageCount++;
            return (new Reply(new TextMessageResult(
                $"Вы спросили это {state.MessageCount} раз.")), 
                state);
        }
    }
}