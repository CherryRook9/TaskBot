using System;

namespace TaskBot.Models
{
    public class PersonalTask
    {
        public Guid Id { get; set; }
        
        public long CreatorDeviceId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public long? ResponsibleDeviceId { get; set; }

        public User Responsible { get; set; }
    }
}