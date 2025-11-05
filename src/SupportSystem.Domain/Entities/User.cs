using SupportSystem.Domain.Enums;

namespace SupportSystem.Domain.Entities;

/// <summary>
/// Representa um usuário da plataforma com suas credenciais e permissões.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Nome completo do usuário para identificação no atendimento.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Endereço de e-mail utilizado para comunicação e login.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash da senha para autenticação segura.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Telefone de contato opcional do usuário.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Cargo ou setor relacionado ao usuário técnico.
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Perfil de acesso configurado para controlar permissões.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Chamados abertos pelo usuário cliente.
    /// </summary>
    public ICollection<Ticket> RequestedTickets { get; set; } = new List<Ticket>();

    /// <summary>
    /// Chamados atribuídos ao usuário técnico.
    /// </summary>
    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();

    /// <summary>
    /// Avaliações realizadas pelo usuário após o atendimento.
    /// </summary>
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
