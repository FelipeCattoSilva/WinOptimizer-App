using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WinOptimizer.Models;

namespace WinOptimizer.Views;

public partial class CreditsView : UserControl
{
    private record Credito(string Nome, string Autor, string Desc, string Url);

    public CreditsView(AppState state)
    {
        InitializeComponent();

        string D(string en, string pt) => Loc.I.IsEn ? en : pt;

        List.ItemsSource = new[]
        {
            new Credito("WinUtil", "ChrisTitusTech",
                D("The biggest reference in this space. Splits tweaks into Essential/Advanced; its docs shaped the tabs in this app.",
                  "Maior referência da categoria. Organiza tweaks em Essential/Advanced; a doc oficial montou a base das abas deste app."),
                "https://github.com/ChrisTitusTech/winutil"),
            new Credito("WinUtil Docs", "christitus.com",
                D("Full list of tweaks, features and fixes used as reference.",
                  "Lista completa de tweaks, features e fixes usada como referência."),
                "https://winutil.christitus.com"),
            new Credito("Win11Debloat", "Raphire",
                D("Focused debloat, works on Win10 and Win11. Inspired the applied-tweak detection and the Appx list.",
                  "Debloat focado, funciona em Win10 e Win11. Inspirou a detecção de tweaks já aplicados e a lista de Appx."),
                "https://github.com/Raphire/Win11Debloat"),
            new Credito("Optimizer", "hellzerg",
                D("Classic optimizer (discontinued), a reference for privacy and performance tweaks.",
                  "Otimizador clássico (descontinuado), referência em tweaks de privacidade e desempenho."),
                "https://github.com/hellzerg/optimizer"),
            new Credito("OptimizerNXT", "hellzerg",
                D("Successor to Optimizer, now CLI-based.",
                  "Sucessor do Optimizer, agora em CLI."),
                "https://github.com/hellzerg/optimizerNXT"),
            new Credito("privacy.sexy", "undergroundwires",
                D("Cross-platform, focused on reversible and transparent scripts. Basis for many privacy tweaks.",
                  "Multiplataforma, foco em scripts reversíveis e transparentes. Base de muitos tweaks de privacidade."),
                "https://github.com/undergroundwires/privacy.sexy"),
            new Credito("O&O ShutUp10++", "O&O Software",
                D("Not open source, but a reference for risk-badged toggle UX (Recommended/Limited/Not recommended).",
                  "Não é open source, mas referência em UX de toggle com selo de risco (Recomendado/Limitado/Não recomendado)."),
                "https://www.oo-software.com/en/shutup10"),
        };
    }

    private const string ReleasesUrl = "https://github.com/FelipeCattoSilva/WinOptimizer-App/releases";

    private void OpenLink(object sender, RequestNavigateEventArgs e)
    {
        try { Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true }); }
        catch { }
        e.Handled = true;
    }

    private void OpenReleases(object sender, RoutedEventArgs e)
    {
        try { Process.Start(new ProcessStartInfo(ReleasesUrl) { UseShellExecute = true }); }
        catch { }
    }
}
