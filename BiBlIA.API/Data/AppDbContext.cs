using BíblIA.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BíblIA.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<BibleVerse> BibleVerses { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Chats)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // BibleVerse configuration
        modelBuilder.Entity<BibleVerse>()
            .HasKey(b => b.Id);
        
        modelBuilder.Entity<BibleVerse>()
            .HasIndex(b => new { b.Book, b.Chapter, b.Verse })
            .IsUnique();

        // Chat configuration
        modelBuilder.Entity<Chat>()
            .HasKey(c => c.Id);
        
        modelBuilder.Entity<Chat>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // ChatMessage configuration
        modelBuilder.Entity<ChatMessage>()
            .HasKey(m => m.Id);
    }
}