using Kenbi.DockerTools.Containers.Interfaces;
using Kenbi.DockerTools.Reports;
using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Containers;

/// <inheritdoc />
public sealed class SqlServerContainer : IDatabaseContainer
{
    /// <inheritdoc />
    public string Image => "mcr.microsoft.com/mssql/server";

    /// <inheritdoc />
    public string Tag => "2022-latest";

    /// <inheritdoc />
    public IEnumerable<PortConfiguration> Ports => new List<PortConfiguration>
    {
        new()
        {
            Container = "1433"
        }
    };

    /// <inheritdoc />
    public HealthCheckConfig HealthCheck => new HealthCheckConfig
    {
        Test = new List<string>
        {
            "CMD-SHELL",
            "/opt/mssql-tools/bin/sqlcmd -U $DB_USER -P $SA_PASSWORD -Q 'select 1' -b -o /dev/null"
        }
    };
    
    private string Username { get; set; } = "sa";
    private string Password { get; set; } = "ABc123$%";
    private string Database { get; set; } = "DockerTools";
    public string ScriptExecutionBaseCommand => $"/opt/mssql-tools/bin/sqlcmd -U {this.Username} -P {this.Password} -d {this.Database} -Q";
    private IList<string> EnvironmentVariables => new List<string>
    {
        "ACCEPT_EULA=true",
        $"MSSQL_SA_PASSWORD={this.Password}"
        
    };

    /// <inheritdoc />
    public void UpsertParameters(IContainerParameters parameters)
    {
        
    }

    /// <inheritdoc />
    public IList<string> GetEnvironmentVariables()
    {
        return this.EnvironmentVariables;
    }
    
    /// <inheritdoc />
    public Task PerformPostStartOperationsAsync(DockerToolsClient client, string id, CancellationToken token = default)
    {
        var command = $"/opt/mssql-tools/bin/sqlcmd -U {this.Username} -P {this.Password} -Q";
        var script = $"CREATE DATABASE {this.Database};";

        var commands = RunCommandUtils.SetupCommand(command, script);

        return RunCommandUtils.InternalExecuteCommandAsync(client, id, commands, token);
    }

    /// <inheritdoc />
    public Task<CommandExecutionReport> ExecuteCommandAsync(DockerToolsClient client, string id, string command, CancellationToken token = default)
    {
        var commands = RunCommandUtils.SetupCommand(this.ScriptExecutionBaseCommand, command);

        return RunCommandUtils.InternalExecuteCommandAsync(client, id, commands, token);
    }

    /// <inheritdoc />
    public string CreateConnectionString(string hostPort) => $"Server=localhost,{hostPort};Database={this.Database};User Id={this.Username};Password={this.Password};";
}