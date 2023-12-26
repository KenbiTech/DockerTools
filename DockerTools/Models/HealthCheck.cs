namespace Kenbi.DockerTools.Models;

/// <summary>
/// Allows for configuring the health check on the container.
/// This allows Docker to verify if the container has initialized
/// correctly. See container documentation to know how to set this up.
/// </summary>
internal class HealthCheck
{
    /// <summary>
    /// Command to execute on the container.
    /// </summary>
    internal string Command { get; init; }

    /// <summary>
    /// How long to wait in between attempts.
    /// </summary>
    internal TimeSpan Interval { get; init; } = new(0, 0, 0);

    /// <summary>
    /// Time to wait for a reply.
    /// </summary>
    internal TimeSpan Timeout { get; init; } = new(0, 0, 0);

    /// <summary>
    /// Number of attempts.
    /// </summary>
    internal int Retries { get; init; } = 0;

    /// <summary>
    /// Time in seconds to wait before first attempting to check health.
    /// </summary>
    internal long StartPeriod { get; init; } = 0;
}