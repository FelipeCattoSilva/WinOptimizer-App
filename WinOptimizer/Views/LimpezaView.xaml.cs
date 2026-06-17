using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WinOptimizer.Models;
using WinOptimizer.Services;

namespace WinOptimizer.Views;

public partial class LimpezaView : UserControl
{
    private readonly AppState _state;
    private bool _busy;

    public LimpezaView(AppState state)
    {
        InitializeComponent();
        _state = state;
        List.ItemsSource = _state.Cleanup;

        foreach (var c in _state.Cleanup) c.PropertyChanged += OnItemChanged;
        Unloaded += (_, _) => { foreach (var c in _state.Cleanup) c.PropertyChanged -= OnItemChanged; };

        UpdateTotal();
        ScanAsync();
    }

    private void OnItemChanged(object? s, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CleanupCategory.Selecionado)) UpdateTotal();
    }

    // measure the real size of each category in the background
    private async void ScanAsync()
    {
        var items = _state.Cleanup.ToList();
        foreach (var c in items)
        {
            long bytes = await Task.Run(() => CleanupService.Scan(c.Id));
            c.TamanhoMb = (int)(bytes / (1024 * 1024));
        }
        UpdateTotal();
    }

    private int Total() => _state.Cleanup.Where(i => i.Selecionado).Sum(i => i.TamanhoMb);

    private void UpdateTotal()
    {
        int total = Total();
        TotalText.Text = Fmt(total);
        CleanBtn.IsEnabled = total > 0 && !_busy;
    }

    private async void Clean(object sender, RoutedEventArgs e)
    {
        if (_busy) return;
        _busy = true;
        CleanBtn.IsEnabled = false;
        CleanBtn.Content = Loc.T("cleanup_cleaning");

        var alvos = _state.Cleanup.Where(i => i.Selecionado).ToList();
        long freedBytes = 0;
        foreach (var c in alvos)
        {
            freedBytes += await Task.Run(() => CleanupService.Clean(c.Id));
            c.TamanhoMb = (int)(await Task.Run(() => CleanupService.Scan(c.Id)) / (1024 * 1024));
        }

        FreedText.Text = string.Format(Loc.T("cleanup_done"), FmtBytes(freedBytes));
        FreedBanner.Visibility = Visibility.Visible;

        CleanBtn.Content = Loc.T("cleanup_now");
        _busy = false;
        UpdateTotal();
    }

    private static string Fmt(int mb) => mb >= 1024 ? $"{mb / 1024.0:0.0} GB" : $"{mb} MB";

    private static string FmtBytes(long b)
    {
        double mb = b / (1024.0 * 1024.0);
        return mb >= 1024 ? $"{mb / 1024.0:0.00} GB" : $"{mb:0} MB";
    }
}
