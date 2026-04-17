namespace TelegramBot.Domain.Abstractions;

public interface IClusterService
{
    string GetOrchestratorStatus();
    Task<string> GetOllamaStatusAsync();
    Task<List<OllamaModel>> GetOllamaModelsAsync();
    string GetK8sPodsStatus();
}

public record OllamaModel(string Name, long Size, string ModifiedAt);
