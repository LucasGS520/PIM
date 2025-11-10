using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SupportSystem.Application.AI
{
    // Interface para clientes de IA responsáveis por classificar chamados.
    // Implementações devem analisar título e descrição do chamado e retornar informações como categoria, prioridade e artigos sugeridos.
    public interface IAssistantClient
    {
        // Analisa o texto informado e retorna categoria, prioridade e artigos sugeridos.
        Task<AssistantResult> AnalyzeTicketAsync(string title, string description, IEnumerable<string>? keywords, CancellationToken cancellationToken);
    }
}
