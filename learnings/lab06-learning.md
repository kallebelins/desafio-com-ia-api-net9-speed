# Lições Aprendidas - Lab 06 Hexagonal (Ports & Adapters) com Cliente

## Data: 2026-01-20

## Resumo
Implementação de API REST para cadastro de clientes usando a arquitetura **Hexagonal (Ports & Adapters)** com Mvp24Hours Framework. A arquitetura garante isolamento total do domínio das dependências externas.

---

## Problema 1: Método ToBusiness não encontrado (String/List)

### Descrição
Os métodos de extensão `ToBusiness<T>()` do Mvp24Hours não funcionam em strings ou listas diretamente.

### Erro
```
error CS1929: "string" não contém uma definição para "ToBusiness" e a melhor sobrecarga 
do método de extensão requer um receptor do tipo "IPipelineMessage"
```

### Solução
Criar BusinessResult diretamente usando o construtor:

```csharp
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;

// ✅ Para sucesso - usar construtor com data
return new BusinessResult<ClienteResponse>(response);

// ✅ Para erro - criar método helper
private static IBusinessResult<T> CreateErrorResult<T>(string message)
{
    IReadOnlyCollection<IMessageResult> messages = new List<IMessageResult>
    {
        new MessageResult(message, MessageType.Error)
    };
    return new BusinessResult<T>(default!, messages);
}
```

---

## Problema 2: FluentValidation Downgrade

### Descrição
Conflito de versões entre FluentValidation 11.* e versão usada pelo Mvp24Hours.

### Solução
Usar versão 12.* do FluentValidation:

```xml
<PackageReference Include="FluentValidation" Version="12.*" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="12.*" />
```

---

## Problema 3: Conflito de Namespace com Value Objects

### Descrição
O namespace `Lab06.Infrastructure.Adapters.Outbound.Email` conflita com o Value Object `Email` de `Lab06.Domain.ValueObjects`.

### Erro
```
error CS0234: O nome de tipo ou namespace "Create" não existe no namespace "...Email"
```

### Solução
Usar aliases para os Value Objects:

```csharp
// Alias para evitar conflito com namespace
using DomainEmail = Lab06.Domain.ValueObjects.Email;
using DomainCPF = Lab06.Domain.ValueObjects.CPF;

// Uso no código
builder.Property(c => c.Email)
    .HasConversion(
        email => email.Value,
        value => DomainEmail.Create(value));
```

---

## Problema 4: EF Core não consegue mapear construtor de Value Object

### Descrição
O EF Core não consegue fazer binding do construtor do Value Object porque os nomes dos parâmetros não correspondem às propriedades.

### Erro
```
No suitable constructor was found for entity type 'Endereco'.
Cannot bind 'cep' in 'Endereco(string logradouro, ... string cep)'
```

### Solução
Adicionar um construtor sem parâmetros privado para EF Core:

```csharp
public sealed class Endereco : IEquatable<Endereco>
{
    // Propriedades com setters privados para EF Core
    public string Logradouro { get; private set; } = string.Empty;
    public string CEP { get; private set; } = string.Empty;
    // ... outras propriedades

    // Construtor sem parâmetros para EF Core
#pragma warning disable CS8618
    private Endereco() { }
#pragma warning restore CS8618

    // Construtor privado com parâmetros
    private Endereco(string logradouro, ...) { ... }

    // Factory method público
    public static Endereco Create(...) { ... }
}
```

---

## Problema 5: EF Core não traduz propriedades de Value Objects em LINQ

### Descrição
Queries LINQ que acessam propriedades de Value Objects (como `c.Email.Value`) não podem ser traduzidas para SQL.

### Erro
```
The LINQ expression 'DbSet<Cliente>()
    .Where(c => c.Email.Value == __emailLower_0)' could not be translated.
```

### Solução
Carregar entidades em memória antes de filtrar por propriedades de Value Objects:

```csharp
public async Task<Cliente?> GetByEmailAsync(string email, CancellationToken ct = default)
{
    var emailLower = email.ToLowerInvariant().Trim();
    // Carrega todos os clientes e filtra em memória
    var clientes = await _context.Clientes.ToListAsync(ct);
    return clientes.FirstOrDefault(c => c.Email.Value == emailLower);
}
```

**Nota**: Para produção com muitos registros, considere criar um índice específico ou usar stored procedures.

---

## Problema 6: Tabela não criada pelo EnsureCreatedAsync

### Descrição
Ao usar banco existente de outro lab, `EnsureCreatedAsync()` não cria novas tabelas porque o banco já existe.

### Solução
Para desenvolvimento, usar `EnsureDeletedAsync()` + `EnsureCreatedAsync()`:

```csharp
public static async Task ConfigureDatabaseAsync(this WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    
    // Recria o banco de dados para garantir schema correto
    // Em produção, usar migrações!
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();
}
```

---

## Padrões Implementados

### 1. Arquitetura Hexagonal (Ports & Adapters)

