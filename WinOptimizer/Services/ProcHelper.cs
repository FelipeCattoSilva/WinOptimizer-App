using System.Diagnostics;

namespace WinOptimizer.Services;

// runs command-line processes (app is already elevated)
public static class ProcHelper
{
    // run and wait. returns (exit code, combined output)
    public static (int code, string output) Run(string exe, string args, int timeoutMs = 60000)
    {
        try
        {
            var psi = new ProcessStartInfo(exe, args)
            {
                RedirectStandardOutput = true, RedirectStandardError = true,
                UseShellExecute = false, CreateNoWindow = true
            };
            using var p = Process.Start(psi);
            if (p == null) return (-1, "falha ao iniciar");
            string outp = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
            if (!p.WaitForExit(timeoutMs)) { try { p.Kill(true); } catch { } return (-1, "timeout"); }
            return (p.ExitCode, outp);
        }
        catch (Exception ex) { return (-1, ex.Message); }
    }

    public static (int, string) PowerShell(string script)
        => Run("powershell.exe", $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{script}\"");

    public static (int, string) PowerCfg(string args) => Run("powercfg.exe", args);
}
