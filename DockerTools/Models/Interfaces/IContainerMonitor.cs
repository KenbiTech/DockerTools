using Docker.DotNet.Models;
using Kenbi.DockerTools.Containers.Interfaces;
using Kenbi.DockerTools.Reports;
using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Models.Interfaces;

/// <summary>
/// Gathers and contains all information about a container.
/// </summary>
public interface IContainerMonitor
{
    /// <summary>
    /// Container id. Set on creation.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Reference name. Uses the image name, not
    /// the container name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The container configuration.
    /// </summary>
    public CreateContainerParameters Configuration { get; }

    /// <summary>
    /// Indicates whether or not the container was created by the tool,
    /// or merely monitored by it. Monitored containers are not removed.
    /// </summary>
    public bool CreatedByDockerTools { get; }

    /// <summary>
    /// List all mappings between host and container ports.
    /// </summary>
    public List<PortConfiguration> PortBindings { get; }

    /// <summary>
    /// Additional information pertaining the container,
    /// e.g., connection string to a database.
    /// </summary>
    public ContainerMonitorAdditionalInformation AdditionalInformation { get; }

    /// <summary>
    /// The type of container generated.
    /// </summary>
    public IContainer Container { get; }

    internal DockerToolsClient Client { get; }
    
    /// <summary>
    /// Executes a command inside the container.
    /// If container is <see cref="IDatabaseContainer"/>, command is run as a database script.
    /// </summary>
    /// <param name="command">The command to execute inside the container.</param>
    /// <param name="token">A cancellation token. Optional.</param>
    /// <returns></returns>
    public Task<CommandExecutionReport> ExecuteCommandAsync(string command, CancellationToken token = default) => this.Container.ExecuteCommandAsync(this.Client, this.Id, command, token);

    internal void AddId(string id, bool newContainer);

    internal void ReplacePortBindings(IEnumerable<PortConfiguration> portConfigurations);

    internal void AddAdditionalInformation();

    internal Task PerformPostStartOperationsAsync(CancellationToken token) => this.Container.PerformPostStartOperationsAsync(this.Client, this.Id, token);
}