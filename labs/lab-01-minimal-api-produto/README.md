# Lab 01 - Minimal API com Cadastro de Produto

## 🎯 Objetivo
Criar uma API REST simples para cadastro de produtos usando a arquitetura **Minimal API** do Mvp24Hours.

## 📋 Requisito de Negócio
- **Entidade**: Produto
- **Campos**: Id, Nome, Descrição, Preço, Ativo, DataCriacao
- **Operações**: CRUD completo (Create, Read, Update, Delete)

## 🏗️ Arquitetura
**Minimal API** - Estrutura de projeto único, ideal para microserviços e APIs simples.

```
Lab01.MinimalApi/
├── Lab01.MinimalApi.csproj
├── Program.cs
├── appsettings.json
├── Entities/
│   └── Produto.cs
├── ValueObjects/
│   ├── ProdutoDto.cs
│   ├── ProdutoCreateDto.cs
│   └── ProdutoUpdateDto.cs
├── Validators/
│   ├── ProdutoCreateValidator.cs
│   └── ProdutoUpdateValidator.cs
├── Data/
│   ├── DataContext.cs
│   └── Configurations/
│       └── ProdutoConfiguration.cs
├── Endpoints/
│   └── ProdutoEndpoints.cs
└── Extensions/
    └── ServiceBuilderExtensions.cs
```

## 🔧 Recursos Utilizados

| Recurso | Descrição |
|---------|-----------|
| **Repository Pattern** | `IRepositoryAsync<T>` do Mvp24Hours |
| **Unit of Work** | `IUnitOfWorkAsync` para transações |
| **Validation** | FluentValidation para validação de DTOs |
| **Entity Framework Core** | Persistência com SQL Server |
| **Swagger** | Documentação automática da API |
| **Health Checks** | Monitoramento da saúde da aplicação |

## 📦 Pacotes NuGet

```xml
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />
<PackageReference Include="Mvp24Hours.Infrastructure.Data.EFCore" Version="9.*" />
<PackageReference Include="Mvp24Hours.WebAPI" Version="9.*" />
<PackageReference Include="FluentValidation" Version="11.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.*" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.*" />
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
    "DefaultConnection": "Server=sqlserver;Database=Lab01_Produtos;User Id=sa;Password=Lab@Mvp24Hours!;TrustServerCertificate=True;"
  }
}
```

### Executar Infraestrutura

```bash
# Na pasta labs/
cd ..
docker-compose up -d sqlserver
```

## 🚀 Endpoints da API

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/produtos` | Listar todos os produtos |
| GET | `/api/produtos/{id}` | Buscar produto por ID |
| POST | `/api/produtos` | Criar novo produto |
| PUT | `/api/produtos/{id}` | Atualizar produto |
| DELETE | `/api/produtos/{id}` | Excluir produto |

## 📚 Swagger

Este laboratório inclui documentação automática da API via Swagger.

### Configuração

**No arquivo `Program.cs`:**
```csharp
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Lab01 Minimal API - Produtos", Version = "v1" });
});

// ... resto do código ...

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab01 Minimal API v1"));
}
```

**No arquivo `.csproj`:**
```xml
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.*" />
```

### Acessar Swagger UI

Após iniciar a aplicação, acesse:
- **Swagger UI**: `http://localhost:5000/swagger` ou `https://localhost:5001/swagger`
- **Swagger JSON**: `http://localhost:5000/swagger/v1/swagger.json`

## ✅ Checklist de Implementação

- [ ] Criar projeto Minimal API (.NET 9)
- [ ] Configurar pacotes NuGet do Mvp24Hours
- [ ] Criar entidade `Produto` com `EntityBase<int>`
- [ ] Criar DTOs (Create, Update, Response)
- [ ] Criar validadores com FluentValidation
- [ ] Configurar DbContext com Mvp24HoursContext
- [ ] Criar endpoints usando Minimal API
- [ ] Configurar Swagger
- [ ] Configurar Health Checks
- [ ] Testar todos os endpoints

## 💡 Conceitos Aprendidos

1. Estrutura de projeto único (Single Project)
2. Uso do `EntityBase<T>` do Mvp24Hours
3. Repository Pattern com `IUnitOfWorkAsync`
4. Validação com FluentValidation
5. Endpoints com Minimal API pattern
6. Extensões `ToBusiness()` e `ToBusinessPaging()`

## 🔗 Ferramentas MCP Utilizadas

```
mvp24h_get_template({ template_name: "minimal-api" })
mvp24h_database_advisor({ provider: "sqlserver", patterns: ["repository"] })
mvp24h_reference_guide({ topic: "validation" })
```

---
**Nível de Complexidade**: ⭐ Básico
