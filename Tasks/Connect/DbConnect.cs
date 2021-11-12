using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tasks.Models;

namespace Tasks.Context
{
    public class DbConnect : IdentityDbContext<Users>
    {
        public DbConnect(DbContextOptions<DbConnect> options) : base(options) { }

        public DbSet<Tags> Tags { get; set; }
        public DbSet<TasksList> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
