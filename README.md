# XmlDataProcessor

Sistema Full Stack desenvolvido para simular um cenário real de integração e processamento de dados através de arquivos XML.

A aplicação recebe arquivos contendo movimentações de entrada e saída, registra a importação, processa os dados, valida os movimentos, identifica duplicidades, persiste as informações no SQL Server e disponibiliza o acompanhamento através de uma interface web desenvolvida em Angular.

O projeto foi construído com foco em arquitetura em camadas, separação de responsabilidades, SOLID, TDD, idempotência, tratamento de erros e integração entre frontend, API e banco de dados.

---

## 🎯 Objetivo

Simular um cenário próximo de aplicações corporativas que recebem arquivos periodicamente de sistemas externos.

O fluxo principal da aplicação é:

```text
Arquivo XML
    ↓
Angular
    ↓
ASP.NET Core Web API
    ↓
Application
    ↓
Domain
    ↓
Leitura e validação do XML
    ↓
Verificação de duplicidade
    ↓
Dapper
    ↓
SQL Server
    ↓
Resultado do processamento
    ↓
Interface Angular
```

Cada importação mantém informações como:

- arquivo recebido;
- data de recebimento;
- status do processamento;
- quantidade total de registros;
- registros processados com sucesso;
- registros com erro;
- registros duplicados.

---

# 🏗️ Arquitetura

A solução backend foi organizada em projetos com responsabilidades distintas:

```text
XmlDataProcessor
│
├── XmlDataProcessor.Api
│   ├── Contracts
│   ├── Controllers
│   └── ExceptionHandling
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
├── xml-data-processor-web
│
├── Arquivos
│
└── Script.sql
```

A separação permite que regras de negócio permaneçam independentes de detalhes como SQL Server, Dapper, HTTP ou leitura física dos arquivos XML.

---

## 🧠 Domain

O projeto `XmlDataProcessor.Domain` concentra as entidades e regras de negócio.

Principais entidades:

- `Importacao`
- `Movimento`

Principais enums:

- `StatusImportacao`
- `TipoMovimento`

O domínio controla o ciclo de vida de uma importação e impede transições inválidas de estado.

### Status de uma importação

```text
Recebida
   ↓
Processando
   ↓
├── Concluída
├── Concluída com erros
└── Falhou
```

A entidade também controla os contadores relacionados ao processamento:

```text
TotalRegistros
TotalSucessos
TotalErros
TotalDuplicados
```

Dessa forma, regras importantes não ficam espalhadas entre Controller, banco de dados e frontend.

---

# ⚙️ Application

A camada Application contém os casos de uso e realiza a orquestração entre domínio e infraestrutura.

Entre os fluxos implementados estão:

- criação de uma importação;
- validação do arquivo recebido;
- início do processamento;
- leitura dos movimentos;
- identificação de duplicidades;
- persistência dos movimentos;
- contabilização de sucessos;
- contabilização de erros;
- contabilização de duplicados;
- atualização do estado da importação;
- consulta das importações.

A Application depende de abstrações como:

```text
IImportacaoRepository
IMovimentoRepository
ILeitorMovimentosXml
```

Isso mantém a camada desacoplada das implementações técnicas.

---

# 🔌 Infrastructure

A camada Infrastructure contém as implementações relacionadas aos recursos externos.

Atualmente é responsável por:

- conexão com SQL Server;
- execução de SQL através do Dapper;
- persistência das importações;
- persistência dos movimentos;
- consultas ao banco;
- verificação de duplicidades;
- leitura dos arquivos XML;
- parsing utilizando `XDocument`;
- conversão e validação dos dados XML.

A conexão com o banco é centralizada através de:

```text
ISqlConnectionFactory
        ↓
SqlConnectionFactory
        ↓
SqlConnection
        ↓
SQL Server
```

Os repositories recebem a factory através de Dependency Injection.

---

# 📄 Processamento XML

Um arquivo XML pode possuir múltiplos movimentos.

Exemplo:

```xml
<Movimentos>
  <Movimento>
    <IdExterno>MOV-001</IdExterno>
    <Tipo>Entrada</Tipo>
    <Valor>150.50</Valor>
    <DataMovimento>2026-08-24</DataMovimento>
    <Documento>DOC-001</Documento>
  </Movimento>

  <Movimento>
    <IdExterno>MOV-002</IdExterno>
    <Tipo>Saida</Tipo>
    <Valor>50.00</Valor>
    <DataMovimento>2026-08-24</DataMovimento>
    <Documento>DOC-002</Documento>
  </Movimento>
</Movimentos>
```

O processamento ocorre aproximadamente assim:

