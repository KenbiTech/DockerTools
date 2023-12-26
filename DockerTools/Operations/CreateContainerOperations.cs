using Docker.DotNet;
using Docker.DotNet.Models;
using Kenbi.DockerTools.Containers.Templates;
using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Operations;

internal static class CreateContainerOperations
{
    internal static async Task<string> CreateContainerAsync(DockerClient client, IContainerTemplate container, Guid instanceId, CancellationToken token)
    {
        var @params = ConfigureCreation(container, instanceId);

        var response = await client.Containers.CreateContainerAsync(@params, token);

        return response.ID;
    }

    private static CreateContainerParameters ConfigureCreation(IContainerTemplate container, Guid instanceId)
    {
        var labels = new Dictionary<string, string>
        {
            { "de.kenbi.dockertools.instance", instanceId.ToString() },
            { "de.kenbi.dockertools", bool.TrueString }
        };
        
        if (container is Valkyrie)
        {
            labels.Add("de.kenbi.dockertools.valkyrie", bool.TrueString);
        }
        
        return new CreateContainerParameters
        {
            Image = string.Concat(container.Image, ":", container.Tag),
            ExposedPorts = container.Ports.ConvertToExposedPorts(),
            HostConfig = new HostConfig
            {
                PortBindings = container.Ports.ConvertToPortBindings(),
                RestartPolicy = new RestartPolicy
                {
                    Name = RestartPolicyKind.No
                },
                Binds = container.Volumes.ConvertToVolumeBinds()
            },
            Env = container.EnvironmentVariables,
            Healthcheck = container.HealthCheck == null
                ? null
                : new HealthConfig
                {
                    Test = new List<string>
                    {
                        "CMD-SHELL",
                        container.HealthCheck.Command
                    },
                    Interval = container.HealthCheck.Interval,
                    Timeout = container.HealthCheck.Timeout,
                    Retries = container.HealthCheck.Retries,
                    StartPeriod = container.HealthCheck.StartPeriod * 1000000 * 1000 // Docker expects this in nanoseconds, DockerTools configures this in seconds
                },
            Labels = labels,
            Volumes = container.Volumes.ConvertToVolumes()
        };
    }
}