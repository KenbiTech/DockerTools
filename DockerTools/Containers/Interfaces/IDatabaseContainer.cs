namespace Kenbi.DockerTools.Containers.Interfaces;

/// <inheritdoc />
public interface IDatabaseContainer : IContainer
{
    /// <summary>
    /// Generate the connection string to the container's database.
    /// </summary>
    /// <param name="hostPort"></param>
    /// <returns></returns>
    string CreateConnectionString(string hostPort);
}