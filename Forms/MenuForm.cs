using System.Threading.Tasks;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace TaskBot.Forms
{
    class MenuForm : FormBase
    {
        public override async Task Action(MessageResult message)
        {
            var call = message.GetData<CallbackData>();
            await message.ConfirmAction();

            if (call.Method == "nav" && call.Value == "create")
            {
                await NavigateTo(new TaskCreationForm());
            }
        }

        
        public override async Task Render(MessageResult message)
        {
            await base.Render(message);
            var buttons = new ButtonForm();
            buttons.AddButtonRow("Создать задачу", new CallbackData("nav", "create").Serialize());
            await this.Device.Send("Выберите действие", buttons);
        }
    }
}