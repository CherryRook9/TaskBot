using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using System.Linq;
using Telegram.Bot.Args;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using SimpleTgBot.Dispatchers;

namespace SimpleTgBot
{
    class Application
    {
        private List<IUpdateDispatcher> dispatchers;

        public Application()
        {
            dispatchers = new List<IUpdateDispatcher>() 
            {
                new StartDispatcher(),
                new CountDispatcher(),
                new CreateTaskDispatcher(),
                new DeleteTaskDispatcher(),
                new EditTaskDispatcher(),
                new WatchTaskDispatcher(),
                new TaskOperationDispatcher()
            };
        }

        internal (Reply, State)? Dispatch(State state, Update update)
        {
            foreach (var dispatcher in dispatchers)
            {
                if (dispatcher.IsApplicable(state, update))
                {
                    return dispatcher.Dispatch(state, update);
                }
            }

            return null;
        }
    }

}