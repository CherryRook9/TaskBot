using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskBot.Services;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace TaskBot.Forms
{
    class TaskListForm : FormBase, IDisposable
    {
        [Dependency]
        public TasksContext db { get; set; }

        public override async Task Action(MessageResult message)
        {

            if (message.GetData<CallbackData>() is CallbackData call)
            {
                if (call.Method == "nav" && call.Value == "back")
                {
                    await NavigateTo(new MenuForm());
                }
                else if (call.Method == "edit")
                {
                    var editTaskId = Guid.Parse(call.Value);
                    await NavigateTo(DI.Resolve(new TaskEditForm(editTaskId)));
                }
                else if (call.Method == "delete")
                {
                    var deleteTaskId = Guid.Parse(call.Value);
                    var task = await db.Tasks.FindAsync(deleteTaskId);
                    db.Tasks.Remove(task);
                    await db.SaveChangesAsync();
                }
                else if (call.Method == "accept")
                {
                    /*var deleteTaskId = Guid.Parse(call.Value);
                    var task = await db.Tasks.FindAsync(deleteTaskId);
                    db.Tasks.Remove(task);
                    await db.SaveChangesAsync();*/
                }
                else if (call.Method == "reject")
                {
                    /*var deleteTaskId = Guid.Parse(call.Value);
                    var task = await db.Tasks.FindAsync(deleteTaskId);
                    db.Tasks.Remove(task);
                    await db.SaveChangesAsync();*/
                }
            }
        }

        public override async Task Render(MessageResult message)
        {
            await base.Render(message);

            var tasks = await db.Tasks
                .Where(x => x.CreatorDeviceId == message.DeviceId || x.ResponsibleDeviceId == message.DeviceId)
                .Include(x => x.Responsible)
                .ToListAsync();
            var buttons = new ButtonForm();
            if (tasks.Count == 0)
            {
                await Device.Send("Вы не имеете созданных или назначенных задач.");
            }
            //созданы мной
            else if (CreatorDeviceId == message.DeviceId)
            {
                await Device.Send("Список созданных или назначенных задач");
                foreach (var task in tasks)
                {
                    var taskButtons = new ButtonForm();
                    taskButtons.AddButtonRow(
                            new ButtonBase("Редактивовать задачу", new CallbackData("edit", task.Id.ToString()).Serialize()),
                            new ButtonBase("Удалить задачу", new CallbackData("delete", task.Id.ToString()).Serialize()));

                    //taskButtons.AddButtonRow("Редактивовать задачу", new CallbackData("edit", task.Id.ToString()).Serialize());
                    //taskButtons.AddButtonRow("Удалить задачу", new CallbackData("delete", task.Id.ToString()).Serialize());
                    await Device.Send($"Задача\n\nНазвание:{task.Title}\nОписание:\n{task.Description}\nОтветственный пользователь:\n{task.Responsible.Login}", taskButtons);
                }
            }
            //созданы для меня
            else if (ResponsibleDeviceId == message.DeviceId)
            {
                await Device.Send("Список созданных или назначенных задач");
                foreach (var task in tasks)
                {
                    var taskButtons = new ButtonForm();
                    taskButtons.AddButtonRow(
                            new ButtonBase("Принять задачу", new CallbackData("accept", task.Id.ToString()).Serialize()),
                            new ButtonBase("Отклонить задачу", new CallbackData("reject", task.Id.ToString()).Serialize()));

                    //taskButtons.AddButtonRow("Редактивовать задачу", new CallbackData("edit", task.Id.ToString()).Serialize());
                    //taskButtons.AddButtonRow("Удалить задачу", new CallbackData("delete", task.Id.ToString()).Serialize());
                    await Device.Send($"Задача\n\nНазвание:{task.Title}\nОписание:\n{task.Description}\nОтветственный пользователь:\n{task.Responsible.Login}", taskButtons);
                }
            }

            /*else 
            {
                await Device.Send("Список созданных или назначенных задач");
                foreach (var task in tasks)
                {
                    var taskButtons = new ButtonForm();
                    taskButtons.AddButtonRow(
                            new ButtonBase("Редактивовать задачу", new CallbackData("edit", task.Id.ToString()).Serialize()),
                            new ButtonBase("Удалить задачу", new CallbackData("delete", task.Id.ToString()).Serialize()));

                    //taskButtons.AddButtonRow("Редактивовать задачу", new CallbackData("edit", task.Id.ToString()).Serialize());
                    //taskButtons.AddButtonRow("Удалить задачу", new CallbackData("delete", task.Id.ToString()).Serialize());
                    await Device.Send($"Задача\n\nНазвание:{task.Title}\nОписание:\n{task.Description}\nОтветственный пользователь:\n{task.Responsible.Login}", taskButtons);
                }
            }*/

            buttons.AddButtonRow("Вернуться в меню.", new CallbackData("nav", "back").Serialize());
            await this.Device.Send("Отображены все задачи", buttons);
        }

        void IDisposable.Dispose()
        {
            logger.Debug($"Disposing {nameof(TaskListForm)}.");
            db.Dispose();
        }
    }
}