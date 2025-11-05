using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;
using SupportSystem.Domain.Enums;

namespace SupportSystem.Application.Services;

/// <summary>
/// Implementa envio e leitura de notificações do sistema.
/// </summary>
public class NotificationService : INotificationService, INotificationDispatcher
{
    private readonly IRepository<Notification> _repository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Inicializa o serviço com acesso ao repositório.
    /// </summary>
    public NotificationService(IRepository<Notification> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<NotificationDto> SendAsync(CreateNotificationDto dto, CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            UserId = dto.UsuarioId,
            Message = dto.Mensagem,
            Type = dto.Tipo,
            TicketId = dto.ChamadoId
        };

        await _repository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new NotificationDto(notification.Id, notification.UserId, notification.Message, notification.Type, notification.IsRead, notification.CreatedAt);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<NotificationDto>> GetByUserAsync(Guid userId, bool includeRead, CancellationToken cancellationToken)
    {
        var query = _repository.Query()
            .AsNoTracking()
            .Where(n => n.UserId == userId);

        if (!includeRead)
        {
            query = query.Where(n => !n.IsRead);
        }

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

        return notifications
            .Select(n => new NotificationDto(n.Id, n.UserId, n.Message, n.Type, n.IsRead, n.CreatedAt))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<bool> MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken)
    {
        var notification = await _repository.GetByIdAsync(notificationId, cancellationToken);
        if (notification is null)
        {
            return false;
        }

        notification.IsRead = true;
        notification.UpdatedAt = DateTime.UtcNow;
        _repository.Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task DispatchTicketUpdateAsync(Ticket ticket, string message, CancellationToken cancellationToken)
    {
        var recipients = new List<Guid> { ticket.RequesterId };
        if (ticket.AssigneeId.HasValue)
        {
            recipients.Add(ticket.AssigneeId.Value);
        }

        // Utilizamos HashSet para prevenir notificações duplicadas no mesmo evento.
        foreach (var userId in recipients.ToHashSet())
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                TicketId = ticket.Id,
                Type = NotificationType.Sistema
            };

            await _repository.AddAsync(notification, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
