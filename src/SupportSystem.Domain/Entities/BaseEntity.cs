namespace SupportSystem.Domain.Entities;

// Representa uma entidade base com identificador e datas auditáveis.
public abstract class BaseEntity
{
    // Identificador único da entidade.
    public Guid Id { get; set; } = Guid.NewGuid();

    // Data de criação do registro.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Data de última atualização do registro.
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
