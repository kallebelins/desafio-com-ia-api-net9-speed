using Lab09.Application.DTOs;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab09.Application.Commands;

/// <summary>
/// Command para aplicar desconto à venda
/// </summary>
public record AplicarDescontoCommand : IMediatorCommand<VendaDto>
{
    public Guid VendaId { get; init; }
    
    /// <summary>
    /// Valor do desconto (se usar valor absoluto)
    /// </summary>
    public decimal? ValorDesconto { get; init; }
    
    /// <summary>
    /// Percentual de desconto (se usar percentual - de 0 a 100)
    /// </summary>
    public decimal? PercentualDesconto { get; init; }
    
    /// <summary>
    /// Motivo do desconto (opcional)
    /// </summary>
    public string? Motivo { get; init; }
}
