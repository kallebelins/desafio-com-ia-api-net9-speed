# Lab 05 - CQRS + Observability com Cadastro de Produto

## 🎯 Objetivo
Criar uma API REST para cadastro de produtos com **CQRS** e implementar **Observability** completa usando OpenTelemetry, Logging estruturado e Health Checks.

## 📋 Requisito de Negócio
- **Entidade**: Produto
- **Campos**: Id, Nome, Descrição, Preço, SKU, Categoria, Ativo
- **Observabilidade**: Logs, Traces, Métricas e Health Checks

## 🏗️ Arquitetura
**CQRS + Observability** - Separação de leitura/escrita com monitoramento completo.

```
Lab05.CQRS.Observability/
├── Lab05.CQRS.Observability.sln
├── src/
│   ├── Lab05.Core/
│   │   ├── Entities/
│   │   │   └── Produto.cs
│   │   └── ValueObjects/
│   │       └── ProdutoDto.cs
│   │
│   ├── Lab05.Application/
│   │   ├── Commands/
│   │   │   └── CreateProdutoCommand.cs
│   │   ├── Queries/
│   │   │   └── GetProdutoByIdQuery.cs
│   │   ├── Handlers/
│   │   │   └── ...
│   │   ├── Behaviors/
│   │   │   ├── LoggingBehavior.cs
│   │   │   ├── ValidationBehavior.cs
│   │   │   └── TracingBehavior.cs
│   │   └── Metrics/
│   │       └── ProdutoMetrics.cs
│   │
│   ├── Lab05.Infrastructure/
│   │   ├── Data/
│   │   │   └── DataContext.cs
│   │   └── Observability/
│   │       ├── OpenTelemetrySetup.cs
│   │       └── CustomActivitySource.cs
│   │
│   └── Lab05.WebAPI/
│       ├── Program.cs
│       ├── NLog.config
│       ├── Controllers/
│       │   └── ProdutoController.cs
│       ├── Middlewares/
│       │   ├── CorrelationIdMiddleware.cs
│       │   └── ExceptionMiddleware.cs
│       └── Extensions/
│           ├── ServiceBuilderExtensions.cs
│           └── ObservabilityExtensions.cs
```

## 🔧 Recursos Utilizados

| Recurso | Descrição |
|---------|-----------|
| **CQRS** | Commands e Queries separados |
| **OpenTelemetry** | Tracing distribuído |
| **NLog** | Logging estruturado |
| **Prometheus** | Métricas da aplicação |
| **Health Checks** | Monitoramento de saúde |
| **Pipeline Behaviors** | Logging e Tracing automáticos |

## 📦 Pacotes NuGet

```xml
<!-- Observability -->
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.*" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.*" />
<PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.*" />
<PackageReference Include="OpenTelemetry.Instrumentation.SqlClient" Version="1.*" />
<PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.*" />
<PackageReference Include="OpenTelemetry.Exporter.Prometheus.AspNetCore" Version="1.*" />

<!-- Logging -->
<PackageReference Include="NLog.Web.AspNetCore" Version="5.*" />

<!-- Health Checks -->
<PackageReference Include="AspNetCore.HealthChecks.UI.Client" Version="8.*" />
<PackageReference Include="AspNetCore.HealthChecks.SqlServer" Version="8.*" />
```

## 🔐 Credenciais e Configuração

Este lab utiliza os serviços do `docker-compose.yml` principal localizado em `../docker-compose.yml`.

### Serviços Utilizados

| Serviço | Host | Porta | Credenciais |
|---------|------|-------|-------------|
| **SQL Server** | `sqlserver` | `1433` | Usuário: `sa`<br>Senha: `Lab@Mvp24Hours!` |
| **Jaeger** | `jaeger` | `16686` (UI)<br>`4317` (OTLP gRPC)<br>`4318` (OTLP HTTP) | Sem autenticação |
| **Prometheus** | `prometheus` | `9090` | Sem autenticação |
| **Grafana** | `grafana` | `3000` | Usuário: `admin`<br>Senha: `admin` |

### String de Conexão

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver;Database=Lab05_Produtos;User Id=sa;Password=Lab@Mvp24Hours!;TrustServerCertificate=True;"
  },
  "OpenTelemetry": {
    "JaegerEndpoint": "http://jaeger:4317"
  },
  "Prometheus": {
    "Endpoint": "http://prometheus:9090"
  }
}
```

### Executar Infraestrutura

```bash
# Na pasta labs/
cd ..
docker-compose up -d sqlserver jaeger prometheus grafana
```

## 📚 Swagger

Este laboratório inclui documentação automática da API via Swagger.

### Configuração

**No arquivo `Program.cs`:**
```csharp
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Lab05 CQRS + Observability - Produtos", Version = "v1" });
});

