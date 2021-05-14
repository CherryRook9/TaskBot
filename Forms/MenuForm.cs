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
            else if (call.Method == "nav" && call.Value == "list-assigned")
            {
                await NavigateTo(new TaskListForm(TaskListForm.DisplayMode.Assigned));
            }
            else if (call.Method == "nav" && call.Value == "list-created")
            {
                await NavigateTo(new TaskListForm(TaskListForm.DisplayMode.Created));
            }
        }


        public override async Task Render(MessageResult message)
        {
            await base.Render(message);
            var buttons = new ButtonForm();
             buttons.AddButtonRow(
                new ButtonBase("Создать задачу", new CallbackData("nav", "create").Serialize()),
                new ButtonBase("Созданные задачи", new CallbackData("nav", "list-created").Serialize()),
                new ButtonBase("Назначенные задачи", new CallbackData("nav", "list-assigned").Serialize()));
            await this.Device.Send("Выберите действие", buttons);
        }
    }
}