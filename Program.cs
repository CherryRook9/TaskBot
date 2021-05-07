using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using Telegram.Bot;
using TelegramBotBase;
using TaskBot.Forms;

namespace TaskBot {
    public static class Program
    {
        static string token = Environment.GetEnvironmentVariable("BOT_TOKEN");

        public static async Task Main(string[] args)
        {
            ConfigureLogger();

            var bot = new BotBase<StartForm>(token);
            bot.BotCommands = new List<Telegram.Bot.Types.BotCommand> {
                (new Telegram.Bot.Types.BotCommand() { Command = "start", Description = "Запускает бота"}),
                (new Telegram.Bot.Types.BotCommand() { Command = "stop", Description = "Останавливает бота"}),
            };
            await bot.UploadBotCommands();
            bot.Start();
            Log.Logger.Information("Bot started, hit Ctrl+Z to stop.");
            await Console.In.ReadToEndAsync();
        }

        public static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}