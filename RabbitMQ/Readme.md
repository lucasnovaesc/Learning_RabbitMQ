# ?? Sistema de Relat�rios com RabbitMQ e PostgreSQL

## ?? Vis�o Geral

Este projeto demonstra uma implementa��o completa de **Event-Driven Architecture** utilizando .NET 8, RabbitMQ, PostgreSQL e MassTransit. O sistema processa solicita��es de relat�rios de forma ass�ncrona, ilustrando conceitos fundamentais de mensageria e arquitetura orientada a eventos.

## ??? Arquitetura do Sistema

### Diagrama de Arquitetura
```mermaid
graph TB
    subgraph "Cliente"
        A[Aplica��o Cliente]
    end
    
    subgraph "API Layer"
        B[ASP.NET Core API]
        C[Controllers/Endpoints]
    end
    
    subgraph "Business Layer"
        D[RelatorioService]
        E[IPublishBus]
    end
    
    subgraph "Message Broker"
        F[RabbitMQ]
        G[Exchange Fanout]
        H[Queue]
    end
    
    subgraph "Data Layer"
        I[RelatorioRepository]
        J[Entity Framework]
        K[PostgreSQL]
    end
    
    subgraph "Background Processing"
        L[Event Consumer]
    end
    
    A --> B
    B --> C
    C --> D
    D --> E
    D --> I
    E --> F
    F --> G
    G --> H
    H --> L
    L --> I
    I --> J
    J --> K
```

### Componentes Principais

| Componente | Responsabilidade | Tecnologia |
|------------|------------------|------------|
| **API REST** | Interface HTTP para solicita��es | ASP.NET Core |
| **Service Layer** | L�gica de neg�cio e orquestra��o | C# Services |
| **Message Broker** | Comunica��o ass�ncrona | RabbitMQ + MassTransit |
| **Repository** | Abstra��o de acesso a dados | Entity Framework Core |
| **Database** | Persist�ncia de dados | PostgreSQL |
| **Background Worker** | Processamento ass�ncrono | Event Consumer |

## ?? Fluxo Detalhado de Dados

### ?? Fluxo de Solicita��o (POST)

```mermaid
sequenceDiagram
    participant C as Cliente
    participant API as API Endpoint
    participant S as RelatorioService
    participant DB as PostgreSQL
    participant PUB as PublishBus
    participant RMQ as RabbitMQ
    participant CONS as Consumer
    
    C->>API: POST /api/relatorios/solicitar/{nome}
    API->>S: SolicitarRelatorioAsync(nome)
    S->>DB: Salvar nova solicita��o (status: "Pendente")
    DB-->>S: Retorna entidade criada
    S->>PUB: PublishAsync(RelatorioSolicitadoEvent)
    PUB->>RMQ: Enviar mensagem para fila
    S-->>API: Retorna SolicitacaoRelatorio
    API-->>C: HTTP 201 Created + dados da solicita��o
    
    Note over RMQ: Mensagem aguarda processamento
    
    RMQ->>CONS: Entrega mensagem
    CONS->>S: ProcessarRelatorioAsync(id, nome)
    S->>DB: Atualizar status para "Completado"
    DB-->>S: Confirma��o
    S-->>CONS: Processamento conclu�do
```

### ?? Fluxo de Consulta (GET)

```mermaid
sequenceDiagram
    participant C as Cliente
    participant API as API Endpoint
    participant S as RelatorioService
    participant R as Repository
    participant DB as PostgreSQL
    
    C->>API: GET /api/relatorios
    API->>S: ObterTodosRelatoriosAsync()
    S->>R: ObterTodosAsync()
    R->>DB: SELECT * FROM solicitacoes_relatorio
    DB-->>R: Resultados ordenados
    R-->>S: Lista de relat�rios
    S-->>API: Lista completa
    API-->>C: HTTP 200 OK + dados
```

## ??? Tecnologias e Frameworks

### Stack Principal
- **.NET 8**: Framework base
- **ASP.NET Core**: Web API
- **Entity Framework Core**: ORM
- **PostgreSQL**: Banco de dados relacional
- **RabbitMQ**: Message broker
- **MassTransit**: Abstra��o para RabbitMQ
- **Docker**: Containeriza��o dos servi�os

