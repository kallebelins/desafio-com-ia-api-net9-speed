# Lab 07 - Event-Driven + Saga Pattern com Venda

## 🎯 Objetivo
Criar uma API REST para gestão de vendas implementando o **Saga Pattern** para orquestrar operações distribuídas entre Cliente, Produto e Venda.

## 📋 Requisito de Negócio
- **Entidades**: Cliente, Produto, Venda, ItemVenda
- **Fluxo de Venda**:
  1. Validar Cliente existe e está ativo
  2. Validar Produtos existem e têm estoque
  3. Reservar estoque dos produtos
  4. Criar a venda
  5. Notificar cliente por email
- **Compensação**: Se qualquer etapa falhar, reverter as anteriores (Saga)

## 🏗️ Arquitetura
**Event-Driven + Saga** - Orquestração de processos distribuídos com compensação.

```
Lab07.Saga/
├── Lab07.Saga.sln
├── src/
│   ├── Lab07.Core/
│   │   ├── Entities/
│   │   │   ├── Cliente.cs
│   │   │   ├── Produto.cs
│   │   │   ├── Venda.cs
│   │   │   └── ItemVenda.cs
│   │   ├── Events/
│   │   │   ├── VendaIniciadaEvent.cs
│   │   │   ├── EstoqueReservadoEvent.cs
│   │   │   ├── EstoqueLiberadoEvent.cs
│   │   │   ├── VendaCriadaEvent.cs
│   │   │   └── VendaCanceladaEvent.cs
│   │   ├── ValueObjects/
│   │   │   ├── VendaDto.cs
│   │   │   └── ItemVendaDto.cs
│   │   └── Enums/
│   │       ├── VendaStatus.cs
│   │       └── SagaStepStatus.cs
│   │
│   ├── Lab07.Application/
│   │   ├── Sagas/
│   │   │   ├── CriarVendaSaga.cs
│   │   │   ├── Steps/
│   │   │   │   ├── ValidarClienteStep.cs
│   │   │   │   ├── ValidarProdutosStep.cs
│   │   │   │   ├── ReservarEstoqueStep.cs
│   │   │   │   ├── CriarVendaStep.cs
│   │   │   │   └── NotificarClienteStep.cs
│   │   │   └── Compensations/
│   │   │       ├── LiberarEstoqueCompensation.cs
│   │   │       └── CancelarVendaCompensation.cs
│   │   ├── Services/
│   │   │   ├── VendaService.cs
│   │   │   ├── ClienteService.cs
│   │   │   └── ProdutoService.cs
│   │   └── EventHandlers/
│   │       └── ...
│   │
│   ├── Lab07.Infrastructure/
│   │   ├── Data/
│   │   │   ├── DataContext.cs
│   │   │   └── Configurations/
│   │   ├── Messaging/
│   │   │   └── RabbitMQ/
│   │   └── Outbox/
│   │       ├── OutboxMessage.cs
│   │       └── OutboxProcessor.cs
│   │
│   └── Lab07.WebAPI/
│       ├── Program.cs
│       ├── Controllers/
│       │   ├── ClienteController.cs
│       │   ├── ProdutoController.cs
│       │   └── VendaController.cs
│       ├── HostedServices/
│       │   ├── SagaOrchestratorService.cs
│       │   └── OutboxProcessorService.cs
│       └── Extensions/
│           └── ServiceBuilderExtensions.cs
```

## 🔧 Recursos Utilizados

| Recurso | Descrição |
|---------|-----------|
| **Saga Pattern** | Orquestração de transações distribuídas |
| **Outbox Pattern** | Garantia de entrega de eventos |
| **RabbitMQ** | Message Broker para eventos |
| **Compensating Transactions** | Rollback de operações |
| **Pipeline Pattern** | Mvp24Hours Pipeline para steps |

## 📦 Pacotes NuGet

```xml
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />
<PackageReference Include="Mvp24Hours.Infrastructure.Pipe" Version="9.*" />
<PackageReference Include="Mvp24Hours.Infrastructure.RabbitMQ" Version="9.*" />
<PackageReference Include="Mvp24Hours.Infrastructure.Data.EFCore" Version="9.*" />
```

## 🔐 Credenciais e Configuração

Este lab utiliza os serviços do `docker-compose.yml` principal localizado em `../docker-compose.yml`.

### Serviços Utilizados

| Serviço | Host | Porta | Credenciais |
|---------|------|-------|-------------|
| **SQL Server** | `sqlserver` | `1433` | Usuário: `sa`<br>Senha: `Lab@Mvp24Hours!` |
| **RabbitMQ** | `rabbitmq` | `5672` (AMQP)<br>`15672` (Management UI) | Usuário: `guest`<br>Senha: `guest` |

### String de Conexão

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver;Database=Lab07_Vendas;User Id=sa;Password=Lab@Mvp24Hours!;TrustServerCertificate=True;"
  },
  "RabbitMQ": {
    "HostName": "rabbitmq",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "Exchange": "lab07.exchange"
  }
}
```

### Executar Infraestrutura

```bash
# Na pasta labs/
cd ..
docker-compose up -d sqlserver rabbitmq
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
    c.SwaggerDoc("v1", new() { Title = "Lab07 Event-Driven + Saga - Vendas", Version = "v1" });
});

