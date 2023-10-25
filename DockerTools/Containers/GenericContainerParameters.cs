using Kenbi.DockerTools.Containers.Interfaces;
using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Containers;

/// <inheritdoc />
public class GenericContainerParameters : IContainerParameters
{
    internal string Image;
    internal IEnumerable<PortConfiguration> Ports;
    internal IEnumerable<string> EnvironmentVariables;
    internal HealthCheckConfig HealthCheckConfig;

    /// <summary>
    /// Defines the image to use to generate the container.
    /// </summary>
    /// <param name="image">Image name, in the format of "image:tag".</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If the parameter is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If the parameter is incorrectly formatted.</exception>
    public GenericContainerParameters WithImage(string image)
    {
        if (string.IsNullOrWhiteSpace(image))
        {
            throw new ArgumentNullException(nameof(image), "Image cannot be empty.");
        }

        var split = image.Split(':');

        if (split.Length != 2)
        {
            throw new ArgumentOutOfRangeException(nameof(image), "Image must be in the format of \"image:tag\".");
        }

        this.Image = image;

        return this;
    }

    /// <summary>
    /// Defines what ports to expose.
    /// </summary>
    /// <param name="ports">List of ports to expose.</param>
    /// <returns></returns>
    public GenericContainerParameters WithPorts(IEnumerable<PortConfiguration> ports)
    {
        this.Ports = ports;

        return this;
    }

    /// <summary>
    /// Configures environment variables.
    /// </summary>
    /// <param name="variables">Collection of variables in the format of "key=value".</param>
    /// <returns></returns>
    public GenericContainerParameters WithEnvironmentVariables(IEnumerable<string> variables)
    {
        this.EnvironmentVariables = variables;

        return this;
    }

    /// <summary>
    /// Sets up the container healthcheck. Check the image documentation
    /// to see what parameters to supply here. If the image has no healthcheck configured,
    /// leave blank.
    /// </summary>
    /// <param name="config">The healthcheck configuration.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If the configuration is null.</exception>
    public GenericContainerParameters WithHealthCheck(HealthCheckConfig config)
    {
        this.HealthCheckConfig = config ?? throw new ArgumentNullException(nameof(config), "Healthcheck configuration cannot be null.");

        return this;
    }
}