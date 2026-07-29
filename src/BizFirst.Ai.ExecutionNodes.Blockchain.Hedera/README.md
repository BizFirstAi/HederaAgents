# Hedera ExecutionNode (`hedera`)

Hedera Hashgraph ExecutionNode for the BizFirst workflow engine. Implements the design in
`Documentation/.../010_NodeDesign-Engineer/ExecutionNodes/Hedera/44_Features`, following the guidelines in
`020_NodeServerProject-Engineer` and modelled on the **Ethereum** node's real patterns.

## Projects (3, net9.0)
- `BizFirst.Integration.Hedera.Domain` — sealed result records + network options (zero deps).
- `BizFirst.Integration.Hedera.Services` — Mirror Node REST client + resource services (per network).
- `BizFirst.Ai.ExecutionNodes.Blockchain.Hedera` — executor: routing, config, operation DTOs, feature partials.

## Operations implemented (mirror-read + local slice — build-verified)
| Resource | Operation | Path |
|---|---|---|
| account | `getBalance`, `getInfo` | Mirror REST |
| token | `getInfo`, `getBalances` | Mirror REST |
| topic | `getMessages` | Mirror REST |
| transaction | `get` | Mirror REST |
| utility | `convertUnits`, `validateAddress` | local |

All reads are free/eventually-consistent Mirror Node REST (`HederaMirrorClient`, typed `HttpClient`), so
they need no signing credential and no third-party SDK. Network is selected per operation via the
`network` config key (mainnet/testnet/previewnet); mirror URLs come from the `Hedera` appsettings section
(`HederaNetworkOptions`).

## Write operations (next milestone)
Signed writes (account create/transfer, token mint/associate/transfer, topic create/submit, sign message,
etc.) follow the identical partial/service pattern. They require the Hedera .NET SDK: the community
**`Hashgraph`** package (bugbytesinc) is net9-compatible and is the intended dependency for the Services
layer; the `Hiero` successor currently targets net10 only. The signing credential is a vault
**`CRYPTO_WALLET`** record resolved in the executor (see the Ethereum node's `.Credentials.cs` for the
`ReadCredentialRawPrimaryAsync` → `CryptoWalletRecord` pattern), plus the payer account id + key algorithm
as config fields. See `INFRASTRUCTURE_WalletManagement.md` in the design docs and ISSUE-01 (SDK spike).

## Configuration (`appsettings.json`)
```json
"Hedera": {
  "DefaultNetwork": "testnet",
  "Networks": {
    "mainnet":    { "MirrorRestUrl": "https://mainnet-public.mirrornode.hedera.com" },
    "testnet":    { "MirrorRestUrl": "https://testnet.mirrornode.hedera.com" },
    "previewnet": { "MirrorRestUrl": "https://previewnet.mirrornode.hedera.com" }
  }
}
```

## Registration
`HederaDependency.RegisterDefaults(services)` registers the services, the executor (Scoped), and the
`ExecutorRegistry` entry (`hedera`). As with Ethereum, also add an explicit
`new HederaDependency().RegisterDefaults(services);` line to `Plugins_RegisterAllNodes(...)` in
`ServiceCollectionExtensionsForAI.cs` so the assembly is force-loaded and discoverable at runtime.
