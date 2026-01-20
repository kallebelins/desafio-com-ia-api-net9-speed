# Lab 07 - Event-Driven + Saga Pattern - Lições Aprendidas

## Visão Geral
Implementação de API de Vendas com Saga Pattern para orquestração de transações distribuídas usando Mvp24Hours Framework.

## Padrões Implementados

### 1. Saga Pattern (Orchestration)
O padrão Saga foi implementado para orquestrar múltiplos passos de uma transação distribuída:

```csharp
// Interface do Step de Saga
public interface ISagaStep<TData> where TData : class
{
    string Name { get; }
    int Order { get; }
    bool CanCompensate { get; }
    Task ExecuteAsync(TData data, CancellationToken cancellationToken = default);
    Task CompensateAsync(TData data, CancellationToken cancellationToken = default);
}
```

### 2. Compensating Transactions
Quando um step falha, os steps anteriores são compensados em ordem reversa:

```csharp
// Compensação em ordem reversa
while (executedSteps.TryPop(out var step))
{
    if (step.CanCompensate)
        await step.CompensateAsync(context, cancellationToken);
}
```

### 3. Outbox Pattern
Para garantia de entrega de eventos:

```csharp
public class OutboxMessage : EntityBase<Guid>
{
    public string EventType { get; set; }
    public string Payload { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int RetryCount { get; set; }
    public OutboxStatus Status { get; set; }
}
```

## Problemas Encontrados e Soluções

### 1. Migrations vs EnsureCreated

**Problema**: O projeto não tinha migrations configuradas, causando erro ao tentar criar as tabelas.

**Solução**: Implementar verificação inteligente que usa EnsureCreated quando não há migrations:

```csharp
public static async Task ApplyMigrationsAsync(this WebApplication app)
{
    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
    
    if (pendingMigrations.Any())
    {
        await context.Database.MigrateAsync();
    }
    else
    {
        // Sem migrations, usa EnsureCreated
        await context.Database.EnsureCreatedAsync();
    }
}
```

### 2. RabbitMQ Opcional

**Problema**: O pacote `Mvp24Hours.Infrastructure.RabbitMQ` não estava disponível na versão usada.

**Solução**: Implementar o OutboxProcessor de forma que simule a publicação quando RabbitMQ não está disponível:

```csharp
// Modo simulado - apenas loga (RabbitMQ seria integrado aqui)
_logger.LogInformation(
    "[OUTBOX] Publicando evento {EventType}: {Payload}",
    eventTypeName,
    message.Payload);
```

### 3. Computed Properties no EF Core

**Problema**: Propriedades calculadas como `ValorTotal` e `EstoqueDisponivel` causavam erro no mapeamento.

**Solução**: Ignorar propriedades calculadas na configuração do EF Core:

```csharp
// Ignore computed property
builder.Ignore(p => p.EstoqueDisponivel);
builder.Ignore(i => i.ValorTotal);
```

## Estrutura de Pastas Final

```
Lab07.Saga/
├── Lab07.Saga.sln
└── src/
    ├── Lab07.Core/
    │   ├── Entities/
    │   │   ├── Cliente.cs
    │   │   ├── Produto.cs
    │   │   ├── Venda.cs
    │   │   ├── ItemVenda.cs
    │   │   └── SagaState.cs
    │   ├── Events/
    │   │   ├── VendaIniciadaEvent.cs
    │   │   ├── EstoqueReservadoEvent.cs
    │   │   ├── VendaCriadaEvent.cs
    │   │   └── ...
    │   ├── ValueObjects/
    │   │   ├── VendaDto.cs
    │   │   ├── CriarVendaRequest.cs
    │   │   └── SagaResult.cs
    │   └── Enums/
    │       ├── VendaStatus.cs
    │       ├── SagaStatus.cs
    │       └── OutboxStatus.cs
    │
    ├── Lab07.Application/
    │   ├── Sagas/
    │   │   ├── ISagaStep.cs
    │   │   ├── CriarVendaSaga.cs
    │   │   ├── SagaContext.cs
    │   │   └── Steps/
    │   │       ├── ValidarClienteStep.cs
    │   │       ├── ValidarProdutosStep.cs
    │   │       ├── ReservarEstoqueStep.cs
    │   │       ├── CriarVendaStep.cs
    │   │       └── NotificarClienteStep.cs
    │   └── Services/
    │       ├── IClienteService.cs
    │       ├── IProdutoService.cs
    │       ├── IVendaService.cs
    │       └── INotificacaoService.cs
    │
    ├── Lab07.Infrastructure/
    │   ├── Data/
    │   │   ├── DataContext.cs
    │   │   └── Configurations/
    │   └── Outbox/
    │       ├── OutboxMessage.cs
    │       └── OutboxService.cs
    │
    └── Lab07.WebAPI/
        ├── Program.cs
        ├── Controllers/
        ├── HostedServices/
        │   └── OutboxProcessorService.cs
        └── Extensions/
            └── ServiceBuilderExtensions.cs
```

