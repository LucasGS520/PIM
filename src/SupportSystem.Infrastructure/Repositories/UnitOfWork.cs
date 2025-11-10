using SupportSystem.Application.Interfaces;
using SupportSystem.Infrastructure.Data;

namespace SupportSystem.Infrastructure.Repositories;

// Implementa a unidade de trabalho (Unit of Work) sobre o contexto do Entity Framework.
// Fornece um ponto central para persistir alterações feitas por repositórios.
public class UnitOfWork : IUnitOfWork
{
    // Contexto do Entity Framework compartilhado entre repositórios.
    // Mantido como readonly para garantir que a instância não seja substituída após a criação.
    private readonly SupportSystemDbContext _context;

    // Inicializa a instância com o contexto compartilhado.
    public UnitOfWork(SupportSystemDbContext context)
    {
        _context = context; // Atribuição do contexto via injeção de dependência
    }

    // Persiste todas as alterações pendentes no contexto no banco de dados de forma assíncrona.
    // Chamado tipicamente após operações de repositórios para confirmar as alterações.

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        _context.SaveChangesAsync(cancellationToken); // Retorna o número de entidades afetadas
}
