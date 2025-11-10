using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.AI;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;

namespace SupportSystem.Infrastructure.AI;

// Implementação simplificada do módulo de IA com regras heurísticas.
// Este cliente usa regras locais e pesquisa em base de conhecimento para sugerir categoria, prioridade e artigos relacionados a um chamado.
public class RuleBasedAssistantClient : IAssistantClient
{
    // Repositório de artigos da base de conhecimento (acesso a dados).
    private readonly IRepository<KnowledgeBaseArticle> _articles;

    // Inicializa o cliente com acesso aos artigos para sugerir soluções.
    public RuleBasedAssistantClient(IRepository<KnowledgeBaseArticle> articles)
    {
        _articles = articles; // Repositório para consultar artigos publicados
    }

    // Analisa título/descrição/chaves do chamado e retorna sugestão baseada em regras.
    public async Task<AssistantResult> AnalyzeTicketAsync(string title, string description, IEnumerable<string>? keywords, CancellationToken cancellationToken)
    {
        // Monta o texto que será avaliado pelas regras (título + descrição + palavras-chave).
        var text = $"{title} {description} {string.Join(' ', keywords ?? Array.Empty<string>())}".ToLowerInvariant();

        // Determina categoria com regras simples de palavras-chave.
        var category = Categorize(text);

        // Determina prioridade com regras simples de palavras-chave.
        var priority = InferPriority(text);

        // Busca artigos relacionados na base de conhecimento.
        var relatedArticles = await SuggestArticlesAsync(text, cancellationToken);

        return new AssistantResult(category, priority, relatedArticles);
    }

    // Categoriza o texto em áreas comuns (Rede, Acessos, Hardware, Software, Outros).
    private static string Categorize(string text)
    {
        // Regras simples: verifica substring de termos relevantes.
        if (text.Contains("rede") || text.Contains("wi-fi") || text.Contains("vpn"))
        {
            return "Rede";
        }

        if (text.Contains("acesso") || text.Contains("login") || text.Contains("senha"))
        {
            return "Acessos";
        }

        if (text.Contains("impressora") || text.Contains("hardware") || text.Contains("equipamento"))
        {
            return "Hardware";
        }

        if (text.Contains("sistema") || text.Contains("erro") || text.Contains("bug"))
        {
            return "Software";
        }

        // Categoria padrão quando nenhuma regra anterior corresponde.
        return "Outros";
    }

    // Inferência simples de prioridade com base em termos críticos, intermitentes ou dúvidas.
    private static string InferPriority(string text)
    {
        // Termos que indicam parada total ou impacto crítico.
        if (text.Contains("parado") || text.Contains("impossível trabalhar") || text.Contains("urgente"))
        {
            return "Crítica";
        }

        // Termos que indicam degradação de serviço.
        if (text.Contains("intermitente") || text.Contains("lento"))
        {
            return "Alta";
        }

        // Termos que indicam apenas dúvida ou consulta.
        if (text.Contains("dúvida") || text.Contains("consulta"))
        {
            return "Baixa";
        }

        // Prioridade padrão quando nenhuma regra corresponde.
        return "Média";
    }

    // Sugere até 3 artigos publicados relacionados ao texto de entrada.
    private async Task<IReadOnlyCollection<Guid>> SuggestArticlesAsync(string text, CancellationToken cancellationToken)
    {
        // Observação: a utilização de ToLower na consulta pode impedir indexação; ajustar conforme provedor/coluna.
        var query = _articles.Query()
            .Where(a => a.IsPublished &&
                        (a.Title.ToLower().Contains(text) || a.Content.ToLower().Contains(text) || a.Keywords.ToLower().Contains(text)))
            .OrderByDescending(a => a.UpdatedAt)
            .Take(3);

        // Executa a query retornando apenas os IDs (eficiente para tráfego reduzido).
        var articles = await query.Select(a => a.Id).ToListAsync(cancellationToken);
        return articles;
    }
}
