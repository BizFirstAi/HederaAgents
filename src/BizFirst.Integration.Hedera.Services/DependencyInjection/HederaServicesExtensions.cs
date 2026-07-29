using BizFirst.Integration.Hedera.Domain;

namespace BizFirst.Integration.Hedera.Services.DependencyInjection;

/// <summary>
/// DI extension methods for Hedera integration services. Binds the network catalogue lazily,
/// registers the Mirror Node typed HttpClient (Guideline 04 Flavour B) and one service per resource
/// (scoped, concrete classes — matches the Ethereum/Redis convention: no interfaces).
/// </summary>
public static class HederaServicesExtensions
{
    public static IServiceCollection AddDependenciesHederaServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Bound lazily against IConfiguration resolved from DI when HederaNetworkOptions is first
        // requested — deliberately NOT services.BuildServiceProvider() mid-registration.
        services.AddOptions<HederaNetworkOptions>().BindConfiguration(HederaNetworkOptions.SectionName);

        services.AddTransient<HederaMirrorRateLimitHandler>();
        services.AddHttpClient(HederaMirrorClient.HttpClientName)
            .ConfigureHttpClient((sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<HederaNetworkOptions>>().Value;
                var seconds = opts.MirrorTimeoutSeconds > 0 ? opts.MirrorTimeoutSeconds : 30;
                client.Timeout = TimeSpan.FromSeconds(seconds);
            })
            .AddHttpMessageHandler<HederaMirrorRateLimitHandler>();
        services.AddSingleton<HederaMirrorClient>();

        // Resource services (scoped, concrete classes).
        services.AddScoped<HederaAccountService>();
        services.AddScoped<HederaTokenService>();
        services.AddScoped<HederaTopicService>();
        services.AddScoped<HederaTransactionService>();
        services.AddScoped<HederaUtilityService>();

        return services;
    }
}
