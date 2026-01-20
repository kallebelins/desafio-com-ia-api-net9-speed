# Lab 09 - CQRS + Event Sourcing com Venda

## 🎯 Objetivo
Criar um sistema de vendas usando **CQRS com Event Sourcing**, onde o estado é derivado da sequência de eventos, permitindo auditoria completa e reconstrução de estados históricos.

## 📋 Requisito de Negócio
- **Entidade**: Venda (Aggregate Root)
- **Eventos**: 
  - VendaIniciada
  - ItemAdicionado
  - ItemRemovido
  - DescontoAplicado
  - VendaFinalizada
  - VendaCancelada
- **Funcionalidades**:
  - Histórico completo de alterações
  - Reconstrução de estado em qualquer ponto no tempo
  - Projeções para leitura otimizada

## 🏗️ Arquitetura
**CQRS + Event Sourcing** - Write model baseado em eventos, Read model com projeções.

```
Lab09.EventSourcing/
├── Lab09.EventSourcing.sln
├── src/
│   ├── Lab09.Core/
│   │   ├── Aggregates/
│   │   │   └── VendaAggregate.cs
│   │   ├── Events/
│   │   │   ├── VendaIniciadaEvent.cs
│   │   │   ├── ItemAdicionadoEvent.cs
│   │   │   ├── ItemRemovidoEvent.cs
│   │   │   ├── DescontoAplicadoEvent.cs
│   │   │   ├── VendaFinalizadaEvent.cs
│   │   │   └── VendaCanceladaEvent.cs
│   │   ├── ValueObjects/
│   │   │   ├── VendaId.cs
│   │   │   ├── ItemVenda.cs
│   │   │   └── Money.cs
│   │   └── Interfaces/
│   │       ├── IAggregateRoot.cs
│   │       ├── IEventStore.cs
│   │       └── IProjection.cs
│   │
│   ├── Lab09.Application/
│   │   ├── Commands/
│   │   │   ├── IniciarVendaCommand.cs
│   │   │   ├── AdicionarItemCommand.cs
│   │   │   ├── RemoverItemCommand.cs
│   │   │   ├── AplicarDescontoCommand.cs
│   │   │   ├── FinalizarVendaCommand.cs
│   │   │   └── CancelarVendaCommand.cs
│   │   ├── Queries/
│   │   │   ├── GetVendaByIdQuery.cs
│   │   │   ├── GetVendaHistoryQuery.cs
│   │   │   └── GetVendasPorPeriodoQuery.cs
│   │   ├── Handlers/
│   │   │   ├── Commands/
│   │   │   │   └── VendaCommandHandler.cs
│   │   │   └── Queries/
│   │   │       └── VendaQueryHandler.cs
│   │   └── Projections/
│   │       ├── VendaReadModel.cs
│   │       ├── VendaProjection.cs
│   │       └── RelatorioVendasProjection.cs
│   │
│   ├── Lab09.Infrastructure/
│   │   ├── EventStore/
│   │   │   ├── EventStoreDbContext.cs
│   │   │   ├── StoredEvent.cs
│   │   │   └── EfCoreEventStore.cs
│   │   ├── Projections/
│   │   │   ├── ProjectionDbContext.cs
│   │   │   └── ProjectionEngine.cs
│   │   └── Snapshots/
│   │       ├── SnapshotStore.cs
│   │       └── VendaSnapshot.cs
│   │
│   └── Lab09.WebAPI/
│       ├── Program.cs
│       ├── Controllers/
│       │   ├── VendaCommandController.cs
│       │   └── VendaQueryController.cs
│       ├── HostedServices/
│       │   └── ProjectionHostedService.cs
│       └── Extensions/
│           └── ServiceBuilderExtensions.cs
```

## 🔧 Recursos Utilizados

| Recurso | Descrição |
|---------|-----------|
| **Event Sourcing** | Estado derivado de eventos |
| **Aggregate Root** | VendaAggregate com eventos |
| **Event Store** | Persistência de eventos |
| **Projections** | Read models derivados |
| **Snapshots** | Otimização de reconstrução |
| **CQRS** | Separação Write/Read |

