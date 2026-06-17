using System.ComponentModel;
using System.Runtime.CompilerServices;
using WinOptimizer;
using WinOptimizer.Data;

namespace WinOptimizer.Models;

// global session state - detected version + live collections
public class AppState
{
    public WindowsVersion Version { get; set; }
    public List<TweakItem> Tweaks { get; } = SeedData.Tweaks();
    public List<AppxPackage> Appx { get; } = SeedData.Appx();
    public List<ServiceItem> Services { get; } = SeedData.Services();
    public List<StartupItem> Startup { get; } = SeedData.Startup();
    public List<CleanupCategory> Cleanup { get; } = SeedData.Cleanup();
}

public enum WindowsVersion { Windows10, Windows11 }

public enum RiscoTweak { Seguro, Moderado, Avancado }

// inotifypropertychanged base so toggle bindings update the ui
public abstract class Observable : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnChanged(name);
        return true;
    }
}

public class TweakItem : Observable
{
    public string Id { get; set; } = "";
    public string TituloEn { get; set; } = "";
    public string TituloPt { get; set; } = "";
    public string DescricaoEn { get; set; } = "";
    public string DescricaoPt { get; set; } = "";
    public string Categoria { get; set; } = "";   // language-neutral key
    public WindowsVersion? AlvoOS { get; set; }   // null = both
    public bool RequerReinicio { get; set; }
    public RiscoTweak Risco { get; set; }

    public string Titulo => Loc.I.IsEn ? TituloEn : TituloPt;
    public string Descricao => Loc.I.IsEn ? DescricaoEn : DescricaoPt;
    public string CategoriaLabel => Loc.Cat(Categoria);

    private bool _ativo;
    public bool Ativo { get => _ativo; set => Set(ref _ativo, value); }
}

public class AppxPackage : Observable
{
    public string Id { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Publisher { get; set; } = "";
    public WindowsVersion? AlvoOS { get; set; }
    public bool SeguroRemover { get; set; }
    public string Pattern { get; set; } = "";   // appx package name pattern (remove-appxpackage)

    private bool _selecionado;
    public bool Selecionado { get => _selecionado; set => Set(ref _selecionado, value); }

    private bool _removido;
    public bool Removido { get => _removido; set { if (Set(ref _removido, value)) OnChanged(nameof(Habilitado)); } }

    public bool Habilitado => SeguroRemover && !Removido; // protected/removed apps stay non-interactive
}

public enum ServiceStatus { EmExecucao, Parado }
public enum StartupType { Automatico, Manual, Desabilitado }

public class ServiceItem : Observable
{
    public string Id { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Descricao { get; set; } = "";
    public bool Core { get; set; }

    private ServiceStatus _status;
    public ServiceStatus Status { get => _status; set { Set(ref _status, value); OnChanged(nameof(EmExecucao)); } }

    private StartupType _startup;
    public StartupType Startup { get => _startup; set => Set(ref _startup, value); }

    public bool EmExecucao => Status == ServiceStatus.EmExecucao;
    public bool NaoCore => !Core;  // core services are protected from toggling
}

public enum ImpactoStartup { Alto, Medio, Baixo }

public class StartupItem : Observable
{
    public string Id { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Publisher { get; set; } = "";
    public ImpactoStartup Impacto { get; set; }
    public string Hive { get; set; } = "HKCU";   // which hive holds the run key
    public string ValueName { get; set; } = "";   // value name under the run key

    private bool _ativo;
    public bool Ativo { get => _ativo; set => Set(ref _ativo, value); }
}

public class CleanupCategory : Observable
{
    public string Id { get; set; } = "";
    public string NomeEn { get; set; } = "";
    public string NomePt { get; set; } = "";
    public string DescricaoEn { get; set; } = "";
    public string DescricaoPt { get; set; } = "";
    public string Nome => Loc.I.IsEn ? NomeEn : NomePt;
    public string Descricao => Loc.I.IsEn ? DescricaoEn : DescricaoPt;

    private int _tamanhoMb;
    public int TamanhoMb { get => _tamanhoMb; set => Set(ref _tamanhoMb, value); }

    private bool _selecionado;
    public bool Selecionado { get => _selecionado; set => Set(ref _selecionado, value); }
}

public enum AppStatus { Aguardando, Instalando, Instalado, Falhou }

// app installable via winget
public class AppEntry : Observable
{
    public string Id { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Categoria { get; set; } = "";   // language-neutral key
    public string WingetId { get; set; } = "";

    public string CategoriaLabel => Loc.Cat(Categoria);

    private bool _selecionado;
    public bool Selecionado { get => _selecionado; set => Set(ref _selecionado, value); }

    private AppStatus _status = AppStatus.Aguardando;
    public AppStatus Status { get => _status; set => Set(ref _status, value); }
}

// predefined tweak bundle. selector picks which tweaks the preset turns on
public class PresetDef
{
    public string Id { get; set; } = "";
    public string Icone { get; set; } = "";
    public string NomeEn { get; set; } = "";
    public string NomePt { get; set; } = "";
    public string ResumoEn { get; set; } = "";
    public string ResumoPt { get; set; } = "";
    public string[] MelhoriasEn { get; set; } = Array.Empty<string>();
    public string[] MelhoriasPt { get; set; } = Array.Empty<string>();

    public string Nome => Loc.I.IsEn ? NomeEn : NomePt;
    public string Resumo => Loc.I.IsEn ? ResumoEn : ResumoPt;
    public string[] Melhorias => Loc.I.IsEn ? MelhoriasEn : MelhoriasPt;

    public Func<TweakItem, bool> Selector { get; set; } = _ => false;
    public bool Limpar { get; set; }   // true = turn everything off instead of on
}
