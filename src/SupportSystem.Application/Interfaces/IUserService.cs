using SupportSystem.Application.Common;
using SupportSystem.Application.DTOs;

namespace SupportSystem.Application.Interfaces;

/// <summary>
/// Define operações de alto nível para gerenciamento de usuários.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Realiza consulta paginada dos usuários cadastrados.
    /// </summary>
    Task<PagedResult<UserDto>> GetAsync(int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém um usuário específico pelo identificador.
    /// </summary>
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Cria um novo usuário com perfil definido.
    /// </summary>
    Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza dados cadastrais e perfil de acesso de um usuário.
    /// </summary>
    Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Remove um usuário do sistema de forma lógica.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
