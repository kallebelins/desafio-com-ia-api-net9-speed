# Lições Aprendidas - Lab 09: CQRS + Event Sourcing

## Problema 1: FluentValidation - Conflito de Versão

### Erro
```
error NU1605: Downgrade de pacote detectado: FluentValidation de 12.1.1 para 11.12.0
```

### Causa
O pacote `Mvp24Hours.Infrastructure.Cqrs` depende do FluentValidation 12.x, mas o projeto tentou usar a versão 11.x.

### Solução
Usar versão 12.* do FluentValidation no projeto Application:
```xml
<PackageReference Include="FluentValidation" Version="12.*" />
```

---

## Problema 2: EF Core - Múltiplos DbContexts e EnsureCreated

### Erro
```
Microsoft.Data.SqlClient.SqlException: Invalid object name 'VendaSnapshots'.
Microsoft.Data.SqlClient.SqlException: Invalid object name 'ProjectionCheckpoints'.
```

### Causa
Quando múltiplos DbContexts (`EventStoreDbContext`, `ProjectionDbContext`, `SnapshotDbContext`) usam a mesma connection string (mesmo banco), o `EnsureCreated()` só cria as tabelas do primeiro DbContext chamado. Os demais DbContexts não criam suas tabelas porque o banco já existe.

### Solução
Criar as tabelas manualmente via SQL no startup:
```csharp
// Criar tabelas que podem estar faltando
await context.Database.ExecuteSqlRawAsync(@"
    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NomeTabela' and xtype='U')
    CREATE TABLE NomeTabela (...)
");
```

### Alternativa
Usar migrações do EF Core para cada DbContext separadamente.

---

## Problema 3: Dynamic Type e Enum.TryParse

### Erro
```
error CS8197: Não é possível inferir o tipo da variável out de tipo implícito 'status'.
```

### Causa
Ao usar `dynamic` e `Enum.TryParse<T>(..., out var status)`, o compilador não consegue inferir o tipo da variável `status`.

### Solução
Declarar o tipo explicitamente:
```csharp
// Errado
if (Enum.TryParse<VendaStatus>(state.Status, out var status))

// Correto
string statusStr = state.Status;
if (Enum.TryParse<VendaStatus>(statusStr, out VendaStatus parsedStatus))
```

---

## Problema 4: Referência ao Microsoft.EntityFrameworkCore no Application Layer

### Erro
```
error CS0234: O nome de tipo ou namespace "EntityFrameworkCore" não existe no namespace "Microsoft"
```

### Causa
O projeto Application usa `DbContext` para as projeções mas não referenciava o pacote EF Core.

### Solução
Adicionar a referência ao pacote no `.csproj`:
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.*" />
```

---

## Boas Práticas Aprendidas

### 1. Event Sourcing com Mvp24Hours
- Usar interfaces próprias do Mvp24Hours para CQRS (`IMediatorCommand<T>`, `IMediatorQuery<T>`)
- **NÃO** usar MediatR - Mvp24Hours tem implementação própria
- Implementar `IAggregateRoot` customizada para Event Sourcing

### 2. Aggregate Root Pattern
- O estado é derivado da sequência de eventos (não armazenado diretamente)
- Método `FromHistory()` para reconstituir o Aggregate a partir de eventos
- Método `FromSnapshot()` para otimizar reconstrução
- `RaiseEvent()` para emitir eventos e atualizar estado atomicamente

### 3. Event Store
- Eventos são imutáveis e armazenados em sequência
- Controle de concorrência via `expectedVersion`
- Índice por `(AggregateId, Version)` para garantir unicidade

### 4. Projections
- Read Models são criados processando eventos
- Usar `ProjectionCheckpoint` para controlar posição de processamento
- Projeções devem ser idempotentes (podem ser reprocessadas)
- Hosted Service para processar projeções em background

### 5. Snapshots
- Salvar snapshot a cada N eventos para otimizar reconstrução
- Carregar do snapshot e aplicar apenas eventos subsequentes

### 6. Time Travel
- Possível reconstruir estado em qualquer momento do passado
- Filtrar eventos até o timestamp desejado e reconstruir Aggregate

---

## Estrutura do Projeto Event Sourcing

```
Lab09.EventSourcing/
├── src/
│   ├── Lab09.Core/           # Domain: Aggregates, Events, ValueObjects
│   ├── Lab09.Application/    # Commands, Queries, Handlers, Projections
│   ├── Lab09.Infrastructure/ # EventStore, Snapshots, DbContexts
│   └── Lab09.WebAPI/         # Controllers, HostedServices
```

## Pacotes Necessários

```xml
<!-- Core -->
<PackageReference Include="Mvp24Hours.Core" Version="9.*" />

<!-- Application -->
<PackageReference Include="Mvp24Hours.Infrastructure.Cqrs" Version="9.*" />
<PackageReference Include="FluentValidation" Version="12.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.*" />

<!-- Infrastructure -->
<PackageReference Include="Mvp24Hours.Infrastructure.Data.EFCore" Version="9.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.*" />

<!-- WebAPI -->
<PackageReference Include="Mvp24Hours.WebAPI" Version="9.*" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.*" />
```
