using WinOptimizer.Models;

namespace WinOptimizer.Data;

// catalog of popular apps installable via winget
public static class AppCatalog
{
    public static List<AppEntry> All() => new()
    {
        // browsers
        A("Google Chrome", "browsers", "Google.Chrome"),
        A("Brave", "browsers", "Brave.Brave"),
        A("Mozilla Firefox", "browsers", "Mozilla.Firefox"),

        // comms
        A("Discord", "comms", "Discord.Discord"),
        A("TeamSpeak 3", "comms", "TeamSpeakSystems.TeamSpeakClient"),
        A("Telegram", "comms", "Telegram.TelegramDesktop"),
        A("WhatsApp", "comms", "9NKSQGP7F2NH"),

        // gaming
        A("Steam", "gaming", "Valve.Steam"),
        A("Epic Games Launcher", "gaming", "EpicGames.EpicGamesLauncher"),
        A("EA app", "gaming", "ElectronicArts.EADesktop"),
        A("Ubisoft Connect", "gaming", "Ubisoft.Connect"),

        // drivers / hardware
        A("NVIDIA App", "drivers", "Nvidia.GeForceExperience"),

        // media
        A("Spotify", "media", "Spotify.Spotify"),
        A("VLC", "media", "VideoLAN.VLC"),
        A("OBS Studio", "media", "OBSProject.OBSStudio"),

        // utilities
        A("Lightshot", "utilities", "Skillbrains.Lightshot"),
        A("TinyTask", "utilities", "Balanced.TinyTask"),
        A("7-Zip", "utilities", "7zip.7zip"),
        A("Notepad++", "utilities", "Notepad++.Notepad++"),
        A("PowerToys", "utilities", "Microsoft.PowerToys"),
        A("qBittorrent", "utilities", "qBittorrent.qBittorrent"),
        A("VS Code", "utilities", "Microsoft.VisualStudioCode"),
        A("WinRAR", "utilities", "RARLab.WinRAR"),
    };

    private static AppEntry A(string nome, string cat, string winget)
        => new() { Id = winget, Nome = nome, Categoria = cat, WingetId = winget };
}
