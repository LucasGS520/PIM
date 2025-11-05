using SupportSystem.Application.DTOs;

namespace SupportSystem.Application.Interfaces;

/// <summary>
/// Define operações relacionadas às avaliações dos atendimentos.
/// </summary>
public interface IFeedbackService
{
    /// <summary>
    /// Registra uma avaliação de atendimento realizada pelo cliente.
    /// </summary>
    Task<FeedbackDto> CreateAsync(CreateFeedbackDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém os feedbacks associados a um chamado.
    /// </summary>
    Task<IReadOnlyCollection<FeedbackDto>> GetByTicketAsync(Guid ticketId, CancellationToken cancellationToken);
}
