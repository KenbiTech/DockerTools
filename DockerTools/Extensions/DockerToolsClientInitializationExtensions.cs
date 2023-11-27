using Kenbi.DockerTools.Exceptions;

namespace Kenbi.DockerTools.Extensions;

internal static class DockerToolsClientInitializationExtensions
{
    private const string ApiUri = "http://localhost:2375";
    private const int Timeout = 30000;

    internal static bool TryGetUriByEnvironment(out Uri uri)
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

    internal static bool TryConnectToInstance(DockerToolsClient client)
    {
        try
        {
            var task = client.Client.System.PingAsync();

            task.Wait(Timeout);
            
            if (!task.IsCompleted)
            {
                throw new DockerUnreachableException("The operation has timed out: unable to reach Docker instance within the allotted time.");
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new DockerUnreachableException(ex);
        }
    }
}