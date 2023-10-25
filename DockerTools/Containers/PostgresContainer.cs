using Kenbi.DockerTools.Containers.Interfaces;
using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Containers;

/// <inheritdoc />
public class PostgresContainer : IDatabaseContainer
{
    /// <inheritdoc />
    public string Image => "postgres";

    /// <inheritdoc />
    public string Tag => "14.3";

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

    /// <inheritdoc />
    public void UpsertParameters(IContainerParameters parameters)
    {
        var mappedParameters = (PostgresContainerParameters)parameters;

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
    }

    /// <inheritdoc />
    public IList<string> GetEnvironmentVariables()
    {
        return new List<string>
        {
            $"POSTGRES_USER={this.Username}",
            $"POSTGRES_DB={this.Database}",
            $"POSTGRES_PASSWORD={this.Password}"
        };
    }

    /// <inheritdoc />
    public string CreateConnectionString(string hostPort)
    {
        return $"Server=localhost;Port={hostPort};Database={this.Database};User Id={this.Username};Password={this.Password};";
    }
}