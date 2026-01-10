# 🎮 FCG.Notifications.Worker

WorkerService desenvolvido em .NET 8 para consumir mensagens de uma fila no RabbitMQ e processá-las redirecionando para o Azure Communication Services para envio de e-mails.
- Hospedada na Azure usando Kubernetes Services e imagem docker publicada no ACR (Azure Container Registry).
- [Vídeo com a apresentação da Fase 1](https://youtu.be/bmRaU8VjJZU)
- [Vídeo com a apresentação da Fase 2](https://youtu.be/BXBc6JKnRpw)
- [Vídeo com a apresentação da Fase 3](https://youtu.be/3OxTOgieuMg)
- [Vídeo com a apresentação da Fase 4](https://youtu.be/WwIfjVCoxc8)

## 📌 Objetivo do projeto como um todo

Desenvolver um worker service robusto e escalável, aplicando:

### **Fase 1:**
  - Domain-Driven Design (DDD) 
  - Clean Architecture 
  - Principios SOLID 
  - Middleware para log de requisições e traces 
  - Middleware de tratamento de erros centralizado
  - Exceptions personalizadas
  - Uso do Entity Framework Core com migrations
  - Autenticação baseada em JWT
  - Autorização baseada em permissões
  - Hash seguro de senhas com salt
  - Validações de domínio e DTOs
  - Testes unitários com TDD 
  - Documententação Swagger com Swashbuckle.AspNetCore
### **Fase 2:**
  - **Escalabilidade:**
    - Utilização de Docker para empacotamento da aplicação em container
    - Versionamento de imagems Docker no ACR 
    - Execução da aplicação em containers orquestrados pelo Azure Container Apps garantindo resiliência
  - **Confiabilidade:**
    - Build, testes unitários e push da imagem Docker via CI/CD multi-stage
    - Parametrização de variáveis e secrets no GitHub Environments
    - Testes de carga e performance utilziando K6
  - **Monitoramento:**
    - Traces no New Relic
    - Logs no New Relic
    - Dashboards de monitoramento (New Relic e Azure)
### **Fase 3:** 
  - **Migração arquitetura Monolitica x Micro-serviços:**
    - Separação da API em serviços distintos com base nos contextos delimitados (Users, Games, Orders, Payments)
    - Cada API com seu próprio repositório e infraestrutura (banco de dados, container app e pipeline CI/CD)
  - **Adoção de soluções Serverless:**
    - Arquitetura orientada a eventos com comunicação assíncrona via mensageria (Azure Service Bus)
    - Utilização de Azure Functions como gatilho das mensagens do Service Bus (Tópicos e Subscriptions)
    - Utilização do Azure API Management para gerenciamento e segurança das APIs com políticas de rate limit e cache
  - **Otimização na busca de jogos:**
    - Implementação de ElasticSearch para indexação dos jogos e logs 
    - Ganho de performance com consultas avançadas
    - Implementação de filtros, paginação e ordenação, inclusive endpoint de jogos mais bem avaliados
### **Fase 4:**     
  - **Orquestração de Containers usando Kubernetes:**
    - Migração das aplicações hospedadas em Azure Container Apps (ACA) para Azure Kubernetes Services (AKS)
    - Implementação de HPA (Horizontal Pod AutoScaler) para escalonamento horizontal automatico
    - Implementação de configMap e secrets do Kubernetes para gerenciamento de configurações sensíveis
    - Implementação de Health Probes para garantir a disponibilidade da aplicação
    - Implementação de Deployments e Services para gerenciamento dos pods e exposição das aplicações
    - Implementação de Statefulset e PVC (Persistent Volume Claim) para serviços que necessitam de persistência de dados
  - **Comunicação Assíncrona entre serviços:**
    - Utilização de filas e tópicos no RabbitMQ e ServiceBus para enfilerar requisições e garantir resiliência 
  - **Otimização das imagens Docker**
    - Migração versão da imagem Docker do .NET para uma versão mais leve, otimizando recursos dos containers
    - Aplicações adaptadas para trabalhar com a versão mais leve
    - Redução de aproximadamente 50% do tamanho das imagens
  - **Monitoramento**
    - Elastic.APM instrumentado nas apis e no worker service
    - dashboards com métricas de CPU, memória, requisições, pods...
  

## 🚀 Tecnologias Utilizadas

| Tecnologia            | Versão/Detalhes                                           |
|-|-|
| .NET                  | .NET 8                                                    |
| C#                    | 12                                                        |
| Entity Framework      | Core, com Migrations                                      |
| Banco de Dados        | SQL Server (ou SQLite para testes)                        |
| Autenticação          | JWT (Bearer Token)                                        |
| Testes                | xUnit, Moq, FluentAssertions                              |
| Swagger               | Swashbuckle.AspNetCore                                    |
| Segurança             | PBKDF2 + salt com SHA256                                  |
| Logger                | Middleware de Request/Response + LogId                    |
| Docker                | Multi-stage Dockerfile para build e runtime               |
| Monitoramento         | Elastic.APM + New Relic (.NET Agent) + Azure              |
| Mensageria            | Azure Service Bus (Tópicos e Subscriptions) + RabbitMQ    |
| Consumer de Mensagens | Azure Functions                                           |
| Orquestração          | Azure Kubernetes Services                                 |
| API Gateway           | Azure API Management                                      |
| CI/CD                 | GitHub Actions                                            |
| Testes de Carga       | K6                                                        |
| ElasticSearch         | Indexação e busca avançada                                |


## 🧠 Padrões e Boas Práticas

- Camadas separadas por responsabilidade suguindo principios SOLID
- Interfaces para abstração de serviços externos no domínio
- Injeção de dependência configurada via Program.cs


## ✅ Principais Funcionalidades

### Serviço de Notificações
- ✅ Consumo de mensagens do RabbitMQ
- ✅ Redirecionamento para Azure Communication Services para envio de e-mails
- ✅ Possibildiade de evoluir para outros canais de comunicação (SMS, Push Notifications)
- ✅ Templates customizados
- ✅ Tratamento de falhas e retries

## ⚙️ Pré-requisitos
- .NET 8 SDK instalado


 ## 📁 Estrutura de Pastas

 ```bash
FCG.Notification.Worker/
│
├── Domain/                     # Camada de domínio (regra de negócio, entidades, contratos)
├── Infrastructure/             # Camada de infraestrutura (implementações concretas)
├── Services/                   # Camada de serviços (Envio de email e consumo do RabbitMQ)
├── Kubernetes/                 # Manifests para deploy no AKS
├── Documentation/              # Documentação do projeto
├── .github/                    # Configurações do GitHub Actions para CI/CD
├── .gitignore                  # Arquivo para ignorar arquivos no Git
├── Dockerfile                  # Dockerfile para containerização
├── Program.cs                  # Ponto de entrada da aplicação
├── Worker.cs                   # Serviço de Worker para consumo de mensagens
├── README.md                   # Este arquivo

 ```

## 🚀 Pipeline CI/CD

Os workflows estão definidos em `.github/workflows/`. 
Automatizando os seguintes passos:

- Build da aplicação
- Build da imagem Docker
- Push para Azure Container Registry (ACR)
- MultiStage para Deploy automatizado no AKS:
   - DEV
   - UAT (necessário aprovação)
   - PRD (apenas com PR na branch `master` e necessário aprovação)
   

## ☁️ Infraestrutura na Azure

O projeto utiliza os seguintes recursos na Azure:

- **Azure Resource Group**: `RG_FCG`
- **Azure Container Registry (ACR)**: `acrfcg.azurecr.io`
- **Azure Kubernetes Services (AKS)**: `aks-fcg-notification`
- **RabbitMQ**: `fcg.notification.queue`

As variáveis de ambiente sensíveis (como strings de conexão) são gerenciadas via Azure e GitHub Secrets.
[Link para o desenho de infraestrutura](https://miro.com/app/board/uXjVIteOb6w=/?share_link_id=230805148396)

## 🐳Dockerfile e 📊Monitoramento

Este projeto utiliza um Dockerfile em duas etapas para garantir uma imagem otimizada e segura:

- **Stage 1 - Build**: Usa a imagem oficial do .NET SDK 8.0 para restaurar dependências, compilar e publicar a aplicação em modo Release.
- **Stage 2 - Runtime**: Utiliza a versão alpine (mais leve do ASP.NET 8.0) para executar a aplicação, copiando apenas os artefatos publicados da etapa de build, o que reduz o tamanho final da imagem.

Além disso, o agente do **New Relic** é instalado na imagem de runtime para habilitar monitoramento detalhado da aplicação. As variáveis de ambiente necessárias para a configuração do agente são definidas no Dockerfile, podendo ser sobrescritas via ambiente de execução (ex.: Kubernetes, Azure Container Apps).

Esse processo segue as melhores práticas:

- **Multi-stage build:** mantém a imagem final enxuta e rápida para deploy.
- **Separação clara:** entre build e runtime para evitar expor ferramentas de desenvolvimento.
- **Instalação do agente New Relic:** automatizada e integrada para facilitar o monitoramento.
- **Configuração via variáveis de ambiente:** flexível e segura para licenças e nomes de aplicação.

 ## ✍️ Autor
- Frank Vieira
- GitHub: @fkwesley
- Projeto desenvolvido para fins educacionais no curso da FIAP.
