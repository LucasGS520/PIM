using SupportSystem.Domain.Enums;

namespace SupportSystem.Application.DTOs;

/// <summary>
/// Representa um chamado com dados relevantes para as interfaces.
/// </summary>
public record TicketDto(
    Guid Id,
    string Titulo,
    string Descricao,
    string Categoria,
    TicketPriority Prioridade,
    TicketStatus Status,
    Guid SolicitanteId,
    Guid? TecnicoId,
    DateTime CriadoEm,
    DateTime? AtualizadoEm,
    DateTime? Prazo,
    DateTime? EncerradoEm
);

/// <summary>
/// DTO utilizado na criação de chamados.
/// </summary>
public record CreateTicketDto(
    string Titulo,
    string Descricao,
    string? Categoria,
    Guid SolicitanteId,
    IEnumerable<string>? PalavrasChave,
    IEnumerable<string>? Anexos
);

/// <summary>
/// DTO utilizado para atualizações de chamados por técnicos.
/// </summary>
public record UpdateTicketDto(
    string? Descricao,
    TicketStatus? Status,
    TicketPriority? Prioridade,
    Guid? TecnicoId,
    string? Mensagem,
    IEnumerable<string>? Anexos
);

/// <summary>
/// Representa uma entrada do histórico do chamado.
/// </summary>
public record TicketHistoryDto(
    Guid Id,
    Guid AutorId,
    string Mensagem,
    DateTime CriadoEm,
    string StatusCapturado
);
