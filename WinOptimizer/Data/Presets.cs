using WinOptimizer.Models;

namespace WinOptimizer.Data;

// optimization presets. each one turns on a subset of the tweaks
public static class Presets
{
    // ids in the "minimo" preset - just the safe essentials
    private static readonly HashSet<string> MinimoIds = new()
    {
        "perf-power-high", "perf-storage-sense", "priv-start-ads",
        "ui-dark", "ui-file-ext", "game-mode"
    };

    public static List<PresetDef> All() => new()
    {
        new PresetDef
        {
            Id = "equilibrado", Icone = "⚖",
            NomeEn = "Balanced", NomePt = "Equilibrado",
            ResumoEn = "Turns on every safe tweak. Recommended for most people — zero risk.",
            ResumoPt = "Ativa todos os ajustes seguros. Recomendado para a maioria — zero risco.",
            MelhoriasEn = new[]
            {
                "Snappier system (power + visual effects)",
                "Basic privacy (ads and suggestions off)",
                "Fewer background apps eating resources",
            },
            MelhoriasPt = new[]
            {
                "Sistema mais responsivo (energia + efeitos visuais)",
                "Privacidade básica (anúncios e sugestões desligados)",
                "Menos apps em segundo plano consumindo recursos",
            },
            Selector = t => t.Risco == RiscoTweak.Seguro,
        },
        new PresetDef
        {
            Id = "minimo", Icone = "🪶",
            NomeEn = "Minimal", NomePt = "Mínimo",
            ResumoEn = "Just the essentials. Minimal, 100% reversible changes.",
            ResumoPt = "Só o essencial. Mudanças mínimas e 100% reversíveis.",
            MelhoriasEn = new[]
            {
                "High performance power plan",
                "Dark mode and visible file extensions",
                "Storage Sense and Game Mode on",
            },
            MelhoriasPt = new[]
            {
                "Plano de energia de alto desempenho",
                "Modo escuro e extensões de arquivo visíveis",
                "Storage Sense e Modo de Jogo ligados",
            },
            Selector = t => MinimoIds.Contains(t.Id),
        },
        new PresetDef
        {
            Id = "desempenho", Icone = "🚀",
            NomeEn = "Max performance", NomePt = "Desempenho máximo",
            ResumoEn = "Focused on speed. Includes moderate/advanced tweaks — some need a restart.",
            ResumoPt = "Foco em velocidade. Inclui tweaks moderados/avançados — alguns pedem reinício.",
            MelhoriasEn = new[]
            {
                "Prioritizes CPU/GPU and aggressive power plans",
                "Disables background services (SysMain, memory compression)",
                "Game optimizations (HAGS left out — can hurt old GPUs)",
            },
            MelhoriasPt = new[]
            {
                "Prioriza CPU/GPU e planos de energia agressivos",
                "Desativa serviços de fundo (SysMain, compactação de memória)",
                "Otimizações de jogo (HAGS fica de fora — pode piorar GPUs antigas)",
            },
            Selector = t => (t.Categoria is "performance" or "power" or "gaming")
                            && t.Id != "game-hags",
        },
        new PresetDef
        {
            Id = "privacidade", Icone = "🔒",
            NomeEn = "Max privacy", NomePt = "Privacidade máxima",
            ResumoEn = "Cuts Microsoft telemetry, tracking and ads.",
            ResumoPt = "Corta telemetria, rastreio e anúncios da Microsoft.",
            MelhoriasEn = new[]
            {
                "Telemetry (DiagTrack) and activity history off",
                "Advertising ID, location and suggestions blocked",
                "Less data sent to Microsoft",
            },
            MelhoriasPt = new[]
            {
                "Telemetria (DiagTrack) e histórico de atividades desligados",
                "Advertising ID, localização e sugestões bloqueados",
                "Menos dados enviados à Microsoft",
            },
            Selector = t => t.Categoria == "privacy",
        },
        new PresetDef
        {
            Id = "gamer", Icone = "🎮",
            NomeEn = "Gamer", NomePt = "Gamer",
            ResumoEn = "All about FPS and low latency.",
            ResumoPt = "Tudo voltado a FPS e baixa latência.",
            MelhoriasEn = new[]
            {
                "Game Mode on, Game Bar and background recording off",
                "High/Ultimate Performance power, lower network latency",
                "HAGS NOT included (can stutter old GPUs — enable manually if you want)",
            },
            MelhoriasPt = new[]
            {
                "Modo de Jogo ligado, Game Bar e gravação em 2º plano desligados",
                "Energia em alto/Ultimate Performance, latência de rede reduzida",
                "HAGS NÃO entra (pode travar GPUs antigas — ligue manual se quiser)",
            },
            Selector = t => (t.Categoria == "gaming" && t.Id != "game-hags")
                            || t.Id is "perf-power-high" or "power-ultimate" or "net-nagle",
        },
        new PresetDef
        {
            Id = "limpar", Icone = "✖",
            NomeEn = "Clear selection", NomePt = "Limpar seleção",
            ResumoEn = "Turns off every tweak and returns to a neutral state.",
            ResumoPt = "Desativa todos os tweaks e volta ao estado neutro.",
            MelhoriasEn = new[] { "Resets all active optimizations", "Nothing is applied to the system" },
            MelhoriasPt = new[] { "Zera todas as otimizações ativas", "Nada é aplicado ao sistema" },
            Selector = _ => false, Limpar = true,
        },
    };
}
