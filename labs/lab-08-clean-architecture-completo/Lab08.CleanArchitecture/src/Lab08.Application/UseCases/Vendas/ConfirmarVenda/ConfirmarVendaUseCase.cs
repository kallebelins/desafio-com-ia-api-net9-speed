using Lab08.Application.Interfaces;
using Lab08.Domain.Exceptions;
using Lab08.Domain.Interfaces;
using Lab08.Domain.Services;

namespace Lab08.Application.UseCases.Vendas.ConfirmarVenda;

/// <summary>
/// Input para confirmar venda
/// </summary>
public record ConfirmarVendaInput(int VendaId);

/// <summary>
/// Output da confirmação de venda
/// </summary>
public record ConfirmarVendaOutput
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public string? ErrorMessage { get; init; }

    public static ConfirmarVendaOutput Ok()
        => new() { Success = true, Message = "Venda confirmada com sucesso" };

    public static ConfirmarVendaOutput Error(string message)
        => new() { Success = false, ErrorMessage = message };
}

/// <summary>
/// Use Case para confirmar uma venda (com baixa de estoque)
/// </summary>
public class ConfirmarVendaUseCase : IUseCase<ConfirmarVendaInput, ConfirmarVendaOutput>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly VendaDomainService _vendaDomainService;

    public ConfirmarVendaUseCase(IUnitOfWork unitOfWork, VendaDomainService vendaDomainService)
    {
        _unitOfWork = unitOfWork;
        _vendaDomainService = vendaDomainService;
    }

    public async Task<ConfirmarVendaOutput> ExecuteAsync(ConfirmarVendaInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            // Buscar venda com itens
            var venda = await _unitOfWork.Vendas.GetByIdWithItensAsync(input.VendaId, cancellationToken);
            if (venda == null)
                return ConfirmarVendaOutput.Error("Venda não encontrada");

            // Buscar produtos dos itens
            var produtoIds = venda.Itens.Select(i => i.ProdutoId).ToList();
            var produtos = await _unitOfWork.Produtos.GetByIdsAsync(produtoIds, cancellationToken);

            // Usar domain service para confirmar e baixar estoque
            _vendaDomainService.ConfirmarVendaComBaixaEstoque(venda, produtos);

            // Persistir alterações (venda e produtos)
            await _unitOfWork.Vendas.UpdateAsync(venda, cancellationToken);
            foreach (var produto in produtos)
            {
                await _unitOfWork.Produtos.UpdateAsync(produto, cancellationToken);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ConfirmarVendaOutput.Ok();
        }
        catch (DomainException ex)
        {
            return ConfirmarVendaOutput.Error(ex.Message);
        }
    }
}
