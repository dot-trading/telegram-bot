using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TelegramBot.Infrastructure.Persistence;

/// <summary>
/// Used by the EF Core CLI (dotnet ef migrations add / database update)
/// when no DI host is available at design time.
/// </summary>
public class TradingDbContextFactory : IDesignTimeDbContextFactory<TradingDbContext>
{
    public TradingDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
            ?? BuildConnectionString();

        var options = new DbContextOptionsBuilder<TradingDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new TradingDbContext(options);
    }

    private static string BuildConnectionString()
    {
        var host     = Environment.GetEnvironmentVariable("POSTGRES_HOST")     ?? "localhost";
        var port     = Environment.GetEnvironmentVariable("POSTGRES_PORT")     ?? "5432";
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "trading_secure_pwd_2026";

        return $"Host={host};Port={port};Database=trading;Username=trading;Password={password}";
    }
}
