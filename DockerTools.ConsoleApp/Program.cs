// See https://aka.ms/new-console-template for more information

using System.Data.Common;
using System.Diagnostics;
using Kenbi.DockerTools;
using Kenbi.DockerTools.Containers;
using Kenbi.DockerTools.Containers.Templates;

namespace DockerTools.ConsoleApp;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Starting new DockerTools run...");

        var tasks = new List<Task>();
        
        tasks.Add(ContainerRunAsync<Postgres>(1));
        tasks.Add(ContainerRunAsync<Postgis>(2));
        tasks.Add(ContainerRunAsync<SqlServer>(3));
        
        await Task.WhenAll(tasks);
        
        Console.WriteLine("Execution ended");
    }

    private static async Task ContainerRunAsync<T>(int instance)
        where T : class, IDatabaseContainerTemplate, new()
    {
        Console.WriteLine($"Instance {instance} starting...");
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        IContainer? container;
        try
        {
            container = await new DockerTools<T>()
                //.WithCleanUp(true)
                .CreateDatabaseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Instance {instance} failed: {ex.Message}");
            return;
        }
        finally
        {
            stopwatch.Stop();
            Console.WriteLine($"Instance {instance} initialization time: {stopwatch.ElapsedMilliseconds}ms");
        }

        if (typeof(T) == typeof(Postgis))
        {
            const string script = @"CREATE TABLE spatial_ref_sys2
(
    srid integer NOT NULL
);";

            var result = await container.RunScriptAsync(script);

            if (result)
            {
                Console.WriteLine($"Instance {instance} ran script successfully!");
            }
            else
            {
                Console.WriteLine($"Instance {instance} script failed with message: " + result);
            }
        }

        try
        {
            var dbContainer = container as IDatabaseContainer;
            if (dbContainer is null)
            {
                Console.WriteLine($"Container {instance} does not represent a database container type");
                return;
            }

            var connectionString = await dbContainer.CreateDatabaseAsync("test");
            
            using (var connection = dbContainer.Connect(connectionString))
            {
                await connection.OpenAsync();

                Console.WriteLine($"Databases for instance {instance}:");
                ListDatabases(dbContainer, connection);
                CreateTable(dbContainer, connection);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        await container.DisposeAsync();
        
        Console.WriteLine($"Instance {instance} has been removed");
    }

    private static void ListDatabases(IDatabaseContainer container, DbConnection connection)
    {
        string sql = container.GetListDatabasesQuery();

        using (var command = container.CreateCommand(connection, sql))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine("{0}", reader.GetString(0));
                }
            }
        }
    }
    
    private static void CreateTable(IDatabaseContainer container, DbConnection connection)
    {
        string sql = "CREATE TABLE test (id INT);";

        using (var command = container.CreateCommand(connection, sql))
        {
            command.ExecuteNonQuery();
            
            Console.WriteLine("Created table with success");
        }
    }
}