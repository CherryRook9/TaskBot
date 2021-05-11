using System;
using System.Threading.Tasks;
using TaskBot.Services;
using TelegramBotBase.Base;

namespace TaskBot.Forms
{
    class TaskCreationForm : FormBase
    {
        [Dependency]
        public TasksContext db { get; set; }

        string taskTitle = null;
        string taskDescription = null;

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
            }
        }

        public override async Task Render(MessageResult message)
        {
            await base.Render(message);

            if (taskTitle == null && taskDescription == null)
            {
                await Device.Send("Начинаем создание новой задачи.\nВведите название:");
            }
            else if (taskTitle != null && taskDescription == null)
            {
                await Device.Send("Введите описание:");
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
                    CreatorDeviceId = message.DeviceId
                });

                await db.SaveChangesAsync();
                await NavigateTo(DI.Resolve(new AssignTaskForm(id)));

            }
        }
    }
}