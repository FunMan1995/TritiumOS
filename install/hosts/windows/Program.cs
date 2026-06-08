using System.Drawing;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace TritiumOS;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}

sealed class MainForm : Form
{
    readonly TextBox _log = new() { Multiline = true, ReadOnly = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical };
    readonly TextBox _input = new() { Dock = DockStyle.Bottom };
    string _assistantName = "";
    bool _bootstrapped;
    ComputeConfig.Root _compute = new();

    TritiumForthVM? _vm;

    public MainForm()
    {
        Text = "TritiumOS";
        Size = new Size(720, 480);
        StartPosition = FormStartPosition.CenterScreen;
        var bar = new Panel { Dock = DockStyle.Top, Height = 36 };
        var about = new Button { Text = "About", Dock = DockStyle.Right, Width = 80 };
        about.Click += (_, _) => ShowAbout();
        bar.Controls.Add(about);
        Controls.Add(_log);
        Controls.Add(_input);
        Controls.Add(bar);
        _input.KeyDown += OnInputKeyDown;
        if (!RunFirstBoot()) return;
        _compute = ComputeConfig.Load();
        Append("TritiumOS by Draco — Windows 11 (initial platform)" + Environment.NewLine);
        Append("Core: TritiumForth (DuskOS/CollapseOS reference patterns)" + Environment.NewLine);
        Append($"Assistant: {_assistantName}" + Environment.NewLine);
        Append($"Compute: {_compute.active} ({ComputeConfig.ActiveTestProvider(_compute)})" + Environment.NewLine);
        Append($"Data: {UserEvolveDir()}" + Environment.NewLine);
        Append("Type help for commands. Forth core: tritium.poly/core/boot.fs" + Environment.NewLine);

        InitForthVM();
    }

