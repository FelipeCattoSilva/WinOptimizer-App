using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WinOptimizer.Models;
using WinOptimizer.Services;

namespace WinOptimizer.Views;

public partial class DebloatView : UserControl
{
    private readonly AppState _state;
    private List<AppxPackage> _visiveis = new();

    public DebloatView(AppState state)
    {
        InitializeComponent();
        _state = state;

        HeaderTitle.Text = Loc.T("debloat_prefix") +
            (state.Version == WindowsVersion.Windows10 ? "Windows 10" : "Windows 11");

        _visiveis = _state.Appx.Where(p => p.AlvoOS == null || p.AlvoOS == state.Version).ToList();
        List.ItemsSource = _visiveis;

        foreach (var p in _visiveis) p.PropertyChanged += OnPkgChanged;
        Unloaded += (_, _) => { foreach (var p in _visiveis) p.PropertyChanged -= OnPkgChanged; };

        UpdateButton();
    }

    private void OnPkgChanged(object? s, PropertyChangedEventArgs e) => UpdateButton();

    private List<AppxPackage> Selecionados()
        => _visiveis.Where(p => p.Selecionado && !p.Removido).ToList();

    private void UpdateButton()
    {
        int n = Selecionados().Count;
        RemoverBtn.Content = string.Format(Loc.T("debloat_remove"), n);
        RemoverBtn.IsEnabled = n > 0;
    }

    private void PresetSeguro(object sender, RoutedEventArgs e)
    {
        foreach (var p in _visiveis)
            if (!p.Removido) p.Selecionado = p.SeguroRemover;
    }

    private async void Remover(object sender, RoutedEventArgs e)
    {
        var alvos = Selecionados();
        RemoverBtn.IsEnabled = false;
        RemoverBtn.Content = Loc.T("debloat_removing");

        foreach (var p in alvos)
        {
            bool ok = await Task.Run(() => DebloatService.Remover(p.Pattern));
            if (ok) { p.Removido = true; p.Selecionado = false; }
        }

        UpdateButton();
    }
}
