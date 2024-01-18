using Docker.DotNet;

namespace Kenbi.DockerTools.Options.Connection.ConnectionTypes;

internal sealed class AutoDetectConnection : DockerToolsConnectionType
{
    internal async Task<DockerClient> CreateClientAsync(CancellationToken token)
    {
        var (result, uri) = await TryAccessingDockerViaRemoteApiAsync(token).ConfigureAwait(false);

        if (result)
        {
            base.Uri = new Uri(uri!);
            return new DockerClientConfiguration(base.Uri).CreateClient();
        }

        Client = new DockerClientConfiguration().CreateClient();

        await PingAsync(token).ConfigureAwait(false);

        return Client;
    }

    private static async Task<(bool, string?)> TryAccessingDockerViaRemoteApiAsync(CancellationToken token)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:2375", UriKind.Absolute),
            Timeout = new TimeSpan(0, 0, 10)
        };

        try
        {
            var response = await client.GetAsync($"_ping", token).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return (true, client.BaseAddress!.ToString());
            }
        }
        catch
        {
            // Don't need to do anything here, just handle
            // any possible exceptions gracefully.
        }
        finally
        {
            client.CancelPendingRequests();
            client.Dispose();
        }

        return (false, null);
    }
}