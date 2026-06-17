using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;

namespace WinOptimizer.Services;

// reads real cpu/ram/disk/net/gpu plus a system summary
public sealed class SystemInfoService : IDisposable
{
    private readonly PerformanceCounter? _cpu;
    private readonly PerformanceCounter? _disk;
    private PerformanceCounter[] _net = Array.Empty<PerformanceCounter>();
    private PerformanceCounter[] _gpu = Array.Empty<PerformanceCounter>();

    public SystemInfoService()
    {
        try { _cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total"); } catch { }
        try { _disk = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total"); } catch { }
        try
        {
            var cat = new PerformanceCounterCategory("Network Interface");
            _net = cat.GetInstanceNames()
                      .Select(n => new PerformanceCounter("Network Interface", "Bytes Total/sec", n))
                      .ToArray();
        }
        catch { }
        try
        {
            var cat = new PerformanceCounterCategory("GPU Engine");
            _gpu = cat.GetInstanceNames()
                      .Where(n => n.Contains("engtype_3D"))
                      .Select(n => new PerformanceCounter("GPU Engine", "Utilization Percentage", n))
                      .ToArray();
        }
        catch { }
    }

    public int Cpu() => Read(_cpu);
    public int Disk() => Math.Min(100, Read(_disk));

    public int Ram()
    {
        var m = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
        return GlobalMemoryStatusEx(ref m) ? (int)m.dwMemoryLoad : 0;
    }

    // network usage in mbps (instant)
    public int NetMbps()
    {
        double bytes = 0;
        foreach (var c in _net) { try { bytes += c.NextValue(); } catch { } }
        return (int)(bytes * 8 / 1_000_000);
    }

    public int Gpu()
    {
        double sum = 0;
        foreach (var c in _gpu) { try { sum += c.NextValue(); } catch { } }
        return Math.Min(100, (int)sum);
    }

    private static int Read(PerformanceCounter? c)
    {
        if (c == null) return 0;
        try { return (int)Math.Round(c.NextValue()); } catch { return 0; }
    }

    // key->value rows for the system summary
    public static List<(string, string)> Summary()
    {
        // returns (loc-key, value) - the view localizes the key
        var rows = new List<(string, string)>();
        string os = Wmi("Win32_OperatingSystem", "Caption") ?? "Windows";
        rows.Add(("sum_system", os.Replace("Microsoft ", "").Trim()));
        rows.Add(("sum_cpu", Wmi("Win32_Processor", "Name")?.Trim() ?? "—"));
        rows.Add(("sum_mem", RamSummary()));
        rows.Add(("sum_gpu", Wmi("Win32_VideoController", "Name")?.Trim() ?? "—"));
        rows.Add(("sum_storage", DiskSummary()));
        rows.Add(("sum_uptime", Uptime()));
        return rows;
    }

    private static string RamSummary()
    {
        var m = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
        if (!GlobalMemoryStatusEx(ref m)) return "—";
        double gb = m.ullTotalPhys / (1024.0 * 1024 * 1024);
        return $"{gb:0} GB";
    }

    private static string DiskSummary()
    {
        try
        {
            var d = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory) ?? "C:\\");
            double totalGb = d.TotalSize / (1024.0 * 1024 * 1024);
            double freeGb = d.AvailableFreeSpace / (1024.0 * 1024 * 1024);
            return string.Format(Loc.T("storage_fmt"), $"{totalGb:0}", $"{freeGb:0}");
        }
        catch { return "—"; }
    }

    private static string Uptime()
    {
        var up = TimeSpan.FromMilliseconds(Environment.TickCount64);
        return up.Days > 0 ? $"{up.Days}d {up.Hours}h" : $"{up.Hours}h {up.Minutes}min";
    }

    private static string? Wmi(string cls, string prop)
    {
        try
        {
            using var s = new ManagementObjectSearcher($"SELECT {prop} FROM {cls}");
            foreach (var o in s.Get())
            {
                var v = o[prop]?.ToString();
                if (!string.IsNullOrWhiteSpace(v)) return v;
            }
        }
        catch { }
        return null;
    }

    public void Dispose()
    {
        _cpu?.Dispose(); _disk?.Dispose();
        foreach (var c in _net) c.Dispose();
        foreach (var c in _gpu) c.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MEMORYSTATUSEX
    {
        public uint dwLength, dwMemoryLoad;
        public ulong ullTotalPhys, ullAvailPhys, ullTotalPageFile, ullAvailPageFile,
                     ullTotalVirtual, ullAvailVirtual, ullAvailExtendedVirtual;
    }

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
}