```text
XML
 ↓
LeitorMovimentosXml
 ↓
XDocument
 ↓
Validação
 ↓
Movimento
 ↓
ProcessarImportacaoService
 ↓
Verificação de duplicidade
 ↓
Repository
 ↓
SQL Server
```

São tratados como obrigatórios:

- `IdExterno`
- `Tipo`
- `Valor`
- `DataMovimento`

O campo:

```text
Documento
```

é opcional.

Também são tratados valores inválidos de:

- tipo de movimento;
- valor;
- data;
- estrutura do XML;
- campos obrigatórios ausentes.

---

# 🔁 Idempotência e duplicidades

Cada movimento possui um:

```text
IdExterno
```

Esse identificador representa o registro originado pelo sistema externo.

Antes de persistir um movimento, a aplicação verifica se o `IdExterno` já existe.

Além disso, o SQL Server possui uma restrição de unicidade.

```sql
UNIQUE (IdExterno)
```

Portanto existem duas camadas de proteção:

```text
Application
     ↓
verifica IdExterno
     ↓
SQL Server
     ↓
UNIQUE(IdExterno)
```

Isso ajuda a garantir idempotência e evita que o mesmo movimento seja processado repetidamente.

Quando um registro já existe, ele é contabilizado como duplicado em vez de ser inserido novamente.

---

# 🗄️ Banco de dados

Banco utilizado:

```text
SQL Server
Database: XmlDataProcessor
```

Principais tabelas:

```text
dbo.Importacoes
dbo.Movimentos
```

O script utilizado para criação da estrutura está disponível no arquivo:

```text
Script.sql
```

## Importacoes

Armazena cada arquivo recebido e o resultado de seu processamento.

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

## Movimentos

Armazena os movimentos válidos processados.

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

# 💾 Persistência

A persistência utiliza:

- Dapper;
- Microsoft.Data.SqlClient;
- SQL Server;
- queries SQL parametrizadas;
- Repository Pattern;
- Connection Factory.

Entre as operações implementadas estão:

```text
Adicionar importação
Obter importação por ID
Listar importações
Atualizar importação
Persistir movimentos
Consultar movimentos
Verificar duplicidades
```

---

# 🌐 REST API

A aplicação disponibiliza endpoints REST para gerenciamento das importações.

Base local:

```text
http://localhost:5287
```

## Criar importação

```http
POST /api/Importacoes
```

Exemplo:

```json
{
  "nomeArquivo": "movimentos-2026-08-25.xml",
  "dataRecebimento": "2026-08-26T17:30:00"
}
```

Resposta:

```http
201 Created
```

```json
{
  "id": 1
}
```

A API também informa o recurso criado através do header `Location`.

---

## Listar importações

```http
GET /api/Importacoes
```

Retorna as importações registradas com seus respectivos indicadores de processamento.

---

## Consultar importação

```http
GET /api/Importacoes/{id}
```

Exemplo:

```http
GET /api/Importacoes/42
```

Esse endpoint também é utilizado pelo frontend para carregar o painel de detalhes da importação.

---

## Processar importação

```http
POST /api/Importacoes/{id}/processar
```

Exemplo:

```http
POST /api/Importacoes/42/processar
```

O processamento:

```text
Importação
    ↓
Validação do estado
    ↓
Arquivo XML
    ↓
Parsing
    ↓
Movimentos
    ↓
Validação
    ↓
Duplicidade
    ↓
Persistência
    ↓
Atualização dos contadores
    ↓
Status final
```

---

# 🚨 Tratamento global de exceções

A API possui tratamento centralizado de exceções utilizando:

```text
IExceptionHandler
```

O `GlobalExceptionHandler` converte exceções da aplicação em respostas HTTP adequadas.

Exemplos:

```text
ArgumentException
→ 400 Bad Request

InvalidDataException
→ 400 Bad Request

XmlException
→ 400 Bad Request

FileNotFoundException
→ 404 Not Found

InvalidOperationException
→ 409 Conflict

Erro inesperado
→ 500 Internal Server Error
```

As respostas utilizam `ProblemDetails`.

Exemplo:

```json
{
  "title": "Operação inválida",
  "status": 409,
  "detail": "A importação só pode iniciar o processamento quando estiver com status Recebida."
}
```

---

# 🛡️ Validação de arquivos

A aplicação aceita arquivos XML para processamento.

A extensão `.xml` é validada em duas camadas:

```text
Angular
   ↓
validação de formulário
   ↓
ASP.NET Core
   ↓
validação da regra
```

Isso significa que o frontend melhora a experiência do usuário, enquanto o backend continua protegendo a regra mesmo quando a API é chamada diretamente.

Exemplos:

```text
movimentos.xml       → válido
MOVIMENTOS.XML       → válido
arquivo.txt          → inválido
arquivo              → inválido
```

---

# 🖥️ Frontend

