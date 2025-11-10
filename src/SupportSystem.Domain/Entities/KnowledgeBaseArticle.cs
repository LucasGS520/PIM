namespace SupportSystem.Domain.Entities;

// Representa um artigo da base de conhecimento interno.
public class KnowledgeBaseArticle : BaseEntity
{
    // Título descritivo do artigo.
    public string Title { get; set; } = string.Empty;

    // Categoria principal do artigo para organização.
    public string Category { get; set; } = string.Empty;

    // Conteúdo detalhado do artigo, permitindo markdown.
    public string Content { get; set; } = string.Empty;

    // Palavras-chave usadas para busca e recomendação.
    public string Keywords { get; set; } = string.Empty;

    // Indicador de publicação para controle editorial.
    public bool IsPublished { get; set; } = true;

    // Sugestões vinculadas a chamados atendidos.
    public ICollection<TicketKnowledgeBaseSuggestion> TicketSuggestions { get; set; } = new List<TicketKnowledgeBaseSuggestion>();
}
