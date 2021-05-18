using System;
using System.Threading.Tasks;
using TaskBot.Dialogs;
using TaskBot.Services;
using TelegramBotBase.Base;
using TelegramBotBase.Controls;

namespace TaskBot.Forms
{
    class TaskCreationForm : FormBase
    {
        [Dependency]
        public TasksContext db { get; set; }

        string taskTitle = null;
        string taskDescription = null;
        DateTime? date = null;
        TaskBot.Models.Priority? priority = null;

        public override async Task Load(MessageResult message)
        {
            await base.Load(message);
            if (!string.IsNullOrEmpty(message.MessageText))
            {
                if (taskTitle == null && taskDescription == null)
                {
                    taskTitle = message.MessageText;
                }
                else if (taskTitle != null && taskDescription == null)
                {
                    taskDescription = message.MessageText;
                }
                else if (date == null)
                {

                }
                else if (priority == null)
                {

                }
            }
        }

        public override async Task Render(MessageResult message)
        {
            await base.Render(message);

            if (taskTitle == null)
            {
                await Device.Send("Начинаем создание новой задачи.\nВведите название:");
            }
            else if (taskDescription == null)
            {
                await Device.Send("Введите описание:");
            }
            else if (date == null)
            {
                var deadlineSelection = new DateSelectionDialog();
                deadlineSelection.Completed += date =>
                {
                    this.date = date;
                };
                await OpenModal(deadlineSelection);

            }
            else if (priority == null)
            {
                var prioritySelection = new SelectPriorityDialog();
                prioritySelection.Completed += priority =>
                {
                    this.priority = priority;
                };
                await OpenModal(prioritySelection);

            }
            else
            {
                await Device.Send($"Задача создана.\nНазвание: {taskTitle}\nОписание:\n{taskDescription}");
                var id = Guid.NewGuid();



                db.Tasks.Add(new Models.PersonalTask()
                {
                    Id = id,
                    Title = taskTitle,
                    Description = taskDescription,
                    CreatorDeviceId = message.DeviceId,
                    Deadline = date.Value,
                    Priority = priority.Value
                });

                await db.SaveChangesAsync();
                await NavigateTo(DI.Resolve(new AssignTaskForm(id)));

            }
        }
    }
}