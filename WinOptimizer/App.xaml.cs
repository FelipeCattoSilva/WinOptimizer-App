using System.ComponentModel;
using System.Windows;
using WinOptimizer.Models;
using WinOptimizer.Services;

namespace WinOptimizer;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // auto-detect os, no picker screen
        var state = new AppState { Version = DetectWindowsVersion() };

        // load saved selection before subscribing, so startup doesn't re-apply everything
        SettingsService.Load(state);

        // from here on, each toggle applies/reverts on the real system
        foreach (var t in state.Tweaks)
            t.PropertyChanged += OnTweakToggled;

        new MainWindow(state).Show();
    }

    private void OnTweakToggled(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(TweakItem.Ativo) || sender is not TweakItem t) return;
        bool ativo = t.Ativo;
        // keep registry/service/powercfg work off the ui thread
        Task.Run(() => { try { TweakService.Apply(t, ativo); } catch { } });
    }

    // win11 = build >= 22000, anything lower is treated as win10
    private static WindowsVersion DetectWindowsVersion()
        => Environment.OSVersion.Version.Build >= 22000
            ? WindowsVersion.Windows11
            : WindowsVersion.Windows10;
}
