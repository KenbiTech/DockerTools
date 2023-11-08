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
    public DockerToolsClient Create()
    {
        if (this._uri != null)
        {
            return new DockerToolsClient(this._uri);
        }
        
        return DockerToolsClientInitializationExtensions.TryGetUriByEnvironment(out var uri) ? new DockerToolsClient(uri) : new DockerToolsClient();
    }
}