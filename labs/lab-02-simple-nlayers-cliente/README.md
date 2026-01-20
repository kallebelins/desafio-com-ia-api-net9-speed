# Lab 02 - Simple N-Layers com Cadastro de Cliente

## 🎯 Objetivo
Criar uma API REST para cadastro de clientes usando a arquitetura **Simple N-Layers** (3 camadas) do Mvp24Hours.

## 📋 Requisito de Negócio
- **Entidade**: Cliente
- **Campos**: Id, Nome, Email, Telefone, Ativo, DataCriacao
- **Operações**: CRUD completo com validação de email único

## 🏗️ Arquitetura
**Simple N-Layers** - Separação em 3 camadas: Core, Infrastructure e WebAPI.

```
Lab02.SimpleNLayers/
├── Lab02.SimpleNLayers.sln
├── src/
│   ├── Lab02.Core/
│   │   ├── Lab02.Core.csproj
│   │   ├── Entities/
│   │   │   └── Cliente.cs
│   │   ├── ValueObjects/
│   │   │   ├── ClienteDto.cs
│   │   │   ├── ClienteCreateDto.cs
│   │   │   └── ClienteUpdateDto.cs
│   │   └── Validators/
│   │       ├── ClienteCreateValidator.cs
│   │       └── ClienteUpdateValidator.cs
│   │
│   ├── Lab02.Infrastructure/
│   │   ├── Lab02.Infrastructure.csproj
│   │   └── Data/
│   │       ├── DataContext.cs
│   │       └── Configurations/
│   │           └── ClienteConfiguration.cs
│   │
│   └── Lab02.WebAPI/
│       ├── Lab02.WebAPI.csproj
│       ├── Program.cs
│       ├── appsettings.json
│       ├── Controllers/
│       │   └── ClienteController.cs
│       └── Extensions/
│           └── ServiceBuilderExtensions.cs
```

## 🔧 Recursos Utilizados

| Recurso | Descrição |
|---------|-----------|
| **3 Camadas** | Core, Infrastructure, WebAPI |
| **Repository Pattern** | `IRepositoryAsync<T>` do Mvp24Hours |
| **Unit of Work** | Transações com `IUnitOfWorkAsync` |
| **Validation** | FluentValidation com regras de negócio |
| **Controllers** | API Controllers tradicional |
| **AutoMapper** | Mapeamento de entidades para DTOs |

## 📦 Pacotes NuGet

### Core
```xml
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />
<PackageReference Include="FluentValidation" Version="11.*" />
```

### Infrastructure
```xml
<PackageReference Include="Mvp24Hours.Infrastructure.Data.EFCore" Version="9.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.*" />
```

### WebAPI
```xml
<PackageReference Include="Mvp24Hours.WebAPI" Version="9.*" />
<PackageReference Include="AutoMapper" Version="12.*" />
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
    "DefaultConnection": "Server=sqlserver;Database=Lab02_Clientes;User Id=sa;Password=Lab@Mvp24Hours!;TrustServerCertificate=True;"
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
| GET | `/api/clientes` | Listar todos os clientes (paginado) |
| GET | `/api/clientes/{id}` | Buscar cliente por ID |
| GET | `/api/clientes/email/{email}` | Buscar cliente por email |
| POST | `/api/clientes` | Criar novo cliente |
| PUT | `/api/clientes/{id}` | Atualizar cliente |
| DELETE | `/api/clientes/{id}` | Excluir cliente |

## 📚 Swagger

Este laboratório inclui documentação automática da API via Swagger.

### Configuração

**No arquivo `Program.cs`:**
```csharp
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Lab02 Simple N-Layers - Clientes", Version = "v1" });
});

// ... resto do código ...

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab02 Simple N-Layers v1"));
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

## ✅ Checklist de Implementação

- [ ] Criar solução com 3 projetos (Core, Infrastructure, WebAPI)
- [ ] Configurar referências entre projetos
- [ ] Criar entidade `Cliente` com `EntityBase<int>`
- [ ] Criar DTOs no Core layer
- [ ] Criar validadores com validação de email único
- [ ] Configurar DbContext no Infrastructure
- [ ] Criar Controller no WebAPI
- [ ] Configurar ServiceBuilderExtensions
- [ ] Configurar AutoMapper profiles
- [ ] Adicionar Health Checks
- [ ] Testar todos os endpoints

## 💡 Conceitos Aprendidos

1. Separação de responsabilidades em camadas
2. Inversão de dependências
3. Controllers tradicionais vs Minimal API
4. Validação com regra de negócio (email único)
5. Uso de AutoMapper para mapeamento
6. `Mvp24HoursContext` como base do DbContext

## 🔗 Ferramentas MCP Utilizadas

```
mvp24h_architecture_advisor({ complexity: "medium", entity_count: "few" })
mvp24h_get_template({ template_name: "simple-nlayers" })
mvp24h_database_advisor({ patterns: ["repository", "unit-of-work"] })
mvp24h_reference_guide({ topic: "mapping" })
```

---
**Nível de Complexidade**: ⭐⭐ Intermediário
