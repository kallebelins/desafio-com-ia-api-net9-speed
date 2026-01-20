# Lições Aprendidas - Lab 03 CQRS com Mvp24Hours

## Data: 2026-01-20

## Resumo
Implementação de API REST usando CQRS (Command Query Responsibility Segregation) com o Mvp24Hours Framework. Este documento registra os problemas encontrados e suas soluções durante o desenvolvimento.

---

## Problema 1: Conflito de Versões FluentValidation

### Descrição
Ao configurar o projeto com `FluentValidation Version="11.*"`, o restore falhou com erro de downgrade de pacote.

### Erro
```
error NU1605: Aviso como Erro: Downgrade de pacote detectado: FluentValidation de 12.1.1 para 11.12.0
Lab03.Application -> Mvp24Hours.Infrastructure.Cqrs 9.1.21 -> FluentValidation (>= 12.1.1)
```

### Causa
O pacote `Mvp24Hours.Infrastructure.Cqrs` versão 9.x requer `FluentValidation >= 12.1.1`, mas foi especificada versão 11.x no projeto.

### Solução
Atualizar a versão do FluentValidation para 12.x:

```xml
<PackageReference Include="FluentValidation" Version="12.*" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="12.*" />
```

---

## Problema 2: Interface IEntityDateLog Inexistente

### Descrição
Tentativa de usar `IEntityDateLog` do `Mvp24Hours.Core.Entities` que não existe.

### Erro
```
error CS0246: O nome do tipo ou do namespace "IEntityDateLog" não pode ser encontrado
```

### Causa
A documentação sugeria interfaces que não existem na versão atual do Mvp24Hours.

### Solução
Usar apenas `EntityBase<TKey>` e implementar campos de auditoria manualmente:

```csharp
using Mvp24Hours.Core.Entities;

public class Produto : EntityBase<int>
{
    public string Nome { get; set; } = string.Empty;
    // ... outros campos
    
    // Campos de auditoria manuais
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
}
```

### Interfaces Disponíveis no Mvp24Hours 9.x
| Interface | Namespace | Descrição |
|-----------|-----------|-----------|
| `EntityBase<TKey>` | `Mvp24Hours.Core.Entities` | Base com ID tipado |
| `IEntityBase` | `Mvp24Hours.Core.Contract.Domain` | Interface base |
| `IEntity<TId>` | `Mvp24Hours.Core.Contract.Domain` | Interface com ID genérico |

---

## Problema 3: Método AddMvp24HoursCqrs Inexistente

### Descrição
Tentativa de usar `services.AddMvp24HoursCqrs()` que não existe.

### Erro
```
error CS1061: 'IServiceCollection' não contém uma definição para "AddMvp24HoursCqrs"
```

### Causa
O método correto para registrar o CQRS/Mediator é `AddMvpMediator`.

### Solução
```csharp
using Mvp24Hours.Infrastructure.Cqrs.Extensions;

services.AddMvpMediator(options =>
{
    options.RegisterHandlersFromAssemblyContaining<CreateProdutoCommandHandler>();
    options.RegisterLoggingBehavior = true;
    // Outras opções...
});
```

---

## Problema 4: ValidationBehavior com TypeLoadException

### Descrição
Ao habilitar `RegisterValidationBehavior = true`, ocorreu erro de carregamento de tipo.

### Erro
```
System.TypeLoadException: Could not load type 'Mvp24Hours.Core.Exceptions.ValidationException' 
from assembly 'Mvp24Hours.Core, Version=9.10.29.1, Culture=neutral, PublicKeyToken=null'.
```

### Causa
Conflito interno entre versões dos pacotes `Mvp24Hours.Core` e `Mvp24Hours.Infrastructure.Cqrs`. A classe `ValidationException` esperada pelo behavior de validação não existe na versão instalada do Core.

### Solução Temporária
Desabilitar o `ValidationBehavior` e fazer validação manual no Controller:

