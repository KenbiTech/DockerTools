using Docker.DotNet;
using Docker.DotNet.Models;
using Kenbi.DockerTools.Containers.Templates;
using Kenbi.DockerTools.Models;

namespace Kenbi.DockerTools.Containers;

/// <inheritdoc />
public sealed class DatabaseContainer : IDatabaseContainer
{
    private IDatabaseContainerTemplate _containerTemplate;
    private readonly DockerClient _client;

    DockerClient IContainer.Client => _client;

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public string ConnectionString { get; private set; }

    internal DatabaseContainer(string id, DockerClient client, IDatabaseContainerTemplate containerTemplate, string hostPort)
    {
        this.Id = id;
        _client = client;
        _containerTemplate = containerTemplate;
        this.ConnectionString = containerTemplate.GetConnectionString(hostPort);
    }

    ~DatabaseContainer()
    {
        InternalDisposeAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    /// <inheritdoc />
    public Task<ScriptExecutionResult> RunScriptAsync(string script, CancellationToken token = default)
    {
        return _containerTemplate.RunScriptAsync(_client, this.Id, script, token);
    }
    
    public async Task<string> CreateDatabaseAsync(string name, CancellationToken token)
    {
        return await _containerTemplate.CreateDatabaseAsync(_client, this.Id, name, token);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await InternalDisposeAsync();

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        InternalDisposeAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        GC.SuppressFinalize(this);
    }

    private async Task InternalDisposeAsync()
    {
        var token = CancellationToken.None;
        await _client.Containers.KillContainerAsync(this.Id, new ContainerKillParameters(), token).ConfigureAwait(false);
        await _client.Containers.RemoveContainerAsync(
                this.Id,
                new ContainerRemoveParameters
                {
                    Force = true,
                    RemoveVolumes = true
                },
                token)
            .ConfigureAwait(false);

        _client.Dispose();

        this._containerTemplate = default!;
        this.ConnectionString = null!;
    }
}