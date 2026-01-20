# Lab 08 - Clean Architecture com Sistema Completo

## 🎯 Objetivo
Criar um sistema completo de vendas (Cliente, Produto, Venda) usando **Clean Architecture** com todas as camadas bem definidas e separação clara de responsabilidades.

## 📋 Requisito de Negócio
- **Entidades**: Cliente, Produto, Venda, ItemVenda
- **Funcionalidades**:
  - Cadastro completo de Clientes
  - Cadastro completo de Produtos com categorias
  - Registro de Vendas com múltiplos produtos
  - Relatório de vendas por período
  - Relatório de produtos mais vendidos

## 🏗️ Arquitetura
**Clean Architecture** - Camadas concêntricas com dependência apontando para o centro (Domain).

```
Lab08.CleanArchitecture/
├── Lab08.CleanArchitecture.sln
├── src/
│   ├── Lab08.Domain/                    # Enterprise Business Rules
│   │   ├── Lab08.Domain.csproj
│   │   ├── Entities/
│   │   │   ├── Cliente.cs
│   │   │   ├── Produto.cs
│   │   │   ├── Categoria.cs
│   │   │   ├── Venda.cs
│   │   │   └── ItemVenda.cs
│   │   ├── ValueObjects/
│   │   │   ├── Email.cs
│   │   │   ├── CPF.cs
│   │   │   ├── Money.cs
│   │   │   └── Endereco.cs
│   │   ├── Interfaces/
│   │   │   ├── IClienteRepository.cs
│   │   │   ├── IProdutoRepository.cs
│   │   │   └── IVendaRepository.cs
│   │   ├── Services/
│   │   │   └── VendaDomainService.cs
│   │   └── Exceptions/
│   │       └── DomainException.cs
│   │
│   ├── Lab08.Application/               # Application Business Rules
│   │   ├── Lab08.Application.csproj
│   │   ├── UseCases/
│   │   │   ├── Clientes/
│   │   │   │   ├── CreateCliente/
│   │   │   │   │   ├── CreateClienteUseCase.cs
│   │   │   │   │   ├── CreateClienteInput.cs
│   │   │   │   │   └── CreateClienteOutput.cs
│   │   │   │   ├── GetCliente/
│   │   │   │   └── ListClientes/
│   │   │   ├── Produtos/
│   │   │   │   ├── CreateProduto/
│   │   │   │   ├── GetProduto/
│   │   │   │   └── ListProdutos/
│   │   │   └── Vendas/
│   │   │       ├── CreateVenda/
│   │   │       ├── GetVenda/
│   │   │       └── RelatorioVendas/
│   │   ├── Interfaces/
│   │   │   └── IUseCase.cs
│   │   ├── DTOs/
│   │   │   ├── ClienteDto.cs
│   │   │   ├── ProdutoDto.cs
│   │   │   └── VendaDto.cs
│   │   └── Validators/
│   │       ├── CreateClienteValidator.cs
│   │       ├── CreateProdutoValidator.cs
│   │       └── CreateVendaValidator.cs
│   │
│   ├── Lab08.Infrastructure/            # Interface Adapters (Data)
│   │   ├── Lab08.Infrastructure.csproj
│   │   ├── Data/
│   │   │   ├── DataContext.cs
│   │   │   ├── Configurations/
│   │   │   │   ├── ClienteConfiguration.cs
│   │   │   │   ├── ProdutoConfiguration.cs
│   │   │   │   └── VendaConfiguration.cs
│   │   │   └── Repositories/
│   │   │       ├── ClienteRepository.cs
│   │   │       ├── ProdutoRepository.cs
│   │   │       └── VendaRepository.cs
│   │   └── Services/
│   │       └── DateTimeService.cs
│   │
│   └── Lab08.WebAPI/                    # Interface Adapters (Presentation)
│       ├── Lab08.WebAPI.csproj
│       ├── Program.cs
│       ├── appsettings.json
│       ├── Controllers/
│       │   ├── ClienteController.cs
│       │   ├── ProdutoController.cs
│       │   └── VendaController.cs
│       ├── Presenters/
│       │   └── JsonPresenter.cs
│       └── Extensions/
│           └── ServiceBuilderExtensions.cs
│
└── tests/
    ├── Lab08.Domain.Tests/
    ├── Lab08.Application.Tests/
    └── Lab08.Integration.Tests/
```

## 🔧 Recursos Utilizados

| Recurso | Descrição |
|---------|-----------|
| **Clean Architecture** | 4 camadas concêntricas |
| **Use Cases** | Application Business Rules |
| **Domain Services** | Lógica que não pertence a uma entidade |
| **Repository Pattern** | Interfaces no Domain, implementação na Infrastructure |
| **Value Objects** | Email, CPF, Money do Mvp24Hours |
| **Presenters** | Formatação de saída |

## 📦 Pacotes NuGet

### Domain (mínimo de dependências)
```xml
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />
```

