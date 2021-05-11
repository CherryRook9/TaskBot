using System.Collections.Generic;

namespace TaskBot.Models
{
    public class User
    {
        public long DeviceId { get; set; }
        public string Login { get; set; }
        public bool BotStarted { get; set; } 
        public List<PersonalTask> PersonalTasks { get; set; }
    }
}