### Pacotes NuGet
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
<PackageReference Include="MassTransit" Version="8.0.0" />
<PackageReference Include="MassTransit.RabbitMQ" Version="8.0.0" />
```

## ?? Estrutura Detalhada do Projeto

```
RabbitMQ/
??? ?? Bus/                                    # Camada de Mensageria
?   ??? IPublishBus.cs                        # Interface para publica��o
?   ??? PublishBus.cs                         # Implementa��o do publisher
?   ??? RelatorioSolicitadoEventConsumer.cs   # Consumer de eventos
??? ?? Controllers/                           # Camada de Apresenta��o
?   ??? ApiEndpoints.cs                       # Endpoints REST
?   ??? ?? Relatorios/
?       ??? RelatorioSolicitadoEvent.cs       # Evento de dom�nio
??? ?? Data/                                  # Camada de Dados
?   ??? ApplicationDbContext.cs               # Contexto EF Core
?   ??? ?? Entities/
?   ?   ??? SolicitacaoRelatorio.cs          # Entidade de banco
?   ??? ?? Repositories/
?       ??? RelatorioRepository.cs            # Repository pattern
??? ?? Services/                              # Camada de Neg�cio
?   ??? RelatorioService.cs                   # L�gica de neg�cio
??? ?? Extension/                             # Configura��es
?   ??? AppExtensions.cs                      # DI e setup
??? ?? Properties/
?   ??? launchSettings.json                   # Config desenvolvimento
??? Program.cs                                # Ponto de entrada
??? appsettings.json                          # Configura��es
??? docker-compose.yml                        # Orquestra��o containers
```

## ?? Configura��o e Execu��o

### Pr�-requisitos
- **.NET 8 SDK**
- **Docker** e **Docker Compose**
- **Visual Studio** ou **VS Code**

### 1. Clonar e Preparar
```bash
git clone <repository-url>
cd RabbitMQ
dotnet restore
```

### 2. Subir Infraestrutura
```bash
# Subir PostgreSQL e RabbitMQ
docker-compose up -d

# Verificar se servi�os est�o rodando
docker-compose ps
```

### 3. Configurar Banco de Dados
```bash
# Criar migration inicial
dotnet ef migrations add InitialCreate

# Aplicar migration
dotnet ef database update
```

### 4. Executar Aplica��o
```bash
dotnet run
```

### 5. Acessar Interfaces
- **API**: https://localhost:7042
- **Swagger**: https://localhost:7042/swagger
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **PostgreSQL**: localhost:5432 (postgres/postgres123)

## ?? API Reference

### ?? POST `/api/relatorios/solicitar/{nome}`
Cria uma nova solicita��o de relat�rio.

**Request:**
```bash
curl -X POST "https://localhost:7042/api/relatorios/solicitar/vendas-2024" \
     -H "Content-Type: application/json"
```

**Response (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "nome": "vendas-2024",
  "status": "Pendente",
  "dataCriacao": "2025-08-19T14:30:00Z",
  "dataProcessamento": null,
  "observacoes": null
}
```

### ?? GET `/api/relatorios`
Lista todas as solicita��es de relat�rio.

**Request:**
```bash
curl -X GET "https://localhost:7042/api/relatorios"
```