    bool RunFirstBoot()
    {
        var evolve = UserEvolveDir();
        var nameFile = Path.Combine(evolve, "assistant-name.trit");
        if (File.Exists(nameFile))
        {
            _assistantName = File.ReadAllText(nameFile).Trim();
            _bootstrapped = true;
            Text = $"{_assistantName} — powered by TritiumOS";
            return true;
        }

        var key = Prompt("TritiumOS — License", "Enter license key (scaffold: 8+ chars):");
        if (string.IsNullOrWhiteSpace(key) || key.Length < 8) { MessageBox.Show("License required."); Close(); return false; }

        _assistantName = Prompt("Name your assistant", "Choose a name for your assistant:") ?? "";
        if (string.IsNullOrWhiteSpace(_assistantName)) _assistantName = "Assistant";
        File.WriteAllText(nameFile, _assistantName, Encoding.UTF8);

        var edition = MessageBox.Show("Use 64-bit (magenta) edition?\nNo = 32-bit (cyan).", "Edition",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes ? "64" : "32";
        File.WriteAllText(Path.Combine(evolve, "edition.trit"), edition, Encoding.UTF8);

        _bootstrapped = true;
        Text = $"{_assistantName} — powered by TritiumOS";
        return true;
    }

    void ShowAbout()
    {
        MessageBox.Show(
            $"{_assistantName}\n\nTritiumOS by Draco\n\nThe line tread between madness and genius.",
            "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    static string? Prompt(string title, string label)
    {
        using var f = new Form { Text = title, Width = 420, Height = 160, FormBorderStyle = FormBorderStyle.FixedDialog, StartPosition = FormStartPosition.CenterParent };
        var box = new TextBox { Left = 12, Top = 36, Width = 380 };
        var ok = new Button { Text = "OK", Left = 280, Top = 72, Width = 100, DialogResult = DialogResult.OK };
        f.Controls.Add(new Label { Text = label, Left = 12, Top = 12, AutoSize = true });
        f.Controls.Add(box);
        f.Controls.Add(ok);
        f.AcceptButton = ok;
        return f.ShowDialog() == DialogResult.OK ? box.Text : null;
    }

    void OnInputKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        var line = _input.Text.Trim();
        _input.Clear();
        if (line.Length == 0) return;
        HandleCommand(line);
    }

    void HandleCommand(string line)
    {
        Append($"> {line}" + Environment.NewLine);
        var parts = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var cmd = parts[0].ToLowerInvariant();
        switch (cmd)
        {
            case "help":
                Append("help | status | compute | compute-set | compute-test | core-path | rename | qwantum-search | qwantum-dump | load-core | drena-demo | rekiA-demo" + Environment.NewLine);
                Append("assimilate | bootstrap-host | full-stack-optimize | host-info | load-refined" + Environment.NewLine);
                Append("Any other input is sent to the TritiumForth VM (try '1 2 + .' or the demos)." + Environment.NewLine);
                break;
            case "compute":
                Append(FormatCompute() + Environment.NewLine);
                break;
            case "compute-set":
                if (parts.Length < 2) {
                    Append("compute-set aer_local | braket_local | braket_cloud | ibm_open" + Environment.NewLine);
                    break;
                }
                if (!SetComputeBackend(parts[1])) break;
                Append($"compute active -> {_compute.active}" + Environment.NewLine);
                break;
            case "compute-test":
                Append(RunComputeTest() + Environment.NewLine);
                break;
            case "qwantum-search":
                Append(QwantumHint("search"));
                break;
            case "qwantum-dump":
                Append(QwantumHint("dump"));
                break;
            case "status":
                Append($"product=TritiumOS creator=Draco assistant={_assistantName} bootstrapped={_bootstrapped} compute={_compute.active}" + Environment.NewLine);
                break;
            case "core-path":
                var core = Path.Combine(AppContext.BaseDirectory, "poly", "core", "boot.fs");
                if (!File.Exists(core)) core = Path.Combine(AppContext.BaseDirectory, "tritium.poly", "core", "boot.fs");
                Append(File.Exists(core) ? core : "(core not bundled yet — run from repo or rebuild)") ;
                Append(Environment.NewLine);
                break;
            case "rename":
                if (parts.Length < 2) { Append("rename <name>" + Environment.NewLine); break; }
                _assistantName = parts[1];
                File.WriteAllText(Path.Combine(UserEvolveDir(), "assistant-name.trit"), _assistantName);
                Text = $"{_assistantName} — powered by TritiumOS";
                Append($"renamed to {_assistantName}" + Environment.NewLine);
                break;
            case "load-core":
                _vm?.LoadCore();
                break;
            case "drena-demo":
                Append(_vm?.Evaluate("drena-demo") ?? "VM not ready");
                break;
            case "rekiA-demo":
                Append(_vm?.Evaluate("rekiA-demo") ?? "VM not ready");
                break;
            case "assimilate":
                Append(_vm?.Evaluate("assimilate") ?? "VM not ready");
                LoadRefinedModules(); // pick up any newly emitted refined modules from the assimilation
                // Extra: exercise a "host knowledge" neuron + rekiA to tie assimilation artifacts into the DRENA/REKIA engine
                try { _vm.Evaluate("99 1 drena-spawn"); _vm.Evaluate("host-evolve-dir swap drena-link"); _vm.Evaluate("host-evolve-dir rekiA-refine"); } catch { }
                break;
            case "bootstrap-host":
                Append(_vm?.Evaluate("bootstrap-host") ?? "VM not ready");
                LoadRefinedModules();
                break;
            case "full-stack-optimize":
                Append(_vm?.Evaluate("full-stack-optimize") ?? "VM not ready");
                LoadRefinedModules(); // newly created host-*.fs from the cycle become live immediately
                break;
            case "host-info":
                Append(_vm?.Evaluate("host-hw-info . cr host-evolve-dir . cr") ?? "VM not ready");
                break;
            case "load-refined":
                LoadRefinedModules();
                break;
            default:
                // Route unknown input (and normal Forth) to the VM
                if (_vm != null)
                {
                    var result = _vm.Evaluate(line);
                    if (!string.IsNullOrEmpty(result))
                        Append(result);
                }
                else
                {
                    Append($"[{_assistantName}] scaffold reply — connect R.E.K.I.A. next." + Environment.NewLine);
                }
                break;
        }
    }

    void Append(string s) => _log.AppendText(s);

    string FormatCompute()
    {
        var lines = new List<string> { $"active={_compute.active} test={ComputeConfig.ActiveTestProvider(_compute)} allow_qpu={_compute.allow_qpu} max_shots={_compute.max_shots}" };
        foreach (var kv in _compute.backends)
            lines.Add($"  {kv.Key}{(kv.Key == _compute.active ? " *" : "")} -> {kv.Value.test_provider} ({kv.Value.label})");
        lines.Add("Edit qd/compute.json or: compute-set braket_local");
        return string.Join(Environment.NewLine, lines);
    }

    bool SetComputeBackend(string id)
    {
        if (!_compute.backends.ContainsKey(id)) {
            Append($"unknown backend: {id}" + Environment.NewLine);
            return false;
        }
        if (id == "ibm_open" && !_compute.ibm_enabled) {
            Append("ibm_open disabled (ibm_enabled=false). Fix IBM instance first." + Environment.NewLine);
            return false;
        }
        _compute.active = id;
        SaveComputeConfig();
        return true;
    }

    void SaveComputeConfig()
    {
        var root = RepoRoot();
        var path = Path.Combine(root, "qd", "compute.json");
        if (!Directory.Exists(Path.GetDirectoryName(path)!))
            path = Path.Combine(AppContext.BaseDirectory, "qd", "compute.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var json = JsonSerializer.Serialize(_compute, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json, Encoding.UTF8);
        var polyStub = Path.Combine(root, "tritium.poly", "compute.json");
        if (Directory.Exists(Path.GetDirectoryName(polyStub)!))
        {
            var stub = new {
                version = _compute.version,
                active = _compute.active,
                allow_qpu = _compute.allow_qpu,
                max_shots = _compute.max_shots,
                ibm_enabled = _compute.ibm_enabled,
                doc = "See qd/compute.json"
            };
            File.WriteAllText(polyStub, JsonSerializer.Serialize(stub, new JsonSerializerOptions { WriteIndented = true }), Encoding.UTF8);
        }
    }

    static string RepoRoot()
    {
        // Walk upward from baseDir looking for dev repo marker (TritiumOS.txt or qd/compute.json.example)
        // Stops at filesystem root or when hitting a 'dist' folder (to avoid mistaking published layout)
        var dir = AppContext.BaseDirectory;
        for (int i = 0; i < 8; i++)
        {
            if (string.IsNullOrEmpty(dir)) break;
            if (Directory.Exists(Path.Combine(dir, "qd")) && File.Exists(Path.Combine(dir, "TritiumOS.txt")))
                return dir;
            var parent = Path.GetDirectoryName(dir);
            if (parent == null || parent == dir) break;
            // if we are in or under a dist/ from a build, don't claim it as repo root
            if (Path.GetFileName(dir).Equals("dist", StringComparison.OrdinalIgnoreCase)) break;
            dir = parent;
        }
        // Fallback: if qd present next to base use it (e.g. content copied in publish)
        if (Directory.Exists(Path.Combine(AppContext.BaseDirectory, "qd")))
            return AppContext.BaseDirectory;
        return AppContext.BaseDirectory;
    }

    static string UserEvolveDir()
    {
        var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dir = Path.Combine(local, "TritiumOS", "evolve");
        Directory.CreateDirectory(dir);
        return dir;
    }

    string RunComputeTest()
    {
        var root = RepoRoot();
        var ps1 = Path.Combine(root, "tools", "run-compute.ps1");
        if (!File.Exists(ps1))
            return "tools/run-compute.ps1 not found. Run from repo: .\\tools\\run-compute.ps1";
        try {
            var psi = new System.Diagnostics.ProcessStartInfo {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{ps1}\"",
                WorkingDirectory = root,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using var p = System.Diagnostics.Process.Start(psi);
            var o = p!.StandardOutput.ReadToEnd();
            var e = p.StandardError.ReadToEnd();
            p.WaitForExit(120000);
            return o + (string.IsNullOrEmpty(e) ? "" : "\nERR:\n" + e);
        } catch (Exception ex) {
            return "compute-test failed: " + ex.Message;
        }
    }

    static string QwantumHint(string action)
    {
        var root = RepoRoot();
        if (!Directory.Exists(Path.Combine(root, "tools")))
            root = AppContext.BaseDirectory;
        return action switch
        {
            "search" => $"Run in PowerShell:\n  {root}\\tools\\qwantum-search.ps1\nPaste prompt into Qwantum Compute.\n",
            "dump" => $"Save Qwantum reply, then:\n  {root}\\tools\\qwantum-dump.ps1 -InputPath <reply.txt> -Apply\n",
            _ => ""
        };
    }

    void InitForthVM()
    {
        _vm = new TritiumForthVM();
        _vm.SetOutputCallback(s => Append(s));

        // Evolve dir for persistence + assimilation/bootstrap artifacts (key for "forth to c#" host optimization loop)
        _vm.EvolveDir = UserEvolveDir();

        // Determine core directory (same logic as core-path, but for dir)
        string coreDir = FindCoreDir();
        _vm.CoreDir = coreDir;

        Append("[VM] TritiumForthVM created. CoreDir=" + coreDir + Environment.NewLine);

        // Auto-load the core sources (trit + kernel + drena + rekia)
        // This makes the DRENA data blocks and REKIA math available immediately.
        try
        {
            _vm.LoadCore();
        }
        catch (Exception ex)
        {
            Append("[VM] LoadCore error: " + ex.Message + Environment.NewLine);
        }

        // Set edition from what the user chose (32/64)
        // The kernel exposes set-edition
        try
        {
            // Read the edition.trit we wrote in RunFirstBoot
            var edFile = Path.Combine(UserEvolveDir(), "edition.trit");
            if (File.Exists(edFile))
            {
                var ed = File.ReadAllText(edFile).Trim();
                if (int.TryParse(ed, out int edition))
                {
                    _vm.Evaluate($" {edition} set-edition ");
                }
            }
        }
        catch { /* non fatal */ }

        // Call platform hook from kernel
        try { _vm.Evaluate("platform-init"); } catch { }

        // Quick self-test of the VM
        try
        {
            Append("[VM] Self-test: ");
            _vm.Evaluate("1 2 + . cr");
        }
        catch (Exception ex)
        {
            Append("[VM] Self-test error: " + ex.Message + Environment.NewLine);
        }

        Append("[VM] Ready. Type 'drena-demo' or 'rekiA-demo' or 'full-stack-optimize' or raw Forth. 'load-refined' to pull in emitted modules from prior assimilation.\n");

        // Auto-test the engines on boot for iteration/testing (remove or comment for production)
        try
        {
            Append("[VM] Auto-running engine tests for verification...\n");
            _vm.Evaluate("drena-demo");
            _vm.Evaluate("rekiA-demo");
            Append("[VM] Engine tests complete.\n");
        }
        catch (Exception ex)
        {
            Append("[VM] Auto-test failed: " + ex.Message + "\n");
        }

        // Demonstrate the new assimilation + host bootstrap capability (Forth intelligence inside C# driving full-stack host opt)
        // NOTE: Keep this light on auto-boot. Full dir scans + host-exec (systeminfo etc) can trigger AV heuristics.
        // User must have added folder exclusion (see README-Win11.md). Recommend manual "assimilate" / "full-stack-optimize" after boot.
        try
        {
            Append("[VM] Host bridge ready (forth-to-c# assimilation + full-stack host OS optimize).\n");
            _vm.Evaluate("host-hw-info . cr");
            _vm.Evaluate("host-evolve-dir . cr");
            Append("[VM] Try: 'assimilate' (ingest host software for this hardware), 'bootstrap-host', or 'full-stack-optimize' (chains engines+assimilate+bootstrap, writes to evolve/).\n");
            Append("[VM] (Requires AV exclusion on project + %LocalAppData%\\TritiumOS as assimilation uses host-exec + writes many .ingest/.ps1/.fs artifacts.)\n");
        }
        catch (Exception ex)
        {
            Append("[VM] Host bridge demo note: " + ex.Message + "\n");
        }

        // Auto-load any previously emitted refined modules (from prior assimilate/bootstrap cycles).
        // This makes the "forth to c#" assimilation produce persistent, loadable intelligence extensions.
        try { LoadRefinedModules(); } catch (Exception ex) { Append("[VM] LoadRefined note: " + ex.Message + "\n"); }
    }

    string FindCoreDir()
    {
        // Mirror the improved search from core-path / RepoRoot but return the dir containing the .fs files
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "poly", "core"),
            Path.Combine(AppContext.BaseDirectory, "tritium.poly", "core"),
            Path.Combine(RepoRoot(), "tritium.poly", "core"),
            Path.Combine(RepoRoot(), "poly", "core"),
        };

        foreach (var c in candidates)
        {
            if (Directory.Exists(c) && File.Exists(Path.Combine(c, "boot.fs")))
                return c;
        }

        // Last resort
        return Path.Combine(AppContext.BaseDirectory, "tritium.poly", "core");
    }

    void LoadRefinedModules()
    {
        if (_vm == null) return;
        var evolve = UserEvolveDir();
        var refinedDir = Path.Combine(evolve, "forth", "refined");
        if (!Directory.Exists(refinedDir)) return;

        var modules = Directory.GetFiles(refinedDir, "*.fs", SearchOption.TopDirectoryOnly)
            .OrderBy(f => f) // deterministic order
            .ToArray();

        if (modules.Length == 0) return;

        Append($"[VM] Loading refined modules from {refinedDir} ({modules.Length} files)...\n");
        foreach (var mod in modules)
        {
            try
            {
                var src = File.ReadAllText(mod);
                Append($"[VM]   include {Path.GetFileName(mod)}\n");
                _vm.Evaluate(src); // Interpret the emitted Forth (host bridges like host-hw-info etc are available)
            }
            catch (Exception ex)
            {
                Append($"[VM]   Failed to load {Path.GetFileName(mod)}: {ex.Message}\n");
            }
        }
        Append("[VM] Refined modules loaded. Intelligence extensions from assimilation/bootstrap are now active.\n");
    }
}


# write-probe-115125 
