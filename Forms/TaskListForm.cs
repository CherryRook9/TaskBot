using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskBot.Models;
using TaskBot.Services;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace TaskBot.Forms
{
    class TaskListForm : FormBase, IDisposable
    {
        public enum DisplayMode { Assigned, Created }

        private readonly DisplayMode mode;

        [Dependency]
        public TasksContext db { get; set; }

        public TaskListForm(DisplayMode mode)
        {
            this.mode = mode;
        }

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

            List<PersonalTask> tasks = await GetTaskList(message.DeviceId);

            if (tasks.Count == 0)
            {
                await Device.Send("???? ???? ???????????? ?????????????????? ?????? ?????????????????????? ??????????.");
            }

            await Device.Send("???????????? ??????????????????");

            foreach (var task in tasks)
            {
                ButtonForm taskButtons;
                if (mode == DisplayMode.Created)
                {
                    taskButtons = new ButtonForm();
                    taskButtons.AddButtonRow(
                        new ButtonBase("?????????????????????????? ????????????",
                                       new CallbackData("edit", task.Id.ToString()).Serialize()),
                        new ButtonBase("?????????????? ????????????",
                                       new CallbackData("delete", task.Id.ToString()).Serialize()));
                }
                else
                {
                    taskButtons = null;
                }

                var messageText = 
                    $"????????????\n\n????????????????: *{task.Title}*\n" +
                    $"????????????????:\n{task.Description}\n" +
                    $"?????????????????????????? ????????????????????????: {task.Responsible?.Login ?? "??????"}\n"+
                    $"???????? ??????????????????: {task.Deadline}\n"+
                    $"?????????????????? ????????????: {task.Priority}";

                await Device.Send(
                    messageText,
                    taskButtons,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }

            var buttons = new ButtonForm();
            buttons.AddButtonRow("?????????????????? ?? ????????.", new CallbackData("nav", "back").Serialize());
            await this.Device.Send("???????????????????? ?????? ????????????", buttons);
        }

        private async Task<List<PersonalTask>> GetTaskList(long deviceId)
        {
            List<PersonalTask> tasks;
            if (mode == DisplayMode.Assigned)
            {
                tasks = await db.Tasks
                    .Where(x => x.ResponsibleDeviceId == deviceId)
                    .Include(x => x.Responsible)
                    .ToListAsync();
            }
            else if (mode == DisplayMode.Created)
            {
                tasks = await db.Tasks
                    .Where(x => x.CreatorDeviceId == deviceId)
                    .Include(x => x.Responsible)
                    .ToListAsync();
            }
            else
            {
                throw new Exception($"?????????? ?????????????????????? ???????????? ?????????? ?????????? ???????????????????????? ???????????????? '{mode}'.");
            }

            return tasks;
        }

        void IDisposable.Dispose()
        {
            logger.Debug($"Disposing {nameof(TaskListForm)}.");
            db.Dispose();
        }
    }
}