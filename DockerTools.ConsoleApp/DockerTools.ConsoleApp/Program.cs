// See https://aka.ms/new-console-template for more information

using Kenbi.DockerTools.Containers;
using Kenbi.DockerTools.Extensions;
using Kenbi.DockerTools.Initialization;

Console.WriteLine("Hello, World!");

try
{
    var client = new DockerToolsClientConfiguration()
        .UsingEnvironmentDetection()
        .Create();

    client
        .Setup<PostgresContainer, PostgresContainerParameters>()
        .WithParameters(() => new PostgresContainerParameters()
            .WithDefaultDatabase("Recruitments"))
        .Build();

    try
    {
        await client.CreateAsync();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error when creating containers: {ex.Message}");
    }

    await client.StartAsync();
    
    await client.StopAndRemoveAsync();

    await client.DisposeAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"{ex.Message}: {ex.InnerException!.Message}");
}

Console.WriteLine("Press Enter to complete run...");
Console.ReadLine();