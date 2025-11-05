using SupportSystem.Domain.Entities;

namespace SupportSystem.Application.Interfaces;

/// <summary>
/// Contrato genérico para operações básicas de persistência.
/// </summary>
/// <typeparam name="TEntity">Entidade do domínio manipulado.</typeparam>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    /// <summary>
    /// Retorna uma consulta rastreável sobre a entidade.
    /// </summary>
    IQueryable<TEntity> Query();

    /// <summary>
    /// Obtém uma entidade pelo identificador único.
    /// </summary>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Adiciona uma nova entidade ao contexto.
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza uma entidade existente no contexto.
    /// </summary>
    void Update(TEntity entity);

    /// <summary>
    /// Remove uma entidade do contexto.
    /// </summary>
    void Remove(TEntity entity);
}
