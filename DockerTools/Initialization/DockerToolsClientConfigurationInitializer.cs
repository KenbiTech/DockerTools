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
        return this._uri == null ? new DockerToolsClient() : new DockerToolsClient(this._uri);
    }
}