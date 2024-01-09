using System.Text.RegularExpressions;
using Docker.DotNet.Models;
using Newtonsoft.Json;

namespace Kenbi.DockerTools.Utils;

internal static class ConversionUtils
{
    private static readonly Regex ErrorMessageRegex = new("\"[a-zA-Z0-9]{64}\"");

    internal static Dictionary<string, EmptyStruct> ConvertToExposedPorts(this IEnumerable<PortConfiguration> ports)
    {
        return ports
            .Select(port => port.Container.AddTcpToPort())
            .ToDictionary(host => host, _ => new EmptyStruct());
    }

    internal static Dictionary<string, IList<PortBinding>> ConvertToPortBindings(this IEnumerable<PortConfiguration> ports)
    {
        var result = new Dictionary<string, IList<PortBinding>>();

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

        foreach (var portBinding in portBindings)
        {
            var containerPort = portBinding.Key;

            var hostPort = portBinding.Value.FirstOrDefault()?.HostPort ?? "0";

            result.Add(new PortConfiguration
            {
                Container = containerPort,
                Host = hostPort
            });
        }

        return result;
    }

    internal static bool TryParseExistingIdInResponse(string errorMessage, out string result)
    {
        result = string.Empty;

        var substring = errorMessage
            .Substring(errorMessage.IndexOf('{'), errorMessage.Length - errorMessage.IndexOf('{'));

        ErrorMessage conversion;

        try
        {
            conversion = JsonConvert.DeserializeObject<ErrorMessage>(substring);
        }
        catch
        {
            return false;
        }

        if (conversion == null)
        {
            return false;
        }

        var match = ErrorMessageRegex.Match(conversion.Message);

        if (string.IsNullOrWhiteSpace(match.Value))
        {
            return false;
        }

        result = match.Value.Replace("\"", string.Empty);
        return true;
    }

    internal static string StripTcpFromPort(this string port)
    {
        if ("/tcp".Contains(port))
        {
            return port.Substring(0, port.IndexOf("/", StringComparison.Ordinal));
        }

        return port;
    }

    private static string AddTcpToPort(this string port)
    {
        if (!port.Contains("/tcp"))
        {
            port = string.Concat(port, "/tcp");
        }

        return port;
    }

    internal class ErrorMessage
    {
        public string Message { get; set; }
    }
}