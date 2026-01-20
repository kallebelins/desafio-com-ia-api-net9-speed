# Lab 10 - Full Stack com Todos os Recursos

## 🎯 Objetivo
Criar um sistema completo de vendas implementando **TODAS** as técnicas e recursos aprendidos nos laboratórios anteriores: CQRS, Event-Driven, Saga, Observability, Messaging, Clean Architecture.

## 📋 Requisito de Negócio
- **Entidades**: Cliente, Produto, Categoria, Venda, ItemVenda, Pagamento
- **Funcionalidades Completas**:
  - Cadastro de Clientes com validação de CPF
  - Cadastro de Produtos com categorias e estoque
  - Processo de Venda completo (Saga)
  - Processamento de Pagamento
  - Notificações por email
  - Dashboard com relatórios
  - Auditoria completa

## 🏗️ Arquitetura
**Full Stack** - Combinação de Clean Architecture + CQRS + Event-Driven + Observability.

```
Lab10.FullStack/
├── Lab10.FullStack.sln
├── docker-compose.yml
├── src/
│   ├── Lab10.Domain/                    # Domain Layer
│   │   ├── Entities/
│   │   │   ├── Cliente.cs
│   │   │   ├── Produto.cs
│   │   │   ├── Categoria.cs
│   │   │   ├── Venda.cs
│   │   │   ├── ItemVenda.cs
│   │   │   └── Pagamento.cs
│   │   ├── ValueObjects/
│   │   │   ├── Email.cs
│   │   │   ├── CPF.cs
│   │   │   ├── Money.cs
│   │   │   └── Endereco.cs
│   │   ├── Events/
│   │   │   ├── Domain/
│   │   │   │   ├── ClienteCriadoEvent.cs
│   │   │   │   ├── VendaCriadaEvent.cs
│   │   │   │   └── PagamentoProcessadoEvent.cs
│   │   │   └── Integration/
│   │   │       ├── ClienteCriadoIntegrationEvent.cs
│   │   │       └── VendaFinalizadaIntegrationEvent.cs
│   │   ├── Interfaces/
│   │   │   ├── IClienteRepository.cs
│   │   │   ├── IProdutoRepository.cs
│   │   │   ├── IVendaRepository.cs
│   │   │   └── IPagamentoRepository.cs
│   │   ├── Services/
│   │   │   └── VendaDomainService.cs
│   │   └── Exceptions/
│   │       └── DomainException.cs
│   │
│   ├── Lab10.Application/               # Application Layer (CQRS)
│   │   ├── Commands/
│   │   │   ├── Clientes/
│   │   │   │   ├── CreateClienteCommand.cs
│   │   │   │   └── UpdateClienteCommand.cs
│   │   │   ├── Produtos/
│   │   │   │   ├── CreateProdutoCommand.cs
│   │   │   │   └── AtualizarEstoqueCommand.cs
│   │   │   └── Vendas/
│   │   │       ├── IniciarVendaCommand.cs
│   │   │       ├── AdicionarItemCommand.cs
│   │   │       └── FinalizarVendaCommand.cs
│   │   ├── Queries/
│   │   │   ├── Clientes/
│   │   │   │   ├── GetClienteByIdQuery.cs
│   │   │   │   └── GetAllClientesQuery.cs
│   │   │   ├── Produtos/
│   │   │   │   ├── GetProdutoByIdQuery.cs
│   │   │   │   └── GetProdutosByCategoriaQuery.cs
│   │   │   └── Vendas/
│   │   │       ├── GetVendaByIdQuery.cs
│   │   │       └── GetRelatorioVendasQuery.cs
│   │   ├── Handlers/
│   │   │   ├── Commands/
│   │   │   └── Queries/
│   │   ├── Behaviors/
│   │   │   ├── LoggingBehavior.cs
│   │   │   ├── ValidationBehavior.cs
│   │   │   ├── TracingBehavior.cs
│   │   │   └── TransactionBehavior.cs
│   │   ├── Sagas/
│   │   │   ├── ProcessarVendaSaga.cs
│   │   │   └── Steps/
│   │   │       ├── ValidarClienteStep.cs
│   │   │       ├── ReservarEstoqueStep.cs
│   │   │       ├── ProcessarPagamentoStep.cs
│   │   │       └── CriarVendaStep.cs
│   │   ├── EventHandlers/
│   │   │   ├── Domain/
│   │   │   └── Integration/
│   │   └── Validators/
│   │
│   ├── Lab10.Infrastructure/            # Infrastructure Layer
│   │   ├── Data/
│   │   │   ├── WriteDbContext.cs
│   │   │   ├── ReadDbContext.cs
│   │   │   ├── Configurations/
│   │   │   └── Repositories/
│   │   ├── Messaging/
│   │   │   └── RabbitMQ/
│   │   │       ├── EventPublisher.cs
│   │   │       └── Consumers/
│   │   ├── Outbox/
│   │   │   ├── OutboxMessage.cs
│   │   │   └── OutboxProcessor.cs
│   │   ├── Email/
│   │   │   └── SmtpEmailService.cs
│   │   ├── ExternalServices/
│   │   │   └── PagamentoGateway.cs
│   │   └── Observability/
│   │       ├── OpenTelemetrySetup.cs
│   │       └── Metrics/
│   │
│   └── Lab10.WebAPI/                    # Presentation Layer
│       ├── Program.cs
│       ├── appsettings.json
│       ├── NLog.config
│       ├── Controllers/
│       │   ├── ClienteController.cs
│       │   ├── ProdutoController.cs
│       │   ├── VendaController.cs
│       │   └── RelatorioController.cs
│       ├── Middlewares/
│       │   ├── CorrelationIdMiddleware.cs
│       │   ├── ExceptionMiddleware.cs
│       │   └── TenantMiddleware.cs
│       ├── HostedServices/
│       │   ├── OutboxProcessorService.cs
│       │   └── EventConsumerService.cs
│       └── Extensions/
│           ├── ServiceBuilderExtensions.cs
│           ├── CqrsExtensions.cs
│           ├── MessagingExtensions.cs
│           └── ObservabilityExtensions.cs
│
└── tests/
    ├── Lab10.Domain.Tests/
    ├── Lab10.Application.Tests/
    ├── Lab10.Infrastructure.Tests/
    └── Lab10.Integration.Tests/
```

