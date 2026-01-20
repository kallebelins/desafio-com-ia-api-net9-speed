# Lab 04 - Event-Driven + RabbitMQ com Cadastro de Cliente

## 🎯 Objetivo
Criar uma API REST para cadastro de clientes com arquitetura **Event-Driven**, publicando eventos no RabbitMQ quando ocorrerem operações.

## 📋 Requisito de Negócio
- **Entidade**: Cliente
- **Campos**: Id, Nome, Email, CPF, Telefone, Ativo, DataCriacao
- **Eventos**: ClienteCriado, ClienteAtualizado, ClienteExcluido
- **Consumidor**: Enviar email de boas-vindas quando cliente for criado

## 🏗️ Arquitetura
**Event-Driven** - Comunicação através de eventos de domínio e eventos de integração.

```
Lab04.EventDriven/
├── Lab04.EventDriven.sln
├── src/
│   ├── Lab04.Core/
│   │   ├── Lab04.Core.csproj
│   │   ├── Entities/
│   │   │   └── Cliente.cs
│   │   ├── Events/
│   │   │   ├── Domain/
│   │   │   │   ├── ClienteCriadoEvent.cs
│   │   │   │   ├── ClienteAtualizadoEvent.cs
│   │   │   │   └── ClienteExcluidoEvent.cs
│   │   │   └── Integration/
│   │   │       └── ClienteCriadoIntegrationEvent.cs
│   │   ├── ValueObjects/
│   │   │   └── ClienteDto.cs
│   │   └── Contract/
│   │       └── Events/
│   │           ├── IDomainEventDispatcher.cs
│   │           └── IIntegrationEventPublisher.cs
│   │
│   ├── Lab04.Application/
│   │   ├── Lab04.Application.csproj
│   │   ├── Services/
│   │   │   └── ClienteService.cs
│   │   └── EventHandlers/
│   │       ├── Domain/
│   │       │   └── ClienteCriadoEventHandler.cs
│   │       └── Integration/
│   │           └── EmailBoasVindasHandler.cs
│   │
│   ├── Lab04.Infrastructure/
│   │   ├── Lab04.Infrastructure.csproj
│   │   ├── Data/
│   │   │   └── DataContext.cs
│   │   ├── Events/
│   │   │   ├── DomainEventDispatcher.cs
│   │   │   └── IntegrationEventPublisher.cs
│   │   └── Messaging/
│   │       └── RabbitMQ/
│   │           └── RabbitMQConnection.cs
│   │
│   └── Lab04.WebAPI/
│       ├── Lab04.WebAPI.csproj
│       ├── Program.cs
│       ├── Controllers/
│       │   └── ClienteController.cs
│       ├── HostedServices/
│       │   └── IntegrationEventConsumerService.cs
│       └── Extensions/
│           └── ServiceBuilderExtensions.cs
```

## 🔧 Recursos Utilizados

| Recurso | Descrição |
|---------|-----------|
| **Domain Events** | Eventos internos do domínio |
| **Integration Events** | Eventos para outros serviços via RabbitMQ |
| **RabbitMQ** | Message Broker para eventos |
| **Event Handlers** | Handlers para processar eventos |
| **Hosted Service** | Consumidor de eventos em background |

## 📦 Pacotes NuGet

```xml
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />
<PackageReference Include="Mvp24Hours.Infrastructure.RabbitMQ" Version="9.*" />
<PackageReference Include="RabbitMQ.Client" Version="6.*" />
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
    "DefaultConnection": "Server=sqlserver;Database=Lab04_Clientes;User Id=sa;Password=Lab@Mvp24Hours!;TrustServerCertificate=True;"
  },
  "RabbitMQ": {
    "HostName": "rabbitmq",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "Exchange": "lab04.exchange"
  }
}
```

### Executar Infraestrutura

```bash
# Na pasta labs/
cd ..
docker-compose up -d sqlserver rabbitmq
```

## 🚀 Fluxo de Eventos

```
┌─────────────┐     ┌─────────────────┐     ┌──────────────┐
│   Cliente   │────▶│ Domain Event    │────▶│ Handler      │
│   Service   │     │ (ClienteCriado) │     │ (Publicar)   │
└─────────────┘     └─────────────────┘     └──────┬───────┘
                                                   │
                                                   ▼
┌─────────────┐     ┌─────────────────┐     ┌──────────────┐
│   Email     │◀────│ Consumer        │◀────│  RabbitMQ    │
│   Service   │     │ (HostedService) │     │  Queue       │
└─────────────┘     └─────────────────┘     └──────────────┘
```

## 📝 Eventos de Domínio

```csharp
public record ClienteCriadoEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => nameof(ClienteCriadoEvent);
    
    public int ClienteId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
```

## 📝 Eventos de Integração

```csharp
public record ClienteCriadoIntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public string EventType => nameof(ClienteCriadoIntegrationEvent);
    public string CorrelationId { get; init; } = string.Empty;
    
    public int ClienteId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
```

## ✅ Checklist de Implementação

- [ ] Criar estrutura de projetos
- [ ] Definir eventos de domínio (Domain Events)
- [ ] Definir eventos de integração (Integration Events)
- [ ] Implementar DomainEventDispatcher
- [ ] Implementar IntegrationEventPublisher com RabbitMQ
- [ ] Criar Service que dispara eventos após operações
- [ ] Criar HostedService para consumir eventos
- [ ] Implementar handler de email de boas-vindas
- [ ] Configurar RabbitMQ no docker-compose
- [ ] Testar fluxo completo de eventos

## 💡 Conceitos Aprendidos

1. Event-Driven Architecture
2. Domain Events vs Integration Events
3. Publish/Subscribe com RabbitMQ
4. Event Handlers e Consumers
5. Hosted Services para processamento background
6. Desacoplamento através de eventos

## 🔗 Ferramentas MCP Utilizadas

```
mvp24h_get_template({ template_name: "event-driven" })
mvp24h_messaging_patterns({ pattern: "rabbitmq" })
mvp24h_cqrs_guide({ topic: "domain-events" })
mvp24h_cqrs_guide({ topic: "integration-events" })
```

---
**Nível de Complexidade**: ⭐⭐⭐ Avançado
