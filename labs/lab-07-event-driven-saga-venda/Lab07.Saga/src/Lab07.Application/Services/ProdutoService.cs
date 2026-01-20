using Lab07.Core.Entities;
using Lab07.Core.ValueObjects;
using Microsoft.Extensions.Logging;
using Mvp24Hours.Core.Contract.Data;

namespace Lab07.Application.Services;

/// <summary>
/// Implementação do serviço de produtos
/// </summary>
public class ProdutoService : IProdutoService
{
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoService(
        IUnitOfWorkAsync unitOfWork,
        ILogger<ProdutoService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ProdutoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Produto>();
        var produto = await repository.GetByIdAsync(id);
        
        if (produto == null) return null;

        return MapToDto(produto);
    }

    public async Task<IEnumerable<ProdutoDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Produto>();
        var produtos = await repository.ListAsync();
        
        return produtos.Where(p => p.Removed == null).Select(MapToDto);
    }

    public async Task<ProdutoDto> CreateAsync(string nome, string descricao, decimal preco, int estoque, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Produto>();
        
        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Descricao = descricao,
            Preco = preco,
            Estoque = estoque,
            EstoqueReservado = 0,
            Ativo = true,
            Created = DateTime.UtcNow
        };

        await repository.AddAsync(produto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Produto {ProdutoId} criado: {Nome}", produto.Id, nome);

        return MapToDto(produto);
    }

    public async Task<ProdutoDto?> UpdateAsync(Guid id, string nome, string descricao, decimal preco, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Produto>();
        var produto = await repository.GetByIdAsync(id);
        
        if (produto == null) return null;

        produto.Nome = nome;
        produto.Descricao = descricao;
        produto.Preco = preco;
        produto.Modified = DateTime.UtcNow;

        await repository.ModifyAsync(produto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Produto {ProdutoId} atualizado", id);

        return MapToDto(produto);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Produto>();
        var produto = await repository.GetByIdAsync(id);
        
        if (produto == null) return false;

        produto.Removed = DateTime.UtcNow;
        produto.Ativo = false;

        await repository.ModifyAsync(produto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Produto {ProdutoId} removido", id);

        return true;
    }

    public async Task<bool> AtivarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Produto>();
        var produto = await repository.GetByIdAsync(id);
        
        if (produto == null) return false;

        produto.Ativo = true;
        produto.Modified = DateTime.UtcNow;

        await repository.ModifyAsync(produto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Produto {ProdutoId} ativado", id);

        return true;
    }

    public async Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Produto>();
        var produto = await repository.GetByIdAsync(id);
        
        if (produto == null) return false;

        produto.Ativo = false;
        produto.Modified = DateTime.UtcNow;

        await repository.ModifyAsync(produto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Produto {ProdutoId} desativado", id);

        return true;
    }

    public async Task<bool> ReservarEstoqueAsync(Guid produtoId, int quantidade, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Produto>();
        var produto = await repository.GetByIdAsync(produtoId);
        
        if (produto == null) return false;

        if (!produto.ReservarEstoque(quantidade))
        {
            _logger.LogWarning(
                "Falha ao reservar estoque do produto {ProdutoId}. Disponível: {Disponivel}, Solicitado: {Quantidade}",
                produtoId, produto.EstoqueDisponivel, quantidade);
            return false;
        }

        produto.Modified = DateTime.UtcNow;

        await repository.ModifyAsync(produto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Estoque reservado: Produto {ProdutoId}, Quantidade {Quantidade}",
            produtoId, quantidade);

        return true;
    }

    public async Task<bool> LiberarReservaAsync(Guid produtoId, int quantidade, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Produto>();
        var produto = await repository.GetByIdAsync(produtoId);
        
        if (produto == null) return false;

        produto.LiberarReserva(quantidade);
        produto.Modified = DateTime.UtcNow;

        await repository.ModifyAsync(produto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Reserva liberada: Produto {ProdutoId}, Quantidade {Quantidade}",
            produtoId, quantidade);

        return true;
    }

    public async Task<bool> ConfirmarReservaAsync(Guid produtoId, int quantidade, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Produto>();
        var produto = await repository.GetByIdAsync(produtoId);
        
        if (produto == null) return false;

        produto.ConfirmarReserva(quantidade);
        produto.Modified = DateTime.UtcNow;

        await repository.ModifyAsync(produto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Reserva confirmada: Produto {ProdutoId}, Quantidade {Quantidade}",
            produtoId, quantidade);

        return true;
    }

    public async Task<bool> AdicionarEstoqueAsync(Guid produtoId, int quantidade, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Produto>();
        var produto = await repository.GetByIdAsync(produtoId);
        
        if (produto == null) return false;

        produto.Estoque += quantidade;
        produto.Modified = DateTime.UtcNow;

        await repository.ModifyAsync(produto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Estoque adicionado: Produto {ProdutoId}, Quantidade {Quantidade}",
            produtoId, quantidade);

        return true;
    }

    private static ProdutoDto MapToDto(Produto produto) => new()
    {
        Id = produto.Id,
        Nome = produto.Nome,
        Descricao = produto.Descricao,
        Preco = produto.Preco,
        Estoque = produto.Estoque,
        EstoqueReservado = produto.EstoqueReservado,
        EstoqueDisponivel = produto.EstoqueDisponivel,
        Ativo = produto.Ativo,
        Created = produto.Created
    };
}
