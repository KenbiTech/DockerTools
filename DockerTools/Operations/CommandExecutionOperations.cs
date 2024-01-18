using Docker.DotNet;
using Docker.DotNet.Models;
using Kenbi.DockerTools.Models;

namespace Kenbi.DockerTools.Operations;

internal static class CommandExecutionOperations
{
    internal static Task<ScriptExecutionResult> RunScriptAsync(IDockerClient client, string id, string command, string script, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(script))
        {
            throw new ArgumentNullException(nameof(script), "Cannot run a null or empty script");
        }
        
        var commands = command.Split(" ");
        
        commands = commands.Append(script).ToArray();
        
        return InternalExecuteCommandAsync(client, id, commands, token);
    }
    
    private static async Task<ScriptExecutionResult> InternalExecuteCommandAsync(IDockerClient client, string id, IList<string> commands, CancellationToken token)
    {
        var @params = new ContainerExecCreateParameters
        {
            Cmd = commands,
            AttachStdout = true,
            AttachStderr = true,
            Tty = false
        };

        var exec = await client.Exec.ExecCreateContainerAsync(id, @params, token).ConfigureAwait(false);
        using var stream = await client.Exec.StartAndAttachContainerExecAsync(exec.ID, false, token).ConfigureAwait(false);

        var (_, stderr) = await stream.ReadOutputToEndAsync(token).ConfigureAwait(false);

        return new ScriptExecutionResult(stderr);
    }
}