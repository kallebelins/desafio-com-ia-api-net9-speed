# Lab 03 - Complex N-Layers + CQRS com Cadastro de Produto

## 🎯 Objetivo
Criar uma API REST para cadastro de produtos usando **CQRS (Command Query Responsibility Segregation)** com o Mediator do Mvp24Hours.

## 📋 Requisito de Negócio
- **Entidade**: Produto
- **Campos**: Id, Nome, Descrição, Preço, Categoria, Estoque, Ativo, DataCriacao, DataAtualizacao
- **Operações**: CRUD separando Commands (escrita) de Queries (leitura)

## ⚠️ IMPORTANTE
**NÃO usar MediatR!** O Mvp24Hours possui implementação própria de CQRS/Mediator:
- Use `IMediatorCommand<T>`, `IMediatorQuery<T>` do namespace `Mvp24Hours.Infrastructure.Cqrs.Abstractions`

## 🏗️ Arquitetura
**Complex N-Layers + CQRS** - Separação de leitura e escrita com Mediator pattern.

```
Lab03.CQRS/
├── Lab03.CQRS.sln
├── src/
│   ├── Lab03.Core/
│   │   ├── Lab03.Core.csproj
│   │   ├── Entities/
│   │   │   └── Produto.cs
│   │   └── ValueObjects/
│   │       └── ProdutoDto.cs
│   │
│   ├── Lab03.Application/
│   │   ├── Lab03.Application.csproj
│   │   ├── Commands/
│   │   │   ├── CreateProdutoCommand.cs
│   │   │   ├── UpdateProdutoCommand.cs
│   │   │   └── DeleteProdutoCommand.cs
│   │   ├── Queries/
│   │   │   ├── GetProdutoByIdQuery.cs
│   │   │   └── GetAllProdutosQuery.cs
│   │   ├── Handlers/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateProdutoCommandHandler.cs
│   │   │   │   ├── UpdateProdutoCommandHandler.cs
│   │   │   │   └── DeleteProdutoCommandHandler.cs
│   │   │   └── Queries/
│   │   │       ├── GetProdutoByIdQueryHandler.cs
│   │   │       └── GetAllProdutosQueryHandler.cs
│   │   └── Validators/
│   │       ├── CreateProdutoValidator.cs
│   │       └── UpdateProdutoValidator.cs
│   │
│   ├── Lab03.Infrastructure/
│   │   ├── Lab03.Infrastructure.csproj
│   │   └── Data/
│   │       ├── DataContext.cs
│   │       └── Configurations/
│   │           └── ProdutoConfiguration.cs
│   │
│   └── Lab03.WebAPI/
│       ├── Lab03.WebAPI.csproj
│       ├── Program.cs
│       ├── Controllers/
│       │   └── ProdutoController.cs
│       └── Extensions/
│           └── ServiceBuilderExtensions.cs
```

## 🔧 Recursos Utilizados

| Recurso | Descrição |
|---------|-----------|
| **CQRS** | Separação de Commands e Queries |
| **Mediator** | `ISender` do Mvp24Hours (NÃO MediatR!) |
| **Commands** | `IMediatorCommand<T>` para escrita |
| **Queries** | `IMediatorQuery<T>` para leitura |
| **Behaviors** | Pipeline behaviors para validação |
| **Repository** | Para Handlers de escrita |

## 📦 Pacotes NuGet

### Application
```xml
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />
<PackageReference Include="Mvp24Hours.Infrastructure.Cqrs" Version="9.*" />
<PackageReference Include="FluentValidation" Version="11.*" />
```

## 🚀 Commands e Queries

### Commands (Escrita)
```csharp
// Criar produto
public record CreateProdutoCommand(
    string Nome, 
    string Descricao, 
    decimal Preco,
    string Categoria,
    int Estoque
) : IMediatorCommand<ProdutoDto>;

// Handler
public class CreateProdutoCommandHandler 
    : IMediatorCommandHandler<CreateProdutoCommand, ProdutoDto>
{
    public async Task<ProdutoDto> Handle(
        CreateProdutoCommand request, 
        CancellationToken cancellationToken) { ... }
}
```

### Queries (Leitura)
```csharp
// Buscar produto
public record GetProdutoByIdQuery(int Id) 
    : IMediatorQuery<ProdutoDto>;

// Handler
public class GetProdutoByIdQueryHandler 
    : IMediatorQueryHandler<GetProdutoByIdQuery, ProdutoDto>
{
    public async Task<ProdutoDto> Handle(
        GetProdutoByIdQuery request, 
        CancellationToken cancellationToken) { ... }
}
```

## ✅ Checklist de Implementação

- [ ] Criar solução com 4 projetos (Core, Application, Infrastructure, WebAPI)
- [ ] Configurar pacote CQRS do Mvp24Hours
- [ ] Criar Commands para Create, Update, Delete
- [ ] Criar Queries para GetById, GetAll
- [ ] Implementar CommandHandlers
- [ ] Implementar QueryHandlers
- [ ] Criar Validators para Commands
- [ ] Configurar registro do Mediator no DI
- [ ] Criar Controller usando ISender
- [ ] Testar separação de Commands e Queries

## 💡 Conceitos Aprendidos

1. Padrão CQRS (Command Query Responsibility Segregation)
2. Mediator pattern do Mvp24Hours (não MediatR!)
3. `IMediatorCommand<T>` vs `IMediatorQuery<T>`
4. Handlers específicos para cada operação
5. Separation of Concerns em nível de operação
6. Validação em Pipeline Behaviors

## 🔗 Ferramentas MCP Utilizadas

```
mvp24h_cqrs_guide({ topic: "overview" })
mvp24h_cqrs_guide({ topic: "commands" })
mvp24h_cqrs_guide({ topic: "queries" })
mvp24h_cqrs_guide({ topic: "behaviors" })
```

## 📝 Diferença MediatR vs Mvp24Hours

| MediatR (❌ NÃO USAR) | Mvp24Hours (✅ USAR) |
|----------------------|---------------------|
| `IRequest<T>` | `IMediatorCommand<T>` ou `IMediatorQuery<T>` |
| `IRequestHandler` | `IMediatorCommandHandler` ou `IMediatorQueryHandler` |
| `INotification` | `IMediatorNotification` |
| `IPipelineBehavior` | `IPipelineBehavior` (do Mvp24Hours) |

---
**Nível de Complexidade**: ⭐⭐⭐ Avançado