## 🔧 Todos os Recursos Utilizados

| Recurso | Descrição |
|---------|-----------|
| **Clean Architecture** | Separação em camadas concêntricas |
| **CQRS** | Commands e Queries separados |
| **Mediator** | Mvp24Hours CQRS (NÃO MediatR!) |
| **Domain Events** | Eventos internos do domínio |
| **Integration Events** | Eventos entre serviços |
| **RabbitMQ** | Message Broker |
| **Saga Pattern** | Transações distribuídas |
| **Outbox Pattern** | Garantia de entrega |
| **Repository/UoW** | Acesso a dados |
| **OpenTelemetry** | Tracing distribuído |
| **Prometheus** | Métricas |
| **NLog** | Logging estruturado |
| **Health Checks** | Monitoramento |
| **FluentValidation** | Validação |
| **AutoMapper** | Mapeamento |
| **Value Objects** | Email, CPF, Money |
| **Pipeline Behaviors** | Cross-cutting concerns |

## 📦 Todos os Pacotes NuGet

```xml
<!-- Core -->
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />
<PackageReference Include="Mvp24Hours.Application" Version="9.*" />

<!-- CQRS -->
<PackageReference Include="Mvp24Hours.Infrastructure.Cqrs" Version="9.*" />

<!-- Database -->
<PackageReference Include="Mvp24Hours.Infrastructure.Data.EFCore" Version="9.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.*" />

<!-- Messaging -->
<PackageReference Include="Mvp24Hours.Infrastructure.RabbitMQ" Version="9.*" />
<PackageReference Include="RabbitMQ.Client" Version="6.*" />

<!-- Pipeline -->
<PackageReference Include="Mvp24Hours.Infrastructure.Pipe" Version="9.*" />

<!-- Caching -->
<PackageReference Include="Mvp24Hours.Infrastructure.Caching.Redis" Version="9.*" />

<!-- WebAPI -->
<PackageReference Include="Mvp24Hours.WebAPI" Version="9.*" />

<!-- Observability -->
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.*" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.*" />
<PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.*" />
<PackageReference Include="OpenTelemetry.Instrumentation.SqlClient" Version="1.*" />
<PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.*" />
<PackageReference Include="OpenTelemetry.Exporter.Prometheus.AspNetCore" Version="1.*" />
<PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.*" />
<PackageReference Include="NLog.Web.AspNetCore" Version="5.*" />

<!-- Health Checks -->
<PackageReference Include="AspNetCore.HealthChecks.UI.Client" Version="8.*" />
<PackageReference Include="AspNetCore.HealthChecks.SqlServer" Version="8.*" />
<PackageReference Include="AspNetCore.HealthChecks.Redis" Version="8.*" />
<PackageReference Include="AspNetCore.HealthChecks.RabbitMQ" Version="8.*" />

<!-- Validation & Mapping -->
<PackageReference Include="FluentValidation" Version="11.*" />
<PackageReference Include="AutoMapper" Version="12.*" />
```

