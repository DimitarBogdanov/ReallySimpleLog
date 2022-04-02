namespace ReallySimpleLog;

/// <summary>
/// Describes how the old logs should be handled when reusing a file path.
/// </summary>
public enum PreviousLogPreserveMode
{
    /// <summary>
    /// Clear the contents of the log file and start anew.
    /// </summary>
    Override,
    
    /// <summary>
    /// Move the previous contents of the log file to another file (logfileName + "-PREVIOUS")
    /// </summary>
    MoveToAnotherFile,
    
    /// <summary>
    /// Just keep writing to the log file.
    /// </summary>
    Append
}