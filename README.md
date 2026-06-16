# WinOptimizer

A native desktop app for **Windows 10 and 11** with optimization, cleanup and debloat tools. Lightweight, transparent and open source — every action is traceable and reversible, no obscure registry "magic".

> Built with **C# + WPF + .NET 8**, with a custom dark theme. Runs elevated as administrator.

---

## ✨ Features

- **Dashboard** — real-time CPU, RAM, disk, network and GPU (PerformanceCounter + WMI), system summary and an optimization score.
- **Optimizations** — ~58 real registry/service/powercfg tweaks across 10 categories, each with a risk badge (Safe / Moderate / Advanced) and fully reversible:
  - Performance · Privacy · Interface · Network · Power · Gaming · Personalization · Advanced · Windows Features · Fixes
- **Presets** — ready-made bundles (Balanced, Minimal, Max Performance, Max Privacy, Gamer) with a popup detailing what each one does, its risk, and whether a restart is required.
- **Install Apps** — installs popular apps via `winget` (Chrome, Brave, Discord, Steam, Epic, NVIDIA, OBS, VS Code and more), grouped by category, with a live log.
- **Cleanup** — removes temp files, Windows Update cache, recycle bin, thumbnails and old logs. Shows real space freed.
- **Startup & Services** — lists real services (`ServiceController`) and registry boot items, with core system services protected.
- **Debloat** — removes pre-installed Appx apps via `Remove-AppxPackage`, with a "remove all safe" preset.
- **Restore point** — creates a real restore point before risky changes.
- **Persistence** — toggle state saved to `%AppData%\WinOptimizer\settings.json`.

## 🛡️ Safety & reversibility

- Every tweak knows how to revert to the Windows default — just turn the toggle off.
- Protected items (core services, Microsoft Store) can't be removed by accident.
- winget installs verify the installer hash; bypass only with an explicit confirmation.
- Creating a **restore point** before applying Advanced / Debloat changes is recommended.

## 📦 Download

Grab `WinOptimizer.exe` from the [**Releases**](../../releases) tab. It's a **self-contained single-file** executable — runs on any Windows 10/11 x64 with no .NET install required.

> Windows SmartScreen may warn that the app is unrecognized (it isn't code-signed). Click **More info → Run anyway**.

## 🔧 Build from source

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
# Run in development
dotnet run --project WinOptimizer

# Produce the portable single-file executable
dotnet publish WinOptimizer -c Release -r win-x64 --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

The final `.exe` lands in `WinOptimizer/bin/Release/net8.0-windows/win-x64/publish/`.

## 🧰 Stack

| | |
|---|---|
| Language | C# / .NET 8 |
| UI | WPF (custom dark theme, own ResourceDictionary) |
| Elevation | `requireAdministrator` manifest |
| Persistence | JSON in `%AppData%\WinOptimizer` |
| Distribution | self-contained single-file publish |

## 🙏 Credits

Inspired and validated by well-established projects in this space:

- [ChrisTitusTech/winutil](https://github.com/ChrisTitusTech/winutil) — the category's biggest reference
- [Raphire/Win11Debloat](https://github.com/Raphire/Win11Debloat) — focused debloat for Win10/11
- [hellzerg/optimizer](https://github.com/hellzerg/optimizer) · [optimizerNXT](https://github.com/hellzerg/optimizerNXT)
- [undergroundwires/privacy.sexy](https://github.com/undergroundwires/privacy.sexy) — reversible, transparent tweaks
- [O&O ShutUp10++](https://www.oo-software.com/en/shutup10) — reference for risk-badged toggle UX

## ⚠️ Disclaimer

Use at your own risk. Optimizations modify the system registry, services and apps. Although reversible, creating a restore point first is recommended. Personal project, no warranty.

## 📄 License

MIT.
