using Microsoft.EntityFrameworkCore;
using TikTakServer.Models;

namespace TikTakServer.Database
{
    public class TikTakContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<UserTagInteraction> UserTagsInteractions { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Video> Videos { get; set; }

        public TikTakContext(DbContextOptions<TikTakContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(e => e.Likes)
                .WithOne(x => x.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Video>()
                .HasMany(e => e.Likes)
                .WithOne(x => x.Video)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
