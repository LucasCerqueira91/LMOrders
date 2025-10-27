# LM Orders - Microserviço de Pedidos

## 📋 Visão Geral

Microserviço desenvolvido para gerenciar pedidos da LM Mobilidade, implementando arquitetura de microserviços com armazenamento híbrido e comunicação assíncrona.

## 🏗️ Arquitetura

### Padrões Utilizados
- **Domain-Driven Design (DDD)**: Separação clara entre Domain, Application e Infrastructure
- **CQRS**: Separação entre Commands e Queries
- **Repository Pattern**: Abstração da camada de dados
- **Mediator Pattern**: Desacoplamento através do MediatR
- **SOLID Principles**: Código limpo e manutenível

@startuml LMOrders-Modules
title LMOrders – Módulos e Dependências

skinparam componentStyle rectangle
skinparam packageStyle rectangle
skinparam ArrowColor #888

package "LMOrders" {
  [LM.Orders.Api] as Api
  [LM.Orders.Application] as App
  [LM.Orders.Domain] as Domain
  [LM.Orders.Infrastructure] as Infra
  [LM.Orders.Tests] as Tests

  Api -down-> App : Controllers / DI / Swagger
  App -down-> Domain : Commands/Queries usam Entidades/VOs
  App -right-> Infra : Implementações (Repo/EF/Bus)
  Infra -up-> Domain : Repo/EF referenciam o domínio

  Tests ..> Api : API/Integration Tests
  Tests ..> App : Application/Unit Tests
  Tests ..> Domain : Domain/Unit Tests
}

rectangle "Domain" as D {
  [Entity<TId>]
  [Order]
  [OrderItem]
  [ValueObjects\nOrderId(Guid), ProductId, Money]
  [Events\nOrderCreated, OrderItemAdded]
}

rectangle "Application" as A {
  [Commands\nCreateOrder, AddItemToOrder, CheckoutOrder]
  [Queries\nGetOrderById, ListOrders]
  [Handlers\nCreateOrderHandler, AddItemHandler]
  [DTOs/Mappings]
  [Behaviors\nValidation, Logging, Transaction]
}

rectangle "Infrastructure" as I {
  [EF Core Context]
  [Repositories]
  [Migrations]
  [Outbox/Bus (opcional)]
  [Adapters externos]
}

Api -[hidden]-> D
A -[hidden]-> I
@enduml


@startuml LMOrders-Folders
title LMOrders – Mapa de Pastas (alto nível)

' Requer PlantUML >= v1.2020 para MindMap/WBS
@startmindmap
* LMOrders
** src
*** LM.Orders.Api
**** Controllers
**** Config (DI/Swagger/Health)
**** Filters/Middlewares
*** LM.Orders.Application
**** Commands
***** CreateOrder
***** AddItemToOrder
**** Queries
***** GetOrderById
***** ListOrders
**** Handlers
**** DTOs
**** Behaviors (Validation/Logging)
*** LM.Orders.Domain
**** Entities
***** Order
***** OrderItem
**** ValueObjects
***** OrderId (Guid)
***** ProductId
***** Money
**** Events
*** LM.Orders.Infrastructure
**** Persistence
***** DbContext (EF)
***** Migrations
**** Repositories
**** Outbox/Bus (opcional)
** tests
*** LM.Orders.Tests
**** Domain
**** Application
**** Api/Integration
@endmindmap
@enduml


@startuml LMOrders-CQRS-CreateOrder
title Fluxo CQRS – CreateOrder

actor Client
participant "API\n(OrdersController)" as API
participant "Application\n(CreateOrderHandler)" as APP
participant "Domain\n(Order, VOs)" as DOM
database "Infra\n(EF Core / Repo)" as DB
participant "Bus/Outbox\n(opcional)" as BUS

Client -> API : POST /orders { customerId, items[] }
API -> APP : Send(CreateOrderCommand)
APP -> DOM : new Order(OrderId, items…)\nvalida regras (invariantes)
DOM --> APP : Order + DomainEvent(OrderCreated)
APP -> DB : UnitOfWork.Begin()\nRepository.Add(Order)
DB --> APP : OK
APP -> BUS : Outbox.Append(OrderCreated) (opcional)
APP -> DB : UnitOfWork.Commit()
APP --> API : OrderId
API --> Client : 201 Created { orderId }

