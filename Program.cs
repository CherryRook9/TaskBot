using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;

using Telegram.Bot.Types.Enums;
using Telegram.Bot.Args;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using System.IO;
using System.Linq;


namespace SimpleTgBot
{
    static class Program
    {

        static TelegramBotClient Bot;
        static string BOT_TOKEN = "1797661510:AAHL8wXn6mewm93qEYIZqEVaTSoOT2nuY6w";

        static async Task Main()
        {
            var cts = new CancellationTokenSource();

            try
            {
                Bot = new TelegramBotClient(BOT_TOKEN);
                var me = await Bot.GetMeAsync();
                await Bot.SetMyCommandsAsync(new List<BotCommand>()
                {
                    new BotCommand() { Command = "/start", Description = "Активирует бота" },
                    new BotCommand() { Command = "/count", Description = "Считает выполнения этой команды" },
                    new BotCommand() { Command = "/create", Description = "Создает задачу" },
                });
                Console.WriteLine($"Start listening for @{me.Username}");

                await Bot.ReceiveAsync(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cts.Cancel();
            }
        }

        static Dictionary<long, State> states = new Dictionary<long, State>();

        static Application application = new Application();

        static async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            try
            {
                State state;
                if (!states.TryGetValue(GetChatId(update), out state))
                {
                    state = null;
                }

                var dispatchResult = application.Dispatch(state, update);
                if (dispatchResult.HasValue && dispatchResult.Value is var (reply, state_))
                {
                    states[GetChatId(update)] = state_;
                    if (reply.TextMessage is var textMessage)
                    {
                        await client.SendTextMessageAsync(state_.ChatId, textMessage.Text, replyMarkup: textMessage.ReplyMarkup);
                    }
                }
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(client, exception, cancellationToken);
            }
        }

        static long GetChatId(Update update)
        {
            if (update.Message != null)
            {
                return update.Message.Chat.Id;
                
            }
            else if (update.CallbackQuery != null)
            {
                return Convert.ToInt64(update.CallbackQuery.Data.Deserialize()["cid"]);
            }
            else
            {
                throw new Exception("Can't get chat id from this update.");
            }
        }

        static Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        static async Task Usage(Message message)
        {
            const string usage = "Usage:\n" +
                                    "/start  - send inline keyboard\n" +
                                    "/keyboard - send custom keyboard\n" +
                                    "/photo    - send a photo\n" +
                                    "/request  - request location or contact";
            await Bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: usage,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

    }

}