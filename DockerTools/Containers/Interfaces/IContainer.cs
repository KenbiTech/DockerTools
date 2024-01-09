using Kenbi.DockerTools.Reports;
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
    internal void UpsertParameters(IContainerParameters parameters);

    /// <summary>
    /// Gets a formatted list of environment variables.
    /// Should be properly formatted for use with Docker.
    /// </summary>
    /// <returns>A list of environment variables.</returns>
    internal IList<string> GetEnvironmentVariables();

    /// <summary>
    /// Runs post-start operations like database setup scripts.
    /// </summary>
    /// <param name="client">The DockerTools client that initialized the container.</param>
    /// <param name="id">The container id.</param>
    /// <param name="token">A cancellation token. Optional.</param>
    internal Task PerformPostStartOperationsAsync(DockerToolsClient client, string id, CancellationToken token = default);
    
    /// <summary>
    /// Executes a command inside the container.
    /// </summary>
    /// <param name="client">The DockerTools client that initialized the container.</param>
    /// <param name="id">The container id.</param>
    /// <param name="command">The command to execute inside the container.</param>
    /// <param name="token">A cancellation token. Optional.</param>
    internal Task<CommandExecutionReport> ExecuteCommandAsync(DockerToolsClient client, string id, string command, CancellationToken token = default);
}