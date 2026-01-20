using Lab08.Application.DTOs;
using Lab08.Application.Interfaces;
using Lab08.Domain.Entities;
using Lab08.Domain.Exceptions;
using Lab08.Domain.Interfaces;
using Lab08.Domain.Services;

namespace Lab08.Application.UseCases.Vendas.CreateVenda;

/// <summary>
/// Use Case para criar uma nova venda
/// </summary>
public class CreateVendaUseCase : IUseCase<CreateVendaInput, CreateVendaOutput>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly VendaDomainService _vendaDomainService;

    public CreateVendaUseCase(IUnitOfWork unitOfWork, VendaDomainService vendaDomainService)
    {
        _unitOfWork = unitOfWork;
        _vendaDomainService = vendaDomainService;
    }

    public async Task<CreateVendaOutput> ExecuteAsync(CreateVendaInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            // Buscar cliente
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(input.ClienteId, cancellationToken);
            if (cliente == null)
                return CreateVendaOutput.Error("Cliente não encontrado");

            // Buscar produtos
            var produtoIds = input.Itens.Select(i => i.ProdutoId).ToList();
            var produtos = await _unitOfWork.Produtos.GetByIdsAsync(produtoIds, cancellationToken);

            // Validar se todos os produtos foram encontrados
            var produtosDict = produtos.ToDictionary(p => p.Id);
            foreach (var itemInput in input.Itens)
            {
                if (!produtosDict.ContainsKey(itemInput.ProdutoId))
                    return CreateVendaOutput.Error($"Produto com ID {itemInput.ProdutoId} não encontrado");
            }

            // Montar lista de itens para o domain service
            var itens = input.Itens.Select(i => (produtosDict[i.ProdutoId], i.Quantidade));

            // Criar venda usando domain service
            var venda = _vendaDomainService.CriarVenda(cliente, itens);

            // Adicionar observação se houver
            if (!string.IsNullOrWhiteSpace(input.Observacao))
                venda.AdicionarObservacao(input.Observacao);

            // Persistir
            await _unitOfWork.Vendas.AddAsync(venda, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Mapear para DTO
            var dto = MapToDto(venda, cliente);

            return CreateVendaOutput.Ok(dto);
        }
        catch (DomainException ex)
        {
            return CreateVendaOutput.Error(ex.Message);
        }
    }

    private static VendaDto MapToDto(Venda venda, Cliente cliente)
    {
        return new VendaDto
        {
            Id = venda.Id,
            ClienteId = venda.ClienteId,
            ClienteNome = cliente.Nome,
            DataVenda = venda.DataVenda,
            Status = venda.Status.ToString(),
            Total = venda.Total.Valor,
            TotalFormatado = venda.Total.ToStringFormatado(),
            Observacao = venda.Observacao,
            Itens = venda.Itens.Select(i => new ItemVendaDto
            {
                Id = i.Id,
                ProdutoId = i.ProdutoId,
                ProdutoNome = i.Produto?.Nome ?? string.Empty,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario.Valor,
                PrecoUnitarioFormatado = i.PrecoUnitario.ToStringFormatado(),
                Subtotal = i.Subtotal.Valor,
                SubtotalFormatado = i.Subtotal.ToStringFormatado()
            }).ToList()
        };
    }
}