## 📦 Pacotes NuGet

```xml
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />
<PackageReference Include="Mvp24Hours.Infrastructure.Cqrs" Version="9.*" />
<PackageReference Include="Mvp24Hours.Infrastructure.Data.EFCore" Version="9.*" />
```

## 🔐 Credenciais e Configuração

Este lab utiliza os serviços do `docker-compose.yml` principal localizado em `../docker-compose.yml`.

### Serviços Utilizados

| Serviço | Host | Porta | Credenciais |
|---------|------|-------|-------------|
| **SQL Server** | `sqlserver` | `1433` | Usuário: `sa`<br>Senha: `Lab@Mvp24Hours!` |

### String de Conexão

```json
{
  "ConnectionStrings": {
    "EventStore": "Server=sqlserver;Database=Lab09_EventStore;User Id=sa;Password=Lab@Mvp24Hours!;TrustServerCertificate=True;",
    "ReadModel": "Server=sqlserver;Database=Lab09_ReadModel;User Id=sa;Password=Lab@Mvp24Hours!;TrustServerCertificate=True;"
  }
}
```

### Executar Infraestrutura

```bash
# Na pasta labs/
cd ..
docker-compose up -d sqlserver
```

## 🔄 Fluxo Event Sourcing

```
┌─────────────────────────────────────────────────────────────────┐
│                      WRITE SIDE (Commands)                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│   Command ──▶ Handler ──▶ Aggregate ──▶ Events ──▶ Event Store  │
│                              │                                   │
│                              ▼                                   │
│                    Apply Events                                  │
│                    (Update State)                                │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                      READ SIDE (Queries)                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│   Event Store ──▶ Projection Engine ──▶ Read Model Database     │
│                                                │                 │
│                                                ▼                 │
│   Query ──────────────────────────────▶ Read Model ──▶ Response │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

## 📝 Aggregate Root com Event Sourcing

```csharp
public class VendaAggregate : IAggregateRoot
{
    private readonly List<IDomainEvent> _uncommittedEvents = new();
    
    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public List<ItemVenda> Itens { get; private set; } = new();
    public decimal Total { get; private set; }
    public decimal Desconto { get; private set; }
    public VendaStatus Status { get; private set; }
    public int Version { get; private set; }

    // Reconstituir do histórico de eventos
    public static VendaAggregate FromHistory(IEnumerable<IDomainEvent> events)
    {
        var aggregate = new VendaAggregate();
        foreach (var @event in events)
        {
            aggregate.Apply(@event);
            aggregate.Version++;
        }
        return aggregate;
    }

    // Comandos que geram eventos
    public void Iniciar(Guid clienteId)
    {
        if (Status != VendaStatus.None)
            throw new DomainException("Venda já iniciada");

        RaiseEvent(new VendaIniciadaEvent
        {
            VendaId = Id,
            ClienteId = clienteId,
            DataInicio = DateTime.UtcNow
        });
    }

    public void AdicionarItem(Guid produtoId, string produtoNome, int quantidade, decimal precoUnitario)
    {
        if (Status != VendaStatus.EmAndamento)
            throw new DomainException("Venda não está em andamento");

        RaiseEvent(new ItemAdicionadoEvent
        {
            VendaId = Id,
            ProdutoId = produtoId,
            ProdutoNome = produtoNome,
            Quantidade = quantidade,
            PrecoUnitario = precoUnitario
        });
    }

    // Aplicar eventos (mutar estado)
    private void Apply(IDomainEvent @event)
    {
        switch (@event)
        {
            case VendaIniciadaEvent e:
                Id = e.VendaId;
                ClienteId = e.ClienteId;
                Status = VendaStatus.EmAndamento;
                break;

            case ItemAdicionadoEvent e:
                Itens.Add(new ItemVenda(e.ProdutoId, e.ProdutoNome, e.Quantidade, e.PrecoUnitario));
                RecalcularTotal();
                break;

            case DescontoAplicadoEvent e:
                Desconto = e.ValorDesconto;
                RecalcularTotal();
                break;

            case VendaFinalizadaEvent e:
                Status = VendaStatus.Finalizada;
                break;

            case VendaCanceladaEvent e:
                Status = VendaStatus.Cancelada;
                break;
        }
    }

