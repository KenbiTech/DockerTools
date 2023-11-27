using Kenbi.DockerTools.Extensions;

namespace Kenbi.DockerTools.Initialization;

/// <summary>
/// 
/// </summary>
public class DockerToolsClientConfigurationInitializer
{
    private readonly Uri _uri;

    internal DockerToolsClientConfigurationInitializer(Uri uri)
    {
        this._uri = uri;
    }

    internal DockerToolsClientConfigurationInitializer()
    {
    }

    /// <summary>
    /// Creates the requested instance.
    /// </summary>
    /// <returns>A new DockerToolsClient instance.</returns>
    /// <exception cref="DockerTools.Exceptions.DockerUnreachableException">Thrown if connection to Docker instance cannot be made (e.g., Docker is not running on host).</exception>
    public DockerToolsClient Create()
    {
        DockerToolsClient client = null;

        if (this._uri != null)
        {
            client = new DockerToolsClient(this._uri);
        }
        else if (DockerToolsClientInitializationExtensions.TryGetUriByEnvironment(out var uri))
        {
            client = new DockerToolsClient(uri);
        }
        else
        {
            client = new DockerToolsClient();
        }

        DockerToolsClientInitializationExtensions.TryConnectToInstance(client);

        return client;
    }
}