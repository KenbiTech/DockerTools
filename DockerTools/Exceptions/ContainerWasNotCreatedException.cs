namespace Kenbi.DockerTools.Exceptions;

/// <inheritdoc />
public class ContainerWasNotCreatedException : Exception
{
    /// <inheritdoc />
    public ContainerWasNotCreatedException(string message) : base(message)
    {
        
    }
}