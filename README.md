# Desafio com IA - API .NET 9 Speed

> Projeto experimental onde laboratórios completos são desenvolvidos exclusivamente por IA, sem intervenção humana na codificação.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Mvp24Hours](https://img.shields.io/badge/Mvp24Hours-Framework-blue)](https://mvp24hours.dev/)
[![MCP](https://img.shields.io/badge/MCP-Enabled-green)](https://www.npmjs.com/package/mvp24hours-dotnet-mcp)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## Sobre o Projeto

Este repositório é um **experimento de desenvolvimento assistido por IA**. Aqui, desenvolvemos uma série de laboratórios práticos utilizando o framework **Mvp24Hours** para .NET 9, onde todo o código é gerado exclusivamente por Inteligência Artificial através do **Model Context Protocol (MCP)**.

### O Desafio

- **Zero codificação humana** - Todo código é escrito pela IA
- **Orientação por MCP** - A IA utiliza ferramentas MCP para seguir padrões corretos
- **Progressão de complexidade** - De APIs simples até arquiteturas enterprise
- **Documentação automática** - READMEs e comentários também são gerados pela IA

---

## Tecnologias Utilizadas

| Tecnologia | Descrição |
|------------|-----------|
| **.NET 9** | Plataforma de desenvolvimento |
| **Mvp24Hours Framework** | Framework completo com CQRS, Repository, Value Objects e mais |
| **MCP (Model Context Protocol)** | Protocolo para assistência de IA contextualizada |
| **SQL Server** | Banco de dados relacional |
| **RabbitMQ** | Message broker para eventos |
| **Docker** | Containerização da infraestrutura |
| **OpenTelemetry** | Observabilidade (logs, traces, metrics) |

---

## Mvp24Hours Framework

O **Mvp24Hours** é um framework .NET completo que fornece implementações prontas para:

- **CQRS/Mediator** - Implementação própria (não usa MediatR)
- **Repository/Unit of Work** - Abstração de acesso a dados
- **Value Objects** - Email, CPF, CNPJ, Money, Address, etc.
- **Pipeline Behaviors** - Validação, logging, caching
- **Domain Events** - Eventos de domínio e integração
- **E muito mais...**

### Links Importantes

| Recurso | Link |
|---------|------|
| **Documentação Oficial** | [https://mvp24hours.dev](https://mvp24hours.dev/#/) |
| **MCP Server (npm)** | [mvp24hours-dotnet-mcp](https://www.npmjs.com/package/mvp24hours-dotnet-mcp) |
| **Autor** | [Kallebe Lins - LinkedIn](https://www.linkedin.com/in/kallebelins/) |

---

## MCP - Model Context Protocol

O **MCP** permite que a IA tenha acesso contextualizado à documentação e templates do Mvp24Hours. Através das ferramentas MCP, a IA pode:

```
mvp24h_get_started()        → Visão geral do framework
mvp24h_architecture_advisor() → Recomendação de arquitetura
mvp24h_database_advisor()    → Configuração de banco de dados
mvp24h_cqrs_guide()          → Implementação de CQRS
mvp24h_get_template()        → Templates de código prontos
mvp24h_build_context()       → Contexto completo para implementação
```

### Instalação do MCP Server

```bash
npm install -g mvp24hours-dotnet-mcp
```

---

## Estrutura do Projeto

```
desafio-com-ia-api-net9-speed/
├── README.md                  # Este arquivo
├── .cursorrules               # Regras para a IA seguir
└── labs/                      # Série de laboratórios
    ├── README.md              # Índice dos laboratórios
    ├── docker-compose.yml     # Infraestrutura compartilhada
    ├── lab-01-minimal-api-produto/
    ├── lab-02-simple-nlayers-cliente/
    ├── lab-03-cqrs-produto/
    ├── lab-04-event-driven-cliente/
    ├── lab-05-cqrs-observability-produto/
    ├── lab-06-hexagonal-cliente/
    ├── lab-07-event-driven-saga-venda/
    ├── lab-08-clean-architecture-completo/
    ├── lab-09-cqrs-event-sourcing-venda/
    └── lab-10-fullstack-completo/
```

---

## Laboratórios

A série de laboratórios foi projetada para demonstrar diferentes arquiteturas e recursos do Mvp24Hours, com complexidade progressiva:

| Nível | Lab | Arquitetura | Recursos Principais |
|-------|-----|-------------|---------------------|
| Básico | 01 | Minimal API | Repository, Validation |
| Intermediário | 02 | Simple N-Layers | Repository, UoW, AutoMapper |
| Avançado | 03 | Complex N-Layers + CQRS | CQRS/Mediator, Behaviors |
| Avançado | 04 | Event-Driven | Domain Events, RabbitMQ |
| Avançado+ | 05 | CQRS + Observability | OpenTelemetry, Metrics |
| Avançado+ | 06 | Hexagonal | Ports & Adapters |
| Expert | 07 | Event-Driven + Saga | Saga Pattern, Outbox |
| Expert | 08 | Clean Architecture | Use Cases, Domain Services |
| Expert+ | 09 | CQRS + Event Sourcing | Event Store, Projections |
| Master | 10 | Full Stack | Todos os recursos combinados |

Veja o [README dos Labs](./labs/README.md) para mais detalhes.

---

## Como Executar

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js](https://nodejs.org/) (para MCP)

### Subir a Infraestrutura

```bash
cd labs
docker-compose up -d
```

### Executar um Laboratório

```bash
cd labs/lab-01-minimal-api-produto
dotnet run
```

---

## Regras para a IA

O arquivo `.cursorrules` define regras importantes que a IA deve seguir:

### Proibições

- **Nunca usar MediatR** - O Mvp24Hours tem implementação própria de CQRS
- **Nunca recriar Value Objects** - Usar os prontos (Email, CPF, CNPJ, etc.)
- **Nunca reinventar Repository/UnitOfWork** - Usar as abstrações do framework

### Obrigações

- **Sempre consultar MCP** antes de implementar
- **Sempre usar namespaces corretos** do Mvp24Hours
- **Sempre seguir os padrões** documentados no framework

---

## Contribuição

Este é um projeto experimental. Contribuições são bem-vindas, especialmente:

- Sugestões de novos laboratórios
- Melhorias nas regras para IA
- Correções e otimizações

---

## Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## Autor

**Kallebe Lins**

- LinkedIn: [linkedin.com/in/kallebelins](https://www.linkedin.com/in/kallebelins/)
- Framework: [mvp24hours.dev](https://mvp24hours.dev/#/)

---

> *"O futuro do desenvolvimento de software está na colaboração entre humanos e IA. Este projeto explora esse futuro."*
