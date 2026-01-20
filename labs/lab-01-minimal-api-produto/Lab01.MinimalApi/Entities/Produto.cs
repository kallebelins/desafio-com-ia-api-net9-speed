using Mvp24Hours.Core.Entities;

namespace Lab01.MinimalApi.Entities;

public class Produto : EntityBase<int>
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
