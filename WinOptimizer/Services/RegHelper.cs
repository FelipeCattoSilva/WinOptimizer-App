using Microsoft.Win32;

namespace WinOptimizer.Services;

// small wrapper to read/write the registry. path format "HKLM\Sub\Key"
public static class RegHelper
{
    private static (RegistryKey root, string sub) Split(string path)
    {
        int i = path.IndexOf('\\');
        string hive = path[..i];
        string sub = path[(i + 1)..];
        RegistryKey root = hive.ToUpperInvariant() switch
        {
            "HKLM" or "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
            "HKCU" or "HKEY_CURRENT_USER" => Registry.CurrentUser,
            "HKCR" or "HKEY_CLASSES_ROOT" => Registry.ClassesRoot,
            _ => throw new ArgumentException($"hive desconhecido: {hive}")
        };
        return (root, sub);
    }

    public static void SetDword(string path, string name, int value)
    {
        var (root, sub) = Split(path);
        using var k = root.CreateSubKey(sub, true);
        k.SetValue(name, value, RegistryValueKind.DWord);
    }

    public static void SetString(string path, string name, string value)
    {
        var (root, sub) = Split(path);
        using var k = root.CreateSubKey(sub, true);
        k.SetValue(name, value, RegistryValueKind.String);
    }

    public static object? Get(string path, string name)
    {
        var (root, sub) = Split(path);
        using var k = root.OpenSubKey(sub, false);
        return k?.GetValue(name);
    }

    public static int? GetDword(string path, string name)
        => Get(path, name) is int i ? i : null;

    public static void DeleteValue(string path, string name)
    {
        var (root, sub) = Split(path);
        using var k = root.OpenSubKey(sub, true);
        try { k?.DeleteValue(name, false); } catch { }
    }

    public static void DeleteKey(string path)
    {
        var (root, sub) = Split(path);
        try { root.DeleteSubKeyTree(sub, false); } catch { }
    }

    public static void CreateKey(string path)
    {
        var (root, sub) = Split(path);
        using var _ = root.CreateSubKey(sub, true);
    }
}
