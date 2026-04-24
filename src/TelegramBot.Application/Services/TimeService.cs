using TelegramBot.Application.Common.Abstractions;

namespace TelegramBot.Application.Services;

public class TimeService : ITimeService
{
    /// <summary>
    /// The goal is to centralize the used data logic
    /// cross all module in the application
    /// </summary>
    /// <returns>Current Date (generally UTC)</returns>
    public DateTime GetDateTimeNow()
    {
        return DateTime.UtcNow;
    }
}