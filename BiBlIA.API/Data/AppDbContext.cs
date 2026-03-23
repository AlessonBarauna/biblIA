using BíblIA.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BíblIA.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Auth / Chat (existentes)
    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }

    // Bíblia
    public DbSet<BibleBook> BibleBooks { get; set; }
    public DbSet<BibleVerse> BibleVerses { get; set; }

    // Teologia
    public DbSet<TheologyCourse> TheologyCourses { get; set; }
    public DbSet<TheologyModule> TheologyModules { get; set; }
    public DbSet<TheologyQuiz> TheologyQuizzes { get; set; }

    // História da Igreja
    public DbSet<ChurchHero> ChurchHeroes { get; set; }
    public DbSet<Revival> Revivals { get; set; }

    // Escatologia
    public DbSet<EschatologyView> EschatologyViews { get; set; }

    // Dados do usuário (session-based, sem auth obrigatória)
    public DbSet<UserProgress> UserProgress { get; set; }
    public DbSet<ChatSession> ChatSessions { get; set; }
    public DbSet<BookmarkVerse> BookmarkVerses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User → Chats
        modelBuilder.Entity<User>()
            .HasMany(u => u.Chats)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Chat → Messages
        modelBuilder.Entity<Chat>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // BibleBook → BibleVerses
        modelBuilder.Entity<BibleBook>()
            .HasMany(b => b.Verses)
            .WithOne()
            .HasForeignKey(v => v.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        // Versículo único por livro/capítulo/versículo
        modelBuilder.Entity<BibleVerse>()
            .HasIndex(v => new { v.BookId, v.Chapter, v.Verse })
            .IsUnique();

        // Course → Modules → Quizzes
        modelBuilder.Entity<TheologyCourse>()
            .HasMany(c => c.Modules)
            .WithOne(m => m.Course)
            .HasForeignKey(m => m.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TheologyModule>()
            .HasMany(m => m.Quizzes)
            .WithOne(q => q.Module)
            .HasForeignKey(q => q.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices para busca por session
        modelBuilder.Entity<ChatSession>()
            .HasIndex(s => s.SessionId);

        modelBuilder.Entity<UserProgress>()
            .HasIndex(p => p.SessionId);

        modelBuilder.Entity<BookmarkVerse>()
            .HasIndex(b => b.SessionId);
    }
}
