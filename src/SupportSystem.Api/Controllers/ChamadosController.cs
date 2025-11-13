using System;
using Microsoft.AspNetCore.Mvc;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Enums;

namespace SupportSystem.Api.Controllers;

// Controlador responsável pela gestão do ciclo de vida dos chamados.
// Fornece endpoints para criar, atualizar, recuperar, reabrir e obter histórico.
[ApiController]
[Route("api/[controller]")]
public class ChamadosController : ControllerBase
{
    // Serviço que encapsula a lógica de domínio dos chamados (CRUD, filtros, histórico).
    private readonly ITicketService _ticketService;

    // Serviço que fornece sugestões da base de conhecimento dado um texto (utilizado ao criar chamados).
    private readonly IKnowledgeBaseService _knowledgeBaseService;

    // Construtor com injeção dos serviços necessários.
    public ChamadosController(ITicketService ticketService, IKnowledgeBaseService knowledgeBaseService)
    {
        _ticketService = ticketService;
        _knowledgeBaseService = knowledgeBaseService;
    }

    // Lista chamados com paginação e filtros opcionais.
    [HttpGet]
    public async Task<ActionResult> GetAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] TicketStatus? status = null,
        [FromQuery] Guid? requesterId = null,
        [FromQuery] Guid? technicianId = null,
        CancellationToken cancellationToken = default)
    {
        // Recupera lista paginada via serviço. O serviço aplica filtros e retorna resultado pronto para a API.
        var result = await _ticketService.GetAsync(page, pageSize, status, requesterId, technicianId, cancellationToken);
        return Ok(result);
    }

    // Obtém detalhes de um chamado específico por Id.
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Busca o chamado. Retorna 404 se não encontrado.
        var chamado = await _ticketService.GetByIdAsync(id, cancellationToken);
        if (chamado is null)
        {
            return NotFound();
        }

        return Ok(chamado);
    }

    // Cria um novo chamado e retorna sugestões da base de conhecimento relacionadas à descrição.
    [HttpPost]
    public async Task<ActionResult> CriarAsync([FromBody] CriarChamadoDto dto, CancellationToken cancellationToken)
    {
        // Cria o chamado via serviço de domínio.
        var chamado = await _ticketService.CreateAsync(dto, cancellationToken);

        // Solicita sugestões da base de conhecimento com base na descrição informada.
        // Limita a 3 sugestões por padrão.
        var sugestoes = await _knowledgeBaseService.SuggestAsync(dto.Descricao, 3, cancellationToken);

        // Retorna 201 com local do recurso e payload contendo o chamado e as sugestões.
        return CreatedAtAction(nameof(ObterPorIdAsync), new { id = chamado.Id }, new { chamado, sugestoes });
    }

    // Atualiza informações do chamado e registra histórico de alterações.
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> AtualizarAsync(Guid id, [FromBody] AtualizarChamadoDto dto, CancellationToken cancellationToken)
    {
        // Atualiza via serviço; serviço retorna null se o recurso não existir.
        var atualizado = await _ticketService.UpdateAsync(id, dto, cancellationToken);
        if (atualizado is null)
        {
            return NotFound();
        }

        return Ok(atualizado);
    }

    // Reabre um chamado dentro do prazo permitido pelo sistema.
    [HttpPost("{id:guid}/reabrir")]
    public async Task<ActionResult> ReabrirAsync(Guid id, [FromQuery] Guid requesterId, CancellationToken cancellationToken)
    {
        // Lógica de reabertura delegada ao serviço de tickets (valida prazo, autorizações, etc.).
        var reaberto = await _ticketService.ReopenAsync(id, requesterId, cancellationToken);
        if (reaberto is null)
        {
            return NotFound();
        }

        return Ok(reaberto);
    }

    // Recupera o histórico de interações e alterações de um chamado.
    [HttpGet("{id:guid}/historico")]
    public async Task<ActionResult> ObterHistoricoAsync(Guid id, CancellationToken cancellationToken)
    {
        // Retorna histórico (eventos, comentários, mudanças de status) fornecido pelo serviço.
        var historico = await _ticketService.GetHistoryAsync(id, cancellationToken);
        return Ok(historico);
    }
}
