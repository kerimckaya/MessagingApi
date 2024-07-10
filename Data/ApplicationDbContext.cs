using MessagingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MessagingApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) {
        
        }

        public DbSet<User> User{ get; set; }
        public DbSet<Messages> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(x => x.PhoneNumber).IsUnique();
            base.OnModelCreating(modelBuilder);
        }

    }
}
