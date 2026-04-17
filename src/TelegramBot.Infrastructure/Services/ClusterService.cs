using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TelegramBot.Domain.Abstractions;
using TelegramBot.Domain.Settings;

namespace TelegramBot.Infrastructure.Services;

public class ClusterService(IOptions<ClusterSettings> settings) : IClusterService
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(5) };

    public string GetOrchestratorStatus()
    {
        try
        {
            var output = RunKubectl("-n trading-ai get pods -l app=orchestrator --no-headers -o custom-columns=STATUS:.status.phase");
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (lines.Length == 0) return "❌ Introuvable (K8s)";
            if (lines.Any(l => l == "Running")) return "✅ Running";
            if (lines.Any(l => l == "Pending")) return "⏳ Pending";
            return $"❌ {lines[0]}";
        }
        catch
        {
            return "❌ Erreur / Inconnu";
        }
    }

    public async Task<string> GetOllamaStatusAsync()
    {
        try
        {
            await Http.GetAsync($"{settings.Value.OllamaUrl}/api/tags");
            return "✅ Running";
        }
        catch
        {
            return "❌ KO";
        }
    }

    public async Task<List<OllamaModel>> GetOllamaModelsAsync()
    {
        try
        {
            var response = await Http.GetAsync($"{settings.Value.OllamaUrl}/api/tags");
            response.EnsureSuccessStatusCode();
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("models")
                .EnumerateArray()
                .Select(m => new OllamaModel(
                    m.GetProperty("name").GetString() ?? "",
                    m.TryGetProperty("size", out var s) ? s.GetInt64() : 0,
                    m.TryGetProperty("modified_at", out var d) ? d.GetString() ?? "" : ""))
                .ToList();
        }
        catch
        {
            return [];
        }
    }

    public string GetK8sPodsStatus() =>
        RunKubectl("-n trading-ai get pods --no-headers -o custom-columns=NAME:.metadata.name,STATUS:.status.phase,NODE:.spec.nodeName");

    private static string RunKubectl(string args)
    {
        var psi = new ProcessStartInfo("kubectl", args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        using var proc = Process.Start(psi) ?? throw new InvalidOperationException("kubectl not found");
        proc.WaitForExit(10_000);
        return proc.StandardOutput.ReadToEnd().Trim();
    }
}
