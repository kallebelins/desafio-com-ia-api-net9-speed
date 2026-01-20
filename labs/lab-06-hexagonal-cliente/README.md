# Lab 06 - Hexagonal (Ports & Adapters) com Cadastro de Cliente

## 🎯 Objetivo
Criar uma API REST para cadastro de clientes usando a arquitetura **Hexagonal (Ports & Adapters)**, garantindo isolamento total do domínio das dependências externas.

## 📋 Requisito de Negócio
- **Entidade**: Cliente
- **Campos**: Id, Nome, Email, CPF, Endereco (Value Object), Telefone, Ativo
- **Integrações**: Banco de dados, Serviço de Email, Serviço de Validação de CPF externo
- **Operações**: CRUD com envio de email de boas-vindas

## 🏗️ Arquitetura
**Hexagonal (Ports & Adapters)** - Domínio no centro, completamente isolado de infraestrutura.

```
Lab06.Hexagonal/
├── Lab06.Hexagonal.sln
├── src/
│   ├── Lab06.Domain/                    # Núcleo (SEM dependências externas)
│   │   ├── Lab06.Domain.csproj
│   │   ├── Entities/
│   │   │   └── Cliente.cs
│   │   ├── ValueObjects/
│   │   │   ├── Email.cs
│   │   │   ├── CPF.cs
│   │   │   └── Endereco.cs
│   │   └── Exceptions/
│   │       ├── DomainException.cs
│   │       └── ClienteNotFoundException.cs
│   │
│   ├── Lab06.Application/               # Orquestração (Use Cases)
│   │   ├── Lab06.Application.csproj
│   │   ├── Ports/
│   │   │   ├── Inbound/                 # Driving Ports (Use Cases)
│   │   │   │   ├── ICreateClienteUseCase.cs
│   │   │   │   ├── IGetClienteUseCase.cs
│   │   │   │   ├── IUpdateClienteUseCase.cs
│   │   │   │   └── IDeleteClienteUseCase.cs
│   │   │   └── Outbound/                # Driven Ports (Interfaces)
│   │   │       ├── IClienteRepository.cs
│   │   │       ├── IEmailService.cs
│   │   │       └── ICpfValidationService.cs
│   │   ├── UseCases/
│   │   │   ├── CreateClienteUseCase.cs
│   │   │   ├── GetClienteUseCase.cs
│   │   │   ├── UpdateClienteUseCase.cs
│   │   │   └── DeleteClienteUseCase.cs
│   │   ├── DTOs/
│   │   │   ├── Requests/
│   │   │   │   ├── CreateClienteRequest.cs
│   │   │   │   └── UpdateClienteRequest.cs
│   │   │   └── Responses/
│   │   │       ├── ClienteResponse.cs
│   │   │       └── ClienteListResponse.cs
│   │   └── Validators/
│   │       └── CreateClienteValidator.cs
│   │
│   ├── Lab06.Infrastructure/            # Adapters de saída
│   │   ├── Lab06.Infrastructure.csproj
│   │   └── Adapters/
│   │       └── Outbound/
│   │           ├── Persistence/
│   │           │   ├── DataContext.cs
│   │           │   ├── ClienteRepository.cs
│   │           │   └── Configurations/
│   │           │       └── ClienteConfiguration.cs
│   │           ├── Email/
│   │           │   └── SmtpEmailService.cs
│   │           └── ExternalServices/
│   │               └── CpfValidationService.cs
│   │
│   └── Lab06.WebAPI/                    # Adapter de entrada (HTTP)
│       ├── Lab06.WebAPI.csproj
│       ├── Program.cs
│       ├── Adapters/
│       │   └── Inbound/
│       │       └── Http/
│       │           └── Controllers/
│       │               └── ClienteController.cs
│       └── Extensions/
│           └── ServiceBuilderExtensions.cs
```

## 🔧 Recursos Utilizados

| Recurso | Descrição |
|---------|-----------|
| **Inbound Ports** | Interfaces de Use Cases (Driving) |
| **Outbound Ports** | Interfaces para infraestrutura (Driven) |
| **Inbound Adapters** | Controllers HTTP |
| **Outbound Adapters** | Repository, Email, APIs externas |
| **Value Objects** | Email, CPF, Endereco do Mvp24Hours |
| **Domain Entities** | Entidades puras sem dependências |

## 📦 Pacotes NuGet

### Domain (ZERO dependências externas!)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>
  <!-- SEM PackageReference! -->
</Project>
```

### Application
```xml
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />
<PackageReference Include="FluentValidation" Version="11.*" />
```

### Infrastructure
```xml
<PackageReference Include="Mvp24Hours.Infrastructure.Data.EFCore" Version="9.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.*" />
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
    "DefaultConnection": "Server=sqlserver;Database=Lab06_Clientes;User Id=sa;Password=Lab@Mvp24Hours!;TrustServerCertificate=True;"
  }
}
```

### Executar Infraestrutura

```bash
# Na pasta labs/
cd ..
docker-compose up -d sqlserver
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
    c.SwaggerDoc("v1", new() { Title = "Lab06 Hexagonal - Clientes", Version = "v1" });
});

// ... resto do código ...

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab06 Hexagonal v1"));
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

## 🎯 Ports & Adapters

### Inbound Port (Use Case Interface)
```csharp
// Application/Ports/Inbound/ICreateClienteUseCase.cs
public interface ICreateClienteUseCase
{
    Task<IBusinessResult<ClienteResponse>> ExecuteAsync(
        CreateClienteRequest request, 
        CancellationToken cancellationToken = default);
}
```

