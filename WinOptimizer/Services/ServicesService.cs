using System.IO;
using System.ServiceProcess;
using Microsoft.Win32;
using WinOptimizer.Models;

namespace WinOptimizer.Services;

// real windows services + startup items
public static class ServicesService
{
    // core services the app refuses to disable
    private static readonly HashSet<string> Core = new(StringComparer.OrdinalIgnoreCase)
    {
        "wuauserv", "BITS", "Dnscache", "RpcSs", "RpcEptMapper", "DcomLaunch", "LSM",
        "Schedule", "EventLog", "mpssvc", "WinDefend", "ProfSvc", "Power", "PlugPlay",
        "Themes", "AudioSrv", "Audiosrv", "CryptSvc", "Dhcp", "nsi", "BFE", "gpsvc",
        "SamSs", "Winmgmt", "UserManager", "CoreMessagingRegistrar"
    };

    // services
    public static List<ServiceItem> LoadServices()
    {
        var list = new List<ServiceItem>();
        ServiceController[] svcs;
        try { svcs = ServiceController.GetServices(); } catch { return list; }

        foreach (var sc in svcs)
        {
            try
            {
                list.Add(new ServiceItem
                {
                    Id = sc.ServiceName,
                    Nome = sc.ServiceName,
                    Descricao = string.IsNullOrWhiteSpace(sc.DisplayName) ? sc.ServiceName : sc.DisplayName,
                    Status = sc.Status == ServiceControllerStatus.Running ? ServiceStatus.EmExecucao : ServiceStatus.Parado,
                    Startup = ReadStartType(sc.ServiceName),
                    Core = Core.Contains(sc.ServiceName)
                });
            }
            catch { }
            finally { sc.Dispose(); }
        }
        return list.OrderByDescending(s => s.EmExecucao).ThenBy(s => s.Descricao).ToList();
    }

    private static StartupType ReadStartType(string name)
    {
        int? start = RegHelper.GetDword($@"HKLM\SYSTEM\CurrentControlSet\Services\{name}", "Start");
        return start switch { 2 => StartupType.Automatico, 4 => StartupType.Desabilitado, _ => StartupType.Manual };
    }

    // enable, or stop + disable, a service
    public static bool SetService(ServiceItem item, bool ligar)
    {
        try
        {
            if (ligar)
            {
                ProcHelper.Run("sc", $"config {item.Nome} start= demand");
                using var sc = new ServiceController(item.Nome);
                if (sc.Status != ServiceControllerStatus.Running)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
                item.Status = ServiceStatus.EmExecucao;
                item.Startup = StartupType.Manual;
            }
            else
            {
                using (var sc = new ServiceController(item.Nome))
                {
                    if (sc.CanStop && sc.Status == ServiceControllerStatus.Running)
                    {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    }
                }
                ProcHelper.Run("sc", $"config {item.Nome} start= disabled");
                item.Status = ServiceStatus.Parado;
                item.Startup = StartupType.Desabilitado;
            }
            return true;
        }
        catch { return false; }
    }

    // startup
    private const string RunCU = @"HKCU\Software\Microsoft\Windows\CurrentVersion\Run";
    private const string RunLM = @"HKLM\Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ApprovedCU = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
    private const string ApprovedLM = ApprovedCU;

    public static List<StartupItem> LoadStartup()
    {
        var list = new List<StartupItem>();
        ReadRun(Registry.CurrentUser, "HKCU", RunCU, list);
        ReadRun(Registry.LocalMachine, "HKLM", RunLM, list);
        return list;
    }

    private static void ReadRun(RegistryKey hive, string hiveName, string runPath, List<StartupItem> list)
    {
        using var run = hive.OpenSubKey(runPath[(runPath.IndexOf('\\') + 1)..], false);
        if (run == null) return;
        using var approved = hive.OpenSubKey(ApprovedCU, false);

        foreach (var name in run.GetValueNames())
        {
            if (string.IsNullOrWhiteSpace(name)) continue;
            string cmd = run.GetValue(name)?.ToString() ?? "";
            bool ativo = IsEnabled(approved, name);
            list.Add(new StartupItem
            {
                Id = hiveName + ":" + name,
                Nome = name,
                ValueName = name,
                Hive = hiveName,
                Publisher = Trim(cmd),
                Impacto = ImpactoStartup.Medio,
                Ativo = ativo
            });
        }
    }

    private static bool IsEnabled(RegistryKey? approved, string name)
    {
        if (approved?.GetValue(name) is byte[] b && b.Length > 0)
            return (b[0] & 0x01) == 0; // 0x02 = enabled, 0x03 = disabled
        return true;
    }

    // enable/disable a startup item (via StartupApproved)
    public static bool SetStartup(StartupItem item, bool enable)
    {
        try
        {
            var hive = item.Hive == "HKLM" ? Registry.LocalMachine : Registry.CurrentUser;
            using var k = hive.CreateSubKey(ApprovedCU, true);
            var data = new byte[12];
            data[0] = (byte)(enable ? 0x02 : 0x03);
            k.SetValue(item.ValueName, data, RegistryValueKind.Binary);
            item.Ativo = enable;
            return true;
        }
        catch { return false; }
    }

    private static string Trim(string cmd)
    {
        cmd = cmd.Trim('"', ' ');
        int exe = cmd.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
        if (exe > 0) cmd = cmd[..(exe + 4)];
        try { return Path.GetFileName(cmd); } catch { return cmd; }
    }
}