O projeto possui uma interface web desenvolvida em Angular.

Diretório:

```text
xml-data-processor-web
```

Principais tecnologias:

- Angular;
- TypeScript;
- Reactive Forms;
- HttpClient;
- Signals;
- Tailwind CSS.

A interface consome diretamente a ASP.NET Core Web API.

```text
Angular
   ↓ HttpClient
ASP.NET Core
   ↓
Application
   ↓
Infrastructure
   ↓
SQL Server
```

---

# 📊 Dashboard de importações

A tela principal possui indicadores para acompanhamento do processamento.

Atualmente são exibidos:

```text
Total de importações
Recebidas
Concluídas
Com erro / falha
```

A tabela apresenta:

- ID;
- arquivo;
- data;
- status;
- quantidade de registros;
- sucessos;
- erros;
- duplicados;
- ações.

Os status são apresentados visualmente através de badges.

---

# 🔍 Busca e filtros

A listagem permite pesquisar importações por:

```text
ID
Nome do arquivo
```

Também é possível filtrar por:

```text
Todos
Recebida
Processando
Concluída
Concluída com erros
Falhou
```

Busca e status podem ser utilizados simultaneamente.

A interface também informa a quantidade de resultados encontrados.

Exemplo:

```text
Exibindo 14 de 42 importações
```

Quando existe algum filtro aplicado, é disponibilizada a ação:

```text
Limpar filtros
```

---

# 🔎 Detalhes da importação

Cada registro possui a ação:

```text
Detalhes
```

Ao clicar, o frontend executa:

```http
GET /api/Importacoes/{id}
```

e apresenta um painel contendo:

- ID da importação;
- arquivo;
- data de recebimento;
- status;
- total de registros;
- sucessos;
- erros;
- duplicados.

Isso mantém a listagem separada da consulta individual do recurso.

---

# ▶️ Processamento pelo frontend

Importações com status:

```text
Recebida
```

podem ser processadas diretamente pela interface.

O Angular executa:

```http
POST /api/Importacoes/{id}/processar
```

Durante a operação a interface apresenta o estado:

```text
Processando...
```

Após a conclusão, a listagem é atualizada com o resultado retornado pelo backend.

---

# 📱 Responsividade

A interface foi preparada para diferentes tamanhos de tela.

Entre os comportamentos implementados estão:

- cards responsivos;
- formulário adaptável;
- painel de detalhes responsivo;
- tabela com largura mínima;
- scroll horizontal em telas menores;
- coluna de ações fixa durante o scroll horizontal.

Isso permite utilizar a aplicação mesmo quando o espaço horizontal disponível é reduzido.

---

# 🔄 CORS

Durante o desenvolvimento, frontend e backend executam em portas diferentes:

```text
Angular
http://localhost:4200

ASP.NET Core
http://localhost:5287
```

A API possui configuração de CORS permitindo a comunicação entre os dois ambientes durante o desenvolvimento.

---

# 💉 Dependency Injection

As dependências da aplicação são configuradas no ASP.NET Core.

Exemplo simplificado:

```text
Controller
    ↓
Application Service
    ↓
Repository Interface
    ↓
Repository
    ↓
Connection Factory
    ↓
SQL Server
```

Esse modelo permite substituir implementações sem acoplar as regras de negócio aos detalhes técnicos.

---

# 🧪 Testes automatizados

O projeto utiliza:

```text
xUnit
```

e foi desenvolvido de maneira incremental utilizando conceitos de TDD.

Fluxo utilizado:

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

Atualmente:

```text
77 testes automatizados
```

Todos executando com sucesso.

Os testes cobrem principalmente:

- regras do Domain;
- ciclo de vida da importação;
- validações;
- casos de uso;
- processamento dos movimentos;
- contabilização de registros;
- duplicidades;
- tratamento de erros;
- leitura XML;
- múltiplos movimentos;
- campos obrigatórios;
- valores inválidos;
- validação da extensão `.xml`.

Para executar:

```bash
dotnet test
```

---

# 🛠️ Tecnologias

## Backend

- C#
- .NET 10
- ASP.NET Core Web API
- Dapper
- Microsoft.Data.SqlClient
- SQL Server
- LINQ
- LINQ to XML
- XDocument
- Dependency Injection
- ProblemDetails
- xUnit

## Frontend

- Angular
- TypeScript
- Reactive Forms
- HttpClient
- Signals
- Tailwind CSS

## Desenvolvimento

- Git
- GitHub
- Visual Studio
- Visual Studio Code
- SQL Server Management Studio
- PowerShell

---

# ▶️ Executando o projeto

## Banco de dados

Execute o script:

```text
Script.sql
```

em uma instância do SQL Server.

Configure a connection string da API de acordo com seu ambiente.

