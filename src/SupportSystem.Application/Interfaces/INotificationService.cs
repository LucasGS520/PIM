using SupportSystem.Application.DTOs;

namespace SupportSystem.Application.Interfaces;

/// <summary>
/// Define operações para o envio e leitura de notificações.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Envia uma nova notificação manualmente.
    /// </summary>
    Task<NotificationDto> SendAsync(CreateNotificationDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém notificações pendentes de um usuário.
    /// </summary>
    Task<IReadOnlyCollection<NotificationDto>> GetByUserAsync(Guid userId, bool includeRead, CancellationToken cancellationToken);

    /// <summary>
    /// Marca uma notificação como lida.
    /// </summary>
    Task<bool> MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken);
}
