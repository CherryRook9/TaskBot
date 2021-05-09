using System;
using System.Threading.Tasks;
using TaskBot.Services;
using TelegramBotBase.Base;

namespace TaskBot.Forms
{
    class TaskDeleteForm : FormBase 
    {
        [Dependency] 
        public TasksContext db { get; set; }

       
    }
}