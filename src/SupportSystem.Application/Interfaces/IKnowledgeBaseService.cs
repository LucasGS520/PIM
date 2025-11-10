using SupportSystem.Application.Common;
using SupportSystem.Application.DTOs;

namespace SupportSystem.Application.Interfaces;

// Operações para gerenciamento da base de conhecimento (artigos).
// Inclui listagem paginada, CRUD e sugestões baseadas em texto.
// Os métodos suportam  cancelamento de operações assíncronas.
public interface IKnowledgeBaseService
{
    // Retorna uma listagem paginada de artigos.
    Task<PagedResult<KnowledgeBaseArticleDto>> GetAsync(int page, int pageSize, string? category, CancellationToken cancellationToken);

    // Obtém um artigo específico.
    Task<KnowledgeBaseArticleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    // Cria um novo artigo.
    Task<KnowledgeBaseArticleDto> CreateAsync(CreateKnowledgeBaseArticleDto dto, CancellationToken cancellationToken);

    // Atualiza um artigo existente.
    Task<KnowledgeBaseArticleDto?> UpdateAsync(Guid id, UpdateKnowledgeBaseArticleDto dto, CancellationToken cancellationToken);

    // Remove um artigo da base.
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    // Obtém sugestões de artigos relevantes com base no texto informado.
    Task<IReadOnlyCollection<KnowledgeBaseArticleDto>> SuggestAsync(string text, int limit, CancellationToken cancellationToken);
}
