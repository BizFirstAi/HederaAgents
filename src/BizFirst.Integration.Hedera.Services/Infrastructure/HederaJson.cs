namespace BizFirst.Integration.Hedera.Services;

/// <summary>
/// Defensive JSON accessors for mirror-node responses. Mirror fields are not always the JSON type you
/// expect (e.g. <c>total_supply</c> may be a string or a number depending on magnitude), so these read
/// loosely rather than throwing <see cref="InvalidOperationException"/> from a wrong-typed accessor.
/// </summary>
internal static class HederaJson
{
    /// <summary>Returns a field's text whether it is a JSON string, number, or absent.</summary>
    public static string GetStringLoose(JsonElement el) => el.ValueKind switch
    {
        JsonValueKind.String => el.GetString() ?? string.Empty,
        JsonValueKind.Number => el.GetRawText(),
        JsonValueKind.True => "true",
        JsonValueKind.False => "false",
        JsonValueKind.Null or JsonValueKind.Undefined => string.Empty,
        _ => el.GetRawText(),
    };

    /// <summary>Returns an integer whether the field is a JSON number or numeric string; 0 otherwise.</summary>
    public static int GetIntLoose(JsonElement el) =>
        el.ValueKind == JsonValueKind.Number && el.TryGetInt32(out var v) ? v
        : el.ValueKind == JsonValueKind.String && int.TryParse(el.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var sv) ? sv
        : 0;

    /// <summary>Returns a long whether the field is a JSON number or numeric string; 0 otherwise.</summary>
    public static long GetLongLoose(JsonElement el) =>
        el.ValueKind == JsonValueKind.Number && el.TryGetInt64(out var v) ? v
        : el.ValueKind == JsonValueKind.String && long.TryParse(el.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var sv) ? sv
        : 0;
}
