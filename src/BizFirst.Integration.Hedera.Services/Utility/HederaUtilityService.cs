namespace BizFirst.Integration.Hedera.Services;

/// <summary>Local utility operations — no network access (UTIL01 validate address, UTIL02 convert units).</summary>
public sealed class HederaUtilityService
{
    /// <summary>UTIL02 — exact tinybar ↔ HBAR conversion (decimal math, no float).</summary>
    public HederaConvertUnitsResult ConvertUnits(string amount, string fromUnit, string toUnit)
    {
        var from = (fromUnit ?? string.Empty).Trim().ToLowerInvariant();
        var to = (toUnit ?? string.Empty).Trim().ToLowerInvariant();
        if (from is not ("hbar" or "tinybar")) return HederaConvertUnitsResult.Fail("VAL_INVALID_UNIT", "fromUnit must be 'hbar' or 'tinybar'.");
        if (to is not ("hbar" or "tinybar")) return HederaConvertUnitsResult.Fail("VAL_INVALID_UNIT", "toUnit must be 'hbar' or 'tinybar'.");
        if (!decimal.TryParse(amount, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
            return HederaConvertUnitsResult.Fail("VAL_INVALID_AMOUNT", $"'{amount}' is not a valid number.");

        if (from == to)
            return HederaConvertUnitsResult.Ok(value.ToString(CultureInfo.InvariantCulture), from, to);

        if (from == "hbar" && to == "tinybar")
        {
            var tinybars = value * HederaUnits.TinybarsPerHbar;
            if (tinybars != decimal.Truncate(tinybars))
                return HederaConvertUnitsResult.Fail("VAL_INVALID_AMOUNT", "HBAR amount has sub-tinybar precision (>8 decimal places).");
            return HederaConvertUnitsResult.Ok(decimal.Truncate(tinybars).ToString("0", CultureInfo.InvariantCulture), from, to);
        }

        // tinybar → hbar
        if (value != decimal.Truncate(value))
            return HederaConvertUnitsResult.Fail("VAL_INVALID_AMOUNT", "tinybar amount must be a whole number.");
        var hbar = value / HederaUnits.TinybarsPerHbar;
        return HederaConvertUnitsResult.Ok(hbar.ToString("0.########", CultureInfo.InvariantCulture), from, to);
    }

    /// <summary>UTIL01 — classify an address as native (shard.realm.num), EVM long-zero, or EVM alias.</summary>
    public HederaValidateAddressResult ValidateAddress(string address)
    {
        var addr = (address ?? string.Empty).Trim();
        if (addr.Length == 0) return HederaValidateAddressResult.Fail("VAL_MISSING_ADDRESS", "address is required.");

        // Native shard.realm.num (e.g. 0.0.12345)
        var parts = addr.Split('.');
        if (parts.Length == 3 && parts.All(p => p.Length > 0 && p.All(char.IsDigit)))
            return HederaValidateAddressResult.Ok(true, "nativeId", addr);

        // EVM 20-byte hex (with or without 0x)
        var hex = addr.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? addr[2..] : addr;
        if (hex.Length == 40 && hex.All(Uri.IsHexDigit))
        {
            // Long-zero form has the first 12 bytes (24 hex chars) zeroed.
            var isLongZero = hex[..24].All(c => c == '0');
            return HederaValidateAddressResult.Ok(true, isLongZero ? "evmLongZero" : "evmAlias", "0x" + hex.ToLowerInvariant());
        }

        return HederaValidateAddressResult.Ok(false, "unknown", addr);
    }
}
