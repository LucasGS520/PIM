using SupportSystem.Domain.Entities;

namespace SupportSystem.Application.Interfaces;

/// <summary>
/// Define um mecanismo de envio de notificações automáticas.
/// </summary>
public interface INotificationDispatcher
{
    /// <summary>
    /// Envia mensagem informativa relacionada a um chamado para as partes interessadas.
    /// </summary>
    Task DispatchTicketUpdateAsync(Ticket ticket, string message, CancellationToken cancellationToken);
}
