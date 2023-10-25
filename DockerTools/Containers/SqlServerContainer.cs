using Kenbi.DockerTools.Containers.Interfaces;
using Kenbi.DockerTools.Utils;

namespace Kenbi.DockerTools.Containers;

/// <inheritdoc />
public class SqlServerContainer : IDatabaseContainer
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

    private IList<string> EnvironmentVariables => new List<string>
    {
        "ACCEPT_EULA=true"
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
    public string CreateConnectionString(string hostPort)
    {
        return string.Empty;
    }
}