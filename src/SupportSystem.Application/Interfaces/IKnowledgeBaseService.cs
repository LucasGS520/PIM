using SupportSystem.Application.Common;
using SupportSystem.Application.DTOs;

namespace SupportSystem.Application.Interfaces;

/// <summary>
/// Define operações para gerenciamento da base de conhecimento.
/// </summary>
public interface IKnowledgeBaseService
{
    /// <summary>
    /// Retorna uma listagem paginada de artigos.
    /// </summary>
    Task<PagedResult<KnowledgeBaseArticleDto>> GetAsync(int page, int pageSize, string? category, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém um artigo específico.
    /// </summary>
    Task<KnowledgeBaseArticleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Cria um novo artigo.
    /// </summary>
    Task<KnowledgeBaseArticleDto> CreateAsync(CreateKnowledgeBaseArticleDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza um artigo existente.
    /// </summary>
    Task<KnowledgeBaseArticleDto?> UpdateAsync(Guid id, UpdateKnowledgeBaseArticleDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Remove um artigo da base.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém sugestões de artigos relevantes com base no texto informado.
    /// </summary>
    Task<IReadOnlyCollection<KnowledgeBaseArticleDto>> SuggestAsync(string text, int limit, CancellationToken cancellationToken);
}
