using Lab10.Domain.Enums;

namespace Lab10.Application.DTOs;

public record VendaDto(
    int Id,
    int ClienteId,
    string? ClienteNome,
    VendaStatus Status,
    decimal ValorTotal,
    DateTime DataCriacao,
    DateTime? DataFinalizacao,
    string? Observacao,
    IEnumerable<ItemVendaDto> Itens,
    PagamentoDto? Pagamento);

public record VendaResumoDto(
    int Id,
    int ClienteId,
    string? ClienteNome,
    VendaStatus Status,
    decimal ValorTotal,
    int QuantidadeItens,
    DateTime DataCriacao);

public record ItemVendaDto(
    int Id,
    int ProdutoId,
    string ProdutoNome,
    int Quantidade,
    decimal PrecoUnitario,
    decimal ValorTotal);

public record IniciarVendaDto(
    int ClienteId,
    IEnumerable<ItemVendaCreateDto> Itens);

public record ItemVendaCreateDto(
    int ProdutoId,
    int Quantidade);

public record AdicionarItemDto(
    int ProdutoId,
    int Quantidade);
