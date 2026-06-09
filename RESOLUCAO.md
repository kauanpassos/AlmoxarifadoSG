# RESOLUCAO.md

## O que quebrou?
O erro genérico “Não foi possível carregar o dashboard via Firebase” no app MAUI tava escondendo dois problemas sérios e nada a ver um com o outro.

**Primeiro problema:** gargalo de rede na API (Kestrel). O middleware `app.UseHttpsRedirection()` tava forçando o emulador Android (que roda no IP `10.0.2.2`) a tentar acessar `https://localhost`. Resultado: bloqueio na hora (`Connection Refused`). O emulador não conseguia falar com a API.

**Segundo problema:** no motor de banco (`FirebaseEngine<T>`). Quando a gente usava `System.Text.Json` pra reconstruir uma `Solicitacao`, a lista privada `_itens` (e a propriedade pública `IReadOnlyCollection<ItemSolicitacao>`) era simplesmente ignorada. Por quê? Porque não tinha setter público e o construtor não recebia os itens. Isso causava perda de dados silenciosa (solicitações carregavam vazias) ou, pior, erro 500 quando o Firebase devolvia registros antigos sem campos obrigatórios e o `ArgumentException` pipocava.

## O que foi feito pra resolver

### 1. Isolamento do redirecionamento HTTPS (`Program.cs`)
Envolvemos o `app.UseHttpsRedirection()` num `if (!app.Environment.IsDevelopment())`.  
**Por que resolve:** Agora, no ambiente local, o Kestrel aceita requisições HTTP puro. O emulador Android consegue se comunicar na porta 5144 sem ser forçado a um HTTPS que não funciona no celular.

### 2. Construtor mais flexível na `Solicitacao.cs`
A assinatura do construtor era muito restrita: só `(string id, string usuarioId, string observacao)`. Refatoramos pra incluir parâmetros opcionais anuláveis:
- `string? status`
- `DateTime? createdAt`
- `DateTime? updatedAt`
- `IEnumerable<ItemSolicitacao>? itens`

**Por que resolve:** Agora o desserializador JSON consegue jogar todos os dados do banco na hora de criar a entidade. Os itens, que antes eram ignorados, entram direto na lista `_itens`. E datas e status deixam de ser resetados com valores fixos (ex.: `DateTime.UtcNow` toda vez). Usamos `is not null` pra não tomar erro.

## Por que isso é uma solução inteligente e não vai causar regressão?

- **Sem firula:** Não mexemos no `FirebaseEngine<T>` – ele continua genérico. Deixamos a entidade amigável pro JSON nativo, mantendo o modelo rico.
- **Clean Architecture sem violação:** Nenhuma anotação de infra (`[JsonIgnore]`, `[JsonPropertyName]`) entrou no Domínio. A integridade da camada de domínio continua intacta.
- **Tipagem forte:** Sem `object`, sem `dynamic`, sem dicionário maluco. Tudo via `IEnumerable<ItemSolicitacao>` e parâmetros opcionais.

## Quais riscos foram cobertos?

- **Status e data corrompidos:** Antes, cada `Solicitacao` carregada resetava a data de criação porque o construtor ignorava o valor vindo do banco. Agora usamos `createdAt ?? DateTime.UtcNow` – preserva o que veio, só usa `UtcNow` se não tiver.
- **Documento “sujo” do Firebase:** Se um documento vier sem status, a checagem `string.IsNullOrWhiteSpace(status) ? "Pendente" : status` impede estouro 500.

## Checklist do que a gente seguiu (e deu certo)

- [x] **SOLID** – Responsabilidade única: API cuida da resposta, motor serializa via construtor.
- [x] **Clean Architecture** – Domínio não tem dependência de infra (Firebase).
- [x] **Clean Code** – Nenhum comentário besta, funções diretas.
- [x] **Zero `any` / `object`** – Tudo tipado, nada de gambiarra com dicionário.
- [x] **Padrão sênior** – `is not null` no lugar de `!= null`, nullables bem usados, propriedades blindadas.