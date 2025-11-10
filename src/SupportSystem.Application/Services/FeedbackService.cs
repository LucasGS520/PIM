using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;

namespace SupportSystem.Application.Services;

// Serviço responsável pela criação e consulta de avaliações de atendimento (feedback).
public class FeedbackService : IFeedbackService
{
    private readonly IRepository<Feedback> _repository;
    private readonly IUnitOfWork _unitOfWork;

    // Inicializa o serviço com as dependências necessárias (repositório e unidade de trabalho).
    public FeedbackService(IRepository<Feedback> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    // Cria um novo feedback a partir do DTO fornecido e persiste no repositório.
    // Retorna o DTO com os dados salvo.
    public async Task<FeedbackDto> CreateAsync(CreateFeedbackDto dto, CancellationToken cancellationToken)
    {
        // Mapeia o DTO de entrada para a entidade de domínio.
        var feedback = new Feedback
        {
            TicketId = dto.ChamadoId, // Id do chamado associado ao feedback.
            UserId = dto.UsuarioId, // Id do usuário que está avaliando.
            Score = dto.Nota, // Nota dada pelo usuário.
            Comment = dto.Comentario // Comentário opcional.
        };

        // Adiciona a entidade ao repositório (não salva ainda).
        await _repository.AddAsync(feedback, cancellationToken);

        // Persiste todas as alterações pendentes usando a Unit of Work.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Retorna um DTO representando o feedback recém-criado.
        return new FeedbackDto(feedback.Id, feedback.TicketId, feedback.UserId, feedback.Score, feedback.Comment, feedback.CreatedAt);
    }

    // Recupera todos os feedbacks associados a um chamado (ticket), ordenados por data de criação descendente.

    public async Task<IReadOnlyCollection<FeedbackDto>> GetByTicketAsync(Guid ticketId, CancellationToken cancellationToken)
    {
        // Consulta o repositório com AsNoTracking para melhorar performance de leitura.
        var feedbacks = await _repository.Query() 
            .AsNoTracking()
            .Where(f => f.TicketId == ticketId) // Filtra pelo Id do chamado.
            .OrderByDescending(f => f.CreatedAt) // Ordena do mais recente para o mais antigo.
            .ToListAsync(cancellationToken); // Executa a consulta e obtém a lista de feedbacks.

        // Projeta as entidades para DTOs de saída e retorna como lista somente leitura.
        return feedbacks
            .Select(f => new FeedbackDto(f.Id, f.TicketId, f.UserId, f.Score, f.Comment, f.CreatedAt))
            .ToList();
    }
}
