# Docker - Guia de Deploy

Este documento descreve como fazer o deploy da aplicação RentABike usando Docker.

## 📋 Pré-requisitos

- Docker 20.10 ou superior
- Docker Compose 2.0 ou superior

## 🚀 Deploy com Docker Compose

### Opção 1: Deploy Completo (Recomendado)

O `docker-compose.yml` inclui todos os serviços necessários (API, PostgreSQL e RabbitMQ):

```bash
# Build e iniciar todos os serviços
docker-compose up -d --build

# Ver logs
docker-compose logs -f api

# Parar todos os serviços
docker-compose down

# Parar e remover volumes (cuidado: apaga dados)
docker-compose down -v
```

### Opção 2: Apenas a API

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

## 🔧 Variáveis de Ambiente

### Configuração via docker-compose.yml

As variáveis de ambiente podem ser configuradas no `docker-compose.yml`:

```yaml
environment:
  - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=RentABikeDb;Username=admin;Password=admin123
  - RabbitMQ__Host=rabbitmq
  - RabbitMQ__Port=5672
  - RabbitMQ__Username=admin
  - RabbitMQ__Password=admin123
  - Storage__Local__BasePath=/app/wwwroot/uploads
  - Storage__Local__BaseUrl=/uploads
  - ASPNETCORE_ENVIRONMENT=Production
```

### Configuração via arquivo .env

Crie um arquivo `.env` na raiz do projeto:

```env
POSTGRES_USER=admin
POSTGRES_PASSWORD=admin123
POSTGRES_DB=RentABikeDb
RABBITMQ_USER=admin
RABBITMQ_PASS=admin123
```

E referencie no `docker-compose.yml`:

```yaml
environment:
  - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}
```

## 📦 Build da Imagem

### Build manual

```bash
# Build da imagem
docker build -t rentabike-api:latest .

# Build com tag específica
docker build -t rentabike-api:v1.0.0 .
```

### Build otimizado (usando cache)

```bash
# Build aproveitando cache de camadas
docker build --cache-from rentabike-api:latest -t rentabike-api:latest .
```

## 🗄️ Migrações do Banco de Dados

### Executar migrações antes de iniciar a API

**Importante**: As migrações devem ser aplicadas antes de iniciar a API pela primeira vez.

#### Opção 1: Executar migrações localmente (Recomendado)

```bash
# Certifique-se de que o PostgreSQL está rodando
docker-compose up -d postgres

# Aguardar PostgreSQL estar pronto
sleep 5

# Executar migração
dotnet ef database update \
  --project src/RentABike.Infrastructure \
  --startup-project src/RentABike.API

# Agora iniciar a API
docker-compose up -d api
```

#### Opção 2: Executar migrações via container temporário

```bash
# Criar um container temporário com .NET SDK para executar migrações
docker run --rm \
  --network rentabikesolution_default \
  -v ${PWD}:/app \
  -w /app \
  mcr.microsoft.com/dotnet/sdk:8.0 \
  dotnet ef database update \
    --project src/RentABike.Infrastructure \
    --startup-project src/RentABike.API \
    --connection "Host=postgres;Port=5432;Database=RentABikeDb;Username=admin;Password=admin123"
```

#### Opção 3: Executar migrações dentro do container da API (após build)

```bash
# Build da imagem primeiro
docker-compose build api

# Executar migrações usando o container
docker run --rm \
  --network rentabikesolution_default \
  -e ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=RentABikeDb;Username=admin;Password=admin123" \
  rentabikesolution-api:latest \
  dotnet ef database update --project /app/src/RentABike.Infrastructure --startup-project /app/src/RentABike.API
```

**Nota**: A opção mais simples é executar as migrações localmente antes de iniciar os containers.

## 🔍 Verificação e Debug

### Ver logs

```bash
# Logs da API
docker logs rentabike-api

# Logs em tempo real
docker logs -f rentabike-api

# Logs de todos os serviços
docker-compose logs -f
```

### Verificar saúde dos containers

```bash
# Status dos containers
docker-compose ps

# Inspecionar container
docker inspect rentabike-api

# Verificar recursos utilizados
docker stats
```

### Acessar o container

```bash
# Entrar no container da API
docker exec -it rentabike-api bash

# Entrar no container do PostgreSQL
docker exec -it rentabike-postgres psql -U admin -d RentABikeDb
```

## 🌐 Acessar a Aplicação

Após iniciar os containers:

- **API**: http://localhost:8080
- **Swagger**: http://localhost:8080/swagger (se habilitado em produção)
- **RabbitMQ Management**: http://localhost:15672 (usuário: admin, senha: admin123)

## 📁 Volumes e Persistência

Os seguintes volumes são criados para persistência de dados:

- `postgres_data`: Dados do PostgreSQL
- `rabbitmq_data`: Dados do RabbitMQ
- `uploads_data`: Arquivos de upload (imagens CNH)

### Backup de volumes

```bash
# Backup do PostgreSQL
docker exec rentabike-postgres pg_dump -U admin RentABikeDb > backup.sql

# Restaurar backup
docker exec -i rentabike-postgres psql -U admin RentABikeDb < backup.sql
```

## 🔒 Segurança em Produção

### Recomendações

1. **Altere as senhas padrão** no `docker-compose.yml`
2. **Use secrets** do Docker Swarm ou Kubernetes Secrets
3. **Configure HTTPS** via reverse proxy (Nginx, Traefik)
4. **Desabilite Swagger** em produção
5. **Configure firewall** adequadamente
6. **Use variáveis de ambiente** para configurações sensíveis

### Exemplo com secrets

```yaml
secrets:
  postgres_password:
    external: true
  rabbitmq_password:
    external: true

services:
  postgres:
    environment:
      POSTGRES_PASSWORD_FILE: /run/secrets/postgres_password
    secrets:
      - postgres_password
```

## 🚢 Deploy em Produção

### Build para produção

```bash
# Build otimizado
docker build --target final -t rentabike-api:prod .

# Tag para registry
docker tag rentabike-api:prod your-registry/rentabike-api:v1.0.0

# Push para registry
docker push your-registry/rentabike-api:v1.0.0
```

### Deploy em servidor

```bash
# Pull da imagem
docker pull your-registry/rentabike-api:v1.0.0

# Executar com configurações de produção
docker run -d \
  --name rentabike-api \
  --restart unless-stopped \
  -p 8080:8080 \
  --env-file .env.production \
  your-registry/rentabike-api:v1.0.0
```

## 🐛 Troubleshooting

### Container não inicia

```bash
# Ver logs de erro
docker logs rentabike-api

# Verificar se as dependências estão rodando
docker-compose ps

# Verificar conectividade
docker exec rentabike-api ping postgres
docker exec rentabike-api ping rabbitmq
```

### Problemas de conexão com banco

```bash
# Verificar se PostgreSQL está acessível
docker exec rentabike-postgres pg_isready -U admin

# Testar conexão
docker exec -it rentabike-postgres psql -U admin -d RentABikeDb
```

### Problemas de conexão com RabbitMQ

```bash
# Verificar status do RabbitMQ
docker exec rentabike-rabbitmq rabbitmq-diagnostics ping

# Verificar usuários
docker exec rentabike-rabbitmq rabbitmqctl list_users
```

## 📝 Notas

- A porta padrão da API é **8080**
- O Swagger está habilitado apenas em **Development** por padrão
- As imagens de upload são armazenadas em `/app/wwwroot/uploads` dentro do container
- Use volumes nomeados para persistência de dados
- Em produção, considere usar um reverse proxy (Nginx, Traefik) para HTTPS

