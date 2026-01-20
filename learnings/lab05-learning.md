# Lições Aprendidas - Lab 05 CQRS + Observability com Produto

## Data: 2026-01-20

## Resumo
Implementação de API REST para cadastro de produtos com padrão CQRS usando Mvp24Hours e Observability completa usando OpenTelemetry, NLog e Health Checks.

---

## Problema 1: Interface IEntityDateLog não encontrada

### Descrição
Ao tentar usar `IEntityDateLog` do namespace `Mvp24Hours.Core.Entities`, o compilador não encontrava a interface.

### Erro
```
error CS0246: O nome do tipo ou do namespace "IEntityDateLog" não pode ser encontrado
```

### Solução
Usar apenas `IEntityBase` do namespace `Mvp24Hours.Core.Contract.Domain.Entity`:

```csharp
using Mvp24Hours.Core.Contract.Domain.Entity;

public class Produto : IEntityBase
{
    public Guid Id { get; set; }
    
    // Implementação de IEntityBase
    object IEntityBase.EntityKey => Id;
    
    // Campos de auditoria manuais
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
```

---

## Problema 2: FluentValidation Downgrade

### Descrição
Conflito de versões entre FluentValidation 11.* especificado e 12.* usado pelo Mvp24Hours.Infrastructure.Cqrs.

### Erro
```
error NU1605: Downgrade de pacote detectado: FluentValidation de 12.1.1 para 11.12.0
```

### Solução
Usar a mesma versão do FluentValidation que o Mvp24Hours:

```xml
<PackageReference Include="FluentValidation" Version="12.*" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="12.*" />
```

---

## Problema 3: OpenTelemetry Pacotes Beta/RC

### Descrição
Alguns pacotes OpenTelemetry não têm versão estável 1.*, apenas versões beta ou release candidate.

### Pacotes Afetados
- `OpenTelemetry.Instrumentation.SqlClient`
- `OpenTelemetry.Exporter.Prometheus.AspNetCore`

### Solução
Especificar versões beta/rc explicitamente:

```xml
<PackageReference Include="OpenTelemetry.Instrumentation.SqlClient" Version="1.10.0-beta.1" />
<PackageReference Include="OpenTelemetry.Exporter.Prometheus.AspNetCore" Version="1.10.0-rc.1" />
```

**Nota**: Avisos de versão (NU1603) são normais e não afetam a funcionalidade.

---

## Problema 4: Método ToBusiness não encontrado

### Descrição
O método de extensão `ToBusiness()` não existe ou está em namespace diferente.

### Erro
```
error CS1061: 'ProdutoDto' não contém uma definição para "ToBusiness"
```

### Solução
Criar BusinessResult diretamente usando o construtor:

```csharp
// ❌ INCORRETO - ToBusiness pode não existir
return dto.ToBusiness<ProdutoDto>();

// ✅ CORRETO - Usar construtor diretamente
return new BusinessResult<ProdutoDto>(dto);

// Para erros:
private static IBusinessResult<T> CreateErrorResult<T>(string message)
{
    IReadOnlyCollection<IMessageResult> messages = new List<IMessageResult>
    {
        new MessageResult(message, MessageType.Error)
    };
    return new BusinessResult<T>(default!, messages);
}
```

---

## Problema 5: IMediator do Mvp24Hours - Registração

### Descrição
O método `AddMvp24HoursCqrs()` ou `AddMvp24HoursMediator()` não encontrado.

### Solução
Criar implementação própria do IMediator que resolve handlers via DI:

```csharp
public class SimpleMediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public SimpleMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> SendAsync<TResponse>(
        IMediatorCommand<TResponse> command, 
        CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IMediatorCommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(TResponse));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)command, cancellationToken);
    }

    public async Task<TResponse> SendAsync<TResponse>(
        IMediatorQuery<TResponse> query, 
        CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IMediatorQueryHandler<,>)
            .MakeGenericType(query.GetType(), typeof(TResponse));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)query, cancellationToken);
    }
    
    // ... outros membros
}
```

### Registração Manual

```csharp
// Registrar Mediator
services.AddScoped<IMediator, SimpleMediator>();

// Registrar Command Handlers
services.AddScoped<IMediatorCommandHandler<CreateProdutoCommand, IBusinessResult<ProdutoDto>>, 
    CreateProdutoCommandHandler>();

// Registrar Query Handlers
services.AddScoped<IMediatorQueryHandler<GetProdutoByIdQuery, IBusinessResult<ProdutoDto>>, 
    GetProdutoByIdQueryHandler>();
```

