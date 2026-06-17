using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinOptimizer.Data;
using WinOptimizer.Models;
using WinOptimizer.Services;

namespace WinOptimizer.Views;

public partial class AppsView : UserControl
{
    private readonly List<AppEntry> _apps = AppCatalog.All();
    private bool _busy;
    private string _tab = "";
    private readonly Dictionary<string, (Border badge, TextBlock count)> _badges = new();

    public AppsView(AppState state)
    {
        InitializeComponent();

        foreach (var a in _apps) a.PropertyChanged += OnAppChanged;
        Unloaded += (_, _) => { foreach (var a in _apps) a.PropertyChanged -= OnAppChanged; };

        BuildTabs();
        UpdateButton();

        if (!WingetService.Disponivel())
        {
            SubText.Text = Loc.T("apps_no_winget");
            InstallBtn.IsEnabled = false;
        }
    }

    private void BuildTabs()
    {
        var cats = _apps.Select(a => a.Categoria).Distinct().ToList();
        bool first = true;
        foreach (var cat in cats)
        {
            var content = new StackPanel { Orientation = Orientation.Horizontal };
            content.Children.Add(new TextBlock { Text = Loc.Cat(cat), VerticalAlignment = VerticalAlignment.Center });

            var count = new TextBlock
            {
                FontSize = 10, FontWeight = FontWeights.Medium,
                Foreground = (Brush)Application.Current.Resources["PrimaryBrush"],
                VerticalAlignment = VerticalAlignment.Center
            };
            var badge = new Border
            {
                CornerRadius = new CornerRadius(8), Padding = new Thickness(6, 1, 6, 1),
                Margin = new Thickness(6, 0, 0, 0),
                Background = (Brush)Application.Current.Resources["PrimarySoftBrush"],
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = Visibility.Collapsed, Child = count
            };
            content.Children.Add(badge);
            _badges[cat] = (badge, count);

            var rb = new RadioButton
            {
                Style = (Style)FindResource("TabItemButton"),
                GroupName = "appcats", Content = content, Tag = cat, IsChecked = first
            };
            rb.Checked += (_, _) => { _tab = cat; Refresh(); };
            TabBar.Children.Add(rb);

            if (first) _tab = cat;
            first = false;
        }
        Refresh();
    }

    private void OnAppChanged(object? s, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppEntry.Selecionado)) { UpdateButton(); UpdateBadges(); }
    }

    private void Refresh()
    {
        List.ItemsSource = _apps.Where(a => a.Categoria == _tab).ToList();
        UpdateBadges();
    }

    private void UpdateBadges()
    {
        foreach (var (cat, (badge, count)) in _badges)
        {
            int n = _apps.Count(a => a.Categoria == cat && a.Selecionado);
            count.Text = n.ToString();
            badge.Visibility = n > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private List<AppEntry> Selecionados() => _apps.Where(a => a.Selecionado).ToList();

    private void UpdateButton()
    {
        int n = Selecionados().Count;
        InstallBtn.Content = string.Format(Loc.T("apps_install"), n);
        InstallBtn.IsEnabled = n > 0 && !_busy;
    }

    private async void Install(object sender, RoutedEventArgs e)
    {
        if (_busy) return;
        var alvos = Selecionados();
        if (alvos.Count == 0) return;

        _busy = true;
        UpdateButton();
        InstallBtn.Content = Loc.T("apps_installing");
        LogText.Text = "";

        foreach (var app in alvos)
        {
            app.Status = AppStatus.Instalando;
            Log($"› {app.Nome} ({app.WingetId})");

            var r = await WingetService.InstalarAsync(app.WingetId, Log);

            if (r == WingetService.Resultado.HashMismatch && PerguntaBypassHash(app.Nome))
            {
                Log($"  ⚠ ignorando verificação de hash de {app.Nome}...");
                r = await WingetService.InstalarAsync(app.WingetId, Log, ignoreHash: true);
            }

            bool ok = r == WingetService.Resultado.Ok;
            app.Status = ok ? AppStatus.Instalado : AppStatus.Falhou;
            Log(ok ? $"  ✓ {app.Nome} instalado" : $"  ✕ {app.Nome} falhou");
        }

        _busy = false;
        UpdateButton();
    }

    private bool PerguntaBypassHash(string nome)
    {
        var r = MessageBox.Show(Window.GetWindow(this),
            string.Format(Loc.T("apps_hash_msg"), nome),
            Loc.T("apps_hash_title"),
            MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
        return r == MessageBoxResult.Yes;
    }

    private void Log(string line)
    {
        Dispatcher.Invoke(() =>
        {
            LogText.Text += (LogText.Text.Length == 0 ? "" : "\n") + line;
            LogScroller.ScrollToEnd();
        });
    }
}
