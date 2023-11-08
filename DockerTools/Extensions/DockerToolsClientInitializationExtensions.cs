using System.Runtime.InteropServices;

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
}