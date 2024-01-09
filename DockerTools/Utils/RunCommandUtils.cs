using Docker.DotNet.Models;
using Kenbi.DockerTools.Reports;

namespace Kenbi.DockerTools.Utils;

internal static class RunCommandUtils
{
    internal static string[] SetupCommand(string command, string? script = null)
    {
        var elements = command.Split(" ");
        var commands = new List<string>(elements.Length + 1);
        commands.AddRange(elements);

        if (script != null)
        {
            commands.Add(script);
        }

        return commands.ToArray();
    }

    internal static Task<CommandExecutionReport> InternalExecuteCommandAsync(DockerToolsClient client, string id, IList<string> commands, CancellationToken token = default)
    {
        return InternalExecuteCommandAsync(client, id, commands, Array.Empty<string>(), token);
    }

    internal static async Task<CommandExecutionReport> InternalExecuteCommandAsync(DockerToolsClient client, string id, IList<string> commands, IList<string> environmentVariables, CancellationToken token = default)
    {
        var @params = new ContainerExecCreateParameters
        {
            Cmd = commands,
            Env = environmentVariables,
            AttachStdout = true,
            AttachStderr = true
        };

        var exec = await client.Client.Exec.ExecCreateContainerAsync(id, @params, token);
        using (var stream = await client.Client.Exec.StartAndAttachContainerExecAsync(exec.ID, false, token))
        {
            var (stdout, stderr) = await stream.ReadOutputToEndAsync(token);

            return new CommandExecutionReport(stdout, stderr);
        }
    }
}