using Microsoft.EntityFrameworkCore;
using SupportSystem.Domain.Entities;

namespace SupportSystem.Infrastructure.Data;

/// Contexto de banco de dados responsável por mapear as entidades para o MS SQL Server.
public class SupportSystemDbContext : DbContext
{
    /// Constrói o contexto utilizando as opções configuradas pela aplicação.
    public SupportSystemDbContext(DbContextOptions<SupportSystemDbContext> options) : base(options)
    {
    }

    /// Conjunto de usuários cadastrados.
    public DbSet<User> Users => Set<User>();

    /// Conjunto de chamados registrados.
    public DbSet<Ticket> Tickets => Set<Ticket>();

    /// Conjunto de históricos de chamados.
    public DbSet<TicketHistory> TicketHistories => Set<TicketHistory>();

    /// Conjunto de anexos de chamados.
    public DbSet<TicketAttachment> TicketAttachments => Set<TicketAttachment>();

    /// Conjunto de sugestões de conhecimento geradas pela IA.
    public DbSet<TicketKnowledgeBaseSuggestion> TicketKnowledgeBaseSuggestions => Set<TicketKnowledgeBaseSuggestion>();

    /// Conjunto de artigos da base de conhecimento.
    public DbSet<KnowledgeBaseArticle> KnowledgeBaseArticles => Set<KnowledgeBaseArticle>();

    /// Conjunto de avaliações de atendimento.
    public DbSet<Feedback> Feedbacks => Set<Feedback>();

    /// Conjunto de notificações enviadas pela plataforma.
    public DbSet<Notification> Notifications => Set<Notification>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Utilizamos separação de configuração para manter o método organizado.
        ConfigureUser(modelBuilder);
        ConfigureTicket(modelBuilder);
        ConfigureKnowledgeBase(modelBuilder);
        ConfigureFeedback(modelBuilder);
        ConfigureNotification(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FullName).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(256).IsRequired();
        });
    }

    private static void ConfigureTicket(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(120);
            entity.HasOne(e => e.Requester)
                .WithMany(u => u.RequestedTickets)
                .HasForeignKey(e => e.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Assignee)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(e => e.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TicketHistory>(entity =>
        {
            entity.Property(e => e.Message).HasMaxLength(2000).IsRequired();
        });

        modelBuilder.Entity<TicketAttachment>(entity =>
        {
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.ContentType).HasMaxLength(200);
        });

        modelBuilder.Entity<TicketKnowledgeBaseSuggestion>(entity =>
        {
            entity.HasOne(s => s.Ticket)
                .WithMany(t => t.KnowledgeBaseSuggestions)
                .HasForeignKey(s => s.TicketId);
            entity.HasOne(s => s.KnowledgeBaseArticle)
                .WithMany(a => a.TicketSuggestions)
                .HasForeignKey(s => s.KnowledgeBaseArticleId);
        });
    }

    private static void ConfigureKnowledgeBase(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KnowledgeBaseArticle>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(120).IsRequired();
        });
    }

    private static void ConfigureFeedback(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.Property(e => e.Comment).HasMaxLength(1000);
        });
    }

    private static void ConfigureNotification(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(e => e.Message).HasMaxLength(500).IsRequired();
        });
    }
}