    private void RaiseEvent(IDomainEvent @event)
    {
        Apply(@event);
        _uncommittedEvents.Add(@event);
    }

    public IReadOnlyList<IDomainEvent> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();
    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();
}
```

## 📝 Event Store

```csharp
public class StoredEvent
{
    public Guid Id { get; set; }
    public string AggregateId { get; set; }
    public string AggregateType { get; set; }
    public string EventType { get; set; }
    public string EventData { get; set; }  // JSON
    public int Version { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; }
}

public class EfCoreEventStore : IEventStore
{
    public async Task SaveEventsAsync(
        string aggregateId, 
        IEnumerable<IDomainEvent> events, 
        int expectedVersion)
    {
        var version = expectedVersion;
        
        foreach (var @event in events)
        {
            version++;
            var stored = new StoredEvent
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                AggregateType = "Venda",
                EventType = @event.GetType().Name,
                EventData = JsonSerializer.Serialize(@event, @event.GetType()),
                Version = version,
                Timestamp = DateTime.UtcNow
            };
            
            await _context.StoredEvents.AddAsync(stored);
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task<IList<IDomainEvent>> GetEventsAsync(string aggregateId)
    {
        var storedEvents = await _context.StoredEvents
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.Version)
            .ToListAsync();

        return storedEvents
            .Select(e => DeserializeEvent(e.EventType, e.EventData))
            .ToList();
    }
}
```

## 📊 Projection para Read Model

```csharp
public class VendaProjection : IProjection
{
    private readonly ProjectionDbContext _context;

    public async Task HandleAsync(IDomainEvent @event)
    {
        switch (@event)
        {
            case VendaIniciadaEvent e:
                await CreateReadModel(e);
                break;

            case ItemAdicionadoEvent e:
                await AddItemToReadModel(e);
                break;

            case VendaFinalizadaEvent e:
                await FinalizeReadModel(e);
                break;
        }
    }

    private async Task CreateReadModel(VendaIniciadaEvent e)
    {
        var readModel = new VendaReadModel
        {
            Id = e.VendaId,
            ClienteId = e.ClienteId,
            DataInicio = e.DataInicio,
            Status = "EmAndamento",
            Itens = new List<ItemVendaReadModel>()
        };
        
        await _context.Vendas.AddAsync(readModel);
        await _context.SaveChangesAsync();
    }
}
```

## ✅ Checklist de Implementação

- [ ] Criar Aggregate Root com Event Sourcing
- [ ] Definir todos os eventos de domínio
- [ ] Implementar Event Store com EF Core
- [ ] Criar Commands para cada operação
- [ ] Implementar Command Handler que usa Aggregate
- [ ] Criar Projections para Read Models
- [ ] Implementar Projection Engine
- [ ] Criar Queries para leitura
- [ ] Implementar Snapshots para otimização
- [ ] Criar endpoint para histórico de eventos
- [ ] Testar reconstrução de estado

## 💡 Conceitos Aprendidos

1. Event Sourcing - Estado como sequência de eventos
2. Aggregate Root com eventos de domínio
3. Event Store para persistência de eventos
4. Projections para criar Read Models
5. CQRS com bancos separados (Write/Read)
6. Snapshots para otimizar reconstrução
7. Auditoria automática e completa
8. Time Travel (reconstituir estado histórico)

## 🔗 Ferramentas MCP Utilizadas

```
mvp24h_cqrs_guide({ topic: "event-sourcing" })
mvp24h_cqrs_guide({ topic: "commands" })
mvp24h_cqrs_guide({ topic: "queries" })
mvp24h_cqrs_guide({ topic: "domain-events" })
mvp24h_database_advisor({ patterns: ["repository"] })
```

---
**Nível de Complexidade**: ⭐⭐⭐⭐⭐ Expert+
