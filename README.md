# XmlDataProcessor

API desenvolvida em **ASP.NET Core / .NET 10** para processamento de arquivos XML contendo movimentações de entrada e saída.

O projeto está sendo construído com foco em **arquitetura em camadas, princípios SOLID, TDD, processamento de dados, idempotência e persistência com SQL Server**.

> 🚧 Projeto em desenvolvimento.

---

## 🎯 Objetivo

Simular um cenário real de integração entre sistemas no qual arquivos XML contendo movimentações são recebidos e processados periodicamente.

O fluxo esperado da aplicação é:

```text
Arquivo XML
    ↓
API
    ↓
Application
    ↓
Domain
    ↓
Leitura e validação dos movimentos
    ↓
Verificação de duplicidade
    ↓
Persistência
    ↓
SQL Server
```

A aplicação deverá registrar cada importação, processar os movimentos recebidos e manter informações como:

- quantidade total de registros;
- registros processados com sucesso;
- registros duplicados;
- registros com erro;
- status da importação.

---

## 🏗️ Arquitetura

A solução está dividida em projetos com responsabilidades distintas:

```text
XmlDataProcessor
│
├── XmlDataProcessor.Api
│   ├── Contracts
│   └── Controllers
│
├── XmlDataProcessor.Application
│   ├── Abstractions
│   │   ├── Repositories
│   │   └── Xml
│   └── UseCases
│
├── XmlDataProcessor.Domain
│   ├── Entities
│   └── Enums
│
├── XmlDataProcessor.Infrastructure
│   ├── Persistence
│   │   ├── Connection
│   │   └── Repositories
│   └── Xml
│
├── XmlDataProcessor.Tests
│
└── ScriptsDB
    └── Script.sql
```

### Domain

Contém as entidades e regras de negócio da aplicação.

Principais entidades:

- `Importacao`
- `Movimento`

Principais enums:

- `StatusImportacao`
- `TipoMovimento`

O domínio controla o ciclo de vida da importação, incluindo processamento, conclusão, falha e contabilização dos registros.

---

### Application

Responsável pelos casos de uso e pela orquestração das regras de negócio.

Casos de uso atualmente implementados:

- início de uma importação;
- processamento de uma importação;
- leitura dos movimentos;
- controle de sucessos;
- controle de erros;
- controle de duplicidades;
- atualização do estado da importação.

A Application depende de abstrações para acessar recursos externos, evitando dependência direta de banco de dados ou parser XML.

---

### Infrastructure

Responsável pelas implementações técnicas utilizadas pela aplicação.

Atualmente contém:

- leitura de arquivos XML;
- validação estrutural dos elementos do XML;
- conversão de valores;
- `SqlConnectionFactory`;
- acesso ao SQL Server;
- persistência utilizando Dapper.

---

## 📄 Processamento XML

O leitor XML transforma registros como:

```xml
<Movimentos>
  <Movimento>
    <IdExterno>MOV-001</IdExterno>
    <Tipo>Entrada</Tipo>
    <Valor>150.50</Valor>
    <DataMovimento>2026-08-24</DataMovimento>
    <Documento>DOC-001</Documento>
  </Movimento>
</Movimentos>
```

em entidades do domínio:

```text
XML
 ↓
LeitorMovimentosXml
 ↓
Movimento
```

Atualmente são tratados como obrigatórios:

- `IdExterno`
- `Tipo`
- `Valor`
- `DataMovimento`

O campo `Documento` é opcional.

Também são tratados valores inválidos para:

- `Tipo`
- `Valor`
- `DataMovimento`

Os erros de entrada são convertidos para exceções mais claras no contexto do processamento.

---

## 🔁 Idempotência

Cada movimento possui um `IdExterno`, utilizado para identificar registros originados pelo sistema externo.

A aplicação verifica a existência do identificador antes da persistência.

Além disso, o banco possui uma restrição:

```sql
UNIQUE (IdExterno)
```

Isso adiciona uma segunda camada de proteção contra registros duplicados.

```text
Application
    ↓
verifica IdExterno

Banco de Dados
    ↓
UNIQUE(IdExterno)
```

---

## 🗄️ Banco de dados

Banco utilizado:

```text
SQL Server
Database: XmlDataProcessor
```

Tabelas atualmente criadas:

```text
dbo.Importacoes
dbo.Movimentos
```

O script de criação está disponível em:

```text
ScriptsDB/Script.sql
```

### Importacoes

Armazena informações sobre cada arquivo recebido e seu processamento.

Principais campos:

```text
Id
NomeArquivo
DataRecebimento
Status
TotalRegistros
TotalSucessos
TotalErros
TotalDuplicados
```

### Movimentos

Armazena os movimentos processados.

Principais campos:

```text
Id
IdExterno
Tipo
Valor
DataMovimento
Documento
```

---

## 🔌 Persistência

A persistência está sendo implementada utilizando:

- **Dapper**
- **Microsoft.Data.SqlClient**
- **SQL Server**

