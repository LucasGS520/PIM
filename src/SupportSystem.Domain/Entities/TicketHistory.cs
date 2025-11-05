namespace SupportSystem.Domain.Entities;

/// <summary>
/// Registra as interações e alterações relevantes de um chamado.
/// </summary>
public class TicketHistory : BaseEntity
{
    /// <summary>
    /// Identificador do chamado relacionado.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Entidade navegacional para o chamado.
    /// </summary>
    public Ticket? Ticket { get; set; }

    /// <summary>
    /// Identificador do autor da atualização.
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Entidade navegacional para o autor.
    /// </summary>
    public User? Author { get; set; }

    /// <summary>
    /// Conteúdo textual da interação registrada.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Status do chamado após a interação.
    /// </summary>
    public string StatusSnapshot { get; set; } = string.Empty;
}
