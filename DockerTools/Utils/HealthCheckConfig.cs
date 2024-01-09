namespace Kenbi.DockerTools.Utils;

/// <summary>
/// Healthcheck settings
/// </summary>
public class HealthCheckConfig
{
    /// <summary>
    /// List of commands to send to the container.
    /// </summary>
    public IList<string> Test { get; set; }

    /// <summary>
    /// The amount of time between attempts.
    /// </summary>
    public TimeSpan Interval { get; set; }

    /// <summary>
    /// How long to wait until assuming command failed.
    /// </summary>
    public TimeSpan Timeout { get; set; }

    /// <summary>
    /// Number of attempts.
    /// </summary>
    public long Retries { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public long StartPeriod { get; set; }
}