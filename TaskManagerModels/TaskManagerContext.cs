﻿using Microsoft.EntityFrameworkCore;
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
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Server=tcp:taskmanagerdev.database.windows.net,1433;Initial Catalog=TaskManager;Persist Security Info=False;User ID=lipfiggy;Password=12341234Yv;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;");
        //    }
        //
        //}
    }
}