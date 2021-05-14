using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskBot.Models;
using TaskBot.Services;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace TaskBot.Dialogs
{
    class SelectPriorityDialog : ModalDialog
    {
        public event Action<Models.Priority> Completed;

        public override async Task Action(MessageResult message)
        {
            await base.Load(message);
            if (message.GetData<CallbackData>() is CallbackData call)
            {
                if (call.Method == "normal")
                {
                    Completed(Models.Priority.Normal);
                    await this.CloseForm();
                }
                else if (call.Method == "important")
                {
                    Completed(Models.Priority.Important);
                    await this.CloseForm();
                }
                else if (call.Method == "unimportant")
                {
                    Completed(Models.Priority.Unimportant);
                    await this.CloseForm();
                }
            }
        }
        public override async Task Render(MessageResult message)
        {
            await base.Render(message);
            // var task = await db.Tasks.FindAsync(editTaskId);
            var taskButtons = new ButtonForm();
            taskButtons.AddButtonRow(
                    new ButtonBase("Обычно", new CallbackData("normal", "").Serialize()),
                    new ButtonBase("Важно", new CallbackData("important", "").Serialize()),
                    new ButtonBase("Неважно", new CallbackData("unimportant", "").Serialize()));

            taskButtons.AddButtonRow("Звершить редактирование", new CallbackData("back", "").Serialize());
            await Device.Send($"Выберете приоритет:", taskButtons);
        }
    }
}