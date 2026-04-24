Here's the improved `README.md` file, incorporating the new content while maintaining the existing structure and information:

# GoodHamburger

## Objetivo do Projeto

GoodHamburger é uma aplicação para gerenciamento de pedidos e cardápio de uma hamburgueria. O projeto permite que clientes visualizem o cardápio disponível, realizem pedidos personalizados com seleção de sanduíches e acompanhamentos, editem pedidos existentes e visualizem a lista de seus pedidos com cálculo automático de descontos promocionais.

## Arquitetura de Software

O projeto implementa a arquitetura **Clean Architecture** com separação clara de responsabilidades em camadas:

### Estrutura de Camadas

- **GoodHamburger.Blazor** - Interface do usuário
  - Componentes Razor com `@page` para roteamento
  - Componentes `.razor.cs` com lógica de apresentação
  - Serviços para comunicação com a API
  - Validações de entrada de dados

- **GoodHamburger.API** - Camada de controle
  - Controllers REST para exposição dos endpoints
  - Roteamento e orquestração de requisições

- **GoodHamburger.Application** - Casos de uso
  - `UseCases` contendo a lógica de negócio
  - Mapeamento de DTOs (Data Transfer Objects)
  - Orquestração entre repositórios

- **GoodHamburger.Domain** - Entidades e regras de negócio
  - Modelos de domínio (`Pedido`, `ItemPedido`)
  - Exceções customizadas
  - Validações de negócio

- **GoodHamburger.Infrastructure** - Acesso a dados
  - Repositórios para persistência
  - Implementação de interfaces de porta

- **GoodHamburger.Tests** - Testes automatizados
  - Testes unitários dos casos de uso

### Padrões Utilizados

- **Dependency Injection**: Injeção de dependências via `.NET DI Container`
- **Repository Pattern**: Abstração de acesso a dados
- **SOLID Principles**: Aplicação de princípios de design sólido
- **Hexagonal Architecture**: Isolamento de domínio através de portas (interfaces)

## Tecnologias Aplicadas

### Backend
- **.NET 8** - Framework de aplicação
- **ASP.NET Core** - Framework web
- **Dapper** - ORM para acesso a dados
- **SQL Server** - Banco de dados relacional

### Frontend
- **Blazor Server** - Framework para interfaces interativas em C#
- **Razor Components** - Componentes reutilizáveis
- **HTML5 & CSS3** - Marcação e estilos
- **JavaScript Interop** - Interoperabilidade JavaScript quando necessário

### Desenvolvimento
- **C# 12** - Linguagem de programação
- **xUnit** - Framework de testes unitários
- **Moq** - Framework de testes para simulação de dados fakes
- **Visual Studio 2022** - IDE de desenvolvimento
- **SQL Server 2016 ou superior** - Servidor de Banco de dados 

## Instruções de Como Rodar o Projeto

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download) instalado
- [SQL Server](https://www.microsoft.com/sql-server/) configurado
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)
- Git instalado

### Passos para Execução

1. **Clone o repositório**
   git clone https://github.com/marciliojcg/GoodHamburger.git
   cd GoodHamburger

2. **Configure a string de conexão**
- Abra o arquivo `GoodHamburger.API\appsettings.Development.json`
- Atualize a string de conexão do banco de dados:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=GoodHamburger;Trusted_Connection=true;"
  }
  ```

3. **Configure o endereço base da API no Blazor**
- Abra o arquivo `GoodHamburger.Blazor\appsettings.Development.json`
- Verifique se o `ApiBaseAddress` aponta para a porta correta:
  ```json
  "ApiBaseAddress": "https://localhost:7089"
  ```

4. **Restaure os pacotes NuGet**
   dotnet restore

5. **Aplique as migrações do banco de dados**
   rode o script database.sql na sessão do banco de dados SQL Server

6. **Execute a API (em um terminal)**
   cd GoodHamburger.API
   dotnet run

7. **Execute o Blazor (em outro terminal)**
   cd GoodHamburger.Blazor
   dotnet run

8. **Acesse a aplicação**
- Abra o navegador e navegue para `https://localhost:7001` (ou a porta exibida no terminal)

### Executar Testes

dotnet test

## Observações de Restrições

### Funcionalidades Limitadas
- A aplicação não possui autenticação/autorização implementada
- Não há persistência de histórico de alterações em pedidos
- O sistema de descontos é baseado em regras fixas e não é configurável

### Limitações de Segurança
- Não há validação CORS configurada (aceita requisições de qualquer origem)
- As credenciais de banco de dados devem ser protegidas em ambiente de produção
- Recomenda-se implementar HTTPS obrigatório em produção

### Requisitos de Ambiente
- Acesso de leitura/escrita ao SQL Server configurado
- Conexão entre as portas 7001 (Blazor) e 7089 (API)
- Certificados SSL/TLS válidos para ambiente de desenvolvimento

### Escalabilidade
- A aplicação não está otimizada para alto volume de requisições
- Cache não está implementado
- Recomenda-se implementar cache distribuído para ambiente de produção

This revised `README.md` maintains the original structure while integrating the new content seamlessly, ensuring clarity and coherence throughout the document.