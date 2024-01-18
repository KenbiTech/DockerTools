using Docker.DotNet;
using Docker.DotNet.Models;
using Kenbi.DockerTools.Containers.Templates;
using Kenbi.DockerTools.Models;

namespace Kenbi.DockerTools.Containers;

public sealed class Container<T> : IContainer<T> where T : IContainerTemplate, new()
{
    private T _containerTemplate;
    private readonly DockerClient _client;

    DockerClient IContainer<T>.Client => _client;
    public string Id { get; }
    public string ConnectionString { get; private set; }

    internal Container(string id, DockerClient client, T containerTemplate, string hostPort)
    {
        this.Id = id;
        _client = client;
        _containerTemplate = containerTemplate;
        this.ConnectionString = containerTemplate.GetConnectionString(hostPort);
    }

    public Task<ScriptExecutionResult> RunScriptAsync(string script, CancellationToken token = default)
    {
        return _containerTemplate.RunScriptAsync(_client, this.Id, script, token);
    }

    public async ValueTask DisposeAsync()
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
            token).ConfigureAwait(false);
        await Task.Run(() => _client.Dispose(), token).ConfigureAwait(false);
        this._containerTemplate = new T();
        this.ConnectionString = null!;
    }
}