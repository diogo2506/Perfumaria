using Microsoft.Extensions.Logging;
using Perfumaria.Application.DTOs.Produtos;
using Perfumaria.Application.Interfaces.Repositories;
using Perfumaria.Application.Mappers;
using Perfumaria.Domain.Entities;
using Perfumaria.Domain.Exceptions;

namespace Perfumaria.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IRepository<Categoria, int> _categoriaRepository;
    private readonly IRepository<Fabricante, int> _fabricanteRepository;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoService(
        IProdutoRepository produtoRepository,
        IRepository<Categoria, int> categoriaRepository,
        IRepository<Fabricante, int> fabricanteRepository,
        ILogger<ProdutoService> logger)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
        _fabricanteRepository = fabricanteRepository;
        _logger = logger;
    }

    public async Task<ProdutoResponseDto> CriarAsync(ProdutoRequestDto dto)
    {
        _logger.LogInformation(
            "Iniciando criação de produto. Sku={CodigoSku} CategoriaId={CategoriaId} FabricanteId={FabricanteId}",
            dto.CodigoSku, dto.CategoriaId, dto.FabricanteId);

        var categoria = await _categoriaRepository.ObterPorIdAsync(dto.CategoriaId)
            ?? throw new ResourceNotFoundException(nameof(Categoria), dto.CategoriaId);

        var fabricante = await _fabricanteRepository.ObterPorIdAsync(dto.FabricanteId)
            ?? throw new ResourceNotFoundException(nameof(Fabricante), dto.FabricanteId);

        if (await _produtoRepository.ObterPorSkuAsync(dto.CodigoSku) is not null)
            throw new ConflictException($"Já existe um produto com o SKU '{dto.CodigoSku}'.");

        var produto = dto.ToEntity();
        await _produtoRepository.AdicionarAsync(produto);

        produto.Categoria = categoria;
        produto.Fabricante = fabricante;

        _logger.LogInformation(
            "Produto criado com sucesso. ProdutoId={ProdutoId} Sku={CodigoSku}",
            produto.Id, produto.CodigoSku);

        return produto.ToResponseDto();
    }

    public async Task<ProdutoResponseDto> AtualizarAsync(Guid id, ProdutoRequestDto dto)
    {
        _logger.LogInformation("Iniciando atualização do produto {ProdutoId}", id);

        var produto = await _produtoRepository.ObterPorIdAsync(id)
            ?? throw new ResourceNotFoundException(nameof(Produto), id);

        var categoria = await _categoriaRepository.ObterPorIdAsync(dto.CategoriaId)
            ?? throw new ResourceNotFoundException(nameof(Categoria), dto.CategoriaId);

        var fabricante = await _fabricanteRepository.ObterPorIdAsync(dto.FabricanteId)
            ?? throw new ResourceNotFoundException(nameof(Fabricante), dto.FabricanteId);

        var existente = await _produtoRepository.ObterPorSkuAsync(dto.CodigoSku);
        if (existente is not null && existente.Id != id)
            throw new ConflictException($"Já existe um produto com o SKU '{dto.CodigoSku}'.");

        dto.ApplyTo(produto);
        await _produtoRepository.AtualizarAsync(produto);

        produto.Categoria = categoria;
        produto.Fabricante = fabricante;

        _logger.LogInformation("Produto {ProdutoId} atualizado com sucesso.", produto.Id);

        return produto.ToResponseDto();
    }
}
