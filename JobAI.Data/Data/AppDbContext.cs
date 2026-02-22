using JobAI.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace JobAI.Data.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<RemoteJob> RemoteJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RemoteJob>()
                .HasIndex(j => j.ExternalId)
                .IsUnique();
        }
    }
}
