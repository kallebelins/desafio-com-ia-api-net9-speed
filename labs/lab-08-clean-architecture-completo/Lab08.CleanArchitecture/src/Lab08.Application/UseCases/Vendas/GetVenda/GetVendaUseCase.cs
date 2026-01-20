using Lab08.Application.DTOs;
using Lab08.Application.Interfaces;
using Lab08.Domain.Entities;
using Lab08.Domain.Interfaces;

namespace Lab08.Application.UseCases.Vendas.GetVenda;

/// <summary>
/// Input para buscar venda
/// </summary>
public record GetVendaInput(int Id);

/// <summary>
/// Output da busca de venda
/// </summary>
public record GetVendaOutput
{
    public bool Success { get; init; }
    public VendaDto? Venda { get; init; }
    public string? ErrorMessage { get; init; }

    public static GetVendaOutput Ok(VendaDto venda)
        => new() { Success = true, Venda = venda };

    public static GetVendaOutput NotFound()
        => new() { Success = false, ErrorMessage = "Venda não encontrada" };
}

/// <summary>
/// Use Case para buscar uma venda por ID
/// </summary>
public class GetVendaUseCase : IUseCase<GetVendaInput, GetVendaOutput>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetVendaUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetVendaOutput> ExecuteAsync(GetVendaInput input, CancellationToken cancellationToken = default)
    {
        var venda = await _unitOfWork.Vendas.GetByIdCompletoAsync(input.Id, cancellationToken);

        if (venda == null)
            return GetVendaOutput.NotFound();

        var dto = MapToDto(venda);
        return GetVendaOutput.Ok(dto);
    }

    private static VendaDto MapToDto(Venda venda)
    {
        return new VendaDto
        {
            Id = venda.Id,
            ClienteId = venda.ClienteId,
            ClienteNome = venda.Cliente?.Nome ?? string.Empty,
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
