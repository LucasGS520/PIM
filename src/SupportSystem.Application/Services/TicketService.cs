using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.AI;
using SupportSystem.Application.Common;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;
using SupportSystem.Domain.Enums;

namespace SupportSystem.Application.Services;

// Implementa as regras de negócio ligadas ao ciclo de vida de chamados.
// Serviço responsável por criar, atualizar, reabrir e consultar chamados, além de registrar histórico, anexos e sugestões da base de conhecimento fornecidas pela IA.
public class TicketService : ITicketService
{
    // Constante que define por quantas horas o solicitante pode reabrir um chamado após fechamento.
    private const int HorasReabertura = 48;

    // Repositórios para persistência das diferentes entidades envolvidas.
    private readonly IRepository<Ticket> _tickets;
    private readonly IRepository<User> _users;
    private readonly IRepository<TicketHistory> _history;
    private readonly IRepository<TicketAttachment> _attachments;
    private readonly IRepository<TicketKnowledgeBaseSuggestion> _suggestions;

    // Cliente de assistente/IA usado para análise automática do chamado.
    private readonly IAssistantClient _assistantClient;

    // Dispatcher responsável por enviar notificações sobre atualizações de chamados.
    private readonly INotificationDispatcher _notificationDispatcher;

    // Unidade de trabalho para salvar alterações em múltiplos repositórios de forma atômica.
    private readonly IUnitOfWork _unitOfWork;

    // Inicializa o serviço com seus repositórios e clientes auxiliares.
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

    // Obtém a lista paginada de chamados.
    public async Task<PagedResult<TicketDto>> GetAsync(int page, int pageSize, TicketStatus? status, Guid? requesterId, Guid? assigneeId, CancellationToken cancellationToken)
    {
        // Constrói a query base aplicando filtros opcionais.
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

        // Obtém total para paginação e lista de itens paginada.
        var total = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(MapToDto).ToList();
        return new PagedResult<TicketDto>(dtos, total, page, pageSize);
    }

    // Obtém um chamado por Id.
    public async Task<TicketDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Busca um chamado por id sem rastreamento para leitura.
        var entity = await _tickets.Query().AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    // Cria um novo chamado.
    public async Task<TicketDto> CreateAsync(CreateTicketDto dto, CancellationToken cancellationToken)
    {
        // Valida existência do solicitante.
        var requester = await _users.Query().AsNoTracking().FirstOrDefaultAsync(u => u.Id == dto.SolicitanteId, cancellationToken);
        if (requester is null)
        {
            throw new InvalidOperationException("Solicitante não encontrado para abertura do chamado.");
        }

        // Chama a IA/assistente para analisar título/descrição e sugerir categoria/prioridade/artigos.
        var analysis = await _assistantClient.AnalyzeTicketAsync(dto.Titulo, dto.Descricao, dto.PalavrasChave, cancellationToken);
        var priority = ParsePriority(analysis.PrioridadeSugerida);
        var category = string.IsNullOrWhiteSpace(dto.Categoria) ? analysis.CategoriaSugerida : dto.Categoria!;

        // Criação da entidade Ticket com dados iniciais.
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

        // Registramos sugestões retornadas pela IA para consultas futuras (mapeamento de artigo sugerido).
        foreach (var articleId in analysis.ArtigosRelacionados)
        {
            var suggestion = new TicketKnowledgeBaseSuggestion
            {
                TicketId = ticket.Id,
                KnowledgeBaseArticleId = articleId,
                // Score inicial padrão; pode ser ajustado conforme regras de negócio.
                Score = 0.8
            };

            await _suggestions.AddAsync(suggestion, cancellationToken);
        }

        // Registro inicial de histórico indicando que o chamado foi aberto.
        var historyEntry = new TicketHistory
        {
            TicketId = ticket.Id,
            AuthorId = requester.Id,
            Message = "Chamado aberto pelo solicitante.",
            StatusSnapshot = ticket.Status.ToString()
        };

        await _history.AddAsync(historyEntry, cancellationToken);

        // Persiste todas as alterações (ticket, anexos, sugestões, histórico).
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notifica partes interessadas sobre o novo chamado.
        await _notificationDispatcher.DispatchTicketUpdateAsync(ticket, "Chamado aberto e aguardando atendimento.", cancellationToken);
        return MapToDto(ticket);
    }

