# Aplicativo Consultorio

Aplicativo de consultorio odontologico desenvolvido em `.NET MAUI` com `C#`.

O projeto esta em desenvolvimento e faz parte do meu portfólio como estudo de arquitetura mobile, interface e regras de negocio para agendamento de consultas.

## Status

Projeto em andamento.

Atualmente o app ja possui:

- tela de login
- area do cliente
- area do consultorio
- agendamento de consultas
- selecao de profissional para atendimento
- bloqueio de conflito de horario por profissional
- confirmacao e cancelamento de consultas
- calendario customizado para escolha de data

## Tecnologias

- .NET MAUI
- C#
- XAML
- Injeção de dependencia com `Microsoft.Extensions.DependencyInjection`

## Estrutura

O projeto esta organizado em:

- `Models`
- `Services`
- `Views`
- `ViewModels`
- `Platforms`

## Funcionalidades atuais

### Cliente

- escolhe a profissional desejada
- seleciona data no calendario
- escolhe horario disponivel
- visualiza suas consultas
- cancela consultas pendentes

### Consultorio

- visualiza consultas pendentes
- confirma ou cancela consultas
- acompanha notificacoes internas de alteracoes

## Proximos passos

- persistencia real de dados
- autenticacao real de usuarios
- backend/API
- banco de dados
- melhoria de validacoes
- refinamento visual

## Observacoes

- Hoje os dados ainda nao estao persistidos em banco de dados definitivo.
- O projeto ainda esta em fase de prototipo funcional.
- Nenhum dado real de paciente deve ser utilizado neste repositório.


## Portfolio

Este repositório representa minha evolucao no desenvolvimento de um sistema de agendamento para consultorio, desde a interface ate as regras de negocio.
