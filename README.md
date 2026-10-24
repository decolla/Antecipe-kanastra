# Antecipe — Hackathon Kanastra

Portal de antecipação de recebíveis para o **ANTECIPE FIDC**: a empresa envia um lote de notas (CSV),
a API valida cada recebível contra regras de elegibilidade configuráveis e calcula o valor líquido
(valor de face menos o desconto simples).

```
antecipe-kanastra-back/    API ASP.NET Core (.NET 10)
antecipe-kanastra-front/   Front-end
PROMPTS.md                 Registro cronológico dos prompts de IA (exigência do edital, item 4.1.2)
```

## Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) **ou** Docker com Docker Compose

## Subindo o back-end

### Opção 1 — Docker (recomendado)

```bash
cd antecipe-kanastra-back
docker compose up --build
```

API em `http://localhost:8080`.

### Opção 2 — .NET SDK local

```bash
cd antecipe-kanastra-back/antecipe-kanastra-back
dotnet run
```

API em `http://localhost:5080`.

### Verificando

| Endpoint               | Descrição                |
|------------------------|--------------------------|
| `GET /health`          | Health check (`Healthy`) |
| `GET /openapi/v1.json` | Contrato OpenAPI da API  |

## Configuração

| Arquivo | Para quê |
|---|---|
| `antecipe-kanastra-back/antecipe-kanastra-back/Config/validators.json` | Regras de elegibilidade e precificação do fundo. Cada validador tem `codigo`, `ativo` e `parametros`. |
| `antecipe-kanastra-back/antecipe-kanastra-back/appsettings.json` | CORS (`Cors:Origens`), limite de upload (`Upload:TamanhoMaximoMb`, padrão 512). |

Qualquer chave do `appsettings.json` pode ser sobrescrita por variável de ambiente
(ex.: `Upload__TamanhoMaximoMb=1024`). Nenhum segredo é versionado: use variáveis de ambiente ou `.env` (ignorado pelo git).

## Formato do CSV

Separador `;`, UTF-8, com cabeçalho:

```
NOME_FUNDO;DOC_FUNDO;NOME_CEDENTE;DOC_CEDENTE;NOME_SACADO;DOC_SACADO;NU_DOCUMENTO;TIPO_RECEBIVEL;VALOR_NOMINAL;DATA_VENCIMENTO_ORIGINAL;DATA_AQUISICAO
```

Valores com vírgula decimal (`1234,56`) e datas `dd/MM/yyyy`.

## Arquitetura do back-end

```
Domain/          Modelos e contratos puros (Recebivel, Lote, Recusa, IRegra, IPrecificador)
Application/     Casos de uso e abstrações de infraestrutura
Infrastructure/  Leitura de CSV, configuração, persistência (a implementar)
Config/          validators.json (regras do fundo, alterável sem deploy)
```

## Logs

Logs estruturados em JSON no stdout. Por regra, **nenhum dado do CSV é logado** — apenas ids de lote,
contagens e durações.
