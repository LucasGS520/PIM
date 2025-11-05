using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.Common;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;

namespace SupportSystem.Application.Services;

/// <summary>
/// Implementa rotinas de manutenção da base de conhecimento.
/// </summary>
public class KnowledgeBaseService : IKnowledgeBaseService
{
    private readonly IRepository<KnowledgeBaseArticle> _repository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Inicializa o serviço com o repositório e unidade de trabalho.
    /// </summary>
    public KnowledgeBaseService(IRepository<KnowledgeBaseArticle> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<PagedResult<KnowledgeBaseArticleDto>> GetAsync(int page, int pageSize, string? category, CancellationToken cancellationToken)
    {
        var query = _repository.Query().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(a => a.Category == category);
        }

        var total = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderBy(a => a.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(MapToDto).ToList();
        return new PagedResult<KnowledgeBaseArticleDto>(dtos, total, page, pageSize);
    }

    /// <inheritdoc />
    public async Task<KnowledgeBaseArticleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var article = await _repository.Query().AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        return article is null ? null : MapToDto(article);
    }

    /// <inheritdoc />
    public async Task<KnowledgeBaseArticleDto> CreateAsync(CreateKnowledgeBaseArticleDto dto, CancellationToken cancellationToken)
    {
        var article = new KnowledgeBaseArticle
        {
            Title = dto.Titulo,
            Category = dto.Categoria,
            Content = dto.Conteudo,
            Keywords = dto.PalavrasChave,
            IsPublished = dto.Publicado
        };

        await _repository.AddAsync(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(article);
    }

    /// <inheritdoc />
    public async Task<KnowledgeBaseArticleDto?> UpdateAsync(Guid id, UpdateKnowledgeBaseArticleDto dto, CancellationToken cancellationToken)
    {
        var article = await _repository.GetByIdAsync(id, cancellationToken);
        if (article is null)
        {
            return null;
        }

        article.Title = dto.Titulo;
        article.Category = dto.Categoria;
        article.Content = dto.Conteudo;
        article.Keywords = dto.PalavrasChave;
        article.IsPublished = dto.Publicado;
        article.UpdatedAt = DateTime.UtcNow;

        _repository.Update(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(article);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var article = await _repository.GetByIdAsync(id, cancellationToken);
        if (article is null)
        {
            return false;
        }

        _repository.Remove(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<KnowledgeBaseArticleDto>> SuggestAsync(string text, int limit, CancellationToken cancellationToken)
    {
        text = text.ToLowerInvariant();
        var items = await _repository.Query()
            .AsNoTracking()
            .Where(a => a.IsPublished &&
                        (a.Title.ToLower().Contains(text) || a.Content.ToLower().Contains(text) || a.Keywords.ToLower().Contains(text)))
            .OrderByDescending(a => a.UpdatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return items.Select(MapToDto).ToList();
    }

    private static KnowledgeBaseArticleDto MapToDto(KnowledgeBaseArticle article) => new(
        article.Id,
        article.Title,
        article.Category,
        article.Content,
        article.Keywords,
        article.IsPublished
    );
}