A criação das conexões foi centralizada através de:

```text
ISqlConnectionFactory
        ↓
SqlConnectionFactory
        ↓
SqlConnection
```

Os repositories recebem a factory através de Dependency Injection.

---

## 🌐 API

O primeiro endpoint funcional já está disponível:

### Criar importação

```http
POST /api/importacoes
```

Exemplo de requisição:

```json
{
  "nomeArquivo": "movimentos-2026-08-24.xml",
  "dataRecebimento": "2026-08-24T20:30:00"
}
```

Fluxo atual:

```text
POST /api/importacoes
        ↓
ImportacoesController
        ↓
IniciarImportacaoService
        ↓
IImportacaoRepository
        ↓
ImportacaoRepository
        ↓
Dapper
        ↓
SQL Server
```

Esse fluxo já foi validado com persistência real na tabela:

```text
dbo.Importacoes
```

---

## 💉 Dependency Injection

As dependências estão sendo configuradas no `Program.cs`.

Exemplo do fluxo de resolução:

```text
IniciarImportacaoService
        ↓
IImportacaoRepository
        ↓
ImportacaoRepository
        ↓
ISqlConnectionFactory
        ↓
SqlConnectionFactory
```

A aplicação utiliza diferentes ciclos de vida de acordo com a responsabilidade de cada componente.

---

## 🧪 Testes

O projeto está sendo desenvolvido utilizando **xUnit** e uma abordagem incremental baseada em TDD.

Durante o desenvolvimento são utilizados ciclos:

```text
RED
 ↓
teste falha

GREEN
 ↓
implementação mínima

REFACTOR
 ↓
melhoria mantendo os testes verdes
```

Atualmente o projeto possui **mais de 70 testes automatizados**, cobrindo principalmente:

- regras do Domain;
- ciclo de vida da importação;
- validações;
- casos de uso da Application;
- processamento de movimentos;
- duplicidades;
- tratamento de erros;
- leitura de XML;
- XML com múltiplos movimentos;
- campos obrigatórios;
- valores inválidos.

---

## 🛠️ Tecnologias

- C#
- .NET 10
- ASP.NET Core Web API
- SQL Server
- Dapper
- Microsoft.Data.SqlClient
- LINQ
- LINQ to XML / XDocument
- Dependency Injection
- xUnit
- Git / GitHub

---

## ▶️ Executando a API

Na raiz da solução:

```bash
dotnet run --project XmlDataProcessor.Api
```

Para executar os testes:

```bash
dotnet test
```

Para compilar toda a solução:

```bash
dotnet build
```

---

## 📌 Status atual

### Concluído

- [x] Estrutura inicial da solução
- [x] Separação Domain / Application / Infrastructure / API
- [x] Entidade `Importacao`
- [x] Entidade `Movimento`
- [x] Ciclo de vida da importação
- [x] Casos de uso iniciais
- [x] Contratos de repositories
- [x] Contrato para leitura XML
- [x] Implementação do leitor XML
- [x] Parsing de múltiplos movimentos
- [x] Validação de campos obrigatórios do XML
- [x] Tratamento de valores inválidos
- [x] Controle de duplicidades na Application
- [x] Schema inicial do SQL Server
- [x] `SqlConnectionFactory`
- [x] Configuração da connection string
- [x] Integração com Dapper
- [x] `ImportacaoRepository.AdicionarAsync`
- [x] Dependency Injection inicial
- [x] `ImportacoesController`
- [x] `POST /api/importacoes`
- [x] Primeiro INSERT real validado no SQL Server
- [x] Mais de 70 testes automatizados

### Próximas etapas

- [ ] Implementar `ImportacaoRepository.ObterPorIdAsync`
- [ ] Implementar `ImportacaoRepository.AtualizarAsync`
- [ ] Implementar `MovimentoRepository`
- [ ] Finalizar persistência dos movimentos
- [ ] Integrar processamento XML completo com SQL Server
- [ ] Finalizar registros de Dependency Injection
- [ ] Criar endpoints de processamento e consulta
- [ ] Implementar tratamento global de exceções
- [ ] Evoluir documentação OpenAPI
- [ ] Adicionar testes de integração
- [ ] Avaliar processamento em streaming para arquivos XML grandes
- [ ] Criar interface web em Angular

---

## 📚 Conceitos praticados

Este projeto também está sendo utilizado como aperfeiçoamento sobre meus conhecimentos em .NET e SQL Server

- Clean Architecture
- Separation of Concerns
- Dependency Inversion
- Dependency Injection
- Repository Pattern
- Factory Pattern
- TDD
- SOLID
- programação assíncrona
- SQL parametrizado
- idempotência
- integração entre sistemas
- processamento de XML
- persistência relacional
- tratamento de erros
- APIs REST

---

## 🚧 Em desenvolvimento

O projeto ainda está em evolução.

O próximo marco será concluir a persistência da entidade `Importacao`, implementar a persistência dos `Movimentos` e conectar o processamento completo do XML ao SQL Server.
