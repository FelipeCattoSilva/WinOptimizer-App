using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WinOptimizer.Models;
using WinOptimizer.Services;

namespace WinOptimizer.Views;

public partial class LogsView : UserControl
{
    private readonly Paragraph _para = new() { Margin = new Thickness(0) };

    public LogsView(AppState state)
    {
        InitializeComponent();

        LogBox.Document = new FlowDocument(_para)
        {
            FontFamily = LogBox.FontFamily,
            FontSize = LogBox.FontSize,
            PagePadding = new Thickness(0)
        };

        foreach (var e in Log.Entries) Append(e);
        UpdateEmpty();
        ScrollIfWanted();

        Log.Entries.CollectionChanged += OnLogChanged;
        Unloaded += (_, _) => Log.Entries.CollectionChanged -= OnLogChanged;
    }

    private void OnLogChanged(object? s, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            foreach (LogEntry entry in e.NewItems) Append(entry);
        else if (e.Action == NotifyCollectionChangedAction.Reset)
            _para.Inlines.Clear();
        else if (e.Action == NotifyCollectionChangedAction.Remove)
            // trim of the oldest line (cap reached): drop first 4 inlines (time/level/text/break)
            for (int i = 0; i < 4 && _para.Inlines.FirstInline != null; i++)
                _para.Inlines.Remove(_para.Inlines.FirstInline);

        UpdateEmpty();
        ScrollIfWanted();
    }

    private void Append(LogEntry e)
    {
        _para.Inlines.Add(new Run(e.TimeText + "  ") { Foreground = Res.B("MutedForegroundBrush") });
        _para.Inlines.Add(new Run(e.LevelText.PadRight(5)) { Foreground = LevelBrush(e.Level), FontWeight = FontWeights.SemiBold });
        _para.Inlines.Add(new Run(e.Text) { Foreground = Res.B("ForegroundBrush") });
        _para.Inlines.Add(new LineBreak());
    }

    private static Brush LevelBrush(LogLevel level) => level switch
    {
        LogLevel.Action => Res.B("PrimaryBrush"),
        LogLevel.Exec => Res.B("ForegroundBrush"),
        LogLevel.Reg => Res.B("AccentBrush"),
        LogLevel.Result => Res.B("SuccessBrush"),
        LogLevel.Warn => Res.B("WarningBrush"),
        LogLevel.Error => Res.B("DestructiveBrush"),
        _ => Res.B("MutedForegroundBrush")
    };

    private void ScrollIfWanted()
    {
        if (AutoScroll.IsChecked == true) Dispatcher.BeginInvoke(() => LogBox.ScrollToEnd());
    }

    private void UpdateEmpty()
        => EmptyState.Visibility = Log.Entries.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    private void CopyAll(object sender, RoutedEventArgs e)
    {
        try { Clipboard.SetText(Log.Dump()); } catch { }
        CopyBtn.Content = Loc.T("log_copied");
    }

    private void ClearAll(object sender, RoutedEventArgs e) => Log.Clear();
}