```
┌─────────────────────────────────────────────────────────────┐
│                        WebAPI                               │
│  (Inbound Adapter: HTTP Controllers)                        │
└─────────────────────────────┬───────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                     Application                             │
│  ┌─────────────────┐     ┌─────────────────┐               │
│  │ Inbound Ports   │     │ Outbound Ports  │               │
│  │ (Use Cases)     │     │ (Interfaces)    │               │
│  └────────┬────────┘     └────────┬────────┘               │
│           └───────────┬───────────┘                         │
│                 Use Cases                                   │
└───────────────────────┼─────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                       Domain                                │
│  (Entities, Value Objects, Domain Exceptions)               │
│  NO EXTERNAL DEPENDENCIES                                   │
└─────────────────────────────────────────────────────────────┘
                        ▲
                        │
┌───────────────────────┴─────────────────────────────────────┐
│                    Infrastructure                           │
│  (Outbound Adapters: Repository, Email, External Services)  │
└─────────────────────────────────────────────────────────────┘
```

### 2. Inbound Ports (Driving Ports)
Interfaces que definem os Use Cases:

```csharp
public interface ICreateClienteUseCase
{
    Task<IBusinessResult<ClienteResponse>> ExecuteAsync(
        CreateClienteRequest request,
        CancellationToken cancellationToken = default);
}
```

### 3. Outbound Ports (Driven Ports)
Interfaces para serviços externos:

```csharp
public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Cliente cliente, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string to, string customerName, CancellationToken ct = default);
}

public interface ICpfValidationService
{
    Task<bool> ValidateAsync(string cpf, CancellationToken ct = default);
}
```

### 4. Value Objects no Domínio
Value Objects com validação própria:

```csharp
public sealed class Email : IEquatable<Email>
{
    public string Value { get; }
    
    public static Email Create(string value)
    {
        // Validação
        return new Email(value.ToLowerInvariant().Trim());
    }
}
```

### 5. EF Core Value Conversion
Configuração para persistir Value Objects:

```csharp
builder.Property(c => c.Email)
    .HasConversion(
        email => email.Value,
        value => DomainEmail.Create(value))
    .IsRequired()
    .HasMaxLength(256);
```

---

## Estrutura de Pastas Final

```
Lab06.Hexagonal/
├── Lab06.Hexagonal.sln
├── src/
│   ├── Lab06.Domain/                    # Núcleo (SEM dependências externas)
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
│   │   ├── Ports/
│   │   │   ├── Inbound/                 # Driving Ports
│   │   │   │   ├── ICreateClienteUseCase.cs
│   │   │   │   ├── IGetClienteUseCase.cs
│   │   │   │   ├── IUpdateClienteUseCase.cs
│   │   │   │   └── IDeleteClienteUseCase.cs
│   │   │   └── Outbound/                # Driven Ports
│   │   │       ├── IClienteRepository.cs
│   │   │       ├── IEmailService.cs
│   │   │       └── ICpfValidationService.cs
│   │   ├── UseCases/
│   │   ├── DTOs/
│   │   └── Validators/
│   │
│   ├── Lab06.Infrastructure/            # Adapters de saída
│   │   └── Adapters/Outbound/
│   │       ├── Persistence/
│   │       ├── Email/
│   │       └── ExternalServices/
│   │
│   └── Lab06.WebAPI/                    # Adapter de entrada (HTTP)
│       ├── Program.cs
│       ├── Adapters/Inbound/Http/Controllers/
│       └── Extensions/
```

---

## Versões Utilizadas

| Pacote | Versão |
|--------|--------|
| .NET SDK | 9.0 |
| Mvp24Hours.Core | 9.* |
| Mvp24Hours.Infrastructure.Data.EFCore | 9.* |
| Microsoft.EntityFrameworkCore.SqlServer | 9.* |
| FluentValidation | 12.* |
| Mvp24Hours.WebAPI | 9.* |
| Swashbuckle.AspNetCore | 7.* |

---

## Como Executar

1. Subir infraestrutura (SQL Server):
```bash
cd labs
docker-compose up -d sqlserver
```

2. Executar a API:
```bash
cd lab-06-hexagonal-cliente/Lab06.Hexagonal
dotnet run --project src/Lab06.WebAPI --urls "http://localhost:5006"
```

3. Endpoints disponíveis:
   - Swagger UI: `http://localhost:5006/swagger`
   - GET todos: `GET /api/cliente`
   - GET por ID: `GET /api/cliente/{id}`
   - POST criar: `POST /api/cliente`
   - PUT atualizar: `PUT /api/cliente/{id}`
   - DELETE remover: `DELETE /api/cliente/{id}`

---

## Benefícios da Arquitetura Hexagonal

1. **Isolamento do Domínio**: Zero dependências externas no Domain layer
2. **Testabilidade**: Ports podem ser mockados facilmente
3. **Flexibilidade**: Trocar implementações sem afetar o domínio
4. **Dependency Inversion**: Dependências apontam para abstrações
5. **Separação de Responsabilidades**: Cada camada tem função clara

---

## Tags
`mvp24hours` `hexagonal` `ports-adapters` `domain-driven` `value-objects` `ef-core` `clean-architecture`
