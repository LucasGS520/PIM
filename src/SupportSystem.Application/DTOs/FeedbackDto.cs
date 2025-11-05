namespace SupportSystem.Application.DTOs;

/// <summary>
/// Representa o feedback emitido por um cliente após o atendimento.
/// </summary>
public record FeedbackDto(
    Guid Id,
    Guid ChamadoId,
    Guid UsuarioId,
    int Nota,
    string? Comentario,
    DateTime CriadoEm
);

/// <summary>
/// DTO utilizado para registrar uma avaliação de atendimento.
/// </summary>
public record CreateFeedbackDto(
    Guid ChamadoId,
    Guid UsuarioId,
    int Nota,
    string? Comentario
);
