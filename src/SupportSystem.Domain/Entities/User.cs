using SupportSystem.Domain.Enums;

namespace SupportSystem.Domain.Entities;

// Representa um usuário da plataforma com suas credenciais e permissões.
public class User : BaseEntity
{
    // Nome completo do usuário para identificação no atendimento.
    public string FullName { get; set; } = string.Empty;

    // Endereço de e-mail utilizado para comunicação e login.
    public string Email { get; set; } = string.Empty;

    // Hash da senha para autenticação segura.
    public string PasswordHash { get; set; } = string.Empty;

    // Telefone de contato opcional do usuário.
    public string? PhoneNumber { get; set; }

    // Cargo ou setor relacionado ao usuário técnico.
    public string? Department { get; set; }

    // Perfil de acesso configurado para controlar permissões.
    public UserRole Role { get; set; }

    // Chamados abertos pelo usuário cliente.
    public ICollection<Ticket> RequestedTickets { get; set; } = new List<Ticket>();

    // Chamados atribuídos ao usuário técnico.
    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();

    // Avaliações realizadas pelo usuário após o atendimento.
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
