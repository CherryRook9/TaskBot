using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskBot.Services;
using TelegramBotBase.Base;

namespace TaskBot.Forms
{
    class TaskListForm : FormBase
    {
        [Dependency] 
        public TasksContext db { get; set; }

        public override async Task Render(MessageResult message)
        {
            await base.Render(message);
            
            var tasks = await db.Tasks.Where(x => x.CreatorDeviceId == message.DeviceId).ToListAsync();
            if (tasks.Count == 0)
            {
                await Device.Send("Вы не имеете созданны или назначенных задач.");
            }
            else
            {
                foreach (var task in tasks)
                {
                    await Device.Send($"Задача\n\nНазвание:{task.Title}\nОписание:\n{task.Description}");
                }
            }
            
            await NavigateTo(new MenuForm());
        }
    }
}