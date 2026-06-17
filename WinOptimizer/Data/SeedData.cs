using WinOptimizer.Models;

namespace WinOptimizer.Data;

// seed data - the tweaks, appx, services and cleanup targets the app ships with.
// categories are language-neutral keys; display labels come from Loc.Cat(key).
public static class SeedData
{
    public static readonly string[] Categorias =
    {
        "performance", "privacy", "interface", "network", "power", "gaming",
        "personalization", "advanced", "features", "fixes"
    };

    public static List<TweakItem> Tweaks() => new()
    {
        // performance
        T("perf-power-high", "High performance power plan", "Plano de energia \"Alto desempenho\"",
            "Activates the plan that favors performance over power saving.",
            "Ativa o plano que prioriza performance sobre economia de energia.",
            "performance", null, false, RiscoTweak.Seguro, true),
        T("perf-visual", "Reduce visual effects and animations", "Reduzir efeitos visuais e animações",
            "Turns off window animations and transparency for a snappier feel.",
            "Desativa animações de janela e transparências para respostas mais rápidas.",
            "performance", null, false, RiscoTweak.Seguro, false),
        T("perf-sysmain", "Disable SysMain / Superfetch", "Desativar SysMain / Superfetch",
            "Recommended on SSDs. Cuts background disk usage.",
            "Recomendado em SSD. Reduz uso de disco em segundo plano.",
            "performance", null, true, RiscoTweak.Moderado, false),
        T("perf-mem-compress", "Disable memory compression", "Desativar compactação de memória",
            "Can help on machines with plenty of free RAM.",
            "Pode ajudar em máquinas com bastante RAM disponível.",
            "performance", null, true, RiscoTweak.Avancado, false),
        T("perf-bg-apps", "Restrict background apps", "Restringir apps em segundo plano",
            "Stops Store apps from running when they're not in use.",
            "Impede que apps da Store rodem quando não estão em uso.",
            "performance", null, false, RiscoTweak.Seguro, false),
        T("perf-storage-sense", "Enable automatic Storage Sense", "Ativar Storage Sense automático",
            "Cleans temp files and the recycle bin automatically.",
            "Limpa arquivos temporários e lixeira automaticamente.",
            "performance", null, false, RiscoTweak.Seguro, true),

        // privacy
        T("priv-telemetry", "Disable telemetry (DiagTrack)", "Desativar telemetria (DiagTrack)",
            "Stops Microsoft's diagnostics collection service.",
            "Para o serviço de coleta de diagnósticos da Microsoft.",
            "privacy", null, true, RiscoTweak.Moderado, false),
        T("priv-ad-id", "Disable Advertising ID", "Desativar Advertising ID",
            "Prevents personalized ads based on your usage.",
            "Impede anúncios personalizados com base no seu uso.",
            "privacy", null, false, RiscoTweak.Seguro, false),
        T("priv-timeline", "Disable activity history (Timeline)", "Desativar histórico de atividades (Timeline)",
            "Stops the activity log sent to your Microsoft account.",
            "Para o registro de atividades enviado à conta Microsoft.",
            "privacy", null, false, RiscoTweak.Seguro, false),
        T("priv-location", "Disable background location tracking", "Desativar rastreio de localização em 2º plano",
            "Blocks background apps from accessing your location.",
            "Bloqueia acesso à localização por apps em segundo plano.",
            "privacy", null, false, RiscoTweak.Seguro, false),
        T("priv-start-ads", "Disable Start menu suggestions and ads", "Desativar sugestões e anúncios no Menu Iniciar",
            "Removes suggested apps and promotions from the menu.",
            "Remove apps sugeridos e promoções do menu.",
            "privacy", null, false, RiscoTweak.Seguro, true),
        T("priv-cortana", "Disable Cortana", "Desativar Cortana",
            "Turns off the Cortana assistant.",
            "Desativa a assistente Cortana no sistema.",
            "privacy", WindowsVersion.Windows10, true, RiscoTweak.Moderado, false),

        // interface
        T("ui-dark", "Force dark mode", "Forçar modo escuro",
            "Applies the dark theme to the system and apps.",
            "Aplica o tema escuro ao sistema e aos apps.",
            "interface", null, false, RiscoTweak.Seguro, true),
        T("ui-file-ext", "Show file extensions", "Mostrar extensões de arquivo",
            "Shows extensions like .exe and .txt in Explorer.",
            "Exibe extensões como .exe e .txt no Explorer.",
            "interface", null, false, RiscoTweak.Seguro, true),
        T("ui-context-classic", "Restore classic context menu", "Restaurar menu de contexto clássico",
            "Brings back the old right-click menu without \"Show more options\".",
            "Volta o menu antigo do botão direito sem \"Mostrar mais opções\".",
            "interface", WindowsVersion.Windows11, true, RiscoTweak.Moderado, false),
        T("ui-taskbar-widgets", "Remove Widgets / Chat / Copilot from taskbar", "Remover Widgets / Chat / Copilot da taskbar",
            "Hides those buttons from the taskbar.",
            "Esconde os botões da barra de tarefas.",
            "interface", WindowsVersion.Windows11, false, RiscoTweak.Seguro, false),
        T("ui-taskbar-left", "Align taskbar icons to the left", "Alinhar ícones da taskbar à esquerda",
            "Moves the icons to the left corner, Windows 10 style.",
            "Move os ícones para o canto esquerdo, estilo Windows 10.",
            "interface", WindowsVersion.Windows11, false, RiscoTweak.Seguro, false),
        T("ui-news", "Disable News and Interests", "Desativar Notícias e Interesses",
            "Removes the news feed from the taskbar.",
            "Remove o feed de notícias da barra de tarefas.",
            "interface", WindowsVersion.Windows10, false, RiscoTweak.Seguro, false),

        // network
        T("net-nagle", "Disable Nagle's Algorithm", "Desativar Nagle's Algorithm",
            "Cuts latency in online games by not batching small packets.",
            "Reduz latência em jogos online ao não agrupar pacotes pequenos.",
            "network", null, true, RiscoTweak.Avancado, false),
        T("net-dns", "Switch DNS to Cloudflare / Google", "Trocar DNS para Cloudflare / Google",
            "Sets 1.1.1.1 or 8.8.8.8 on every interface in one click.",
            "Aplica 1.1.1.1 ou 8.8.8.8 em todas as interfaces, em 1 clique.",
            "network", null, false, RiscoTweak.Seguro, false),
        T("net-delivery", "Disable Delivery Optimization", "Desativar Delivery Optimization",
            "Stops P2P sharing of Windows updates between PCs.",
            "Impede o P2P de updates do Windows entre PCs.",
            "network", null, false, RiscoTweak.Moderado, false),
        T("net-winsock", "Reset network stack (Winsock)", "Resetar pilha de rede (Winsock)",
            "One-shot action to fix connectivity problems.",
            "Ação pontual para resolver problemas de conectividade.",
            "network", null, true, RiscoTweak.Avancado, false),

        // power
        T("power-ultimate", "Enable \"Ultimate Performance\" plan", "Habilitar plano \"Ultimate Performance\"",
            "Reveals the hidden maximum-performance power plan.",
            "Revela o plano de energia oculto de máxima performance.",
            "power", null, false, RiscoTweak.Moderado, false),
        T("power-usb", "Disable USB selective suspend", "Desativar suspensão seletiva USB",
            "Stops USB devices from going to sleep.",
            "Evita que dispositivos USB entrem em suspensão.",
            "power", null, false, RiscoTweak.Seguro, false),
        T("power-hibernate", "Disable hibernation and remove hiberfil.sys", "Desativar hibernação e remover hiberfil.sys",
            "Frees disk space equal to your RAM size.",
            "Libera espaço em disco igual ao tamanho da RAM.",
            "power", null, true, RiscoTweak.Moderado, false),

        // gaming
        T("game-mode", "Enable Game Mode", "Ativar Modo de Jogo",
            "Prioritizes system resources for the focused game.",
            "Prioriza recursos do sistema para o jogo em foco.",
            "gaming", null, false, RiscoTweak.Seguro, true),
        T("game-gamebar", "Disable Xbox Game Bar and background recording", "Desativar Xbox Game Bar e gravação em 2º plano",
            "Removes the overlay and automatic gameplay capture.",
            "Remove overlay e captura automática de gameplay.",
            "gaming", null, false, RiscoTweak.Seguro, false),
        T("game-hags", "Enable GPU Hardware-Accelerated Scheduling", "Ativar GPU Hardware-Accelerated Scheduling",
            "Can lower latency on newer GPUs. WARNING: on older GPUs (GTX 10 series or earlier) it may become a CPU bottleneck and LOWER your FPS. Needs a restart.",
            "Pode reduzir latência em GPUs novas. ATENÇÃO: em GPUs antigas (GTX série 10 ou anteriores) pode virar gargalo de CPU e BAIXAR o FPS. Exige reinício.",
            "gaming", null, true, RiscoTweak.Avancado, false),

        // performance (extra)
        T("perf-consumer", "Disable ConsumerFeatures", "Desativar ConsumerFeatures",
            "Stops Microsoft from auto-installing suggested apps.",
            "Impede instalação automática de apps sugeridos pela Microsoft.",
            "performance", null, false, RiscoTweak.Seguro, false),

        // privacy (extra)
        T("priv-tailored", "Disable Tailored Experiences", "Desativar Tailored Experiences",
            "Stops personalized experiences based on diagnostic data.",
            "Para experiências personalizadas com base em dados de diagnóstico.",
            "privacy", null, false, RiscoTweak.Seguro, false),
        T("priv-copilot", "Disable Copilot / Recall / Windows AI", "Desativar Copilot / Recall / Windows AI",
            "Turns off the Windows AI assistant. Some apps rely on it.",
            "Desliga o assistente de IA do Windows. Alguns apps dependem disso.",
            "privacy", null, true, RiscoTweak.Moderado, false),
        T("priv-findmydevice", "Disable \"Find My Device\"", "Desativar \"Find My Device\"",
            "Stops device location tracking by your Microsoft account.",
            "Para o rastreio de localização do dispositivo pela conta Microsoft.",
            "privacy", null, false, RiscoTweak.Seguro, false),
        T("priv-feedback", "Disable feedback prompts", "Desativar pedidos de feedback",
            "Windows stops asking for feedback periodically.",
            "Windows para de pedir feedback periodicamente.",
            "privacy", null, false, RiscoTweak.Seguro, false),

        // interface (extra)
        T("ui-transparency", "Disable transparency / Mica", "Desativar transparência / Mica",
            "Turns off interface transparency effects.",
            "Desliga efeitos de transparência da interface.",
            "interface", null, false, RiscoTweak.Seguro, false),
        T("ui-endtask", "Enable \"End Task\" on the taskbar", "Habilitar \"Finalizar Tarefa\" na taskbar",
            "Adds End Task to the taskbar right-click menu.",
            "Adiciona Finalizar Tarefa no clique direito da barra de tarefas.",
            "interface", WindowsVersion.Windows11, false, RiscoTweak.Seguro, false),

        // power (extra)
        T("power-waketimers", "Disable wake timers", "Desativar wake timers",
            "Stops scheduled tasks from waking the PC from sleep.",
            "Impede que tarefas agendadas acordem o PC da suspensão.",
            "power", null, false, RiscoTweak.Seguro, false),
        T("power-fastboot", "Disable Fast Startup", "Desativar Inicialização Rápida",
            "Turns off Fast Startup (hybrid boot). Fixes some boot bugs.",
            "Desliga o Fast Startup (hybrid boot). Resolve alguns bugs de boot.",
            "power", null, true, RiscoTweak.Moderado, false),

        // personalization
        T("pers-hidden-files", "Show hidden files", "Mostrar arquivos ocultos",
            "Shows hidden files and folders in Explorer.",
            "Exibe arquivos e pastas ocultos no Explorer.",
            "personalization", null, false, RiscoTweak.Seguro, false),
        T("pers-mouse-accel", "Disable mouse acceleration", "Desativar aceleração do mouse",
            "Removes acceleration for more predictable aim/movement.",
            "Remove a aceleração para mira/movimento mais previsível.",
            "personalization", null, false, RiscoTweak.Seguro, false),
        T("pers-numlock", "Num Lock on at startup", "Num Lock ativado ao iniciar",
            "Turns Num Lock on automatically at boot.",
            "Liga o Num Lock automaticamente no boot.",
            "personalization", null, false, RiscoTweak.Seguro, false),
        T("pers-scrollbars", "Always show scrollbars", "Barras de rolagem sempre visíveis",
            "Keeps scrollbars always visible in Store apps.",
            "Mantém as scrollbars sempre exibidas nos apps da Store.",
            "personalization", null, false, RiscoTweak.Seguro, false),
        T("pers-sticky", "Disable Sticky Keys", "Desativar Sticky Keys",
            "Turns off the accessibility shortcut that sometimes triggers by accident.",
            "Desliga o atalho de acessibilidade que às vezes ativa sem querer.",
            "personalization", null, false, RiscoTweak.Seguro, false),
        T("pers-search-icon", "Hide the taskbar search box", "Ocultar caixa de busca da taskbar",
            "Removes the search bar/icon from the taskbar.",
            "Remove a barra/ícone de busca da barra de tarefas.",
            "personalization", null, false, RiscoTweak.Seguro, false),
        T("pers-taskview", "Hide the Task View button", "Ocultar botão Task View",
            "Removes the Task View button from the taskbar.",
            "Remove o botão de Visão de Tarefas da taskbar.",
            "personalization", null, false, RiscoTweak.Seguro, false),
        T("pers-bing-search", "Disable Bing search in the Start menu", "Desativar busca do Bing no Menu Iniciar",
            "Removes web/Bing results from Start menu search.",
            "Remove resultados web/Bing da busca do menu iniciar.",
            "personalization", null, false, RiscoTweak.Seguro, false),
        T("pers-longpaths", "Enable long paths", "Habilitar caminhos longos (Long Paths)",
            "Allows file paths longer than 260 characters.",
            "Permite caminhos de arquivo acima de 260 caracteres.",
            "personalization", null, true, RiscoTweak.Seguro, false),
        T("pers-verbose-login", "Verbose logon", "Login verboso",
            "Shows more detail during boot and logon.",
            "Mostra mais detalhes durante o boot e logon.",
            "personalization", null, false, RiscoTweak.Seguro, false),

        // advanced
        T("adv-utc", "Set system clock to UTC", "Definir hora do sistema para UTC",
            "Makes the BIOS clock use UTC (dual-boot with Linux). Niche use.",
            "Faz o relógio do BIOS usar UTC (dual-boot com Linux). Uso específico.",
            "advanced", null, true, RiscoTweak.Avancado, false),
        T("adv-onedrive", "Remove Microsoft OneDrive", "Remover Microsoft OneDrive",
            "Uninstalls OneDrive. Reversible by reinstalling.",
            "Desinstala o OneDrive do sistema. Reversível reinstalando.",
            "advanced", null, false, RiscoTweak.Avancado, false),
        T("adv-wpbt", "Disable Windows Platform Binary Table (WPBT)", "Desativar Windows Platform Binary Table (WPBT)",
            "Stops firmware from injecting executables into Windows.",
            "Impede que o firmware injete executáveis no Windows.",
            "advanced", null, true, RiscoTweak.Avancado, false),

        // windows features (dism)
        T("feat-netfx3", "Enable .NET Framework 3.5", "Habilitar .NET Framework 3.5",
            "Needed by older programs.",
            "Necessário para programas mais antigos.",
            "features", null, false, RiscoTweak.Seguro, false),
        T("feat-mediaplayer", "Enable legacy Windows Media Player", "Habilitar Windows Media Player legado",
            "Reinstalls legacy media components.",
            "Reinstala componentes legados de mídia.",
            "features", null, false, RiscoTweak.Seguro, false),
        T("feat-wsl", "Enable Windows Subsystem for Linux (WSL)", "Habilitar Windows Subsystem for Linux (WSL)",
            "Turns on the Linux subsystem. Needs a restart.",
            "Ativa o subsistema Linux. Requer reinício.",
            "features", null, true, RiscoTweak.Moderado, false),
        T("feat-sandbox", "Enable Windows Sandbox", "Habilitar Windows Sandbox",
            "Disposable isolated environment. Needs virtualization in the BIOS.",
            "Ambiente descartável isolado. Requer virtualização na BIOS.",
            "features", null, true, RiscoTweak.Moderado, false),
        T("feat-hyperv", "Enable Hyper-V", "Habilitar Hyper-V",
            "Microsoft's hypervisor. May conflict with VirtualBox.",
            "Hypervisor da Microsoft. Pode conflitar com VirtualBox.",
            "features", null, true, RiscoTweak.Moderado, false),

        // fixes (one-shot actions)
        T("fix-sfc-dism", "Check system corruption (DISM + SFC)", "Verificar corrupção do sistema (DISM + SFC)",
            "Runs DISM /RestoreHealth and sfc /scannow. Toggle on to run.",
            "Roda DISM /RestoreHealth e sfc /scannow. Ligue para executar.",
            "fixes", null, false, RiscoTweak.Seguro, false),
        T("fix-wu-reset", "Reset Windows Update components", "Resetar componentes do Windows Update",
            "Clears the corrupt Windows Update cache.",
            "Limpa o cache corrompido do Windows Update.",
            "fixes", null, false, RiscoTweak.Moderado, false),
        T("fix-network-reset", "Reset network stack (Winsock/TCP-IP)", "Resetar pilha de rede (Winsock/TCP-IP)",
            "Resets Winsock and IP. Only use if you have connection issues.",
            "Reseta Winsock e IP. Use só se tiver problema de conexão.",
            "fixes", null, true, RiscoTweak.Moderado, false),
        T("fix-ntp", "Fix the time server (NTP)", "Corrigir servidor de horário (NTP)",
            "Re-syncs the clock with the time server.",
            "Ressincroniza o relógio com o servidor de tempo.",
            "fixes", null, false, RiscoTweak.Seguro, false),

        // ======================================================
        //  extras ported from winutil (ChrisTitusTech)
        // ======================================================

        // network (winutil)
        T("net-ipv4-pref", "Prefer IPv4 over IPv6", "Preferir IPv4 sobre IPv6",
            "Keeps IPv6 on but makes the system prefer IPv4. Fixes slowness on some routers.",
            "Mantém o IPv6 ligado mas o sistema prioriza IPv4. Resolve lentidão em alguns roteadores.",
            "network", null, true, RiscoTweak.Moderado, false),
        T("net-teredo", "Disable Teredo", "Desativar Teredo",
            "Turns off the Teredo tunnel (IPv6 over IPv4). Can improve online latency.",
            "Desliga o túnel Teredo (IPv6 sobre IPv4). Pode melhorar latência online.",
            "network", null, true, RiscoTweak.Moderado, false),
        T("net-ipv6-off", "Disable IPv6 completely", "Desativar IPv6 completamente",
            "Disables IPv6 on every adapter. Only use if your network is 100% IPv4.",
            "Desabilita o IPv6 em todos os adaptadores. Só use se sua rede for 100% IPv4.",
            "network", null, true, RiscoTweak.Avancado, false),
        T("net-adobe-block", "Block Adobe telemetry (hosts)", "Bloquear telemetria da Adobe (hosts)",
            "Adds Adobe domains to the hosts file pointing them at 0.0.0.0.",
            "Adiciona domínios da Adobe ao arquivo hosts apontando para 0.0.0.0.",
            "network", null, false, RiscoTweak.Moderado, false),

        // gaming (winutil)
        T("game-fso", "Disable Fullscreen Optimizations (FSO)", "Desativar Fullscreen Optimizations (FSO)",
            "Turns off Windows fullscreen optimization. Usually reduces input lag/stutter in games.",
            "Desliga a otimização de tela cheia do Windows. Costuma reduzir input lag/stutter em jogos.",
            "gaming", null, false, RiscoTweak.Seguro, false),
        T("game-mpo", "Disable Multiplane Overlay (MPO)", "Desativar Multiplane Overlay (MPO)",
            "Fixes flickering/black screen in some games and GPUs. Needs a restart.",
            "Corrige flickering/black screen em alguns jogos e GPUs. Requer reinício.",
            "gaming", null, true, RiscoTweak.Moderado, false),

        // interface (winutil)
        T("ui-remove-home-gallery", "Remove Home and Gallery from Explorer", "Remover Home e Galeria do Explorer",
            "Removes the \"Home\" and \"Gallery\" shortcuts from the Explorer side panel.",
            "Tira os atalhos \"Início\" e \"Galeria\" do painel lateral do Explorer.",
            "interface", WindowsVersion.Windows11, false, RiscoTweak.Moderado, false),
        T("ui-no-notifications", "Disable notification center and toasts", "Desativar central de notificações e toasts",
            "Turns off pop-up notifications and the notification center.",
            "Desliga as notificações pop-up e a central de notificações.",
            "interface", null, false, RiscoTweak.Moderado, false),
        T("ui-battery-percent", "Show battery % in the tray", "Mostrar % da bateria na bandeja",
            "Shows the battery percentage over the tray icon (laptops).",
            "Exibe a porcentagem da bateria sobre o ícone da bandeja (notebooks).",
            "interface", null, false, RiscoTweak.Seguro, false),
        T("ui-hide-settings-home", "Hide the Settings home page", "Esconder página inicial das Configurações",
            "Removes the Home page from Settings, opening straight to the section list.",
            "Remove a Home do app Configurações, abrindo direto na lista de seções.",
            "interface", WindowsVersion.Windows11, false, RiscoTweak.Seguro, false),
        T("ui-login-blur", "Disable login screen blur", "Desativar blur na tela de login",
            "Removes the acrylic blur behind the logon screen.",
            "Remove o efeito de desfoque acrílico atrás da tela de logon.",
            "interface", null, false, RiscoTweak.Seguro, false),
        T("ui-explorer-autodiscovery", "Disable automatic folder type discovery", "Desativar descoberta automática de tipo de pasta",
            "Stops Explorer from switching to a \"photos/music\" layout on its own and lagging in big folders.",
            "Impede o Explorer de virar layout de \"fotos/música\" sozinho e ficar lento em pastas grandes.",
            "interface", null, false, RiscoTweak.Moderado, false),

        // personalization (winutil)
        T("pers-new-outlook", "Disable the \"new Outlook\"", "Desativar o \"novo Outlook\"",
            "Forces classic Outlook and hides the switch to the new Outlook.",
            "Força o Outlook clássico e esconde o botão de troca para o novo Outlook.",
            "personalization", null, false, RiscoTweak.Seguro, false),

        // privacy (winutil)
        T("priv-store-search", "Block Microsoft Store recommended search", "Bloquear busca recomendada da Microsoft Store",
            "Denies access to the Store database to stop recommended search results.",
            "Nega acesso ao banco da Store para parar resultados de busca recomendados.",
            "privacy", null, false, RiscoTweak.Moderado, false),

        // power (winutil)
        T("power-standby-fix", "Fix network on modern standby (S0)", "Corrigir rede em suspensão moderna (S0)",
            "Keeps network connectivity during Modern Standby.",
            "Mantém a conectividade de rede durante o Modern Standby.",
            "power", null, false, RiscoTweak.Moderado, false),
        T("power-s3-sleep", "Force S3 sleep (disable Modern Standby)", "Forçar suspensão S3 (desligar Modern Standby)",
            "Goes back to classic S3 sleep. Can fix a PC that won't sleep/wakes by itself. Needs a restart.",
            "Volta à suspensão S3 clássica. Pode resolver PC que não dorme/acorda sozinho. Requer reinício.",
            "power", null, true, RiscoTweak.Avancado, false),

        // advanced (winutil)
        T("adv-razer-block", "Block Razer software auto-install", "Bloquear auto-instalação de software Razer",
            "Stops Windows from installing Razer Synapse on its own when you plug in a device.",
            "Impede o Windows de baixar/instalar o Razer Synapse sozinho ao plugar um dispositivo.",
            "advanced", null, false, RiscoTweak.Moderado, false),
        T("adv-detailed-bsod", "Detailed BSOD (verbose mode)", "BSOD detalhado (modo verboso)",
            "Shows technical parameters on the blue screen, without the emoji. Helps diagnose crashes.",
            "Mostra parâmetros técnicos na tela azul, sem o emoji. Ajuda a diagnosticar travamentos.",
            "advanced", null, false, RiscoTweak.Seguro, false),
        T("adv-edge-debloat", "Debloat Microsoft Edge", "Debloat do Microsoft Edge",
            "Turns off telemetry, ads, Rewards, Shopping and widgets in Edge via policies.",
            "Desliga telemetria, anúncios, Rewards, Shopping e widgets do Edge via políticas.",
            "advanced", null, false, RiscoTweak.Moderado, false),
        T("adv-brave-debloat", "Debloat Brave", "Debloat do Brave",
            "Disables Rewards, Wallet, VPN, AI and usage reporting in Brave via policies.",
            "Desativa Rewards, Wallet, VPN, IA e relatórios de uso do Brave via políticas.",
            "advanced", null, false, RiscoTweak.Moderado, false),
        T("adv-remove-edge", "Remove Microsoft Edge", "Remover Microsoft Edge",
            "Tries to uninstall Edge with the official uninstaller. May fail depending on the version.",
            "Tenta desinstalar o Edge pelo desinstalador oficial. Pode falhar dependendo da versão.",
            "advanced", null, true, RiscoTweak.Avancado, false),
        T("adv-bitlocker-off", "Disable BitLocker on the system drive", "Desativar BitLocker no disco do sistema",
            "Decrypts and turns off BitLocker on the Windows drive. Decryption takes a while.",
            "Descriptografa e desliga o BitLocker da unidade do Windows. A descriptografia leva tempo.",
            "advanced", null, true, RiscoTweak.Avancado, false),

        // fixes (winutil)
        T("fix-services-manual", "Optimize services (set several to Manual)", "Otimizar serviços (definir vários como Manual)",
            "Sets non-essential services (telemetry, maps, fax, search...) to Manual.",
            "Coloca serviços não essenciais (telemetria, mapas, fax, busca...) em Manual.",
            "fixes", null, false, RiscoTweak.Moderado, false),
        T("fix-oosu", "Open O&O ShutUp10++ (privacy)", "Abrir O&O ShutUp10++ (privacidade)",
            "Downloads and opens the O&O ShutUp10++ tool for fine privacy tuning.",
            "Baixa e abre a ferramenta O&O ShutUp10++ para ajustes finos de privacidade.",
            "fixes", null, false, RiscoTweak.Seguro, false),
    };

