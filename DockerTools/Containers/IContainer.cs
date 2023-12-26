using Docker.DotNet;
using Kenbi.DockerTools.Containers.Templates;
using Kenbi.DockerTools.Models;

namespace Kenbi.DockerTools.Containers;

public interface IContainer<T> : IAsyncDisposable where T : IContainerTemplate
{
    internal DockerClient Client { get; }
    
    public string Id { get; }
    
    /// <summary>
    /// The connection string to the database instance.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Run a script on the container's database.
    /// </summary>
    /// <param name="script">The script to run.</param>
    /// <param name="token">A cancellation token. Optional.</param>
    /// <returns>True if script ran successfully; false otherwise.</returns>
    Task<ScriptExecutionResult> RunScriptAsync(string script, CancellationToken token = default);
}