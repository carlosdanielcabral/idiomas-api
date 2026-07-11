# Design: Login Google OAuth (Fase 2)

**Data:** 2026-07-11
**Status:** Aprovado (brainstorming)
**Escopo:** Implementar login com Google na IdiomasAPI sobre a base separada user/credential da Fase 1.

## Contexto

A Fase 1 separou identidade (`User`) de autenticação (`UserCredential`), com suporte a múltiplos providers via `AuthProvider` enum. A infraestrutura para credenciais externas já está pronta: `ExternalSubject`, `GetByExternalSubject`, `ITransactionManager`.

O fluxo escolhido no brainstorming foi **ID Token verification (mobile-first)**: o app Flutter obtém o ID token via `google_sign_in` e envia à API, que valida com `Google.Apis.Auth` (`GoogleJsonWebSignature.ValidateAsync`).

## Decisões de produto

1. **`email_verified = false` → rejeitar login inteiro.** O Google deve atestar a posse do email antes de aceitarmos.
2. **Validar audience (Client ID).** Rejeitar tokens emitidos para outros apps. Previne token confusion attack.
3. **Account linking preserva o `name` existente.** A credencial Google é apenas uma porta de entrada adicional. Na criação de conta nova, usa o `name` do Google.
4. **Client ID só em variável de ambiente** (`Google__ClientId`). Não versionar no `appsettings.json`.
5. **Endpoint: `POST /auth/google`** com body `{ "idToken": "..." }`.
6. **`ForgotPassword` não envia email para usuários só-Google**, retorna 200 (consistente com usuário inexistente).
7. **`ResetPassword` rejeita com "Token inválido ou expirado"** quando não há credencial local (evita enumeração).
8. **User órfão (credencial existe mas user não) → rejeitar** com `ApiException` (estado inconsistente, não criar segunda conta).
9. **Sem sufixo `Async`** em métodos novos (ex: `Verify` em vez de `VerifyAsync`).
10. **Renomear `MailPasswordLoginResponseDTO` → `LoginResponseDTO`** (reusado pelos dois fluxos de login).

## Arquitetura

### Camada de Infrastructure (adapter para lib externa)

**`GoogleTokenPayload` (value object em `Infrastructure/Service/`):**
```csharp
public record GoogleTokenPayload(
    string Subject,
    string Email,
    string Name,
    bool EmailVerified);
```

**`IGoogleTokenVerifier` (Interface/Service):**
```csharp
public interface IGoogleTokenVerifier
{
    Task<GoogleTokenPayload> Verify(string idToken);
}
```

**`GoogleTokenVerifier` (Infrastructure/Service):**
- Usa `GoogleJsonWebSignature.ValidateAsync(idToken, settings)` com `Audience = [clientId]`.
- Lê `clientId` de `IConfiguration["Google:ClientId"]` (env var `Google__ClientId`).
- Captura exceções da lib e traduz para `ApiException("Token do Google inválido", Unauthorized)`.
- Mapeia o payload da lib para `GoogleTokenPayload` próprio.

### Camada de Application

**`GoogleLoginDTO` (em `Application/DTO/Auth.cs`):**
```csharp
public record GoogleLoginDTO(string IdToken);
```

