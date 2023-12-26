using Docker.DotNet.Models;
using Kenbi.DockerTools.Models;

namespace Kenbi.DockerTools.Utils;

internal static class ConversionUtils
{
    internal static Dictionary<string, EmptyStruct>? ConvertToExposedPorts(this IEnumerable<PortConfiguration>? ports)
    {
        return ports?
            .Select(port => port.Container.AddTcpToPort())
            .ToDictionary(host => host, _ => new EmptyStruct());
    }

    internal static Dictionary<string, IList<PortBinding>> ConvertToPortBindings(this IEnumerable<PortConfiguration>? ports)
    {
        var result = new Dictionary<string, IList<PortBinding>>();

        if (ports == null)
        {
            return result;
        }

        foreach (var port in ports)
        {
            var containerPort = port.Container.AddTcpToPort();

            result.Add(containerPort, new List<PortBinding>
            {
                new()
                {
                    HostPort = port.Host
                }
            });
        }

        return result;
    }

    internal static IEnumerable<PortConfiguration> ConvertToPortConfiguration(this IDictionary<string, IList<PortBinding>> portBindings)
    {
        var result = new List<PortConfiguration>();

        foreach (var (containerPort, list) in portBindings)
        {
            var hostPort = list.FirstOrDefault()?.HostPort ?? "0";

            result.Add(new PortConfiguration
            {
                Container = containerPort,
                Host = hostPort
            });
        }

        return result;
    }

    internal static Dictionary<string, EmptyStruct>? ConvertToVolumes(this Dictionary<string, string>? volumes)
    {
        return volumes?
            .Select(volume => volume.Key)
            .ToDictionary(volume => volume, _ => new EmptyStruct());
    }

    internal static IList<string>? ConvertToVolumeBinds(this Dictionary<string, string>? volumes)
    {
        return volumes?
            .Select(x => $"{x.Key}:{x.Value}")
            .ToList();
    }

    private static string AddTcpToPort(this string port)
    {
        if (!port.Contains("/tcp"))
        {
            port = string.Concat(port, "/tcp");
        }

        return port;
    }
}