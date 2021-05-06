using Telegram.Bot.Types;
using SimpleTgBot;
using System.Collections.Generic;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;

using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using System.IO;
using Telegram.Bot.Args;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;

namespace SimpleTgBot.Dispatchers
{
    internal class TaskOperationDispatcher : IUpdateDispatcher
    {
        public bool IsApplicable(State state, Update update)
        {
            return state != null && update.Type == UpdateType.Message && update.Message.Text == "/create";
        }
        public static (Reply, State) DispatchStart(State state, Update update)
        {
            var cid = update.Message.Chat.Id.ToString();
            var buttonData = new Dictionary<string, string>() { { "cid", cid } };
            var markup = new InlineKeyboardMarkup(new[]{
                new [] // first row
                {
                    InlineKeyboardButton.WithCallbackData(
                        "Создать задачу",
                        buttonData
                            .Append(new KeyValuePair<string, string>("action", "create_task"))
                            .Serialize()),
                    InlineKeyboardButton.WithCallbackData(
                        "Посмореть задачи",
                        buttonData
                            .Append(new KeyValuePair<string, string>("action", "watch_tasks"))
                            .Serialize()),
                },
                new [] // second row
                {
                    InlineKeyboardButton.WithCallbackData(
                        "Удалить задачу",
                        buttonData
                            .Append(new KeyValuePair<string, string>("action", "delete_task"))
                            .Serialize()),
                    InlineKeyboardButton.WithCallbackData(
                        "Редактировать задачу",
                        buttonData
                            .Append(new KeyValuePair<string, string>("action", "edit_task"))
                            .Serialize()),
                }
            });

            state.TaskCreationState = TaskCreationStateType.SelectingOption;
            var reply = new Reply(new TextMessageResult($"Выберете действие", markup));
            return (reply, state);
        }
        public (Reply, State) Dispatch(State state, Update update)
        {
            return DispatchStart(state, update);
        }
    }
}