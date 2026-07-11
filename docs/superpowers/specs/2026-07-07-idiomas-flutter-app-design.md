# Idiomas App — Flutter Mobile Client Design

> Design document for the Android companion app of IdiomasAPI.
> Date: 2026-07-07
> Status: Approved for implementation planning

## 1. Context

IdiomasAPI is a .NET 9 REST backend for language learning. It provides JWT authentication, AI-powered conversations via Google Gemini, a personal vocabulary dictionary, and file upload support. This document specifies the design of the first Flutter mobile client that consumes the API.

**Repository location:** `C:\Users\devca\Projetos\idiomas\idiomas-android\` — a dedicated directory alongside the backend, not inside it.

## 2. Goals

- Deliver a focused MVP with login/registration, dictionary CRUD, and basic conversation practice.
- Avoid Android Studio; use only Flutter + Dart inside Windsurf/VS Code.
- Follow the same layered architectural mindset used by the backend (Clean Architecture).
- Build a codebase that is testable and easy to extend toward offline-first, biometrics, and profile management.

## 3. Non-Goals

The following features are intentionally out of scope for the MVP but documented as future work:

- User profile editing.
- File upload/attachment support.
- Detailed conversation history for ended conversations.
- Offline-first dictionary synchronization.
- Biometric authentication and "remember me".
- Push notifications.
- App internationalization.
- Custom fonts (system font is used in the MVP).

## 4. Technology Stack

| Layer | Choice | Rationale |
|-------|--------|-----------|
| Framework | Flutter + Dart | Cross-platform, UI in code, no Android Studio required, hot reload, large agent-friendly ecosystem. |
| IDE | Windsurf / VS Code | Lightweight, AI-agent friendly, no need for Android Studio. |
| State management | Riverpod + MVVM | Declarative, testable, scales naturally. |
| Navigation | GoRouter | Declarative routing with deep-link support and redirects. |
| HTTP client | Dio | Interceptors, cancellation, and error mapping out of the box. |
| Secure storage | flutter_secure_storage | Stores JWT in Android Keystore / iOS Keychain. |
| Functional helpers | fpdart | Lightweight `Result<T>` and functional composition. |
| Immutable models | Freezed + json_serializable | Boilerplate reduction for DTOs/entities. |
| Icons | lucide_flutter | Lucide icon set. Consistent with the Figma Make mockups. |
| Tests | mocktail + flutter_test | Unit and widget tests. |

## 5. Architecture

### 5.1. Layered structure per feature

```
lib/
├── core/
│   ├── constants/
│   ├── errors/
│   ├── network/
│   │   ├── http_client.dart              # Port/contract
│   │   └── dio_http_client.dart          # Adapter for Dio
│   ├── storage/
│   │   ├── token_storage.dart            # Port/contract
│   │   └── flutter_secure_token_storage.dart
│   ├── theme/
│   │   ├── app_colors.dart
│   │   ├── app_spacing.dart
│   │   ├── app_typography.dart
│   │   └── app_theme.dart              # Includes button/input theme configs
│   ├── widgets/
│   │   ├── app_button.dart
│   │   ├── app_input.dart
│   │   ├── app_card.dart
│   │   └── app_empty_state.dart
│   └── routing/
│       └── app_router.dart
│
├── features/
│   ├── auth/
│   │   ├── data/
│   │   ├── domain/
│   │   └── presentation/
│   ├── dictionary/
│   │   ├── data/
│   │   ├── domain/
│   │   └── presentation/
│   ├── conversation/
│   │   ├── data/
│   │   ├── domain/
│   │   └── presentation/
│   └── home/
│       └── presentation/              # No domain/data: reads providers from other features directly
│
└── main.dart
```

### 5.2. Dependency rule

Dependency direction is inward:

- `presentation` depends on `domain`.
- `data` depends on `domain`.
- `core` is depended on by all layers.
- `domain` does not depend on Flutter, Dio, or storage implementations.

### 5.3. Data flow

A typical screen interaction follows this flow:

```
Screen -> ViewModel -> UseCase -> Repository -> RemoteDataSource -> HttpClient -> IdiomasAPI
Screen <- ViewModel <- UseCase <- Repository <- RemoteDataSource <- HttpClient <- IdiomasAPI
```

- `ViewModel` exposes `AsyncValue<T>` states observed by the UI.
- `UseCase` encapsulates a single business action and returns `Result<T>`.
- `Repository` maps data models to domain entities and handles errors.
- `RemoteDataSource` performs raw HTTP calls.
- `HttpClient` adds the `Authorization` header and maps HTTP failures to `AppError`.

## 6. State Management

Riverpod is used for both dependency injection and state management.

Examples:

- `loginViewModelProvider` exposes `AsyncValue<AuthState>`.
- `dictionaryListViewModelProvider` exposes `AsyncValue<List<Word>>`.
- `conversationViewModelProvider` exposes `AsyncValue<ConversationDetail>`.

ViewModels are kept thin: they delegate business logic to UseCases and translate `Result<T>` into `AsyncValue`.

## 7. Navigation

GoRouter is configured declaratively. The router guards authenticated routes by reading the auth state.

### Routes

| Route | Screen | Notes |
|-------|--------|-------|
| `/` | `SplashScreen` | Verifies token and redirects to login or home. |
| `/login` | `LoginScreen` | Public. |
| `/register` | `RegisterScreen` | Public. |
| `/home` | `HomeScreen` | Authenticated. Bottom nav item: Início. |
| `/dictionary` | `DictionaryListScreen` | Authenticated. Bottom nav item: Dicionário. |
| `/dictionary/new` | `WordFormScreen` | Authenticated. |
| `/dictionary/:id/edit` | `WordFormScreen` | Authenticated. |
| `/conversations` | `ConversationListScreen` | Authenticated. Bottom nav item: Conversas. |
| `/conversations/new` | `ConversationStartScreen` | Authenticated. |
| `/conversations/:id` | `ConversationScreen` | Authenticated. |

### Bottom navigation

After authentication, the user sees three tabs:

- Início (`/home`)
- Dicionário (`/dictionary`)
- Conversas (`/conversations`)

## 8. UI Design

### 8.1. Color palette

| Token | Hex | Usage |
|-------|-----|-------|
| Primary | `#5F3232` | Marsala. Buttons, FAB, active bottom nav, links, statistics, splash screen background. |
| Secondary | `#C06C4F` | Terracota. Selected chips/options, secondary action buttons, accents. |
| Background | `#FFFFFF` | Scaffold background. |
| Surface | `#F9FAFB` | Cards, input backgrounds. |
| Text primary | `#1F2937` | Headlines and body text. |
| Text secondary | `#6B7280` | Subtitles, hints, metadata. |
| Error | `#B91C1C` | Validation errors and failure states. |

