using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.AI;
using SupportSystem.Application.Common;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;
using SupportSystem.Domain.Enums;

namespace SupportSystem.Application.Services;

/// <summary>
/// Implementa as regras de negócio ligadas ao ciclo de vida de chamados.
/// </summary>
public class TicketService : ITicketService
{
    private const int HorasReabertura = 48;

    private readonly IRepository<Ticket> _tickets;
    private readonly IRepository<User> _users;
    private readonly IRepository<TicketHistory> _history;
    private readonly IRepository<TicketAttachment> _attachments;
    private readonly IRepository<TicketKnowledgeBaseSuggestion> _suggestions;
    private readonly IAssistantClient _assistantClient;
    private readonly INotificationDispatcher _notificationDispatcher;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Inicializa o serviço com seus repositórios e clientes auxiliares.
    /// </summary>
    public TicketService(
        IRepository<Ticket> tickets,
        IRepository<User> users,
        IRepository<TicketHistory> history,
        IRepository<TicketAttachment> attachments,
        IRepository<TicketKnowledgeBaseSuggestion> suggestions,
        IAssistantClient assistantClient,
        INotificationDispatcher notificationDispatcher,
        IUnitOfWork unitOfWork)
    {
        _tickets = tickets;
        _users = users;
        _history = history;
        _attachments = attachments;
        _suggestions = suggestions;
        _assistantClient = assistantClient;
        _notificationDispatcher = notificationDispatcher;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<PagedResult<TicketDto>> GetAsync(int page, int pageSize, TicketStatus? status, Guid? requesterId, Guid? assigneeId, CancellationToken cancellationToken)
    {
        var query = _tickets.Query().AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status);
        }

        if (requesterId.HasValue)
        {
            query = query.Where(t => t.RequesterId == requesterId);
        }

        if (assigneeId.HasValue)
        {
            query = query.Where(t => t.AssigneeId == assigneeId);
        }

