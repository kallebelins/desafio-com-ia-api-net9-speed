using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.Data.EFCore;
using Lab01.MinimalApi.Entities;
using Lab01.MinimalApi.ValueObjects;

namespace Lab01.MinimalApi.Endpoints;

public static class ProdutoEndpoints
{
    public static void MapProdutoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/produtos")
            .WithTags("Produtos");

        group.MapGet("/", GetAllAsync);
        group.MapGet("/{id:int}", GetByIdAsync);
        group.MapPost("/", CreateAsync);
        group.MapPut("/{id:int}", UpdateAsync);
        group.MapDelete("/{id:int}", DeleteAsync);
    }

    private static async Task<IResult> GetAllAsync(
        [FromServices] IUnitOfWorkAsync uow)
    {
        var repository = uow.GetRepository<Produto>();
        var produtos = await repository.ListAsync();
        
        var produtosDto = produtos.Select(p => new ProdutoDto(
            p.Id,
            p.Nome,
            p.Descricao,
            p.Preco,
            p.Ativo,
            p.DataCriacao
        )).ToList();
        
        return Results.Ok(produtosDto);
    }

    private static async Task<IResult> GetByIdAsync(
        int id,
        [FromServices] IUnitOfWorkAsync uow)
    {
        var repository = uow.GetRepository<Produto>();
        var produto = await repository.GetByIdAsync(id);
        
        if (produto == null)
            return Results.NotFound();
        
        var produtoDto = new ProdutoDto(
            produto.Id,
            produto.Nome,
            produto.Descricao,
            produto.Preco,
            produto.Ativo,
            produto.DataCriacao
        );
            
        return Results.Ok(produtoDto);
    }

    private static async Task<IResult> CreateAsync(
        [FromBody] ProdutoCreateDto dto,
        [FromServices] IValidator<ProdutoCreateDto> validator,
        [FromServices] IUnitOfWorkAsync uow)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList();
            return Results.BadRequest(errors);
        }

        var produto = new Produto
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Preco = dto.Preco,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        var repository = uow.GetRepository<Produto>();
        await repository.AddAsync(produto);
        await uow.SaveChangesAsync();

        var produtoDto = new ProdutoDto(
            produto.Id,
            produto.Nome,
            produto.Descricao,
            produto.Preco,
            produto.Ativo,
            produto.DataCriacao
        );

        return Results.Created($"/api/produtos/{produto.Id}", produtoDto);
    }

    private static async Task<IResult> UpdateAsync(
        int id,
        [FromBody] ProdutoUpdateDto dto,
        [FromServices] IValidator<ProdutoUpdateDto> validator,
        [FromServices] IUnitOfWorkAsync uow)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList();
            return Results.BadRequest(errors);
        }

        var repository = uow.GetRepository<Produto>();
        var produto = await repository.GetByIdAsync(id);
        
        if (produto == null)
            return Results.NotFound();

        produto.Nome = dto.Nome;
        produto.Descricao = dto.Descricao;
        produto.Preco = dto.Preco;
        produto.Ativo = dto.Ativo;

        await repository.ModifyAsync(produto);
        await uow.SaveChangesAsync();

        var produtoDto = new ProdutoDto(
            produto.Id,
            produto.Nome,
            produto.Descricao,
            produto.Preco,
            produto.Ativo,
            produto.DataCriacao
        );

        return Results.Ok(produtoDto);
    }

    private static async Task<IResult> DeleteAsync(
        int id,
        [FromServices] IUnitOfWorkAsync uow)
    {
        var repository = uow.GetRepository<Produto>();
        var produto = await repository.GetByIdAsync(id);
        
        if (produto == null)
            return Results.NotFound();

        await repository.RemoveAsync(produto);
        await uow.SaveChangesAsync();

        return Results.NoContent();
    }
}
