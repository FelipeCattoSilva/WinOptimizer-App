using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace WinOptimizer.Services;

public enum LogLevel { Info, Action, Exec, Reg, Result, Warn, Error }

public sealed class LogEntry
{
    public DateTime Time { get; init; } = DateTime.Now;
    public LogLevel Level { get; init; }
    public string Text { get; init; } = "";

    public string TimeText => Time.ToString("HH:mm:ss");
    public string LevelText => Level switch
    {
        LogLevel.Info => "INFO",
        LogLevel.Action => "RUN",
        LogLevel.Exec => "EXEC",
        LogLevel.Reg => "REG",
        LogLevel.Result => "OK",
        LogLevel.Warn => "WARN",
        LogLevel.Error => "ERR",
        _ => ""
    };

    public string Line => $"{TimeText}  {LevelText,-4}  {Text}";
}

// central, UI-thread-safe activity log. every system action funnels through here.
public static class Log
{
    public const int Max = 2000;

    public static ObservableCollection<LogEntry> Entries { get; } = new();
    public static event Action<LogEntry>? Added;

    public static void Info(string t) => Add(LogLevel.Info, t);
    public static void Action(string t) => Add(LogLevel.Action, t);
    public static void Exec(string t) => Add(LogLevel.Exec, t);
    public static void Reg(string t) => Add(LogLevel.Reg, t);
    public static void Result(string t) => Add(LogLevel.Result, t);
    public static void Warn(string t) => Add(LogLevel.Warn, t);
    public static void Error(string t) => Add(LogLevel.Error, t);

    public static void Clear() => OnUi(Entries.Clear);

    public static string Dump()
    {
        var sb = new StringBuilder();
        foreach (var e in Entries) sb.AppendLine(e.Line);
        return sb.ToString();
    }

    private static void Add(LogLevel level, string text)
    {
        var entry = new LogEntry { Level = level, Text = text };
        OnUi(() =>
        {
            Entries.Add(entry);
            while (Entries.Count > Max) Entries.RemoveAt(0);
            Added?.Invoke(entry);
        });
    }

    private static void OnUi(Action a)
    {
        var app = Application.Current;
        if (app == null) { a(); return; }
        if (app.Dispatcher.CheckAccess()) a();
        else app.Dispatcher.BeginInvoke(a);
    }
}
