using SupportSystem.Domain.Enums;

namespace SupportSystem.Domain.Entities;

// Representa um chamado de suporte aberto por um usuário.
public class Ticket : BaseEntity
{
    // Título curto para resumir o problema relatado.
    public string Title { get; set; } = string.Empty;

    // Descrição detalhada informada pelo solicitante.
    public string Description { get; set; } = string.Empty;

    // Categoria atribuída ao chamado, seja manualmente ou pela IA.
    public string Category { get; set; } = string.Empty;

    // Prioridade aplicada ao chamado para orientar o SLA.
    public TicketPriority Priority { get; set; } = TicketPriority.Media;

    // Situação atual do chamado dentro do fluxo de atendimento.
    public TicketStatus Status { get; set; } = TicketStatus.Aberto;

    // Prazo limite sugerido pela plataforma para atendimento.
    public DateTime? DueDate { get; set; }

    // Data de encerramento efetiva do chamado.
    public DateTime? ClosedAt { get; set; }

    // Identificador do solicitante responsável pela abertura.
    public Guid RequesterId { get; set; }

    // Entidade navegacional para o solicitante.
    public User? Requester { get; set; }

    // Identificador do técnico responsável pelo atendimento atual.
    public Guid? AssigneeId { get; set; }

    // Entidade navegacional para o técnico designado.
    public User? Assignee { get; set; }

    // Histórico das interações registradas no chamado.
    public ICollection<TicketHistory> History { get; set; } = new List<TicketHistory>();

    // Anexos adicionados ao chamado para evidências.
    public ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();

    // Sugestões de conhecimento vinculadas ao chamado.
    public ICollection<TicketKnowledgeBaseSuggestion> KnowledgeBaseSuggestions { get; set; } = new List<TicketKnowledgeBaseSuggestion>();

    // Registra se o chamado pode ser reaberto e até quando.
    public DateTime? ReopenAllowedUntil { get; set; }
}