### Outbound Port (Repository Interface)
```csharp
// Application/Ports/Outbound/IClienteRepository.cs
public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Cliente?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IList<Cliente>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Cliente cliente, CancellationToken ct = default);
    Task UpdateAsync(Cliente cliente, CancellationToken ct = default);
    Task DeleteAsync(Cliente cliente, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
```

### Use Case Implementation
```csharp
// Application/UseCases/CreateClienteUseCase.cs
public class CreateClienteUseCase : ICreateClienteUseCase
{
    private readonly IClienteRepository _repository;
    private readonly IEmailService _emailService;
    private readonly ICpfValidationService _cpfService;

    public async Task<IBusinessResult<ClienteResponse>> ExecuteAsync(
        CreateClienteRequest request, 
        CancellationToken cancellationToken = default)
    {
        // 1. Validar CPF com serviço externo
        var cpfValido = await _cpfService.ValidateAsync(request.Cpf);
        if (!cpfValido)
            return default(ClienteResponse).ToBusiness("CPF inválido");

        // 2. Criar entidade de domínio
        var cliente = new Cliente(request.Nome, request.Email, request.Cpf);

        // 3. Persistir
        await _repository.AddAsync(cliente, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // 4. Enviar email (fire and forget)
        _ = _emailService.SendWelcomeEmailAsync(cliente.Email, cliente.Nome);

        return new ClienteResponse(...).ToBusiness();
    }
}
```

## 🔄 Regra de Dependência

```
                    ┌─────────────────────────────────┐
                    │           WebAPI                │
                    │    (Inbound Adapter: HTTP)      │
                    └──────────────┬──────────────────┘
                                   │
                                   ▼
                    ┌─────────────────────────────────┐
                    │         Application             │
                    │  ┌─────────────┬─────────────┐  │
                    │  │Inbound Ports│Outbound Ports│  │
                    │  └──────┬──────┴──────┬──────┘  │
                    │         │             │         │
                    │         └──── Use ────┘         │
                    │              Cases              │
                    └──────────────┬──────────────────┘
                                   │
                                   ▼
                    ┌─────────────────────────────────┐
                    │           Domain                │
                    │  (Entities, Value Objects)      │
                    │     NO EXTERNAL DEPENDENCIES    │
                    └─────────────────────────────────┘
                                   ▲
                                   │
                    ┌──────────────┴──────────────────┐
                    │        Infrastructure           │
                    │   (Outbound Adapters)           │
                    │ Repository, Email, External APIs │
                    └─────────────────────────────────┘
```

## ✅ Checklist de Implementação

- [ ] Criar Domain layer SEM dependências externas
- [ ] Criar Value Objects (Email, CPF, Endereco)
- [ ] Criar entidade Cliente com validações de domínio
- [ ] Definir Inbound Ports (Use Case interfaces)
- [ ] Definir Outbound Ports (Repository, Services interfaces)
- [ ] Implementar Use Cases
- [ ] Criar Outbound Adapters (Repository EF Core, SMTP Service)
- [ ] Criar Inbound Adapter (HTTP Controller)
- [ ] Registrar dependências no DI
- [ ] Testar isolamento do domínio

## 💡 Conceitos Aprendidos

1. Hexagonal Architecture (Ports & Adapters)
2. Driving Ports (Inbound) vs Driven Ports (Outbound)
3. Domínio completamente isolado
4. Dependency Inversion Principle na prática
5. Testabilidade através de ports mockáveis
6. Facilidade de trocar implementações

## 🧪 Testabilidade

A arquitetura Hexagonal permite testar Use Cases sem infraestrutura real:

```csharp
[Fact]
public async Task CreateCliente_ShouldSucceed_WhenCpfIsValid()
{
    // Arrange - Mock dos Outbound Ports
    var repositoryMock = new Mock<IClienteRepository>();
    var emailMock = new Mock<IEmailService>();
    var cpfMock = new Mock<ICpfValidationService>();
    cpfMock.Setup(x => x.ValidateAsync(It.IsAny<string>()))
           .ReturnsAsync(true);

    var useCase = new CreateClienteUseCase(
        repositoryMock.Object,
        emailMock.Object,
        cpfMock.Object);

    // Act
    var result = await useCase.ExecuteAsync(new CreateClienteRequest(...));

    // Assert
    Assert.True(result.HasData);
    repositoryMock.Verify(x => x.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

## 📖 Lições Aprendidas Compartilhadas

Este lab pode utilizar lições aprendidas de outros labs para resolver problemas mais rapidamente. Consulte a pasta `../../learnings/` para acessar documentações de problemas comuns e soluções já implementadas.

**Localização da pasta:** `learnings/` (na raiz do projeto)

**Como usar:**
- Ao encontrar um problema ou erro, pesquise na pasta `learnings/` por documentações relacionadas
- As lições aprendidas incluem problemas comuns, soluções e boas práticas
- Exemplos: configurações de banco de dados, conflitos com frameworks, padrões de implementação

## 🔗 Ferramentas MCP Utilizadas

```
mvp24h_get_template({ template_name: "hexagonal" })
mvp24h_core_patterns({ topic: "value-objects" })
mvp24h_core_patterns({ topic: "entity-interfaces" })
mvp24h_core_patterns({ topic: "infrastructure-abstractions" })
```

---
**Nível de Complexidade**: ⭐⭐⭐⭐ Avançado+
