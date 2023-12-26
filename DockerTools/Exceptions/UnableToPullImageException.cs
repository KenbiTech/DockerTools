namespace Kenbi.DockerTools.Exceptions;

public class UnableToPullImageException : Exception
{
    public UnableToPullImageException(Exception inner)
        : base("An error has occurred while attempting to pull the requested image. See the inner exception for details.", inner)
    {
        
    }
}