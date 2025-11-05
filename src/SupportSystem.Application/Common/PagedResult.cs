namespace SupportSystem.Application.Common;

/// <summary>
/// Representa um resultado paginado utilizado em listagens.
/// </summary>
/// <typeparam name="T">Tipo de dado retornado.</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Itens retornados na página atual.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; }

    /// <summary>
    /// Quantidade total de itens encontrados.
    /// </summary>
    public long Total { get; }

    /// <summary>
    /// Número da página solicitada.
    /// </summary>
    public int Page { get; }

    /// <summary>
    /// Quantidade de itens por página aplicada.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Cria um novo resultado paginado imutável.
    /// </summary>
    public PagedResult(IReadOnlyCollection<T> items, long total, int page, int pageSize)
    {
        // Mantemos as propriedades imutáveis para simplificar o uso em cache e respostas idempotentes.
        Items = items;
        Total = total;
        Page = page;
        PageSize = pageSize;
    }
}
