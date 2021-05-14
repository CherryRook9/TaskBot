using System;
using System.Linq;
using System.Threading.Tasks;
using TaskBot.Models;
using TaskBot.Services;
using Telegram.Bot.Types.Enums;
using TelegramBotBase.Base;

namespace TaskBot.Forms
{
    class StartForm : FormBase
    {
        private bool botEnabled = false;

        public StartForm()
        {
            DI.Resolve(this);
        }

        [Dependency]
        public TasksContext db { get; set; }
        public override async Task Load(MessageResult message)
        {
            try
            {
                await base.Load(message);
                               
                if (message.MessageType == MessageType.Text
                    && message.MessageText == "/start")
                {
                    var existingUser = db.Users.FirstOrDefault(user => user.DeviceId == message.DeviceId);
                    if (existingUser != null)
                    {
                        existingUser.BotStarted = true;
                        botEnabled = true;
                        db.Users.Update(existingUser);
                    }
                    else
                    {
                        var user = new User()
                        {
                            Login = message.Message.From.Username,
                            DeviceId = message.DeviceId,
                            BotStarted = true
                        };
                        botEnabled = true;
                        await db.Users.AddAsync(user);
                    }
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public override async Task Render(MessageResult message)
        {
            await base.Render(message);
            if (botEnabled)
            {
                await this.Device.Send("Бот запущен.");
                var menuForm = new MenuForm();
                await NavigateTo(menuForm);
            }
        }
    }
}