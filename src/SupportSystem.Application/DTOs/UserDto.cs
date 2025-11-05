using SupportSystem.Domain.Enums;

namespace SupportSystem.Application.DTOs;

/// <summary>
/// Representa os dados expostos de um usuário para as APIs.
/// </summary>
public record UserDto(
    Guid Id,
    string NomeCompleto,
    string Email,
    string? Telefone,
    string? Departamento,
    UserRole Perfil
);

/// <summary>
/// DTO para criação de usuários com validação básica.
/// </summary>
public record CreateUserDto(
    string NomeCompleto,
    string Email,
    string Senha,
    string? Telefone,
    string? Departamento,
    UserRole Perfil
);

/// <summary>
/// DTO utilizado para atualização de usuários existentes.
/// </summary>
public record UpdateUserDto(
    string NomeCompleto,
    string? Telefone,
    string? Departamento,
    UserRole Perfil
);
