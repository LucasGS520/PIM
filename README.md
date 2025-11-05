# Sistema Integrado de Gestão de Chamados

Este repositório contém uma API desenvolvida em ASP.NET Core para gerenciamento inteligente de chamados de suporte técnico. O backend está preparado para integração com clientes web, mobile ou desktop e utiliza MS SQL Server como banco de dados principal.

## Recursos Principais

- **Gestão de Usuários** com perfis (Cliente, Técnico, Gestor) e controle de permissões.
- **Abertura de Chamados** com classificação automática de categoria e prioridade por meio de um módulo de IA heurístico.
- **Sugestões de Conhecimento** para autoatendimento e apoio aos técnicos.
- **Atualização de Chamados** com histórico completo, anexos e notificações automáticas.
- **Feedback de Atendimento** e cálculo de indicadores gerenciais (KPIs).
- **Relatórios em Tempo Real** com métricas de produtividade e satisfação.

## Estrutura do Projeto

- `src/SupportSystem.Api`: camada de apresentação com controladores REST e configuração da aplicação.
- `src/SupportSystem.Application`: camada de aplicação com serviços, DTOs e interfaces de negócio.
- `src/SupportSystem.Domain`: entidades e enumerações do domínio de suporte.
- `src/SupportSystem.Infrastructure`: persistência com Entity Framework Core, implementação do módulo de IA e injeção de dependências.

## Configuração

1. Ajuste a string de conexão em `appsettings.json` com as credenciais do servidor MS SQL Server.
2. Execute as migrações do Entity Framework (não incluídas neste repositório) para criar o esquema do banco de dados.
3. Inicie a API com `dotnet run --project src/SupportSystem.Api/SupportSystem.Api.csproj` e acesse a documentação interativa pelo Swagger em `https://localhost:5001/swagger`.

## Considerações

- O módulo de IA utiliza regras heurísticas simples para priorização e categorização, permitindo substituição futura por modelos treinados.
- Comentários e docstrings foram escritos em português para facilitar a manutenção pela equipe local.
