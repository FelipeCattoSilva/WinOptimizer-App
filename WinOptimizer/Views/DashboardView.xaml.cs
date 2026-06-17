using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using WinOptimizer.Models;
using WinOptimizer.Services;

namespace WinOptimizer.Views;

public partial class DashboardView : UserControl
{
    private const int History = 60;
    private readonly DispatcherTimer _timer;
    private readonly SystemInfoService _sys = new();
    private readonly AppState _state;

    private readonly double[] _cpu = new double[History];
    private readonly double[] _ram = new double[History];
    private readonly double[] _disk = new double[History];
    private readonly double[] _net = new double[History];
    private readonly double[] _gpu = new double[History];

    public DashboardView(AppState state)
    {
        InitializeComponent();
        _state = state;

        GpuName.Text = SummaryGpu();
        BuildSummary();
        ScoreFromTweaks();

        foreach (var pl in new[] { CpuLine, RamLine, DiskLine, NetLine, GpuLine }) pl.Stretch = Stretch.Fill;

        Tick(null, EventArgs.Empty);
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += Tick;
        _timer.Start();

        Unloaded += (_, _) => { _timer.Stop(); _sys.Dispose(); };
    }

    private void Tick(object? sender, EventArgs e)
    {
        Push(_cpu, _sys.Cpu());
        Push(_ram, _sys.Ram());
        Push(_disk, _sys.Disk());
        int net = _sys.NetMbps();
        Push(_net, net);
        Push(_gpu, _sys.Gpu());

        CpuVal.Text = ((int)_cpu[^1]).ToString();
        RamVal.Text = ((int)_ram[^1]).ToString();
        DiskVal.Text = ((int)_disk[^1]).ToString();
        NetVal.Text = net.ToString();
        GpuVal.Text = ((int)_gpu[^1]).ToString();

        CpuLine.Points = ToPoints(_cpu, 100);
        RamLine.Points = ToPoints(_ram, 100);
        DiskLine.Points = ToPoints(_disk, 100);
        NetLine.Points = ToPoints(_net, Math.Max(10, _net.Max()));
        GpuLine.Points = ToPoints(_gpu, 100);
    }

    private static void Push(double[] s, double v)
    {
        Array.Copy(s, 1, s, 0, s.Length - 1);
        s[^1] = v;
    }

    private static PointCollection ToPoints(double[] s, double max)
    {
        var pts = new PointCollection(s.Length);
        for (int i = 0; i < s.Length; i++)
            pts.Add(new Point(i, max - Math.Clamp(s[i], 0, max)));
        return pts;
    }

    private string SummaryGpu()
    {
        var gpu = SystemInfoService.Summary().FirstOrDefault(r => r.Item1 == "sum_gpu");
        return string.IsNullOrEmpty(gpu.Item2) ? "GPU" : gpu.Item2;
    }

    private void BuildSummary()
    {
        var muted = (Brush)Application.Current.Resources["MutedForegroundBrush"];
        foreach (var (k, val) in SystemInfoService.Summary())
        {
            var border = new Border
            {
                BorderBrush = (Brush)Application.Current.Resources["BorderBrush"],
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(0, 0, 16, 8),
                Margin = new Thickness(0, 0, 16, 6)
            };
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var key = new TextBlock { Text = Loc.T(k), FontSize = 13, Foreground = muted, VerticalAlignment = VerticalAlignment.Center };
            var val2 = new TextBlock
            {
                Text = val, FontSize = 13, FontWeight = FontWeights.Medium,
                HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis, TextAlignment = TextAlignment.Right,
                Margin = new Thickness(12, 0, 0, 0), ToolTip = val
            };
            Grid.SetColumn(key, 0);
            Grid.SetColumn(val2, 1);
            grid.Children.Add(key);
            grid.Children.Add(val2);
            border.Child = grid;
            SummaryGrid.Children.Add(border);
        }
    }

    // score = % of safe tweaks applied
    private void ScoreFromTweaks()
    {
        var seguros = _state.Tweaks.Where(t => t.Risco == RiscoTweak.Seguro
            && (t.AlvoOS == null || t.AlvoOS == _state.Version)).ToList();
        int ativos = seguros.Count(t => t.Ativo);
        int score = seguros.Count == 0 ? 0 : (int)Math.Round(100.0 * ativos / seguros.Count);

        ScoreValue.Text = score.ToString();
        ScoreSub.Text = string.Format(Loc.T("dash_score_sub"), ativos, seguros.Count);
        DrawScoreArc(score);
    }

    private void DrawScoreArc(int score)
    {
        const double r = 60, cx = 65, cy = 65;
        double sweep = Math.Max(0.01, score / 100.0 * 360.0);
        double start = -90, end = start + sweep;

        Point P(double deg)
        {
            double rad = deg * Math.PI / 180.0;
            return new Point(cx + r * Math.Cos(rad), cy + r * Math.Sin(rad));
        }

        var fig = new PathFigure { StartPoint = P(start), IsClosed = false };
        fig.Segments.Add(new ArcSegment(P(end), new Size(r, r), 0, sweep > 180, SweepDirection.Clockwise, true));
        ScoreArc.Data = new PathGeometry(new[] { fig });
    }
}
