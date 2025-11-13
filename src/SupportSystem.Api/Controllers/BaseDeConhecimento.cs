using System;
using Microsoft.AspNetCore.Mvc;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;

namespace SupportSystem.Api.Controllers;

// Controlador responsável pela manutenção da base de conhecimento.
// Expõe endpoints REST para CRUD de artigos, paginação, busca por categoria e sugestões.
[ApiController]
[Route("api/[controller]")]
public class BaseDeConhecimento : ControllerBase
{
    // Serviço que encapsula a lógica de negócio e acesso a dados da base de conhecimento.
    private readonly IKnowledgeBaseService _service;

    // Construtor com injeção do serviço de base de conhecimento.
    // A injeção facilita testes e separação de responsabilidades.
    public BaseDeConhecimento(IKnowledgeBaseService service)
    {
        _service = service;
    }

    // Lista artigos com paginação e filtro opcional por categoria.
    [HttpGet]
    public async Task<ActionResult> ObterAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? category = null, CancellationToken cancellationToken = default)
    {
        // Delega ao serviço a recuperação dos artigos aplicando paginação e filtro de categoria.
        var result = await _service.GetAsync(page, pageSize, category, cancellationToken);
        return Ok(result);
    }

    // Recupera um artigo específico pelo identificador.
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Busca o artigo pelo id; se não existir, retorna 404.
        var artigo = await _service.GetByIdAsync(id, cancellationToken);
        if (artigo is null)
        {
            return NotFound();
        }

        return Ok(artigo);
    }


    // Cria um novo artigo na base.
    [HttpPost]
    public async Task<ActionResult> CriarAsync([FromBody] CriarArtigoBaseConhecimentoDto dto, CancellationToken cancellationToken)
    {
        // Validação adicional pode ser aplicada no serviço ou via filtros/atributos.
        var artigo = await _service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(ObterPorIdAsync), new { id = artigo.Id }, artigo);
    }


    // Atualiza um artigo existente.
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> AtualizarAsync(Guid id, [FromBody] AtualizarArtigoBaseConhecimentoDto dto, CancellationToken cancellationToken)
    {
        // Chama serviço para atualizar; serviço deve retornar null se não existir o artigo.
        var artigo = await _service.UpdateAsync(id, dto, cancellationToken);
        if (artigo is null)
        {
            return NotFound();
        }

        return Ok(artigo);
    }

    // Exclui um artigo da base.
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> ExcluirAsync(Guid id, CancellationToken cancellationToken)
    {
        // O serviço retorna booleano indicando sucesso da operação.
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }


    // Sugere artigos com base no texto informado (por exemplo, para autocomplete ou busca relacionada).
    [HttpGet("sugestoes")]
    public async Task<ActionResult> SugerirAsync([FromQuery] string text, [FromQuery] int limit = 5, CancellationToken cancellationToken = default)
    {
        // Encaminha para o serviço que pode utilizar análise de texto, similaridade, ou mecanismo de busca.
        var suggestions = await _service.SuggestAsync(text, limit, cancellationToken);
        return Ok(suggestions);
    }
}
