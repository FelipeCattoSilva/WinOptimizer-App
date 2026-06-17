using System.Diagnostics;

namespace WinOptimizer.Services;

// installs apps via winget (already elevated by the manifest)
public static class WingetService
{
    // true if winget is on PATH
    public static bool Disponivel()
    {
        try
        {
            var psi = new ProcessStartInfo("winget", "--version")
            {
                RedirectStandardOutput = true, RedirectStandardError = true,
                UseShellExecute = false, CreateNoWindow = true
            };
            using var p = Process.Start(psi);
            if (p == null) return false;
            p.WaitForExit(5000);
            return p.ExitCode == 0;
        }
        catch { return false; }
    }

    public enum Resultado { Ok, HashMismatch, Falhou }

    // installs an app. ignoreHash skips the installer integrity check
    public static async Task<Resultado> InstalarAsync(string wingetId, Action<string> log, bool ignoreHash = false)
    {
        var args = $"install --id {wingetId} -e --silent " +
                   "--accept-package-agreements --accept-source-agreements" +
                   (ignoreHash ? " --ignore-security-hash" : "");

        var psi = new ProcessStartInfo("winget", args)
        {
            RedirectStandardOutput = true, RedirectStandardError = true,
            UseShellExecute = false, CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8
        };

        var buffer = new System.Text.StringBuilder();
        void Capture(string? d)
        {
            if (string.IsNullOrWhiteSpace(d)) return;
            buffer.AppendLine(d);
            log(d);
        }

        try
        {
            using var p = new Process { StartInfo = psi };
            p.OutputDataReceived += (_, e) => Capture(e.Data);
            p.ErrorDataReceived += (_, e) => Capture(e.Data);

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            await p.WaitForExitAsync();

            if (p.ExitCode == 0) return Resultado.Ok;

            // winget fails when the installer hash doesn't match the manifest
            string outp = buffer.ToString().ToLowerInvariant();
            bool hashMismatch = outp.Contains("hash") && (outp.Contains("match") || outp.Contains("corresponde"));
            return hashMismatch && !ignoreHash ? Resultado.HashMismatch : Resultado.Falhou;
        }
        catch (Exception ex)
        {
            log($"erro: {ex.Message}");
            return Resultado.Falhou;
        }
    }
}
