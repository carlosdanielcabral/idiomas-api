# Design: Separação user/credential (Fase 1 do Google OAuth)

**Data:** 2026-07-11
**Status:** Aprovado (brainstorming)
**Escopo:** Refatoração arquitetural para separar identidade de autenticação, preparando o terreno para login Google (Fase 2, spec separada).

## Contexto e motivação

A API IdiomasAPI (.NET 9, Clean Architecture, Minimal APIs) hoje mistura identidade e autenticação na mesma entidade `User`, que possui `id, name, email, password`. Isso viola a separação de responsabilidades e dificulta suportar múltiplos providers de auth (Google, Apple, Microsoft) no futuro.

A meta é implementar login com Google. A pesquisa apontou que o fluxo adequado para uma API mobile-first consumida por um app Flutter é o **ID Token verification** (o app obtém o ID token via `google_sign_in` e envia à API, que valida com `Google.Apis.Auth`). O fluxo de redirect server-side (`/signin-google`) não se aplica a APIs que não renderizam páginas.

Decidiu-se dividir em duas fases:
- **Fase 1 (este spec):** refatoração para separar `user` (identidade) de `user_credential` (auth), sem mudar comportamento. Migration dos dados existentes.
- **Fase 2 (spec futura):** adicionar login Google sobre a nova base.

## Decisões de produto alinhadas

1. **Account linking:** quando um usuário faz login com Google pela primeira vez e já existe conta com o mesmo email (criada por email/senha), vincula à conta existente. Mitigação de account takeover: checar `email_verified=true` no token do Google (aplicará na Fase 2).
2. **Separação auth/identidade:** auth e identidade são coisas separadas e devem ser refletidas no banco e no código.
3. **Divisão em fases:** refatoração primeiro (Fase 1), Google depois (Fase 2).
4. **Abordagem de repositórios:** `IUserRepository` (identidade) + `IUserCredentialRepository` (auth) separados. EF Core dá atomicidade via `IDbContextTransaction` explícito nos use cases que criam/atualizam user + credencial juntos. Não introduz Unit of Work em todo o projeto.

## Arquitetura

### Camada de Domínio

**`User` (identidade — sem nada de auth):**
```csharp
public class User(string id, string name, string email)
{
    public string Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
}
```
Remove o campo `Password` e o setter público de `Password` (que hoje viola a regra de evitar setters).

**Nova entidade `UserCredential` (auth):**
```csharp
public class UserCredential(
    string id,
    string userId,
    AuthProvider provider,
    string? passwordHash,
    string? externalSubject)
{
    public string Id { get; private set; } = id;
    public string UserId { get; private set; } = userId;
    public AuthProvider Provider { get; private set; } = provider;
    public string? PasswordHash { get; private set; } = passwordHash;
    public string? ExternalSubject { get; private set; } = externalSubject;

    public void UpdatePasswordHash(string passwordHash)
    {
        this.PasswordHash = passwordHash;
    }
}
```
- `passwordHash`: só preenchido para `provider=Local`. Nullable para contas só-Google.
- `externalSubject`: o claim `sub` do Google. Só para providers externos. Nullable para contas locais.
- `UpdatePasswordHash`: mutação controlada e explícita, não setter aberto.

**Novo enum `AuthProvider` em `Domain/Enum`:**
```csharp
public enum AuthProvider { Local, Google }
```
Usar enum (não string mágico) para facilitar extensão futura.

### Camada de Infrastructure (banco de dados)

**`UserModel` (sem `password`):**
```csharp
[Table("user")]
public class UserModel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("name")]
    public required string Name { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("email")]
    public required string Email { get; set; }

    public ICollection<WordModel> Dictionary { get; set; } = [];
    public ICollection<FileModel> Files { get; set; } = [];
}
```

**Nova `UserCredentialModel`:**
```csharp
[Table("user_credential")]
public class UserCredentialModel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    [Required]
    [Column("user_id")]
    public required Guid UserId { get; set; }

    [Required]
    [Column("provider")]
    public required AuthProvider Provider { get; set; }

    [MaxLength(255)]
    [Column("password_hash")]
    public string? PasswordHash { get; set; }

    [MaxLength(255)]
    [Column("external_subject")]
    public string? ExternalSubject { get; set; }

    [Required]
    [Column("created_at")]
    public required DateTime CreatedAt { get; set; }

    public UserModel? User { get; set; }
}
```

