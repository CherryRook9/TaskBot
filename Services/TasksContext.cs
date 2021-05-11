using Microsoft.EntityFrameworkCore;
using TaskBot.Models;

namespace TaskBot.Services
{
    class TasksContext : DbContext
    {
        public TasksContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<PersonalTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(
                x => x.HasKey(x => x.DeviceId));
            modelBuilder.Entity<PersonalTask>().HasKey(x => x.Id);
            modelBuilder.Entity<PersonalTask>()
                    .HasOne(x => x.Responsible)
                    .WithMany(t => t.PersonalTasks)
                    .HasForeignKey(x => x.ResponsibleDeviceId);
        }
    }
}