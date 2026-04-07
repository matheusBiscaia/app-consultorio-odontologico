# App Consultorio Odontologico

> Aplicativo de consultorio odontologico em desenvolvimento com `.NET MAUI`, `C#` e `XAML`, focado em agendamento de consultas, organizacao da agenda e fluxo entre cliente e consultorio.

Projeto criado como estudo pratico e portfólio, com foco em interface, regras de negocio e evolucao para um sistema mais proximo de um caso real de uso.

## Estrutura do Projeto

```text
AplicativoConsultorio/
├── AplicativoConsultorio.sln
├── README.md
├── .gitignore
└── ClinicaOdontologicaApp/
    ├── Models/
    ├── Services/
    ├── ViewModels/
    ├── Views/
    │   ├── Cliente/
    │   └── Consultorio/
    ├── Platforms/
    ├── Resources/
    ├── App.xaml
    ├── AppShell.xaml
    ├── MauiProgram.cs
    └── ClinicaOdontologicaApp.csproj
```

## Visao Geral

O aplicativo foi pensado para simular a rotina de um consultorio odontologico, permitindo:

- agendamento de consultas pelo cliente
- escolha da profissional desejada
- controle de horarios disponiveis por profissional
- acompanhamento das consultas pelo consultorio
- confirmacao e cancelamento de atendimentos
- notificacoes internas sobre alteracoes da agenda

## Funcionalidades Principais

### Cliente

- login inicial
- visualizacao da area do cliente
- selecao da profissional antes do agendamento
- calendario customizado com navegacao por mes, ano e dia
- selecao de horario disponivel
- visualizacao de consultas marcadas
- cancelamento apenas de consultas pendentes

### Consultorio

- painel com consultas pendentes
- confirmacao de consultas
- cancelamento de consultas
- visualizacao de notificacoes de novas marcacoes e alteracoes

## Regras de Negocio Ja Implementadas

- uma consulta confirmada nao pode mais ser alterada pelo cliente
- consultas confirmadas ou canceladas saem da lista principal do consultorio
- o cliente escolhe a profissional antes de marcar
- o mesmo horario pode ser usado por profissionais diferentes
- o mesmo horario nao pode ser reservado duas vezes para a mesma profissional
- horarios ocupados aparecem como indisponiveis no fluxo de agendamento

## Tecnologias Utilizadas

- `.NET MAUI`
- `C#`
- `XAML`
- `Microsoft.Extensions.DependencyInjection`


## Status Atual

Projeto em andamento.

Atualmente o app ja possui uma base funcional para:

- login
- agendamento de consulta
- calendario interativo
- selecao de profissional
- painel do consultorio
- regras de bloqueio de horario

Proximas etapas planejadas:

- persistencia real de dados
- banco de dados
- autenticacao real
- backend/API
- refinamento visual
- evolucao para um produto mais completo

## Objetivo no Portfolio

Este repositório representa minha evolucao na construcao de um aplicativo de agenda para consultorio, desde a interface inicial ate a implementacao de regras de negocio mais proximas de um sistema real.

## Observacoes

- o projeto ainda nao utiliza banco de dados definitivo
- os dados atuais nao devem ser tratados como base real de pacientes
- este repositório tem fins de estudo, evolução tecnica e portfólio
