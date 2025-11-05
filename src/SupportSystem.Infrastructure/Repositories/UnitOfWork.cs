using SupportSystem.Application.Interfaces;
using SupportSystem.Infrastructure.Data;

namespace SupportSystem.Infrastructure.Repositories;

/// <summary>
/// Implementa a unidade de trabalho sobre o contexto do Entity Framework.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SupportSystemDbContext _context;

    /// <summary>
    /// Inicializa a instância com o contexto compartilhado.
    /// </summary>
    public UnitOfWork(SupportSystemDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);
}
