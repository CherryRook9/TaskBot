using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using Telegram.Bot;
using TelegramBotBase;
using TaskBot.Forms;
using Microsoft.EntityFrameworkCore;
using TaskBot.Services;

namespace TaskBot
{
    public static class Program
    {
        private const string tokenEnvVarName = "BOT_TOKEN";
        static string Token => Environment.GetEnvironmentVariable(tokenEnvVarName) 
            ?? throw new Exception($"Environment variable '{tokenEnvVarName}' not set.");
        const bool storeSessions = false;

        public static async Task Main(string[] args)
        {
            ConfigureLogger();
            try
            {
                ConfigureDependecies();

                var bot = new BotBase<StartForm>(Token);

                if (storeSessions)
                {
                    #pragma warning disable 0162
                    bot.StateMachine = new TelegramBotBase.States.XMLStateMachine(AppContext.BaseDirectory + "config\\states.xml");
                    #pragma warning restore
                }
                
                bot.BotCommands = new List<Telegram.Bot.Types.BotCommand> {
                    (new Telegram.Bot.Types.BotCommand() { Command = "start", Description = "Запускает бота"}),
                    (new Telegram.Bot.Types.BotCommand() { Command = "stop", Description = "Останавливает бота"}),
                };

                bot.Exception += (s, e) => Log.Logger.Error(e.Error, "Exception on {sender}", s.GetType());

                await bot.UploadBotCommands();
                    bot.Start();
                    Log.Logger.Information("Bot started, hit Ctrl+Z to stop.");
                    Console.In.ReadToEnd();
            }
            catch(Exception e)
            {
                Log.Logger.Fatal(e, "Fatal exception");
            }
        }

        public static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
        }

        public static void ConfigureDependecies()
        {
            var resolver = new DependecyResolver(Log.Logger)
                .Add(GetDbContext);
            
            DI.SetResolver(resolver);
        }

        internal static TasksContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite("Data Source=taskbot.sqlite")
                .Options;
            return new TasksContext(options);
        }
    }
}