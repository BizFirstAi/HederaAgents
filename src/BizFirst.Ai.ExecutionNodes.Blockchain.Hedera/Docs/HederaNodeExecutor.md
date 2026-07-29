# Hedera ExecutionNode — Operation Reference

`NodeTypeName`: `hedera` · Area: `Blockchain`

Every operation takes `resource`, `operation`, and an optional `network` (defaults to the
`Hedera:DefaultNetwork` application setting, typically `testnet`) as config keys, plus the operation-specific fields
listed below. Read operations use the free Mirror Node REST API; write operations (future) will require a vault
`credentialID` pointing at a secret holding the wallet's private key.

## account

| operation | fields | notes |
|---|---|---|
| `getBalance` | `accountId`* | Returns HBAR balance (tinybars + formatted) and held tokens. |
| `getInfo` | `accountId`* | Account metadata: EVM address, memo, auto-renew period, key, deleted flag. |

## token (HTS)

| operation | fields | notes |
|---|---|---|
| `getInfo` | `tokenId`* | Token metadata: name, symbol, decimals, total supply, type, treasury, deleted. |
| `getBalances` | `accountId`*, `limit` (1-100, def 25) | Account's token holdings (ID, balance, decimals). |

## topic (HCS)

| operation | fields | notes |
|---|---|---|
| `getMessages` | `topicId`*, `limit` (1-100, def 25), `sequenceFrom`, `decodeUtf8` (def true) | Topic messages in sequence order; contents base64-decoded to UTF-8 by default. |

## transaction

| operation | fields | notes |
|---|---|---|
| `get` | `transactionId`* | Transaction status, result, consensus timestamp, charged fee. Accepts native (`0.0.1@1690000000.000000000`) or dash form. |

## utility

| operation | fields |
|---|---|
| `convertUnits` | `amount`*, `fromUnit` (hbar\|tinybar, def hbar), `toUnit` (def tinybar) |
| `validateAddress` | `address`* |

`*` = required.

### Supported networks

- `mainnet` — Hedera mainnet (production, real HBAR transfers)
- `testnet` — Hedera testnet (default, free faucet HBAR)
- `previewnet` — Hedera previewnet (early features)

Add more networks via the `Hedera:Networks` appsettings section.
