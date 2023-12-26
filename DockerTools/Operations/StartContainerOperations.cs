using Docker.DotNet;
using Docker.DotNet.Models;
using Kenbi.DockerTools.Containers.Templates;
using Kenbi.DockerTools.Models;
using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Operations;

internal static class StartContainerOperations
{
    private const int Attempts = 10;
    private const int AttemptTimeout = 2000;
    private const string Running = "Running";
    private const string Healthy = "healthy";

    internal static Task<bool> TryStartContainerAsync(DockerClient client, string id, CancellationToken token)
    {
        return client.Containers.StartContainerAsync(id, new ContainerStartParameters(), token);
    }

    internal static async Task<bool> IsContainerHealthy(DockerClient client, string id, IContainerTemplate container, CancellationToken token)
    {
        var startPeriod = (int)(container.HealthCheck?.StartPeriod ?? 0) * 1000;
        Thread.Sleep(startPeriod);

        for (var i = 0; i < Attempts; i++)
        {
            var response = await client.Containers.InspectContainerAsync(id, token);

            if ((response.State.Health == null && response.State.Status == Running) ||
                response.State.Health?.Status == Healthy)
            {
                return true;
            }

            Thread.Sleep(AttemptTimeout);
        }

        return false;
    }

    internal static async Task<IEnumerable<PortConfiguration>> GetRunningPortsAsync(DockerClient client, string id, CancellationToken token)
    {
        var response = await client.Containers.InspectContainerAsync(id, token);

        var portConfiguration = response.NetworkSettings.Ports.ConvertToPortConfiguration();

        return portConfiguration;
    }
}