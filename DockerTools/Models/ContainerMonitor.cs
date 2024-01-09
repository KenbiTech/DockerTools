using Docker.DotNet.Models;
using Kenbi.DockerTools.Containers.Interfaces;
using Kenbi.DockerTools.Models.Interfaces;
using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Models;

/// <inheritdoc />
public sealed class ContainerMonitor : IContainerMonitor
{
    /// <inheritdoc />
    public string Id { get; private set; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public CreateContainerParameters Configuration { get; }

    /// <inheritdoc />
    public bool CreatedByDockerTools { get; private set; }

    /// <inheritdoc />
    public List<PortConfiguration> PortBindings { get; private set; }

    /// <inheritdoc />
    public ContainerMonitorAdditionalInformation AdditionalInformation { get; private set; }

    /// <inheritdoc />
    public IContainer Container { get; private set; }

    public DockerToolsClient Client { get; }

    internal ContainerMonitor(string name, CreateContainerParameters configuration, IContainer container, DockerToolsClient client)
    {
        this.Name = name;
        this.Configuration = configuration;
        this.PortBindings = configuration.HostConfig.PortBindings.ConvertToPortConfiguration().ToList();
        this.Container = container;
        this.Client = client;
    }

    void IContainerMonitor.AddId(string id, bool newContainer)
    {
        if (this.Id != null)
        {
            return;
        }

        this.Id = id;
        this.CreatedByDockerTools = newContainer;
    }

    void IContainerMonitor.ReplacePortBindings(IEnumerable<PortConfiguration> portConfigurations)
    {
        this.PortBindings = portConfigurations.ToList();
    }

    void IContainerMonitor.AddAdditionalInformation()
    {
        string? connectionString = null;

        if (this.Container is IDatabaseContainer container)
        {
            connectionString = container.CreateConnectionString(this.PortBindings[0].Host);
        }

        this.AdditionalInformation = new ContainerMonitorAdditionalInformation
        {
            ConnectionString = connectionString
        };
    }
}