using Docker.DotNet;
using Docker.DotNet.Models;
using Kenbi.DockerTools.Containers;
using Kenbi.DockerTools.Containers.Templates;
using Kenbi.DockerTools.Exceptions;
using Kenbi.DockerTools.Options.Connection;
using Kenbi.DockerTools.Options.Connection.ConnectionTypes;
using Kenbi.DockerTools.Options.Container;

namespace Kenbi.DockerTools;

/// <summary>
/// Entry point to creating a new container instance.
/// </summary>
public class DockerTools<T> where T : IContainerTemplate, new()
{
    private DockerClient? _client;
    private DockerToolsConnectionType _connectionType = new AutoDetectConnection();
    private bool _useValkyrie;
    private readonly Guid _instanceId = Guid.NewGuid();

    private string? Id { get; set; }
    private T Container { get; } = new();

    /// <summary>
    /// Allows for overriding default parameters.
    /// </summary>
    /// <param name="options">The parameters that can be overriden.</param>
    /// <returns>The supplied <see cref="DockerTools{T}"/> with the requested changes.</returns>
    public DockerTools<T> WithParameters(Action<DockerToolsContainerOptions> options = default!)
    {
        var opts = new DockerToolsContainerOptions();
        options.Invoke(opts);

        this.Container.ReplaceDefaultParameters(opts);

        return this;
    }

    /// <summary>
    /// Allows for overriding the default connection to the Docker instance.
    /// Useful when connecting to a remote Docker, e.g., on AWS.
    /// </summary>
    /// <param name="options">The parameters that can be overriden.</param>
    /// <returns>The supplied <see cref="DockerTools{T}"/> with the requested changes.</returns>
    public DockerTools<T> WithDockerHostConnection(Action<DockerToolsConnectionOptions> options = default!)
    {
        var opts = new DockerToolsConnectionOptions();
        options.Invoke(opts);

        if (opts.ConnectionType != null)
        {
            this._connectionType = opts.ConnectionType;
        }

        return this;
    }

    /// <summary>
    /// Initializes a watcher that will automatically remove the container after a set amount of time.
    /// Use this when you cannot dispose of the container instance properly.
    /// </summary>
    /// <param name="value">Set to true to enable.</param>
    /// <returns>The supplied <see cref="DockerTools{T}"/> with the requested changes.</returns>
    public DockerTools<T> WithCleanUp(bool value = false)
    {
        this._useValkyrie = value;

        return this;
    }

    /// <summary>
    /// Creates a container instance representing the remote container.
    /// </summary>
    /// <param name="token">A cancellation token. Optional.</param>
    /// <returns>An instance of <see cref="IContainer{T}"/>.</returns>
    /// <exception cref="DockerUnreachableException">Docker is not available on the requested URI, or DockerTools is unable to properly detect which connection to use.</exception>
    /// <exception cref="UnableToPullImageException">Invalid image:tag provided, repository not accessible, or network problem.</exception>
    /// <exception cref="UnableToCreateContainerException">Container could not be created on Docker host.</exception>
    /// <exception cref="UnableToStartContainerException">Container could not be started on Docker host.</exception>
    /// <exception cref="ContainerIsNotHealthyException">Container has exceeded the allotted time given to start up correctly.</exception>
    public async Task<IContainer<T>> CreateAsync(CancellationToken token = default)
    {
        try
        {
            return await InternalCreateAsync(token);
        }
        catch
        {
            if (this._client == null)
            {
                throw;
            }

            if (this.Id == null)
            {
                await Task.Run(() => this._client.Dispose(), token);
                throw;
            }

            await PerformCleanupAsync(token);

            throw;
        }
    }

    private async Task<IContainer<T>> InternalCreateAsync(CancellationToken token)
    {
        if (this._connectionType is AutoDetectConnection connection)
        {
            this._client = await connection.CreateClientAsync(token);
        }
        else
        {
            this._client = await (this._connectionType as RemoteApiConnection)!.CreateClientAsync(token);
        }

        await Operations.PullImageOperations.PullImageAsync(this._client, this.Container.Image, this.Container.Tag, token);

        this.Id = await Operations.CreateContainerOperations.CreateContainerAsync(this._client, this.Container, this._instanceId, token);

        if (this.Id == null)
        {
            throw new UnableToCreateContainerException();
        }

        if (!await Operations.StartContainerOperations.TryStartContainerAsync(this._client, this.Id, token))
        {
            throw new UnableToStartContainerException();
        }

        if (!await Operations.StartContainerOperations.IsContainerHealthy(this._client, this.Id, this.Container, token))
        {
            throw new ContainerIsNotHealthyException();
        }

        var ports = await Operations.StartContainerOperations.GetRunningPortsAsync(this._client, this.Id, token);

        var result = await this.Container.PerformPostStartOperationsAsync(this._client, this.Id, token);

        if (!result.Success)
        {
            throw new UnableToSetupContainerException(result.ErrorMessage!);
        }

        if (this._useValkyrie)
        {
            await StartValkyrieAsync(token);
        }

        return new Container<T>(this.Id, this._client, this.Container, ports.First().Host!);
    }

    private async Task StartValkyrieAsync(CancellationToken token)
    {
        IContainerTemplate container = new Valkyrie();
        container.ReplaceDefaultParameters(new DockerToolsContainerOptions().WithInstanceId(this._instanceId));
        await Operations.PullImageOperations.PullImageAsync(this._client!, container.Image, container.Tag, token);
        var id = await Operations.CreateContainerOperations.CreateContainerAsync(this._client!, container, this._instanceId, token);
        if (!await Operations.StartContainerOperations.TryStartContainerAsync(this._client!, id, token))
        {
            throw new UnableToStartValkyrieException();
        }
    }

    private async Task PerformCleanupAsync(CancellationToken token)
    {
        try
        {
            await this._client!.Containers.KillContainerAsync(this.Id, new ContainerKillParameters(), token);
        }
        catch
        {
            // If container is failing to start, attempting to stop it will throw
            // an error; nothing needs to be done here, just gracefully handle it 
        }

        await this._client!.Containers.RemoveContainerAsync(
            this.Id,
            new ContainerRemoveParameters
            {
                Force = true,
                RemoveVolumes = true
            },
            token);

        await Task.Run(() => this._client.Dispose(), token);
    }
}