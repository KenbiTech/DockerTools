using Docker.DotNet;
using Kenbi.DockerTools.Containers.Templates;
using Kenbi.DockerTools.Models;

namespace Kenbi.DockerTools.Containers;

/// <summary>
/// Represents an instance of a DockerTools database container tracker.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IDatabaseContainer<T> : IContainer<T> where T : class, IDatabaseContainerTemplate
{
    /// <summary>
    /// The connection string to the database instance.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Create a database on the container instance
    /// </summary>
    /// <param name="name">The name of the database</param>
    /// <param name="token">A cancellation token. Optional.</param>
    /// <returns>The connectionstring for the database</returns>
    Task<string> CreateDatabaseAsync(string name, CancellationToken token = default);
}