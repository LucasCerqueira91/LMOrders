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

### Módulos e Dependências

O projeto é organizado em 4 camadas principais:

**LM.Orders.Api** - Camada de apresentação (Controllers, DI, Swagger, Filters/Middlewares)  
**LM.Orders.Application** - Camada de casos de uso (Commands, Queries, Handlers, DTOs, Behaviors)  
**LM.Orders.Domain** - Camada de domínio (Entities, Value Objects, Events)  
**LM.Orders.Infrastructure** - Camada de infraestrutura (Repositories, EF, Mongo, Redis, RabbitMQ)

**Principais Componentes:**

**Domain Layer:**
- `Entity<TId>` - Classe base para entidades
- `Order`, `OrderItem` - Entidades de domínio
- `OrderId`, `ProductId`, `Money` - Value Objects
- `OrderCreated`, `OrderItemAdded` - Eventos de domínio

**Application Layer:**
- `CreateOrderCommand`, `AddItemToOrderCommand` - Commands
- `GetOrderByIdQuery`, `ListOrdersQuery` - Queries
- `CreateOrderHandler`, `AddItemHandler` - Handlers
- `ValidationBehavior`, `LoggingBehavior` - Behaviors (Pipeline)

**Infrastructure Layer:**
- `AppDbContext` - Entity Framework Core
- `OrderSqlRepository` - Repositório SQL Server
- `OrderItemsMongoRepository` - Repositório MongoDB
- `RedisCacheService` - Cache distribuído
- `RabbitEventBus` - Message broker

**Estrutura do Projeto:**

1. **src/** - Código fonte do projeto

   - **LM.Orders.Api** - API REST (Minimal APIs)
   
   - **LM.Orders.Application** - Casos de uso e regras de negócio
   
   - **LM.Orders.Domain** - Entidades e regras de domínio
   
   - **LM.Orders.Infrastructure** - Implementações de infraestrutura
     - Sql - SQL Server (dados principais)
     - Mongo - MongoDB (itens do pedido)
     - Cache - Redis (cache)
     - Messaging - RabbitMQ (eventos)

2. **tests/** - Testes do projeto

   - **LM.Orders.Tests** - Testes unitários e de integração

### Fluxo CQRS - Criação de Pedido

**Passo a passo:**

1. **Cliente** envia requisição `POST /orders` com `customerId` e `items[]`

2. **API (Minimal API)** recebe a requisição e envia `CreateOrderCommand` via MediatR

3. **Application Layer** processa o comando:
   - `ValidationBehavior` valida os dados
   - `CreateOrderHandler` processa o comando

4. **Domain Layer** cria a entidade:
   - Instancia `Order` com `OrderId` e `items`
   - Valida regras de negócio (invariantes)
   - Gera evento de domínio `OrderCreated`

5. **Infrastructure Layer** persiste os dados:
   - `OrderSqlRepository.Add(Order)` - Persiste no SQL Server
   - `OrderItemsMongoRepository.Add(Items)` - Persiste no MongoDB
   - `UnitOfWork.Commit()` - Confirma transação

6. **Event Bus** publica evento assíncrono:
   - `RabbitEventBus.Publish(OrderCreated)` - Envia para fila

7. **Resposta** retorna `201 Created` com o `orderId`



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

**Pré-requisitos:**
- Docker Desktop
- Docker Compose

**Comandos:**

1. **Clonar o repositório:**
   ```bash
   git clone <repository-url>
   cd LMOrders
   ```

2. **Executar todos os serviços:**
   ```bash
   docker-compose up -d
   ```

3. **Verificar logs:**
   ```bash
   docker-compose logs -f lm-orders-api
   ```

4. **Parar os serviços:**
   ```bash
   docker-compose down
   ```

**Serviços Disponíveis:**
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- RabbitMQ Management: http://localhost:15672 (admin/admin)
- SQL Server: localhost:1433
- MongoDB: localhost:27017
- Redis: localhost:6379

## 🧪 Executando Localmente

**Pré-requisitos:**
- .NET 8 SDK
- SQL Server (ou Docker)
- MongoDB
- Redis
- RabbitMQ

**Configuração:**

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

2. **Executar a aplicação:**
   ```bash
   cd src/LM.Orders.Api
   dotnet run
   ```

3. **Executar testes:**
   ```bash
   cd tests/LM.Orders.Tests
   dotnet test
   ```

## 📊 Decisões Técnicas

**1. Armazenamento Híbrido**
- SQL Server: Dados principais do pedido (ID, Cliente, Status, Data)
- MongoDB: Itens do pedido (flexibilidade para produtos variados)
- Justificativa: Separação de responsabilidades e otimização de consultas

**2. Cache Redis**
- TTL: 2 minutos para pedidos consultados
- Justificativa: Reduz carga no banco e melhora performance

**3. Eventos Assíncronos**
- RabbitMQ: Comunicação com sistema de faturamento
- Justificativa: Desacoplamento e resiliência

**4. Minimal APIs**
- Escolha: APIs mais leves e performáticas
- Justificativa: Menos overhead para microserviços

**5. CQRS com MediatR**
- Separação: Commands (escrita) vs Queries (leitura)
- Justificativa: Escalabilidade e manutenibilidade

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

**Executando Testes:**

1. **Todos os testes:**
   ```bash
   dotnet test
   ```

2. **Com cobertura:**
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```

**Cobertura de Testes:**
- ✅ Criação de pedidos válidos
- ✅ Validação de dados inválidos
- ✅ Cálculo de totais
- ✅ Mudança de status




## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.