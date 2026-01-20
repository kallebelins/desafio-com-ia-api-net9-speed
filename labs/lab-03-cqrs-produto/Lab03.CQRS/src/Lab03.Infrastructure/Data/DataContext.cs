using Lab03.Core.Entities;
using Lab03.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Infrastructure.Data.EFCore;

namespace Lab03.Infrastructure.Data;

/// <summary>
/// Contexto do banco de dados para o Lab03
/// </summary>
public class DataContext : Mvp24HoursContext
{
    public DataContext(DbContextOptions<DataContext> options) 
        : base(options)
    {
    }

    public DbSet<Produto> Produtos => Set<Produto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Aplica as configurações de entidades
        modelBuilder.ApplyConfiguration(new ProdutoConfiguration());
    }
}
