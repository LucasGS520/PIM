using SupportSystem.Application.Common;
using SupportSystem.Application.DTOs;

namespace SupportSystem.Application.Interfaces;

// Interface que define operações de alto nível para gerenciamento de usuários.
// Fornece métodos assíncronos para listagem, consulta, criação, atualização e remoção lógica de usuários.
public interface IUserService
{
    // Realiza consulta paginada dos usuários cadastrados.
    Task<PagedResult<UserDto>> GetAsync(int page, int pageSize, CancellationToken cancellationToken);

    // Obtém um usuário específico pelo identificador.
    // Retorna null se o usuário não existir.
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    // Cria um novo usuário com perfil definido.
    // Deve validar dados e persistir a nova entidade.
    Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken);

    // Atualiza dados cadastrais e perfil de acesso de um usuário existente.
    // Retorna null se o usuário não for encontrado.
    Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken);

    // Remove um usuário do sistema de forma lógica.
    // Não remove fisicamente o registro, apenas marca como inativo/excluído.
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