The palette is derived from the app icon: a marsala background with a speech-brain icon representing AI-powered language learning.

### 8.2. Typography

System font (Roboto on Android, San Francisco on iOS). No custom font in the MVP.

Styles:

| Style | Size | Weight | Usage |
|-------|------|--------|-------|
| Headline | 24 | Bold | Splash title, screen titles. |
| Title | 18 | Semibold | AppBar titles, card titles. |
| Body | 14 | Regular | Body text, input labels. |
| Caption | 12 | Regular | Hints, IPA, dates, metadata. |

### 8.3. Icons

Package: `lucide_flutter`. Consistent with the Figma Make mockups.

| Usage | Icon |
|-------|------|
| Bottom nav — Início | `Home` |
| Bottom nav — Dicionário | `BookOpen` |
| Bottom nav — Conversas | `MessageCircle` |
| Search field | `Search` |
| Send message | `Send` |
| Language dropdown | `ChevronDown` |
| FAB — Add word | `Plus` |
| Edit word | `Edit2` |
| Delete word | `Trash2` |
| End conversation | `StopCircle` |

### 8.4. Spacing tokens

| Token | Value |
|-------|-------|
| xs | 4 |
| sm | 8 |
| md | 16 |
| lg | 24 |
| xl | 32 |

### 8.5. Theme setup

