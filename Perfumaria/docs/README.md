# docs/

Pasta reservada para os artefatos de entrega (evidências do CP3 e do CP4).

## CP3

- `swagger.png` (ou similar) — print/export da Swagger UI (`/swagger`) mostrando os
  endpoints documentados com seus `ProducesResponseType`.
- `problem-details-exemplo.json` — trecho de uma resposta `ProblemDetails` real,
  capturada de um teste de erro (ex.: `GET /api/produtos/{id-inexistente}` → 404),
  com qualquer dado sensível anonimizado.
- `mer.png` / `mer.svg` — diagrama MER do domínio (herdado do CP1, com os nomes de
  campos atualizados para o domínio de perfumaria).

## CP4

- `health-healthy.json` — trecho da resposta de `GET /health` com a API e o banco
  no ar (status 200, todos os checks `Healthy`).
- `health-unhealthy.json` (ou print) — trecho da resposta de `GET /health` com o
  banco indisponível (status 503), obtido apontando `ConnectionStrings:DefaultConnection`
  para um arquivo/caminho inválido em ambiente **local** (nunca commitar credenciais
  reais) ou parando o SGBD.
- `log-traceid.png` (ou trecho de texto) — log de console de um `POST /api/pedidos`
  mostrando as mensagens de início/sucesso com `TraceId` correlacionado.
- `dotnet-test.png` (ou saída de texto) — resultado de `dotnet test` com todos os
  testes passando (`Perfumaria.Domain.Tests`, `Perfumaria.Application.Tests` e
  `Perfumaria.Infrastructure.Tests`).

Estes arquivos não foram gerados automaticamente nesta adaptação e devem ser
adicionados pelo grupo antes da entrega final (não é necessário para rodar o projeto).
