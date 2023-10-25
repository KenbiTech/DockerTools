namespace Kenbi.DockerTools.Containers.Interfaces;

/// <summary>
/// An intermediate on the container creation setup.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IContainerSetup<T> where T : class, IContainer
{
    /// <summary>
    /// Converts the setup to a container entry on the main client.
    /// </summary>
    void Build();
}

/// <summary>
/// An intermediate on the container creation setup.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TParameters"></typeparam>
public interface IContainerSetup<T,TParameters> where T : class, IContainer where TParameters : class, IContainerParameters
{
    /// <summary>
    /// Allows for the setting of parameters.
    /// </summary>
    /// <param name="parameters">List of parameters to set.</param>
    /// <returns>The supplied instance of <see cref="IContainerSetup{T}"/>.</returns>
    IContainerSetup<T,TParameters> WithParameters(Func<TParameters> parameters);

    /// <summary>
    /// Converts the setup to a container entry on the main client.
    /// </summary>
    void Build();
}