A single `AppTheme.light()` returns a `ThemeData` configured with `ColorScheme`, `TextTheme`, and `ElevatedButtonThemeData`. All custom colors, spacing, and typography are imported from `app_colors.dart`, `app_spacing.dart`, and `app_typography.dart`.

### 8.6. Screens

1. **SplashScreen** — full-screen marsala background with the app icon and the tagline "Aprenda idiomas com IA". It checks for a stored token and redirects to `/home` or `/login`.
2. **LoginScreen** — e-mail and password fields, primary "Entrar" button, link to register.
3. **RegisterScreen** — name, e-mail, password fields, primary "Cadastrar" button, link to login.
4. **HomeScreen** — dashboard with quick stats (words saved, conversations, active language) and shortcuts to start a conversation or open the dictionary.
5. **DictionaryListScreen** — searchable list of words, floating action button to add a word.
6. **WordFormScreen** — form for word, IPA, and a dynamic list of meanings/examples. Used for both create and edit.
7. **ConversationStartScreen** — language dropdown, mode selection (Free/Guided), scenario selection. First options are pre-selected and highlighted in secondary color.
8. **ConversationListScreen** — list of active and past conversations.
9. **ConversationScreen** — chat interface with user messages, assistant messages, and inline correction cards.

### 8.7. Reusable widgets

- `AppButton` — primary, secondary, and terracota variants.
- `AppInput` — text input with consistent decoration and error handling.
- `AppCard` — surface card for dashboard and list items.
- `AppEmptyState` — placeholder for empty lists.
- `SelectableOption` — chip used in ConversationStartScreen.
- `CorrectionCard` — displays original fragment, suggestion, explanation, and error type.

## 9. Domain Entities

### User

```dart
class User {
  const User({required this.id, required this.name, required this.email});

  final String id;
  final String name;
  final String email;
}
```

### Word / Meaning

```dart
class Word {
  const Word({
    required this.id,
    required this.word,
    required this.ipa,
    required this.meanings,
  });

  final String id;
  final String word;
  final String ipa;
  final List<Meaning> meanings;
}

class Meaning {
  const Meaning({required this.id, required this.meaning, this.example});

  final String id;
  final String meaning;
  final String? example;
}
```

### Conversation / Message / Correction

```dart
class Conversation {
  const Conversation({
    required this.id,
    required this.language,
    required this.mode,
    this.scenarioId,
    required this.createdAt,
    required this.updatedAt,
    required this.isActive,
  });

  final String id;
  final Language language;
  final ConversationMode mode;
  final String? scenarioId;
  final DateTime createdAt;
  final DateTime updatedAt;
  final bool isActive;
}

class ConversationDetail extends Conversation {
  const ConversationDetail({
    required super.id,
    required super.language,
    required super.mode,
    super.scenarioId,
    required super.createdAt,
    required super.updatedAt,
    required super.isActive,
    required this.messages,
  });

  final List<Message> messages;
}

class Message {
  const Message({
    required this.id,
    required this.content,
    required this.role,
    required this.corrections,
    required this.createdAt,
  });

  final String id;
  final String content;
  final MessageRole role;
  final List<Correction> corrections;
  final DateTime createdAt;
}

class Correction {
  const Correction({
    required this.originalFragment,
    required this.suggestedFragment,
    required this.explanation,
    required this.type,
  });

  final String originalFragment;
  final String suggestedFragment;
  final String explanation;
  final ErrorType type;
}
```

### Scenario

```dart
class Scenario {
  const Scenario({
    required this.id,
    required this.title,
    required this.description,
  });

  final String id;
  final String title;
  final String description;
}
```

### Enums

Mapped directly from the API:

- `Language` — english, spanish, french, german, italian, portuguese.
- `ConversationMode` — free, guided.
- `MessageRole` — user, assistant, system.
- `ErrorType` — grammar, vocabulary, pronunciation, spelling, syntax.

## 10. API Contracts