**Use case `GoogleLogin` (em `Application/UseCase/AuthCase/`):**
```csharp
public class GoogleLogin(
    IGoogleTokenVerifier tokenVerifier,
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    ITransactionManager transactionManager)
{
    public async Task<User> Execute(GoogleLoginDTO dto)
    {
        GoogleTokenPayload payload = await this._tokenVerifier.Verify(dto.IdToken);

        if (!payload.EmailVerified)
        {
            throw new ApiException("Email não verificado pelo Google", HttpStatusCode.Unauthorized);
        }

        UserCredential? credential = await this._userCredentialRepository
            .GetByExternalSubject(AuthProvider.Google, payload.Subject);

        if (credential != null)
        {
            User? user = await this._userRepository.GetById(credential.UserId);

            if (user == null)
            {
                throw new ApiException("Conta não encontrada", HttpStatusCode.Unauthorized);
            }

            return user;
        }

        User? existingUser = await this._userRepository.GetByEmail(payload.Email);

        if (existingUser != null)
        {
            return await this.LinkGoogleCredential(existingUser.Id, payload);
        }

        return await this.CreateNewGoogleUser(payload);
    }

    private async Task<User> LinkGoogleCredential(string userId, GoogleTokenPayload payload)
    {
        await using IDatabaseTransaction transaction = await this._transactionManager.BeginTransactionAsync();

        UserCredential credential = new(
            UUIDGenerator.Generate(),
            userId,
            AuthProvider.Google,
            null,
            payload.Subject
        );

        await this._userCredentialRepository.Insert(credential);

        await transaction.CommitAsync();

        return await this._userRepository.GetById(userId);
    }

    private async Task<User> CreateNewGoogleUser(GoogleTokenPayload payload)
    {
        await using IDatabaseTransaction transaction = await this._transactionManager.BeginTransactionAsync();

        User user = new(UUIDGenerator.Generate(), payload.Name, payload.Email);

        User createdUser = await this._userRepository.Insert(user);

        UserCredential credential = new(
            UUIDGenerator.Generate(),
            createdUser.Id,
            AuthProvider.Google,
            null,
            payload.Subject
        );

        await this._userCredentialRepository.Insert(credential);

        await transaction.CommitAsync();

        return createdUser;
    }
}
```

### Camada de Presentation

**`AuthController.GoogleLogin`:**
- Recebe `GoogleLoginDTO`, executa use case, gera JWT, seta cookie (igual ao `MailPasswordLogin`).

**`AuthRoute`:**
- `POST /auth/google` com `WithValidation<GoogleLoginValidator, GoogleLoginDTO>`.

**`GoogleLoginValidator` (em `Presentation/Http/Validator/Auth/`):**
- Valida que `IdToken` não é vazio.

**Renomear `MailPasswordLoginResponseDTO` → `LoginResponseDTO`:**
- Reusado pelos dois fluxos de login.

### Mudanças em use cases existentes

**`ForgotPassword`:**
- Ganha dependência `IUserCredentialRepository`.
- Após encontrar o user, checa se existe credencial local.
- Se não existe, retorna sem enviar email (mesmo comportamento de user inexistente).

**`ResetPassword`:**
- Troca mensagem de "Usuário não possui credencial local" para "Token inválido ou expirado" quando não há credencial local.

### Dependência

- NuGet `Google.Apis.Auth` — usada apenas em `Infrastructure/Service/GoogleTokenVerifier`.

### Configuração

- Variável de ambiente `Google__ClientId` (lida via `IConfiguration["Google:ClientId"]`).
- Registro no DI: `services.AddScoped<IGoogleTokenVerifier, GoogleTokenVerifier>()`.

## Plano de implementação

1. Adicionar NuGet `Google.Apis.Auth` ao projeto `Idiomas.Core`.
2. Criar `GoogleTokenPayload` (Infrastructure/Service) + `IGoogleTokenVerifier` (Interface/Service) + `GoogleTokenVerifier` (Infrastructure/Service) + test.
3. Criar `GoogleLoginDTO` (Application/DTO).
4. Criar `GoogleLogin` use case + test.
5. Criar `GoogleLoginValidator` (Presentation/Validator).
6. Renomear `MailPasswordLoginResponseDTO` → `LoginResponseDTO` + atualizar referências.
7. Adicionar `GoogleLogin` no `AuthController` + `IAuthController` + `AuthRoute`.
8. Atualizar `ForgotPassword` para checar credencial local + atualizar test.
9. Atualizar `ResetPassword` mensagem de erro + atualizar test.
10. Registrar `IGoogleTokenVerifier` no DI.
11. Verificação final (build + testes).
