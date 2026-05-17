# 🎓 Guia Rápido: Como tratamos erros na nossa API (Para Devs Juniores)

Se você está chegando agora no projeto e abriu a pasta `Middleware` ou `Exceptions`, pode estar se perguntando: *"Pra que tanta classe de erro diferente se a gente podia só dar um `return BadRequest()` e pronto?"*

Aqui está a explicação de **o que fizemos**, **por que fizemos** e **como a mágica funciona**.

---

## 1. O que foi feito?
Antes, sempre que uma regra de negócio era quebrada (ex: "Sem estoque" ou "Usuário já existe"), a API inteira cuspia um genérico **Erro 400 (Bad Request)**. 

O que nós fizemos foi:
1. Criamos "tipos" diferentes de exceções na pasta `Almoxarifado.Domain/Exceptions` (ex: `ConflictException`, `NotFoundException`, `UnprocessableEntityException`).
2. Fomos no "porteiro" da nossa API (o `ExceptionMiddleware.cs`) e ensinamos ele a olhar para a "cara" do erro.
3. Se for um erro de duplicidade, ele devolve um **Status 409 (Conflict)**. Se for uma regra de negócio complexa violada, devolve um **Status 422 (Unprocessable Entity)**. 

## 2. Por que foi feito? (O problema do "400 para tudo")
Imagine que o Front-end (App Mobile) mande fazer um cadastro e receba um **400 Bad Request**. O que o dev do Mobile deve pensar?
* *"Será que mandei um texto no lugar de um número?"* (Erro de sintaxe)
* *"Será que esqueci a senha?"* (Erro de validação)
* *"Será que esse e-mail já existe no banco?"* (Erro de conflito)

Quando a gente joga tudo no balaio do 400, o Mobile tem que ficar "lendo o texto da mensagem" para adivinhar o que aconteceu. E texto muda! Se a gente corrigir um erro de português na mensagem, o App quebra.

**A Solução Sênior:** Usar os Status Codes oficiais da web.
Quando o Mobile recebe um **409**, ele *sabe* matematicamente que é um conflito de dados. Quando recebe **422**, ele *sabe* que os dados estavam no formato certo, mas a regra de negócio não deixou passar. É muito mais profissional.

## 3. Por que isso funciona sem quebrar o código antigo?
Aqui entra o poder da Orientação a Objetos (Herança) e do C#!

Se você olhar as novas exceções, todas elas herdam da exceção base:
```csharp
public class ConflictException(string message) : DomainException(message);
```
O nosso `ExceptionMiddleware` usa um `switch` inteligente para capturar o erro:
```csharp
var statusCode = exception switch
{
    ConflictException => HttpStatusCode.Conflict, // 409
    UnprocessableEntityException => HttpStatusCode.UnprocessableEntity, // 422
    
    // O pulo do gato:
    DomainException => HttpStatusCode.BadRequest // 400
};
```
**Como o C# lê isso:**
1. Ocorreu um erro! É um `ConflictException`? Sim! Então toma um `409`.
2. Ocorreu outro erro! É um `EstoqueInsuficienteException` (um código antigo do sistema)? O C# olha a lista... Não é Conflict, não é Unprocessable... Opa! O código antigo herda de `DomainException`. Então ele cai na última linha e toma o antigo `400`!

**Resumo:** Nós criamos regras novas de elite, mas mantivemos uma "rede de segurança" para que o código antigo continue funcionando exatamente como antes, sem quebrar nada no Front-end!
