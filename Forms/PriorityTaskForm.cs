using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskBot.Models;
using TaskBot.Services;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace TaskBot.Forms
{
    class PriorityTaskForm : FormBase
    {
        // [Dependency]
        // public TasksContext db { get; set; }
        public Task completed;
        private TaskCompletionSource<object> tcs;

        public PriorityTaskForm(PersonalTask task, FormBase parentForm)
        {
            this.task = task;
            this.tcs = new TaskCompletionSource<object>();
            this.completed = tcs.Task;
            this.parentForm = parentForm;
        }

        private readonly FormBase parentForm;
        private readonly PersonalTask task;

        public override async Task Action(MessageResult message)
        {

            await base.Load(message);
            // var task = await db.Tasks.FirstAsync(x => x.Id == editTaskId);
            if (message.GetData<CallbackData>() is CallbackData call)
            {
                if (call.Method == "normal")
                {
                    task.Priority = Models.Priority.Normal;
                }
                else if (call.Method == "important")
                {
                    task.Priority = Models.Priority.Important;
                }
                else if (call.Method == "unimportant")
                {
                    task.Priority = Models.Priority.Unimportant;
                }
                else if (call.Method == "back")
                {
                    await NavigateTo(new MenuForm());
                }
                tcs.SetResult(null);
                await NavigateTo(parentForm);
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