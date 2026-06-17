using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinOptimizer.Models;
using WinOptimizer.Services;
using WinOptimizer.Views;

namespace WinOptimizer;

public partial class MainWindow : Window
{
    private const string ReleasesUrl = "https://github.com/FelipeCattoSilva/WinOptimizer-App/releases";

    private readonly AppState _state;
    private string _section = "dashboard";

    // nav section -> meta key prefix
    private static readonly Dictionary<string, string> MetaKey = new()
    {
        ["dashboard"] = "dashboard",
        ["otimizacoes"] = "optimizations",
        ["tweaks"] = "tweaks",
        ["presets"] = "presets",
        ["apps"] = "apps",
        ["limpeza"] = "cleanup",
        ["servicos"] = "services",
        ["debloat"] = "debloat",
        ["creditos"] = "credits",
    };

    public MainWindow(AppState state)
    {
        InitializeComponent();
        _state = state;

        var v = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);
        TbVersion.Text = $"v{v.Major}.{v.Minor}.{v.Build}";

        string ver = state.Version == WindowsVersion.Windows10 ? "Windows 10" : "Windows 11";
        TbTargetVersion.Text = ver;

        if (Loc.I.IsPt) LangPt.IsChecked = true; else LangEn.IsChecked = true;
        Loc.LanguageChanged += OnLanguageChanged;

        Show("dashboard");
    }

    private void Nav_Checked(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded) return;
        if (sender is RadioButton rb && rb.Tag is string tag) Show(tag);
    }

    private void Show(string section)
    {
        _section = section;
        UpdateMeta();

        ContentHost.Content = section switch
        {
            "dashboard" => new DashboardView(_state),
            "otimizacoes" => new OtimizacoesView(_state),
            "tweaks" => new TweaksView(_state),
            "presets" => new PresetsView(_state),
            "apps" => new AppsView(_state),
            "limpeza" => new LimpezaView(_state),
            "servicos" => new ServicosView(_state),
            "debloat" => new DebloatView(_state),
            "creditos" => new CreditsView(_state),
            _ => null
        };
    }

    private void UpdateMeta()
    {
        string k = MetaKey.TryGetValue(_section, out var v) ? v : _section;
        TbTitle.Text = Loc.T($"meta_{k}_t");
        TbDesc.Text = Loc.T($"meta_{k}_d");
    }

    // ---- language ----
    private void Lang_Checked(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded) return;
        if (sender is RadioButton rb && rb.Tag is string tag)
            Loc.I.Current = tag == "Pt" ? Lang.Pt : Lang.En;
    }

    private void OnLanguageChanged()
    {
        if (!IsLoaded) return;
        UpdateMeta();
        Show(_section);              // rebuild current view in the new language
        SettingsService.Save(_state);
    }

    // ---- global search ----
    private void Search_Changed(object sender, TextChangedEventArgs e)
    {
        string q = SearchBox.Text.Trim();
        SearchHint.Visibility = q.Length == 0 ? Visibility.Visible : Visibility.Collapsed;

        if (q.Length == 0) { SearchPopup.IsOpen = false; return; }

        var hits = _state.Tweaks.Where(t => Match(t, q)).Take(25).ToList();
        SearchResults.ItemsSource = hits;
        SearchEmpty.Visibility = hits.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        SearchPopup.IsOpen = true;
    }

    private static bool Match(TweakItem t, string q)
    {
        return t.TituloEn.Contains(q, StringComparison.OrdinalIgnoreCase)
            || t.TituloPt.Contains(q, StringComparison.OrdinalIgnoreCase)
            || t.DescricaoEn.Contains(q, StringComparison.OrdinalIgnoreCase)
            || t.DescricaoPt.Contains(q, StringComparison.OrdinalIgnoreCase)
            || t.Id.Contains(q, StringComparison.OrdinalIgnoreCase);
    }

    private async void RestorePoint(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        btn.IsEnabled = false;
        btn.Content = Loc.T("restore_creating");

        var (ok, msg) = await Task.Run(() => RestoreService.Criar("WinOptimizer"));

        btn.Content = Loc.T("create_restore_point");
        btn.IsEnabled = true;
        MessageBox.Show(this, msg, Loc.T("restore_title"),
            MessageBoxButton.OK, ok ? MessageBoxImage.Information : MessageBoxImage.Warning);
    }

    protected override void OnClosed(EventArgs e)
    {
        Loc.LanguageChanged -= OnLanguageChanged;
        SettingsService.Save(_state);   // persist selection in %AppData%
        base.OnClosed(e);
    }

    // window chrome
    private void TitleBar_Drag(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2) { MaxRestore(sender, e); return; }
        if (e.ButtonState == MouseButtonState.Pressed) DragMove();
    }

    private void OpenReleases(object sender, RoutedEventArgs e)
    {
        try { Process.Start(new ProcessStartInfo(ReleasesUrl) { UseShellExecute = true }); }
        catch { }
    }

    private void Minimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void MaxRestore(object sender, RoutedEventArgs e)
        => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void CloseWin(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
}
