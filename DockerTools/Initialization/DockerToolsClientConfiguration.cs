namespace Kenbi.DockerTools.Initialization;

/// <summary>
/// 
/// </summary>
public sealed class DockerToolsClientConfiguration
{
    private const string WindowsPipe = "npipe://./pipe/docker_engine";
    private const string UnixSocket = "unix:///var/run/docker.sock";

    /// <summary>
    /// Creates an instance connected to the default Unix socket.
    /// </summary>
    /// <returns></returns>
    public DockerToolsClientConfigurationInitializer UsingUnixSocket()
    {
        return new DockerToolsClientConfigurationInitializer(new Uri(UnixSocket));
    }

    /// <summary>
    /// Creates an instance connected to the default Windows pipe.
    /// </summary>
    /// <returns></returns>
    public DockerToolsClientConfigurationInitializer UsingWindowsPipes()
    {
        return new DockerToolsClientConfigurationInitializer(new Uri(WindowsPipe));
    }

    /// <summary>
    /// Creates an instance connected to a supplied URI.
    /// </summary>
    /// <param name="uri">The URI to connect to.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">If the supplied URI matches the path of the Unix socket, Windows pipe,
    /// or is not absolute.</exception>
    public DockerToolsClientConfigurationInitializer UsingUri(Uri uri)
    {
        if (uri.OriginalString == UnixSocket)
        {
            throw new ArgumentException($"Please use the {nameof(this.UsingUnixSocket)} option.", nameof(uri));
        }
        
        if (uri.OriginalString == WindowsPipe)
        {
            throw new ArgumentException($"Please use the {nameof(this.UsingWindowsPipes)} option.", nameof(uri));
        }

        if (!uri.IsAbsoluteUri)
        {
            throw new ArgumentException("URI must be absolute.", nameof(uri));
        }
        
        return new DockerToolsClientConfigurationInitializer(uri);
    }

    /// <summary>
    /// Creates an instance using auto detection based on host OS.
    /// </summary>
    /// <returns></returns>
    public DockerToolsClientConfigurationInitializer UsingEnvironmentDetection()
    {
        return new DockerToolsClientConfigurationInitializer();
    }
}