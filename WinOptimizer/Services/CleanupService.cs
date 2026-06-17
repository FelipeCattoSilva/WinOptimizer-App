using System.IO;
using System.Runtime.InteropServices;

namespace WinOptimizer.Services;

// real disk cleanup. each category maps to a concrete target.
// best-effort: files in use are skipped without aborting the batch.
public static class CleanupService
{
    private static string Win => Environment.GetEnvironmentVariable("windir") ?? @"C:\Windows";
    private static string LocalAppData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    // current size of the category, in bytes
    public static long Scan(string id) => id switch
    {
        "cl-temp-user" => DirSize(Path.GetTempPath()),
        "cl-temp-win" => DirSize(Path.Combine(Win, "Temp")),
        "cl-wu-cache" => DirSize(Path.Combine(Win, "SoftwareDistribution", "Download")),
        "cl-recycle" => RecycleBinSize(),
        "cl-thumbnails" => GlobSize(Path.Combine(LocalAppData, @"Microsoft\Windows\Explorer"), "thumbcache_*.db", "iconcache_*.db"),
        "cl-logs" => DirSize(Path.Combine(Win, "Logs"), olderThanDays: 30),
        _ => 0
    };

    // run the cleanup. returns bytes freed
    public static long Clean(string id) => id switch
    {
        "cl-temp-user" => PurgeDir(Path.GetTempPath()),
        "cl-temp-win" => PurgeDir(Path.Combine(Win, "Temp")),
        "cl-wu-cache" => PurgeDir(Path.Combine(Win, "SoftwareDistribution", "Download")),
        "cl-recycle" => EmptyRecycleBin(),
        "cl-thumbnails" => PurgeGlob(Path.Combine(LocalAppData, @"Microsoft\Windows\Explorer"), "thumbcache_*.db", "iconcache_*.db"),
        "cl-logs" => PurgeDir(Path.Combine(Win, "Logs"), olderThanDays: 30),
        _ => 0
    };

    // size helpers
    private static long DirSize(string dir, int olderThanDays = 0)
    {
        if (!Directory.Exists(dir)) return 0;
        long total = 0;
        var cutoff = DateTime.Now.AddDays(-olderThanDays);
        foreach (var f in SafeFiles(dir))
        {
            try
            {
                var fi = new FileInfo(f);
                if (olderThanDays > 0 && fi.LastWriteTime > cutoff) continue;
                total += fi.Length;
            }
            catch { }
        }
        return total;
    }

    private static long GlobSize(string dir, params string[] patterns)
    {
        if (!Directory.Exists(dir)) return 0;
        long total = 0;
        foreach (var pat in patterns)
            foreach (var f in SafeEnumerate(dir, pat))
                try { total += new FileInfo(f).Length; } catch { }
        return total;
    }

    // delete helpers
    private static long PurgeDir(string dir, int olderThanDays = 0)
    {
        if (!Directory.Exists(dir)) return 0;
        long freed = 0;
        var cutoff = DateTime.Now.AddDays(-olderThanDays);

        foreach (var f in SafeFiles(dir))
        {
            try
            {
                var fi = new FileInfo(f);
                if (olderThanDays > 0 && fi.LastWriteTime > cutoff) continue;
                long len = fi.Length;
                fi.Attributes = FileAttributes.Normal;
                fi.Delete();
                freed += len;
            }
            catch { /* file in use -> skip */ }
        }

        // remove empty subfolders (only when wiping everything)
        if (olderThanDays == 0)
            foreach (var sub in SafeDirs(dir))
                try { Directory.Delete(sub, true); } catch { }

        return freed;
    }

    private static long PurgeGlob(string dir, params string[] patterns)
    {
        if (!Directory.Exists(dir)) return 0;
        long freed = 0;
        foreach (var pat in patterns)
            foreach (var f in SafeEnumerate(dir, pat))
            {
                try
                {
                    var fi = new FileInfo(f);
                    long len = fi.Length;
                    fi.Attributes = FileAttributes.Normal;
                    fi.Delete();
                    freed += len;
                }
                catch { }
            }
        return freed;
    }

    // safe enumeration (skips folders we can't access)
    private static IEnumerable<string> SafeFiles(string root)
    {
        var stack = new Stack<string>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            var dir = stack.Pop();
            string[] subs = Array.Empty<string>(), files = Array.Empty<string>();
            try { subs = Directory.GetDirectories(dir); } catch { }
            try { files = Directory.GetFiles(dir); } catch { }
            foreach (var s in subs) stack.Push(s);
            foreach (var f in files) yield return f;
        }
    }

    private static IEnumerable<string> SafeDirs(string root)
    {
        try { return Directory.GetDirectories(root); }
        catch { return Array.Empty<string>(); }
    }

    private static IEnumerable<string> SafeEnumerate(string dir, string pattern)
    {
        try { return Directory.GetFiles(dir, pattern); }
        catch { return Array.Empty<string>(); }
    }

    // recycle bin via shell
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    private struct SHQUERYRBINFO { public int cbSize; public long i64Size; public long i64NumItems; }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHQueryRecycleBin(string? pszRootPath, ref SHQUERYRBINFO pSHQueryRBInfo);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);

    private const uint SHERB_NOCONFIRMATION = 0x1, SHERB_NOPROGRESSUI = 0x2, SHERB_NOSOUND = 0x4;

    private static long RecycleBinSize()
    {
        var info = new SHQUERYRBINFO { cbSize = Marshal.SizeOf<SHQUERYRBINFO>() };
        return SHQueryRecycleBin(null, ref info) == 0 ? info.i64Size : 0;
    }

    private static long EmptyRecycleBin()
    {
        long before = RecycleBinSize();
        SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
        return before;
    }
}
