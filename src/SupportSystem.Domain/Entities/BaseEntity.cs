namespace SupportSystem.Domain.Entities;

/// <summary>
/// Representa uma entidade base com identificador e datas auditáveis.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único da entidade.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Data de criação do registro.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de última atualização do registro.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
