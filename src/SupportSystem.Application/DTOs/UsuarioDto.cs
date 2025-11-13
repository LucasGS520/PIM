using System;
using SupportSystem.Domain.Enums;

// Conjunto de DTOs para operações com usuário (leitura, criação e atualização).
// Mantêm uma representação leve e segura dos dados trafegados entre camadas.
namespace SupportSystem.Application.DTOs;

// Representa os dados expostos de um usuário para as APIs.
public record UsuarioDto(
    Guid Id, // Identificador único do usuário
    string NomeCompleto, // Nome completo do usuário
    string Email, // Endereço de e-mail do usuário
    string? Telefone, // Telefone de contato (opcional)
    string? Departamento, // Departamento ou setor do usuário (opcional)
    UserRole Perfil // Perfil do usuário
);

// DTO para criação de usuários com validação básica.

// A senha aqui é recebida em texto plano da API e deve ser tratada com segurança:
// fazer hashing antes de persistir e nunca logar ou retornar a senha.
public record CriarUsuarioDto(
    string NomeCompleto, // Nome completo do novo usuário 
    string Email, // E-mail do novo usuário
    string Senha, // Senha em texto plano recebida na criação (tratar com segurança)
    string? Telefone, // Telefone de contato (opcional)
    string? Departamento, // Departamento ou setor (opcional)
    UserRole Perfil // Perfil a ser atribuído ao usuário
);

// DTO utilizado para atualização de usuários existentes.
// Não contém campos sensíveis como senha ou e-mail para alteração por este DTO;
// se houver necessidade de alterar a senha ou o e-mail, utilizar endpoints/Dtos específicos.
public record AtualizarUsuarioDto(
    string NomeCompleto, // Nome completo atualizado do usuário
    string? Telefone, // Telefone atualizado (opcional)
    string? Departamento, // Departamento atualizado (opcional)
    UserRole Perfil // Perfil atualizado do usuário
);
