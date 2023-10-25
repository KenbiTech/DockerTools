#pragma warning disable CS1591
namespace Kenbi.DockerTools.Utils;

public class PortConfiguration
{
    /// <summary>
    /// Optional. If not set, will be randomly assigned.
    /// </summary>
    public string Host { get; set; }

    public string Container { get; set; }
}