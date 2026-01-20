# 🧪 Série de Laboratórios Mvp24Hours

> Uma série progressiva de laboratórios para aprender a usar o **Mvp24Hours Framework** em diferentes arquiteturas e cenários.

## 📚 Sobre a Série

Esta série de laboratórios foi projetada para ensinar progressivamente os conceitos e recursos do Mvp24Hours Framework, desde implementações simples até sistemas enterprise completos.

Cada laboratório foca em um aspecto específico:
- **Arquitetura**: Minimal API, N-Layers, CQRS, Hexagonal, Clean Architecture
- **Recursos**: Repository, Unit of Work, Events, Messaging, Observability, Saga
- **Negócio**: Cadastros de Cliente, Produto e Vendas

## 🎯 Pré-requisitos

- .NET 9 SDK
- Docker e Docker Compose
- SQL Server (via Docker)
- RabbitMQ (via Docker - para labs avançados)
- Conhecimento básico de C# e ASP.NET Core

## 📋 Índice dos Laboratórios

### Nível Básico ⭐

| Lab | Arquitetura | Negócio | Recursos |
|-----|-------------|---------|----------|
| [Lab 01](./lab-01-minimal-api-produto/) | Minimal API | Produto | Repository, Validation |

### Nível Intermediário ⭐⭐

| Lab | Arquitetura | Negócio | Recursos |
|-----|-------------|---------|----------|
| [Lab 02](./lab-02-simple-nlayers-cliente/) | Simple N-Layers | Cliente | Repository, UoW, Validation, AutoMapper |

### Nível Avançado ⭐⭐⭐

| Lab | Arquitetura | Negócio | Recursos |
|-----|-------------|---------|----------|
| [Lab 03](./lab-03-cqrs-produto/) | Complex N-Layers + CQRS | Produto | CQRS/Mediator, Behaviors |
| [Lab 04](./lab-04-event-driven-cliente/) | Event-Driven | Cliente | Domain Events, RabbitMQ |

### Nível Avançado+ ⭐⭐⭐⭐

| Lab | Arquitetura | Negócio | Recursos |
|-----|-------------|---------|----------|
| [Lab 05](./lab-05-cqrs-observability-produto/) | CQRS + Observability | Produto | OpenTelemetry, Logging, Metrics |
| [Lab 06](./lab-06-hexagonal-cliente/) | Hexagonal | Cliente | Ports & Adapters, DI |

### Nível Expert ⭐⭐⭐⭐⭐

| Lab | Arquitetura | Negócio | Recursos |
|-----|-------------|---------|----------|
| [Lab 07](./lab-07-event-driven-saga-venda/) | Event-Driven + Saga | Venda | Saga Pattern, Outbox, RabbitMQ |
| [Lab 08](./lab-08-clean-architecture-completo/) | Clean Architecture | Sistema Completo | Use Cases, Domain Services |

### Nível Expert+ ⭐⭐⭐⭐⭐+

| Lab | Arquitetura | Negócio | Recursos |
|-----|-------------|---------|----------|
| [Lab 09](./lab-09-cqrs-event-sourcing-venda/) | CQRS + Event Sourcing | Venda | Event Store, Projections, Snapshots |

### Nível Master 🏆

| Lab | Arquitetura | Negócio | Recursos |
|-----|-------------|---------|----------|
| [Lab 10](./lab-10-fullstack-completo/) | Full Stack | Sistema Completo | TODOS os recursos combinados |

## 🗺️ Mapa de Aprendizado

```
Lab 01 ──▶ Lab 02 ──▶ Lab 03 ──┬──▶ Lab 05 ──▶ Lab 07 ──┐
 (Básico)  (Intermediário)      │                        │
                                │                        │
                                └──▶ Lab 04 ──▶ Lab 06 ──┤
                                                         │
                                        Lab 08 ◀─────────┤
                                        Lab 09 ◀─────────┤
                                                         │
                                        Lab 10 ◀─────────┘
                                       (Master)
```

## 🔧 Recursos por Laboratório

| Recurso | L01 | L02 | L03 | L04 | L05 | L06 | L07 | L08 | L09 | L10 |
|---------|-----|-----|-----|-----|-----|-----|-----|-----|-----|-----|
| Repository | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Unit of Work | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Validation | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| AutoMapper | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| CQRS/Mediator | - | - | ✅ | - | ✅ | - | - | - | ✅ | ✅ |
| Domain Events | - | - | - | ✅ | - | - | ✅ | ✅ | ✅ | ✅ |
| RabbitMQ | - | - | - | ✅ | - | - | ✅ | - | - | ✅ |
| OpenTelemetry | - | - | - | - | ✅ | - | - | - | - | ✅ |
| Health Checks | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Saga Pattern | - | - | - | - | - | - | ✅ | - | - | ✅ |
| Outbox Pattern | - | - | - | - | - | - | ✅ | - | - | ✅ |
| Event Sourcing | - | - | - | - | - | - | - | - | ✅ | - |
| Pipeline Behaviors | - | - | ✅ | - | ✅ | - | ✅ | - | - | ✅ |
| Value Objects | - | - | - | - | - | ✅ | - | ✅ | ✅ | ✅ |

