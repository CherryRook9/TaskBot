using System;
using System.Threading.Tasks;
using TaskBot.Services;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace TaskBot.Forms
{
    class AssignTaskForm : FormBase
    {
        [Dependency]
        public TasksContext db { get; set; }

        public AssignTaskForm(Guid editTaskId)
        {
            this.editTaskId = editTaskId;
            this.parentForm = null;
        }

        public AssignTaskForm(FormBase parentForm, Guid editTaskId) : this(editTaskId)
        {
            this.parentForm = parentForm;
        }

        private readonly Guid editTaskId;
        private readonly FormBase parentForm;

         public override async Task Action(MessageResult message)
        {

            if (message.GetData<CallbackData>() is CallbackData call)
            {
                if (call.Method == "nav" && call.Value == "back")
                {
                    await NavigateTo(new MenuForm());
                }
                else if (call.Method == "assign-user")
                {
                    var deviceId = long.Parse(call.Value);
                    var task =  await db.Tasks.FindAsync(editTaskId);
                    task.ResponsibleDeviceId = deviceId;
                    db.Tasks.Update(task);
                    await db.SaveChangesAsync();
                    if (parentForm == null)
                    {
                        await NavigateTo(DI.Resolve(new MenuForm()));
                    }
                    else
                    {
                        await NavigateTo(DI.Resolve(parentForm));
                    }
                }

            }
        }
        public override async Task Render(MessageResult message)
        {
            await base.Render(message);
            var task = await db.Tasks.FindAsync(editTaskId);
            var taskButtons = new ButtonForm();
            
            foreach (var user in db.Users)
            {    
                taskButtons.AddButtonRow(user.Login, new CallbackData("assign-user", user.DeviceId.ToString()).Serialize());  
            }
            await Device.Send($"Выберете пользователя:", taskButtons);
        }
    }
}