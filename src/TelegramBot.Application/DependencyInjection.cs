using Cortex.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Application.Common.Abstractions;
using TelegramBot.Application.Services;
using TelegramBot.Domain.Abstractions;

namespace TelegramBot.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMessageFormatter, MessageFormatter>();
        services.AddScoped<ITimeService, TimeService>();

        // Register all ICommandHandler / IQueryHandler implementations in this assembly.
        services.AddCortexMediator([typeof(DependencyInjection)]);

        return services;
    }
}
