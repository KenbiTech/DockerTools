using Kenbi.DockerTools.Containers.Interfaces;
using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Containers;

/// <inheritdoc />
public sealed class PostgisContainer : IDatabaseContainer
{
    /// <inheritdoc />
    public string Image => "postgis/postgis";

    /// <inheritdoc />
    public string Tag { get; private set; } = "14-3.3";

    /// <inheritdoc />
    public IEnumerable<PortConfiguration> Ports => new List<PortConfiguration>
    {
        new()
        {
            Container = "5432"
        }
    };

    /// <inheritdoc />
    public HealthCheckConfig HealthCheck => new HealthCheckConfig
    {
        Test = new List<string>
        {
            "CMD-SHELL",
            "pg_isready"
        },
        Interval = new TimeSpan(0, 0, 10),
        Timeout = new TimeSpan(0, 0, 5),
        Retries = 5
    };

    private string Username { get; set; } = "postgres";
    private string Password { get; set; } = "postgres";
    private string Database { get; set; } = "postgres";
    public string ScriptExecutionBaseCommand => $"psql -U {this.Username} -d {this.Database} -c";

    /// <inheritdoc />
    public void UpsertParameters(IContainerParameters parameters)
    {
        var mappedParameters = (PostgisContainerParameters)parameters;

        if (!string.IsNullOrWhiteSpace(mappedParameters.Username))
        {
            this.Username = mappedParameters.Username;
        }

        if (!string.IsNullOrWhiteSpace(mappedParameters.Password))
        {
            this.Password = mappedParameters.Password;
        }

        if (!string.IsNullOrWhiteSpace(mappedParameters.Database))
        {
            this.Database = mappedParameters.Database;
        }

        if (!string.IsNullOrWhiteSpace(mappedParameters.Version))
        {
            this.Tag = mappedParameters.Version;
        }
    }

    /// <inheritdoc />
    public IList<string> GetEnvironmentVariables()
    {
        return new List<string>
        {
            $"POSTGRES_USER={this.Username}",
            $"PGUSER={this.Username}",
            $"POSTGRES_DB={this.Database}",
            $"PGDATABASE={this.Database}",
            $"POSTGRES_PASSWORD={this.Password}",
            $"PGPASSWORD={this.Password}"
        };
    }
    
    /// <inheritdoc />
    public Task PerformPostStartOperationsAsync(DockerToolsClient client, string id, CancellationToken token = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ExecuteCommandAsync(DockerToolsClient client, string id, string command, CancellationToken token = default)
    {
        var commands = RunCommandUtils.SetupCommand(this.ScriptExecutionBaseCommand, command);
        var variables = new List<string>
        {
            $"PGPASSWORD={this.Password}"
        };

        return RunCommandUtils.InternalExecuteCommandAsync(client, id, commands, variables, token);
    }

    /// <inheritdoc />
    public string CreateConnectionString(string hostPort) => $"Server=localhost;Port={hostPort};Database={this.Database};User Id={this.Username};Password={this.Password};";
}