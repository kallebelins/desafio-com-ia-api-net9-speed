using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Infrastructure.Data.EFCore;
using Lab01.MinimalApi.Entities;

namespace Lab01.MinimalApi.Data;

public class DataContext : Mvp24HoursContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Produto> Produtos => Set<Produto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }
}