        var total = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(MapToDto).ToList();
        return new PagedResult<TicketDto>(dtos, total, page, pageSize);
    }

    /// <inheritdoc />
    public async Task<TicketDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _tickets.Query().AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<TicketDto> CreateAsync(CreateTicketDto dto, CancellationToken cancellationToken)
    {
        var requester = await _users.Query().AsNoTracking().FirstOrDefaultAsync(u => u.Id == dto.SolicitanteId, cancellationToken);
        if (requester is null)
        {
            throw new InvalidOperationException("Solicitante não encontrado para abertura do chamado.");
        }

        var analysis = await _assistantClient.AnalyzeTicketAsync(dto.Titulo, dto.Descricao, dto.PalavrasChave, cancellationToken);
        var priority = ParsePriority(analysis.PrioridadeSugerida);
        var category = string.IsNullOrWhiteSpace(dto.Categoria) ? analysis.CategoriaSugerida : dto.Categoria!;

        var ticket = new Ticket
        {
            Title = dto.Titulo,
            Description = dto.Descricao,
            Category = category,
            Priority = priority,
            RequesterId = dto.SolicitanteId,
            Status = TicketStatus.Aberto,
            ReopenAllowedUntil = DateTime.UtcNow.AddHours(HorasReabertura)
        };

        await _tickets.AddAsync(ticket, cancellationToken);

        // Armazenamos anexos como metadados, assumindo que o frontend cuidará do upload físico.
        if (dto.Anexos is not null)
        {
            foreach (var fileName in dto.Anexos)
            {
                var attachment = new TicketAttachment
                {
                    TicketId = ticket.Id,
                    FileName = fileName,
                    FilePath = $"/anexos/{ticket.Id}/{fileName}",
                    ContentType = "application/octet-stream"
                };

                await _attachments.AddAsync(attachment, cancellationToken);
            }
        }

        // Registramos sugestões retornadas pela IA para consultas futuras.
        foreach (var articleId in analysis.ArtigosRelacionados)
        {
            var suggestion = new TicketKnowledgeBaseSuggestion
            {
                TicketId = ticket.Id,
                KnowledgeBaseArticleId = articleId,
                Score = 0.8
            };

            await _suggestions.AddAsync(suggestion, cancellationToken);
        }

        var historyEntry = new TicketHistory
        {
            TicketId = ticket.Id,
            AuthorId = requester.Id,
            Message = "Chamado aberto pelo solicitante.",
            StatusSnapshot = ticket.Status.ToString()
        };

        await _history.AddAsync(historyEntry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationDispatcher.DispatchTicketUpdateAsync(ticket, "Chamado aberto e aguardando atendimento.", cancellationToken);
        return MapToDto(ticket);
    }

    /// <inheritdoc />
    public async Task<TicketDto?> UpdateAsync(Guid id, UpdateTicketDto dto, CancellationToken cancellationToken)
    {
        var ticket = await _tickets.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return null;
        }

        if (dto.Descricao is not null)
        {
            ticket.Description = dto.Descricao;
        }

        if (dto.Status.HasValue)
        {
            ticket.Status = dto.Status.Value;
            if (ticket.Status is TicketStatus.Resolvido or TicketStatus.Fechado)
            {
                ticket.ClosedAt = DateTime.UtcNow;
            }
        }

        if (dto.Prioridade.HasValue)
        {
            ticket.Priority = dto.Prioridade.Value;
        }

        if (dto.TecnicoId.HasValue)
        {
            ticket.AssigneeId = dto.TecnicoId;
        }

        ticket.UpdatedAt = DateTime.UtcNow;

        if (dto.Anexos is not null)
        {
            foreach (var fileName in dto.Anexos)
            {
                var attachment = new TicketAttachment
                {
                    TicketId = ticket.Id,
                    FileName = fileName,
                    FilePath = $"/anexos/{ticket.Id}/{fileName}",
                    ContentType = "application/octet-stream"
                };

                await _attachments.AddAsync(attachment, cancellationToken);
            }
        }

        var historyEntry = new TicketHistory
        {
            TicketId = ticket.Id,
            AuthorId = dto.TecnicoId ?? ticket.AssigneeId ?? ticket.RequesterId,
            Message = dto.Mensagem ?? "Chamado atualizado.",
            StatusSnapshot = ticket.Status.ToString()
        };

        await _history.AddAsync(historyEntry, cancellationToken);
        _tickets.Update(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationDispatcher.DispatchTicketUpdateAsync(ticket, dto.Mensagem ?? "Chamado atualizado.", cancellationToken);
        return MapToDto(ticket);
    }

    /// <inheritdoc />
    public async Task<TicketDto?> ReopenAsync(Guid id, Guid requesterId, CancellationToken cancellationToken)
    {
        var ticket = await _tickets.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return null;
        }

        if (ticket.ReopenAllowedUntil is null || ticket.ReopenAllowedUntil < DateTime.UtcNow)
        {
            throw new InvalidOperationException("O prazo de reabertura do chamado expirou.");
        }

        if (ticket.RequesterId != requesterId)
        {
            throw new InvalidOperationException("Somente o solicitante original pode reabrir o chamado.");
        }

        ticket.Status = TicketStatus.Reaberto;
        ticket.UpdatedAt = DateTime.UtcNow;

        var historyEntry = new TicketHistory
        {
            TicketId = ticket.Id,
            AuthorId = requesterId,
            Message = "Chamado reaberto pelo solicitante.",
            StatusSnapshot = ticket.Status.ToString()
        };

        await _history.AddAsync(historyEntry, cancellationToken);
        _tickets.Update(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationDispatcher.DispatchTicketUpdateAsync(ticket, "Chamado reaberto para nova análise.", cancellationToken);
        return MapToDto(ticket);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<TicketHistoryDto>> GetHistoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var entries = await _history.Query()
            .AsNoTracking()
            .Where(h => h.TicketId == id)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync(cancellationToken);

        return entries
            .Select(h => new TicketHistoryDto(h.Id, h.AuthorId, h.Message, h.CreatedAt, h.StatusSnapshot))
            .ToList();
    }

    private static TicketDto MapToDto(Ticket ticket) => new(
        ticket.Id,
        ticket.Title,
        ticket.Description,
        ticket.Category,
        ticket.Priority,
        ticket.Status,
        ticket.RequesterId,
        ticket.AssigneeId,
        ticket.CreatedAt,
        ticket.UpdatedAt,
        ticket.DueDate,
        ticket.ClosedAt
    );

    private static TicketPriority ParsePriority(string value) => value?.ToLowerInvariant() switch
    {
        "baixa" => TicketPriority.Baixa,
        "alta" => TicketPriority.Alta,
        "crítica" => TicketPriority.Critica,
        "critica" => TicketPriority.Critica,
        _ => TicketPriority.Media
    };
}