**`ApplicationContext` — adiciona `DbSet<UserCredentialModel>` e configurações:**
```csharp
public DbSet<UserCredentialModel> UserCredential { get; set; }

// em OnModelCreating:
modelBuilder.Entity<UserCredentialModel>()
    .HasIndex(credential => new { credential.Provider, credential.ExternalSubject })
    .IsUnique();

modelBuilder.Entity<UserCredentialModel>()
    .HasIndex(credential => new { credential.UserId, credential.Provider })
    .IsUnique();

modelBuilder.Entity<UserCredentialModel>()
    .HasOne(credential => credential.User)
    .WithMany()
    .HasForeignKey(credential => credential.UserId);

modelBuilder.Entity<UserModel>()
    .HasIndex(user => user.Email)
    .IsUnique();
```

- Índice unique em `(provider, external_subject)`: garante que um mesmo `sub` do Google não vincule duas contas.
- Índice unique em `(user_id, provider)`: um usuário não tem duas credenciais do mesmo provider.
- `email` vira unique no banco (hoje só é checado em runtime no `CreateUser` — fecha a race condition).
- Sem navigation collection em `User` para manter identidade pura.

**Mappers:**
- `UserMappingExtension` (Infrastructure): `ToEntity`/`ToModel` sem `password`.
- Novo `UserCredentialMappingExtension` em `Infrastructure/Database/Mapper`: `ToEntity`/`ToModel`.

**Migration — estratégia de segurança (dois passos):**
1. **Step 1:** cria a tabela `user_credential` + popula a partir dos `password` existentes (`INSERT INTO user_credential SELECT NEWID(), id, ''Local'', password, NULL, GETUTCDATE() FROM user WHERE password IS NOT NULL`). Não dropa a coluna `password` ainda. Permite rollback fácil.
2. **Step 2:** drop da coluna `password` da tabela `user` (só após confirmar que `user_credential` está populada corretamente).

A migration gerada pelo EF Core deve ser **revisada manualmente** para incluir o `INSERT` antes do `DROP COLUMN`. Backup do banco antes de aplicar (responsabilidade operacional).

### Camada de Interface (repositórios)

**`IUserRepository` (só identidade):**
```csharp
public interface IUserRepository
{
    public Task<User> Insert(User user);
    public Task<User?> GetByEmail(string email);
    public Task<User?> GetById(string id);
    public Task<User> Update(User user);
}
```

**Nova `IUserCredentialRepository` (auth):**
```csharp
public interface IUserCredentialRepository
{
    public Task<UserCredential> Insert(UserCredential credential);

    public Task<UserCredential?> GetByExternalSubject(AuthProvider provider, string externalSubject);

    public Task<UserCredential?> GetByUserIdAndProvider(string userId, AuthProvider provider);

    public Task<UserCredential> Update(UserCredential credential);
}
```

- `GetByExternalSubject`: usado na Fase 2 pelo `GoogleLogin` (busca direta por `sub`).
- `GetByUserIdAndProvider`: usado pelo `MailPasswordLogin`, `UpdateUser` e `ResetPassword` para buscar credencial local; e na Fase 2 para verificar vinculação.
- `Update`: usado por `UpdateUser` e `ResetPassword` para atualizar o hash.

**Implementações** em `Infrastructure/Database/Repository`:
- `UserRepository`: igual à atual, mas o `Update` não toca mais em `Password` (só `Name`/`Email`).
- Novo `UserCredentialRepository`: consultas via `ApplicationContext.UserCredential`.

### Camada de Application (use cases e mappers)

**`CreateUser` (cria user + credencial local numa transação):**
```csharp
public class CreateUser(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IHash hash,
    ApplicationContext database)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserCredentialRepository _userCredentialRepository = userCredentialRepository;
    private readonly IHash _hash = hash;
    private readonly ApplicationContext _database = database;

    public async Task<User> Execute(CreateUserDTO dto)
    {
        await this.ValidateUser(dto);

        using IDbContextTransaction transaction = await this._database.BeginTransactionAsync();

        User user = await this.CreateUserEntity(dto);

        await this.CreateLocalCredential(dto, user.Id);

        await transaction.CommitAsync();

        return user;
    }

    private async Task<User> CreateUserEntity(CreateUserDTO dto)
    {
        User user = dto.ToEntity();

        return await this._userRepository.Insert(user);
    }

    private async Task CreateLocalCredential(CreateUserDTO dto, string userId)
    {
        string passwordHash = this._hash.Hash(dto.Password);

        UserCredential credential = dto.ToCredentialEntity(userId, passwordHash);

        await this._userCredentialRepository.Insert(credential);
    }

    private async Task ValidateUser(CreateUserDTO dto) { /* igual: checa email existente */ }
}
```

- `Execute` só orquestra: valida -> transação -> cria user -> cria credencial -> commit.
- `CreateUserEntity` e `CreateLocalCredential` são métodos separados (SRP).
- Atomicidade via `IDbContextTransaction` explícito.
- Captura de `DbUpdateException` (email unique violation) traduzida para `ApiException("E-mail já cadastrado", Conflict)`.

