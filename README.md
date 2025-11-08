# RentABike - Sistema de Aluguel de Motos

Aplicação de microserviço desenvolvida em .NET 8 seguindo os princípios SOLID e Clean Architecture para gerenciar aluguel de motos e entregadores.

## 📋 Índice

- [Arquitetura](#arquitetura)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Funcionalidades](#funcionalidades)
- [Regras de Negócio](#regras-de-negócio)
- [Configuração](#configuração)
- [Executando a Aplicação](#executando-a-aplicação)
- [Docker](#docker)
- [Testes](#testes)
- [Estrutura de Pastas](#estrutura-de-pastas)
- [Princípios Aplicados](#princípios-aplicados)
- [Swagger de Referência](#swagger-de-referência)

## 🏗️ Arquitetura

A aplicação está organizada em camadas seguindo Clean Architecture e Domain-Driven Design (DDD):

- **RentABike.Domain**: Entidades de domínio, interfaces, eventos e contratos
- **RentABike.Application**: Services de aplicação, DTOs, mapeamentos e validações
- **RentABike.Infrastructure**: Implementações de repositórios, mensageria, storage, banco de dados e workers
- **RentABike.API**: Controllers REST e configuração da API

## 🛠️ Tecnologias Utilizadas

### Backend
- **.NET 8** - Framework principal
- **Entity Framework Core 8** - ORM para acesso a dados
- **PostgreSQL** - Banco de dados relacional
- **MassTransit + RabbitMQ** - Mensageria para eventos assíncronos
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validação de dados
- **Serilog** - Logging estruturado

### Testes
- **xUnit** - Framework de testes unitários
- **Moq** - Framework para criação de mocks
- **FluentAssertions** - Biblioteca para assertions mais legíveis
- **Entity Framework InMemory** - Banco em memória para testes de repositórios

### Documentação
- **Swagger/OpenAPI** - Documentação automática da API

## ✨ Funcionalidades

### Motos
- ✅ Cadastro de motos (Identificador, Ano, Modelo, Placa)
- ✅ Validação de placa única
- ✅ Evento de moto cadastrada publicado via mensageria (RabbitMQ)
- ✅ Consumidor que armazena notificações para motos do ano 2024
- ✅ Worker em background para monitoramento de mensageria
- ✅ Consulta de motos com filtro por placa
- ✅ Atualização de placa
- ✅ Remoção de moto (apenas se não houver locações)

### Entregadores
- ✅ Cadastro de entregadores (Identificador, Nome, CNPJ, Data de Nascimento, Número da CNH, Tipo da CNH)
- ✅ Validação de CNPJ único
- ✅ Validação de número da CNH único
- ✅ Tipos de CNH válidos: A, B ou A+B
- ✅ Upload de imagem da CNH (PNG ou BMP)
- ✅ Armazenamento de imagens em storage local (extensível para S3, MinIO, etc.)
- ✅ Atualização de imagem da CNH

### Locações
- ✅ Criação de locação com planos de 7, 15, 30, 45 ou 50 dias
- ✅ Validação de entregador habilitado (CNH tipo A ou AB)
- ✅ Validação de locação ativa por entregador
- ✅ Cálculo automático de valores por plano
- ✅ Data de início obrigatoriamente no primeiro dia após criação
- ✅ Devolução com cálculo de multas e valores adicionais
- ✅ Consulta de valor total da locação (simulação sem persistir)

## 📐 Regras de Negócio

### Planos de Locação
- **7 dias**: R$ 30,00/dia
- **15 dias**: R$ 28,00/dia
- **30 dias**: R$ 22,00/dia
- **45 dias**: R$ 20,00/dia
- **50 dias**: R$ 18,00/dia

### Multas e Valores Adicionais

#### Devolução Antecipada
- **Plano de 7 dias**: 20% sobre o valor das diárias não efetivadas
- **Plano de 15 dias**: 40% sobre o valor das diárias não efetivadas
- **Outros planos**: Sem multa

#### Devolução Atrasada
- R$ 50,00 por diária adicional

### Validações
- Apenas entregadores com CNH tipo A ou AB podem alugar motos
- Um entregador não pode ter mais de uma locação ativa simultaneamente
- Não é possível remover uma moto que possui locações registradas
- Placa da moto deve ser única
- CNPJ do entregador deve ser único
- Número da CNH deve ser único

## ⚙️ Configuração

### Banco de Dados
A aplicação utiliza PostgreSQL. Configure a connection string no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=RentABikeDb;Username=admin;Password=admin123"
  }
}
```

**Nota:** Ajuste os valores de `Host`, `Port`, `Username` e `Password` conforme sua instalação do PostgreSQL.

### RabbitMQ
Configure o RabbitMQ no `appsettings.json`:

```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "admin",
    "Password": "admin123"
  }
}
```

**Nota:** Certifique-se de que o RabbitMQ está instalado e rodando antes de iniciar a aplicação.

### Storage
Por padrão, as imagens são armazenadas localmente. Configure no `appsettings.json`:

```json
{
  "Storage": {
    "Local": {
      "BasePath": "wwwroot/uploads",
      "BaseUrl": "/uploads"
    }
  }
}
```

**Nota:** O serviço de storage é extensível e pode ser facilmente adaptado para S3, MinIO ou outros provedores de storage.

## 🚀 Executando a Aplicação

### Opção 1: Docker Compose (Recomendado) ⭐

A forma mais fácil de executar a aplicação é usando Docker Compose, que inclui todos os serviços necessários:

```bash
# Clone o repositório
git clone <repository-url>
cd RentABikeSolution

# Iniciar todos os serviços (PostgreSQL, RabbitMQ e API)
docker-compose up -d --build

# Ver logs
docker-compose logs -f api

# Parar serviços
docker-compose down
```

A aplicação estará disponível em: **http://localhost:8080**

**Swagger**: http://localhost:8080/swagger (habilitar via variável `EnableSwagger=true`)

**RabbitMQ Management**: http://localhost:15672 (usuário: admin, senha: admin123)

**Importante**: Execute as migrações do banco de dados antes de iniciar a API:

```bash
# Aguardar PostgreSQL estar pronto
sleep 5

# Executar migrações
dotnet ef database update --project src/RentABike.Infrastructure --startup-project src/RentABike.API
```

### Opção 2: Execução Local

#### Pré-requisitos
- .NET 8 SDK
- PostgreSQL 12 ou superior
- RabbitMQ 3.8 ou superior

#### Passos para Execução

1. **Clone o repositório**:
```bash
git clone <repository-url>
cd RentABikeSolution
```

2. **Iniciar dependências com Docker**:
```bash
# PostgreSQL
docker run -d --name postgres-local \
  -e POSTGRES_USER=admin \
  -e POSTGRES_PASSWORD=admin123 \
  -e POSTGRES_DB=RentABikeDb \
  -e LANG=en_US.UTF-8 \
  -e LC_ALL=en_US.UTF-8 \
  -e PGDATA=/var/lib/postgresql/data/pgdata \
  -p 5432:5432 \
  -v pgdata:/var/lib/postgresql/data \
  postgres:18

# RabbitMQ
docker run -d --name rabbitmq-local \
  -e RABBITMQ_DEFAULT_USER=admin \
  -e RABBITMQ_DEFAULT_PASS=admin123 \
  -p 5672:5672 \
  -p 15672:15672 \
  -v rabbitmq_data:/var/lib/rabbitmq \
  rabbitmq:3-management
```

3. **Restaurar dependências**:
```bash
dotnet restore
```

4. **Aplicar migrações do banco de dados**:
```bash
dotnet ef database update --project src/RentABike.Infrastructure --startup-project src/RentABike.API
```

5. **Executar a aplicação**:
```bash
cd src/RentABike.API
dotnet run
```

6. **Acessar Swagger**:
```
https://localhost:5001/swagger
ou
http://localhost:5000/swagger
```

### Opção 3: Docker (Apenas API)

Se você já tem PostgreSQL e RabbitMQ rodando:

```bash
# Build da imagem
docker build -t rentabike-api:latest .

# Executar container
docker run -d \
  --name rentabike-api \
  -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=RentABikeDb;Username=admin;Password=admin123" \
  -e RabbitMQ__Host=host.docker.internal \
  -e RabbitMQ__Port=5672 \
  -e RabbitMQ__Username=admin \
  -e RabbitMQ__Password=admin123 \
  rentabike-api:latest
```

Para mais informações sobre Docker, consulte [DOCKER.md](DOCKER.md).

## 🐳 Docker

O projeto inclui suporte completo para Docker com os seguintes arquivos:

- **Dockerfile**: Multi-stage build otimizado para produção
- **docker-compose.yml**: Orquestração completa (API + PostgreSQL + RabbitMQ)
- **docker-compose.override.yml**: Configurações para desenvolvimento local
- **.dockerignore**: Otimização do build excluindo arquivos desnecessários

### Comandos Rápidos

```bash
# Build e iniciar tudo
docker-compose up -d --build

# Ver logs
docker-compose logs -f api

# Parar tudo
docker-compose down

# Rebuild apenas a API
docker-compose build api
docker-compose up -d api
```

Para detalhes completos, consulte [DOCKER.md](DOCKER.md).

## 🧪 Testes

O projeto possui uma suíte completa de testes unitários cobrindo todas as camadas da aplicação.

### Executando os Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes de um projeto específico
dotnet test tests/RentABike.Tests

# Executar com output detalhado
dotnet test --logger "console;verbosity=detailed"

# Executar com cobertura de código
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Cobertura de Testes

- ✅ **Domain Entities**: 19 testes
  - Rental (6 testes)
  - Motorcycle (4 testes)
  - DeliveryPerson (5 testes)
  - LicenseType (4 testes via Theory)

- ✅ **Application Services**: 20 testes
  - MotorcycleService (8 testes)
  - DeliveryPersonService (6 testes)
  - RentalService (6 testes)

- ✅ **API Controllers**: 20 testes
  - MotorcyclesController (7 testes)
  - DeliveryPersonsController (6 testes)
  - RentalsController (7 testes)

- ✅ **Infrastructure Repositories**: 28 testes
  - MotorcycleRepository (10 testes)
  - DeliveryPersonRepository (8 testes)
  - RentalRepository (10 testes)

**Total: 93 testes unitários** ✅

### Estrutura de Testes

```
tests/RentABike.Tests/
├── Domain/
│   └── Entities/          # Testes de entidades de domínio
├── Application/
│   └── Services/          # Testes de services
├── API/
│   └── Controllers/       # Testes de controllers
└── Infrastructure/
    └── Repositories/      # Testes de repositórios
```

## 📁 Estrutura de Pastas

```
RentABikeSolution/
├── src/
│   ├── RentABike.API/                    # Camada de apresentação
│   │   ├── Controllers/                  # Controllers REST
│   │   │   ├── MotorcyclesController.cs
│   │   │   ├── DeliveryPersonsController.cs
│   │   │   └── RentalsController.cs
│   │   ├── Program.cs                     # Configuração da API
│   │   ├── appsettings.json              # Configurações
│   │   └── wwwroot/uploads/               # Armazenamento local de imagens
│   │
│   ├── RentABike.Application/             # Camada de aplicação
│   │   ├── Services/                      # Services de aplicação
│   │   │   ├── Interfaces/
│   │   │   │   ├── IMotorcycleService.cs
│   │   │   │   ├── IDeliveryPersonService.cs
│   │   │   │   └── IRentalService.cs
│   │   │   ├── MotorcycleService.cs
│   │   │   ├── DeliveryPersonService.cs
│   │   │   └── RentalService.cs
│   │   ├── DTOs/                          # Data Transfer Objects
│   │   │   ├── MotorcycleDTO.cs
│   │   │   ├── DeliveryPersonDTO.cs
│   │   │   └── RentalDTO.cs
│   │   ├── Mappings/                      # AutoMapper profiles
│   │   │   └── MappingProfile.cs
│   │   └── Validators/                    # FluentValidation validators
│   │       ├── CreateMotorcycleDTOValidator.cs
│   │       ├── CreateDeliveryPersonDTOValidator.cs
│   │       └── CreateRentalDTOValidator.cs
│   │
│   ├── RentABike.Domain/                  # Camada de domínio
│   │   ├── Entities/                      # Entidades de domínio
│   │   │   ├── Motorcycle.cs
│   │   │   ├── DeliveryPerson.cs
│   │   │   ├── Rental.cs
│   │   │   ├── MotorcycleNotification.cs
│   │   │   └── LicenseType.cs
│   │   ├── Events/                        # Eventos de domínio
│   │   │   └── MotorcycleRegisteredEvent.cs
│   │   └── Interfaces/                    # Interfaces (contratos)
│   │       ├── IRepository.cs
│   │       ├── IMotorcycleRepository.cs
│   │       ├── IDeliveryPersonRepository.cs
│   │       ├── IRentalRepository.cs
│   │       ├── IMotorcycleNotificationRepository.cs
│   │       ├── IMessageBus.cs
│   │       └── IStorageService.cs
│   │
│   └── RentABike.Infrastructure/          # Camada de infraestrutura
│       ├── Data/                          # DbContext e configurações
│       │   ├── ApplicationDbContext.cs
│       │   └── ApplicationDbContextFactory.cs
│       ├── Repositories/                  # Implementações de repositórios
│       │   ├── MotorcycleRepository.cs
│       │   ├── DeliveryPersonRepository.cs
│       │   ├── RentalRepository.cs
│       │   └── MotorcycleNotificationRepository.cs
│       ├── Messaging/                     # Mensageria (RabbitMQ)
│       │   ├── MessageBus.cs
│       │   └── Consumers/
│       │       └── MotorcycleRegisteredConsumer.cs
│       ├── Workers/                       # Background workers
│       │   └── MessageConsumerWorker.cs
│       ├── Storage/                       # Serviço de storage
│       │   └── LocalStorageService.cs
│       └── Migrations/                    # Migrações do Entity Framework
│           └── 20251108200409_InitialCreate.cs
│
├── tests/
│   └── RentABike.Tests/                   # Projeto de testes unitários
│       ├── Domain/
│       │   └── Entities/
│       ├── Application/
│       │   └── Services/
│       ├── API/
│       │   └── Controllers/
│       └── Infrastructure/
│           └── Repositories/
│
└── RentABike.sln                          # Solution file
```

## 🎯 Princípios Aplicados

### SOLID
- **Single Responsibility**: Cada classe tem uma única responsabilidade
- **Open/Closed**: Extensível através de interfaces, sem modificar código existente
- **Liskov Substitution**: Implementações podem ser substituídas por suas interfaces
- **Interface Segregation**: Interfaces específicas e focadas
- **Dependency Inversion**: Dependências injetadas através de interfaces

### Domain-Driven Design (DDD)
- **Camada de Domínio**: Entidades ricas com lógica de negócio encapsulada
- **Camada de Aplicação**: Services que orquestram casos de uso
- **Camada de Infraestrutura**: Repositórios e implementações técnicas
- **Separação de Responsabilidades**: Cada camada tem responsabilidades bem definidas
- **Repository Pattern**: Abstração de acesso a dados
- **Service Layer**: Lógica de aplicação separada da lógica de domínio

### Clean Architecture
- **Independência de Frameworks**: Código de negócio não depende de frameworks
- **Testabilidade**: Fácil de testar através de mocks e injeção de dependência
- **Independência de UI**: Interface pode ser alterada sem afetar o core
- **Independência de Banco de Dados**: Pode trocar de banco sem alterar regras de negócio

## 📡 Mensageria

A aplicação utiliza MassTransit com RabbitMQ para processamento assíncrono de eventos:

- **Evento**: `MotorcycleRegisteredEvent` - Publicado quando uma moto é cadastrada
- **Consumer**: `MotorcycleRegisteredConsumer` - Processa eventos e armazena notificações para motos de 2024
- **Worker**: `MessageConsumerWorker` - Worker em background para monitoramento da mensageria

## 🔍 Swagger de Referência

A aplicação segue as especificações do Swagger:
https://app.swaggerhub.com/apis-docs/Mottu/mottu_desafio_backend/1.0.0