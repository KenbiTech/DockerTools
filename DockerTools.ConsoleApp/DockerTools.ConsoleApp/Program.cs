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
        .WithDefaultDatabase("MatchMaking"))
    .Build();

client
    .Setup<PostgresContainer>()
    .Build();

await client.CreateAsync();
await client.StartAsync();
await client.StopAndRemoveAsync();

await client.DisposeAsync();

Console.ReadLine();