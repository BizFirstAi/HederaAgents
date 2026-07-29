# HederaAgents

Hedera Hashgraph community node for [BizFirst.Ai](https://github.com/BizFirstAi) — a ProcessEngine
`ExecutionNode` (`hedera`) that exposes Hedera Mirror Node reads and utility operations as drag-and-drop
steps in BizFirst.Ai workflow automations.

## What it does

`HederaAgents` lets a BizFirst.Ai workflow query the Hedera network without touching an SDK or managing a
node connection. All current operations hit the free, eventually-consistent **Mirror Node REST API**, so no
signing credential or third-party SDK is required to read data.

| Resource | Operation | Description |
|---|---|---|
| `account` | `getBalance` | HBAR balance (tinybars + formatted) and held tokens for an account. |
| `account` | `getInfo` | Account metadata — EVM address, memo, auto-renew period, key, deleted flag. |
| `token` | `getInfo` | HTS token metadata — name, symbol, decimals, supply, type, treasury. |
| `token` | `getBalances` | An account's token holdings. |
| `topic` | `getMessages` | HCS topic messages in sequence order, base64-decoded to UTF-8 by default. |
| `transaction` | `get` | Transaction status, result, consensus timestamp, charged fee. |
| `utility` | `convertUnits` | Convert between `hbar` and `tinybar`. |
| `utility` | `validateAddress` | Validate a Hedera account/address string. |

Every operation accepts a `network` config key (`mainnet` \| `testnet` \| `previewnet`), defaulting to
`Hedera:DefaultNetwork` in application settings.

See [`Docs/Operations.md`](Docs/Operations.md) for the full field-by-field reference, or the published
[GitHub Pages site](https://bizfirstai.github.io/HederaAgents/) for the rendered version.

## Project layout

```
src/
├── BizFirst.Integration.Hedera.Domain     # Result records + network options (zero deps)
├── BizFirst.Integration.Hedera.Services   # Mirror Node REST client + resource services
└── BizFirst.Ai.ExecutionNodes.Blockchain.Hedera  # Executor: routing, config, operation DTOs
```

Targets **.NET 9**.

## Configuration

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

`HederaDependency.RegisterDefaults(services)` registers the Mirror Node client, resource services, the
executor (scoped), and the `ExecutorRegistry` entry (`hedera`). Host applications should also add
`new HederaDependency().RegisterDefaults(services);` to their node-plugin bootstrap so the assembly is
force-loaded and discoverable at runtime.

## Roadmap

Signed write operations (account create/transfer, token mint/associate/transfer, topic create/submit,
message signing) are next. They'll follow the same partial/service pattern as the reads above and require
the community **`Hashgraph`** .NET SDK (bugbytesinc), with the signing key resolved from a vault
`CRYPTO_WALLET` credential plus payer account id / key algorithm config fields.

## License

Community node maintained by the BizFirst.Ai team.
