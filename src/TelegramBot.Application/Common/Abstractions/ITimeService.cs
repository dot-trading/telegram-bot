namespace TelegramBot.Application.Common.Abstractions;

public interface ITimeService
{
    /// <summary>
    /// The goal is to centralize the used data logic
    /// cross all module in the application
    /// </summary>
    /// <returns>Current Date (generally UTC)</returns>
    DateTime GetDateTimeNow();
}