using System;
using System.Threading.Tasks;
using TaskBot.Services;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace TaskBot.Forms
{
    class TaskEditForm : FormBase
    {
        enum EditState { Watch, Title, Description }

        EditState currentState = EditState.Watch;

        [Dependency]
        public TasksContext db { get; set; }

        public TaskEditForm(Guid editTaskId)
        {
            this.editTaskId = editTaskId;
        }

        private readonly Guid editTaskId;

        public override async Task Load(MessageResult message)
        {
            await base.Load(message);
            var task = await db.Tasks.FindAsync(editTaskId);
            switch (currentState)
            {
                case EditState.Watch:
                    if (message.GetData<CallbackData>() is CallbackData call)
                    {
                        if (call.Method == "back")
                        {
                            await NavigateTo(new MenuForm());
                        }
                        else if (call.Method == "edit-title")
                        {
                            currentState = EditState.Title;
                        }
                        else if (call.Method == "edit-description")
                        {
                            currentState = EditState.Description;
                        }
                    }
                    return;
                case EditState.Title:
                    var newTitle = message.MessageText;
                    task.Title = newTitle;
                    db.Tasks.Update(task);
                    await db.SaveChangesAsync();
                    currentState = EditState.Watch;
                    return;
                case EditState.Description:
                    var newDescription = message.MessageText;
                    task.Description = newDescription;
                    db.Tasks.Update(task);
                    await db.SaveChangesAsync();
                    currentState = EditState.Watch;
                    return;
                default:
                    return;
            }
        }

        public override async Task Render(MessageResult message)
        {
            var task = await db.Tasks.FindAsync(editTaskId);
            switch (currentState)
            {
                case EditState.Watch:
                    await Device.Send($"{task.Title}\n{task.Description}");
                    var buttons = new ButtonForm();
                    buttons.AddButtonRow("Редактивовать название", new CallbackData("edit-title", "").Serialize());
                    buttons.AddButtonRow("Редактивовать описание", new CallbackData("edit-description", "").Serialize());
                    buttons.AddButtonRow("Звершить редактирование", new CallbackData("back", "").Serialize());
                    await Device.Send("Выберете, что редактировать", buttons);
                    return;
                case EditState.Title:
                    await Device.Send($"Введите новое название");
                    return;
                case EditState.Description:
                    await Device.Send($"Введите новое описание");
                    return;
                default:
                    return;
            }
        }

    }
}