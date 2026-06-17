using System.Windows;
using System.Windows.Media;
using WinOptimizer.Models;

namespace WinOptimizer;

public partial class PresetDialog : Window
{
    private readonly AppState _state;
    private readonly PresetDef _preset;
    private readonly List<TweakItem> _afetados;

    public PresetDialog(PresetDef preset, AppState state)
    {
        InitializeComponent();
        _state = state;
        _preset = preset;

        Icone.Text = preset.Icone;
        Nome.Text = preset.Nome;
        Resumo.Text = preset.Resumo;
        MelhoriasList.ItemsSource = preset.Melhorias;

        // tweaks valid for the current windows version
        var validos = _state.Tweaks.Where(t => t.AlvoOS == null || t.AlvoOS == _state.Version);

        if (preset.Limpar)
        {
            _afetados = validos.Where(t => t.Ativo).ToList();
            AfetadosTitle.Text = Loc.T("pd_will_disable");
            ApplyBtn.Content = Loc.T("pd_clear_all");
        }
        else
        {
            _afetados = validos.Where(preset.Selector).ToList();
        }

        AfetadosList.ItemsSource = _afetados;
        EmptyAfetados.Visibility = _afetados.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        ApplyBtn.IsEnabled = _afetados.Count > 0;

        CountText.Text = string.Format(Loc.T("pd_count"), _afetados.Count);

        bool reinicio = _afetados.Any(t => t.RequerReinicio);
        RestartBadge.Visibility = reinicio ? Visibility.Visible : Visibility.Collapsed;

        BuildRiskBadge();
    }

    private void BuildRiskBadge()
    {
        // highest risk among the affected tweaks
        RiscoTweak max = _afetados.Count == 0 ? RiscoTweak.Seguro : _afetados.Max(t => t.Risco);
        var conv = (RiscoLabelConverter)Application.Current.Resources["RiscoLabel"];
        var fg = (RiscoForegroundConverter)Application.Current.Resources["RiscoFg"];
        var bg = (RiscoBackgroundConverter)Application.Current.Resources["RiscoBg"];
        var ci = System.Globalization.CultureInfo.CurrentCulture;

        RiskText.Text = Loc.T("pd_risk") + (string)conv.Convert(max, typeof(string), null!, ci);
        RiskText.Foreground = (Brush)fg.Convert(max, typeof(Brush), null!, ci);
        RiskBadge.Background = (Brush)bg.Convert(max, typeof(Brush), null!, ci);
    }

    private void Aplicar(object sender, RoutedEventArgs e)
    {
        if (_preset.Limpar)
            foreach (var t in _state.Tweaks) t.Ativo = false;
        else
            foreach (var t in _afetados) t.Ativo = true;

        DialogResult = true;
    }

    private void Cancelar(object sender, RoutedEventArgs e) => DialogResult = false;
}
