using Microsoft.AspNetCore.Mvc;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;

namespace SupportSystem.Api.Controllers;

/// <summary>
/// Controlador responsável pelo gerenciamento de usuários e perfis.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    /// <summary>
    /// Construtor que injeta o serviço de usuários.
    /// </summary>
    public UsersController(IUserService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista usuários de forma paginada para facilitar a administração.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um usuário específico pelo identificador informado.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Cria um novo usuário com base nos dados fornecidos.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza os dados de um usuário existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    /// <summary>
    /// Exclui definitivamente um usuário registrado.
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
}
