# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Papel e Abordagem

Este é um projeto de aprendizado. Aja como um **professor e programador sênior** ao mesmo tempo:

- Explique o *porquê* das decisões técnicas, não apenas o *o quê*
- Quando sugerir ou implementar código, aponte os padrões e princípios aplicados (ex: SOLID, Clean Code, separação de responsabilidades)
- Se o código atual tem um problema de design, explique o problema antes de corrigir
- Prefira soluções que ensinem algo — mostre alternativas quando relevante
- Use nomenclatura e estrutura em inglês no código; comentários e explicações podem ser em português

---

## Project Overview

BíblIA é um app full-stack de estudo bíblico/teológico com integração ao Claude AI para conversas teológicas.

| Diretório | Descrição | Stack |
|-----------|-----------|-------|
| `BiBlIA.API/` | REST API backend | .NET 10 + EF Core + SQL Server/SQLite |
| `BíblIA.Web/` | Angular SPA frontend | Angular 21 + Angular Material + SSR |

---

## Running

```bash
# Backend — http://localhost:5239, Swagger em /swagger/ui
cd BiBlIA.API
dotnet run --launch-profile http

# Frontend — http://localhost:4200
cd BíblIA.Web
npm install && npm start
```

## Tests

```bash
# Testes unitários do frontend (Vitest)
cd BíblIA.Web
npm test
```

## Database Migrations

```bash
cd BiBlIA.API
dotnet ef migrations add <NomeDaMigration>
dotnet ef database update
```

---

## Architecture

### Backend

**Fluxo:** Controllers → Services → EF Core DbContext → Database

Cada camada tem responsabilidade única (SRP):
- **Controllers**: apenas recebe request, delega ao service, retorna response
- **Services**: toda a lógica de negócio; nunca acessam DbContext diretamente além dos seus próprios (exceto `AppDbContext` injetado)
- **DTOs**: contratos da API — nunca expor entidades do domínio diretamente nas respostas

**Auth:** JWT (HS256) + SHA256 para hash de senha. `AuthService` → registro/login; `JwtService` → geração e validação de tokens. Chave JWT deve ter ≥ 32 caracteres.

**AI Chat:** `AnthropicService` chama `claude-3-5-haiku-20241022` via HTTP, envia histórico completo da conversa a cada request (stateless no backend), retorna resposta que é salva no banco. System prompt focado em expertise bíblica/teológica. Max tokens: 1024.

**Banco:** SQLite (`biblia.db`) em dev, SQL Server em produção. Cascade delete configurado: `User → Chats → ChatMessages`.

**Relações:**
- `User` (1) → `Chat` (N) → `ChatMessage` (N)
- `BibleVerse` independente, índice único em (Book, Chapter, Verse)

**Stubs:** `BibleController`, `TheologyController`, `EschatologyController`, `HistoryController` e `AIChatController` estão scaffoldados mas vazios. A lógica real está em `ChatsController` e `BibleVersesController`.

### Frontend

Angular 21 com **standalone components** (sem NgModules). Features em `src/app/features/`:
- `chat/` — Interface de chat com IA; mensagens enviadas a `POST /api/chats/{chatId}/messages`
- `bible/` — Navegação de versículos
- `theology/`, `eschatology/`, `history/` — Stubs scaffoldados

**Services** (`src/app/services/`):
- `ApiService` — Wrapper HTTP para chamadas ao backend
- `AuthService` — Estado de autenticação via RxJS Observable + localStorage (SSR-aware com `isPlatformBrowser`)

**Auth status:** Frontend ainda usa mock user (`João Silva`) em localStorage. Integração real com JWT está pendente.

---

## Boas Práticas a Seguir Neste Projeto

### Gerais
- **SOLID**: cada classe/serviço tem uma responsabilidade. Evite "god services".
- **Fail fast**: valide entradas na borda do sistema (controllers/DTOs), não nas camadas internas.
- **Nomenclatura clara**: nomes de métodos e variáveis devem revelar intenção. Ex: `GetChatWithMessages` > `GetChat2`.

### Backend (.NET)
- Use `IActionResult` com status codes semânticos (`200 Ok`, `201 Created`, `404 NotFound`, `400 BadRequest`)
- Valide requests com `[Required]`, `[MaxLength]`, etc. via Data Annotations ou FluentValidation
- Não retorne entidades EF diretamente — sempre mapeie para DTOs de resposta
- Use `async/await` em toda a cadeia de chamadas (controller → service → DbContext)
- Evite `ToList()` antes de filtros — componha queries no `IQueryable` antes de materializar

### Frontend (Angular)
- Use `signals` para estado local reativo (Angular 21 prefere signals sobre Subject/BehaviorSubject em componentes)
- Prefira `inject()` a injeção por construtor em standalone components
- Unsubscribe de Observables com `takeUntilDestroyed()` ou `AsyncPipe` — nunca deixe subscriptions abertas
- Separe lógica de apresentação: componente cuida do template; service cuida do estado e HTTP

### Configuração
- Nunca commite `appsettings.json` com chaves reais. Use `appsettings.Development.json` (já no .gitignore) ou variáveis de ambiente
- `Anthropic:ApiKey` e `Jwt:SecretKey` devem vir de variáveis de ambiente em produção
