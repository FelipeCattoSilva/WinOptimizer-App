namespace WinOptimizer.Services;

// system restore point via powershell
public static class RestoreService
{
    public static (bool ok, string msg) Criar(string descricao)
    {
        // make sure system protection is on for C:
        ProcHelper.PowerShell("Enable-ComputerRestore -Drive 'C:\\'");
        var (code, outp) = ProcHelper.PowerShell(
            $"Checkpoint-Computer -Description '{descricao}' -RestorePointType 'MODIFY_SETTINGS'");

        if (code == 0) return (true, "Ponto de restauração criado.");

        // windows allows one restore point per 24h by default
        if (outp.Contains("1440") || outp.Contains("frequency", StringComparison.OrdinalIgnoreCase))
            return (false, "Já existe um ponto de restauração criado nas últimas 24h.");

        return (false, "Falha ao criar ponto de restauração. Verifique se a Proteção do Sistema está ativa.");
    }
}
