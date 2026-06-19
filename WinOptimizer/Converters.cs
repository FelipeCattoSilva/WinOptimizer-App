using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using WinOptimizer.Models;
using WinOptimizer.Services;

namespace WinOptimizer;

public class LogLevelBrushConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value switch
    {
        LogLevel.Action => Res.B("PrimaryBrush"),
        LogLevel.Exec => Res.B("ForegroundBrush"),
        LogLevel.Reg => Res.B("AccentBrush"),
        LogLevel.Result => Res.B("SuccessBrush"),
        LogLevel.Warn => Res.B("WarningBrush"),
        LogLevel.Error => Res.B("DestructiveBrush"),
        _ => Res.B("MutedForegroundBrush")
    };
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

static class Res
{
    public static Brush B(string key) => (Brush)Application.Current.Resources[key];
}

public class RiscoLabelConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value switch
    {
        RiscoTweak.Seguro => Loc.T("risk_safe"),
        RiscoTweak.Moderado => Loc.T("risk_moderate"),
        RiscoTweak.Avancado => Loc.T("risk_advanced"),
        _ => ""
    };
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

public class RiscoForegroundConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value switch
    {
        RiscoTweak.Seguro => Res.B("SuccessBrush"),
        RiscoTweak.Moderado => Res.B("WarningBrush"),
        RiscoTweak.Avancado => Res.B("DestructiveBrush"),
        _ => Res.B("MutedForegroundBrush")
    };
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

public class RiscoBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value switch
    {
        RiscoTweak.Seguro => Res.B("SuccessSoftBrush"),
        RiscoTweak.Moderado => Res.B("WarningSoftBrush"),
        RiscoTweak.Avancado => Res.B("DestructiveSoftBrush"),
        _ => Res.B("SecondaryBrush")
    };
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

public class BoolToVisConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
    {
        bool b = value is bool x && x;
        if (p as string == "invert") b = !b;
        return b ? Visibility.Visible : Visibility.Collapsed;
    }
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

public class NullableOsToVisConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => value is WindowsVersion ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

public class OsLabelConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value switch
    {
        WindowsVersion.Windows10 => "Win 10",
        WindowsVersion.Windows11 => "Win 11",
        _ => ""
    };
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

public class ServiceStatusTextConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => (value is ServiceStatus s && s == ServiceStatus.EmExecucao) ? Loc.T("svc_running") : Loc.T("svc_stopped");
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

public class ServiceStatusBrushConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => (value is ServiceStatus s && s == ServiceStatus.EmExecucao)
            ? Res.B("SuccessBrush") : Res.B("MutedForegroundBrush");
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

public class StartupTextConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value switch
    {
        StartupType.Automatico => Loc.T("startup_auto"),
        StartupType.Manual => Loc.T("startup_manual"),
        StartupType.Desabilitado => Loc.T("startup_disabled"),
        _ => ""
    };
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

public class ImpactoTextConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value switch
    {
        ImpactoStartup.Alto => Loc.T("impact_high"),
        ImpactoStartup.Medio => Loc.T("impact_medium"),
        ImpactoStartup.Baixo => Loc.T("impact_low"),
        _ => ""
    };
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

public class ImpactoBrushConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value switch
    {
        ImpactoStartup.Alto => Res.B("DestructiveBrush"),
        ImpactoStartup.Medio => Res.B("WarningBrush"),
        ImpactoStartup.Baixo => Res.B("SuccessBrush"),
        _ => Res.B("MutedForegroundBrush")
    };
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

// mb -> "x.y GB" or "x MB"
public class MbConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
    {
        int mb = value is int i ? i : 0;
        return mb >= 1024 ? $"{mb / 1024.0:0.0} GB" : $"{mb} MB";
    }
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

public class AppStatusTextConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value switch
    {
        AppStatus.Aguardando => "",
        AppStatus.Instalando => Loc.T("app_installing"),
        AppStatus.Instalado => Loc.T("app_installed"),
        AppStatus.Falhou => Loc.T("app_failed"),
        _ => ""
    };
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

public class AppStatusBrushConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value switch
    {
        AppStatus.Instalando => Res.B("WarningBrush"),
        AppStatus.Instalado => Res.B("SuccessBrush"),
        AppStatus.Falhou => Res.B("DestructiveBrush"),
        _ => Res.B("MutedForegroundBrush")
    };
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}

public class IniciaisConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
    {
        var s = value as string ?? "";
        return (s.Length >= 2 ? s[..2] : s).ToUpperInvariant();
    }
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
}