## ⚠️ Regras Importantes

### ❌ NUNCA use MediatR
O Mvp24Hours tem implementação própria de CQRS/Mediator:
```csharp
// ❌ ERRADO
using MediatR;
public record Command : IRequest<Result> { }

// ✅ CORRETO
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
public record Command : IMediatorCommand<Result> { }
```

### ❌ NUNCA reinvente Value Objects
Use os Value Objects prontos do Mvp24Hours:
```csharp
using Mvp24Hours.Core.ValueObjects;

// Email, CPF, CNPJ, Money, Address, etc.
var email = Email.Create("user@example.com");
```

### ✅ SEMPRE use os namespaces corretos
| Componente | Namespace |
|------------|-----------|
| Value Objects | `Mvp24Hours.Core.ValueObjects` |
| Entidades | `Mvp24Hours.Core.Entities` |
| CQRS | `Mvp24Hours.Infrastructure.Cqrs.Abstractions` |
| Repository | `Mvp24Hours.Core.Contract.Data` |

## 🛠️ Ferramentas MCP

Cada laboratório indica quais ferramentas MCP do Mvp24Hours devem ser usadas:

```
mvp24h_get_started({ focus: "overview" })
mvp24h_architecture_advisor({ complexity: "high" })
mvp24h_database_advisor({ provider: "sqlserver" })
mvp24h_cqrs_guide({ topic: "commands" })
mvp24h_observability_setup({ component: "tracing" })
mvp24h_messaging_patterns({ pattern: "rabbitmq" })
mvp24h_get_template({ template_name: "cqrs" })
mvp24h_build_context({ architecture: "cqrs", resources: ["database", "observability"] })
```

## 📦 Pacotes NuGet Principais

```xml
<!-- Core -->
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />

<!-- Database -->
<PackageReference Include="Mvp24Hours.Infrastructure.Data.EFCore" Version="9.*" />

<!-- CQRS -->
<PackageReference Include="Mvp24Hours.Infrastructure.Cqrs" Version="9.*" />

<!-- Messaging -->
<PackageReference Include="Mvp24Hours.Infrastructure.RabbitMQ" Version="9.*" />

<!-- Pipeline -->
<PackageReference Include="Mvp24Hours.Infrastructure.Pipe" Version="9.*" />

<!-- WebAPI -->
<PackageReference Include="Mvp24Hours.WebAPI" Version="9.*" />
```

## 🐳 Infraestrutura (Docker)

Para executar os laboratórios avançados, use o docker-compose fornecido em cada lab ou o seguinte para ter toda a infraestrutura:

```bash
# Na pasta raiz dos labs
docker-compose up -d
```

### Serviços Disponíveis

| Serviço | Porta | Descrição |
|---------|-------|-----------|
| SQL Server | 1433 | Banco de dados |
| RabbitMQ | 5672, 15672 | Message Broker + Management UI |
| Redis | 6379 | Cache |
| Jaeger | 16686, 4317 | Tracing |
| Prometheus | 9090 | Métricas |
| Grafana | 3000 | Dashboards |

## 📝 Como Usar

1. **Escolha um laboratório** baseado no seu nível
2. **Leia o README.md** do laboratório escolhido
3. **Use as ferramentas MCP** indicadas para obter os templates
4. **Implemente seguindo o checklist**
5. **Teste todas as funcionalidades**
6. **Avance para o próximo laboratório**

## 🎓 Conclusão

Ao completar todos os laboratórios, você terá domínio sobre:

- ✅ Todas as arquiteturas suportadas pelo Mvp24Hours
- ✅ Padrões de design (Repository, UoW, CQRS, Event Sourcing)
- ✅ Mensageria com RabbitMQ
- ✅ Observability com OpenTelemetry
- ✅ Transações distribuídas com Saga
- ✅ Garantia de entrega com Outbox Pattern
- ✅ Value Objects e Domain-Driven Design
- ✅ Testes automatizados
- ✅ Containerização com Docker

---

**Bom aprendizado! 🚀**

*Desenvolvido para uso com o Mvp24Hours Framework - .NET 9*
