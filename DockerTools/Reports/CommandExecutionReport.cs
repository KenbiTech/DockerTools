namespace Kenbi.DockerTools.Reports;

/// <summary>
/// Lists information about the result of
/// the command run on the container.
/// </summary>
public sealed class CommandExecutionReport
{
    internal CommandExecutionReport(string stdout, string stderr)
    {
        if (!string.IsNullOrWhiteSpace(stderr))
        {
            RanSuccessfully = false;
            ErrorMessage = stderr;
            return;
        }

        RanSuccessfully = true;
        ReturnMessage = stdout;
    }

    /// <summary>
    /// Returns true if command ran successfully, false if it didn't.
    /// If true, output message will be declared on <see cref="ReturnMessage"/>.
    /// If false, output message will be declared on <see cref="ErrorMessage"/>.
    /// </summary>
    public bool RanSuccessfully { get; init; }

    /// <summary>
    /// Populated with the return of the command if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Populated with the return of the command if the operation succeeded.
    /// </summary>
    public string? ReturnMessage { get; init; }
}