## Pacotes NuGet Utilizados

```xml
<!-- Core -->
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />

<!-- Application -->
<PackageReference Include="Mvp24Hours.Infrastructure.Pipe" Version="9.*" />
<PackageReference Include="FluentValidation" Version="11.*" />

<!-- Infrastructure -->
<PackageReference Include="Mvp24Hours.Infrastructure.Data.EFCore" Version="9.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.*" />

<!-- WebAPI -->
<PackageReference Include="Mvp24Hours.WebAPI" Version="9.*" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.*" />
<PackageReference Include="NLog.Web.AspNetCore" Version="5.*" />
```

## Fluxo da Saga de Criação de Venda

```
┌───────────────────────────────────────────────────────────────────────┐
│                          CRIAR VENDA SAGA                              │
├───────────────────────────────────────────────────────────────────────┤
│                                                                        │
│  ┌─────────────┐   ┌─────────────┐   ┌─────────────┐   ┌───────────┐ │
│  │  Validar    │──▶│  Validar    │──▶│  Reservar   │──▶│  Criar    │ │
│  │  Cliente    │   │  Produtos   │   │  Estoque    │   │  Venda    │ │
│  │  (Order 1)  │   │  (Order 2)  │   │  (Order 3)  │   │ (Order 4) │ │
│  └─────────────┘   └─────────────┘   └─────────────┘   └───────────┘ │
│        │                 │                 │                 │        │
│        │                 │                 │                 ▼        │
│        │                 │                 │         ┌───────────┐   │
│        │                 │                 │         │ Notificar │   │
│        │                 │                 │         │  Cliente  │   │
│        │                 │                 │         │ (Order 5) │   │
│        │                 │                 │         └───────────┘   │
│        │                 │                 │                          │
│  CanCompensate:    CanCompensate:    CanCompensate:    CanCompensate: │
│     false              false              true             true       │
│                                                                        │
└───────────────────────────────────────────────────────────────────────┘

Compensação (em caso de falha):
  - NotificarCliente: não compensa (email já enviado)
  - CriarVenda: cancela a venda
  - ReservarEstoque: libera as reservas
  - ValidarProdutos: nada a fazer (apenas validação)
  - ValidarCliente: nada a fazer (apenas validação)
```

## Registro de Serviços (DI)

```csharp
// Services
services.AddScoped<IClienteService, ClienteService>();
services.AddScoped<IProdutoService, ProdutoService>();
services.AddScoped<IVendaService, VendaService>();
services.AddScoped<INotificacaoService, NotificacaoService>();

// Saga Steps (registrados como IEnumerable<ISagaStep<T>>)
services.AddScoped<ISagaStep<CriarVendaSagaContext>, ValidarClienteStep>();
services.AddScoped<ISagaStep<CriarVendaSagaContext>, ValidarProdutosStep>();
services.AddScoped<ISagaStep<CriarVendaSagaContext>, ReservarEstoqueStep>();
services.AddScoped<ISagaStep<CriarVendaSagaContext>, CriarVendaStep>();
services.AddScoped<ISagaStep<CriarVendaSagaContext>, NotificarClienteStep>();

// Saga Orchestrator
services.AddScoped<CriarVendaSaga>();
```

## Boas Práticas Aplicadas

1. **Saga Steps Idempotentes**: Cada step pode ser executado múltiplas vezes sem efeitos colaterais
2. **Compensações Separadas**: Cada step define sua própria lógica de compensação
3. **Logging Detalhado**: Todos os passos são logados para debugging e auditoria
4. **Outbox Pattern**: Garante entrega de eventos mesmo com falhas de rede
5. **Soft Delete**: Entidades não são removidas fisicamente (campo Removed)
6. **DTOs Imutáveis**: Records usados para transferência de dados

## Cenários de Teste

1. **Sucesso**: Cliente ativo + Produtos com estoque → Venda criada
2. **Falha (Cliente Inativo)**: Compensação não necessária (validação)
3. **Falha (Sem Estoque)**: Compensação não necessária (validação)
4. **Falha (Após Reserva)**: Estoque é liberado via compensação

## Referências MCP Utilizadas

```
mvp24h_cqrs_guide({ topic: "saga" })
mvp24h_messaging_patterns({ pattern: "outbox" })
mvp24h_infrastructure_guide({ topic: "pipeline" })
mvp24h_database_advisor({ provider: "sqlserver" })
mvp24h_core_patterns({ topic: "entity-interfaces" })
```
