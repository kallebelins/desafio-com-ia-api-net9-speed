using Lab07.Core.Entities;
using Lab07.Core.Enums;
using Lab07.Core.ValueObjects;
using Microsoft.Extensions.Logging;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;

namespace Lab07.Application.Services;

/// <summary>
/// Implementação do serviço de vendas
/// </summary>
public class VendaService : IVendaService
{
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly ILogger<VendaService> _logger;

    public VendaService(
        IUnitOfWorkAsync unitOfWork,
        ILogger<VendaService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<VendaDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Venda>();
        var paging = new Mvp24Hours.Core.ValueObjects.Logic.PagingCriteria(
            1, 0, navigation: new List<string> { "Itens", "Cliente" });
        
        var vendas = await repository.GetByAsync(v => v.Id == id, paging);
        var venda = vendas.FirstOrDefault();
        
        if (venda == null) return null;

        return MapToDto(venda);
    }

    public async Task<IEnumerable<VendaDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Venda>();
        var paging = new Mvp24Hours.Core.ValueObjects.Logic.PagingCriteria(
            100, 0, navigation: new List<string> { "Itens", "Cliente" });
        
        var vendas = await repository.ListAsync(paging);
        
        return vendas.Where(v => v.Removed == null).Select(MapToDto);
    }

    public async Task<IEnumerable<VendaDto>> GetByClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Venda>();
        var paging = new Mvp24Hours.Core.ValueObjects.Logic.PagingCriteria(
            100, 0, navigation: new List<string> { "Itens", "Cliente" });
        
        var vendas = await repository.GetByAsync(v => v.ClienteId == clienteId && v.Removed == null, paging);
        
        return vendas.Select(MapToDto);
    }

    public async Task<VendaDto?> CriarVendaAsync(Guid vendaId, Guid clienteId, List<ItemVendaDto> itens, decimal valorTotal, CancellationToken cancellationToken = default)
    {
        var vendaRepository = _unitOfWork.GetRepository<Venda>();
        var itemRepository = _unitOfWork.GetRepository<ItemVenda>();

        var venda = new Venda
        {
            Id = vendaId,
            ClienteId = clienteId,
            ValorTotal = valorTotal,
            Status = VendaStatus.Confirmada,
            DataConfirmacao = DateTime.UtcNow,
            Created = DateTime.UtcNow
        };

        await vendaRepository.AddAsync(venda);

        foreach (var item in itens)
        {
            var itemVenda = new ItemVenda
            {
                Id = Guid.NewGuid(),
                VendaId = vendaId,
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,
                Created = DateTime.UtcNow
            };

            await itemRepository.AddAsync(itemVenda);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Venda {VendaId} criada para cliente {ClienteId}. Valor: {ValorTotal:C}",
            vendaId, clienteId, valorTotal);

        return await GetByIdAsync(vendaId, cancellationToken);
    }

    public async Task<bool> ConfirmarVendaAsync(Guid vendaId, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Venda>();
        var venda = await repository.GetByIdAsync(vendaId);
        
        if (venda == null) return false;

        venda.Confirmar();
        venda.Modified = DateTime.UtcNow;

        await repository.ModifyAsync(venda);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Venda {VendaId} confirmada", vendaId);

        return true;
    }

    public async Task<bool> CancelarVendaAsync(Guid vendaId, string motivo, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Venda>();
        var venda = await repository.GetByIdAsync(vendaId);
        
        if (venda == null) return false;

        venda.Cancelar(motivo);
        venda.Modified = DateTime.UtcNow;

        await repository.ModifyAsync(venda);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Venda {VendaId} cancelada: {Motivo}", vendaId, motivo);

        return true;
    }

    private static VendaDto MapToDto(Venda venda) => new()
    {
        Id = venda.Id,
        ClienteId = venda.ClienteId,
        ClienteNome = venda.Cliente?.Nome,
        ValorTotal = venda.ValorTotal,
        Status = venda.Status,
        MotivoFalha = venda.MotivoFalha,
        DataConfirmacao = venda.DataConfirmacao,
        DataCancelamento = venda.DataCancelamento,
        Created = venda.Created,
        Itens = venda.Itens.Select(i => new ItemVendaDto
        {
            Id = i.Id,
            ProdutoId = i.ProdutoId,
            ProdutoNome = i.Produto?.Nome,
            Quantidade = i.Quantidade,
            PrecoUnitario = i.PrecoUnitario,
            ValorTotal = i.ValorTotal
        }).ToList()
    };
}
