namespace SupportSystem.Domain.Entities;

// Relaciona um chamado a um artigo sugerido pela IA.
public class TicketKnowledgeBaseSuggestion : BaseEntity
{
    // Identificador do chamado vinculado à sugestão.
    public Guid TicketId { get; set; }

    // Entidade navegacional do chamado.
    public Ticket? Ticket { get; set; }

    // Identificador do artigo de conhecimento recomendado.
    public Guid KnowledgeBaseArticleId { get; set; }

    // Entidade navegacional do artigo recomendado.
    public KnowledgeBaseArticle? KnowledgeBaseArticle { get; set; }

    // Pontuação de relevância retornada pela IA.
    public double Score { get; set; }
}
