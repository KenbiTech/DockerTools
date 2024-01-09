namespace Kenbi.DockerTools.Exceptions;

public class UnableToSetupContainerException : Exception
{
    public UnableToSetupContainerException(string message)
        : base("An error has occurred while configuring the database: " + message)
    {
        
    }
}