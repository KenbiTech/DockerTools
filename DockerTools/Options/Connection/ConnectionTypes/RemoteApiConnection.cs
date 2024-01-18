using Docker.DotNet;

namespace Kenbi.DockerTools.Options.Connection.ConnectionTypes;

/// <summary>
/// Allows for the configuration of a connection to a remote Docker HTTP API.
/// </summary>
public sealed class RemoteApiConnection : DockerToolsConnectionType
{
    public RemoteApiConnection(Uri uri)
    {
        base.Uri = uri;
    }

    internal async Task<DockerClient> CreateClientAsync(CancellationToken token)
    {
        Client = new DockerClientConfiguration(Uri).CreateClient();

        await PingAsync(token).ConfigureAwait(false);

        return Client;
    }
}