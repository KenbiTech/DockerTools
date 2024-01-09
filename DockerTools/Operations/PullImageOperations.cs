using Docker.DotNet;
using Docker.DotNet.Models;
using Kenbi.DockerTools.Exceptions;

namespace Kenbi.DockerTools.Operations;

internal static class PullImageOperations
{
    internal static async Task PullImageAsync(IDockerClient client, string image, string tag, CancellationToken token)
    {
        if (await DoesImageExistLocallyAsync(client, image, tag, token))
        {
            return;
        }

        try
        {
            await InternalTryPullImageAsync(client, image, tag, token);
        }
        catch (Exception ex)
        {
            throw new UnableToPullImageException(ex);
        }
    }

    private static async Task<bool> DoesImageExistLocallyAsync(IDockerClient client, string image, string tag, CancellationToken token)
    {
        var results = await client.Images.ListImagesAsync(
            new ImagesListParameters
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {
                        "reference", new Dictionary<string, bool>
                        {
                            { $"{image}:{tag}", true }
                        }
                    }
                }
            },
            token);

        return results.Any();
    }

    private static Task InternalTryPullImageAsync(IDockerClient client, string image, string tag, CancellationToken token)
    {
        return client.Images.CreateImageAsync(
            new ImagesCreateParameters
            {
                FromImage = image,
                Tag = tag
            },
            new AuthConfig(),
            new Dictionary<string, string>(),
            new Progress<JSONMessage>(),
            token);
    }
}