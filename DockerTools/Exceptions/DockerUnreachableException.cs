namespace Kenbi.DockerTools.Exceptions;

public class DockerUnreachableException : Exception
{
    public DockerUnreachableException()
    :base("Unable to reach the Docker instance. Please insure it is running and accessible.")
    {
    }
    
    public DockerUnreachableException(Exception inner)
        : base("An error has occurred while attempting to connect to the Docker instance. See the inner exception for details.", inner)
    {
    }

    public DockerUnreachableException(string message) : base(message)
    {
    }
}