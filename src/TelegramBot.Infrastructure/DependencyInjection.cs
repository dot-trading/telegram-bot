using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Domain.Abstractions;
using TelegramBot.Infrastructure.Persistence;
using TelegramBot.Infrastructure.Services;

namespace TelegramBot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
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

        return services;
    }
}
