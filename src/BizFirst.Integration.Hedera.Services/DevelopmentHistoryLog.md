# Hedera Integration Services — Development History

## Initial build (2026-07-29)
- `HederaMirrorClient`: typed HttpClient via factory, per-network base URL from `HederaNetworkOptions`,
  404 → `MIRROR_NOT_FOUND`, raw status+body for defensive parsing.
- `HederaMirrorRateLimitHandler`: DelegatingHandler retrying 429/502/503/504 (up to 3×, honors Retry-After).
- Resource services: `HederaAccountService` (getBalance/getInfo), `HederaTokenService` (getInfo/getBalances),
  `HederaTopicService` (getMessages with UTF-8 base64 decode), `HederaTransactionService` (get with ID normalization),
  `HederaUtilityService` (convertUnits/validateAddress — local, no network).
- All async methods: `catch (OperationCanceledException) { throw; }` first; network/parse/timeout errors mapped
  to `Fail()` with structured codes (MIRROR_TIMEOUT, MIRROR_UNAVAILABLE, MIRROR_NOT_FOUND, MIRROR_PARSE_ERROR).
- `HederaJson` defensive accessors: `GetStringLoose`/`GetIntLoose`/`GetLongLoose` for wrong-typed mirror fields.
- DI: `AddDependenciesHederaServices()`, HttpClient timeout wired to config, services scoped, mirror client singleton.
