using System.Threading;
using System.Threading.Tasks;

namespace SupportSystem.Application.Interfaces
{
    // Define o contrato do padrão Unit of Work responsável por confirmar operações transacionais.
    public interface IUnitOfWork
    {
        // Persiste alterações pendentes no armazenamento definitivo (confirma/commita a unidade).
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
