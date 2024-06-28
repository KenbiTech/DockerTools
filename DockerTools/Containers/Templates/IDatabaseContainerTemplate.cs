using Docker.DotNet;
using Kenbi.DockerTools.Models;
using Kenbi.DockerTools.Options.Container;

namespace Kenbi.DockerTools.Containers.Templates;

/// <summary>
/// Reference for creating database container configurations.
/// </summary>
public interface IDatabaseContainerTemplate : IContainerTemplate
{
    internal Task<string> CreateDatabaseAsync(DockerClient client, string id, string name, CancellationToken token);
}