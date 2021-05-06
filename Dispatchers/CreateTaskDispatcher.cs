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
    internal class CreateTaskDispatcher : IUpdateDispatcher
    {
        public bool IsApplicable(State state, Update update)
        {
            var isSelectedOption = state != null
                && state.TaskCreationState == TaskCreationStateType.SelectingOption
                && update.Type == UpdateType.CallbackQuery;

            var EnteringTaskName = state != null
                && state.TaskCreationState == TaskCreationStateType.Creating
                && update.Type == UpdateType.Message;

            return isSelectedOption || EnteringTaskName;
                
        }


        private (Reply, State) DispatchOption(State state, Telegram.Bot.Types.CallbackQuery query)
        {
            if (query.Data.Deserialize()["action"] == "create_task")
            {
                state.TaskCreationState = TaskCreationStateType.Creating;
                var reply = new Reply(new TextMessageResult("Введите название задачи"));
                return (reply, state);
            }
            else
            {
                var reply = new Reply(new TextMessageResult("Опция не реализована"));
                return (reply, state);
            }
        }

        private (Reply, State) CreationOfDescription(State state, Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                var reply = new Reply(new TextMessageResult("Опция не реализована"));
                return (reply, state);
            }
            else
            {
                state.TaskCreationState = TaskCreationStateType.Creating;
                var reply = new Reply(new TextMessageResult("Введите описание задачи"));
                return (reply, state);
            }
        }

        /*private (Reply, State) CreationOfDescription(State state, Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                var reply = new Reply(new TextMessageResult("Опция не реализована"));
                return (reply, state);
            }
            else
            {
                state.TaskCreationState = TaskCreationStateType.Creating;
                var reply = new Reply(new TextMessageResult("Введите описание задачи"));
                return (reply, state);
            }
        }*/

        public (Reply, State) Dispatch(State state, Update update)
        {
            var isSelectedOption = state != null
                && state.TaskCreationState == TaskCreationStateType.SelectingOption
                && update.Type == UpdateType.CallbackQuery;

            var EnteringTaskName = state != null
                && state.TaskCreationState == TaskCreationStateType.Creating
                && update.Type == UpdateType.Message;
            var reply = new Reply(new TextMessageResult("Задача создана"));

            if (state.TaskCreationState == TaskCreationStateType.Creating
                || state.TaskCreationState == TaskCreationStateType.SelectingOption)
            {
                if (isSelectedOption)
                {
                    return DispatchOption(state, update.CallbackQuery);
                }
                else if (EnteringTaskName)
                {
                    return CreationOfDescription(state, update);
                }
                else
                {
                    throw null;
                }


            }

            return (reply, state);
        }

    }
}