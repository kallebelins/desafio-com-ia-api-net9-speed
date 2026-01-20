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
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.*" />
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
