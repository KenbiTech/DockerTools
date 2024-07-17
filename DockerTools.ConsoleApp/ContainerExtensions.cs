using System.Data.Common;
using Kenbi.DockerTools.Containers;
using Kenbi.DockerTools.Containers.Templates;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace DockerTools.ConsoleApp;

public static class ContainerExtensions
{
    public static DbConnection Connect(this IDatabaseContainer container, string connectionString)
    {
        if (container.GetTemplateType() == typeof(SqlServer))
            return new SqlConnection(connectionString);
        
        return new NpgsqlConnection(connectionString);
    }

    public static DbCommand CreateCommand(this IDatabaseContainer container, DbConnection connection, string commandText)
    {
        if (container.GetTemplateType() == typeof(SqlServer))
            return new SqlCommand(commandText, (SqlConnection)connection);
        
        return new NpgsqlCommand(commandText, (NpgsqlConnection)connection);
    }
    
    public static string GetListDatabasesQuery(this IDatabaseContainer container)
    {
        if (container.GetTemplateType() == typeof(SqlServer))
            return "SELECT name FROM sys.databases";
        
        return "SELECT datname FROM pg_database WHERE datistemplate = false";
    }
}