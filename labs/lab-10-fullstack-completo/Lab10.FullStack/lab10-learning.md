# Lab 10 - Full Stack Completo - Lições Aprendidas

## 1. Pacotes NuGet com Versões Prerelease

### Problema
Alguns pacotes OpenTelemetry ainda não possuem versões estáveis (1.0+):
- `OpenTelemetry.Instrumentation.SqlClient` - versão mais recente é `1.14.0-rc.1`
- `OpenTelemetry.Exporter.Prometheus.AspNetCore` - versão mais recente é `1.14.0-beta.1`

### Solução
Especificar explicitamente a versão prerelease no `.csproj`:

```xml
<PackageReference Include="OpenTelemetry.Instrumentation.SqlClient" Version="1.14.0-rc.1" />
<PackageReference Include="OpenTelemetry.Exporter.Prometheus.AspNetCore" Version="1.14.0-beta.1" />
```

---

## 2. BusinessResult no Mvp24Hours

### Problema
O `BusinessResult<T>` do Mvp24Hours NÃO possui métodos fluent como `SetData()` ou `AddMessage()`.

### Solução Correta
Usar os construtores do `BusinessResult<T>`:

```csharp
// Para retornar sucesso com dados:
return new BusinessResult<ProdutoDto>(dto);

// Para retornar erro:
IReadOnlyCollection<IMessageResult> messages = new List<IMessageResult>
{
    new MessageResult("Mensagem de erro", MessageType.Error)
};
return new BusinessResult<T>(default!, messages);
```

### Verificação de Resultado nos Controllers
Usar `result.Data` e `result.HasErrors`:

```csharp
if (result.Data == null && result.HasErrors)
    return NotFound(result);
```

---

## 3. Mediator - Implementação Manual

### Problema
O Mvp24Hours não fornece método `AddMvp24HoursMediator()` para registro automático do mediator.

### Solução
Criar uma implementação simples do `IMediator` e registrar manualmente:

```csharp
// Application/Infrastructure/SimpleMediator.cs
public class SimpleMediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public async Task<TResponse> SendAsync<TResponse>(IMediatorCommand<TResponse> command, CancellationToken ct)
    {
        var handlerType = typeof(IMediatorCommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)command, ct);
    }
    // ... demais métodos
}

// Registro em ServiceBuilderExtensions.cs
services.AddScoped<IMediator, SimpleMediator>();
services.AddScoped<IMediatorCommandHandler<CreateClienteCommand, IBusinessResult<ClienteDto>>, CreateClienteCommandHandler>();
// ... registrar todos os handlers manualmente
```

---

## 4. Conflito de Namespace com Value Objects

### Problema
Namespace `Lab10.Infrastructure.Email` conflita com o tipo `Email` do domínio (`Lab10.Domain.ValueObjects.Email`).

### Solução
Usar o fully qualified name nas classes de infraestrutura:

```csharp
public async Task<Cliente?> GetByEmailAsync(Lab10.Domain.ValueObjects.Email email, CancellationToken ct)
{
    return await _context.Clientes
        .FirstOrDefaultAsync(c => c.Email.Valor == email.Valor, ct);
}
```

---

## 5. NLog - API Obsoleta

### Problema
`MappedDiagnosticsLogicalContext` está obsoleto no NLog 5+.

### Solução
Usar `ScopeContext` no lugar:

```csharp
// Antigo (obsoleto):
using (MappedDiagnosticsLogicalContext.SetScoped("CorrelationId", correlationId))

// Novo (NLog 5+):
using (ScopeContext.PushProperty("CorrelationId", correlationId))
```

---

## 6. OpenTelemetry - APIs não disponíveis

### Problema
Alguns métodos de instrumentação não estão disponíveis nas versões atuais:
- `SetDbStatementForText` não existe em `SqlClientTraceInstrumentationOptions`
- `AddRuntimeInstrumentation` não existe no `MeterProviderBuilder`
- `AddProcessInstrumentation` não existe no `MeterProviderBuilder`

### Solução
Remover ou comentar essas chamadas:

```csharp
.AddSqlClientInstrumentation(options =>
{
    // options.SetDbStatementForText = true; // Não existe
    options.RecordException = true;
})

.WithMetrics(metrics =>
{
    metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        // .AddRuntimeInstrumentation() // Não existe
        // .AddProcessInstrumentation() // Não existe
        .AddPrometheusExporter();
});
```

---

## 7. Validação de Status em Entidades de Domínio

### Problema
Métodos de transição de estado em entidades como `Venda` podem ser muito restritivos.

### Solução
Flexibilizar transições quando necessário:

```csharp
// Antes - muito restritivo
public void IniciarProcessamento()
{
    if (Status != VendaStatus.Confirmada)
        throw new DomainException("...");
}

// Depois - mais flexível
public void IniciarProcessamento()
{
    if (Status != VendaStatus.Pendente && Status != VendaStatus.Confirmada)
        throw new DomainException("...");
}
```

---

## 8. Activity e OpenTelemetry

### Problema
O método `RecordException` não está disponível diretamente em `Activity`.

### Solução
Usar tags para registrar informações de exceção:

```csharp
catch (Exception ex)
{
    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
    activity?.AddTag("exception.type", ex.GetType().FullName);
    activity?.AddTag("exception.message", ex.Message);
    activity?.AddTag("exception.stacktrace", ex.StackTrace);
    throw;
}
```

Se precisar usar `RecordException`, adicionar o pacote `OpenTelemetry.Api` e usar a extensão.

---

## 9. Estrutura de DTOs para Queries

### Dica
Criar DTOs de resumo separados para listas e DTOs completos para detalhes:

```csharp
// DTO completo para GetById
public record VendaDto(int Id, ..., IEnumerable<ItemVendaDto> Itens, PagamentoDto? Pagamento);

// DTO resumido para listagens
public record VendaResumoDto(int Id, ..., int QuantidadeItens, DateTime DataCriacao);
```

---

## 10. Saga Pattern com Steps Separados

### Implementação
Cada step da saga deve implementar:
- `ExecuteAsync` - lógica de execução
- `CompensateAsync` - lógica de compensação (rollback)

```csharp
public interface ISagaStep<TContext>
{
    string Name { get; }
    int Order { get; }
    Task<SagaStepResult> ExecuteAsync(TContext context, CancellationToken ct);
    Task CompensateAsync(TContext context, CancellationToken ct);
}
```

---

## Resumo de Pacotes Utilizados

```xml
<!-- Domain -->
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />
<PackageReference Include="FluentValidation" Version="12.*" />

<!-- Application -->
<PackageReference Include="Mvp24Hours.Application" Version="9.*" />
<PackageReference Include="Mvp24Hours.Infrastructure.Cqrs" Version="9.*" />
<PackageReference Include="AutoMapper" Version="13.*" />
<PackageReference Include="OpenTelemetry.Api" Version="1.*" />

<!-- Infrastructure -->
<PackageReference Include="Mvp24Hours.Infrastructure.Data.EFCore" Version="9.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.*" />
<PackageReference Include="OpenTelemetry.Instrumentation.SqlClient" Version="1.14.0-rc.1" />

<!-- WebAPI -->
<PackageReference Include="Mvp24Hours.WebAPI" Version="9.*" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.*" />
<PackageReference Include="NLog.Web.AspNetCore" Version="5.*" />
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.*" />
<PackageReference Include="OpenTelemetry.Exporter.Prometheus.AspNetCore" Version="1.14.0-beta.1" />
<PackageReference Include="AspNetCore.HealthChecks.SqlServer" Version="8.*" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="12.*" />
```