// ... resto do código ...

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab07 Event-Driven + Saga v1"));
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

## 🔄 Fluxo da Saga

```
┌──────────────────────────────────────────────────────────────────────┐
│                         CRIAR VENDA SAGA                              │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌─────────────┐   ┌─────────────┐   ┌─────────────┐   ┌──────────┐ │
│  │  Validar    │──▶│  Validar    │──▶│  Reservar   │──▶│  Criar   │ │
│  │  Cliente    │   │  Produtos   │   │  Estoque    │   │  Venda   │ │
│  └─────────────┘   └─────────────┘   └─────────────┘   └──────────┘ │
│        │                 │                 │                 │       │
│        ▼                 ▼                 ▼                 ▼       │
│   Se falhar:        Se falhar:        Se falhar:        Se falhar:  │
│   (nada a fazer)    (nada a fazer)    Liberar estoque   Cancelar    │
│                                                          + Liberar   │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

## 📝 Implementação da Saga

### Definição da Saga
```csharp
public class CriarVendaSaga
{
    private readonly IPipelineAsync _pipeline;
    private readonly ILogger<CriarVendaSaga> _logger;

    public async Task<SagaResult> ExecuteAsync(CriarVendaRequest request)
    {
        var context = new SagaContext(request);
        var executedSteps = new Stack<ISagaStep>();

        try
        {
            // Execute steps
            foreach (var step in GetSteps())
            {
                await step.ExecuteAsync(context);
                executedSteps.Push(step);
                
                if (context.Failed)
                    throw new SagaStepException(step.Name, context.Error);
            }

            return SagaResult.Success(context.Result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Saga failed, starting compensation");
            
            // Compensate in reverse order
            while (executedSteps.Count > 0)
            {
                var step = executedSteps.Pop();
                await step.CompensateAsync(context);
            }

            return SagaResult.Failure(ex.Message);
        }
    }

    private IEnumerable<ISagaStep> GetSteps()
    {
        yield return new ValidarClienteStep();
        yield return new ValidarProdutosStep();
        yield return new ReservarEstoqueStep();
        yield return new CriarVendaStep();
        yield return new NotificarClienteStep();
    }
}
```

### Saga Step Interface
```csharp
public interface ISagaStep
{
    string Name { get; }
    Task ExecuteAsync(SagaContext context);
    Task CompensateAsync(SagaContext context);
}
```

### Exemplo de Step com Compensação
```csharp
public class ReservarEstoqueStep : ISagaStep
{
    public string Name => "ReservarEstoque";

    public async Task ExecuteAsync(SagaContext context)
    {
        var itens = context.Get<List<ItemVendaDto>>("Itens");
        var reservas = new List<ReservaEstoque>();

        foreach (var item in itens)
        {
            var reserva = await _estoqueService.ReservarAsync(
                item.ProdutoId, 
                item.Quantidade);
            
            reservas.Add(reserva);
        }

        context.Set("Reservas", reservas);
    }

    public async Task CompensateAsync(SagaContext context)
    {
        var reservas = context.Get<List<ReservaEstoque>>("Reservas");
        
        foreach (var reserva in reservas)
        {
            await _estoqueService.LiberarReservaAsync(reserva.Id);
        }
    }
}
```

## 📤 Outbox Pattern

```csharp
// Garantir entrega de eventos mesmo se RabbitMQ estiver fora
public class OutboxMessage
{
    public Guid Id { get; set; }
    public string EventType { get; set; }
    public string Payload { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int RetryCount { get; set; }
    public OutboxStatus Status { get; set; }
}
```

## ✅ Checklist de Implementação

- [ ] Criar entidades (Cliente, Produto, Venda, ItemVenda)
- [ ] Definir eventos de domínio
- [ ] Implementar interface ISagaStep
- [ ] Criar steps da saga (Validar, Reservar, Criar, Notificar)
- [ ] Implementar compensações para cada step
- [ ] Criar orquestrador da saga
- [ ] Implementar Outbox Pattern
- [ ] Criar OutboxProcessor como HostedService
- [ ] Configurar RabbitMQ
- [ ] Testar cenários de sucesso e falha

## 💡 Conceitos Aprendidos

1. Saga Pattern (Orchestration)
2. Compensating Transactions
3. Outbox Pattern para garantia de entrega
4. Transações distribuídas
5. Event-driven com consistência eventual
6. Pipeline do Mvp24Hours para orquestração

## 🔗 Ferramentas MCP Utilizadas

```
mvp24h_cqrs_guide({ topic: "saga" })
mvp24h_messaging_patterns({ pattern: "rabbitmq" })
mvp24h_messaging_patterns({ pattern: "outbox" })
mvp24h_infrastructure_guide({ topic: "pipeline" })
```

---
**Nível de Complexidade**: ⭐⭐⭐⭐⭐ Expert
