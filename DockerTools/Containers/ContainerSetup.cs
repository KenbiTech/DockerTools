using Docker.DotNet.Models;
using Kenbi.DockerTools.Containers.Interfaces;
using Kenbi.DockerTools.Models;
using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Containers;


/// <inheritdoc />
public class ContainerSetup<T> : IContainerSetup<T> where T : class, IContainer
{
    private readonly DockerToolsClient _client;

    private IContainer Container { get; }

    internal ContainerSetup(DockerToolsClient client, IContainer container)
    {
        this._client = client;
        this.Container = container;
    }

    /// <inheritdoc />
    public void Build()
    {
        var configuration = new CreateContainerParameters
        {
            Image = string.Concat(this.Container.Image, ":", this.Container.Tag),
            ExposedPorts = this.Container.Ports.ConvertToExposedPorts(),
            HostConfig = new HostConfig
            {
                PortBindings = this.Container.Ports.ConvertToPortBindings(),
                RestartPolicy = new RestartPolicy
                {
                    Name = RestartPolicyKind.Always
                }
            },
            Env = this.Container.GetEnvironmentVariables(),
            Healthcheck = new HealthConfig
            {
                Test = this.Container.HealthCheck.Test,
                Interval = this.Container.HealthCheck.Interval,
                Timeout = this.Container.HealthCheck.Timeout,
                Retries = this.Container.HealthCheck.Retries,
                StartPeriod = this.Container.HealthCheck.StartPeriod
            }
        };

        this._client.TryAddConfiguration(
            new ContainerMonitor(this.Container.Image, configuration, this.Container, this._client),
            out _);
    }
}

/// <inheritdoc />
public class ContainerSetup<T, TParameters> : IContainerSetup<T, TParameters> where T : class, IContainer where TParameters : class, IContainerParameters
{
    private readonly DockerToolsClient _client;

    private IContainer Container { get; }

    internal ContainerSetup(DockerToolsClient client, IContainer container)
    {
        this._client = client;
        this.Container = container;
    }

    /// <inheritdoc />
    public IContainerSetup<T, TParameters> WithParameters(Func<TParameters> parameters)
    {
        if (parameters == null)
        {
            throw new ArgumentException("Field cannot be null.", nameof(parameters));
        }

        this.Container.UpsertParameters(parameters.Invoke());

        return this;
    }

    /// <inheritdoc />
    public void Build()
    {
        var configuration = new CreateContainerParameters
        {
            Image = string.Concat(this.Container.Image, ":", this.Container.Tag),
            ExposedPorts = this.Container.Ports.ConvertToExposedPorts(),
            HostConfig = new HostConfig
            {
                PortBindings = this.Container.Ports.ConvertToPortBindings(),
                RestartPolicy = new RestartPolicy
                {
                    Name = RestartPolicyKind.Always
                }
            },
            Env = this.Container.GetEnvironmentVariables(),
            Healthcheck = new HealthConfig
            {
                Test = this.Container.HealthCheck.Test,
                Interval = this.Container.HealthCheck.Interval,
                Timeout = this.Container.HealthCheck.Timeout,
                Retries = this.Container.HealthCheck.Retries,
                StartPeriod = this.Container.HealthCheck.StartPeriod
            }
        };

        this._client.TryAddConfiguration(
            new ContainerMonitor(this.Container.Image, configuration, this.Container, this._client),
            out _);
    }
}
