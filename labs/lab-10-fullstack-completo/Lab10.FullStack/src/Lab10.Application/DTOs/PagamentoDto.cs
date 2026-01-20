using Lab10.Domain.Enums;

namespace Lab10.Application.DTOs;

public record PagamentoDto(
    int Id,
    int VendaId,
    decimal Valor,
    MetodoPagamento Metodo,
    PagamentoStatus Status,
    string? TransacaoId,
    DateTime DataCriacao,
    DateTime? DataProcessamento,
    string? MotivoFalha);

public record ProcessarPagamentoDto(
    int VendaId,
    MetodoPagamento Metodo);