**Response (200 OK):**
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "nome": "vendas-2024",
    "status": "Completado",
    "dataCriacao": "2025-08-19T14:30:00Z",
    "dataProcessamento": "2025-08-19T14:30:05Z",
    "observacoes": "Processamento conclu�do com sucesso"
  }
]
```

### ?? GET `/api/relatorios/{id}`
Obt�m relat�rio espec�fico por ID.

**Request:**
```bash
curl -X GET "https://localhost:7042/api/relatorios/550e8400-e29b-41d4-a716-446655440000"
```

## ?? Event-Driven Architecture

### Conceitos Implementados

#### 1. **Publisher-Subscriber Pattern**
- **Publisher**: API endpoints que publicam eventos
- **Subscriber**: Consumers que processam eventos
- **Desacoplamento**: Componentes n�o se conhecem diretamente

#### 2. **Event Sourcing**
- Eventos representam mudan�as de estado
- Hist�rico completo de a��es no sistema
- Reprocessamento poss�vel atrav�s dos eventos

#### 3. **CQRS (Command Query Responsibility Segregation)**
- **Commands**: POST (alteram estado)
- **Queries**: GET (apenas leitura)
- Otimiza��o independente de leitura/escrita

#### 4. **Saga Pattern (Preparado)**
- Transa��es distribu�das atrav�s de eventos
- Compensa��o autom�tica em caso de falhas
- Coordena��o de processos longos

## ?? An�lise: Benef�cios vs Malef�cios

### ? **Benef�cios da Programa��o Orientada a Eventos**

#### **Desacoplamento**
- **Baixo acoplamento**: Componentes independentes
- **Alta coes�o**: Cada m�dulo tem responsabilidade espec�fica
- **Flexibilidade**: F�cil adi��o de novos consumers
- **Manutenibilidade**: Mudan�as isoladas n�o afetam outros componentes

#### **Escalabilidade**
- **Horizontal**: M�ltiplos consumers processando em paralelo
- **Vertical**: Aumento de recursos por componente
- **Load Balancing**: Distribui��o autom�tica de carga
- **Elasticidade**: Adi��o/remo��o din�mica de workers

#### **Resili�ncia**
- **Fault Tolerance**: Falha em um componente n�o derruba o sistema
- **Retry Autom�tico**: MassTransit reprocessa mensagens falhas
- **Dead Letter Queue**: Mensagens problem�ticas isoladas
- **Circuit Breaker**: Prote��o contra cascata de falhas

#### **Performance**
- **Processamento Ass�ncrono**: Resposta imediata ao usu�rio
- **Throughput Alto**: Processamento em paralelo
- **Otimiza��o Independente**: Cada componente pode ser otimizado separadamente
- **Cache Eficiente**: Dados podem ser cacheados por camada

#### **Observabilidade**
- **Rastreamento**: Cada evento pode ser logado
- **M�tricas**: Throughput, lat�ncia, erros por componente
- **Debugging**: F�cil identifica��o de problemas
- **Auditoria**: Hist�rico completo de eventos

### ? **Malef�cios e Desafios**

#### **Complexidade**
- **Curva de Aprendizado**: Conceitos avan�ados de arquitetura
- **Debugging Complexo**: Fluxo distribu�do entre componentes
- **Configura��o**: M�ltiplos servi�os para configurar
- **Overhead de Desenvolvimento**: Mais c�digo boilerplate

#### **Consist�ncia Eventual**
- **Eventual Consistency**: Dados podem estar temporariamente inconsistentes
- **Race Conditions**: Ordem de processamento n�o garantida
- **Sincroniza��o**: Dificuldade em manter estado consistente
- **Transa��es Distribu�das**: Complexidade adicional

#### **Infraestrutura**
- **Depend�ncias Externas**: RabbitMQ, PostgreSQL
- **Monitoramento**: Necess�rio monitorar m�ltiplos componentes
- **Deployment**: Orquestra��o de m�ltiplos servi�os
- **Costs**: Infraestrutura adicional

#### **Lat�ncia de Rede**
- **Network Hops**: M�ltiplas chamadas de rede
- **Serializa��o**: Overhead de JSON
- **Message Broker**: Lat�ncia adicional
- **Database Round-trips**: M�ltiplas queries

## ?? RabbitMQ - An�lise Detalhada

### ? **Vantagens do RabbitMQ**

#### **Confiabilidade**
- **Durabilidade**: Mensagens persistem em disco
- **Acknowledgments**: Confirma��o de processamento
- **Clustering**: Alta disponibilidade
- **Backup/Recovery**: Recupera��o de dados

#### **Flexibilidade**
- **M�ltiplos Patterns**: pub/sub, request/reply, routing
- **Exchange Types**: Direct, Topic, Fanout, Headers
- **Binding Din�mico**: Roteamento configur�vel
- **Plugins**: Extensibilidade atrav�s de plugins

#### **Performance**
- **Throughput Alto**: Milhares de mensagens/segundo
- **Low Latency**: Lat�ncia sub-milissegundo
- **Memory Management**: Gerenciamento eficiente de mem�ria
- **Flow Control**: Controle autom�tico de fluxo

#### **Ecossistema**
- **Multi-Language**: Suporte a v�rias linguagens
- **Management UI**: Interface gr�fica completa
- **Monitoring**: M�tricas detalhadas
- **Community**: Grande comunidade e documenta��o

### ? **Desvantagens do RabbitMQ**

#### **Complexidade Operacional**
- **Configura��o**: M�ltiplas op��es podem confundir
- **Tuning**: Otimiza��o requer conhecimento espec�fico
- **Troubleshooting**: Debugging pode ser complexo
- **Learning Curve**: Conceitos AMQP n�o triviais

#### **Limita��es T�cnicas**
- **Single Point of Failure**: Sem clustering adequado
- **Memory Usage**: Pode consumir muita mem�ria
- **Disk I/O**: Performance dependente do disco
- **Network Partitions**: Problemas em falhas de rede

#### **Overhead**
- **Infraestrutura**: Servidor adicional para manter
- **Lat�ncia**: Hop adicional na comunica��o
- **Serializa��o**: Overhead de convers�o JSON
- **Monitoring**: Necess�rio monitorar separadamente

## ?? Mensageria - Benef�cios vs Desafios

### ? **Benef�cios da Mensageria**

#### **Integra��o**
```mermaid
graph LR
    A[Servi�o A] --> M[Message Broker]
    B[Servi�o B] --> M
    C[Servi�o C] --> M
    M --> D[Servi�o D]
    M --> E[Servi�o E]
    M --> F[Servi�o F]
