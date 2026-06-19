using System.Diagnostics;

namespace WinOptimizer.Services;

// runs command-line processes (app is already elevated)
public static class ProcHelper
{
    // run and wait. returns (exit code, combined output)
    public static (int code, string output) Run(string exe, string args, int timeoutMs = 60000)
    {
        Log.Exec($"{exe} {args}".Trim());
        try
        {
            var psi = new ProcessStartInfo(exe, args)
            {
                RedirectStandardOutput = true, RedirectStandardError = true,
                UseShellExecute = false, CreateNoWindow = true
            };
            using var p = Process.Start(psi);
            if (p == null) { Log.Error($"{exe}: failed to start"); return (-1, "falha ao iniciar"); }
            string outp = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
            if (!p.WaitForExit(timeoutMs)) { try { p.Kill(true); } catch { } Log.Warn($"{exe}: timeout"); return (-1, "timeout"); }
            if (p.ExitCode == 0) Log.Result($"↳ exit 0");
            else Log.Warn($"↳ exit {p.ExitCode}{FirstLine(outp)}");
            return (p.ExitCode, outp);
        }
        catch (Exception ex) { Log.Error($"{exe}: {ex.Message}"); return (-1, ex.Message); }
    }

    private static string FirstLine(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        var line = s.Trim().Split('\n')[0].Trim();
        return line.Length == 0 ? "" : "  " + (line.Length > 120 ? line[..120] + "…" : line);
    }

    public static (int, string) PowerShell(string script)
        => Run("powershell.exe", $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{script}\"");

    public static (int, string) PowerCfg(string args) => Run("powercfg.exe", args);
}
