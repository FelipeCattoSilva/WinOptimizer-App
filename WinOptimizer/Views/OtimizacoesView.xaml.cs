using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinOptimizer.Data;
using WinOptimizer.Models;

namespace WinOptimizer.Views;

public partial class OtimizacoesView : UserControl
{
    private readonly AppState _state;
    private string _tab = SeedData.Categorias[0];
    private readonly Dictionary<string, (Border badge, TextBlock count)> _badges = new();

    public OtimizacoesView(AppState state)
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
        bool first = true;
        foreach (var cat in SeedData.Categorias)
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
                GroupName = "tabs", Content = content, Tag = cat, IsChecked = first
            };
            rb.Checked += (_, _) => { _tab = cat; Refresh(); };
            TabBar.Children.Add(rb);
            first = false;
        }
    }

    private void OnTweakChanged(object? s, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TweakItem.Ativo)) UpdateBadges();
    }

    private bool Matches(TweakItem t, string cat)
        => t.Categoria == cat && (t.AlvoOS == null || t.AlvoOS == _state.Version);

    private void Refresh()
    {
        var visiveis = _state.Tweaks.Where(t => Matches(t, _tab)).ToList();
        List.ItemsSource = visiveis;
        EmptyState.Visibility = visiveis.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        UpdateBadges();
    }

    private void UpdateBadges()
    {
        foreach (var cat in SeedData.Categorias)
        {
            int n = _state.Tweaks.Count(t => Matches(t, cat) && t.Ativo);
            var (badge, count) = _badges[cat];
            count.Text = n.ToString();
            badge.Visibility = n > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