```

- **Loose Coupling**: Servi�os independentes
- **Protocol Independence**: Diferentes protocolos
- **Language Agnostic**: Diferentes linguagens
- **Legacy Integration**: Integra��o com sistemas antigos

#### **Escalabilidade Horizontal**
- **Load Distribution**: Carga distribu�da automaticamente
- **Consumer Scaling**: Adicionar consumers conforme demanda
- **Geographic Distribution**: Consumers em diferentes regi�es
- **Resource Optimization**: Recursos alocados dinamicamente

#### **Reliability Patterns**
- **At-Least-Once Delivery**: Garantia de entrega
- **Retry Mechanisms**: Reprocessamento autom�tico
- **Dead Letter Queues**: Isolamento de mensagens problem�ticas
- **Circuit Breakers**: Prote��o contra falhas

### ? **Desafios da Mensageria**

#### **Consist�ncia de Dados**
- **Eventual Consistency**: Estado eventualmente consistente
- **Duplicate Messages**: Possibilidade de mensagens duplicadas
- **Out-of-Order Processing**: Ordem n�o garantida
- **Lost Messages**: Risco de perda em falhas

#### **Complexidade de Debug**
- **Distributed Tracing**: Rastreamento atrav�s de m�ltiplos servi�os
- **Log Correlation**: Correla��o de logs distribu�dos
- **Error Handling**: Tratamento de erros complexo
- **Testing**: Testes de integra��o complexos

#### **Operational Overhead**
- **Infrastructure**: M�ltiplos componentes para manter
- **Monitoring**: Monitoramento de cada componente
- **Deployment**: Orquestra��o complexa
- **Security**: Seguran�a em m�ltiplas camadas

## ?? Patterns de Design Implementados

### 1. **Repository Pattern**
```csharp
// Abstra��o da persist�ncia
public interface IRelatorioRepository
{
    Task<SolicitacaoRelatorio> CriarAsync(SolicitacaoRelatorio solicitacao);
    // ... outros m�todos
}
```
**Benef�cios**: Testabilidade, mudan�a de ORM, abstra��o

### 2. **Dependency Injection**
```csharp
// Invers�o de controle
services.AddScoped<IRelatorioService, RelatorioService>();
```
**Benef�cios**: Testabilidade, flexibilidade, desacoplamento

### 3. **Command Query Separation**
- **Commands**: POST endpoints (alteram estado)
- **Queries**: GET endpoints (apenas leitura)

### 4. **Event-Driven Communication**
```csharp
// Publica��o de evento
await _publishBus.PublishAsync(new RelatorioSolicitadoEvent(id, nome));
```

### 5. **Async/Await Pattern**
```csharp
// Opera��es n�o-bloqueantes
public async Task<SolicitacaoRelatorio> CriarAsync(SolicitacaoRelatorio solicitacao)
```

## ?? Schema do Banco de Dados

### Tabela: `solicitacoes_relatorio`

| Coluna | Tipo | Constraints | Descri��o |
|--------|------|-------------|-----------|
| `id` | UUID | PRIMARY KEY, DEFAULT gen_random_uuid() | Identificador �nico |
| `nome` | VARCHAR(200) | NOT NULL | Nome do relat�rio |
| `status` | VARCHAR(50) | NOT NULL | Status atual |
| `data_criacao` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Data de cria��o |
| `data_processamento` | TIMESTAMP | NULL | Data de conclus�o |
| `observacoes` | VARCHAR(500) | NULL | Observa��es adicionais |

### �ndices Criados
```sql
-- Performance em consultas por status
CREATE INDEX ix_solicitacoes_relatorio_status ON solicitacoes_relatorio(status);

