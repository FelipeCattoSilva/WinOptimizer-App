using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinOptimizer.Models;

namespace WinOptimizer.Views;

public partial class TweaksView : UserControl
{
    private readonly AppState _state;
    private bool _avancado;   // false = essential, true = advanced
    private readonly Dictionary<bool, (Border badge, TextBlock count)> _badges = new();

    public TweaksView(AppState state)
    {
        InitializeComponent();
        _state = state;

        BuildTabs();
        foreach (var t in _state.Tweaks) t.PropertyChanged += OnTweakChanged;
        Unloaded += (_, _) => { foreach (var t in _state.Tweaks) t.PropertyChanged -= OnTweakChanged; };

        Refresh();
    }

    private void BuildTabs()
    {
        AddTab(Loc.T("tweaks_essential"), false, true);
        AddTab(Loc.T("tweaks_advanced"), true, false);
    }

    private void AddTab(string label, bool avancado, bool first)
    {
        var content = new StackPanel { Orientation = Orientation.Horizontal };
        content.Children.Add(new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center });

        var count = new TextBlock
        {
            FontSize = 10,
            FontWeight = FontWeights.Medium,
            Foreground = (Brush)Application.Current.Resources["PrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center
        };
        var badge = new Border
        {
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(6, 1, 6, 1),
            Margin = new Thickness(6, 0, 0, 0),
            Background = (Brush)Application.Current.Resources["PrimarySoftBrush"],
            VerticalAlignment = VerticalAlignment.Center,
            Visibility = Visibility.Collapsed,
            Child = count
        };
        content.Children.Add(badge);
        _badges[avancado] = (badge, count);

        var rb = new RadioButton
        {
            Style = (Style)FindResource("TabItemButton"),
            GroupName = "tweaktabs",
            Content = content,
            Tag = avancado,
            IsChecked = first
        };
        rb.Checked += (_, _) => { _avancado = avancado; Refresh(); };
        TabBar.Children.Add(rb);
    }

    private void OnTweakChanged(object? s, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TweakItem.Ativo)) UpdateBadges();
    }

    private bool VersaoOk(TweakItem t) => t.AlvoOS == null || t.AlvoOS == _state.Version;

    private bool EhAvancado(TweakItem t) => t.Risco == RiscoTweak.Avancado;

    private void Refresh()
    {
        var visiveis = _state.Tweaks
            .Where(t => VersaoOk(t) && EhAvancado(t) == _avancado)
            .OrderBy(t => t.Categoria)
            .ToList();

        List.ItemsSource = visiveis;
        EmptyState.Visibility = visiveis.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        WarnBox.Visibility = _avancado ? Visibility.Visible : Visibility.Collapsed;
        UpdateBadges();
    }

    private void UpdateBadges()
    {
        foreach (var avancado in new[] { false, true })
        {
            int n = _state.Tweaks.Count(t => VersaoOk(t) && EhAvancado(t) == avancado && t.Ativo);
            var (badge, count) = _badges[avancado];
            count.Text = n.ToString();
            badge.Visibility = n > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
