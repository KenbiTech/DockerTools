namespace Kenbi.DockerTools.Exceptions;

/// <inheritdoc />
public class ContainerWasNotStoppedException : Exception
{
    /// <inheritdoc />
    public ContainerWasNotStoppedException(string message) : base(message)
    {
        
    }
}