using System.Collections.Generic;

namespace SupportSystem.Application.Common;

// Representa um resultado paginado genérico utilizado em listagens e respostas de API.
// Classe imutável contendo os itens da página atual e metadados de paginação.
public class PagedResult<T>
{
    // Coleção somente leitura com os itens retornados na página atual.
    public IReadOnlyCollection<T> Items { get; }

    // Quantidade total de itens disponíveis para a consulta.
    public long Total { get; }

    // Número da página solicitada.
    public int Page { get; }

    // Quantidade de itens por página aplicada na consulta.
    public int PageSize { get; }

    // Cria um novo resultado de paginação não modificado com itens.
    public PagedResult(IReadOnlyCollection<T> items, long total, int page, int pageSize)
    {
        Items = items; // Itens retornados na página atual
        Total = total; // Total de itens correspondentes à consulta
        Page = page; // Número da página
        PageSize = pageSize; // Quantidade de itens por página
    }
}
