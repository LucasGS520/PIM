using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupportSystem.Application.DTOs;

namespace SupportSystem.Application.Interfaces
{
    // Interface para operações relacionadas às avaliações (feedback) dos atendimentos.
    // Responsável por criar avaliações e recuperar avaliações associadas a chamados.
    // Implementações devem tratar validação, persistência e mapeamento para FeedbackDto.
    public interface IFeedbackService
    {
        // Registra uma avaliação de atendimento realizada pelo cliente.
        Task<FeedbackDto> CreateAsync(CreateFeedbackDto dto, CancellationToken cancellationToken);


        // Obtém os feedbacks associados a um chamado.
        Task<IReadOnlyCollection<FeedbackDto>> GetByTicketAsync(Guid ticketId, CancellationToken cancellationToken);
    }
}
