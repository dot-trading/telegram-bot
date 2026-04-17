using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TelegramBot.Domain;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureAllSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settingsTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    return ex.Types.Where(t => t is not null)!;
                }
            })
            .Where(t =>
                t is not null &&
                t is { IsClass: true, IsAbstract: false } &&
                t.Name.EndsWith("Settings", StringComparison.Ordinal));

        foreach (var type in settingsTypes)
        {
            if (type is null) continue;

            var method = type.GetMethod(
                "BindSettingsToProperties",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(IServiceCollection), typeof(IConfiguration)],
                modifiers: null);

            method?.Invoke(null, [services, configuration]);
        }

        return services;
    }
}