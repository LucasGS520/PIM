using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;

namespace SupportSystem.Application.Services;

/// <summary>
/// Implementa a captura e consulta de avaliações de atendimento.
/// </summary>
public class FeedbackService : IFeedbackService
{
    private readonly IRepository<Feedback> _repository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Inicializa o serviço com suas dependências.
    /// </summary>
    public FeedbackService(IRepository<Feedback> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<FeedbackDto> CreateAsync(CreateFeedbackDto dto, CancellationToken cancellationToken)
    {
        var feedback = new Feedback
        {
            TicketId = dto.ChamadoId,
            UserId = dto.UsuarioId,
            Score = dto.Nota,
            Comment = dto.Comentario
        };

        await _repository.AddAsync(feedback, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new FeedbackDto(feedback.Id, feedback.TicketId, feedback.UserId, feedback.Score, feedback.Comment, feedback.CreatedAt);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<FeedbackDto>> GetByTicketAsync(Guid ticketId, CancellationToken cancellationToken)
    {
        var feedbacks = await _repository.Query()
            .AsNoTracking()
            .Where(f => f.TicketId == ticketId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);

        return feedbacks
            .Select(f => new FeedbackDto(f.Id, f.TicketId, f.UserId, f.Score, f.Comment, f.CreatedAt))
            .ToList();
    }
}