**`UpdateUser` (atualiza perfil e, se houver senha, credencial):**
```csharp
public class UpdateUser(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IHash hash,
    ApplicationContext database)
{
    public async Task<User> Execute(string userId, UpdateUserDTO dto)
    {
        await this.ValidateUser(userId, dto);

        using IDbContextTransaction transaction = await this._database.BeginTransactionAsync();

        User updatedUser = await this.UpdateUserProfile(userId, dto);

        if (!string.IsNullOrEmpty(dto.Password))
        {
            await this.UpdateUserPassword(userId, dto.Password);
        }

        await transaction.CommitAsync();

        return updatedUser;
    }

    private async Task<User> UpdateUserProfile(string userId, UpdateUserDTO dto)
    {
        User updatedUser = dto.ToEntity(userId);

        return await this._userRepository.Update(updatedUser);
    }

    private async Task UpdateUserPassword(string userId, string password)
    {
        UserCredential? credential = await this._userCredentialRepository
            .GetByUserIdAndProvider(userId, AuthProvider.Local);

        if (credential is null)
        {
            throw new ApiException("Usuário não possui credencial local", HttpStatusCode.BadRequest);
        }

        string passwordHash = this._hash.Hash(dto.Password);
        credential.UpdatePasswordHash(passwordHash);

        await this._userCredentialRepository.Update(credential);
    }

    private async Task ValidateUser(string userId, UpdateUserDTO dto) { /* igual */ }
}
```

- `Execute` orquestra: valida -> transação -> atualiza perfil -> (se houver senha) atualiza senha -> commit.
- `UpdateUserProfile` cuida só do perfil (name/email).
- `UpdateUserPassword` cuida só da credencial (hash + update).
- Se `dto.Password` for vazio/nulo, não toca na credencial.

**`MailPasswordLogin` (busca credencial local):**
```csharp
public class MailPasswordLogin(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IHash hash)
{
    public async Task<User> Execute(MailPasswordLoginDTO dto)
    {
        User? user = await this._userRepository.GetByEmail(dto.Email);

        UserCredential? credential = await this._userCredentialRepository
            .GetByUserIdAndProvider(user?.Id ?? string.Empty, AuthProvider.Local);

        this.ValidateLogin(user, credential, dto);

        return user!;
    }

    private void ValidateLogin(User? user, UserCredential? credential, MailPasswordLoginDTO dto)
    {
        if (user == null || credential == null)
        {
            throw new ApiException("Email ou senha inválidos", HttpStatusCode.BadRequest);
        }

        bool isPasswordValid = this._hash.Verify(dto.Password, credential.PasswordHash!);

        if (!isPasswordValid)
        {
            throw new ApiException("Email ou senha inválidos", HttpStatusCode.BadRequest);
        }
    }
}
```

- Mensagem de erro idêntica à atual (não revela se o email existe).
- `credential == null` cobre o caso de um usuário só-Google tentar logar por senha (Fase 2).

**`ResetPassword` (atualiza credencial):**
```csharp
public class ResetPassword(
    IPasswordResetTokenRepository tokenRepository,
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IHash hash)
{
    public async Task Execute(ResetPasswordDTO dto)
    {
        PasswordResetToken? token = await this._tokenRepository.GetByToken(dto.Token);

        if (token == null || token.IsExpired || token.IsUsed)
        {
            throw new ApiException("Token inválido ou expirado", HttpStatusCode.BadRequest);
        }

        User? user = await this._userRepository.GetById(token.UserId.ToString());

        if (user == null)
        {
            throw new ApiException("Token inválido ou expirado", HttpStatusCode.BadRequest);
        }

        UserCredential? credential = await this._userCredentialRepository
            .GetByUserIdAndProvider(user.Id, AuthProvider.Local);

        if (credential == null)
        {
            throw new ApiException("Usuário não possui credencial local", HttpStatusCode.BadRequest);
        }

        string passwordHash = this._hash.Hash(dto.NewPassword);
        credential.UpdatePasswordHash(passwordHash);

        await this._userCredentialRepository.Update(credential);

        await this._tokenRepository.MarkAsUsed(token);
    }
}
```

**`ForgotPassword` — sem mudança** (não lida com senha, só com token + email).

**Mappers em `Application/Mapper/UserMappingExtension`:**
```csharp
public static User ToEntity(this CreateUserDTO dto)
{
    return new User(UUIDGenerator.Generate(), dto.Name, dto.Email);
}

public static User ToEntity(this UpdateUserDTO dto, string id)
{
    return new User(id, dto.Name, dto.Email);
}

public static UserCredential ToCredentialEntity(this CreateUserDTO dto, string userId, string passwordHash)
{
    return new UserCredential(
        UUIDGenerator.Generate(),
        userId,
        AuthProvider.Local,
        passwordHash,
        null);
}
```

