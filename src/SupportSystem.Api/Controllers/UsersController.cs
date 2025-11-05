using Microsoft.AspNetCore.Mvc;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;

namespace SupportSystem.Api.Controllers;

// Controlador responsável pelo gerenciamento de usuários e perfis.
// Fornece endpoints para listar, obter, criar, atualizar e excluir usuários.
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // Serviço de aplicação para operações relacionadas a usuários (injeção via DI).
    private readonly IUserService _service;

    // Construtor que injeta o serviço de usuários.
    public UsersController(IUserService service)
    {
        _service = service;
    }

    // Lista usuários de forma paginada para facilitar a administração.
    [HttpGet]
    public async Task<ActionResult> GetAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        // Chama o serviço de aplicação para obter os usuários paginados.
        var result = await _service.GetAsync(page, pageSize, cancellationToken);

        // Retorna 200 com o resultado (pode conter metadados de paginação, conforme implementação do serviço).
        return Ok(result);
    }

    // Obtém um usuário específico pelo identificador informado.
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Recupera o usuário pelo id.
        var result = await _service.GetByIdAsync(id, cancellationToken);

        // Se não foi encontrado, retorna 404.
        if (result is null)
        {
            return NotFound();
        }

        // Caso contrário, retorna 200 com o recurso.
        return Ok(result);
    }

    // Cria um novo usuário com base nos dados fornecidos.
    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        // Chama o serviço que realiza a criação do usuário.
        var created = await _service.CreateAsync(dto, cancellationToken);

        // Retorna 201 e inclui a rota para obter o recurso recém-criado.
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    // Atualiza os dados de um usuário existente.
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        // Executa a atualização via camada de aplicação.
        var updated = await _service.UpdateAsync(id, dto, cancellationToken);

        // Se não foi possível atualizar (usuário não existe), retorna 404.
        if (updated is null)
        {
            return NotFound();
        }

        // Retorna 200 com a entidade atualizada.
        return Ok(updated);
    }

    // Exclui definitivamente um usuário registrado.
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        // Solicita exclusão ao serviço de aplicação.
        var deleted = await _service.DeleteAsync(id, cancellationToken);

        // Caso o recurso não exista, retorna 404.
        if (!deleted)
        {
            return NotFound();
        }

        // Sucesso sem conteúdo no corpo da resposta.
        return NoContent();
    }
}
