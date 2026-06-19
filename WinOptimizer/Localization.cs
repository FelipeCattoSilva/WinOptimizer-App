using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Markup;

namespace WinOptimizer;

public enum Lang { En, Pt }

/// <summary>app localization. default english, switchable to portuguese at runtime.</summary>
public sealed class Loc : INotifyPropertyChanged
{
    public static Loc I { get; } = new();
    public event PropertyChangedEventHandler? PropertyChanged;

    // fires after the language actually changes - views rebuild on this
    public static event Action? LanguageChanged;

    private Lang _lang = Lang.En;
    public Lang Current
    {
        get => _lang;
        set
        {
            if (_lang == value) return;
            _lang = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEn)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPt)));
            LanguageChanged?.Invoke();
        }
    }

    public bool IsEn => _lang == Lang.En;
    public bool IsPt => _lang == Lang.Pt;

    // xaml binds {Binding [key], Source={x:Static l:Loc.I}} (usually via the Tr extension)
    public string this[string key] => Get(key);

    public static string T(string key) => Get(key);
    public static string Cat(string key) => Get("cat_" + key);

    private static string Get(string key)
        => Table.TryGetValue(key, out var p) ? (I._lang == Lang.En ? p.en : p.pt) : key;

    private static readonly Dictionary<string, (string en, string pt)> Table = new()
    {
        // ---- nav (icon embedded, language-neutral) ----
        ["nav_dashboard"] = ("⊞   Dashboard", "⊞   Dashboard"),
        ["nav_optimizations"] = ("✦   Optimizations", "✦   Otimizações"),
        ["nav_tweaks"] = ("⚙   Tweaks", "⚙   Tweaks"),
        ["nav_presets"] = ("⚡   Presets", "⚡   Presets"),
        ["nav_apps"] = ("⬇   Install Apps", "⬇   Instalar Apps"),
        ["nav_cleanup"] = ("🗑   Cleanup", "🗑   Limpeza"),
        ["nav_services"] = ("⏻   Startup & Services", "⏻   Inicialização & Serviços"),
        ["nav_debloat"] = ("📦   Debloat", "📦   Debloat"),
        ["nav_logs"] = ("📜   Logs", "📜   Logs"),
        ["nav_credits"] = ("★   Credits", "★   Créditos"),

        // ---- title bar / shell ----
        ["admin"] = ("Administrator", "Administrador"),
        ["activate"] = ("⚡  Activate", "⚡  Ativar"),
        ["activate_title"] = ("Activate Windows / Office", "Ativar Windows / Office"),
        ["activate_summary"] = (
            "This opens PowerShell as Administrator and runs the activation script.",
            "Isso abre o PowerShell como Administrador e executa o script de ativação."),
        ["activate_admin_badge"] = ("🛡 Administrator", "🛡 Administrador"),
        ["activate_visible_badge"] = ("👁 Visible window", "👁 Janela visível"),
        ["activate_cmd_label"] = ("Command that will run", "Comando que será executado"),
        ["activate_credit_label"] = ("Credits", "Créditos"),
        ["activate_credit_text"] = (
            "Microsoft Activation Scripts (MAS) — open source project by massgrave.dev",
            "Microsoft Activation Scripts (MAS) — projeto open source do massgrave.dev"),
        ["activate_continue"] = ("⚡  Activate now", "⚡  Ativar agora"),
        ["detected_version"] = ("Detected version", "Versão detectada"),
        ["create_restore_point"] = ("↺  Create restore point", "↺  Criar ponto de restauração"),
        ["restore_creating"] = ("↺  Creating...", "↺  Criando..."),
        ["restore_title"] = ("Restore point", "Ponto de restauração"),
        ["search_placeholder"] = ("Search functions...", "Pesquisar funções..."),
        ["search_no_results"] = ("No functions found.", "Nenhuma função encontrada."),

        // ---- section meta ----
        ["meta_dashboard_t"] = ("Dashboard", "Dashboard"),
        ["meta_dashboard_d"] = ("Real-time CPU, memory, disk and network monitoring.", "Monitoramento de CPU, memória, disco e rede em tempo real."),
        ["meta_optimizations_t"] = ("Optimizations", "Otimizações"),
        ["meta_optimizations_d"] = ("Registry and service tweaks, grouped by category.", "Tweaks de registro e serviços, organizados por categoria."),
        ["meta_tweaks_t"] = ("Tweaks", "Tweaks"),
        ["meta_tweaks_d"] = ("winutil-style list: Essential and Advanced split by risk.", "Lista no estilo winutil: Essenciais e Avançado separados por risco."),
        ["meta_presets_t"] = ("Presets", "Presets"),
        ["meta_presets_d"] = ("Ready-made optimization bundles. Click to see what each does.", "Conjuntos prontos de otimização. Clique para ver o que cada um faz."),
        ["meta_apps_t"] = ("Install Apps", "Instalar Apps"),
        ["meta_apps_d"] = ("Install popular apps at once via winget.", "Instale apps populares de uma vez via winget."),
        ["meta_cleanup_t"] = ("Cleanup", "Limpeza"),
        ["meta_cleanup_d"] = ("Remove temp files, cache and recycle bin to free space.", "Remova arquivos temporários, cache e lixeira para liberar espaço."),
        ["meta_services_t"] = ("Startup & Services", "Inicialização & Serviços"),
        ["meta_services_d"] = ("Manage what starts with Windows and system services.", "Gerencie o que inicia com o Windows e os serviços do sistema."),
        ["meta_debloat_t"] = ("Debloat", "Debloat"),
        ["meta_debloat_d"] = ("Remove pre-installed Appx apps selectively and safely.", "Remova apps Appx pré-instalados de forma seletiva e segura."),
        ["meta_credits_t"] = ("Credits", "Créditos"),
        ["meta_credits_d"] = ("Open source projects that inspired this app.", "Projetos open source que inspiraram este app."),
        ["meta_logs_t"] = ("Logs", "Logs"),
        ["meta_logs_d"] = ("Everything the app runs on the system, in real time.", "Tudo que o app executa no sistema, em tempo real."),

        // ---- logs ----
        ["log_dock_title"] = ("Live activity", "Atividade ao vivo"),
        ["log_clear"] = ("Clear", "Limpar"),
        ["log_copy"] = ("Copy all", "Copiar tudo"),
        ["log_copied"] = ("Copied!", "Copiado!"),
        ["log_empty"] = ("Nothing run yet. Actions appear here as they execute.",
                         "Nada executado ainda. As ações aparecem aqui conforme rodam."),
        ["log_autoscroll"] = ("Auto-scroll", "Rolagem automática"),

        // ---- categories ----
        ["cat_performance"] = ("Performance", "Desempenho"),
        ["cat_privacy"] = ("Privacy", "Privacidade"),
        ["cat_interface"] = ("Interface", "Interface"),
        ["cat_network"] = ("Network", "Rede"),
        ["cat_power"] = ("Power", "Energia"),
        ["cat_gaming"] = ("Gaming", "Jogos"),
        ["cat_personalization"] = ("Personalization", "Personalização"),
        ["cat_advanced"] = ("Advanced", "Avançado"),
        ["cat_features"] = ("Windows Features", "Recursos do Windows"),
        ["cat_fixes"] = ("Fixes", "Correções"),
        ["cat_browsers"] = ("Browsers", "Navegadores"),
        ["cat_comms"] = ("Communication", "Comunicação"),
        ["cat_drivers"] = ("Drivers", "Drivers"),
        ["cat_media"] = ("Media", "Mídia"),
        ["cat_utilities"] = ("Utilities", "Utilitários"),

        // ---- risk / status / labels ----
        ["risk_safe"] = ("Safe", "Seguro"),
        ["risk_moderate"] = ("Moderate", "Moderado"),
        ["risk_advanced"] = ("Advanced", "Avançado"),
        ["needs_restart"] = ("needs restart", "requer reinício"),
        ["reversible_note"] = ("previous value saved — reversible", "valor anterior guardado — reversível"),
        ["startup_auto"] = ("Automatic", "Automático"),
        ["startup_manual"] = ("Manual", "Manual"),
        ["startup_disabled"] = ("Disabled", "Desabilitado"),
        ["impact_high"] = ("High", "Alto"),
        ["impact_medium"] = ("Medium", "Médio"),
        ["impact_low"] = ("Low", "Baixo"),
        ["svc_running"] = ("Running", "Em execução"),
        ["svc_stopped"] = ("Stopped", "Parado"),
        ["app_installing"] = ("installing...", "instalando..."),
        ["app_installed"] = ("✓ installed", "✓ instalado"),
        ["app_failed"] = ("✕ failed", "✕ falhou"),

        // ---- dashboard ----
        ["dash_cpu"] = ("🧠  CPU", "🧠  CPU"),
        ["dash_mem"] = ("▦  Memory", "▦  Memória"),
        ["dash_disk"] = ("💾  Disk", "💾  Disco"),
        ["dash_net"] = ("📶  Network", "📶  Rede"),
        ["dash_gpu"] = ("🖥  GPU", "🖥  GPU"),
        ["dash_system_summary"] = ("System summary", "Resumo do sistema"),
        ["dash_health"] = ("Optimization health", "Saúde da otimização"),
        ["dash_of100"] = ("of 100", "de 100"),
        ["sum_system"] = ("System", "Sistema"),
        ["sum_cpu"] = ("Processor", "Processador"),
        ["sum_mem"] = ("Memory", "Memória"),
        ["sum_gpu"] = ("Graphics card", "Placa de vídeo"),
        ["sum_storage"] = ("Storage", "Armazenamento"),
        ["sum_uptime"] = ("Uptime", "Tempo ligado"),
        ["storage_fmt"] = ("{0} GB · {1} GB free", "{0} GB · {1} GB livres"),

        // ---- optimizations / tweaks ----
        ["opt_empty"] = ("No tweak in this category for the detected version.", "Nenhum tweak nesta categoria para a versão detectada."),
        ["tweaks_essential"] = ("Essential", "Essenciais"),
        ["tweaks_advanced"] = ("Advanced", "Avançado"),
        ["tweaks_warn"] = ("Advanced tweaks dig deep into the system. Create a restore point first and only turn on what you understand.",
                           "Tweaks avançados podem mexer fundo no sistema. Crie um ponto de restauração antes e ligue só o que entende."),
        ["tweaks_empty"] = ("No tweak here for the detected version.", "Nenhum tweak aqui para a versão detectada."),

        // ---- presets ----
        ["presets_intro"] = ("Click a preset to see what it does, its risk and whether a restart is needed before applying.",
                            "Clique num preset para ver o que ele faz, o risco e se precisa reiniciar antes de aplicar."),
        ["presets_see_details"] = ("See details  →", "Ver detalhes  →"),
        ["preset_cleared"] = ("Selection cleared — all tweaks disabled.", "Seleção limpa — todos os tweaks desativados."),
        ["preset_applied"] = ("Preset \"{0}\" applied. Check the Optimizations tab.", "Preset \"{0}\" aplicado. Veja a aba Otimizações."),
        ["pd_improves"] = ("What it improves", "O que melhora"),
        ["pd_will_enable"] = ("Tweaks that will be enabled", "Tweaks que serão ativados"),
        ["pd_will_disable"] = ("Tweaks that will be disabled", "Tweaks que serão desativados"),
        ["pd_apply"] = ("Apply preset", "Aplicar preset"),
        ["pd_clear_all"] = ("Clear all", "Limpar tudo"),
        ["pd_cancel"] = ("Cancel", "Cancelar"),
        ["pd_restart_badge"] = ("⟳ needs restart", "⟳ requer reinício"),
        ["pd_empty"] = ("No applicable tweak on this Windows version.", "Nenhum tweak aplicável nesta versão do Windows."),
        ["pd_risk"] = ("Risk: ", "Risco: "),
        ["pd_count"] = ("{0} tweaks", "{0} tweaks"),

        // ---- apps ----
        ["apps_header"] = ("Install apps via winget", "Instalar apps via winget"),
        ["apps_sub"] = ("Tick the apps and click install. Uses the Windows winget manager.", "Marque os apps e clique em instalar. Usa o gerenciador winget do Windows."),
        ["apps_installing"] = ("⬇  Installing...", "⬇  Instalando..."),
        ["apps_log_ready"] = ("Ready. Tick apps and click install.", "Pronto. Marque apps e clique em instalar."),
        ["apps_no_winget"] = ("winget not found. Install App Installer from the Microsoft Store.", "winget não encontrado. Instale o App Installer pela Microsoft Store."),
        ["apps_hash_title"] = ("Integrity check failed", "Verificação de integridade falhou"),
        ["apps_install"] = ("⬇  Install ({0})", "⬇  Instalar ({0})"),
        ["apps_hash_msg"] = (
            "The installer hash for \"{0}\" doesn't match winget's.\n\nThis usually happens when the app was updated and the winget manifest hasn't caught up. It may be harmless, but the integrity check exists to catch corrupted or tampered downloads.\n\nInstall anyway, skipping the hash check?",
            "O hash do instalador de \"{0}\" não corresponde ao do winget.\n\nIsso geralmente acontece quando o app foi atualizado e o manifesto do winget ainda não. Pode ser inofensivo, mas a verificação de integridade existe para detectar downloads corrompidos ou adulterados.\n\nDeseja instalar mesmo assim, ignorando a verificação de hash?"),

        // ---- cleanup ----
        ["cleanup_selected"] = ("Selected to clean: ", "Selecionado para limpar: "),
        ["cleanup_now"] = ("🗑  Clean now", "🗑  Limpar agora"),
        ["cleanup_cleaning"] = ("🗑  Cleaning...", "🗑  Limpando..."),
        ["cleanup_done"] = ("Cleanup done — {0} freed.", "Limpeza concluída — {0} liberados."),

        // ---- services ----
        ["svc_startup_title"] = ("Startup items", "Itens de inicialização"),
        ["svc_startup_desc"] = ("Apps that start with Windows (Run registry and StartupApproved folder).", "Apps que iniciam com o Windows (registro Run e pasta StartupApproved)."),
        ["svc_services_title"] = ("Windows services", "Serviços do Windows"),
        ["svc_services_desc"] = ("Core system services are flagged and protected from accidental disabling.", "Serviços core do sistema ficam marcados e protegidos contra desativação acidental."),
        ["svc_impact"] = ("Impact ", "Impacto "),
        ["svc_core"] = ("core", "core"),
        ["svc_safe_disable"] = ("safe to disable", "seguro desabilitar"),
        ["svc_fail_startup"] = ("Couldn't change startup for \"{0}\".", "Não foi possível alterar a inicialização de \"{0}\"."),
        ["svc_fail_service"] = ("Couldn't {0} the service \"{1}\".", "Não foi possível {0} o serviço \"{1}\"."),
        ["svc_verb_start"] = ("start", "iniciar"),
        ["svc_verb_stop"] = ("stop", "parar"),

        // ---- debloat ----
        ["debloat_desc"] = ("Removal via PackageManager (PowerShell fallback). Protected apps can't be removed.", "Remoção via PackageManager (PowerShell como fallback). Apps protegidos não podem ser removidos."),
        ["debloat_preset_safe"] = ("🛡  Preset: remove all safe", "🛡  Preset: remover tudo seguro"),
        ["debloat_removed"] = ("removed", "removido"),
        ["debloat_safe"] = ("safe", "seguro"),
        ["debloat_protected"] = ("protected", "protegido"),
        ["debloat_prefix"] = ("Appx apps · ", "Apps Appx · "),
        ["debloat_remove"] = ("📦  Remove ({0})", "📦  Remover ({0})"),
        ["debloat_removing"] = ("📦  Removing...", "📦  Removendo..."),
        ["dash_score_sub"] = ("{0} of {1} safe optimizations applied.", "{0} de {1} otimizações seguras aplicadas."),

        // ---- credits ----
        ["credits_intro"] = ("WinOptimizer is open source and draws on well-established projects in this space. The tweak, debloat and fix options were validated against these repos — all credit to them.",
                            "WinOptimizer é open source e se inspira em projetos consagrados da categoria. As opções de tweaks, debloat e fixes foram validadas com base nestes repositórios — todo o crédito a eles."),
        ["credits_footer"] = ("Personal open source project · made by Felipe · C# + WPF + .NET 8",
                             "Projeto pessoal open source · feito por Felipe · C# + WPF + .NET 8"),
        ["credits_releases_btn"] = ("↗  View releases on GitHub", "↗  Ver releases no GitHub"),
        ["lang_label"] = ("Language", "Idioma"),
    };
}

/// <summary>xaml shortcut: Text="{l:Tr nav_dashboard}" - binds to Loc so it updates live.</summary>
public sealed class TrExtension : MarkupExtension
{
    public string Key { get; set; } = "";
    public TrExtension() { }
    public TrExtension(string key) => Key = key;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => new Binding($"[{Key}]") { Source = Loc.I, Mode = BindingMode.OneWay }.ProvideValue(serviceProvider);
}
