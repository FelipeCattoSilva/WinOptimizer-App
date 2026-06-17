using System.IO;
using WinOptimizer.Models;

namespace WinOptimizer.Services;

// applies/reverts each tweak on the real system (registry, services, powercfg).
// enable=true applies the tweak, enable=false reverts to the windows default.
public static class TweakService
{
    private const string Advanced = @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
    private const string CDM = @"HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";

    public static void Apply(TweakItem t, bool enable)
    {
        switch (t.Id)
        {
            // ---------- performance ----------
            case "perf-power-high":
                ProcHelper.PowerCfg(enable
                    ? "/setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c"   // High performance
                    : "/setactive 381b4222-f694-41f0-9685-ff5bb260df2e"); // Balanced
                break;
            case "perf-visual":
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects",
                    "VisualFXSetting", enable ? 2 : 0);
                break;
            case "perf-sysmain":
                Service("SysMain", enable);
                break;
            case "perf-mem-compress":
                ProcHelper.PowerShell(enable ? "Disable-MMAgent -mc" : "Enable-MMAgent -mc");
                break;
            case "perf-bg-apps":
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications",
                    "GlobalUserDisabled", enable ? 1 : 0);
                break;
            case "perf-storage-sense":
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\StorageSense\Parameters\StoragePolicy",
                    "01", enable ? 1 : 0);
                break;

            // ---------- privacy ----------
            case "priv-telemetry":
                RegHelper.SetDword(@"HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection",
                    "AllowTelemetry", enable ? 0 : 1);
                Service("DiagTrack", enable);
                break;
            case "priv-ad-id":
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo",
                    "Enabled", enable ? 0 : 1);
                break;
            case "priv-timeline":
                RegHelper.SetDword(@"HKLM\SOFTWARE\Policies\Microsoft\Windows\System",
                    "EnableActivityFeed", enable ? 0 : 1);
                break;
            case "priv-location":
                RegHelper.SetString(@"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location",
                    "Value", enable ? "Deny" : "Allow");
                break;
            case "priv-start-ads":
                RegHelper.SetDword(CDM, "SystemPaneSuggestionsEnabled", enable ? 0 : 1);
                RegHelper.SetDword(CDM, "SubscribedContent-338388Enabled", enable ? 0 : 1);
                break;
            case "priv-cortana":
                RegHelper.SetDword(@"HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search",
                    "AllowCortana", enable ? 0 : 1);
                break;

            // ---------- interface ----------
            case "ui-dark":
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                    "AppsUseLightTheme", enable ? 0 : 1);
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                    "SystemUsesLightTheme", enable ? 0 : 1);
                break;
            case "ui-file-ext":
                RegHelper.SetDword(Advanced, "HideFileExt", enable ? 0 : 1);
                break;
            case "ui-context-classic":
                if (enable)
                    RegHelper.SetString(@"HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32", "", "");
                else
                    RegHelper.DeleteKey(@"HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}");
                break;
            case "ui-taskbar-widgets":
                RegHelper.SetDword(Advanced, "TaskbarDa", enable ? 0 : 1);
                break;
            case "ui-taskbar-left":
                RegHelper.SetDword(Advanced, "TaskbarAl", enable ? 0 : 1);
                break;
            case "ui-news":
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Feeds",
                    "ShellFeedsTaskbarViewMode", enable ? 2 : 0);
                break;

            // ---------- network ----------
            case "net-nagle":
                NagleAll(enable);
                break;
            case "net-dns":
                if (enable)
                    ProcHelper.PowerShell("Get-NetAdapter -Physical | Where-Object Status -eq 'Up' | " +
                        "ForEach-Object { Set-DnsClientServerAddress -InterfaceIndex $_.ifIndex -ServerAddresses ('1.1.1.1','1.0.0.1') }");
                else
                    ProcHelper.PowerShell("Get-NetAdapter -Physical | " +
                        "ForEach-Object { Set-DnsClientServerAddress -InterfaceIndex $_.ifIndex -ResetServerAddresses }");
                break;
            case "net-delivery":
                RegHelper.SetDword(@"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\DeliveryOptimization\Config",
                    "DODownloadMode", enable ? 0 : 1);
                Service("DoSvc", enable);
                break;
            case "net-winsock":
                // one-shot repair action
                if (enable) { ProcHelper.Run("netsh", "winsock reset"); ProcHelper.Run("netsh", "int ip reset"); }
                break;

            // ---------- power ----------
            case "power-ultimate":
                if (enable)
                {
                    ProcHelper.PowerCfg("-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
                    ProcHelper.PowerCfg("/setactive e9a42b02-d5df-448d-aa00-03f14749eb61");
                }
                else ProcHelper.PowerCfg("/setactive 381b4222-f694-41f0-9685-ff5bb260df2e");
                break;
            case "power-usb":
                ProcHelper.PowerCfg($"/setacvalueindex SCHEME_CURRENT 2a737441-1930-4402-8d77-b2bebba308a3 48e6b7a6-50f5-4782-a5d4-53bb8f07e226 {(enable ? 0 : 1)}");
                ProcHelper.PowerCfg("/setactive SCHEME_CURRENT");
                break;
            case "power-hibernate":
                ProcHelper.PowerCfg(enable ? "/hibernate off" : "/hibernate on");
                break;

            // ---------- gaming ----------
            case "game-mode":
                RegHelper.SetDword(@"HKCU\Software\Microsoft\GameBar", "AutoGameModeEnabled", enable ? 1 : 0);
                break;
            case "game-gamebar":
                RegHelper.SetDword(@"HKCU\System\GameConfigStore", "GameDVR_Enabled", enable ? 0 : 1);
                RegHelper.SetDword(@"HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR", "AllowGameDVR", enable ? 0 : 1);
                break;
            case "game-hags":
                RegHelper.SetDword(@"HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers",
                    "HwSchMode", enable ? 2 : 1);
                break;

            // ---------- performance (extra) ----------
            case "perf-consumer":
                RegHelper.SetDword(@"HKLM\SOFTWARE\Policies\Microsoft\Windows\CloudContent",
                    "DisableWindowsConsumerFeatures", enable ? 1 : 0);
                break;

            // ---------- privacy (extra) ----------
            case "priv-tailored":
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Privacy",
                    "TailoredExperiencesWithDiagnosticDataEnabled", enable ? 0 : 1);
                break;
            case "priv-copilot":
                RegHelper.SetDword(@"HKCU\Software\Policies\Microsoft\Windows\WindowsCopilot",
                    "TurnOffWindowsCopilot", enable ? 1 : 0);
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                    "ShowCopilotButton", enable ? 0 : 1);
                break;
            case "priv-findmydevice":
                RegHelper.SetDword(@"HKLM\SOFTWARE\Microsoft\PolicyManager\default\Settings\AllowFindMyDevice",
                    "value", enable ? 0 : 1);
                break;
            case "priv-feedback":
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Siuf\Rules", "NumberOfSIUFInPeriod", enable ? 0 : 1);
                break;

            // ---------- interface (extra) ----------
            case "ui-transparency":
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                    "EnableTransparency", enable ? 0 : 1);
                break;
            case "ui-endtask":
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings",
                    "TaskbarEndTask", enable ? 1 : 0);
                break;

            // ---------- power (extra) ----------
            case "power-waketimers":
                ProcHelper.PowerCfg($"/setacvalueindex SCHEME_CURRENT SUB_SLEEP RTCWAKE {(enable ? 0 : 1)}");
                ProcHelper.PowerCfg("/setactive SCHEME_CURRENT");
                break;
            case "power-fastboot":
                RegHelper.SetDword(@"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Power",
                    "HiberbootEnabled", enable ? 0 : 1);
                break;

            // ---------- personalization ----------
            case "pers-hidden-files":
                RegHelper.SetDword(Advanced, "Hidden", enable ? 1 : 2);
                break;
            case "pers-mouse-accel":
                RegHelper.SetString(@"HKCU\Control Panel\Mouse", "MouseSpeed", enable ? "0" : "1");
                RegHelper.SetString(@"HKCU\Control Panel\Mouse", "MouseThreshold1", enable ? "0" : "6");
                RegHelper.SetString(@"HKCU\Control Panel\Mouse", "MouseThreshold2", enable ? "0" : "10");
                break;
            case "pers-numlock":
                RegHelper.SetString(@"HKCU\Control Panel\Keyboard", "InitialKeyboardIndicators", enable ? "2" : "0");
                break;
            case "pers-scrollbars":
                RegHelper.SetDword(@"HKCU\Control Panel\Accessibility", "DynamicScrollbars", enable ? 0 : 1);
                break;
            case "pers-sticky":
                RegHelper.SetString(@"HKCU\Control Panel\Accessibility\StickyKeys", "Flags", enable ? "506" : "510");
                break;
            case "pers-search-icon":
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Search",
                    "SearchboxTaskbarMode", enable ? 0 : 1);
                break;
            case "pers-taskview":
                RegHelper.SetDword(Advanced, "ShowTaskViewButton", enable ? 0 : 1);
                break;
            case "pers-bing-search":
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Search", "BingSearchEnabled", enable ? 0 : 1);
                RegHelper.SetDword(@"HKCU\Software\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions", enable ? 1 : 0);
                break;
            case "pers-longpaths":
                RegHelper.SetDword(@"HKLM\SYSTEM\CurrentControlSet\Control\FileSystem", "LongPathsEnabled", enable ? 1 : 0);
                break;
            case "pers-verbose-login":
                RegHelper.SetDword(@"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "VerboseStatus", enable ? 1 : 0);
                break;

            // ---------- advanced ----------
            case "adv-utc":
                RegHelper.SetDword(@"HKLM\SYSTEM\CurrentControlSet\Control\TimeZoneInformation", "RealTimeIsUniversal", enable ? 1 : 0);
                break;
            case "adv-onedrive":
                if (enable) OneDriveUninstall();
                break;
            case "adv-wpbt":
                RegHelper.SetDword(@"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager", "DisableWpbtExecution", enable ? 1 : 0);
                break;

            // ---------- windows features (dism) ----------
            case "feat-netfx3": Feature("NetFx3", enable); break;
            case "feat-mediaplayer": Feature("WindowsMediaPlayer", enable); break;
            case "feat-wsl": Feature("Microsoft-Windows-Subsystem-Linux", enable); break;
            case "feat-sandbox": Feature("Containers-DisposableClientVM", enable); break;
            case "feat-hyperv": Feature("Microsoft-Hyper-V-All", enable); break;

            // ---------- fixes (one-shot, only run on enable) ----------
            case "fix-sfc-dism":
                if (enable)
                {
                    ProcHelper.Run("dism.exe", "/Online /Cleanup-Image /RestoreHealth", 600000);
                    ProcHelper.Run("sfc.exe", "/scannow", 600000);
                }
                break;
            case "fix-wu-reset":
                if (enable) WindowsUpdateReset();
                break;
            case "fix-network-reset":
                if (enable) { ProcHelper.Run("netsh", "winsock reset"); ProcHelper.Run("netsh", "int ip reset"); }
                break;
            case "fix-ntp":
                if (enable)
                {
                    ProcHelper.Run("sc", "config w32time start= auto");
                    ProcHelper.Run("net", "start w32time");
                    ProcHelper.Run("w32tm", "/resync /force");
                }
                break;

            // ======================================================
            //  extras ported from winutil
            // ======================================================

            // ---------- network ----------
            case "net-ipv4-pref":
                RegHelper.SetDword(Tcpip6, "DisabledComponents", enable ? 32 : 0);
                break;
            case "net-teredo":
                RegHelper.SetDword(Tcpip6, "DisabledComponents", enable ? 1 : 0);
                ProcHelper.Run("netsh", $"interface teredo set state {(enable ? "disabled" : "default")}");
                break;
            case "net-ipv6-off":
                RegHelper.SetDword(Tcpip6, "DisabledComponents", enable ? 255 : 0);
                ProcHelper.PowerShell($"{(enable ? "Disable" : "Enable")}-NetAdapterBinding -Name * -ComponentID ms_tcpip6");
                break;
            case "net-adobe-block":
                AdobeHosts(enable);
                break;

            // ---------- gaming ----------
            case "game-fso":
                RegHelper.SetDword(@"HKCU\System\GameConfigStore", "GameDVR_DXGIHonorFSEWindowsCompatible", enable ? 1 : 0);
                RegHelper.SetDword(@"HKCU\System\GameConfigStore", "GameDVR_FSEBehavior", enable ? 2 : 0);
                break;
            case "game-mpo":
                if (enable) RegHelper.SetDword(@"HKLM\SOFTWARE\Microsoft\Windows\Dwm", "OverlayTestMode", 5);
                else RegHelper.DeleteValue(@"HKLM\SOFTWARE\Microsoft\Windows\Dwm", "OverlayTestMode");
                break;

            // ---------- interface ----------
            case "ui-remove-home-gallery":
                if (enable)
                {
                    RegHelper.SetDword(@"HKCU\Software\Classes\CLSID\{f874310e-b6b7-47dc-bc84-b9e6b38f5903}", "System.IsPinnedToNameSpaceTree", 0);
                    RegHelper.SetDword(@"HKCU\Software\Classes\CLSID\{e88865ea-0e1c-4e20-9aa6-edcd0212c87c}", "System.IsPinnedToNameSpaceTree", 0);
                    RegHelper.SetDword(Advanced, "LaunchTo", 1);
                }
                else
                {
                    RegHelper.DeleteValue(@"HKCU\Software\Classes\CLSID\{f874310e-b6b7-47dc-bc84-b9e6b38f5903}", "System.IsPinnedToNameSpaceTree");
                    RegHelper.DeleteValue(@"HKCU\Software\Classes\CLSID\{e88865ea-0e1c-4e20-9aa6-edcd0212c87c}", "System.IsPinnedToNameSpaceTree");
                    RegHelper.DeleteValue(Advanced, "LaunchTo");
                }
                break;
            case "ui-no-notifications":
                if (enable)
                {
                    RegHelper.SetDword(@"HKCU\Software\Policies\Microsoft\Windows\Explorer", "DisableNotificationCenter", 1);
                    RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\PushNotifications", "ToastEnabled", 0);
                }
                else
                {
                    RegHelper.DeleteValue(@"HKCU\Software\Policies\Microsoft\Windows\Explorer", "DisableNotificationCenter");
                    RegHelper.SetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\PushNotifications", "ToastEnabled", 1);
                }
                break;
            case "ui-battery-percent":
                if (enable) RegHelper.SetDword(Advanced, "IsBatteryPercentageEnabled", 1);
                else RegHelper.DeleteValue(Advanced, "IsBatteryPercentageEnabled");
                break;
            case "ui-hide-settings-home":
                if (enable) RegHelper.SetString(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "SettingsPageVisibility", "hide:home");
                else RegHelper.DeleteValue(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "SettingsPageVisibility");
                break;
            case "ui-login-blur":
                RegHelper.SetDword(@"HKLM\SOFTWARE\Policies\Microsoft\Windows\System", "DisableAcrylicBackgroundOnLogon", enable ? 1 : 0);
                break;
            case "ui-explorer-autodiscovery":
                ExplorerAutoDiscovery(enable);
                break;

            // ---------- personalization ----------
            case "pers-new-outlook":
                RegHelper.SetDword(@"HKCU\SOFTWARE\Microsoft\Office\16.0\Outlook\Preferences", "UseNewOutlook", enable ? 0 : 1);
                RegHelper.SetDword(@"HKCU\Software\Microsoft\Office\16.0\Outlook\Options\General", "HideNewOutlookToggle", enable ? 1 : 0);
                break;

            // ---------- privacy ----------
            case "priv-store-search":
                StoreSearchBlock(enable);
                break;

            // ---------- power ----------
            case "power-standby-fix":
                RegHelper.SetDword(@"HKCU\SOFTWARE\Policies\Microsoft\Power\PowerSettings\f15576e8-98b7-4186-b944-eafa664402d9", "ACSettingIndex", enable ? 1 : 0);
                break;
            case "power-s3-sleep":
                if (enable) RegHelper.SetDword(@"HKLM\SYSTEM\CurrentControlSet\Control\Power", "PlatformAoAcOverride", 0);
                else RegHelper.DeleteValue(@"HKLM\SYSTEM\CurrentControlSet\Control\Power", "PlatformAoAcOverride");
                break;

            // ---------- advanced ----------
            case "adv-razer-block":
                RegHelper.SetDword(@"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\DriverSearching", "SearchOrderConfig", enable ? 0 : 1);
                RegHelper.SetDword(@"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Device Installer", "DisableCoInstallers", enable ? 1 : 0);
                break;
            case "adv-detailed-bsod":
                RegHelper.SetDword(@"HKLM\SYSTEM\CurrentControlSet\Control\CrashControl", "DisplayParameters", enable ? 1 : 0);
                RegHelper.SetDword(@"HKLM\SYSTEM\CurrentControlSet\Control\CrashControl", "DisableEmoticon", enable ? 1 : 0);
                break;
            case "adv-edge-debloat":
                EdgeDebloat(enable);
                break;
            case "adv-brave-debloat":
                BraveDebloat(enable);
                break;
            case "adv-remove-edge":
                if (enable) RemoveEdge();
                break;
            case "adv-bitlocker-off":
                if (enable) ProcHelper.PowerShell("Disable-BitLocker -MountPoint $Env:SystemDrive");
                else ProcHelper.PowerShell("Enable-BitLocker -MountPoint $Env:SystemDrive -EncryptionMethod XtsAes128 -UsedSpaceOnly -SkipHardwareTest -RecoveryPasswordProtector");
                break;

            // ---------- fixes ----------
            case "fix-services-manual":
                ServicesToManual(enable);
                break;
            case "fix-oosu":
                if (enable) RunOOSU();
                break;
        }
    }

    private const string Tcpip6 = @"HKLM\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters";

    private static readonly string[] AdobeDomains =
    {
        "genuine.adobe.com", "activate.adobe.com", "practivate.adobe.com",
        "lm.licenses.adobe.com", "na1r.services.adobe.com", "hlrcv.stage.adobe.com",
        "lmlicenses.wip4.adobe.com", "3dns-3.adobe.com", "3dns-2.adobe.com",
        "adobe-dns.adobe.com", "adobe-dns-2.adobe.com", "adobe-dns-3.adobe.com",
        "ereg.adobe.com", "activate-sea.adobe.com", "wip3.adobe.com",
        "activate-sjc0.adobe.com", "ic.adobe.io"
    };

    private static void AdobeHosts(bool block)
    {
        const string begin = "# WinOptimizer-Adobe-Block BEGIN";
        const string end = "# WinOptimizer-Adobe-Block END";
        string hosts = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", "etc", "hosts");
        try
        {
            var lines = File.Exists(hosts) ? File.ReadAllLines(hosts).ToList() : new List<string>();
            int b = lines.IndexOf(begin), e = lines.IndexOf(end);
            if (b >= 0 && e >= b) lines.RemoveRange(b, e - b + 1);
            if (block)
            {
                lines.Add(begin);
                foreach (var d in AdobeDomains) lines.Add($"0.0.0.0 {d}");
                lines.Add(end);
            }
            File.WriteAllLines(hosts, lines);
            ProcHelper.Run("ipconfig", "/flushdns");
        }
        catch { }
    }

    private static void ExplorerAutoDiscovery(bool disable)
    {
        const string shell = @"HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell";
        RegHelper.DeleteKey(shell + @"\BagMRU");
        RegHelper.DeleteKey(shell + @"\Bags");
        if (disable)
            RegHelper.SetString(shell + @"\Bags\AllFolders\Shell", "FolderType", "NotSpecified");
    }

    private static void StoreSearchBlock(bool block)
    {
        string db = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Packages", "Microsoft.WindowsStore_8wekyb3d8bbwe", "LocalState", "store.db");
        if (!File.Exists(db)) return;
        ProcHelper.Run("icacls", block ? $"\"{db}\" /deny Everyone:F" : $"\"{db}\" /remove:d Everyone");
    }

    private static void EdgeDebloat(bool on)
    {
        const string p = @"HKLM\SOFTWARE\Policies\Microsoft\Edge";
        void D(string n, int v) { if (on) RegHelper.SetDword(p, n, v); else RegHelper.DeleteValue(p, n); }
        D("PersonalizationReportingEnabled", 0);
        D("ShowRecommendationsEnabled", 0);
        D("HideFirstRunExperience", 1);
        D("UserFeedbackAllowed", 0);
        D("ConfigureDoNotTrack", 1);
        D("AlternateErrorPagesEnabled", 0);
        D("EdgeCollectionsEnabled", 0);
        D("EdgeShoppingAssistantEnabled", 0);
        D("MicrosoftEdgeInsiderPromotionEnabled", 0);
        D("ShowMicrosoftRewards", 0);
        D("WebWidgetAllowed", 0);
        D("DiagnosticData", 0);
        D("EdgeAssetDeliveryServiceEnabled", 0);
        D("CryptoWalletEnabled", 0);
        D("WalletDonationEnabled", 0);
    }

    private static void BraveDebloat(bool on)
    {
        const string p = @"HKLM\SOFTWARE\Policies\BraveSoftware\Brave";
        void D(string n, int v) { if (on) RegHelper.SetDword(p, n, v); else RegHelper.DeleteValue(p, n); }
        D("BraveRewardsDisabled", 1);
        D("BraveWalletDisabled", 1);
        D("BraveVPNDisabled", 1);
        D("BraveAIChatEnabled", 0);
        D("PasswordManagerEnabled", 0);
        D("MetricsReportingEnabled", 0);
    }

    private static void RemoveEdge()
    {
        string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        string edgeApp = Path.Combine(baseDir, "Microsoft", "Edge", "Application");
        try
        {
            if (!Directory.Exists(edgeApp)) return;
            foreach (var ver in Directory.GetDirectories(edgeApp))
            {
                string setup = Path.Combine(ver, "Installer", "setup.exe");
                if (File.Exists(setup))
                    ProcHelper.Run(setup, "--uninstall --system-level --verbose-logging --force-uninstall", 300000);
            }
        }
        catch { }
    }

    // service -> default start type (for revert)
    private static readonly (string svc, string def)[] ManualSvcs =
    {
        ("DiagTrack", "auto"), ("dmwappushservice", "auto"), ("MapsBroker", "auto"),
        ("lfsvc", "auto"), ("RetailDemo", "auto"), ("WSearch", "auto"),
        ("Fax", "demand"), ("RemoteRegistry", "disabled"), ("SysMain", "auto"),
        ("WerSvc", "auto"), ("PcaSvc", "auto"), ("DusmSvc", "auto"),
        ("diagnosticshub.standardcollector.service", "demand")
    };

    private static void ServicesToManual(bool manual)
    {
        foreach (var (svc, def) in ManualSvcs)
            ProcHelper.Run("sc", $"config {svc} start= {(manual ? "demand" : def)}");
    }

    private static void RunOOSU()
    {
        string tmp = Path.Combine(Path.GetTempPath(), "OOSU10.exe");
        ProcHelper.PowerShell(
            $"Invoke-WebRequest -Uri 'https://dl5.oo-software.com/files/ooshutup10/OOSU10.exe' -OutFile '{tmp}'; Start-Process '{tmp}'");
    }

    private static void Feature(string name, bool enable)
        => ProcHelper.Run("dism.exe",
            $"/Online /{(enable ? "Enable" : "Disable")}-Feature /FeatureName:{name} /NoRestart" + (enable ? " /All" : ""),
            300000);

    private static void OneDriveUninstall()
    {
        ProcHelper.Run("taskkill", "/f /im OneDrive.exe");
        string sys = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string sysx86 = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
        foreach (var setup in new[] { Path.Combine(sys, "OneDriveSetup.exe"), Path.Combine(sysx86, "OneDriveSetup.exe") })
            if (File.Exists(setup)) ProcHelper.Run(setup, "/uninstall");
    }

    private static void WindowsUpdateReset()
    {
        ProcHelper.Run("net", "stop wuauserv");
        ProcHelper.Run("net", "stop bits");
        string win = Environment.GetEnvironmentVariable("windir") ?? @"C:\Windows";
        string dl = Path.Combine(win, "SoftwareDistribution", "Download");
        try { if (Directory.Exists(dl)) Directory.Delete(dl, true); } catch { }
        ProcHelper.Run("net", "start bits");
        ProcHelper.Run("net", "start wuauserv");
    }

    // helpers
    private static void Service(string name, bool disable)
    {
        if (disable)
        {
            ProcHelper.Run("sc", $"stop {name}");
            ProcHelper.Run("sc", $"config {name} start= disabled");
        }
        else
        {
            ProcHelper.Run("sc", $"config {name} start= auto");
            ProcHelper.Run("sc", $"start {name}");
        }
    }

    private static void NagleAll(bool disable)
    {
        const string root = @"HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces";
        using var k = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
            @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces", false);
        if (k == null) return;
        foreach (var sub in k.GetSubKeyNames())
        {
            string path = $@"{root}\{sub}";
            if (disable)
            {
                RegHelper.SetDword(path, "TcpAckFrequency", 1);
                RegHelper.SetDword(path, "TCPNoDelay", 1);
            }
            else
            {
                RegHelper.DeleteValue(path, "TcpAckFrequency");
                RegHelper.DeleteValue(path, "TCPNoDelay");
            }
        }
    }

    // best-effort: detect whether the tweak is already applied on the system
    public static bool? IsApplied(TweakItem t) => t.Id switch
    {
        "perf-visual" => RegHelper.GetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting") == 2,
        "perf-bg-apps" => RegHelper.GetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", "GlobalUserDisabled") == 1,
        "perf-storage-sense" => RegHelper.GetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\StorageSense\Parameters\StoragePolicy", "01") == 1,
        "priv-telemetry" => RegHelper.GetDword(@"HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry") == 0,
        "priv-ad-id" => RegHelper.GetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled") == 0,
        "priv-start-ads" => RegHelper.GetDword(CDM, "SystemPaneSuggestionsEnabled") == 0,
        "ui-dark" => RegHelper.GetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme") == 0,
        "ui-file-ext" => RegHelper.GetDword(Advanced, "HideFileExt") == 0,
        "ui-taskbar-left" => RegHelper.GetDword(Advanced, "TaskbarAl") == 0,
        "game-mode" => RegHelper.GetDword(@"HKCU\Software\Microsoft\GameBar", "AutoGameModeEnabled") == 1,
        "game-hags" => RegHelper.GetDword(@"HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers", "HwSchMode") == 2,
        "game-fso" => RegHelper.GetDword(@"HKCU\System\GameConfigStore", "GameDVR_DXGIHonorFSEWindowsCompatible") == 1,
        "game-mpo" => RegHelper.GetDword(@"HKLM\SOFTWARE\Microsoft\Windows\Dwm", "OverlayTestMode") == 5,
        "net-ipv4-pref" => RegHelper.GetDword(Tcpip6, "DisabledComponents") == 32,
        "net-teredo" => RegHelper.GetDword(Tcpip6, "DisabledComponents") == 1,
        "net-ipv6-off" => RegHelper.GetDword(Tcpip6, "DisabledComponents") == 255,
        "ui-battery-percent" => RegHelper.GetDword(Advanced, "IsBatteryPercentageEnabled") == 1,
        "ui-no-notifications" => RegHelper.GetDword(@"HKCU\Software\Microsoft\Windows\CurrentVersion\PushNotifications", "ToastEnabled") == 0,
        "ui-login-blur" => RegHelper.GetDword(@"HKLM\SOFTWARE\Policies\Microsoft\Windows\System", "DisableAcrylicBackgroundOnLogon") == 1,
        "power-s3-sleep" => RegHelper.GetDword(@"HKLM\SYSTEM\CurrentControlSet\Control\Power", "PlatformAoAcOverride") == 0,
        "adv-detailed-bsod" => RegHelper.GetDword(@"HKLM\SYSTEM\CurrentControlSet\Control\CrashControl", "DisplayParameters") == 1,
        "pers-new-outlook" => RegHelper.GetDword(@"HKCU\SOFTWARE\Microsoft\Office\16.0\Outlook\Preferences", "UseNewOutlook") == 0,
        _ => null // desconhecido
    };
}
