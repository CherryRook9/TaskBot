using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using TelegramBotBase.Base;

namespace TaskBot.Forms
{
    class StartForm : FormBase
    {
        private bool botEnabled = false;

        public override async Task Load(MessageResult message)
        {
            await base.Load(message);
            if (message.MessageType == MessageType.Text
                && message.MessageText == "/start")
            {
                botEnabled = true;
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