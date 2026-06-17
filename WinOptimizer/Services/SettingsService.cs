using System.IO;
using System.Text.Json;
using WinOptimizer;
using WinOptimizer.Models;

namespace WinOptimizer.Services;

// persists toggle/selection state to %AppData%\WinOptimizer\settings.json
public static class SettingsService
{
    private static string Dir =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinOptimizer");
    private static string File_ => Path.Combine(Dir, "settings.json");

    private class Snapshot
    {
        public string Language { get; set; } = "En";
        public Dictionary<string, bool> Tweaks { get; set; } = new();
        public Dictionary<string, bool> Appx { get; set; } = new();
        public Dictionary<string, bool> Cleanup { get; set; } = new();
    }

    public static void Load(AppState state)
    {
        try
        {
            if (!File.Exists(File_)) return;
            var snap = JsonSerializer.Deserialize<Snapshot>(File.ReadAllText(File_));
            if (snap == null) return;

            Loc.I.Current = snap.Language == "Pt" ? WinOptimizer.Lang.Pt : WinOptimizer.Lang.En;

            foreach (var t in state.Tweaks)
                if (snap.Tweaks.TryGetValue(t.Id, out var v)) t.Ativo = v;
            foreach (var a in state.Appx)
                if (snap.Appx.TryGetValue(a.Id, out var v)) a.Selecionado = v;
            foreach (var c in state.Cleanup)
                if (snap.Cleanup.TryGetValue(c.Id, out var v)) c.Selecionado = v;
        }
        catch { /* ignore corrupt config */ }
    }

    public static void Save(AppState state)
    {
        try
        {
            Directory.CreateDirectory(Dir);
            var snap = new Snapshot
            {
                Language = Loc.I.Current == WinOptimizer.Lang.Pt ? "Pt" : "En",
                Tweaks = state.Tweaks.ToDictionary(t => t.Id, t => t.Ativo),
                Appx = state.Appx.ToDictionary(a => a.Id, a => a.Selecionado),
                Cleanup = state.Cleanup.ToDictionary(c => c.Id, c => c.Selecionado),
            };
            File.WriteAllText(File_, JsonSerializer.Serialize(snap, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch { }
    }
}