## 🔐 Credenciais e Configuração

Este lab utiliza os serviços do `docker-compose.yml` principal localizado em `../docker-compose.yml`.

### Serviços Utilizados

| Serviço | Host | Porta | Credenciais |
|---------|------|-------|-------------|
| **SQL Server** | `sqlserver` | `1433` | Usuário: `sa`<br>Senha: `Lab@Mvp24Hours!` |
| **RabbitMQ** | `rabbitmq` | `5672` (AMQP)<br>`15672` (Management UI) | Usuário: `guest`<br>Senha: `guest` |
| **Redis** | `redis` | `6379` | Sem autenticação |
| **Jaeger** | `jaeger` | `16686` (UI)<br>`4317` (OTLP gRPC)<br>`4318` (OTLP HTTP) | Sem autenticação |
| **Prometheus** | `prometheus` | `9090` | Sem autenticação |
| **Grafana** | `grafana` | `3000` | Usuário: `admin`<br>Senha: `admin` |
| **Seq** | `seq` | `5341` (Ingestion)<br>`8081` (UI) | Sem autenticação |

### String de Conexão

```json
{
  "ConnectionStrings": {
    "WriteDatabase": "Server=sqlserver;Database=Lab10_Write;User Id=sa;Password=Lab@Mvp24Hours!;TrustServerCertificate=True;",
    "ReadDatabase": "Server=sqlserver;Database=Lab10_Read;User Id=sa;Password=Lab@Mvp24Hours!;TrustServerCertificate=True;",
    "Redis": "redis:6379"
  },
  "RabbitMQ": {
    "HostName": "rabbitmq",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "Exchange": "lab10.exchange"
  },
  "OpenTelemetry": {
    "JaegerEndpoint": "http://jaeger:4317",
    "SeqEndpoint": "http://seq:5341"
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
docker-compose up -d
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
    c.SwaggerDoc("v1", new() { Title = "Lab10 Full Stack - Sistema Completo", Version = "v1" });
});

// ... resto do código ...

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab10 Full Stack v1"));
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

## 🔄 Fluxo Completo de uma Venda

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           FLUXO DE VENDA COMPLETO                            │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  1. POST /api/vendas (IniciarVendaCommand)                                  │
│     │                                                                        │
│     ▼                                                                        │
│  ┌──────────────────────────────────────────────────────────────────────┐   │
│  │                     CQRS Pipeline Behaviors                          │   │
│  │  Logging → Validation → Tracing → Transaction → Handler             │   │
│  └──────────────────────────────────────────────────────────────────────┘   │
│     │                                                                        │
│     ▼                                                                        │
│  2. ProcessarVendaSaga (Orchestration)                                      │
│     │                                                                        │
│     ├─▶ ValidarClienteStep      ──┬── Sucesso ──┐                          │
│     │                              │             │                          │
│     ├─▶ ReservarEstoqueStep     ──┤             │                          │
│     │                              │   Compensar │                          │
│     ├─▶ ProcessarPagamentoStep  ──┤◀── Falha ───┤                          │
│     │                              │             │                          │
│     └─▶ CriarVendaStep          ──┴─────────────┘                          │
│     │                                                                        │
│     ▼                                                                        │
│  3. Domain Events (VendaCriadaEvent)                                        │
│     │                                                                        │
│     ▼                                                                        │
│  4. Outbox → RabbitMQ → Integration Events                                  │
│     │                                                                        │
│     ├─▶ Notification Service (Email)                                        │
│     └─▶ Analytics Service (Relatórios)                                      │
│                                                                              │
│  5. OpenTelemetry: Traces exportados para Jaeger                            │
│     Prometheus: Métricas coletadas                                          │
│     NLog: Logs estruturados                                                 │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

## ✅ Checklist de Implementação

### Domain Layer
- [ ] Criar todas as entidades com regras de domínio
- [ ] Implementar Value Objects (Email, CPF, Money)
- [ ] Definir Domain Events
- [ ] Definir interfaces de Repository

### Application Layer
- [ ] Criar Commands para todas as operações
- [ ] Criar Queries para leitura
- [ ] Implementar Handlers
- [ ] Criar Pipeline Behaviors (Logging, Validation, Tracing, Transaction)
- [ ] Implementar Saga para processo de venda
- [ ] Criar Validators com FluentValidation

### Infrastructure Layer
- [ ] Implementar Repositories
- [ ] Configurar DbContext (Write/Read)
- [ ] Implementar Event Publisher com RabbitMQ
- [ ] Criar Outbox Pattern
- [ ] Implementar Email Service
- [ ] Configurar OpenTelemetry

### WebAPI Layer
- [ ] Criar Controllers
- [ ] Implementar Middlewares
- [ ] Configurar Health Checks
- [ ] Configurar Swagger
- [ ] Criar Hosted Services

### DevOps
- [ ] Criar docker-compose.yml
- [ ] Configurar prometheus.yml
- [ ] Criar dashboards Grafana
- [ ] Configurar CI/CD (opcional)

## 💡 Conceitos Consolidados

Este laboratório consolida TODOS os conceitos aprendidos:

1. ✅ Clean Architecture (camadas bem definidas)
2. ✅ CQRS (Commands/Queries separados)
3. ✅ Mediator do Mvp24Hours (NÃO MediatR!)
4. ✅ Domain Events (dentro do bounded context)
5. ✅ Integration Events (entre serviços)
6. ✅ Saga Pattern (transações distribuídas)
7. ✅ Outbox Pattern (garantia de entrega)
8. ✅ RabbitMQ (messaging)
9. ✅ Repository/Unit of Work
10. ✅ Pipeline Behaviors (cross-cutting concerns)
11. ✅ OpenTelemetry (tracing distribuído)
12. ✅ Prometheus/Grafana (métricas)
13. ✅ NLog (logging estruturado)
14. ✅ Health Checks (monitoramento)
15. ✅ Value Objects do Mvp24Hours
16. ✅ FluentValidation
17. ✅ Docker/Docker Compose

## 📖 Lições Aprendidas Compartilhadas

Este lab pode utilizar lições aprendidas de outros labs para resolver problemas mais rapidamente. Consulte a pasta `../../learnings/` para acessar documentações de problemas comuns e soluções já implementadas.

**Localização da pasta:** `learnings/` (na raiz do projeto)

**Como usar:**
- Ao encontrar um problema ou erro, pesquise na pasta `learnings/` por documentações relacionadas
- As lições aprendidas incluem problemas comuns, soluções e boas práticas
- Exemplos: configurações de banco de dados, conflitos com frameworks, padrões de implementação

## 🔗 Ferramentas MCP Utilizadas

```
mvp24h_build_context({ 
  architecture: "cqrs", 
  resources: ["database", "caching", "observability", "messaging", "security", "testing"] 
})
mvp24h_cqrs_guide({ topic: "overview" })
mvp24h_cqrs_guide({ topic: "saga" })
mvp24h_messaging_patterns({ pattern: "rabbitmq" })
mvp24h_messaging_patterns({ pattern: "outbox" })
mvp24h_observability_setup({ component: "overview" })
mvp24h_infrastructure_guide({ topic: "pipeline" })
mvp24h_infrastructure_guide({ topic: "caching" })
mvp24h_testing_patterns({ topic: "overview" })
mvp24h_containerization_patterns({ topic: "docker-compose" })
```

---
**Nível de Complexidade**: ⭐⭐⭐⭐⭐ MASTER