// ... resto do código ...

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab05 CQRS + Observability v1"));
}
```

**No arquivo `.csproj` (projeto WebAPI):**
```xml
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.*" />
```

### Acessar Swagger UI

Após iniciar a aplicação, acesse:
- **Swagger UI**: `http://localhost:5000/swagger` ou `https://localhost:5001/swagger`
- **Swagger JSON**: `http://localhost:5000/swagger/v1/swagger.json`

## 📊 Observability Stack

```
┌─────────────────────────────────────────────────────────────┐
│                    Observability                             │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│   ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    │
│   │   Logging   │    │   Tracing   │    │   Metrics   │    │
│   │   (NLog)    │    │ (Activity)  │    │   (Meter)   │    │
│   └──────┬──────┘    └──────┬──────┘    └──────┬──────┘    │
│          │                  │                  │            │
│          ▼                  ▼                  ▼            │
│   ┌───────────┐      ┌───────────┐      ┌───────────┐      │
│   │  Console  │      │  Jaeger   │      │Prometheus │      │
│   │  Files    │      │  (OTLP)   │      │  Grafana  │      │
│   └───────────┘      └───────────┘      └───────────┘      │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

## 📝 Pipeline Behavior com Logging

```csharp
public class LoggingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation(
            "Handling {RequestName} {@Request}", 
            requestName, request);

        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        _logger.LogInformation(
            "Handled {RequestName} in {ElapsedMs}ms", 
            requestName, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
```

## 📝 Custom Metrics

```csharp
public class ProdutoMetrics
{
    private readonly Counter<long> _produtosCriados;
    private readonly Histogram<double> _operationDuration;

    public ProdutoMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("Lab05.Produtos");

        _produtosCriados = meter.CreateCounter<long>(
            "produtos_criados_total",
            description: "Total de produtos criados");

        _operationDuration = meter.CreateHistogram<double>(
            "produto_operation_duration_seconds",
            unit: "s",
            description: "Duração das operações de produto");
    }

    public void RecordProdutoCriado() => _produtosCriados.Add(1);
    public void RecordDuration(double seconds) => _operationDuration.Record(seconds);
}
```

## ✅ Checklist de Implementação

- [ ] Criar estrutura de projetos CQRS
- [ ] Configurar OpenTelemetry (Tracing + Metrics)
- [ ] Configurar NLog para logging estruturado
- [ ] Implementar CorrelationIdMiddleware
- [ ] Implementar ExceptionMiddleware
- [ ] Criar LoggingBehavior
- [ ] Criar TracingBehavior com ActivitySource
- [ ] Implementar métricas customizadas
- [ ] Configurar Health Checks (SQL Server, Memory)
- [ ] Configurar endpoints (/health, /metrics)
- [ ] Testar com Jaeger/Prometheus

## 💡 Conceitos Aprendidos

1. Three Pillars of Observability (Logs, Traces, Metrics)
2. OpenTelemetry para tracing distribuído
3. ActivitySource e Activity para criar spans
4. Pipeline Behaviors para cross-cutting concerns
5. Correlation ID para rastreamento de requests
6. Health Checks para Kubernetes readiness/liveness

## 📖 Lições Aprendidas Compartilhadas

Este lab pode utilizar lições aprendidas de outros labs para resolver problemas mais rapidamente. Consulte a pasta `../../learnings/` para acessar documentações de problemas comuns e soluções já implementadas.

**Localização da pasta:** `learnings/` (na raiz do projeto)

**Como usar:**
- Ao encontrar um problema ou erro, pesquise na pasta `learnings/` por documentações relacionadas
- As lições aprendidas incluem problemas comuns, soluções e boas práticas
- Exemplos: configurações de banco de dados, conflitos com frameworks, padrões de implementação

## 🔗 Ferramentas MCP Utilizadas

```
mvp24h_cqrs_guide({ topic: "behaviors" })
mvp24h_observability_setup({ component: "overview" })
mvp24h_observability_setup({ component: "tracing" })
mvp24h_observability_setup({ component: "metrics" })
mvp24h_observability_setup({ component: "logging" })
```

---
**Nível de Complexidade**: ⭐⭐⭐⭐ Avançado+
