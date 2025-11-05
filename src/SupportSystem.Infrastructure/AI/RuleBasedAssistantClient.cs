using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.AI;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;

namespace SupportSystem.Infrastructure.AI;

/// <summary>
/// Implementação simplificada do módulo de IA com regras heurísticas.
/// </summary>
public class RuleBasedAssistantClient : IAssistantClient
{
    private readonly IRepository<KnowledgeBaseArticle> _articles;

    /// <summary>
    /// Inicializa o cliente com acesso aos artigos para sugerir soluções.
    /// </summary>
    public RuleBasedAssistantClient(IRepository<KnowledgeBaseArticle> articles)
    {
        _articles = articles;
    }

    /// <inheritdoc />
    public async Task<AssistantResult> AnalyzeTicketAsync(string title, string description, IEnumerable<string>? keywords, CancellationToken cancellationToken)
    {
        // Aplicamos regras simples baseadas em palavras-chave para manter a demonstração funcional sem dependências externas.
        var text = $"{title} {description} {string.Join(' ', keywords ?? Array.Empty<string>())}".ToLowerInvariant();
        var category = Categorize(text);
        var priority = InferPriority(text);
        var relatedArticles = await SuggestArticlesAsync(text, cancellationToken);
        return new AssistantResult(category, priority, relatedArticles);
    }

    private static string Categorize(string text)
    {
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

        return "Outros";
    }

    private static string InferPriority(string text)
    {
        if (text.Contains("parado") || text.Contains("impossível trabalhar") || text.Contains("urgente"))
        {
            return "Crítica";
        }

        if (text.Contains("intermitente") || text.Contains("lento"))
        {
            return "Alta";
        }

        if (text.Contains("dúvida") || text.Contains("consulta"))
        {
            return "Baixa";
        }

        return "Média";
    }

    private async Task<IReadOnlyCollection<Guid>> SuggestArticlesAsync(string text, CancellationToken cancellationToken)
    {
        var query = _articles.Query()
            .Where(a => a.IsPublished &&
                        (a.Title.ToLower().Contains(text) || a.Content.ToLower().Contains(text) || a.Keywords.ToLower().Contains(text)))
            .OrderByDescending(a => a.UpdatedAt)
            .Take(3);

        var articles = await query.Select(a => a.Id).ToListAsync(cancellationToken);
        return articles;
    }
}
