using Microsoft.EntityFrameworkCore;
using TaskManagerModels;

namespace TaskManagerModels
{

    public class TaskManagerContext : DbContext
    {
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Group> Groups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<PostUser> PostUsers { get; set; }
        public TaskManagerContext(DbContextOptions<TaskManagerContext> options)
            : base(options)
        {

            //Database.EnsureDeleted();
            Database.EnsureCreated();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(x => x.Role)
                .HasDefaultValue(RoleType.User);
        }
    }
}