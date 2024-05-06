using DemoCleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoCleanArchitecture.Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ToDoItem> ToDoItems { get; set; }
        public virtual DbSet<ForgotPasswordRequest> ForgotPasswordRequests { get; set; }
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email);
            base.OnModelCreating(modelBuilder);
        }
    }
}
