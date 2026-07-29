# Hedera ExecutionNode — Development History Log

## 2026-07-29 — Initial scaffold + read/local slice (builds green)
- Created the 3-project structure (Domain / Services / Executor, net9.0) under
  `Blockchain/Hedera/`, matching the Ethereum node's structure, csproj references, and conventions.
- **Domain:** `HederaNetworkOptions` + 10 result records (account balance/info, token info/balances,
  topic messages, transaction get, convert-units, validate-address, shared token-balance/topic-message).
- **Services:** `HederaMirrorClient` (typed HttpClient, per-network base URL, 404→MIRROR_NOT_FOUND) +
  `HederaAccountService`, `HederaTokenService`, `HederaTopicService`, `HederaTransactionService`,
  `HederaUtilityService` + `HederaServicesExtensions` DI. All reads are Mirror Node REST; utilities local.
  `catch (OperationCanceledException){throw;}` first; HTTP/network errors mapped to result codes, never thrown.
- **Executor:** `HederaNodeExecutor` (routing, `ResourceBasedNodeExecutor`, `IActionNodeExecution`,
  `NodeTypeName="hedera"`) + `.Config.cs` (settings, ports, `GetNodeExecutorManifest`) + settings root +
  `BaseHederaOperationInfo` + `HederaOperationInfoFactory` + 8 operation DTOs + 8 feature partials
  (020 code-step 1.1–1.9 output-items-merge contract) + `HederaDependency`.
- **Build:** `dotnet build` green — 0 errors; Hedera source warning-clean (only pre-existing ProcessEngine
  CS1591 warnings remain). Fixed one missing global using (`...Domain.Credentials` for `INodeCredentialsFactory`).

### Next
- Add the Hedera SDK (`Hashgraph`, net9-compatible) to Services + `HederaClientFactory` (network + operator).
- Implement signed writes on the same pattern (account create/transfer, token mint/associate/transfer,
  topic create/submit, wallet sign/verify) using the `CRYPTO_WALLET` credential (see Ethereum `.Credentials.cs`).
- Add the remaining mirror reads (NFT info/list, token NFT, more transaction reads) and triggers.
- Tests project (xUnit + Moq) mirroring the Ethereum Tests layout.
- Add `new HederaDependency().RegisterDefaults(services);` to `Plugins_RegisterAllNodes(...)`.
