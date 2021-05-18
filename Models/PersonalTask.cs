using System;

namespace TaskBot.Models
{
    public enum Priority { Hight, Medium, Low }
    public class PersonalTask
    {
        public Guid Id { get; set; }
        
        public long CreatorDeviceId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public long? ResponsibleDeviceId { get; set; }

        public DateTime Deadline { get; set; }

        public Priority Priority { get; set; }

        public User Responsible { get; set; }
    }
}