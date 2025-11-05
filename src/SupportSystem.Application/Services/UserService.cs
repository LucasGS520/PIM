using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.Common;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;

namespace SupportSystem.Application.Services;

/// <summary>
/// Implementa regras de negócio para o gerenciamento de usuários.
/// </summary>
public class UserService : IUserService
{
    private readonly IRepository<User> _repository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Inicializa a instância com dependências de persistência.
    /// </summary>
    public UserService(IRepository<User> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<PagedResult<UserDto>> GetAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        // Utilizamos AsNoTracking para evitar custos desnecessários na consulta de listagem.
        var query = _repository.Query().AsNoTracking();
        var total = await query.LongCountAsync(cancellationToken);
        var users = await query
            .OrderBy(u => u.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = users.Select(MapToDto).ToList();
        return new PagedResult<UserDto>(dtos, total, page, pageSize);
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.Query().AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc />
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

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.FullName = dto.NomeCompleto;
        entity.PhoneNumber = dto.Telefone;
        entity.Department = dto.Departamento;
        entity.Role = dto.Perfil;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    /// <inheritdoc />
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

    private static UserDto MapToDto(User entity) => new(
        entity.Id,
        entity.FullName,
        entity.Email,
        entity.PhoneNumber,
        entity.Department,
        entity.Role
    );

    private static string HashPassword(string password)
    {
        // Optamos por SHA256 para manter a demonstração independente de pacotes externos.
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hashBytes = sha.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }
}
