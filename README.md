# OrderManagement

A small e-commerce / order management system built with **ASP.NET Core** and a **React** frontend,
designed as a **learning project** for:

- Clean Architecture
- Domain-Driven Design (DDD)
- CQRS
- Application / Infrastructure layering
- EF Core + Oracle
- Dependency Inversion & the Repository Pattern
- Testing (xUnit)
- Docker

> **Educational intent:** this repo optimizes for *readability and clear separation of
> responsibilities*, not for production scale. Abstractions exist only where they teach
> something (no MediatR, no generic repositories, no message brokers).

---

## Table of contents

- [Tech stack](#tech-stack)
- [Architecture](#architecture)
- [Project structure](#project-structure)
- [Domain model & business rules](#domain-model--business-rules)
- [How a request flows](#how-a-request-flows)
- [Getting started](#getting-started)
- [API reference](#api-reference)
- [Configuration](#configuration)
- [Database & migrations](#database--migrations)
- [Frontend](#frontend)
- [Testing](#testing)
- [Docker](#docker)
- [Key decisions & gotchas](#key-decisions--gotchas)
- [Learning guide](#learning-guide)
- [Troubleshooting](#troubleshooting)

---

## Tech stack

| Area | Technology |
|---|---|
| Backend | .NET 10, ASP.NET Core Web API (controllers) |
| Architecture | Clean Architecture, DDD, CQRS (no MediatR — handlers injected directly) |
| Persistence | EF Core 10 + Oracle provider (`Oracle.EntityFrameworkCore`), Code First migrations |
| Database | Oracle Database Free 23ai (Docker) |
| Frontend | React 18 + TypeScript (Vite), Tailwind CSS v4, react-router |
| Tests | xUnit (domain + application handler tests with in-memory fakes) |
| Containers | Docker + Docker Compose (API + Oracle) |

---

## Architecture

Dependency direction — inner layers know nothing about outer layers:

```
                 ┌───────────────┐
                 │      API      │  HTTP only: controllers, middleware, Program.cs
                 └───────┬───────┘
                         │ references
                 ┌───────▼───────┐
                 │  Application  │  Use cases: commands, queries, handlers, DTOs,
                 └───────┬───────┘  repository INTERFACES, AddApplication()
                         │ references
                 ┌───────▼───────┐
                 │    Domain     │  Business model: entities, value objects, enums.
                 └───────────────┘  References NOTHING.

                 ┌───────────────┐
                 │Infrastructure │  EF Core: DbContext, configurations, repository
                 └───────┬───────┘  IMPLEMENTATIONS, AddInfrastructure(). References
                         │ references Application (implements its interfaces).
                         ▼
              Application / Domain
```

| Project | Owns | Must never contain |
|---|---|---|
| `OrderManagement.Domain` | Business rules, state transitions (`Order.Confirm()`, `Product.DecreaseStock()`) | DB, HTTP, EF, SQL |
| `OrderManagement.Application` | Use-case coordination ("load customer → check active → create order") | `DbContext`, EF, Oracle, HTTP |
| `OrderManagement.Infrastructure` | How data is stored (EF mappings, SQL, Oracle) | Business decisions |
| `OrderManagement.Api` | HTTP translation (routes, status codes, JSON) | Business rules, `DbContext` |
| `OrderManagement.Tests` | Executable specification of the rules | Infrastructure/EF |
| `frontend` | UI only — mirrors the API, enforces nothing | Business logic |

**Dependency Inversion in one sentence:** Application defines repository *interfaces*
(`IOrderRepository`); Infrastructure *implements* them with EF Core; `Program.cs`
connects the two (`AddScoped<IOrderRepository, OrderRepository>()`). Tests prove it by
swapping in fake repositories with zero handler changes.

---

## Project structure

```
OrderManagement
├── OrderManagement.Api/                 # Controllers, ExceptionHandlingMiddleware, Program.cs
├── OrderManagement.Application/
│   ├── Common/Exceptions/               # NotFoundException, BusinessRuleException
│   ├── Customers|Products|Orders/
│   │   ├── Commands/<UseCase>/          # Command record + Handler (changes state)
│   │   ├── Queries/<UseCase>/           # Query record + Handler (reads only)
│   │   ├── Dtos/                        # Read models returned to callers
│   │   └── Repositories/                # Interfaces (I*Repository)
│   └── DependencyInjection.cs           # AddApplication()
├── OrderManagement.Domain/
│   ├── Entities/                        # Customer, Product, Order, OrderItem
│   ├── ValueObjects/                    # Money, Address
│   └── Enums/                           # OrderStatus, Currency
├── OrderManagement.Infrastructure/
│   ├── Persistence/
│   │   ├── OrderManagementDbContext.cs  # DbSets only; mappings auto-loaded
│   │   ├── Configurations/              # One IEntityTypeConfiguration per aggregate
│   │   └── Repositories/                # EF Core implementations
│   ├── Migrations/                      # Code First migrations (InitialCreate)
│   └── DependencyInjection.cs           # AddInfrastructure(connectionString)
├── OrderManagement.Tests/
│   ├── Domain/                          # Pure business-rule tests (no DB, no mocks)
│   ├── Application/                     # Handler tests with fake repositories
│   └── Fakes/                           # In-memory I*Repository implementations
├── frontend/                            # React + TS + Tailwind (Vite)
│   └── src/{api,pages,components}/
├── Dockerfile                           # Multi-stage API image
├── docker-compose.yml                   # API + Oracle Free
└── OrderManagement.slnx
```

---

## Domain model & business rules

**Aggregates** (DDD):

- **Customer** — aggregate root (`Activate()` / `Deactivate()`)
- **Product** — aggregate root (`ChangePrice()`, `IncreaseStock()` / `DecreaseStock()`, activate/deactivate)
- **Order** — aggregate root for **OrderItems**; all item access goes through `Order`
  (`AddItem()`, `RemoveItem()`, `Confirm()`, `StartProcessing()`, `Ship()`, `Deliver()`, `Cancel()`)
- **OrderItem** — entity inside the Order aggregate (`ChangeQuantity()`)
- **Money**, **Address** — value objects

Aggregates reference each other by ID only (`Order.CustomerId`, `OrderItem.ProductId`),
never by object navigation — cross-aggregate coordination lives in Application handlers.

**Key rules enforced by the domain:**

1. Customer must be active to create an order (checked in `CreateOrderHandler`, since `Order` cannot query customers).
2. Orders start as `Pending`.
3. Confirm requires `Pending` status **and** at least one item.
4. Items change only while `Pending`.
5. Lifecycle: `Pending → Confirmed → Processing → Shipped → Delivered`; cancel allowed from `Pending`/`Confirmed` only.
6. Quantity > 0; price ≥ 0; stock never negative; can't order more than available stock.
7. `OrderItem.UnitPrice` is a **historical snapshot** — later `Product.Price` changes don't rewrite history.
8. Username/email unique (checked in handler + enforced by DB unique indexes).

**Stock model:** adding an item immediately reserves stock (`DecreaseStock`); removing an
item or cancelling the order restores it. Confirm itself doesn't touch stock.

---

## How a request flows

```
POST /api/orders/{id}/items   { productId, quantity }
  │  Api: OrdersController.AddItem → maps JSON to AddOrderItemCommand
  ▼
  Application: AddOrderItemHandler
    loads Order + Product → checks (active? enough stock?)
    → product.DecreaseStock() + order.AddItem()   (domain invariants)
  │  Repository interfaces: IOrderRepository / IProductRepository
  ▼
  Infrastructure: OrderRepository / ProductRepository → shared DbContext
  │  EF Core change tracker → SQL via Oracle provider
  ▼
  Oracle: rows in Orders / OrderItems / Products updated
  ◀  OrderDto (with TotalAmount) → 200 OK as JSON
```

- **Commands** change state (`AddOrderItemCommand` + handler, `SaveChangesAsync`).
- **Queries** only read (`GetOrderByIdQuery`, `GetCustomerOrdersQuery`).
- Errors: `NotFoundException → 404`, `BusinessRuleException` / `InvalidOperationException` / `ArgumentException → 400`, anything else → 500 — all via `ExceptionHandlingMiddleware`, so controllers contain no try/catch.

---

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for Oracle + API containers)
- Node.js 20+ (for the frontend)
- `dotnet-ef` tool (only if you add migrations): `dotnet tool install -g dotnet-ef`

### Quick start (recommended)

```bash
# 1. Start API + Oracle
docker compose up --build -d

# 2. Wait until Oracle is healthy
docker compose ps   # oracle must show "(healthy)"

# 3. First run only: create the tables
dotnet ef database update \
  --project OrderManagement.Infrastructure \
  --startup-project OrderManagement.Api

# 4. Use it
curl http://localhost:8080/api/products   # → []
```

Open the UI (dev server, see [Frontend](#frontend)) at **http://localhost:5173**,
or explore the API via `OrderManagement.Api.http` (VS Code REST Client / Rider) —
it contains ready-to-run requests for the whole order lifecycle.

### Run the backend locally (without Docker for the API)

```bash
docker compose up oracle -d              # database only
dotnet ef database update \
  --project OrderManagement.Infrastructure \
  --startup-project OrderManagement.Api
dotnet run --project OrderManagement.Api  # port shown in output / launchSettings.json
```

OpenAPI JSON (Development): `/openapi/v1.json`.

---

## API reference

Base URL: `http://localhost:8080` (Docker) — all JSON, error shape `{title, detail, status}`.

**Customers** (`/api/customers`)

| Method & route | Body | Success |
|---|---|---|
| `POST /` | `{username, email}` | `201` Customer |
| `GET /` | — | `200` Customer[] |
| `GET /{id}` | — | `200` Customer / `404` |
| `POST /{id}/activate` | — | `204` |
| `POST /{id}/deactivate` | — | `204` |

**Products** (`/api/products`)

| Method & route | Body | Success |
|---|---|---|
| `POST /` | `{name, price, stock}` | `201` Product |
| `GET /` | — | `200` Product[] |
| `GET /{id}` | — | `200` Product / `404` |
| `PUT /{id}/price` | `{newPrice}` | `204` |
| `POST /{id}/stock/increase` | `{quantity}` | `204` |
| `POST /{id}/stock/decrease` | `{quantity}` | `204` |
| `POST /{id}/activate` | — | `204` |
| `POST /{id}/deactivate` | — | `204` |

**Orders** (`/api/orders`)

| Method & route | Body | Success |
|---|---|---|
| `POST /` | `{customerId}` | `201` Order |
| `GET /{id}` | — | `200` Order / `404` |
| `GET /customer/{customerId}` | — | `200` Order[] |
| `POST /{id}/items` | `{productId, quantity}` | `200` Order |
| `DELETE /{id}/items/{itemId}` | — | `200` Order |
| `POST /{id}/confirm` | — | `200` Order |
| `POST /{id}/cancel` | — | `200` Order |
| `POST /{id}/process` | — | `200` Order |
| `POST /{id}/ship` | — | `200` Order |
| `POST /{id}/deliver` | — | `200` Order |

Order JSON: `{id, customerId, status (0–5), createdAt, items: [{id, productId, quantity, unitPrice, lineTotal}], totalAmount}`.

---

## Configuration

| Setting | Local default | Docker override |
|---|---|---|
| Oracle connection | `localhost:1521/FREEPDB1` (`appsettings.json`) | `oracle:1521/FREEPDB1` via `ConnectionStrings__Oracle` env var |
| DB user / password | `ordermgmt` / `ordermgmt` | same (see `docker-compose.yml`) |
| API port | launch profile | `8080` (`ASPNETCORE_URLS=http://+:8080`) |
| Frontend → API | `VITE_API_URL=http://localhost:8080` (`frontend/.env`) | — |
| CORS origins | `http://localhost:5173`, `:5174` (`Program.cs`) | — |

---

## Database & migrations

- Code First, Oracle provider `Oracle.EntityFrameworkCore` 10.23.
- One `IEntityTypeConfiguration` per aggregate (`Configurations/`); `DbContext` stays small via `ApplyConfigurationsFromAssembly`.
- Money mapped as `NUMBER(18,2)`; `OrderStatus` enum stored as int; Order→items uses a shadow FK `OrderId` (keeps the domain free of persistence IDs) with cascade delete; customer/order history uses `Restrict`.
- Design-time factory (`OrderManagementDbContextFactory`) supplies a connection string for tooling; runtime uses DI configuration.

```bash
# add a migration
dotnet ef migrations add <Name> \
  --project OrderManagement.Infrastructure \
  --startup-project OrderManagement.Api

# apply
dotnet ef database update \
  --project OrderManagement.Infrastructure \
  --startup-project OrderManagement.Api
```

---

## Frontend

React + TypeScript + Tailwind, deliberately as thin as the controllers: no store, no
business logic — the backend is the source of truth and each action re-fetches.
Server rule violations surface verbatim in the UI via the error banner.

```
frontend/src/
├── api/          # types.ts (DTOs), client.ts (fetch wrapper + ApiError),
│                 # customers.ts / products.ts / orders.ts (one fn per endpoint)
├── pages/        # CustomersPage, ProductsPage, OrdersPage, OrderDetailPage
└── components/   # ui.tsx — Page, Card, Field, Btn, ErrorBanner, badges
```

```bash
npm install --prefix frontend
npm run dev --prefix frontend     # http://localhost:5173 (API must run on :8080)
npm run build --prefix frontend   # type-check + production build
```

---

## Testing

```bash
dotnet test OrderManagement.Tests/OrderManagement.Tests.csproj
```

- `Tests/Domain/` — business rules with zero infrastructure: empty-order confirm, post-confirmation edits, lifecycle skips, cancel matrix (`[Theory]` over all statuses), stock guards, price-snapshot preservation.
- `Tests/Application/` — handler coordination via fake repositories: inactive-customer rejection, stock reservation, over-stock rejection, confirm-requires-items.
- `Tests/Fakes/` — in-memory `I*Repository` implementations; swapping them for EF repositories without touching handlers is Dependency Inversion made concrete.

---

## Docker

```bash
docker compose up --build -d   # build + start
docker compose ps              # check health
docker compose stop            # stop, keep data
docker compose down            # remove containers (volume data kept)
docker compose down -v         # ⚠️ also deletes the database data
```

- `Dockerfile`: multi-stage (SDK builds → ASP.NET runtime runs), listens on `8080`.
- `docker-compose.yml`: `api` + `gvenzl/oracle-free:23-slim` with a `sqlplus` healthcheck gating API startup; data persisted in the `oracle-data` volume.
- Migrations are applied from the host (see above), not at container startup.

---

## Key decisions & gotchas

1. **No MediatR, no generic repositories.** Handlers are injected directly and each repository interface is aggregate-specific — every step of a use case stays greppable for learning.
2. **Oracle Free, not XE.** There is no `oracle-xe:23` (XE ended at 21c; 23ai lives in `gvenzl/oracle-free`). Bonus: `oracle-free` ships native ARM images. Note the PDB name differs: `FREEPDB1`, not `XEPDB1`.
3. **`.dockerignore` needs `**/bin/` + `**/obj/`.** Plain `bin/` only matches the repo root, so host `obj/` (with machine-specific `project.assets.json`) overwrote the container's restore and broke publish with `NETSDK1064`.
4. **EF discovery + client-generated Guids.** `DetectChanges` marks navigation-discovered entities with *already-set* keys as `Modified` (it assumes existing rows). Since `OrderItem` gets its `Guid` in the constructor, new items must be registered explicitly — hence `IOrderRepository.AddItem()` (aggregate boundary preserved: still no item repository, no `DbSet` in handlers). `Update()` implementations mark only the root (`Entry(...).State`) instead of graph-walking `Update()`.
5. **EF version pinning.** `Oracle.EntityFrameworkCore` 10.23 depends on EF 10.0.0 while the projects reference EF 10.0.9 — harmless unification warnings (`MSB3277`); if a scratch/test project fails at runtime with `FileNotFoundException` for `Relational 10.0.9.0`, add a direct `Microsoft.EntityFrameworkCore.Relational` 10.0.9 reference.
6. **Private constructors in entities are for EF only** — application code must use the public constructors so aggregates are always created valid.

---

## Learning guide

Suggested review order with a tutor:

1. `Domain/Entities/Order.cs` — how one class guards status + item rules together (aggregate consistency).
2. `Application/Orders/Commands/AddOrderItem/AddOrderItemHandler.cs` — cross-aggregate coordination: load both aggregates → app checks → domain behaviors → one save.
3. `Application/Orders/Repositories/IOrderRepository.cs` — the abstraction Application owns; note `AddItem` and why `GetByIdAsync` must include items.
4. `Infrastructure/Persistence/Repositories/OrderRepository.cs` — the EF translation, especially `AddItem` vs `Update`.
5. `Infrastructure/Persistence/Configurations/` — indexes, precision, shadow FK, delete behaviors.
6. `Api/Controllers/OrdersController.cs` + `Middleware/ExceptionHandlingMiddleware.cs` — thin controllers, centralized errors.
7. `Application/DependencyInjection.cs` + `Infrastructure/DependencyInjection.cs` + `Api/Program.cs` — where interfaces meet implementations (composition root).
8. `Tests/` — run them, then break a domain rule and watch the right test fail.

---

## Troubleshooting

| Symptom | Cause / fix |
|---|---|
| `oracle-xe:23-slim: not found` | Wrong image; this repo already uses `gvenzl/oracle-free:23-slim` |
| `ORA-12541` / connection refused | Oracle not ready — wait for `(healthy)` in `docker compose ps` |
| `ORA-00942: table or view does not exist` | Migrations not applied — run `dotnet ef database update` |
| `table already exists` on migrate | DB already migrated — just run the app |
| `NETSDK1064` in `docker build` | Stale `obj/` in build context — check `.dockerignore` has `**/obj/` |
| `DbUpdateConcurrencyException` on add-item | Fixed via explicit `AddItem`; see gotcha #4 |
| Browser `CORS` / `Failed to fetch` in UI | API must allow the dev origin — `UseCors("Frontend")` + API on `:8080`, `VITE_API_URL` pointing at it |
| Port conflicts on `1521`/`8080` | `docker compose down` to stop existing containers |
