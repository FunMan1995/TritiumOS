using System.Text.Json;

namespace TritiumOS;

static class ComputeConfig
{
    public sealed class BackendEntry
    {
        public string label { get; set; } = "";
        public string provider { get; set; } = "";
        public string test_provider { get; set; } = "";
        public string cost { get; set; } = "";
        public bool qpu { get; set; }
        public bool enabled { get; set; } = true;
    }

    public sealed class Root
    {
        public int version { get; set; } = 1;
        public string active { get; set; } = "aer_local";
        public bool allow_qpu { get; set; }
        public int max_shots { get; set; } = 500;
        public bool ibm_enabled { get; set; }
        public Dictionary<string, BackendEntry> backends { get; set; } = new();
    }

    static string[] SearchPaths()
    {
        var baseDir = AppContext.BaseDirectory;
        return
        [
            Path.Combine(baseDir, "qd", "compute.json"),
            Path.Combine(baseDir, "poly", "compute.json"),
            Path.Combine(baseDir, "tritium.poly", "compute.json"),
            Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "qd", "compute.json")),
        ];
    }

    public static Root Load()
    {
        foreach (var path in SearchPaths())
        {
            if (!File.Exists(path)) continue;
            var json = File.ReadAllText(path);
            var cfg = JsonSerializer.Deserialize<Root>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (cfg is null || string.IsNullOrEmpty(cfg.active)) continue;
            if (cfg.backends.Count == 0 && json.Contains("doc", StringComparison.Ordinal)) continue;
            return cfg;
        }
        return new Root();
    }

    public static string ActiveTestProvider(Root cfg)
    {
        if (cfg.backends.TryGetValue(cfg.active, out var b) && !string.IsNullOrEmpty(b.test_provider))
            return b.test_provider;
        return "aer";
    }
}