    public static List<AppxPackage> Appx() => new()
    {
        A("appx-3dviewer", "3D Viewer", "Microsoft", null, true, true, "Microsoft3DViewer"),
        A("appx-gethelp", "Get Help", "Microsoft", null, true, true, "GetHelp"),
        A("appx-feedback", "Feedback Hub", "Microsoft", null, true, true, "WindowsFeedbackHub"),
        A("appx-solitaire", "Microsoft Solitaire Collection", "Microsoft", null, true, true, "SolitaireCollection"),
        A("appx-people", "People", "Microsoft", null, true, false, "Microsoft.People"),
        A("appx-skype", "Skype", "Microsoft", null, true, true, "SkypeApp"),
        A("appx-mixedreality", "Mixed Reality Portal", "Microsoft", WindowsVersion.Windows10, true, true, "Microsoft.MixedReality.Portal"),
        A("appx-cortana", "Cortana", "Microsoft", WindowsVersion.Windows10, false, false, "549981C3F5F10"),
        A("appx-teams", "Microsoft Teams (personal)", "Microsoft", WindowsVersion.Windows11, true, true, "MicrosoftTeams"),
        A("appx-clipchamp", "Clipchamp", "Microsoft", WindowsVersion.Windows11, true, true, "Clipchamp"),
        A("appx-copilot", "Copilot", "Microsoft", WindowsVersion.Windows11, true, false, "Microsoft.Copilot"),
        A("appx-store", "Microsoft Store", "Microsoft", null, false, false, "WindowsStore"),
        A("appx-photos", "Photos", "Microsoft", null, false, false, "Windows.Photos"),
        A("appx-xbox", "Xbox", "Microsoft", null, true, false, "XboxApp"),
    };

