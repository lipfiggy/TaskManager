using Microsoft.EntityFrameworkCore;
using TaskManagerModels;

namespace TaskManagerWebApi
{

    public class TaskManagerContext : DbContext
    {
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Group> Groups { get; set; }
        public DbSet<User> Users { get; set; }
        public TaskManagerContext(DbContextOptions<TaskManagerContext> options)
            : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}
