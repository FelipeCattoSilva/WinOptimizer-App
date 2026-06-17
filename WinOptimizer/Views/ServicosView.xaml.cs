using System.Windows;
using System.Windows.Controls;
using WinOptimizer.Models;
using WinOptimizer.Services;

namespace WinOptimizer.Views;

public partial class ServicosView : UserControl
{
    public ServicosView(AppState state)
    {
        InitializeComponent();
        // real system lists, loaded in the background
        LoadAsync();
    }

    private async void LoadAsync()
    {
        var startup = await Task.Run(ServicesService.LoadStartup);
        var services = await Task.Run(ServicesService.LoadServices);
        StartupList.ItemsSource = startup;
        ServiceList.ItemsSource = services;
    }

    private async void ToggleStartup(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox cb || cb.DataContext is not StartupItem item) return;
        bool alvo = !item.Ativo;
        cb.IsEnabled = false;
        bool ok = await Task.Run(() => ServicesService.SetStartup(item, alvo));
        cb.IsEnabled = true;
        cb.IsChecked = item.Ativo;
        if (!ok) Aviso(string.Format(Loc.T("svc_fail_startup"), item.Nome));
    }

    private async void ToggleService(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox cb || cb.DataContext is not ServiceItem svc) return;
        if (svc.Core) { cb.IsChecked = true; return; }

        bool ligar = !svc.EmExecucao;
        cb.IsEnabled = false;
        bool ok = await Task.Run(() => ServicesService.SetService(svc, ligar));
        cb.IsEnabled = true;
        cb.IsChecked = svc.EmExecucao;
        if (!ok) Aviso(string.Format(Loc.T("svc_fail_service"),
            Loc.T(ligar ? "svc_verb_start" : "svc_verb_stop"), svc.Nome));
    }

    private void Aviso(string msg)
        => MessageBox.Show(Window.GetWindow(this), msg, "WinOptimizer", MessageBoxButton.OK, MessageBoxImage.Warning);
}
