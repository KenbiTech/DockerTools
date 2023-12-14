namespace Kenbi.DockerTools.Containers.Interfaces;

/// <inheritdoc />
public interface IDatabaseContainer : IContainer
{
    /// <summary>
    /// The base command to allow for operation on the underlying database.
    /// </summary>
    internal string ScriptExecutionBaseCommand { get; }
    
    /// <summary>
    /// Generate the connection string to the container's database.
    /// </summary>
    /// <param name="hostPort">The port assigned by Docker.</param>
    /// <returns>The generated connection string.</returns>
    internal string CreateConnectionString(string hostPort);
}