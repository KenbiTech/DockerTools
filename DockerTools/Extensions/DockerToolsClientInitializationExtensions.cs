using Kenbi.DockerTools.Exceptions;

namespace Kenbi.DockerTools.Extensions;

internal static class DockerToolsClientInitializationExtensions
{
    private const string ApiUri = "http://localhost:2375";

    internal static bool TryGetUriByEnvironment(out Uri? uri)
    {
        HttpClient client = null;

        try
        {
            client = new HttpClient();

            var response = client.GetAsync($"{ApiUri}/version").GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
            {
                uri = new Uri(ApiUri);
                return true;
            }
        }
        catch
        {
            uri = null;
            return false;
        }
        finally
        {
            client?.Dispose();
        }
        
        uri = null;
        return false;
    }

    /// <summary>
    /// Attempts to connect to the Docker instance uri supplied.
    /// </summary>
    /// <param name="uri">Path to validate.</param>
    /// <returns></returns>
    /// <exception cref="DockerUnreachableException"></exception>
    internal static bool TryConnectToInstance(DockerToolsClient client)
    {
        try
        {
            client.Client.System.GetVersionAsync().GetAwaiter().GetResult();

            return true;
        }
        catch (Exception ex)
        {
            throw new DockerUnreachableException(ex);
        }
    }
}