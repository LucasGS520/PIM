using SupportSystem.Domain.Enums;

namespace SupportSystem.Domain.Entities;

/// <summary>
/// Representa uma notificação enviada para usuários do sistema.
/// </summary>
public class Notification : BaseEntity
{
    /// <summary>
    /// Identificador do usuário destinatário.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Entidade navegacional do usuário destinatário.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Conteúdo textual da mensagem entregue.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de notificação utilizada na entrega.
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Indicador se a notificação foi lida pelo usuário.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Identificador opcional do chamado relacionado.
    /// </summary>
    public Guid? TicketId { get; set; }

    /// <summary>
    /// Entidade navegacional do chamado associado.
    /// </summary>
    public Ticket? Ticket { get; set; }
}
