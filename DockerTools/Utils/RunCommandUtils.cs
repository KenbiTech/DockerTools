using Docker.DotNet.Models;

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

    internal static async Task InternalExecuteCommandAsync(DockerToolsClient client, string id, IList<string> commands, CancellationToken token = default)
    {
        await InternalExecuteCommandAsync(client, id, commands, Array.Empty<string>(), token);
    }

    internal static async Task InternalExecuteCommandAsync(DockerToolsClient client, string id, IList<string> commands, IList<string> environmentVariables, CancellationToken token = default)
    {
        var @params = new ContainerExecCreateParameters
        {
            Cmd = commands,
            Env = environmentVariables,
            AttachStdout = true,
            AttachStderr = true,
            Tty = false
        };

        var exec = await client.Client.Exec.ExecCreateContainerAsync(id, @params, token);
        using var stream = await client.Client.Exec.StartAndAttachContainerExecAsync(exec.ID, false, token);

        await stream.CopyOutputToAsync(null, Console.OpenStandardOutput(), Console.OpenStandardError(), token);
    }
}