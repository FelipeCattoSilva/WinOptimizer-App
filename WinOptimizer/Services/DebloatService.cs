namespace WinOptimizer.Services;

// removes appx packages via powershell (all users + provisioned)
public static class DebloatService
{
    public static bool Remover(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern)) return false;

        // remove for all users + the provisioned copy (would reinstall on new accounts)
        var script =
            $"Get-AppxPackage -AllUsers '*{pattern}*' | Remove-AppxPackage -AllUsers -ErrorAction SilentlyContinue; " +
            $"Get-AppxProvisionedPackage -Online | Where-Object {{ $_.DisplayName -like '*{pattern}*' }} | " +
            "Remove-AppxProvisionedPackage -Online -ErrorAction SilentlyContinue; " +
            $"if (Get-AppxPackage -AllUsers '*{pattern}*') {{ exit 1 }} else {{ exit 0 }}";

        var (code, _) = ProcHelper.PowerShell(script);
        return code == 0;
    }
}
