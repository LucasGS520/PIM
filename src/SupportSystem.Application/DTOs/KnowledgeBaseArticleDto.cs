namespace SupportSystem.Application.DTOs;

/// <summary>
/// Representa um artigo da base de conhecimento exposto via API.
/// </summary>
public record KnowledgeBaseArticleDto(
    Guid Id,
    string Titulo,
    string Categoria,
    string Conteudo,
    string PalavrasChave,
    bool Publicado
);

/// <summary>
/// DTO utilizado para criação de novos artigos.
/// </summary>
public record CreateKnowledgeBaseArticleDto(
    string Titulo,
    string Categoria,
    string Conteudo,
    string PalavrasChave,
    bool Publicado
);

/// <summary>
/// DTO utilizado para atualização de artigos existentes.
/// </summary>
public record UpdateKnowledgeBaseArticleDto(
    string Titulo,
    string Categoria,
    string Conteudo,
    string PalavrasChave,
    bool Publicado
);
