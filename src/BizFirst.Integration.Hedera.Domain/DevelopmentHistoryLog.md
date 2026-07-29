# Hedera Integration Domain — Development History

## Initial build (2026-07-29)
- Result records for account (balance/info), token (info/balances), topic (messages), transaction (get),
  utility (convertUnits/validateAddress) — 5 resources, 8 operations (read/local slice).
- Common types: `HederaNetworkOptions`/`HederaNetworkConfig`, `HederaTokenBalance`, `HederaTopicMessage`.
- Positional `sealed record`s with `Ok()`/`Fail()` static factories; `ErrorCode` is `string.Empty` on success.
- Zero project references, zero NuGet packages (framework types only).
