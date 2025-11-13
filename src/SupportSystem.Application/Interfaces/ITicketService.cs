using SupportSystem.Application.Common;
using SupportSystem.Application.DTOs;
using SupportSystem.Domain.Enums;

namespace SupportSystem.Application.Interfaces;

// Serviço para operações relacionadas a chamados (tickets) e fluxo de atendimento.
// Contém métodos para consulta paginada, criação, atualização, reabertura e leitura do histórico.
public interface ITicketService
{
    // Retorna uma lista paginada de chamados aplicando filtros básicos.
    Task<PagedResult<ChamadoDto>> GetAsync(int page, int pageSize, TicketStatus? status, Guid? requesterId, Guid? assigneeId, CancellationToken cancellationToken);

    // Obtém os detalhes de um chamado específico pelo seu identificador.
    Task<ChamadoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    // Cria um novo chamado. A classificação (categoria/prioridade) pode ser assistida por IA.
    Task<ChamadoDto> CreateAsync(CriarChamadoDto dto, CancellationToken cancellationToken);

    // Atualiza os dados de um chamado e registra entrada no histórico (comentários, alterações de status, etc.)
    Task<ChamadoDto?> UpdateAsync(Guid id, AtualizarChamadoDto dto, CancellationToken cancellationToken);

    // Reabre um chamado fechado respeitando regras e prazos configurados no sistema.
    Task<ChamadoDto?> ReopenAsync(Guid id, Guid requesterId, CancellationToken cancellationToken);

    // Retorna o histórico completo de interações e eventos relacionados a um chamado.
    Task<IReadOnlyCollection<HistoricoChamadoDto>> GetHistoryAsync(Guid id, CancellationToken cancellationToken);
}
