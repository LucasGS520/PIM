using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.Common;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;

namespace SupportSystem.Application.Services;

// Implementa regras de negócio para o gerenciamento de usuários.
// Contém operações básicas: listagem paginada, consulta por id, criação, atualização e exclusão.
public class UserService : IUserService
{
    // Repositório genérico para acesso aos dados de User.
    private readonly IRepository<User> _repository;

    // Unidade de trabalho para persistir mudanças de forma transacional.
    private readonly IUnitOfWork _unitOfWork;

    // Inicializa a instância com dependências de persistência.
    // Recebe o repositório e a unidade de trabalho via injeção de dependência.
    public UserService(IRepository<User> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    // Retorna uma página de usuários com total para paginação.
    public async Task<PagedResult<UserDto>> GetAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        // Obtém a base de consulta sem rastreamento de alterações.
        var query = _repository.Query().AsNoTracking();

        // Conta total de registros para cálculo de paginação.
        var total = await query.LongCountAsync(cancellationToken);

        // Aplica ordenação, paginação e executa a consulta.
        var users = await query
            .OrderBy(u => u.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Mapeia entidades para DTOs e retorna resultado paginado.
        var dtos = users.Select(MapToDto).ToList();
        return new PagedResult<UserDto>(dtos, total, page, pageSize);
    }

    // Retorna um usuário pelo identificador ou null se não existir.
    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.Query().AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    // Cria um novo usuário a partir do DTO de criação.
    // Faz hash da senha antes de persistir.
    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken)
    {
        var entity = new User
        {
            FullName = dto.NomeCompleto,
            Email = dto.Email.ToLowerInvariant(),
            PasswordHash = HashPassword(dto.Senha),
            PhoneNumber = dto.Telefone,
            Department = dto.Departamento,
            Role = dto.Perfil
        };

        // Adiciona a entidade e salva as alterações via UnitOfWork.
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    // Atualiza campos editáveis de um usuário existente.
    // Retorna null se o usuário não existir.
    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        // Atualiza apenas os campos permitidos para edição.
        entity.FullName = dto.NomeCompleto;
        entity.PhoneNumber = dto.Telefone;
        entity.Department = dto.Departamento;
        entity.Role = dto.Perfil;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }


    // Remove um usuário pelo id. Retorna true se removido, false se não encontrado.
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _repository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Mapeia entidade User para DTO de saída.
    private static UserDto MapToDto(User entity) => new(
        entity.Id,
        entity.FullName,
        entity.Email,
        entity.PhoneNumber,
        entity.Department,
        entity.Role
    );

    // Gera hash da senha usando SHA256.
    // Observação: SHA256 é usado aqui apenas para demonstração; em produção prefira algoritmos específicos para senhas (ex: BCrypt, Argon2) com salt.
    private static string HashPassword(string password)
    {
        // Optamos por SHA256 para manter a demonstração independente de pacotes externos.
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hashBytes = sha.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }
}