---

## Backend

Na raiz da solução:

```bash
dotnet restore
```

Depois:

```bash
dotnet build
```

Execute os testes:

```bash
dotnet test
```

Inicie a API:

```bash
dotnet run --project XmlDataProcessor.Api
```

Durante o desenvolvimento, a API está configurada para executar localmente em:

```text
http://localhost:5287
```

---

## Frontend

Entre no diretório:

```bash
cd xml-data-processor-web
```

Instale as dependências:

```bash
npm install
```

Inicie o Angular:

```bash
ng serve
```

ou:

```bash
npm start
```

A aplicação ficará disponível em:

```text
http://localhost:4200
```

---

# 🧭 Fluxo completo da aplicação

```text
Usuário
   ↓
Angular
   ↓
Nova importação
   ↓
POST /api/Importacoes
   ↓
SQL Server
   ↓
Importação = Recebida

Usuário
   ↓
Processar
   ↓
POST /api/Importacoes/{id}/processar
   ↓
Application
   ↓
LeitorMovimentosXml
   ↓
Arquivo XML
   ↓
Validação
   ↓
Movimentos
   ↓
Verificação de duplicidade
   ↓
Persistência
   ↓
Atualização da Importação
   ↓
SQL Server
   ↓
Angular
   ↓
Dashboard / Listagem / Detalhes
```

---

# 📚 Conceitos praticados

O projeto também foi desenvolvido como exercício prático de arquitetura e desenvolvimento Full Stack utilizando .NET e Angular.

Entre os conceitos aplicados estão:

- arquitetura em camadas;
- Separation of Concerns;
- SOLID;
- Dependency Inversion;
- Dependency Injection;
- Repository Pattern;
- Factory Pattern;
- TDD;
- programação assíncrona;
- APIs REST;
- códigos HTTP;
- ProblemDetails;
- tratamento global de exceções;
- SQL parametrizado;
- persistência relacional;
- idempotência;
- processamento XML;
- integração entre sistemas;
- integração frontend/backend;
- Reactive Forms;
- Signals;
- consumo de APIs com HttpClient;
- responsividade;
- filtros e pesquisa de dados.

---

# 📌 Status atual

## ✅ Concluído

- [x] Estrutura da solução backend
- [x] Separação Domain / Application / Infrastructure / API
- [x] Entidade `Importacao`
- [x] Entidade `Movimento`
- [x] Ciclo de vida da importação
- [x] Casos de uso
- [x] Repository Pattern
- [x] Connection Factory
- [x] Leitor XML
- [x] Parsing de múltiplos movimentos
- [x] Validação dos elementos XML
- [x] Tratamento de valores inválidos
- [x] Controle de duplicidades
- [x] Idempotência
- [x] SQL Server
- [x] Dapper
- [x] Persistência das importações
- [x] Persistência dos movimentos
- [x] Consulta por ID
- [x] Listagem das importações
- [x] Atualização das importações
- [x] Processamento completo XML → SQL Server
- [x] Dependency Injection
- [x] REST API
- [x] Endpoint de criação
- [x] Endpoint de listagem
- [x] Endpoint de consulta individual
- [x] Endpoint de processamento
- [x] Códigos HTTP adequados
- [x] Global Exception Handler
- [x] ProblemDetails
- [x] CORS
- [x] OpenAPI
- [x] Validação de extensão `.xml`
- [x] Frontend Angular
- [x] Reactive Forms
- [x] Integração Angular → ASP.NET Core
- [x] Dashboard
- [x] Criação de importações pela interface
- [x] Processamento pela interface
- [x] Consulta de detalhes pela API
- [x] Busca por ID/nome
- [x] Filtro por status
- [x] Contador dos resultados filtrados
- [x] Interface responsiva
- [x] 77 testes automatizados

---

# 🚀 Próximas etapas

O projeto já possui o fluxo Full Stack principal funcionando.

As próximas evoluções planejadas são:

- [ ] Paginação das importações no backend
- [ ] Paginação utilizando SQL Server com `OFFSET` / `FETCH`
- [ ] Integração da paginação com Angular
- [ ] Evoluir filtros para consulta server-side
- [ ] Melhorar estados de loading
- [ ] Adicionar testes de integração da API
- [ ] Evoluir documentação OpenAPI
- [ ] Avaliar processamento em streaming para XMLs grandes
- [ ] Melhorar observabilidade e logging
- [ ] Revisar UX e acabamento final da interface
- [ ] Finalizar documentação do projeto

---

# 👨‍💻 Autor

**Wagner Vale**

Projeto desenvolvido para meu aperfeiçoamento de desenvolvimento Full Stack, arquitetura de software, processamento de dados, ASP.NET Core, SQL Server e Angular.