**DTOs:**
- `CreateUserDTO(string Name, string Email, string Password)` — sem mudança.
- `UpdateUserDTO(string Name, string Email, string? Password)` — `Password` vira nullable.
- `UserDTO(string Id, string Name, string Email)` — sem mudança.

### Camada de Presentation

- `AuthController` — **sem mudança**. `MailPasswordLogin` retorna `User` (sem `Password`), controller gera JWT + cookie como hoje.
- `UserController` — **sem mudança**.
- `AuthRoute` / `UserRoute` — **sem mudança**.
- `UserMappingExtension` (Presentation) — **sem mudança** (`ToResponseDTO` já mapeia só `Id, Name, Email`).
- `UserResponseDTO` / `CreateUserResponseDTO` / `UpdateUserResponseDTO` — **sem mudança**.
- `CreateUserValidator` — **sem mudança** (senha continua obrigatória na criação de conta local).
- `UpdateUserValidator` — `Password` vira condicional (só valida tamanho mínimo se informado):
```csharp
if (!string.IsNullOrEmpty(dto.Password) && dto.Password.Length < MINIMUM_PASSWORD_LENGTH)
{
    throw new ApiException($"Senha deve ter pelo menos {MINIMUM_PASSWORD_LENGTH} caracteres", HttpStatusCode.BadRequest);
}
```

### Dependency Injection

`AddDatabase` ganha o registro do novo repositório:
```csharp
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IUserCredentialRepository, UserCredentialRepository>();  // novo
// ... restante inalterado
```

### Testes

**Testes existentes a ajustar:**

| Teste | Mudança |
|---|---|
| `CreateUserTest` | Construtor com 4 deps (user repo + credential repo + hash + database). `User` com 3 args. Assert de `result.Password` removido; novo assert de `IUserCredentialRepository.Insert` chamado. Mock de `IUserCredentialRepository` e transação. |
| `MailPasswordLoginTest` | Construtor com 3 deps (user repo + credential repo + hash). `User` com 3 args. Setup de `IUserCredentialRepository.GetByUserIdAndProvider` retornando credencial com hash. `IHash.Verify` usa `credential.PasswordHash`. |
| `ResetPasswordTest` | Construtor com 4 deps. Setup de `IUserCredentialRepository.GetByUserIdAndProvider`. Assert de `user.Password` substituído por `credential.UpdatePasswordHash` + `IUserCredentialRepository.Update`. |
| `ForgotPasswordTest` | **Sem mudança**. |

**Novos testes a criar:**
- `UserCredentialTest` (entidade): testa `UpdatePasswordHash` muta o hash corretamente.
- `UserCredentialRepositoryTest` (seguindo o padrão do `ScenarioRepositoryTest`): testa `GetByExternalSubject`, `GetByUserIdAndProvider`, `Insert`, `Update` com banco in-memory.
- `UserMappingExtensionTest` (Application mapper): testa `ToEntity` (sem password) e `ToCredentialEntity`.
- `UserCredentialMappingExtensionTest` (Infrastructure mapper): testa `ToEntity`/`ToModel`.

## Edge cases tratados

1. **Usuário só-Google tenta login por senha:** `GetByUserIdAndProvider(userId, Local)` retorna `null` -> "Email ou senha inválidos" (não revela que a conta é Google).
2. **Credencial inexistente (inconsistência de banco):** `credential == null` -> mesma mensagem neutra, não vaza info.
3. **`UpdateUser` sem senha:** atualiza só perfil, não toca na credencial.
4. **`UpdateUser` em usuário só-Google com senha informada:** `GetByUserIdAndProvider(userId, Local)` retorna `null` -> "Usuário não possui credencial local".
5. **Race condition no email único:** `UNIQUE` no banco + captura de `DbUpdateException` no `CreateUser` traduzida para `ApiException("E-mail já cadastrado", Conflict)`.

## Verificação

1. `dotnet test` — todos os testes (ajustados + novos) devem passar.
2. `dotnet build` — sem erros de compilação.
3. `dotnet ef database update` — migration aplicada sem erro em banco de dev.
4. Teste manual dos fluxos: criar usuário local -> login -> update sem senha -> update com senha -> forgot/reset password. Comportamento inalterado, só a arquitetura mudou.

## Fora de escopo (Fase 2 — spec separada)

- Login Google (`POST /auth/google`, `GoogleLogin` use case, `IGoogleTokenVerifier` adapter sobre `Google.Apis.Auth`).
- Adição de `google_sign_in` no app Flutter.
- Configuração de OAuth Client ID no Google Cloud Console.
- Fluxo de vinculação de conta Google a conta local existente por email.

