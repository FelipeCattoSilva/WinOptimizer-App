using System.Windows;
using System.Windows.Controls;
using WinOptimizer.Data;
using WinOptimizer.Models;

namespace WinOptimizer.Views;

public partial class PresetsView : UserControl
{
    private readonly AppState _state;

    public PresetsView(AppState state)
    {
        InitializeComponent();
        _state = state;
        List.ItemsSource = Presets.All();
    }

    private void OpenPreset(object sender, RoutedEventArgs e)
    {
        if (sender is not Button b || b.Tag is not PresetDef preset) return;

        var dlg = new PresetDialog(preset, _state) { Owner = Window.GetWindow(this) };
        if (dlg.ShowDialog() == true)
        {
            AppliedText.Text = preset.Limpar
                ? Loc.T("preset_cleared")
                : string.Format(Loc.T("preset_applied"), preset.Nome);
            AppliedBanner.Visibility = Visibility.Visible;
        }
    }
}
