using SupportSystem.Domain.Enums;

namespace SupportSystem.Domain.Entities;

/// <summary>
/// Representa um chamado de suporte aberto por um usuário.
/// </summary>
public class Ticket : BaseEntity
{
    /// <summary>
    /// Título curto para resumir o problema relatado.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada informada pelo solicitante.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Categoria atribuída ao chamado, seja manualmente ou pela IA.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Prioridade aplicada ao chamado para orientar o SLA.
    /// </summary>
    public TicketPriority Priority { get; set; } = TicketPriority.Media;

    /// <summary>
    /// Situação atual do chamado dentro do fluxo de atendimento.
    /// </summary>
    public TicketStatus Status { get; set; } = TicketStatus.Aberto;

    /// <summary>
    /// Prazo limite sugerido pela plataforma para atendimento.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Data de encerramento efetiva do chamado.
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Identificador do solicitante responsável pela abertura.
    /// </summary>
    public Guid RequesterId { get; set; }

    /// <summary>
    /// Entidade navegacional para o solicitante.
    /// </summary>
    public User? Requester { get; set; }

    /// <summary>
    /// Identificador do técnico responsável pelo atendimento atual.
    /// </summary>
    public Guid? AssigneeId { get; set; }

    /// <summary>
    /// Entidade navegacional para o técnico designado.
    /// </summary>
    public User? Assignee { get; set; }

    /// <summary>
    /// Histórico das interações registradas no chamado.
    /// </summary>
    public ICollection<TicketHistory> History { get; set; } = new List<TicketHistory>();

    /// <summary>
    /// Anexos adicionados ao chamado para evidências.
    /// </summary>
    public ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();

    /// <summary>
    /// Sugestões de conhecimento vinculadas ao chamado.
    /// </summary>
    public ICollection<TicketKnowledgeBaseSuggestion> KnowledgeBaseSuggestions { get; set; } = new List<TicketKnowledgeBaseSuggestion>();

    /// <summary>
    /// Registra se o chamado pode ser reaberto e até quando.
    /// </summary>
    public DateTime? ReopenAllowedUntil { get; set; }
}
