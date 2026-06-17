using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace WinOptimizer;

public partial class ActivateDialog : Window
{
    public ActivateDialog(string command)
    {
        InitializeComponent();
        CmdText.Text = command;
    }

    private void OpenCredit(object sender, RequestNavigateEventArgs e)
    {
        try { Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true }); }
        catch { }
        e.Handled = true;
    }

    private void Confirmar(object sender, RoutedEventArgs e) => DialogResult = true;

    private void Cancelar(object sender, RoutedEventArgs e) => DialogResult = false;
}
