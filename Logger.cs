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
    /// <param name="previousLogPreserveMode">How should old logs be treated (see <see cref="PreviousLogPreserveMode"/> for more info)</param>
    public Logger(string? path, bool outputToConsole, PreviousLogPreserveMode previousLogPreserveMode)
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
        else if (File.Exists(path))
        {
            switch (previousLogPreserveMode)
            {
                case PreviousLogPreserveMode.Override:
                    File.WriteAllText(path, "");
                    break;
                
                case PreviousLogPreserveMode.MoveToAnotherFile:
                    File.WriteAllText($"{path}-PREVIOUS", File.ReadAllText(path));
                    File.WriteAllText(path, "");
                    break;
                
                case PreviousLogPreserveMode.Append:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(previousLogPreserveMode), previousLogPreserveMode, null);
            }
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
    public virtual void Info(params object[]? content) => Log("INFO", ConsoleColor.White, content);
    
    /// <summary>
    /// Logs with an "INFO" tag in white.
    /// </summary>
    public virtual void Info(object? content) => Log("INFO", ConsoleColor.White, content);
    
    /// <summary>
    /// Logs with an "ERROR" tag in red.
    /// </summary>
    public virtual void Error(params object[]? content) => Log("ERROR", ConsoleColor.Red, content);
    
    /// <summary>
    /// Logs with an "ERROR" tag in red.
    /// </summary>
    public virtual void Error(object? content) => Log("ERROR", ConsoleColor.Red, content);
    
    /// <summary>
    /// Logs with a "WARN" tag in yellow.
    /// </summary>
    public virtual void Warn(params object[]? content) => Log("WARN", ConsoleColor.Yellow, content);
    
    /// <summary>
    /// Logs with a "WARN" tag in yellow.
    /// </summary>
    public virtual void Warn(object? content) => Log("WARN", ConsoleColor.Yellow, content);
    
    /// <summary>
    /// Logs with a "DEBUG" tag in green.
    /// </summary>
    public virtual void Debug(params object[]? content) => Log("DEBUG", ConsoleColor.Green, content);
    
    /// <summary>
    /// Logs with a "DEBUG" tag in green.
    /// </summary>
    public virtual void Debug(object? content) => Log("DEBUG", ConsoleColor.Green, content);

    public void Exception(Exception ex) => Log(
            $"Exception thrown in {ex.Source ?? "unknown"}",
            ConsoleColor.Red,
            $"{ex.Message}\n{ex.StackTrace}"
        );

    /// <summary>
    /// Internal logging method - the brains of the entire library.
    /// This calls the other overload of Log (that takes in one object) with a string of the array elements separated by ", ".
    /// </summary>
    /// <param name="tag">The tag to put in square brackets.</param>
    /// <param name="color">The color to print the message in, if console printing is enabled.</param>
    /// <param name="content">What to write.</param>
    protected void Log(string tag, ConsoleColor color, params object[]? content)
    {
        Log(tag, color, content != null ? String.Join(", ", content) : null);
    }
    
    /// <summary>
    /// Internal logging method - the brains of the entire library.
    /// </summary>
    /// <param name="tag">The tag to put in square brackets.</param>
    /// <param name="color">The color to print the message in, if console printing is enabled.</param>
    /// <param name="content">What to write.</param>
    protected void Log(string tag, ConsoleColor color, object? content)
    {
        string message = content?.ToString() ?? "<null>";
        message = $"[{tag}] {message}";
        string datedMessage = $"{DateTime.Now} {message}";
        
        if (_outputToConsole)
        {
            lock (Console.Out)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(OutputDatesInConsole ? datedMessage : message);
                Console.ResetColor();
            }
        }
        
        if (_path == null) return;

        lock (_path)
        {
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
}