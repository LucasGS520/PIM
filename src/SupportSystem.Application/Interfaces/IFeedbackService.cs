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
        Task<AvaliacaoDto> CreateAsync(CriarAvaliacaoDto dto, CancellationToken cancellationToken);


        // Obtém as avaliações associadas a um chamado.
        Task<IReadOnlyCollection<AvaliacaoDto>> GetByTicketAsync(Guid ticketId, CancellationToken cancellationToken);
    }
}
