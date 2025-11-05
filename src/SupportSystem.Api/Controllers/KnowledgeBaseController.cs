using Microsoft.AspNetCore.Mvc;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;

namespace SupportSystem.Api.Controllers;

/// <summary>
/// Controlador responsável pela manutenção da base de conhecimento.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class KnowledgeBaseController : ControllerBase
{
    private readonly IKnowledgeBaseService _service;

    /// <summary>
    /// Construtor com injeção do serviço de base de conhecimento.
    /// </summary>
    public KnowledgeBaseController(IKnowledgeBaseService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista artigos com filtros opcionais por categoria.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? category = null, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAsync(page, pageSize, category, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Recupera um artigo específico pelo identificador.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var article = await _service.GetByIdAsync(id, cancellationToken);
        if (article is null)
        {
            return NotFound();
        }

        return Ok(article);
    }

    /// <summary>
    /// Cria um novo artigo na base.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] CreateKnowledgeBaseArticleDto dto, CancellationToken cancellationToken)
    {
        var article = await _service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = article.Id }, article);
    }

    /// <summary>
    /// Atualiza um artigo existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateKnowledgeBaseArticleDto dto, CancellationToken cancellationToken)
    {
        var article = await _service.UpdateAsync(id, dto, cancellationToken);
        if (article is null)
        {
            return NotFound();
        }

        return Ok(article);
    }

    /// <summary>
    /// Exclui um artigo da base.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Sugere artigos com base no texto informado.
    /// </summary>
    [HttpGet("suggestions")]
    public async Task<ActionResult> SuggestAsync([FromQuery] string text, [FromQuery] int limit = 5, CancellationToken cancellationToken = default)
    {
        var suggestions = await _service.SuggestAsync(text, limit, cancellationToken);
        return Ok(suggestions);
    }
}
