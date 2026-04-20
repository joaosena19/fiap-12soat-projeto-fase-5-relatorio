[![Deploy](https://github.com/joaosena19/fiap-12soat-projeto-fase-5-relatorio/actions/workflows/deploy.yaml/badge.svg)](https://github.com/joaosena19/fiap-12soat-projeto-fase-5-relatorio/actions/workflows/deploy.yaml)

# Identificação

Aluno: João Pedro Sena Dainese  
Registro FIAP: RM365182  

Turma 12SOAT - Software Architecture  
Grupo individual  
Grupo 93  

Discord: joaodainese  
Email: joaosenadainese@gmail.com  

## Sobre este Repositório

Este repositório contém apenas parte do projeto completo da Fase 5. Para visualizar a documentação completa, diagramas de arquitetura, e todos os componentes do projeto, acesse: [Documentação Completa - Fase 5](https://github.com/joaosena19/fiap-12soat-projeto-fase-5-documentacao)

## Descrição

Microsserviço de Relatório em .NET, responsável por agregar eventos de toda a pipeline (Upload e Processamento), expor endpoints HTTP para consulta de status e solicitação de relatórios, e gerar relatórios em JSON, Markdown e PDF via Strategy Pattern. Atua como hub de agregação de eventos e única fonte da verdade para o usuário consultar o status de uma análise. Implementado com Clean Architecture, PostgreSQL no Amazon RDS e Entity Framework Core. Executado em Kubernetes (EKS) com HPA.

## Tecnologias Utilizadas

- **.NET** - Runtime e framework web
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados (Amazon RDS)
- **MassTransit + Amazon SNS/SQS** - Mensageria assíncrona
- **Amazon S3** - Armazenamento de relatórios Markdown e PDF
- **QuestPDF** - Geração de documentos PDF
- **JWT Authentication** - Validação de tokens
- **Swagger** - Documentação API
- **Docker** - Containerização
- **Kubernetes** - Orquestração (Amazon EKS)
- **Terraform** - Provisionamento do banco de dados
- **New Relic** - Monitoramento e observabilidade

## Documentação da API

Para consultar a documentação completa dos endpoints da API, acesse: [Documentação de Endpoints - Relatório](https://github.com/joaosena19/fiap-12soat-projeto-fase-5-documentacao/blob/main/03%20-%20Sistemas/03%20-%20Relat%C3%B3rios/02%20-%20Endpoints/1_endpoints_relatorio.md)
