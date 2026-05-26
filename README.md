# AlmoxarifadoSG

Sistema de gestão de almoxarifado completo, construído com foco em boas práticas, Clean Architecture e com autenticação utilizando **Firebase**.

## 🚀 Funcionalidades

- **Cadastro e Autenticação de Usuários:** Autenticação segura via Firebase (JWT).
- **Gestão de Estoque:** Controle e acompanhamento de itens de estoque.
- **Solução End-to-End:** Backend robusto em C# com uma aplicação cliente multiplataforma construída em .NET MAUI.
- **Padrões Avançados:** Arquitetura limpa (Clean Architecture) e implementação do padrão CQRS utilizando MediatR.

## 🛠 Tecnologias

- **.NET 9 / C#**
- **ASP.NET Core Web API**
- **.NET MAUI** (Mobile / Multiplataforma)
- **Firebase** (Auth e Realtime Database/Firestore)
- **k6** (Testes de carga/performance)

## 📁 Estrutura do projeto

- `Almoxarifado.API` → Camada de Apresentação (API RESTful)
- `Almoxarifado.Application` → Casos de uso da aplicação, Commands e Queries (CQRS)
- `Almoxarifado.Domain` → Entidades de núcleo e regras de negócio
- `Almoxarifado.Infrastructure` → Acesso a dados e integrações externas (Firebase, Repositórios)
- `Almoxarifado.App` → Aplicativo cliente construído com .NET MAUI (MVVM)

## 💻 Como rodar o projeto

### Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) instalado.
- Conta no Firebase configurada (verifique as credenciais no `appsettings.json` e `firebase.json`).

### 1. Clonar o repositório

```bash
git clone https://github.com/kauanpassos/AlmoxarifadoSG.git
cd AlmoxarifadoSG
dotnet restore
dotnet run --project Almoxarifado.API
```

### 2. Rodar os Testes de Carga (Opcional)

```bash
# Instalar k6 globalmente (se ainda não tiver)
# (Siga as instruções oficiais do k6 para sua plataforma)

k6 run k6-stress-test.js
```

## 🤝 Contribuição

Pull requests são bem-vindos! Sinta-se à vontade para abrir uma issue ou enviar sua contribuição.
