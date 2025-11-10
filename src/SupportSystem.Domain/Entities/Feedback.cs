namespace SupportSystem.Domain.Entities;

// Armazena a avaliação de atendimento submetida pelo cliente.
public class Feedback : BaseEntity
{
    // Identificador do chamado avaliado.
    public Guid TicketId { get; set; }

    // Entidade navegacional do chamado avaliado.
    public Ticket? Ticket { get; set; }

    // Identificador do usuário que enviou o feedback.
    public Guid UserId { get; set; }

    // Entidade navegacional do usuário avaliador.
    public User? User { get; set; }

    // Nota atribuída pelo cliente ao atendimento.
    public int Score { get; set; }

    // Comentário opcional com observações do cliente.
    public string? Comment { get; set; }
}
