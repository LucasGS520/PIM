namespace SupportSystem.Domain.Entities;

// Armazena metadados de um anexo associado ao chamado.
public class TicketAttachment : BaseEntity
{
    // Identificador do chamado ao qual o anexo pertence.
    public Guid TicketId { get; set; }

    // Entidade navegacional para o chamado.
    public Ticket? Ticket { get; set; }

    // Nome amigável do arquivo anexado.
    public string FileName { get; set; } = string.Empty;

    // Localização do arquivo no armazenamento configurado.
    public string FilePath { get; set; } = string.Empty;

    // Tipo de conteúdo do arquivo para validação.
    public string ContentType { get; set; } = string.Empty;
}
