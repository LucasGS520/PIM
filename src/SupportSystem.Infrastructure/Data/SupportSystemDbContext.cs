using Microsoft.EntityFrameworkCore;
using SupportSystem.Domain.Entities;

namespace SupportSystem.Infrastructure.Data;

// Contexto do banco de dados responsável por mapear as entidades para o MS SQL Server.
public class SupportSystemDbContext : DbContext
{
    // Constrói o contexto utilizando as opções configuradas pela aplicação (ex.: connection string).
    public SupportSystemDbContext(DbContextOptions<SupportSystemDbContext> options) : base(options)
    {
    }

    // Conjunto de usuários cadastrados.
    // Usado para consultas e alterações na tabela de usuários.
    public DbSet<User> Users => Set<User>();

    // Conjunto de chamados registrados.
    public DbSet<Ticket> Tickets => Set<Ticket>();

    // Conjunto de históricos de chamados.
    public DbSet<TicketHistory> TicketHistories => Set<TicketHistory>();

    // Conjunto de anexos de chamados.
    public DbSet<TicketAttachment> TicketAttachments => Set<TicketAttachment>();

    // Conjunto de sugestões de conhecimento geradas pela IA para um chamado.
    public DbSet<TicketKnowledgeBaseSuggestion> TicketKnowledgeBaseSuggestions => Set<TicketKnowledgeBaseSuggestion>();

    // Conjunto de artigos da base de conhecimento.
    public DbSet<KnowledgeBaseArticle> KnowledgeBaseArticles => Set<KnowledgeBaseArticle>();

    // Conjunto de avaliações de atendimento.
    public DbSet<Feedback> Feedbacks => Set<Feedback>();

    // Conjunto de notificações enviadas pela plataforma.
    public DbSet<Notification> Notifications => Set<Notification>();

    // Configura os modelos e relacionamentos das entidades.
    // Centraliza as configurações para manter o modelo consistente e previsível.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Separa a configuração por entidade para manter o método limpo e organizado.
        ConfigureUser(modelBuilder);
        ConfigureTicket(modelBuilder);
        ConfigureKnowledgeBase(modelBuilder);
        ConfigureFeedback(modelBuilder);
        ConfigureNotification(modelBuilder);
    }

    // Configurações específicas da entidade User.
    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            // Email único para evitar duplicidade de contas.
            entity.HasIndex(e => e.Email).IsUnique();

            // Restrições de tamanho e obrigatoriedade para campos críticos.
            entity.Property(e => e.FullName).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(256).IsRequired();
        });
    }

    // Configurações específicas das entidades relacionadas a chamados.
    private static void ConfigureTicket(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(entity =>
        {
            // Validações de comprimento e obrigatoriedade.
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(120);

            // Relacionamento de requester (quem abriu o chamado) -> muitos chamados.
            entity.HasOne(e => e.Requester)
                .WithMany(u => u.RequestedTickets)
                .HasForeignKey(e => e.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento de assignee (responsável pelo atendimento) -> muitos chamados.
            entity.HasOne(e => e.Assignee)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(e => e.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TicketHistory>(entity =>
        {
            // Histórico do chamado (mensagens/ações) com limite de tamanho.
            entity.Property(e => e.Message).HasMaxLength(2000).IsRequired();
        });

        modelBuilder.Entity<TicketAttachment>(entity =>
        {
            // Metadados do arquivo (nome, caminho, tipo) com limites para evitar campos muito longos.
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.ContentType).HasMaxLength(200);
        });

        modelBuilder.Entity<TicketKnowledgeBaseSuggestion>(entity =>
        {
            // Sugestões ligadas tanto ao ticket quanto ao artigo sugerido.
            // Relação Many-to-One para Ticket e KnowledgeBaseArticle.
            entity.HasOne(s => s.Ticket)
                .WithMany(t => t.KnowledgeBaseSuggestions)
                .HasForeignKey(s => s.TicketId);

            entity.HasOne(s => s.KnowledgeBaseArticle)
                .WithMany(a => a.TicketSuggestions)
                .HasForeignKey(s => s.KnowledgeBaseArticleId);
        });
    }

    // Configurações da base de conhecimento (artigos).
    private static void ConfigureKnowledgeBase(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KnowledgeBaseArticle>(entity =>
        {
            // Título e categoria obrigatórios para organizar artigos.
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(120).IsRequired();
        });
    }

    // Configurações para feedbacks/avaliações de atendimento.
    private static void ConfigureFeedback(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feedback>(entity =>
        {
            // Comentário opcional de avaliação, com limite de tamanho.
            entity.Property(e => e.Comment).HasMaxLength(1000);
        });
    }

    // Configurações para notificações da plataforma.
    private static void ConfigureNotification(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            // Mensagem obrigatória com limite para evitar textos muito longos.
            entity.Property(e => e.Message).HasMaxLength(500).IsRequired();
        });
    }
}