```csharp
// ServiceBuilderExtensions.cs
services.AddMvpMediator(options =>
{
    options.RegisterHandlersFromAssemblyContaining<CreateProdutoCommandHandler>();
    options.RegisterValidationBehavior = false; // Desabilitado - conflito de versões
    options.RegisterLoggingBehavior = true;
});

// ProdutoController.cs
public class ProdutoController : ControllerBase
{
    private readonly IValidator<CreateProdutoCommand>? _createValidator;

    public async Task<ActionResult<ProdutoDto>> Create(
        [FromBody] CreateProdutoCommand command,
        CancellationToken cancellationToken = default)
    {
        // Validação manual
        if (_createValidator is not null)
        {
            var validationResult = await _createValidator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                { 
                    Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) 
                });
            }
        }

        var result = await _sender.SendAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
```

### Recomendação
Aguardar atualização do Mvp24Hours que corrija a compatibilidade entre `Mvp24Hours.Core` e `Mvp24Hours.Infrastructure.Cqrs` para o `ValidationBehavior`.

---

## Boas Práticas Aprendidas

### 1. Registrar Mediator Corretamente
```csharp
services.AddMvpMediator(options =>
{
    options.RegisterHandlersFromAssemblyContaining<SeuHandler>();
    options.RegisterLoggingBehavior = true;
});
```

### 2. Interfaces CQRS do Mvp24Hours

| MediatR (❌ NÃO USAR) | Mvp24Hours (✅ USAR) |
|----------------------|---------------------|
| `IRequest<T>` | `IMediatorCommand<T>` ou `IMediatorQuery<T>` |
| `IRequestHandler` | `IMediatorCommandHandler` ou `IMediatorQueryHandler` |
| `IMediator.Send()` | `ISender.SendAsync()` |

### 3. Namespaces Importantes
```csharp
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;  // Interfaces de Command/Query
using Mvp24Hours.Infrastructure.Cqrs.Extensions;    // Métodos de extensão (AddMvpMediator)
using Mvp24Hours.Core.Contract.Data;                // IRepositoryAsync, IUnitOfWorkAsync
using Mvp24Hours.Core.Entities;                     // EntityBase
using Mvp24Hours.Extensions;                        // AddMvp24HoursDbContext, AddMvp24HoursRepositoryAsync
```

### 4. Exemplo de Command Completo
```csharp
// Command
public record CreateProdutoCommand(
    string Nome,
    string? Descricao,
    decimal Preco,
    string Categoria,
    int Estoque
) : IMediatorCommand<ProdutoDto>;

// Handler
public class CreateProdutoCommandHandler : IMediatorCommandHandler<CreateProdutoCommand, ProdutoDto>
{
    private readonly IRepositoryAsync<Produto> _repository;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public CreateProdutoCommandHandler(
        IRepositoryAsync<Produto> repository,
        IUnitOfWorkAsync unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProdutoDto> Handle(CreateProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = new Produto { /* mapeamento */ };
        await _repository.AddAsync(produto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ProdutoDto.FromEntity(produto);
    }
}
```

---

## Versões Utilizadas

| Pacote | Versão |
|--------|--------|
| .NET SDK | 9.0 |
| Mvp24Hours.Core | 9.* |
| Mvp24Hours.Infrastructure.Cqrs | 9.* |
| Mvp24Hours.Infrastructure.Data.EFCore | 9.* |
| Mvp24Hours.WebAPI | 9.* |
| FluentValidation | 12.* |
| Microsoft.EntityFrameworkCore.SqlServer | 9.* |
| Swashbuckle.AspNetCore | 7.* |

---

## Checklist de Implementação CQRS

- [x] Criar Commands (`IMediatorCommand<T>`)
- [x] Criar Queries (`IMediatorQuery<T>`)
- [x] Criar CommandHandlers (`IMediatorCommandHandler<TCmd, TRes>`)
- [x] Criar QueryHandlers (`IMediatorQueryHandler<TQuery, TRes>`)
- [x] Criar Validators (`AbstractValidator<T>`)
- [x] Registrar com `AddMvpMediator`
- [x] Usar `ISender` no Controller
- [x] Validação manual quando `ValidationBehavior` não funcionar

---

## Tags
`mvp24hours` `cqrs` `mediator` `fluentvalidation` `conflito-versoes` `validationbehavior`
