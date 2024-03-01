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
    /// Also applies to the amount of time awaited
    /// before first attempt.
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
    /// Provides initialization time for containers that need time to bootstrap.
    /// Probe failure during that period will not be counted towards the maximum number of retries.
    /// However, if a health check succeeds during the start period, the container is considered started
    /// and all consecutive failures will be counted towards the maximum number of retries.
    /// </summary>
    internal long StartPeriod { get; init; } = 0;
}