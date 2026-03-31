# ToDo API

API RESTful para gerenciamento de tarefas construída com **.NET 9**, seguindo os princípios de **Clean Architecture**, com autenticação via **ASP.NET Core Identity API Endpoints**.

---

## Índice

- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Pré-requisitos](#pré-requisitos)
- [Primeiros Passos](#primeiros-passos)
  - [1. Clonar o Repositório](#1-clonar-o-repositório)
  - [2. Subir os Containers (Docker)](#2-subir-os-containers-docker)
  - [3. Aplicar as Migrations](#3-aplicar-as-migrations)
  - [4. Acessar a API](#4-acessar-a-api)
- [Endpoints da API](#endpoints-da-api)
  - [Identity (Autenticação)](#identity-autenticação)
  - [TodoItems (Tarefas)](#todoitems-tarefas)
- [Autenticação](#autenticação)
  - [Registrar um Usuário](#registrar-um-usuário)
  - [Fazer Login](#fazer-login)
  - [Usar o Token](#usar-o-token)
- [Testes](#testes)
- [Observabilidade (Seq)](#observabilidade-seq)
- [Configuração](#configuração)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Seed de Dados](#seed-de-dados)
- [Comandos Úteis](#comandos-úteis)

---

## Tecnologias

| Tecnologia | Versão | Finalidade |
|---|---|---|
| .NET | 9.0 | Framework principal |
| ASP.NET Core Identity | 9.0 | Autenticação e autorização |
| Entity Framework Core | 9.0 | ORM e migrations |
| SQL Server | 2022 | Banco de dados relacional |
| MediatR | 12.x | Mediator pattern (CQRS) |
| FluentValidation | 11.x | Validação de comandos |
| Serilog | 8.x | Logging estruturado |
| Seq | latest | Agregação e visualização de logs |
| Docker / Docker Compose | - | Containerização |
| Swagger / Swashbuckle | 7.2 | Documentação interativa da API |

---

## Arquitetura

O projeto segue **Clean Architecture** com 4 camadas:

```
┌──────────────────────────────────────────────┐
│                  Todo.Api                     │  ← Presentation (Controllers, Middleware)
├──────────────────────────────────────────────┤
│              Todo.Application                 │  ← Use Cases (Commands, Queries, DTOs)
├──────────────────────────────────────────────┤
│               Todo.Domain                     │  ← Entities, Enums, Interfaces
├──────────────────────────────────────────────┤
│            Todo.Infrastructure                │  ← EF Core, Identity, Repositories
└──────────────────────────────────────────────┘
```

- **Todo.Domain**: Entidades (`TodoItem`), enums (`TodoItemStatus`), e interfaces de repositório. Zero dependências externas.
- **Todo.Application**: Comandos e queries (CQRS via MediatR), DTOs, validações (FluentValidation), e interfaces compartilhadas.
- **Todo.Infrastructure**: Implementação de persistência (EF Core + SQL Server), configuração do Identity, repositórios e Unit of Work.
- **Todo.Api**: Controllers versionados, middleware de tratamento global de erros, configuração do Swagger e pipeline HTTP.

---

## Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [EF Core CLI Tools](https://learn.microsoft.com/pt-br/ef/core/cli/dotnet) (opcional, para migrations manuais)

Para instalar a ferramenta do EF Core CLI:

```bash
dotnet tool install --global dotnet-ef --version 9.0.0
```

---

## Primeiros Passos

### 1. Clonar o Repositório

```bash
git clone <url-do-repositorio>
cd todo_app
```

### 2. Subir os Containers (Docker)

O Docker Compose inicializa 3 serviços: **SQL Server**, **Seq** e a **API**.

```bash
docker-compose up -d --build
```

Aguarde até que todos os containers estejam saudáveis. O SQL Server possui um **healthcheck** que valida a conexão antes de liberar a API para iniciar.

Para acompanhar os logs:

```bash
docker-compose logs -f
```

### 3. Aplicar as Migrations

As migrations **não são aplicadas automaticamente**. Você precisa rodá-las manualmente:

```bash
dotnet ef database update --project src/Todo.Infrastructure --startup-project src/Todo.Api
```

> **Nota**: Certifique-se de que o container do SQL Server já está rodando e saudável antes de executar este comando.

Se for a primeira vez ou se houver mudanças no modelo de dados, crie uma nova migration primeiro:

```bash
dotnet ef migrations add NomeDaMigration --project src/Todo.Infrastructure --startup-project src/Todo.Api
```

### 4. Acessar a API

| Serviço | URL | Descrição |
|---|---|---|
| Swagger UI | http://localhost:5001/swagger | Documentação interativa da API |
| Seq (Logs) | http://localhost:5341 | Painel de logs estruturados |
| SQL Server | `localhost,1433` | Acesso direto ao banco via SSMS/Azure Data Studio |

---

## Endpoints da API

### Identity (Autenticação)

Endpoints nativos do ASP.NET Core Identity, mapeados em `/api/v1/identity`:

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/v1/identity/register` | Registrar novo usuário |
| `POST` | `/api/v1/identity/login` | Login (retorna access token) |
| `POST` | `/api/v1/identity/refresh` | Renovar access token |
| `GET` | `/api/v1/identity/manage/info` | Informações do usuário autenticado |
| `POST` | `/api/v1/identity/manage/info` | Atualizar informações do usuário |

### TodoItems (Tarefas)

Todos os endpoints exigem autenticação (`Bearer Token`):

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/v1/todoitems` | Listar tarefas (paginado, com filtros) |
| `GET` | `/api/v1/todoitems/{id}` | Buscar tarefa por ID |
| `POST` | `/api/v1/todoitems` | Criar nova tarefa |
| `PUT` | `/api/v1/todoitems/{id}` | Atualizar tarefa existente |
| `DELETE` | `/api/v1/todoitems/{id}` | Deletar tarefa |

#### Filtros disponíveis (GET /api/v1/todoitems)

| Parâmetro | Tipo | Descrição |
|---|---|---|
| `status` | `int?` | Filtrar por status: `0` (Pending), `1` (InProgress), `2` (Completed) |
| `dueBefore` | `DateTime?` | Tarefas com vencimento antes desta data |
| `dueAfter` | `DateTime?` | Tarefas com vencimento após esta data |
| `pageNumber` | `int` | Página atual (padrão: `1`) |
| `pageSize` | `int` | Itens por página (padrão: `10`) |

---

## Autenticação

A API utiliza o sistema nativo de autenticação do **ASP.NET Core Identity API Endpoints** (.NET 9).

### Registrar um Usuário

```http
POST /api/v1/identity/register
Content-Type: application/json

{
  "email": "usuario@email.com",
  "password": "SuaSenha123!"
}
```

### Fazer Login

```http
POST /api/v1/identity/login
Content-Type: application/json

{
  "email": "usuario@email.com",
  "password": "SuaSenha123!"
}
```

A resposta incluirá um `accessToken`:

```json
{
  "tokenType": "Bearer",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6...",
  "expiresIn": 3600,
  "refreshToken": "CfDJ8..."
}
```

### Usar o Token

Passe o token no header `Authorization` de cada requisição protegida:

```http
GET /api/v1/todoitems
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6...
```

No **Swagger UI**, clique no botão **Authorize** (cadeado) e cole o token no formato:

```
eyJhbGciOiJIUzI1NiIsInR5cCI6...
```

> **Nota**: Não é necessário incluir o prefixo `Bearer` no Swagger, ele adiciona automaticamente.

---

## Testes

O projeto possui dois projetos de teste unitário:

```
tests/
├── Todo.Application.UnitTests/   # Testes dos handlers, validadores e use cases
└── Todo.Domain.UnitTests/        # Testes das entidades e regras de negócio
```

### Rodar todos os testes

```bash
dotnet test
```

### Rodar testes de um projeto específico

```bash
# Testes de Application
dotnet test tests/Todo.Application.UnitTests

# Testes de Domain
dotnet test tests/Todo.Domain.UnitTests
```

### Rodar com mais detalhes (verboso)

```bash
dotnet test --verbosity normal
```

---

## Observabilidade (Seq)

A API envia logs estruturados para o **Seq** via Serilog.

| Item | Valor |
|---|---|
| **URL do Painel** | http://localhost:5341 |
| **Usuário** | `admin` |
| **Senha** | `Admin123!` |

No painel do Seq você pode:
- Visualizar logs em tempo real de todas as requisições HTTP.
- Filtrar por nível (Information, Warning, Error, Fatal).
- Buscar por propriedades específicas (ex: `UserId`, `RequestPath`).

---

## Configuração

### Variáveis de Ambiente (Docker)

As configurações dos containers são definidas no `docker-compose.yml`:

| Variável | Serviço | Valor Padrão |
|---|---|---|
| `MSSQL_SA_PASSWORD` | sqlserver | `YourPassword123!` |
| `SEQ_FIRSTRUN_ADMINPASSWORD` | seq | `Admin123!` |
| `ConnectionStrings__DefaultConnection` | todo-api | `Server=sqlserver;Database=TodoDb;...` |
| `Serilog__SeqUrl` | todo-api | `http://seq:5341` |

### Configuração Local (appsettings.json)

Para rodar fora do Docker (localmente), as configurações ficam em `src/Todo.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=TodoDb;User Id=sa;Password=YourPassword123!;Encrypt=False;TrustServerCertificate=True;"
  },
  "Serilog": {
    "SeqUrl": "http://localhost:5341"
  }
}
```

> **Importante**: Ao rodar migrations manualmente via `dotnet ef`, a connection string do `appsettings.json` é utilizada. Certifique-se de que ela aponta para `localhost,1433` e não para `sqlserver` (que é o hostname interno do Docker).

---

## Estrutura do Projeto

```
todo_app/
├── docker-compose.yml              # Orquestração dos containers
├── Dockerfile                      # Build multi-stage da API
├── TodoApp.slnx                    # Solution file
│
├── src/
│   ├── Todo.Api/                   # Camada de Apresentação
│   │   ├── Controllers/
│   │   │   ├── ApiController.cs    # Base controller (versionamento, auth, error handling)
│   │   │   └── TodoItemsController.cs
│   │   ├── Middleware/
│   │   │   └── GlobalExceptionHandlerMiddleware.cs
│   │   ├── Program.cs              # Entry point e configuração do app
│   │   └── appsettings.json
│   │
│   ├── Todo.Application/           # Camada de Aplicação (Use Cases)
│   │   ├── Common/
│   │   │   ├── Interfaces/         # IRepository, IUnitOfWork
│   │   │   └── Models/             # Result, PagedList
│   │   └── TodoItems/
│   │       ├── Commands/           # Create, Update, Delete
│   │       ├── Queries/            # GetAll, GetById
│   │       └── DTOs/               # TodoItemResponse
│   │
│   ├── Todo.Domain/                # Camada de Domínio
│   │   ├── Entities/
│   │   │   └── TodoItem.cs
│   │   └── Enums/
│   │       └── TodoItemStatus.cs   # Pending, InProgress, Completed
│   │
│   └── Todo.Infrastructure/        # Camada de Infraestrutura
│       ├── Persistence/
│       │   ├── ApplicationDbContext.cs
│       │   ├── DataSeeder.cs
│       │   ├── Configurations/     # EF Core Fluent API
│       │   ├── Migrations/         # Migrations do EF Core
│       │   ├── Models/             # ApplicationUser (Identity)
│       │   └── Repositories/       # TodoItemRepository
│       └── DependencyInjection.cs
│
└── tests/
    ├── Todo.Application.UnitTests/ # Testes dos handlers e validadores
    └── Todo.Domain.UnitTests/      # Testes das entidades de domínio
```

---

## Seed de Dados

Ao iniciar a aplicação pela primeira vez (após aplicar as migrations), o `DataSeeder` cria automaticamente:

**Usuário Admin:**

| Campo | Valor |
|---|---|
| Email | `admin@todo.com` |
| Senha | `Admin123!` |

**Tarefas de Exemplo:**

| Título | Descrição |
|---|---|
| Organizar mesa de trabalho | Limpar a poeira e organizar os cabos do setup |
| Comprar mantimentos | Ir ao mercado comprar café, leite e frutas |
| Treino de musculação | Focar em membros superiores hoje |
| Leitura matinal | Ler 10 páginas do livro atual |
| Planejamento semanal | Definir as metas para a próxima semana |

> **Nota**: O seed só é executado se não houver nenhum usuário cadastrado no banco.

---

## Comandos Úteis

### Docker

```bash
# Subir todos os serviços
docker-compose up -d --build

# Parar todos os serviços
docker-compose down

# Parar e remover volumes (reset completo do banco)
docker-compose down -v

# Ver logs de todos os serviços
docker-compose logs -f

# Ver logs apenas da API
docker-compose logs -f todo-api
```

### Entity Framework

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration --project src/Todo.Infrastructure --startup-project src/Todo.Api

# Aplicar migrations no banco
dotnet ef database update --project src/Todo.Infrastructure --startup-project src/Todo.Api

# Reverter última migration
dotnet ef migrations remove --project src/Todo.Infrastructure --startup-project src/Todo.Api

# Listar migrations aplicadas
dotnet ef migrations list --project src/Todo.Infrastructure --startup-project src/Todo.Api
```

### Build e Testes

```bash
# Build da solution
dotnet build

# Rodar todos os testes
dotnet test

# Rodar localmente (sem Docker)
dotnet run --project src/Todo.Api
```

---

## Licença

Este projeto é de uso livre para fins de estudo e desenvolvimento.
