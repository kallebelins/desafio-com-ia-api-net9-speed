using Lab08.Application.DTOs;

namespace Lab08.Application.UseCases.Vendas.CreateVenda;

/// <summary>
/// Output da criação de venda
/// </summary>
public record CreateVendaOutput
{
    public bool Success { get; init; }
    public VendaDto? Venda { get; init; }
    public string? ErrorMessage { get; init; }

    public static CreateVendaOutput Ok(VendaDto venda)
        => new() { Success = true, Venda = venda };

    public static CreateVendaOutput Error(string message)
        => new() { Success = false, ErrorMessage = message };
}