    public static List<ServiceItem> Services() => new()
    {
        S("svc-diagtrack", "DiagTrack", "Connected User Experiences and Telemetry", ServiceStatus.EmExecucao, StartupType.Automatico, false),
        S("svc-sysmain", "SysMain", "Preloads frequently used apps", ServiceStatus.EmExecucao, StartupType.Automatico, false),
        S("svc-wsearch", "WSearch", "Windows Search indexing", ServiceStatus.EmExecucao, StartupType.Automatico, false),
        S("svc-spooler", "Spooler", "Print spooler", ServiceStatus.EmExecucao, StartupType.Automatico, false),
        S("svc-fax", "Fax", "Fax send/receive service", ServiceStatus.Parado, StartupType.Manual, false),
        S("svc-wuauserv", "wuauserv", "Windows Update", ServiceStatus.EmExecucao, StartupType.Manual, true),
        S("svc-bits", "BITS", "Background Intelligent Transfer Service", ServiceStatus.EmExecucao, StartupType.Manual, true),
        S("svc-dnscache", "Dnscache", "DNS Client", ServiceStatus.EmExecucao, StartupType.Automatico, true),
    };

    public static List<StartupItem> Startup() => new()
    {
        U("su-steam", "Steam", "Valve Corporation", ImpactoStartup.Alto, true),
        U("su-discord", "Discord", "Discord Inc.", ImpactoStartup.Medio, true),
        U("su-spotify", "Spotify", "Spotify AB", ImpactoStartup.Medio, true),
        U("su-onedrive", "OneDrive", "Microsoft", ImpactoStartup.Alto, true),
        U("su-teams", "Microsoft Teams", "Microsoft", ImpactoStartup.Alto, false),
        U("su-nvidia", "NVIDIA App", "NVIDIA Corporation", ImpactoStartup.Baixo, true),
        U("su-epic", "Epic Games Launcher", "Epic Games", ImpactoStartup.Medio, false),
    };

