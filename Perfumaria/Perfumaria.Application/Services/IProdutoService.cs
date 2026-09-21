using Perfumaria.Application.DTOs.Produtos;

namespace Perfumaria.Application.Services;

/// <summary>
/// Serviço de aplicação para criação e atualização de produtos (perfumes),
/// concentrando as validações de negócio que envolvem múltiplos repositórios
/// (existência de Categoria/Fabricante, unicidade de SKU) fora do controller.
/// </summary>
public interface IProdutoService
{
    Task<ProdutoResponseDto> CriarAsync(ProdutoRequestDto dto);
    Task<ProdutoResponseDto> AtualizarAsync(Guid id, ProdutoRequestDto dto);
}
