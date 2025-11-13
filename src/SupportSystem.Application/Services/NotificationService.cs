using System;
using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;
using SupportSystem.Domain.Enums;

namespace SupportSystem.Application.Services;

// Serviço responsável por envio e leitura de notificações do sistema.
public class NotificationService : INotificationService, INotificationDispatcher
{
    private readonly IRepository<Notification> _repository;
    private readonly IUnitOfWork _unitOfWork;

    // Inicializa o serviço com repositório e unidade de trabalho para persistência.
    public NotificationService(IRepository<Notification> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    // Cria e persiste uma nova notificação baseada no DTO recebido.
    // Retorna a DTO representando a notificação criada.
    public async Task<NotificacaoDto> SendAsync(CriarNotificacaoDto dto, CancellationToken cancellationToken)
    {
        // Cria a entidade de notificação a partir do DTO
        var notification = new Notification
        {
            UserId = dto.UsuarioId, // Destinatário da notificação
            Message = dto.Mensagem, // Conteúdo da notificação
            Type = dto.Tipo, // Tipo da notificação
            TicketId = dto.ChamadoId // Id do chamado associado (opcional)
        };

        // Adiciona ao repositório e persiste em um único commit
        await _repository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Converte entidade para DTO de retorno
        return new NotificacaoDto(notification.Id, notification.UserId, notification.Message, notification.Type, notification.IsRead, notification.CreatedAt);
    }

    // Recupera notificações de um usuário.
    public async Task<IReadOnlyCollection<NotificacaoDto>> GetByUserAsync(Guid userId, bool includeRead, CancellationToken cancellationToken)
    {
        // Query base: apenas notificações do usuário
        var query = _repository.Query()
            .AsNoTracking()
            .Where(n => n.UserId == userId); // Filtra notificações do usuário

        // Filtra apenas não lidas quando solicitado
        if (!includeRead)
        {
            query = query.Where(n => !n.IsRead);
        }

        // Ordena por criação (mais recentre primeiro) e executa a consulta
        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

        // Projeta entidades para DTOs
        return notifications
            .Select(n => new NotificacaoDto(n.Id, n.UserId, n.Message, n.Type, n.IsRead, n.CreatedAt))
            .ToList();
    }

    // Marca uma notificação como lida.
    public async Task<bool> MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken)
    {
        // Recupera a notificação por id
        var notification = await _repository.GetByIdAsync(notificationId, cancellationToken);
        if (notification is null)
        {
            // Notificação inexistente
            return false;
        }

        // Atualiza flags e timestamp de atualização
        notification.IsRead = true;
        notification.UpdatedAt = DateTime.UtcNow;
        _repository.Update(notification);

        // Persiste alteração
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Dispara notificações relacionadas a atualizações de chamados para destinatários relevantes.
    public async Task DispatchTicketUpdateAsync(Ticket ticket, string message, CancellationToken cancellationToken)
    {
        // Lista inicial de destinatários: solicitante e possivelmente o atribuído
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

            // Adiciona cada notificação ao repositório (commit ao final)
            await _repository.AddAsync(notification, cancellationToken);
        }

        // Persiste todas as notificações em um único SaveChanges
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