-- Performance em consultas por data
CREATE INDEX ix_solicitacoes_relatorio_data_criacao ON solicitacoes_relatorio(data_criacao);
```

## ?? Monitoramento e Observabilidade

### Logs Estruturados
```csharp
_logger.LogInformation(
    "Relat�rio solicitado: {Id} - {Nome}", 
    solicitacaoId, 
    nomeRelatorio
);
```

### M�tricas Importantes
- **Throughput**: Mensagens/segundo processadas
- **Lat�ncia**: Tempo entre solicita��o e conclus�o
- **Error Rate**: Taxa de erros por componente
- **Queue Depth**: Tamanho das filas RabbitMQ

### Dashboards Recomendados
- **RabbitMQ Management**: Filas, exchanges, connections
- **Application Logs**: Structured logging com Serilog
- **Database Metrics**: Connection pool, query performance
- **Custom Metrics**: Business metrics espec�ficos

## ?? Configura��es Avan�adas

### Connection String com Pool
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=relatorios_db;Username=postgres;Password=postgres123;Pooling=true;MinPoolSize=5;MaxPoolSize=100;ConnectionLifeTime=300"
  }
}
```

### RabbitMQ com Retry Policy
```csharp
busConfigurator.UsingRabbitMq((ctx, cfg) =>
{
    cfg.Host(new Uri("amqp://localhost:5672"), host =>
    {
        host.Username("guest");
        host.Password("guest");
    });
    
    cfg.UseMessageRetry(retry => retry.Exponential(
        retryLimit: 3,
        minInterval: TimeSpan.FromSeconds(1),
        maxInterval: TimeSpan.FromMinutes(5),
        intervalDelta: TimeSpan.FromSeconds(2)
    ));
    
    cfg.ConfigureEndpoints(ctx);
});
```

## ?? Testando o Sistema

### Teste de Carga
```bash
# Criar m�ltiplas solicita��es
for i in {1..10}; do
  curl -X POST "https://localhost:7042/api/relatorios/solicitar/teste-$i"
done

# Verificar processamento
curl -X GET "https://localhost:7042/api/relatorios"
```

### Valida��o no Banco
```sql
-- Conectar ao PostgreSQL
docker exec -it rabbitmq_postgres_1 psql -U postgres -d relatorios_db

-- Verificar dados
SELECT nome, status, data_criacao, data_processamento 
FROM solicitacoes_relatorio 
ORDER BY data_criacao DESC;

-- Estat�sticas por status
SELECT status, COUNT(*), 
       AVG(EXTRACT(EPOCH FROM (data_processamento - data_criacao))) as avg_processing_time_seconds
FROM solicitacoes_relatorio 
WHERE data_processamento IS NOT NULL
GROUP BY status;
```

## ?? Roadmap de Melhorias

