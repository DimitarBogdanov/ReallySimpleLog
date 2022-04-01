namespace ReallySimpleLog;

/// <summary>
/// Base logger class.
/// </summary>
public class Logger
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="path">The file path for the log. Set to null for no file log.</param>
    /// <param name="outputToConsole">Set to true if you want the messages to be written to stdout.</param>
    public Logger(string? path, bool outputToConsole)
    {
        if (path == null && !outputToConsole)
        {
            throw new ArgumentException(
                "A logger that doesn't output to file nor console cannot be created, as it does nothing."
            );
        }

        if (path != null && Directory.Exists(path))
        {
            throw new ArgumentException("Log path must be a file");
        }
        
        if (path != null && !File.Exists(path))
        {
            File.Create(path, 0).Close();
        }
        
        _path = path;
        _outputToConsole = outputToConsole;
    }

    public bool OutputDatesInConsole { get; set; } = false;
    public bool OutputDatesInFile { get; set; } = true;
    
    private readonly string? _path;
    private readonly bool _outputToConsole;
    
    /// <summary>
    /// Logs with an "INFO" tag in white.
    /// </summary>
    public virtual void Info(params object[]? content) => Task.Run(() => Log("INFO", ConsoleColor.White, content));
    
    /// <summary>
    /// Logs with an "ERROR" tag in red.
    /// </summary>
    public virtual void Error(params object[]? content) => Task.Run(() => Log("ERROR", ConsoleColor.Red, content));
    
    /// <summary>
    /// Logs with a "WARN" tag in yellow.
    /// </summary>
    public virtual void Warn(params object[]? content) => Task.Run(() => Log("WARN", ConsoleColor.Yellow, content));
    
    /// <summary>
    /// Logs with a "DEBUG" tag in green.
    /// </summary>
    public virtual void Debug(params object[]? content) => Task.Run(() => Log("DEBUG", ConsoleColor.Green, content));

    public void Exception(Exception ex) => Task.Run(() => Log(
            $"Exception thrown in {ex.Source ?? "unknown"}",
            ConsoleColor.Red,
            $"{ex.Message}\n{ex.StackTrace}"
            )
        );

    /// <summary>
    /// Internal logging method - the brains of the entire library.
    /// </summary>
    /// <param name="tag">The tag to put in square brackets.</param>
    /// <param name="color">The color to print the message in, if console printing is enabled.</param>
    /// <param name="content">What to write.</param>
    protected void Log(string tag, ConsoleColor color, params object[]? content)
    {
        string message = content != null ? String.Join(", ", content) : "<null>";
        message = $"[{tag}] {message}";
        string datedMessage = $"{DateTime.Now} {message}";
        
        if (_outputToConsole)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(OutputDatesInConsole ? datedMessage : message);
            Console.ResetColor();
        }

        if (_path == null) return;
        try
        {
            File.AppendAllLines(_path, new[] {OutputDatesInFile ? datedMessage : message});
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SimpleLog :: failed to write to file: {ex.Message}");
            throw;
        }
    }
}