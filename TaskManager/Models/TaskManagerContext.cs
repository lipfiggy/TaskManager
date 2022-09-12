using Microsoft.EntityFrameworkCore;

namespace TaskManager.Models
{
    public class TaskManagerContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; } = null!;
        public DbSet<Group> Groups { get; set; }    


        public TaskManagerContext(DbContextOptions<TaskManagerContext> options)
        :base(options)
        {
            Database.EnsureCreated();
        }
    }
}
