using Microsoft.AspNetCore.Mvc;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Enums;

namespace SupportSystem.Api.Controllers;

/// <summary>
/// Controlador responsável pela gestão do ciclo de vida dos chamados.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly IKnowledgeBaseService _knowledgeBaseService;

    /// <summary>
    /// Construtor com injeção dos serviços necessários.
    /// </summary>
    public TicketsController(ITicketService ticketService, IKnowledgeBaseService knowledgeBaseService)
    {
        _ticketService = ticketService;
        _knowledgeBaseService = knowledgeBaseService;
    }

    /// <summary>
    /// Lista chamados com filtros opcionais de status e responsáveis.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] TicketStatus? status = null,
        [FromQuery] Guid? requesterId = null,
        [FromQuery] Guid? technicianId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _ticketService.GetAsync(page, pageSize, status, requesterId, technicianId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém detalhes de um chamado específico.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return NotFound();
        }

        return Ok(ticket);
    }

    /// <summary>
    /// Cria um chamado e retorna as sugestões de conhecimento relacionadas.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] CreateTicketDto dto, CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.CreateAsync(dto, cancellationToken);

        var suggestions = await _knowledgeBaseService.SuggestAsync(dto.Descricao, 3, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = ticket.Id }, new { ticket, suggestions });
    }

    /// <summary>
    /// Atualiza informações do chamado, registrando histórico.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateTicketDto dto, CancellationToken cancellationToken)
    {
        var updated = await _ticketService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    /// <summary>
    /// Reabre um chamado dentro do prazo estabelecido.
    /// </summary>
    [HttpPost("{id:guid}/reopen")]
    public async Task<ActionResult> ReopenAsync(Guid id, [FromQuery] Guid requesterId, CancellationToken cancellationToken)
    {
        var reopened = await _ticketService.ReopenAsync(id, requesterId, cancellationToken);
        if (reopened is null)
        {
            return NotFound();
        }

        return Ok(reopened);
    }

    /// <summary>
    /// Recupera o histórico de interações do chamado.
    /// </summary>
    [HttpGet("{id:guid}/history")]
    public async Task<ActionResult> GetHistoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var history = await _ticketService.GetHistoryAsync(id, cancellationToken);
        return Ok(history);
    }
}
