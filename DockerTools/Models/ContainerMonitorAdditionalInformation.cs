namespace Kenbi.DockerTools.Models;

/// <summary>
/// Additional information pertaining to a container.
/// </summary>
public class ContainerMonitorAdditionalInformation
{
    /// <summary>
    /// Connection string. Applicable to database containers.
    /// </summary>
    public string ConnectionString { get; internal set; }
}