### Application
```xml
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />
<PackageReference Include="FluentValidation" Version="11.*" />
<PackageReference Include="AutoMapper" Version="12.*" />
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
    "DefaultConnection": "Server=sqlserver;Database=Lab08_Vendas;User Id=sa;Password=Lab@Mvp24Hours!;TrustServerCertificate=True;"
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
    c.SwaggerDoc("v1", new() { Title = "Lab08 Clean Architecture - Sistema Completo", Version = "v1" });
});

// ... resto do código ...

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab08 Clean Architecture v1"));
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

## 🎯 Camadas da Clean Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Frameworks & Drivers                      │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │                    Interface Adapters                       │ │
│  │  ┌───────────────────────────────────────────────────────┐ │ │
│  │  │               Application Business Rules              │ │ │
│  │  │  ┌─────────────────────────────────────────────────┐  │ │ │
│  │  │  │           Enterprise Business Rules             │  │ │ │
│  │  │  │                   (Domain)                      │  │ │ │
│  │  │  │                                                 │  │ │ │
│  │  │  │   Entities, Value Objects, Domain Services      │  │ │ │
│  │  │  │                                                 │  │ │ │
│  │  │  └─────────────────────────────────────────────────┘  │ │ │
│  │  │                                                       │ │ │
│  │  │   Use Cases, Input/Output Boundaries, DTOs            │ │ │
│  │  │                                                       │ │ │
│  │  └───────────────────────────────────────────────────────┘ │ │
│  │                                                            │ │
│  │   Controllers, Presenters, Gateways, Repositories          │ │
│  │                                                            │ │
│  └────────────────────────────────────────────────────────────┘ │
│                                                                  │
│   Web, Devices, DB, External Interfaces, UI                      │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

## 📝 Use Case Pattern

### Interface Base
```csharp
public interface IUseCase<TInput, TOutput>
{
    Task<TOutput> ExecuteAsync(TInput input, CancellationToken ct = default);
}
```

### Implementação
```csharp
public class CreateVendaUseCase : IUseCase<CreateVendaInput, CreateVendaOutput>
{
    private readonly IClienteRepository _clienteRepo;
    private readonly IProdutoRepository _produtoRepo;
    private readonly IVendaRepository _vendaRepo;
    private readonly VendaDomainService _vendaService;

    public async Task<CreateVendaOutput> ExecuteAsync(
        CreateVendaInput input, 
        CancellationToken ct = default)
    {
        // 1. Buscar cliente
        var cliente = await _clienteRepo.GetByIdAsync(input.ClienteId, ct);
        if (cliente == null)
            throw new DomainException("Cliente não encontrado");

        // 2. Buscar produtos e validar
        var produtos = await _produtoRepo.GetByIdsAsync(
            input.Itens.Select(i => i.ProdutoId), ct);

        // 3. Usar Domain Service para criar venda
        var venda = _vendaService.CriarVenda(cliente, produtos, input.Itens);

        // 4. Persistir
        await _vendaRepo.AddAsync(venda, ct);
        await _vendaRepo.SaveChangesAsync(ct);

        // 5. Retornar output
        return new CreateVendaOutput(venda.Id, venda.Total);
    }
}
```

## 📊 Relatórios

### Use Case de Relatório
```csharp
public class RelatorioVendasUseCase 
    : IUseCase<RelatorioVendasInput, RelatorioVendasOutput>
{
    public async Task<RelatorioVendasOutput> ExecuteAsync(
        RelatorioVendasInput input, 
        CancellationToken ct = default)
    {
        var vendas = await _vendaRepo.GetByPeriodoAsync(
            input.DataInicio, 
            input.DataFim, 
            ct);

        return new RelatorioVendasOutput
        {
            TotalVendas = vendas.Count,
            ValorTotal = vendas.Sum(v => v.Total),
            MediaPorVenda = vendas.Average(v => v.Total),
            ProdutosMaisVendidos = CalcularProdutosMaisVendidos(vendas)
        };
    }
}
```

## ✅ Checklist de Implementação

- [ ] Criar estrutura de 4 projetos (Domain, Application, Infrastructure, WebAPI)
- [ ] Criar entidades de domínio com regras de negócio
- [ ] Criar Value Objects (Email, CPF, Money)
- [ ] Definir interfaces de repository no Domain
- [ ] Criar Domain Services para lógica complexa
- [ ] Implementar Use Cases para cada operação
- [ ] Criar Input/Output para cada Use Case
- [ ] Implementar Repositories na Infrastructure
- [ ] Criar Controllers que chamam Use Cases
- [ ] Implementar Presenters para formatação
- [ ] Criar testes unitários para Domain e Application
- [ ] Criar testes de integração para Infrastructure

## 💡 Conceitos Aprendidos

1. Clean Architecture de Uncle Bob
2. Dependency Rule (dependências apontam para dentro)
3. Use Cases como Application Business Rules
4. Domain Services para lógica cross-entity
5. Input/Output Boundaries
6. Presenters para formatação de resposta
7. Testabilidade em cada camada

## 🧪 Estrutura de Testes

```
tests/
├── Lab08.Domain.Tests/
│   ├── Entities/
│   │   └── VendaTests.cs
│   └── ValueObjects/
│       └── MoneyTests.cs
│
├── Lab08.Application.Tests/
│   └── UseCases/
│       ├── CreateVendaUseCaseTests.cs
│       └── RelatorioVendasUseCaseTests.cs
│
└── Lab08.Integration.Tests/
    └── Repositories/
        └── VendaRepositoryTests.cs
```

## 🔗 Ferramentas MCP Utilizadas

```
mvp24h_architecture_advisor({ complexity: "high", business_rules: "complex" })
mvp24h_get_template({ template_name: "clean-architecture" })
mvp24h_core_patterns({ topic: "value-objects" })
mvp24h_core_patterns({ topic: "entity-interfaces" })
mvp24h_testing_patterns({ topic: "unit-testing" })
mvp24h_testing_patterns({ topic: "integration-testing" })
```

---
**Nível de Complexidade**: ⭐⭐⭐⭐⭐ Expert
