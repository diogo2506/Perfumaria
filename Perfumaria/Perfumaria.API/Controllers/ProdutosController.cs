using Microsoft.AspNetCore.Mvc;
using Perfumaria.Application.DTOs.Produtos;
using Perfumaria.Application.Interfaces.Repositories;
using Perfumaria.Application.Mappers;
using Perfumaria.Application.Services;
using Perfumaria.Domain.Entities;
using Perfumaria.Domain.Exceptions;

namespace Perfumaria.API.Controllers;

/// <summary>
/// Produtos (perfumes) do catálogo. Usa o repositório específico <c>IProdutoRepository</c>
/// (que estende o repositório genérico) para leitura, e delega a criação/atualização
/// (que envolve validar Categoria, Fabricante e unicidade de SKU) ao
/// <see cref="IProdutoService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoRepository produtoRepository, IProdutoService produtoService)
    {
        _produtoRepository = produtoRepository;
        _produtoService = produtoService;
    }

    /// <summary>Lista todos os produtos, opcionalmente filtrando por categoria.</summary>
    /// <param name="categoriaId">Filtra produtos de uma categoria específica.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarAsync([FromQuery] int? categoriaId)
    {
        var produtos = categoriaId.HasValue
            ? await _produtoRepository.ObterPorCategoriaAsync(categoriaId.Value)
            : await _produtoRepository.ObterTodosAsync();

        return Ok(produtos.Select(p => p.ToResponseDto()));
    }

    /// <summary>Busca um produto pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorIdAsync(Guid id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id)
            ?? throw new ResourceNotFoundException(nameof(Produto), id);

        return Ok(produto.ToResponseDto());
    }

    /// <summary>Busca um produto pelo código SKU.</summary>
    [HttpGet("sku/{sku}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorSkuAsync(string sku)
    {
        var produto = await _produtoRepository.ObterPorSkuAsync(sku)
            ?? throw new ResourceNotFoundException($"Produto com SKU '{sku}' não foi encontrado.");

        return Ok(produto.ToResponseDto());
    }

    /// <summary>Lista produtos com estoque abaixo do mínimo configurado.</summary>
    [HttpGet("estoque-baixo")]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarComEstoqueBaixoAsync()
    {
        var produtos = await _produtoRepository.ObterComEstoqueBaixoAsync();
        return Ok(produtos.Select(p => p.ToResponseDto()));
    }

    /// <summary>Cadastra um novo produto (perfume).</summary>
    /// <remarks>
    /// Valida a existência de Categoria e Fabricante e a unicidade do CodigoSku
    /// antes de persistir.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CriarAsync([FromBody] ProdutoRequestDto dto)
    {
        var criado = await _produtoService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorIdAsync), new { id = criado.Id }, criado);
    }

    /// <summary>Atualiza um produto existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AtualizarAsync(Guid id, [FromBody] ProdutoRequestDto dto)
    {
        var atualizado = await _produtoService.AtualizarAsync(id, dto);
        return Ok(atualizado);
    }

    /// <summary>Remove um produto pelo id.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverAsync(Guid id)
    {
        if (!await _produtoRepository.ExisteAsync(id))
            throw new ResourceNotFoundException(nameof(Produto), id);

        await _produtoRepository.RemoverAsync(id);
        return NoContent();
    }
}
