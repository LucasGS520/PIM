using System;

namespace SupportSystem.Application.DTOs;


// Conjunto de DTOs utilizados para transportar informações de artigos da Base de Conhecimento entre a API e as camadas internas da aplicação.
// Retornar dados ao consumidor da API.
public record ArtigoBaseConhecimentoDto(
    Guid Id,
    string Titulo,
    string Categoria,
    string Conteudo,
    string PalavrasChave,
    bool Publicado
);


// DTO utilizado para criação de novos artigos.
// payload recebido em endpoints POST para criar um novo artigo.
// Obs: validações (ex.: títulos obrigatórios, tamanho máximo) devem ser aplicadas na camada de aplicação.
public record CriarArtigoBaseConhecimentoDto(
    string Titulo, // Título do novo artigo
    string Categoria, // Categoria atribuída ao novo artigo
    string Conteudo, // Conteúdo/descrição do artigo
    string PalavrasChave, // Palavras-chave para facilitar buscas
    bool Publicado // Indica se o artigo deve ser criado já publicado ou como rascunho
);

// DTO utilizado para atualização de artigos existentes.
// payload recebido em endpoints PUT/PATCH para alterar um artigo existente.
// Obs: O identificador do artigo a ser atualizado normalmente é passado na rota
public record AtualizarArtigoBaseConhecimentoDto(
    string Titulo, // Novo título do artigo ou sem alteração
    string Categoria, // Nova categoria do artigo
    string Conteudo, // Novo conteúdo do artigo
    string PalavrasChave, // Atualização das palavras-chave
    bool Publicado // Indica se o artigo ficará publicado após a atualização
);
