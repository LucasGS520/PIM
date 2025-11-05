using Microsoft.AspNetCore.Mvc;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;

namespace SupportSystem.Api.Controllers;

/// <summary>
/// Controlador responsável pelas avaliações dos atendimentos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _service;

    /// <summary>
    /// Construtor com injeção do serviço de feedbacks.
    /// </summary>
    public FeedbackController(IFeedbackService service)
    {
        _service = service;
    }

    /// <summary>
    /// Registra uma nova avaliação realizada por um cliente.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] CreateFeedbackDto dto, CancellationToken cancellationToken)
    {
        var feedback = await _service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetByTicketAsync), new { ticketId = feedback.ChamadoId }, feedback);
    }

    /// <summary>
    /// Lista feedbacks relacionados a um chamado específico.
    /// </summary>
    [HttpGet("ticket/{ticketId:guid}")]
    public async Task<ActionResult> GetByTicketAsync(Guid ticketId, CancellationToken cancellationToken)
    {
        var feedbacks = await _service.GetByTicketAsync(ticketId, cancellationToken);
        return Ok(feedbacks);
    }
}
