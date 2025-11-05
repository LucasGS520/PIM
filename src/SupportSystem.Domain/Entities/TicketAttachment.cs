namespace SupportSystem.Domain.Entities;

/// <summary>
/// Armazena metadados de um anexo associado ao chamado.
/// </summary>
public class TicketAttachment : BaseEntity
{
    /// <summary>
    /// Identificador do chamado ao qual o anexo pertence.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Entidade navegacional para o chamado.
    /// </summary>
    public Ticket? Ticket { get; set; }

    /// <summary>
    /// Nome amigável do arquivo anexado.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Localização do arquivo no armazenamento configurado.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de conteúdo do arquivo para validação.
    /// </summary>
    public string ContentType { get; set; } = string.Empty;
}
