// See https://aka.ms/new-console-template for more information

using Kenbi.DockerTools.Containers;
using Kenbi.DockerTools.Extensions;
using Kenbi.DockerTools.Initialization;

Console.WriteLine("Hello, World!");

var client = new DockerToolsClientConfiguration()
    .UsingUri(new Uri("http://localhost:2375"))
    .Create();

client
    .Setup<PostgresContainer, PostgresContainerParameters>()
    .WithParameters(() => new PostgresContainerParameters()
        .WithDefaultDatabase("Recruitments"))
    .Build();

client
    .Setup<PostgisContainer, PostgisContainerParameters>()
    .WithParameters(() => new PostgisContainerParameters()
        .WithDefaultDatabase("MatchMaking")
        .WithVersion("latest"))
    .Build();

client
    .Setup<PostgresContainer>()
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

Console.ReadLine();
