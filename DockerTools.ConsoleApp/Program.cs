// See https://aka.ms/new-console-template for more information

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
        
        for (var i = 0; i < 5; i++)
        {
            tasks.Add(ContainerRunAsync(i + 1));
        }
        
        await Task.WhenAll(tasks);
        
        Console.WriteLine("Execution ended");
    }

    private static async Task ContainerRunAsync(int instance)
    {
        Console.WriteLine($"Instance {instance} starting...");
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        IContainer<Postgis>? container;
        try
        {
            container = await new DockerTools<Postgis>()
                //.WithCleanUp(true)
                .CreateAsync();
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

        await container.DisposeAsync();
        
        Console.WriteLine($"Instance {instance} has been removed");
    }
}