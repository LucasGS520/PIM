namespace SupportSystem.Domain.Entities;

// Registra as interações e alterações relevantes de um chamado.
public class TicketHistory : BaseEntity
{
    // Identificador do chamado relacionado.
    public Guid TicketId { get; set; }

    // Entidade navegacional para o chamado.
    public Ticket? Ticket { get; set; }


    // Identificador do autor da atualização.
    public Guid AuthorId { get; set; }

    // Entidade navegacional para o autor.
    public User? Author { get; set; }

    // Conteúdo textual da interação registrada.
    public string Message { get; set; } = string.Empty;

    // Status do chamado após a interação.
    public string StatusSnapshot { get; set; } = string.Empty;
}
