using System.Collections.Generic;

namespace TaskBot.Models
{
    public enum UserKind { Admin, User }
    public class User
    {
        public long DeviceId { get; set; }
        public string Login { get; set; }
        public bool BotStarted { get; set; } 
        public UserKind Kind { get; set; }
        public List<PersonalTask> PersonalTasks { get; set; }
    }
}