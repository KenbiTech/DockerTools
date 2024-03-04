using System.Diagnostics;
using Docker.DotNet;
using Docker.DotNet.Models;
using Kenbi.DockerTools.Containers.Templates;
using Kenbi.DockerTools.Models;
using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Operations;

internal static class StartContainerOperations
{
    private const string Healthy = "healthy";
    private const int DefaultAttemptTimeout = 2000;
    private static readonly TimeSpan DefaultOverallTimeout = new(0, 1, 0);
    private const int ConsecutiveHealthyResults = 5;

    internal static Task<bool> TryStartContainerAsync(DockerClient client, string id, CancellationToken token)
    {
        return client.Containers.StartContainerAsync(id, new ContainerStartParameters(), token);
    }

    internal static async Task<bool> IsContainerHealthy(DockerClient client, string id, IContainerTemplate container, CancellationToken token)
    {
        var startPeriod = (int)(container.HealthCheck?.StartPeriod ?? 0) * 1000;
        var attemptTimeout = container.HealthCheck?.Timeout.Seconds * 1000 ?? DefaultAttemptTimeout;
        var overallTimeout = ((container.HealthCheck?.Timeout + container.HealthCheck?.Interval) * container.HealthCheck?.Retries) ?? DefaultOverallTimeout;
        var healthCounter = 0;
        
        Thread.Sleep(startPeriod);

        var stopWatch = new Stopwatch();
        stopWatch.Start();
        while (stopWatch.Elapsed < overallTimeout)
        {
            var response = await client.Containers.InspectContainerAsync(id, token).ConfigureAwait(false);

            switch (response.State.Health)
            {
                case null when response.State.Running: // container with no health check
                    return true;
                case { Status: Healthy, FailingStreak: 0 }:
                    healthCounter += 1;
                    break;
                default:
                    healthCounter -= 1;
                    break;
            }

            switch (healthCounter)
            {
                case ConsecutiveHealthyResults:
                    return true;
                case ConsecutiveHealthyResults * -1:
                    return false;
                default:
                    Thread.Sleep(attemptTimeout);
                    break;
            }
        }

        return false;
    }

    internal static async Task<IEnumerable<PortConfiguration>> GetRunningPortsAsync(DockerClient client, string id, CancellationToken token)
    {
        var response = await client.Containers.InspectContainerAsync(id, token).ConfigureAwait(false);

        var portConfiguration = response.NetworkSettings.Ports.ConvertToPortConfiguration();

        return portConfiguration;
    }
}