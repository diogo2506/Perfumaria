using Perfumaria.Domain.Entities;

namespace Perfumaria.Application.Interfaces.Repositories;

public interface IProdutoRepository : IRepository<Produto, Guid>
{
    Task<Produto?> ObterPorSkuAsync(string sku);
    Task<IEnumerable<Produto>> ObterPorCategoriaAsync(int categoriaId);
    Task<IEnumerable<Produto>> ObterComEstoqueBaixoAsync();

    /// <summary>
    /// Obtém um produto rastreado (tracking habilitado) com o Estoque carregado,
    /// usado em fluxos que precisam ler e depois debitar a quantidade em estoque
    /// (ex.: criação de pedido).
    /// </summary>
    Task<Produto?> ObterParaVendaAsync(Guid id);
}
