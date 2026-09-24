using System;
using System.IO;
using TritiumOS;

/// <summary>
/// Headless smoke for wave4 item 3: TritiumForthVM.S0AssistDemo without WinForms.
/// Run: dotnet run --project install/hosts/windows/smoke-s0
/// Expect: [s0-assist-demo] OK — refined written; word live
/// </summary>
static class Program
{
    static int Main()
    {
        var evolve = Path.Combine(Path.GetTempPath(), "tritiumos-s0-smoke-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evolve);
        try
        {
            var vm = new TritiumForthVM();
            vm.EvolveDir = evolve;
            // No WinForms callback — capture return string.
            var output = vm.S0AssistDemo();
            Console.Write(output);
            bool ok = output.Contains("[s0-assist-demo] OK — refined written; word live", StringComparison.Ordinal)
                && File.Exists(Path.Combine(evolve, "forth", "refined", "refined-1.fs"))
                && !output.Contains("parity print", StringComparison.OrdinalIgnoreCase)
                && !output.Contains("Example response", StringComparison.OrdinalIgnoreCase)
                && !output.Contains("scaffold", StringComparison.OrdinalIgnoreCase);
            if (!ok)
            {
                Console.Error.WriteLine("[s0-assist-smoke] FAIL");
                return 1;
            }
            Console.WriteLine("[s0-assist-smoke] OK — host twin S0AssistDemo green (net8.0 headless)");
            return 0;
        }
        finally
        {
            try { Directory.Delete(evolve, recursive: true); } catch { }
        }
    }
}