@enduml


## 🚀 Funcionalidades

### ✅ Implementadas
- **POST /orders**: Criação de pedidos com validação
- **GET /orders/{id}**: Consulta de pedidos com cache Redis (2 min)
- **Armazenamento Híbrido**: SQL Server + MongoDB
- **Eventos Assíncronos**: RabbitMQ para comunicação
- **Cache**: Redis para otimização de consultas
- **Validação**: FluentValidation para validação de dados
- **Testes Unitários**: Cobertura de casos críticos

## 🛠️ Tecnologias

- **.NET 8**: Framework principal
- **Entity Framework Core**: ORM para SQL Server
- **MongoDB Driver**: Acesso ao MongoDB
- **Redis**: Cache distribuído
- **RabbitMQ**: Message broker
- **MediatR**: Mediator pattern
- **FluentValidation**: Validação de dados
- **xUnit**: Testes unitários
- **Docker**: Containerização

## 🐳 Executando com Docker

### Pré-requisitos
- Docker Desktop
- Docker Compose

### Comandos
```bash
# Clonar o repositório
git clone <repository-url>
cd LMOrders

# Executar todos os serviços
docker-compose up -d

# Verificar logs
docker-compose logs -f lm-orders-api

# Parar os serviços
docker-compose down
```

### Serviços Disponíveis
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **RabbitMQ Management**: http://localhost:15672 (admin/admin)
- **SQL Server**: localhost:1433
- **MongoDB**: localhost:27017
- **Redis**: localhost:6379

## 🧪 Executando Localmente

### Pré-requisitos
- .NET 8 SDK
- SQL Server (ou Docker)
- MongoDB
- Redis
- RabbitMQ

### Configuração
1. **Configurar connection strings** no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Sql": "Server=localhost,1433;Database=OrdersDb;User Id=sa;Password=P@ssw0rd!;TrustServerCertificate=True",
    "Mongo": "mongodb://localhost:27017",
    "Redis": "localhost:6379"
  },
  "RabbitHost": "localhost"
}
```

2. **Executar a aplicação**:
```bash
cd src/LM.Orders.Api
dotnet run
```

3. **Executar testes**:
```bash
cd tests/LM.Orders.Tests
dotnet test
```

## 📊 Decisões Técnicas

### 1. **Armazenamento Híbrido**
- **SQL Server**: Dados principais do pedido (ID, Cliente, Status, Data)
- **MongoDB**: Itens do pedido (flexibilidade para produtos variados)
- **Justificativa**: Separação de responsabilidades e otimização de consultas

### 2. **Cache Redis**
- **TTL**: 2 minutos para pedidos consultados
- **Justificativa**: Reduz carga no banco e melhora performance

### 3. **Eventos Assíncronos**
- **RabbitMQ**: Comunicação com sistema de faturamento
- **Justificativa**: Desacoplamento e resiliência

### 4. **Minimal APIs**
- **Escolha**: APIs mais leves e performáticas
- **Justificativa**: Menos overhead para microserviços

### 5. **CQRS com MediatR**
- **Separação**: Commands (escrita) vs Queries (leitura)
- **Justificativa**: Escalabilidade e manutenibilidade

## 🔧 Endpoints da API

### POST /orders
Cria um novo pedido.

**Request:**
```json
{
  "id": "guid",
  "customerId": "string",
  "createdAt": "datetime",
  "status": "Created|Paid|Cancelled",
  "items": [
    {
      "product": "string",
      "quantity": 0,
      "unitPrice": 0.00
    }
  ]
}
```

**Response:**
```json
{
  "id": "guid"
}
```

### GET /orders/{id}
Consulta um pedido por ID.

**Response:**
```json
{
  "id": "guid",
  "customerId": "string",
  "createdAt": "datetime",
  "status": "string",
  "totalAmount": 0.00,
  "items": [
    {
      "product": "string",
      "quantity": 0,
      "unitPrice": 0.00
    }
  ]
}
```

## 🧪 Testes

### Executando Testes
```bash
# Todos os testes
dotnet test

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Cobertura de Testes
- ✅ Criação de pedidos válidos
- ✅ Validação de dados inválidos
- ✅ Cálculo de totais
- ✅ Mudança de status


## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.