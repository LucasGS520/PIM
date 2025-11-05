using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;
using SupportSystem.Infrastructure.Data;

namespace SupportSystem.Infrastructure.Repositories;

/// <summary>
/// Implementação genérica de repositório baseada no Entity Framework Core.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade manipulada.</typeparam>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    private readonly SupportSystemDbContext _context;
    private readonly DbSet<TEntity> _set;

    /// <summary>
    /// Inicializa a instância com o contexto compartilhado.
    /// </summary>
    public Repository(SupportSystemDbContext context)
    {
        _context = context;
        _set = context.Set<TEntity>();
    }

    /// <inheritdoc />
    public IQueryable<TEntity> Query() => _set;

    /// <inheritdoc />
    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => _set.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken) => _set.AddAsync(entity, cancellationToken).AsTask();

    /// <inheritdoc />
    public void Update(TEntity entity)
    {
        // Utilizamos o rastreamento do contexto para controlar as modificações.
        _set.Update(entity);
    }

    /// <inheritdoc />
    public void Remove(TEntity entity)
    {
        _set.Remove(entity);
    }
}
