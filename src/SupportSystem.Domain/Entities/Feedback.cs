namespace SupportSystem.Domain.Entities;

/// <summary>
/// Armazena a avaliação de atendimento submetida pelo cliente.
/// </summary>
public class Feedback : BaseEntity
{
    /// <summary>
    /// Identificador do chamado avaliado.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Entidade navegacional do chamado avaliado.
    /// </summary>
    public Ticket? Ticket { get; set; }

    /// <summary>
    /// Identificador do usuário que enviou o feedback.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Entidade navegacional do usuário avaliador.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Nota atribuída pelo cliente ao atendimento.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Comentário opcional com observações do cliente.
    /// </summary>
    public string? Comment { get; set; }
}
