using SupportSystem.Application.Common;
using SupportSystem.Application.DTOs;
using SupportSystem.Domain.Enums;

namespace SupportSystem.Application.Interfaces;

/// <summary>
/// Define operações para gestão de chamados e fluxo de atendimento.
/// </summary>
public interface ITicketService
{
    /// <summary>
    /// Retorna lista paginada de chamados considerando filtros básicos.
    /// </summary>
    Task<PagedResult<TicketDto>> GetAsync(int page, int pageSize, TicketStatus? status, Guid? requesterId, Guid? assigneeId, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém detalhes de um chamado específico.
    /// </summary>
    Task<TicketDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Abre um chamado utilizando classificação assistida por IA.
    /// </summary>
    Task<TicketDto> CreateAsync(CreateTicketDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza os dados de um chamado e registra o histórico correspondente.
    /// </summary>
    Task<TicketDto?> UpdateAsync(Guid id, UpdateTicketDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Reabre um chamado respeitando as regras de prazo configuradas.
    /// </summary>
    Task<TicketDto?> ReopenAsync(Guid id, Guid requesterId, CancellationToken cancellationToken);

    /// <summary>
    /// Retorna o histórico completo de interações de um chamado.
    /// </summary>
    Task<IReadOnlyCollection<TicketHistoryDto>> GetHistoryAsync(Guid id, CancellationToken cancellationToken);
}
