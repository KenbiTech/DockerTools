namespace Kenbi.DockerTools.Models;

/// <summary>
/// Allows for configuring the container ports.
/// </summary>
internal class PortConfiguration
{
    /// <summary>
    /// Port exposed by Docker for access to container.
    /// Set automatically by Docker.
    /// </summary>
    internal string? Host { get; set; }

    /// <summary>
    /// Port on the container side. Must match port
    /// that container service is listening on.
    /// </summary>
    internal string Container { get; set; }
}