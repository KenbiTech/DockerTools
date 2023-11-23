namespace Kenbi.DockerTools.Exceptions;

public class DockerUnreachableException : Exception
{
    public DockerUnreachableException(Exception inner)
        : base("An error has occurred while attempting to connect to the Docker instance. See the inner exception for details.", inner)
    {
    }
}