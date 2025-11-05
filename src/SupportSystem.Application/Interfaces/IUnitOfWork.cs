namespace SupportSystem.Application.Interfaces;

/// <summary>
/// Define contrato para confirmação de operações transacionais.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persiste alterações pendentes no armazenamento definitivo.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
