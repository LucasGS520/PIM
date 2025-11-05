using SupportSystem.Domain.Enums;

namespace SupportSystem.Application.DTOs;

/// <summary>
/// Representa uma notificação comunicada a um usuário.
/// </summary>
public record NotificationDto(
    Guid Id,
    Guid UsuarioId,
    string Mensagem,
    NotificationType Tipo,
    bool Lido,
    DateTime CriadoEm
);

/// <summary>
/// DTO utilizado para envio manual de notificações.
/// </summary>
public record CreateNotificationDto(
    Guid UsuarioId,
    string Mensagem,
    NotificationType Tipo,
    Guid? ChamadoId
);
