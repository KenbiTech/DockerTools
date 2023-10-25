namespace Kenbi.DockerTools.Reports;

/// <summary>
/// Lists information about a container creation.
/// </summary>
public class CreationReport
{
    /// <summary>
    /// Name of the container.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// ID of the container. Assigned by Docker upon creation. Null if an error occurs on creation.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Inner exception returned by the creation process. Null if creation is successful.
    /// </summary>
    public Exception Exception { get; }
    
    /// <summary>
    /// Status of the operation.
    /// </summary>
    public OperationStatus Status { get; }

    internal CreationReport(string name, string id)
    {
        this.Name = name;

        this.Id = id;

        this.Status = OperationStatus.Success;
    }

    internal CreationReport(string name, Exception ex)
    {
        this.Name = name;

        this.Exception = ex;

        this.Status = OperationStatus.Error;
    }
}