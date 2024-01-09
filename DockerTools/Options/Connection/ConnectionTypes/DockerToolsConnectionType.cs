using Docker.DotNet;
using Kenbi.DockerTools.Exceptions;

namespace Kenbi.DockerTools.Options.Connection.ConnectionTypes;

public abstract class DockerToolsConnectionType
{
    protected DockerClient? Client;
    internal Uri? Uri { get; set; }
    
    internal async Task PingAsync(CancellationToken token)
    {
        try
        {
            await Client!.System.PingAsync(token);
        }
        catch (Exception ex)
        {
            throw new DockerUnreachableException(ex);
        }
    }
}