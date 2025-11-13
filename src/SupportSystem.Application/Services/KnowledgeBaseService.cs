using System;
using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.Common;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;

namespace SupportSystem.Application.Services;

// Implementa rotinas de manutenção da base de conhecimento.
// Serviço responsável por CRUD e consultas relacionadas aos artigos.
public class KnowledgeBaseService : IKnowledgeBaseService
{
    // Repositório para operações de acesso a dados da entidade KnowledgeBaseArticle.
    private readonly IRepository<KnowledgeBaseArticle> _repository;

    // Unidade de trabalho para persistência transacional.
    private readonly IUnitOfWork _unitOfWork;

    // Inicializa o serviço com o repositório e unidade de trabalho.
    public KnowledgeBaseService(IRepository<KnowledgeBaseArticle> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository; // Repositório de artigos da base de conhecimento
        _unitOfWork = unitOfWork; // Unidade de trabalho para salvar alterações
    }

    // Obtém artigos paginados, com opção de filtrar por categoria.
    // Retorna um PagedResult contendo itens e total para paginação.
    public async Task<PagedResult<ArtigoBaseConhecimentoDto>> GetAsync(int page, int pageSize, string? category, CancellationToken cancellationToken)
    {
        // Inicia a query sem rastreamento para leitura eficiente.
        var query = _repository.Query().AsNoTracking();

        // Aplica filtro por categoria quando informado.
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(a => a.Category == category);
        }

        // Conta o total de registros que atendem ao filtro (para paginação).
        var total = await query.LongCountAsync(cancellationToken);

        // Aplica ordenação, paginação e materializa os itens.
        var items = await query
            .OrderBy(a => a.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Mapeia entidades para DTOs e retorna resultado paginado.
        var dtos = items.Select(MapToDto).ToList();
        return new PagedResult<ArtigoBaseConhecimentoDto>(dtos, total, page, pageSize);
    }

    // Obtém um artigo por Id. Retorna null se não encontrado.
    // Consulta sem rastreamento.
    public async Task<ArtigoBaseConhecimentoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var article = await _repository.Query().AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        return article is null ? null : MapToDto(article);
    }

    // Cria um novo artigo a partir do DTO fornecido e persiste no repositório.
    public async Task<ArtigoBaseConhecimentoDto> CreateAsync(CriarArtigoBaseConhecimentoDto dto, CancellationToken cancellationToken)
    {
        // Mapeia campos do DTO para a entidade.
        var article = new KnowledgeBaseArticle
        {
            Title = dto.Titulo, // Título do artigo
            Category = dto.Categoria, // Categoria do artigo
            Content = dto.Conteudo, // Conteúdo do artigo
            Keywords = dto.PalavrasChave, // Palavras-chave para busca
            IsPublished = dto.Publicado // Indica se o artigo está publicado
        };

        // Adiciona e salva a entidade.
        await _repository.AddAsync(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(article);
    }

    // Atualiza um artigo existente. 
    // Retorna null se o artigo não existir.
    public async Task<ArtigoBaseConhecimentoDto?> UpdateAsync(Guid id, AtualizarArtigoBaseConhecimentoDto dto, CancellationToken cancellationToken)
    {
        // Recupera o artigo a ser atualizado.
        var article = await _repository.GetByIdAsync(id, cancellationToken);
        if (article is null)
        {
            return null;
        }

        // Atualiza campos relevantes e marca data de atualização.
        article.Title = dto.Titulo; // Título do artigo
        article.Category = dto.Categoria; // Categoria do artigo
        article.Content = dto.Conteudo; // Conteúdo do artigo
        article.Keywords = dto.PalavrasChave; // Palavras-chave para busca
        article.IsPublished = dto.Publicado; // Indica se o artigo está publicado
        article.UpdatedAt = DateTime.UtcNow; // Atualiza a data de modificação

        _repository.Update(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(article);
    }

    // Remove um artigo por Id. 
    // Retorna true se removido, false se não encontrado.
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        // Tenta obter o artigo a ser removido.
        var article = await _repository.GetByIdAsync(id, cancellationToken);
        if (article is null)
        {
            return false;
        }

        _repository.Remove(article); // Remove o artigo do repositório
        await _unitOfWork.SaveChangesAsync(cancellationToken); // Persiste a remoção
        return true;
    }

    // Sugere artigos publicados que contenham o texto informado no título, conteúdo ou palavras-chave.
    public async Task<IReadOnlyCollection<ArtigoBaseConhecimentoDto>> SuggestAsync(string text, int limit, CancellationToken cancellationToken)
    {
        // Normaliza para comparação case-insensitive.
        text = text.ToLowerInvariant();

        // Filtra apenas artigos publicados que contenham o texto em campos relevantes.
        var items = await _repository.Query()
            .AsNoTracking()
            .Where(a => a.IsPublished &&
                        (a.Title.ToLower().Contains(text) || a.Content.ToLower().Contains(text) || a.Keywords.ToLower().Contains(text)))
            .OrderByDescending(a => a.UpdatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return items.Select(MapToDto).ToList();
    }

    // Mapeia a entidade KnowledgeBaseArticle para seu DTO correspondente.
    private static ArtigoBaseConhecimentoDto MapToDto(KnowledgeBaseArticle article) => new(
        article.Id,
        article.Title,
        article.Category,
        article.Content,
        article.Keywords,
        article.IsPublished
    );
}
