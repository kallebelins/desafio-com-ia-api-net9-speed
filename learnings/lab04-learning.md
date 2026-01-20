# Lições Aprendidas - Lab 04 Event-Driven com RabbitMQ

## Data: 2026-01-20

## Resumo
Implementação de API REST com arquitetura Event-Driven usando RabbitMQ para cadastro de clientes. Eventos de domínio disparam eventos de integração que são publicados no RabbitMQ e consumidos por um Hosted Service que envia emails de boas-vindas.

---

## Problema 1: API do BusinessResult alterada no Mvp24Hours

### Descrição
Ao tentar usar `ToBusiness("mensagem de erro")` para retornar erros, o compilador não reconhecia a sobrecarga.

### Erro
```
error CS1503: Argumento 2: não é possível converter de "string" para "Mvp24Hours.Core.Contract.ValueObjects.Logic.IMessageResult"
```

### Causa
A versão 9.x do Mvp24Hours alterou a assinatura do método `ToBusiness()`. Não existe mais sobrecarga que aceita string diretamente.

### Solução
Criar um método helper que constrói o `BusinessResult<T>` com a lista de mensagens:

```csharp
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;

/// <summary>
/// Helper method para criar resultado de erro
/// </summary>
private static IBusinessResult<T> CreateErrorResult<T>(string message)
{
    IReadOnlyCollection<IMessageResult> messages = new List<IMessageResult>
    {
        new MessageResult(message, MessageType.Error)
    };
    return new BusinessResult<T>(default, messages);
}

// Uso:
if (cliente == null)
{
    return CreateErrorResult<ClienteDto>("Cliente não encontrado");
}
```

### Imports Necessários
```csharp
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Extensions;
```

---

## Problema 2: IReadOnlyCollection para Messages

### Descrição
A propriedade `Messages` do `BusinessResult<T>` é do tipo `IReadOnlyCollection<IMessageResult>`, então não é possível usar `.Add()`.

### Erro
```
error CS1061: 'IReadOnlyCollection<IMessageResult>' não contém uma definição para "Add"
```

### Solução
Passar as mensagens no construtor do `BusinessResult<T>`:

```csharp
// ❌ INCORRETO - IReadOnlyCollection não tem Add
var result = new BusinessResult<T>();
result.Messages.Add(new MessageResult(...)); // ERRO!

// ✅ CORRETO - Passar no construtor
IReadOnlyCollection<IMessageResult> messages = new List<IMessageResult>
{
    new MessageResult("Mensagem", MessageType.Error)
};
return new BusinessResult<T>(default, messages);
```

---

## Problema 3: RabbitMQ.Client v6 vs v7

### Descrição
O RabbitMQ.Client versão 7.x teve mudanças significativas na API (async por padrão).

### Solução
Usar a versão 6.x que é estável e compatível com a maioria dos exemplos e documentações:

```xml
<PackageReference Include="RabbitMQ.Client" Version="6.*" />
```

### API Usada (v6)
```csharp
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Criar conexão
var factory = new ConnectionFactory { HostName = "localhost" };
var connection = factory.CreateConnection();

// Criar canal
using var channel = connection.CreateModel();

// Declarar exchange e queue
channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);
channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
channel.QueueBind(queue, exchange, routingKey);

// Consumer assíncrono
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.Received += async (model, ea) => { ... };
channel.BasicConsume(queue, autoAck: false, consumer: consumer);
```

---

## Padrões Implementados

### 1. Domain Events → Integration Events
```
Cliente Criado
    → ClienteCriadoEvent (Domain Event)
        → ClienteCriadoEventHandler
            → ClienteCriadoIntegrationEvent (publicado no RabbitMQ)
                → Consumer (HostedService)
                    → EmailBoasVindasHandler
```

### 2. Interfaces de Eventos

```csharp
// Evento de Domínio
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
}

// Evento de Integração
public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
    string CorrelationId { get; }
}
```

### 3. Dispatcher e Publisher

```csharp
// Domain Event Dispatcher (in-process)
public interface IDomainEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken ct = default)
        where TEvent : IDomainEvent;
}

// Integration Event Publisher (RabbitMQ)
public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, string routingKey, CancellationToken ct = default)
        where TEvent : IIntegrationEvent;
}
```