### Curto Prazo
- [ ] **Retry Policies**: Implementar pol�ticas de retry
- [ ] **Dead Letter Queue**: Isolar mensagens com erro
- [ ] **Health Checks**: Monitoramento de sa�de dos servi�os
- [ ] **Validation**: FluentValidation para requests
- [ ] **Exception Handling**: Global exception handler

### M�dio Prazo
- [ ] **Authentication**: JWT + Identity
- [ ] **Authorization**: Pol�ticas de acesso
- [ ] **Rate Limiting**: Controle de taxa de requisi��es
- [ ] **Caching**: Redis para cache distribu�do
- [ ] **Background Jobs**: Hangfire para jobs programados

### Longo Prazo
- [ ] **Microservices**: Separar em m�ltiplos servi�os
- [ ] **Event Store**: Event sourcing completo
- [ ] **CQRS Read Models**: Views otimizadas
- [ ] **Distributed Tracing**: OpenTelemetry
- [ ] **Message Versioning**: Versionamento de eventos

## ?? Melhores Pr�ticas Implementadas

### **Domain-Driven Design**
- Eventos representam conceitos de dom�nio
- Linguagem ub�qua nos nomes
- Bounded contexts bem definidos

### **Clean Architecture**
- Separa��o de responsabilidades
- Dependency inversion
- Infrastructure isolada

### **Twelve-Factor App**
- Configuration via environment
- Stateless processes
- Port binding configur�vel

### **Observability**
- Structured logging
- Correlation IDs
- Health checks preparados

## ?? Considera��es de Seguran�a

### Implementadas
- **Input Validation**: Valida��o de entrada
- **SQL Injection Protection**: Entity Framework parametrizado
- **HTTPS**: Comunica��o criptografada

### A Implementar
- **Authentication**: JWT tokens
- **Authorization**: Role-based access
- **Message Encryption**: Criptografia de mensagens
- **Audit Trail**: Log de todas as a��es

## ?? Recursos de Aprendizado

### Documenta��o Oficial
- [MassTransit Documentation](https://masstransit-project.com/)
- [RabbitMQ Tutorials](https://www.rabbitmq.com/tutorials/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

### Conceitos Avan�ados
- **Event Sourcing**: Martin Fowler
- **CQRS**: Greg Young
- **Microservices Patterns**: Chris Richardson
- **Building Event-Driven Microservices**: Adam Bellemare

## ?? Contribuindo

1. Fork o projeto
2. Crie feature branch (`git checkout -b feature/nova-funcionalidade`)
3. Commit mudan�as (`git commit -m 'Adiciona nova funcionalidade'`)
4. Push para branch (`git push origin feature/nova-funcionalidade`)
5. Abra Pull Request

### Padr�es de Commit
```
feat: adiciona nova funcionalidade
fix: corrige bug espec�fico
docs: atualiza documenta��o
style: formata��o de c�digo
refactor: refatora��o sem mudan�a de comportamento
test: adiciona ou atualiza testes
chore: tarefas de manuten��o
```

## ?? Troubleshooting

### Problemas Comuns

#### RabbitMQ n�o conecta
```bash
# Verificar se est� rodando
docker ps | grep rabbitmq

# Logs do container
docker logs rabbitmq_rabbitmq_1
```

#### PostgreSQL connection failed
```bash
# Verificar conex�o
docker exec -it rabbitmq_postgres_1 pg_isready

# Logs do PostgreSQL
docker logs rabbitmq_postgres_1
```

#### Migrations falham
```bash
# Reset migrations
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### Mensagens n�o s�o processadas
1. Verificar se consumer est� registrado
2. Confirmar conex�o RabbitMQ
3. Checar logs do consumer
4. Verificar Management UI (http://localhost:15672)

### Logs �teis
```bash
# Logs da aplica��o
dotnet run --verbosity detailed

# Logs espec�ficos EF Core
export ASPNETCORE_ENVIRONMENT=Development
```

## ?? Licen�a

Este projeto � desenvolvido para fins educacionais e demonstra��o de conceitos de arquitetura de software.

---

**?? Este projeto demonstra uma implementa��o robusta e profissional de Event-Driven Architecture, servindo como base s�lida para sistemas distribu�dos modernos.**