# Lições Aprendidas - Lab 02

## EnableRetryOnFailure vs Mvp24Hours Repository Pattern

### 📋 Problema Inicial

Durante o desenvolvimento, encontramos o seguinte erro de falha transitória:

```
System.InvalidOperationException: An exception has been raised that is likely due to a transient failure. 
Consider enabling transient error resiliency by adding 'EnableRetryOnFailure' to the 'UseSqlServer' call.
```

**Causa inicial:**
- Falhas transitórias de rede ao conectar com o SQL Server
- Conexões intermitentes durante inicialização do banco de dados

### ❌ Tentativa de Solução (que causou outro problema)

Adicionamos `EnableRetryOnFailure` na configuração:

```csharp
// ⚠️ CAUSOU CONFLITO COM MVP24HOURS
services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));
```

### 🔴 Novo Erro Gerado

Após adicionar `EnableRetryOnFailure`, surgiu um novo erro:

```
System.InvalidOperationException: The configured execution strategy 
'SqlServerRetryingExecutionStrategy' does not support user-initiated transactions. 
Use the execution strategy returned by 'DbContext.Database.CreateExecutionStrategy()' 
to execute all the operations in the transaction as a retriable unit.
```

### 🔍 Causa Raiz

O `EnableRetryOnFailure()` cria um `SqlServerRetryingExecutionStrategy` que **não é compatível** com transações iniciadas pelo usuário. O Mvp24Hours Framework usa internamente transações através do padrão Repository/UnitOfWork:

1. `IRepositoryAsync<T>` - Operações de repositório
2. `IUnitOfWorkAsync` - Controle de transações
3. Internamente, métodos como `GetByAnyAsync()` podem usar operações transacionais

**Por que o conflito ocorre:**
- Se uma transação falha no meio da execução, o retry strategy não consegue saber o estado da transação parcialmente executada
- O EF Core bloqueia este cenário para evitar inconsistências de dados

### ✅ Solução Final

**Remover** o `EnableRetryOnFailure` para projetos que usam Mvp24Hours com Repository/UoW:

**Localização:** `src/Lab02.WebAPI/Extensions/ServiceBuilderExtensions.cs`

```csharp
// ✅ CONFIGURAÇÃO CORRETA PARA MVP24HOURS
services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection")));
```

### 📊 Comparativo

| Cenário | EnableRetryOnFailure | Resultado |
|---------|---------------------|-----------|
| EF Core direto (sem transações manuais) | ✅ Funciona | Retry automático |
| Mvp24Hours Repository/UoW | ❌ Conflito | Exception |
| Transações com `BeginTransaction()` | ❌ Conflito | Exception |
| Queries simples sem transações | ✅ Funciona | Retry automático |

### 💡 Lições Aprendidas

1. **Frameworks com UoW interno**: Cuidado ao usar `EnableRetryOnFailure` com frameworks que gerenciam transações internamente (Mvp24Hours, ABP, etc.)

2. **Retry Strategy + Transactions = Conflito**: O EF Core não permite retry automático quando há transações de usuário

3. **Simplicidade primeiro**: Para projetos simples ou de laboratório, a configuração padrão do SQL Server é suficiente

4. **Resiliência alternativa**: Se precisar de resiliência, considere outras abordagens

### 🔄 Alternativas para Resiliência (se necessário)

Se você realmente precisa de resiliência com retry, use uma das abordagens abaixo:

#### 1. Polly no nível HTTP (Recomendado)

```csharp
// Resilience no nível do HTTP Client, não no banco
builder.Services.AddHttpClient<IMyService, MyService>()
    .AddStandardResilienceHandler();
```

#### 2. Execution Strategy Manual

```csharp
// Envolver operações transacionais manualmente
var strategy = context.Database.CreateExecutionStrategy();
await strategy.ExecuteAsync(async () =>
{
    using var transaction = await context.Database.BeginTransactionAsync();
    // ... operações ...
    await transaction.CommitAsync();
});
```

#### 3. Retry apenas para inicialização

```csharp
// Retry apenas na criação inicial do banco
var retryCount = 0;
while (retryCount < 5)
{
    try
    {
        context.Database.EnsureCreated();
        break;
    }
    catch (SqlException)
    {
        retryCount++;
        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}
```

### ⚠️ Quando usar EnableRetryOnFailure

**Use quando:**
- ✅ Usando EF Core diretamente sem framework de abstração
- ✅ Sem transações manuais (`BeginTransaction`)
- ✅ Operações CRUD simples sem UnitOfWork

**NÃO use quando:**
- ❌ Usando Mvp24Hours com Repository/UoW
- ❌ Usando outros frameworks com transações internas
- ❌ Usando transações explícitas no código

### 📖 Referências

- [EF Core Connection Resiliency](https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency)
- [EF Core Execution Strategies](https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency#execution-strategies-and-transactions)
- [Mvp24Hours Framework](https://github.com/kalaboratory/mvp24hours-dotnet)

---

**Data:** Janeiro 2026  
**Lab:** Lab 02 - Simple N-Layers  
**Framework:** Mvp24Hours + EF Core + SQL Server
