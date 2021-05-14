using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskBot.Dialogs;
using TaskBot.Services;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace TaskBot.Forms
{
    class TaskEditForm : FormBase
    {
        enum EditState { Watch, Title, Description, Responsible, Deadline, Priority }

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
            var task = await db.Tasks.FirstAsync(x => x.Id == editTaskId);
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
                        else if (call.Method == "responsible")
                        {
                            currentState = EditState.Responsible;
                        }
                        else if (call.Method == "deadline")
                        {
                            var deadlineSelection = new DateSelectionDialog();
                            deadlineSelection.Completed += date => {
                                task.Deadline = date;
                                db.Tasks.Update(task);
                                db.SaveChanges();
                            };
                            await OpenModal(deadlineSelection);
                        }
                        else if (call.Method == "priority")
                        {
                            var prioritySelection = new SelectPriorityDialog();
                            prioritySelection.Completed += priority => {
                                task.Priority = priority;
                                db.Tasks.Update(task);
                                db.SaveChanges();
                            };
                            await OpenModal(prioritySelection);
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
                case EditState.Deadline:
                    currentState = EditState.Watch;
                    return;
                case EditState.Priority:
                    currentState = EditState.Watch;
                    return;
                default:
                    return;
            }
        }

        public override async Task Render(MessageResult message)
        {
            var task = await db.Tasks.Include(x => x.Responsible).FirstAsync(x => x.Id == editTaskId);
            switch (currentState)
            {
                case EditState.Watch:
                    await Device.Send(
                        $"{task.Title}\n{task.Description}\n{task.Responsible.Login}\n" +
                        $"Приоритет: {task.Priority}\nДедлайн: {task.Deadline}");
                    var buttons = new ButtonForm();

                    buttons.AddButtonRow(
                        new ButtonBase("Название", new CallbackData("edit-title", "").Serialize()),
                        new ButtonBase("Описание", new CallbackData("edit-description", "").Serialize()),
                        new ButtonBase("Ответственный", new CallbackData("responsible", "").Serialize()));

                    buttons.AddButtonRow(
                        new ButtonBase("Дата", new CallbackData("deadline", "").Serialize()),
                        new ButtonBase("Приоритет", new CallbackData("priority", "").Serialize()));

                    buttons.AddButtonRow("Звершить редактирование", new CallbackData("back", "").Serialize());
                    await Device.Send("Выберете, что редактировать", buttons);
                    return;
                case EditState.Title:
                    await Device.Send($"Введите новое название");
                    return;
                case EditState.Description:
                    await Device.Send($"Введите новое описание");
                    return;
                case EditState.Responsible:
                    //await Device.Send($"Введите другого ответственного");
                    await NavigateTo(DI.Resolve(new AssignTaskForm(this, editTaskId)));
                    currentState = EditState.Watch;
                    return;
                case EditState.Deadline:
                    return;
                case EditState.Priority:
                    return;
                default:
                    return;
            }
        }

    }
}