| Feature | Method | Endpoint | Purpose |
|---------|--------|----------|---------|
| Auth | POST | `/auth/login` | Login. |
| User | POST | `/user` | Register. |
| Dictionary | GET | `/dictionary/word` | List words. |
| Dictionary | POST | `/dictionary/word` | Create word. |
| Dictionary | PUT | `/dictionary/word/{id}` | Update word. |
| Dictionary | DELETE | `/dictionary/word/{id}` | Delete word. |
| Conversation | GET | `/conversations/scenarios` | List scenarios. |
| Conversation | POST | `/conversations` | Start conversation. |
| Conversation | POST | `/conversations/{id}/messages` | Send message. |
| Conversation | GET | `/conversations/{id}` | Get conversation details. |
| Conversation | GET | `/conversations` | List conversations. |
| Conversation | PATCH | `/conversations/{id}/end` | End conversation. |

All authenticated requests send `Authorization: Bearer <token>`.

## 11. Error Handling

### Error types

```dart
sealed class AppError {
  const AppError();
}

class NetworkError extends AppError {
  const NetworkError(this.message);
  final String message;
}

class UnauthorizedError extends AppError {
  const UnauthorizedError();
}

class ValidationError extends AppError {
  const ValidationError(this.fieldErrors);
  final Map<String, String> fieldErrors;
}

class ServerError extends AppError {
  const ServerError(this.message);
  final String message;
}

class UnknownError extends AppError {
  const UnknownError(this.message);
  final String message;
}
```

### Mapping

- `400` → `ValidationError`
- `401` → `UnauthorizedError` (delete token, redirect to login)
- `404` → `ServerError` with friendly message
- `>= 500` → `ServerError`
- No connection / timeout → `NetworkError`

### UI behavior

- `AsyncValue.loading` → `CircularProgressIndicator` or inline button spinner.
- `AsyncValue.error` → inline error message or retry button.
- Lists support pull-to-refresh and a "Tentar novamente" action on failure.

## 12. Authentication Flow

1. User opens app.
2. `SplashScreen` reads token from secure storage.
3. If token exists, redirect to `/home`.
4. If token is missing or invalid, redirect to `/login`.
5. Login/Register calls API and stores token on success.
6. Logout deletes token and redirects to `/login`.
7. Any `401` response from the API triggers automatic logout.

## 13. Testing Strategy

### Unit tests

- ViewModels: state transitions (loading → success/error).
- UseCases: correct delegation and `Result` mapping.
- Repositories: JSON parsing and error mapping.
- Helpers: enum parsing, date formatting, etc.

### Widget tests

- Login form renders error on invalid credentials.
- Dictionary list renders words after loading.
- Conversation screen shows assistant message with corrections.
- Buttons show loading state and disable user input during submission.

### Test structure

```
test/
├── core/
│   └── network/
│       └── http_client_test.dart
├── features/
│   ├── auth/
│   ├── dictionary/
│   └── conversation/
└── widget/
    ├── login_screen_test.dart
    ├── dictionary_list_screen_test.dart
    └── conversation_screen_test.dart
```

## 14. Implementation Order

1. Project setup, dependencies, and folder structure.
2. Core: theme, HTTP client, secure storage, error handling, base widgets.
3. Auth feature: login, register, splash screen, router guards.
4. Home feature: dashboard screen.
5. Dictionary feature: list, create, edit, delete.
6. Conversation feature: list, start, chat, send message, end.
7. Unit and widget tests.
8. UI polish and edge cases.

## 15. Future Work

- Offline-first dictionary with Drift (SQLite).
- Biometric login and "remember me".
- Profile editing and file upload support.
- Push notifications for conversation reminders.
- Custom fonts and refined animations.
- Internationalization of the app UI.

## 16. Visual References

- **Figma Make mockups:** https://www.figma.com/make/uYKRUSTQF9R8jay8K3CIit — all 8 screens (splash, login, register, home, dictionary list, word form, conversation start, conversation chat) implemented in React using the approved palette.
- **Local Visual Companion:** `.devin/visual-companion/index.html` — HTML mockup served locally at `http://localhost:8765`. Includes side-by-side icon comparison (Material Icons vs Lucide).
- **App icon:** marsala background (`#5F3232`) with a speech-bubble and AI-brain motif. Used as reference for the splash screen.

## 17. Open Questions

None. All design decisions have been validated and are ready for implementation planning.

