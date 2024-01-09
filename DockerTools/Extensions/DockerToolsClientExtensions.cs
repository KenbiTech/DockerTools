using Docker.DotNet;
using Docker.DotNet.Models;
using Kenbi.DockerTools.Exceptions;
using Kenbi.DockerTools.Models.Interfaces;
using Kenbi.DockerTools.Reports;
using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Extensions;

public static class DockerToolsClientExtensions
{
    private const string HealthCheckHealthy = "healthy";
    private const int HealthCheckAttempts = 10;

    /// <summary>
    /// Creates all configured containers.
    /// </summary>
    /// <param name="dc">The DockerToolsClient instance.</param>
    /// <param name="token">A cancellation token. Optional.</param>
    /// <returns>A collection confirming the operation status of every configured container.</returns>
    public static async Task<List<CreationReport>> CreateAsync(this DockerToolsClient dc, CancellationToken token = default)
    {
        var reports = new List<CreationReport>();

        foreach (var container in dc.Containers)
        {
            CreationReport report;

            try
            {
                await PullImageAsync(dc, container.Configuration.Image, token);

                var response = await dc.Client.Containers.CreateContainerAsync(container.Configuration, token);

                container.AddId(response.ID, true);

                report = new CreationReport(container.Configuration.Name, response.ID);
            }
            catch (DockerApiException ex)
            {
                var result = ConversionUtils.TryParseExistingIdInResponse(ex.Message, out var id);

                if (result)
                {
                    container.AddId(id, false);

                    report = new CreationReport(container.Configuration.Name, id);
                }
                else
                {
                    report = new CreationReport(container.Configuration.Name, ex);
                }
            }
            catch (Exception ex)
            {
                report = new CreationReport(container.Configuration.Name, ex);
            }

            reports.Add(report);
        }

        return reports;
    }

    /// <summary>
    /// Starts all configured containers.
    /// </summary>
    /// <param name="dc">The DockerToolsClient instance.</param>
    /// <param name="token">A cancellation token. Optional.</param>
    /// <returns>A collection confirming the operation status of every configured container.</returns>
    public static async Task<List<StartupReport>> StartAsync(this DockerToolsClient dc, CancellationToken token = default)
    {
        var reports = new List<StartupReport>();

        foreach (var container in dc.Containers)
        {
            StartupReport report;

            try
            {
                if (string.IsNullOrWhiteSpace(container.Id))
                {
                    report = new StartupReport(
                        container.Configuration.Name,
                        new ContainerWasNotCreatedException("Container could not be started because it was not created successfully."));
                    reports.Add(report);
                    continue;
                }

                var response = await dc.Client.Containers.StartContainerAsync(container.Id, new ContainerStartParameters(), token);

                if (response)
                {
                    var (result, ports) = await TryGetContainerStatusAsync(dc, container.Id, token);

                    if (result)
                    {
                        container.ReplacePortBindings(ports);
                        container.AddAdditionalInformation();
                        await container.PerformPostStartOperationsAsync(token);
                        report = new StartupReport(container.Configuration.Name, container.Id, OperationStatus.Success);
                    }
                    else
                    {
                        report = new StartupReport(container.Configuration.Name, container.Id, OperationStatus.Inconclusive);
                    }
                }
                else
                {
                    report = new StartupReport(container.Configuration.Name, container.Id, OperationStatus.Error);
                }
            }
            catch (Exception ex)
            {
                report = new StartupReport(container.Configuration.Name, container.Id, ex);
            }

            reports.Add(report);
        }

        return reports;
    }

    /// <summary>
    /// Stops and removes all configured containers that were created by the library.
    /// Previously existing containers are not modified.
    /// </summary>
    /// <param name="dc">The DockerToolsClient instance.</param>
    /// <param name="token">A cancellation token. Optional.</param>
    /// <returns>A collection confirming the operation status of every configured container.</returns>
    public static async Task<List<StopAndRemoveReport>> StopAndRemoveAsync(this DockerToolsClient dc, CancellationToken token = default)
    {
        var result = new List<StopAndRemoveReport>();

        foreach (var container in dc.Containers)
        {
            StopAndRemoveReport report;

            if (!container.CreatedByDockerTools)
            {
                report = new StopAndRemoveReport(
                    container.Configuration.Name,
                    new ContainerWasNotStoppedException("Container was not stopped because it is not managed by Docker Tools."));
                result.Add(report);
                continue;
            }

            report = await StopAsync(dc, container, token);

            if (report.Status != OperationStatus.Success)
            {
                result.Add(report);
                continue;
            }

            report = await RemoveAsync(dc, container, token);

            result.Add(report);
        }

        dc.RemoveAllContainers();

        return result;
    }

    private static Task PullImageAsync(DockerToolsClient dc, string image, CancellationToken token)
    {
        var split = image.Split(':');

        return dc.Client.Images.CreateImageAsync(
            new ImagesCreateParameters
            {
                FromImage = split[0],
                Tag = split[1]
            },
            new AuthConfig(),
            new Dictionary<string, string>(),
            new Progress<JSONMessage>(),
            token);
    }

    private static async Task<(bool, IEnumerable<PortConfiguration>)> TryGetContainerStatusAsync(DockerToolsClient dc, string id, CancellationToken token)
    {
        var healthy = false;
        ContainerInspectResponse? response = null;

        for (var i = 0; i < HealthCheckAttempts; i++)
        {
            response = await dc.Client.Containers.InspectContainerAsync(id, token);

            if (response.State.Health == null || response.State.Health.Status == HealthCheckHealthy)
            {
                healthy = true;
                break;
            }

            Thread.Sleep(new TimeSpan(0, 0, seconds: 2));
        }

        if (!healthy)
        {
            return (false, Array.Empty<PortConfiguration>());
        }

        var portConfiguration = response!.NetworkSettings.Ports.ConvertToPortConfiguration();

        return (true, portConfiguration);
    }

    private static async Task<StopAndRemoveReport> StopAsync(DockerToolsClient dc, IContainerMonitor container, CancellationToken token)
    {
        try
        {
            var response = await dc.Client.Containers.StopContainerAsync(
                container.Id,
                new ContainerStopParameters(),
                token);

            return !response ?
                new StopAndRemoveReport(container.Configuration.Name, container.Id, OperationStatus.Error) :
                new StopAndRemoveReport(container.Configuration.Name, container.Id, OperationStatus.Success);
        }
        catch (Exception ex)
        {
            return new StopAndRemoveReport(container.Configuration.Name, container.Id, ex);
        }
    }

    private static async Task<StopAndRemoveReport> RemoveAsync(DockerToolsClient dc, IContainerMonitor container, CancellationToken token)
    {
        try
        {
            await dc.Client.Containers.RemoveContainerAsync(
                container.Id,
                new ContainerRemoveParameters
                {
                    RemoveVolumes = true
                },
                token);
        }
        catch (Exception ex)
        {
            return new StopAndRemoveReport(container.Configuration.Name, container.Id, ex);
        }

        return new StopAndRemoveReport(container.Configuration.Name, container.Id, OperationStatus.Success);
    }
}