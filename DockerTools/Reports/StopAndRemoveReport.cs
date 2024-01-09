namespace Kenbi.DockerTools.Reports;

/// <summary>
/// Lists information about a container removal.
/// </summary>
public sealed class StopAndRemoveReport
{
    /// <summary>
    /// Name of the container.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// ID of the container. Assigned by Docker upon creation.
    /// </summary>
    public string? Id { get; }

    /// <summary>
    /// Inner exception returned by the stop/removal process. Null if successful.
    /// </summary>
    public Exception? Exception { get; }
    
    /// <summary>
    /// Status of the operation.
    /// </summary>
    public OperationStatus Status { get; }

    internal StopAndRemoveReport(string name, string id, OperationStatus status)
    {
        this.Name = name;
        this.Id = id;
        this.Status = status;
    }
    
    internal StopAndRemoveReport(string name, string id, Exception ex)
    {
        this.Name = name;
        this.Id = id;
        this.Exception = ex;
        
        this.Status = OperationStatus.Error;
    }
    
    internal StopAndRemoveReport(string name, Exception ex)
    {
        this.Name = name;
        this.Exception = ex;
        
        this.Status = OperationStatus.Error;
    }
}