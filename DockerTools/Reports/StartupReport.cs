namespace Kenbi.DockerTools.Reports;

/// <summary>
/// Lists information about a container startup.
/// </summary>
public class StartupReport
{
    /// <summary>
    /// Name of the container.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// ID of the container. Assigned by Docker upon creation.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Inner exception returned by the startup process. Null if startup is successful.
    /// </summary>
    public Exception Exception { get; }
    
    /// <summary>
    /// Status of the operation.
    /// </summary>
    public OperationStatus Status { get; }

    internal StartupReport(string name, string id, OperationStatus status)
    {
        this.Name = name;
        this.Id = id;
        this.Status = status;
    }
    
    internal StartupReport(string name, string id, Exception ex)
    {
        this.Name = name;
        this.Id = id;
        this.Exception = ex;

        this.Status = OperationStatus.Error;
    }
}