    // Atualiza um chamado existente.
    public async Task<TicketDto?> UpdateAsync(Guid id, UpdateTicketDto dto, CancellationToken cancellationToken)
    {
        // Busca o ticket para atualização (rastreado).
        var ticket = await _tickets.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return null;
        }

        // Atualiza campos opcionais quando fornecidos.
        if (dto.Descricao is not null)
        {
            ticket.Description = dto.Descricao;
        }

        if (dto.Status.HasValue)
        {
            ticket.Status = dto.Status.Value;
            if (ticket.Status is TicketStatus.Resolvido or TicketStatus.Fechado)
            {
                // Marca data de fechamento quando o ticket for resolvido/fechado.
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

        // Adiciona novos anexos fornecidos na atualização.
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

        // Registro de histórico para a ação de atualização (autor pode ser técnico, responsável ou solicitante).
        var historyEntry = new TicketHistory
        {
            TicketId = ticket.Id,
            AuthorId = dto.TecnicoId ?? ticket.AssigneeId ?? ticket.RequesterId,
            Message = dto.Mensagem ?? "Chamado atualizado.",
            StatusSnapshot = ticket.Status.ToString()
        };

        await _history.AddAsync(historyEntry, cancellationToken);

        // Marca entidade como modificada e persiste alterações.
        _tickets.Update(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notifica sobre a atualização com a mensagem fornecida ou padrão.
        await _notificationDispatcher.DispatchTicketUpdateAsync(ticket, dto.Mensagem ?? "Chamado atualizado.", cancellationToken);
        return MapToDto(ticket);
    }

    // Reabre um chamado fechado, se dentro do prazo permitido.
    public async Task<TicketDto?> ReopenAsync(Guid id, Guid requesterId, CancellationToken cancellationToken)
    {
        // Obtém o ticket para reabertura.
        var ticket = await _tickets.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return null;
        }

        // Verifica restrição de prazo de reabertura.
        if (ticket.ReopenAllowedUntil is null || ticket.ReopenAllowedUntil < DateTime.UtcNow)
        {
            throw new InvalidOperationException("O prazo de reabertura do chamado expirou.");
        }

        // Apenas solicitante original pode reabrir.
        if (ticket.RequesterId != requesterId)
        {
            throw new InvalidOperationException("Somente o solicitante original pode reabrir o chamado.");
        }

        // Atualiza status para reaberto e registra histórico.
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

        // Notifica sobre a reabertura.
        await _notificationDispatcher.DispatchTicketUpdateAsync(ticket, "Chamado reaberto para nova análise.", cancellationToken);
        return MapToDto(ticket);
    }

    // Obtém o histórico de um chamado.
    public async Task<IReadOnlyCollection<TicketHistoryDto>> GetHistoryAsync(Guid id, CancellationToken cancellationToken)
    {
        // Recupera histórico ordenado por data de criação (mais recente primeiro).
        var entries = await _history.Query()
            .AsNoTracking()
            .Where(h => h.TicketId == id)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync(cancellationToken);

        return entries
            .Select(h => new TicketHistoryDto(h.Id, h.AuthorId, h.Message, h.CreatedAt, h.StatusSnapshot))
            .ToList();
    }

    // Mapeia entidade Ticket para DTO usado pela aplicação (reduzindo campos expostos).
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

    // Converte string de prioridade sugerida pela IA para o enum interno.
    private static TicketPriority ParsePriority(string value) => value?.ToLowerInvariant() switch
    {
        "baixa" => TicketPriority.Baixa,
        "alta" => TicketPriority.Alta,
        "crítica" => TicketPriority.Critica,
        "critica" => TicketPriority.Critica,
        _ => TicketPriority.Media
    };
}
