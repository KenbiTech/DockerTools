using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Containers.Interfaces;

/// <summary>
/// Base container information.
/// </summary>
public interface IContainer
{
    /// <summary>
    /// The image name, as found on DockerHub.
    /// </summary>
    public string Image { get; }

    /// <summary>
    /// The image tag, which references
    /// the version of the image to use.
    /// </summary>
    public string Tag { get; }

    /// <summary>
    /// The ports to map from the container to the host.
    /// </summary>
    public IEnumerable<PortConfiguration> Ports { get; }
    
    /// <summary>
    /// The health check configuration.
    /// </summary>
    public HealthCheckConfig HealthCheck { get; }

    /// <summary>
    /// Allows for parameter replacement. Depending on
    /// the scenario, some parameters might not be replaceable.
    /// </summary>
    /// <param name="parameters">List of parameters to replace.</param>
    void UpsertParameters(IContainerParameters parameters);

    /// <summary>
    /// Gets a formatted list of environment variables.
    /// Should be properly formatted for use with Docker.
    /// </summary>
    /// <returns>A list of environment variables.</returns>
    IList<string> GetEnvironmentVariables();
}