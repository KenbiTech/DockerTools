using Docker.DotNet;
using Docker.DotNet.Models;
using Kenbi.DockerTools.Containers.Templates;
using Kenbi.DockerTools.Models;

namespace Kenbi.DockerTools.Containers;

/// <inheritdoc />
public sealed class Container<T> : IContainer<T> where T : IContainerTemplate, new()
{
    private T _containerTemplate;
    private readonly DockerClient _client;

    DockerClient IContainer<T>.Client => _client;

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public string ConnectionString { get; private set; }

    internal Container(string id, DockerClient client, T containerTemplate, string hostPort)
    {
        this.Id = id;
        _client = client;
        _containerTemplate = containerTemplate;
        this.ConnectionString = containerTemplate.GetConnectionString(hostPort);
    }

    ~Container()
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

        await Task.Run(() => _client.Dispose(), token).ConfigureAwait(false);

        this._containerTemplate = default!;
        this.ConnectionString = null!;
    }
}