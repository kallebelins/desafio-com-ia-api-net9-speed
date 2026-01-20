# Lab 08 - Lições Aprendidas: Clean Architecture com Mvp24Hours

## Data: 2026-01-20

## Contexto
Implementação de um sistema completo de vendas (Cliente, Produto, Venda, ItemVenda) usando Clean Architecture com 4 camadas bem definidas.

---

## 🔴 Problema 1: IEntityBase não encontrado no Mvp24Hours.Core

### Descrição
Ao tentar usar `IEntityBase` junto com `EntityBase<TKey>` nas entidades, o compilador retornou erro:
```
error CS0246: O nome do tipo ou do namespace "IEntityBase" não pode ser encontrado
```

### Causa
A interface `IEntityBase` não existe no namespace `Mvp24Hours.Core.Contract.Domain`. O framework Mvp24Hours usa apenas a classe base `EntityBase<TKey>`.

### Solução
Remover a implementação de `IEntityBase` e usar apenas a herança de `EntityBase<TKey>`:

```csharp
// ❌ ERRADO
public class Cliente : EntityBase<int>, IEntityBase

// ✅ CORRETO
public class Cliente : EntityBase<int>
```

### Prevenção
Consultar a documentação do Mvp24Hours antes de usar interfaces. As interfaces disponíveis são:
- `IEntityBase<TKey>` - Interface base para entidades tipadas
- Não confundir com interfaces de auditoria: `IEntityLog`, `IEntityLogDate`

---

## 🟡 Problema 2: PowerShell não aceita operador &&

### Descrição
Ao executar comandos encadeados com `&&` no PowerShell, ocorre erro:
```
O token '&&' não é um separador de instruções válido nesta versão.
```

### Solução
Usar `;` (ponto e vírgula) em vez de `&&`:

```powershell
# ❌ ERRADO (PowerShell)
cd Lab08.CleanArchitecture && dotnet build

# ✅ CORRETO (PowerShell)
cd Lab08.CleanArchitecture; dotnet build
```

---

## 🟢 Boas Práticas Aplicadas

### 1. Value Objects com Validação
Implementação de Value Objects imutáveis com validação no construtor:

```csharp
public sealed class Email : IEquatable<Email>
{
    private Email(string valor) => Valor = valor;
    
    public static Email Create(string email)
    {
        // Validação completa antes de criar
        if (!IsValid(email))
            throw new DomainException("Email inválido");
        return new Email(email.ToLowerInvariant().Trim());
    }
    
    public static bool IsValid(string email) => /* validação */;
}
```

### 2. Owned Types no EF Core para Value Objects
Configuração correta de Value Objects como Owned Types:

```csharp
builder.OwnsOne(c => c.Email, email =>
{
    email.Property(e => e.Valor)
        .HasColumnName("Email")
        .IsRequired()
        .HasMaxLength(256);
});
```

### 3. Domain Service para Lógica Cross-Entity
Uso de Domain Service para operações que envolvem múltiplas entidades:

```csharp
public class VendaDomainService
{
    public Venda CriarVenda(Cliente cliente, IEnumerable<(Produto, int)> itens)
    {
        // Validações de regras de negócio
        // Criação da venda com todos os itens
        // Mantém consistência do agregado
    }
}
```

### 4. Use Case Pattern Simples
Interface genérica para Use Cases sem dependência de MediatR:

```csharp
public interface IUseCase<in TInput, TOutput>
{
    Task<TOutput> ExecuteAsync(TInput input, CancellationToken ct = default);
}
```

### 5. Encapsulamento de Coleções no Aggregate Root
Uso de backing field privado para coleções:

```csharp
public class Venda : EntityBase<int>
{
    private readonly List<ItemVenda> _itens = new();
    public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();
    
    public ItemVenda AdicionarItem(Produto produto, int quantidade)
    {
        // Lógica de adicionar item
        _itens.Add(item);
        RecalcularTotal();
        return item;
    }
}
```

---

## 📦 Pacotes Utilizados

| Pacote | Versão | Camada | Uso |
|--------|--------|--------|-----|
| Mvp24Hours.Core | 9.* | Domain | EntityBase, ValueObjects |
| FluentValidation | 11.* | Application | Validação de inputs |
| AutoMapper | 13.* | Application | Mapeamento de DTOs |
| Mvp24Hours.Infrastructure.Data.EFCore | 9.* | Infrastructure | DbContext, Repository |
| Microsoft.EntityFrameworkCore.SqlServer | 9.* | Infrastructure | Provider SQL Server |
| Swashbuckle.AspNetCore | 7.* | WebAPI | Swagger/OpenAPI |

---

## 🏗️ Estrutura Final do Projeto

```
Lab08.CleanArchitecture/
├── src/
│   ├── Lab08.Domain/
│   │   ├── Entities/         # Entidades de domínio
│   │   ├── ValueObjects/     # VOs: Email, CPF, Money, Endereco
│   │   ├── Enums/            # StatusVenda
│   │   ├── Interfaces/       # Contratos de repositório
│   │   ├── Services/         # VendaDomainService
│   │   └── Exceptions/       # DomainException
│   │
│   ├── Lab08.Application/
│   │   ├── Interfaces/       # IUseCase, IDateTimeService
│   │   ├── DTOs/             # Data Transfer Objects
│   │   ├── UseCases/         # Casos de uso organizados por feature
│   │   └── Validators/       # FluentValidation validators
│   │
│   ├── Lab08.Infrastructure/
│   │   ├── Data/
│   │   │   ├── DataContext.cs
│   │   │   ├── UnitOfWork.cs
│   │   │   ├── Configurations/  # EF Core configurations
│   │   │   └── Repositories/    # Implementações
│   │   └── Services/
│   │
│   └── Lab08.WebAPI/
│       ├── Controllers/
│       ├── Extensions/
│       ├── Program.cs
│       └── appsettings.json
```

---

## 🎯 Endpoints da API

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | /api/cliente | Lista clientes |
| GET | /api/cliente/{id} | Busca cliente por ID |
| POST | /api/cliente | Cria cliente |
| GET | /api/categoria | Lista categorias |
| POST | /api/categoria | Cria categoria |
| GET | /api/produto | Lista produtos |
| GET | /api/produto/{id} | Busca produto por ID |
| POST | /api/produto | Cria produto |
| GET | /api/venda/{id} | Busca venda por ID |
| POST | /api/venda | Cria venda |
| POST | /api/venda/{id}/confirmar | Confirma venda (baixa estoque) |
| GET | /api/venda/relatorio | Relatório de vendas por período |

---

## 📝 Referências

- [Mvp24Hours Entity Interfaces](https://mvp24hours.dev/#/docs/core/entity-interfaces)
- [Clean Architecture - Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Value Objects in DDD](https://martinfowler.com/bliki/ValueObject.html)
