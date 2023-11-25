using Microsoft.EntityFrameworkCore;
using TikTakServer.Models;

namespace TikTakServer.Database
{
    public class TikTakContext : DbContext
    {
        public DbSet<UserDao> Users { get; set; }
        public DbSet<LikeDao> Likes { get; set; }
        public DbSet<UserTagInteractionDao> UserTagsInteractions { get; set; }
        public DbSet<TagDao> Tags { get; set; }
        public DbSet<VideoDao> Videos { get; set; }
        public DbSet<UserTokenDao> Tokens { get; set; }

        public TikTakContext(DbContextOptions<TikTakContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDao>()
                .HasMany(e => e.Likes)
                .WithOne(x => x.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<VideoDao>()
                .HasMany(e => e.Likes)
                .WithOne(x => x.Video)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserDao>()
                .HasMany(u => u.Tokens)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);
        }
    }
}
