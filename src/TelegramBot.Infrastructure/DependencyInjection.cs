using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Domain.Abstractions;
using TelegramBot.Infrastructure.Persistence;
using TelegramBot.Infrastructure.Services;
using TelegramBot.Infrastructure.Clients;
using TradingProject.Bff.Client;

namespace TelegramBot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ConnectionStringFactory>();

        services.AddDbContext<TradingDbContext>((sp, options) =>
        {
            var factory = sp.GetRequiredService<ConnectionStringFactory>();
            options.UseNpgsql(factory.ConnectionString);
        });

        services.AddScoped<IDatabaseService, DatabaseService>();
        services.AddScoped<IBinanceService, BinanceService>();
        services.AddScoped<IClusterService, ClusterService>();

        // Register the typed BFF API client (used by command handlers for enriched notification DTOs)
        services.AddBffApiClient(configuration);

        // Register the legacy BFF service wrapper (P&L summary only, consumed by MessageFormatter)
        services.AddHttpClient<IBffService, BffServiceClient>();

        return services;
    }
}
