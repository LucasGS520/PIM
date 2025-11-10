using SupportSystem.Domain.Entities;

namespace SupportSystem.Application.Interfaces;

// Interface genérica de repositório para operações básicas de persistência (CRUD).
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    // Retorna uma consulta rastreável (IQueryable) sobre a entidade.
    IQueryable<TEntity> Query();

    // Obtém uma entidade pelo identificador único.
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    // Adiciona uma nova entidade ao contexto/armazenamento de forma assíncrona.
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    // Atualiza uma entidade existente no contexto.
    void Update(TEntity entity);

    // Remove uma entidade do contexto/armazenamento.
    void Remove(TEntity entity);
}