---

## Problema 6: AddRuntimeInstrumentation e AddProcessInstrumentation

### Descrição
Métodos de instrumentação de métricas não encontrados.

### Solução
Esses métodos requerem pacotes adicionais. Se não precisar deles, simplesmente remova:

```csharp
// ❌ Requer pacote adicional
.AddRuntimeInstrumentation()
.AddProcessInstrumentation()

// ✅ Versão simplificada que funciona
.WithMetrics(metrics =>
{
    metrics
        .SetResourceBuilder(resourceBuilder)
        .AddMeter("Lab05.Produtos")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter();
});
```

---

## Padrões Implementados

### 1. CQRS (Command Query Responsibility Segregation)
```
Request -> Controller -> Mediator -> Handler -> Repository -> Database
```

### 2. Estrutura de Commands/Queries

```csharp
// Command
public record CreateProdutoCommand : IMediatorCommand<IBusinessResult<ProdutoDto>>
{
    public string Nome { get; init; }
    // ...
}

// Query
public record GetProdutoByIdQuery : IMediatorQuery<IBusinessResult<ProdutoDto>>
{
    public Guid Id { get; init; }
}
```

### 3. Handler Pattern

```csharp
public class CreateProdutoCommandHandler 
    : IMediatorCommandHandler<CreateProdutoCommand, IBusinessResult<ProdutoDto>>
{
    public async Task<IBusinessResult<ProdutoDto>> Handle(
        CreateProdutoCommand request, 
        CancellationToken cancellationToken)
    {
        // Implementação
    }
}
```

---

## Estrutura de Pastas Final

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
│   │   ├── Queries/
│   │   ├── Handlers/
│   │   ├── Validators/
│   │   ├── Metrics/
│   │   │   └── ProdutoMetrics.cs
│   │   └── Infrastructure/
│   │       └── SimpleMediator.cs
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

---

## Versões Utilizadas

| Pacote | Versão |
|--------|--------|
| .NET SDK | 9.0 |
| Mvp24Hours.Core | 9.* |
| Mvp24Hours.Infrastructure.Cqrs | 9.* |
| Mvp24Hours.Infrastructure.Data.EFCore | 9.* |
| Microsoft.EntityFrameworkCore.SqlServer | 9.* |
| FluentValidation | 12.* |
| OpenTelemetry.Extensions.Hosting | 1.* |
| OpenTelemetry.Instrumentation.SqlClient | 1.10.0-beta.1 |
| OpenTelemetry.Exporter.Prometheus.AspNetCore | 1.10.0-rc.1 |
| NLog.Web.AspNetCore | 5.* |
| AspNetCore.HealthChecks.SqlServer | 8.* |
| Swashbuckle.AspNetCore | 7.* |

---

## Como Executar

1. Subir infraestrutura (SQL Server + Jaeger + Prometheus):
```bash
cd labs
docker-compose up -d sqlserver jaeger prometheus grafana
```

2. Executar a API:
```bash
cd lab-05-cqrs-observability-produto/Lab05.CQRS.Observability
dotnet run --project src/Lab05.WebAPI
```

3. Endpoints disponíveis:
   - Swagger UI: `http://localhost:5000/swagger`
   - Health Check: `http://localhost:5000/health`
   - Metrics: `http://localhost:5000/metrics`

---

## Problema 7: Key property not found no Repository

### Descrição
Ao usar o repositório do Mvp24Hours, ocorre erro "Key property not found" ao tentar fazer operações CRUD.

### Erro
```
System.InvalidOperationException: Key property not found.
   at Mvp24Hours.Infrastructure.Data.EFCore.RepositoryBase`1.GetKeyInfo()
```

### Causa
O Mvp24Hours Repository precisa identificar a chave primária da entidade via Data Annotations.

### Solução
Adicionar atributo `[Key]` na propriedade de chave:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Produto : IEntityBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    
    // ...
}
```

---

## Tags
`mvp24hours` `cqrs` `observability` `opentelemetry` `nlog` `health-checks` `prometheus` `jaeger` `tracing` `metrics`
