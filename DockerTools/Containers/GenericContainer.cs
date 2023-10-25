using Kenbi.DockerTools.Containers.Interfaces;
using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Containers;

/// <inheritdoc />
public class GenericContainer : IContainer
{
    /// <inheritdoc />
    public string Image { get; private set; }

    /// <inheritdoc />
    public string Tag { get; private set; }

    /// <inheritdoc />
    public IEnumerable<PortConfiguration> Ports { get; private set; }

    /// <inheritdoc />
    public HealthCheckConfig HealthCheck { get; private set; }

    private IList<string> EnvironmentVariables { get; set; }

    /// <inheritdoc />
    public void UpsertParameters(IContainerParameters parameters)
    {
        var mappedParameters = (GenericContainerParameters)parameters;

        if (string.IsNullOrWhiteSpace(mappedParameters.Image))
        {
            throw new ArgumentNullException(nameof(mappedParameters.Image), "Image name and tag must be supplied for a generic container.");
        }

        if (mappedParameters.Ports == null || !mappedParameters.Ports.Any())
        {
            throw new ArgumentException("At least one port must be mapped.", nameof(mappedParameters.Ports));
        }

        var split = mappedParameters.Image.Split(':');
        this.Image = split[0];
        this.Tag = split[1];

        this.Ports = mappedParameters.Ports;
        this.HealthCheck = mappedParameters.HealthCheckConfig;
        this.EnvironmentVariables = mappedParameters.EnvironmentVariables?.ToList() ?? new List<string>();
    }

    /// <inheritdoc />
    public IList<string> GetEnvironmentVariables()
    {
        return this.EnvironmentVariables;
    }
}