using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SupportSystem.Application.AI
{
    // Interface para clientes de IA responsáveis por classificar chamados.
    // Implementações devem analisar título e descrição do chamado e retornar informações como categoria, prioridade e artigos sugeridos.
    public interface IAClienteAssistente
    {
        // Analisa o texto informado e retorna categoria, prioridade e artigos sugeridos.
        Task<ResultadoAssistente> AnalisarChamadoAsync(string titulo, string descricao, IEnumerable<string>? palavrasChave, CancellationToken cancellationToken);
    }
}
