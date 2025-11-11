using Microsoft.AspNetCore.Mvc;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;

namespace SupportSystem.Api.Controllers;

// Controlador responsável pelas avaliações (feedbacks) dos atendimentos.
// Fornece endpoints para criação e consulta de feedbacks relacionados a chamados.
[ApiController]
[Route("api/[controller]")]
public class AvaliacoesController : ControllerBase
{
    // Serviço que contém a lógica de negócio para feedbacks (injetado via DI).
    private readonly IFeedbackService _service;

    // Construtor com injeção do serviço de feedbacks.
    public AvaliacoesController(IFeedbackService service)
    {
        _service = service;
    }

    // Registra uma nova avaliação realizada por um cliente.
    // Recebe um DTO com os dados do feedback e persiste via serviço.
    [HttpPost]
    public async Task<ActionResult> CriarAsync([FromBody] CreateFeedbackDto dto, CancellationToken cancellationToken)
    {
        // Chama o serviço para criar o feedback e aguarda resultado.
        var feedback = await _service.CreateAsync(dto, cancellationToken);

        // Retorna 201 Created apontando para o endpoint que consulta feedbacks por chamado.
        // O nome do action e a rota devem corresponder ao GetByTicketAsync abaixo.
        return CreatedAtAction(nameof(ObterPorChamadoAsync), new { ticketId = feedback.ChamadoId }, feedback);
    }

    // Lista feedbacks relacionados a um chamado específico.
    // Consulta via ticketId (GUID) e retorna a lista encontrada.
    [HttpGet("chamado/{ticketId:guid}")]
    public async Task<ActionResult> ObterPorChamadoAsync(Guid ticketId, CancellationToken cancellationToken)
    {
        // Recupera feedbacks do serviço para o chamado informado.
        var feedbacks = await _service.GetByTicketAsync(ticketId, cancellationToken);

        // Retorna os feedbacks encontrados (pode ser lista vazia se não houver registros).
        return Ok(feedbacks);
    }
}
