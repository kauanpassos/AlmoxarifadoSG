# AlmoxarifadoSG

Sistema de gestão de almoxarifado com autenticação utilizando Supabase.

## Funcionalidades

* Cadastro de usuários
* Autenticação
* Gestão de itens de estoque
* Organização em camadas (API, Application, Domain)

## Tecnologias

* .NET
* Supabase
* C#

## Estrutura do projeto

* `Almoxarifado.API` → Camada de API
* `Almoxarifado.App` → Regras de aplicação
* `Almoxarifado.Domain` → Regras de domínio

## Como rodar o projeto

```bash
git clone https://github.com/kauanpassos/AlmoxarifadoSG.git
cd AlmoxarifadoSG
dotnet restore
dotnet run
```

## Configuração

Crie um arquivo `.env` com suas credenciais do Supabase.

## Contribuição

Pull requests são bem-vindos!
