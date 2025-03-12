using DetectiveMysteryGamePlatform.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DetectiveMysteryGamePlatform.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Quest> Quests { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<PlayerSession> PlayerSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Quest>()
                .HasMany<Character>()
                .WithOne()
                .HasForeignKey(c => c.QuestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Quest>()
                .HasMany<Round>()
                .WithOne()
                .HasForeignKey(r => r.QuestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Quest>()
                .HasMany<Content>()
                .WithOne()
                .HasForeignKey(c => c.QuestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Round>()
                .HasMany<Content>()
                .WithOne()
                .HasForeignKey(c => c.RoundId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Character>()
                .HasMany<Content>()
                .WithOne()
                .HasForeignKey(c => c.CharacterId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GameSession>()
                .HasMany<PlayerSession>()
                .WithOne()
                .HasForeignKey(ps => ps.GameSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 