    public static List<CleanupCategory> Cleanup() => new()
    {
        C("cl-temp-user", "User temp files", "Arquivos temporários do usuário", "%Temp% for the current account", "%Temp% da conta atual", 2480, true),
        C("cl-temp-win", "Windows temp files", "Arquivos temporários do Windows", @"C:\Windows\Temp", @"C:\Windows\Temp", 1120, true),
        C("cl-wu-cache", "Windows Update cache", "Cache do Windows Update", @"SoftwareDistribution\Download", @"SoftwareDistribution\Download", 4360, true),
        C("cl-recycle", "Recycle Bin", "Lixeira", "Empty the recycle bin on all drives", "Esvaziar a lixeira de todas as unidades", 890, false),
        C("cl-thumbnails", "Thumbnail cache", "Cache de miniaturas", "Explorer thumbnails", "Miniaturas do Explorer", 310, false),
        C("cl-logs", "Old system logs", "Logs antigos do sistema", "Log files older than 30 days", "Arquivos de log com mais de 30 dias", 540, false),
    };

    // short factories
    static TweakItem T(string id, string en, string pt, string descEn, string descPt, string cat,
                       WindowsVersion? os, bool reinicio, RiscoTweak risco, bool ativo)
        => new()
        {
            Id = id, TituloEn = en, TituloPt = pt, DescricaoEn = descEn, DescricaoPt = descPt,
            Categoria = cat, AlvoOS = os, RequerReinicio = reinicio, Risco = risco, Ativo = ativo
        };

    static AppxPackage A(string id, string nome, string pub, WindowsVersion? os, bool seguro, bool sel, string pattern)
        => new() { Id = id, Nome = nome, Publisher = pub, AlvoOS = os, SeguroRemover = seguro, Selecionado = sel, Pattern = pattern };

    static ServiceItem S(string id, string nome, string desc, ServiceStatus st, StartupType su, bool core)
        => new() { Id = id, Nome = nome, Descricao = desc, Status = st, Startup = su, Core = core };

    static StartupItem U(string id, string nome, string pub, ImpactoStartup imp, bool ativo)
        => new() { Id = id, Nome = nome, Publisher = pub, Impacto = imp, Ativo = ativo };

    static CleanupCategory C(string id, string nomeEn, string nomePt, string descEn, string descPt, int mb, bool sel)
        => new() { Id = id, NomeEn = nomeEn, NomePt = nomePt, DescricaoEn = descEn, DescricaoPt = descPt, TamanhoMb = mb, Selecionado = sel };
}
