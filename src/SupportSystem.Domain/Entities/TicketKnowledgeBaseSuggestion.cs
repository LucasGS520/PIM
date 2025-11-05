namespace SupportSystem.Domain.Entities;

/// <summary>
/// Relaciona um chamado a um artigo sugerido pela IA.
/// </summary>
public class TicketKnowledgeBaseSuggestion : BaseEntity
{
    /// <summary>
    /// Identificador do chamado vinculado à sugestão.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Entidade navegacional do chamado.
    /// </summary>
    public Ticket? Ticket { get; set; }

    /// <summary>
    /// Identificador do artigo de conhecimento recomendado.
    /// </summary>
    public Guid KnowledgeBaseArticleId { get; set; }

    /// <summary>
    /// Entidade navegacional do artigo recomendado.
    /// </summary>
    public KnowledgeBaseArticle? KnowledgeBaseArticle { get; set; }

    /// <summary>
    /// Pontuação de relevância retornada pela IA.
    /// </summary>
    public double Score { get; set; }
}
