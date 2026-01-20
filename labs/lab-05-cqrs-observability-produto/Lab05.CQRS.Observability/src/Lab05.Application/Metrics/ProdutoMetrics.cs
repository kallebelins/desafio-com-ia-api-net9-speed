using System.Diagnostics.Metrics;

namespace Lab05.Application.Metrics;

/// <summary>
/// Métricas customizadas para operações de produto
/// </summary>
public class ProdutoMetrics
{
    private readonly Counter<long> _produtosCriados;
    private readonly Counter<long> _produtosAtualizados;
    private readonly Counter<long> _produtosDeletados;
    private readonly Histogram<double> _operationDuration;

    public ProdutoMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("Lab05.Produtos");

        _produtosCriados = meter.CreateCounter<long>(
            "produtos_criados_total",
            description: "Total de produtos criados");

        _produtosAtualizados = meter.CreateCounter<long>(
            "produtos_atualizados_total",
            description: "Total de produtos atualizados");

        _produtosDeletados = meter.CreateCounter<long>(
            "produtos_deletados_total",
            description: "Total de produtos deletados");

        _operationDuration = meter.CreateHistogram<double>(
            "produto_operation_duration_seconds",
            unit: "s",
            description: "Duração das operações de produto em segundos");
    }

    public void RecordProdutoCriado() => _produtosCriados.Add(1);
    public void RecordProdutoAtualizado() => _produtosAtualizados.Add(1);
    public void RecordProdutoDeletado() => _produtosDeletados.Add(1);
    public void RecordDuration(double seconds) => _operationDuration.Record(seconds);
}
