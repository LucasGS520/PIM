namespace SupportSystem.Domain.Entities;

/// <summary>
/// Representa um artigo da base de conhecimento interno.
/// </summary>
public class KnowledgeBaseArticle : BaseEntity
{
    /// <summary>
    /// Título descritivo do artigo.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Categoria principal do artigo para organização.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Conteúdo detalhado do artigo, permitindo markdown.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Palavras-chave usadas para busca e recomendação.
    /// </summary>
    public string Keywords { get; set; } = string.Empty;

    /// <summary>
    /// Indicador de publicação para controle editorial.
    /// </summary>
    public bool IsPublished { get; set; } = true;

    /// <summary>
    /// Sugestões vinculadas a chamados atendidos.
    /// </summary>
    public ICollection<TicketKnowledgeBaseSuggestion> TicketSuggestions { get; set; } = new List<TicketKnowledgeBaseSuggestion>();
}