---

## Estrutura de Pastas

```
Lab04.EventDriven/
├── src/
│   ├── Lab04.Core/                    # Entidades, Eventos, Contratos
│   │   ├── Entities/
│   │   ├── Events/
│   │   │   ├── Base/                  # IDomainEvent, IIntegrationEvent
│   │   │   ├── Domain/                # Eventos de domínio
│   │   │   └── Integration/           # Eventos de integração
│   │   ├── ValueObjects/              # DTOs
│   │   └── Contract/Events/           # Interfaces
│   │
│   ├── Lab04.Application/             # Serviços e Handlers
│   │   ├── Services/
│   │   └── EventHandlers/
│   │       ├── Domain/                # Handlers de eventos de domínio
│   │       └── Integration/           # Handlers de eventos de integração
│   │
│   ├── Lab04.Infrastructure/          # Data, Messaging
│   │   ├── Data/
│   │   ├── Events/                    # Dispatcher, Publisher
│   │   └── Messaging/RabbitMQ/
│   │
│   └── Lab04.WebAPI/                  # API, Consumers
│       ├── Controllers/
│       ├── HostedServices/            # Consumer RabbitMQ
│       └── Extensions/
```

---

## Lições do Lab 02 Aplicadas

- **NÃO usar EnableRetryOnFailure** com Mvp24Hours Repository/UoW
- Configuração simples do SQL Server:
```csharp
services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
```

---

## Versões Utilizadas

| Pacote | Versão |
|--------|--------|
| .NET SDK | 9.0 |
| Mvp24Hours.Core | 9.* |
| Mvp24Hours.Infrastructure.Data.EFCore | 9.* |
| RabbitMQ.Client | 6.* |
| Microsoft.EntityFrameworkCore.SqlServer | 9.* |
| Swashbuckle.AspNetCore | 7.* |

---

## Checklist de Implementação Event-Driven

- [x] Criar interfaces base (IDomainEvent, IIntegrationEvent)
- [x] Criar eventos de domínio
- [x] Criar eventos de integração
- [x] Criar contratos (IDomainEventDispatcher, IIntegrationEventPublisher)
- [x] Implementar DomainEventDispatcher
- [x] Implementar IntegrationEventPublisher com RabbitMQ
- [x] Criar Service que dispara eventos após operações
- [x] Criar HostedService para consumir eventos
- [x] Implementar handler de email de boas-vindas
- [x] Configurar RabbitMQ connection factory

---

## Como Executar

1. Subir infraestrutura (SQL Server + RabbitMQ):
```bash
cd labs
docker-compose up -d sqlserver rabbitmq
```

2. Executar a API:
```bash
cd lab-04-event-driven-cliente
dotnet run --project src/Lab04.WebAPI
```

3. Acessar Swagger: `http://localhost:5000/swagger`

4. Testar fluxo:
   - POST /api/cliente (criar cliente)
   - Observar logs de email de boas-vindas no console

---

---

## Problema 4: Coluna não configurada no EF Core

### Descrição
Ao criar um cliente, o EF Core tentou inserir na coluna `DataAtualizacao` que não existia na tabela.

### Erro
```
Microsoft.Data.SqlClient.SqlException: Invalid column name 'DataAtualizacao'.
```

### Causa
O EF Core detecta automaticamente todas as propriedades da entidade, mas se você usar `EnsureCreated()` e depois adicionar novas propriedades, a tabela não é atualizada automaticamente.

### Solução
Sempre configurar todas as colunas explicitamente no `OnModelCreating`:

```csharp
modelBuilder.Entity<Cliente>(entity =>
{
    // ... outras configurações ...
    
    entity.Property(e => e.DataAtualizacao)
        .IsRequired(false);
});
```

### Lição
- Em desenvolvimento, se o schema mudar, dropar e recriar o banco
- Em produção, usar Migrations do EF Core para alterar o schema
- Sempre configurar explicitamente todas as propriedades da entidade

---

## Tags
`mvp24hours` `event-driven` `rabbitmq` `domain-events` `integration-events` `hosted-service` `message-broker`
