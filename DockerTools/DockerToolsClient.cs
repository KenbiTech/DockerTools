using Docker.DotNet;
using Kenbi.DockerTools.Containers;
using Kenbi.DockerTools.Containers.Interfaces;
using Kenbi.DockerTools.Models.Interfaces;

namespace Kenbi.DockerTools;

/// <summary>
/// 
/// </summary>
public sealed class DockerToolsClient : IAsyncDisposable
{
    internal readonly DockerClient Client;
    internal readonly Guid InstanceId;

    /// <summary>
    /// List of all configured containers.
    /// </summary>
    public IList<IContainerMonitor> Containers { get; private set; } = new List<IContainerMonitor>();

    internal DockerToolsClient()
    {
        this.Client = new DockerClientConfiguration().CreateClient();
        this.InstanceId = Guid.NewGuid();
    }

    internal DockerToolsClient(Uri uri)
    {
        this.Client = new DockerClientConfiguration(uri).CreateClient();
        this.InstanceId = Guid.NewGuid();
    }

    /// <summary>
    /// Starts the container setup process.
    /// </summary>
    /// <typeparam name="T">Must inherit from <see cref="IContainer"/></typeparam>
    /// <returns></returns>
    public IContainerSetup<T> Setup<T>() where T : class, IContainer, new()
    {
        var container = (IContainer)new T();

        return new ContainerSetup<T>(this, container);
    }

    /// <summary>
    /// Starts the container setup process.
    /// </summary>
    /// <typeparam name="T">Must inherit from <see cref="IContainer"/></typeparam>
    /// <typeparam name="TParameters">Must inherit from <see cref="IContainerParameters"/></typeparam>
    /// <returns></returns>
    public IContainerSetup<T, TParameters> Setup<T, TParameters>() where T : class, IContainer, new() where TParameters : class, IContainerParameters
    {
        var container = (IContainer)new T();

        return new ContainerSetup<T, TParameters>(this, container);
    }

    internal void TryAddConfiguration(IContainerMonitor container, out bool result)
    {
        try
        {
            this.Containers.Add(container);
            result = true;
        }
        catch
        {
            result = false;
        }
    }

    internal void RemoveAllContainers()
    {
        this.Containers = new List<IContainerMonitor>();
    }

    /// <summary>
    /// Frees existing resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await Task.Run(() => this.Client.Dispose());
        GC.SuppressFinalize(this);
    }
}