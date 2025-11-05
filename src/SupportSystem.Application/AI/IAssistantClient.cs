namespace SupportSystem.Application.AI;

/// <summary>
/// Define a interface para clientes de IA responsáveis por classificar chamados.
/// </summary>
public interface IAssistantClient
{
    /// <summary>
    /// Analisa o texto informado e retorna categoria, prioridade e artigos sugeridos.
    /// </summary>
    Task<AssistantResult> AnalyzeTicketAsync(string title, string description, IEnumerable<string>? keywords, CancellationToken cancellationToken);
}
