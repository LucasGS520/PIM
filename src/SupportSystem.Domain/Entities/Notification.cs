using SupportSystem.Domain.Enums;

namespace SupportSystem.Domain.Entities;

// Representa uma notificação enviada para usuários do sistema.
public class Notification : BaseEntity
{
    // Identificador do usuário destinatário.
    public Guid UserId { get; set; }

    // Entidade navegacional do usuário destinatário.
    public User? User { get; set; }

    // Conteúdo textual da mensagem entregue.
    public string Message { get; set; } = string.Empty;

    // Tipo de notificação utilizada na entrega.
    public NotificationType Type { get; set; }

    // Indicador se a notificação foi lida pelo usuário.
    public bool IsRead { get; set; }


    // Identificador opcional do chamado relacionado.
    public Guid? TicketId { get; set; }

    // Entidade navegacional do chamado associado.
    public Ticket? Ticket { get; set; }
}
