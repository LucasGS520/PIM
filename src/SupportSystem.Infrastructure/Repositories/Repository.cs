using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;
using SupportSystem.Infrastructure.Data;

namespace SupportSystem.Infrastructure.Repositories;


// Implementação genérica de repositório baseada no Entity Framework Core.
// Fornece operações CRUD básicas reutilizáveis para entidades que herdam de BaseEntity.
public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    // Contexto do EF Core compartilhado entre repositórios.
    private readonly SupportSystemDbContext _context;

    // DbSet específico para o tipo TEntity, usado para consultas e operações.
    private readonly DbSet<TEntity> _set;

    // Inicializa a instância com o contexto compartilhado.
    // O DbSet é obtido a partir do contexto para evitar repetição de código.
    public Repository(SupportSystemDbContext context)
    {
        _context = context;
        _set = context.Set<TEntity>();
    }

    // Retorna um IQueryable para permitir consultas LINQ sobre a entidade.
    public IQueryable<TEntity> Query() => _set;

    // Recupera uma entidade pelo seu identificador de forma assíncrona.
    // Retorna null se a entidade não for encontrada.
    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _set.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    // Adiciona uma nova entidade ao contexto de forma assíncrona.
    // A persistência no banco ocorrerá quando SaveChanges/SaveChangesAsync for chamado no contexto.
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken) =>
        _set.AddAsync(entity, cancellationToken).AsTask();

    // Marca a entidade como modificada para que as alterações sejam persistidas no próximo SaveChanges.
    // Usamos o rastreamento do contexto para controlar as modificações.
    public void Update(TEntity entity)
    {
        _set.Update(entity);
    }

    // Remove a entidade do contexto. 
    // A exclusão efetiva ocorre no próximo SaveChanges.
    public void Remove(TEntity entity)
    {
        _set.Remove(entity);
    }
}
