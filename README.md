# DockerTools

DockerTools is a simple wrapper on top of [Docker.DotNet](https://github.com/dotnet/Docker.DotNet).

Seen as how it's meant for internal Kenbi use, it's been preconfigured with Postgres support, in order to be used with integration and database testing.
However, it offers basic support to initialize any other container (currently, only bridge mode is supported).

## How to use

Install the package, then

```csharp
var client = new DockerToolsClientConfiguration()
    .UsingEnvironmentDetection()
    .Create();
```

This will initialize a new client, automatically detecting the connection to the Docker client. If necessary, you can specify the type of connection you want to use: via uri (for Docker Remote API), Windows Pipes, or Unix socket.

To configure a new Postgres container, simply do

```csharp
client
    .Setup<PostgresContainer>()
    .Build();
```

If you need to tweak some parameters, you can call the optional `WithParameters` method:

```csharp
client
    .Setup<PostgresContainer, PostgresContainerParameters>()
    .WithParameters(() => new PostgresContainerParameters()
        .WithDefaultDatabase("MyDatabase")
        .WithUsername("MyUsername")
        .WithPassword("MyPassword"))
    .Build();
```

If you need to use another type of container, you can use the `GenericContainer` class:

```csharp
client
    .Setup<GenericContainer>()
    .WithParameters(() => new GenericContainerParameters()
        .WithImage("MyImage:latest")
        .WithPorts(new List<PortConfiguration>
        {
            new()
            {
                Container = "1000",
                Host = "1000" // optional
            }
        })
        .WithEnvironmentVariables(new List<string>
        {
            "IsGeneric=true"
        })
        .WithHealthCheck(new HealthCheckConfig
        {
            Test = new List<string>
            {
                "CMD-SHELL",
                "<command to run>"
            },
            Interval = new TimeSpan(0, 0, 10),
            Retries = 3,
            Timeout = new TimeSpan(0, 0, 10),
            StartPeriod = 10

        }));
```

Note that not all parameters are required:

| Property                | Required | Notes                                                                                                                                                                            |
|-------------------------|----------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Image                   |Yes       | Must be formated as "image name:tag", e.g., "postgres:14.3".                                                                                                                     |
| Ports                   |Yes       | At least one container port must be assigned. Confirm on the image documentation what port(s) to map. If no host port is mapped, Docker will automatically assign one at random. |
| Environment Variables   |N/A| Will depend on the image requirements. Confirm on the image documentation what environment variables are available/required.                                                     |
| Health Check            |No| Will depend on requirements and image used. Recommended when possible.                                                                                                           |
| Health Check/Test       |Yes| If configuring health checks, this must be supplied. First entry should be "CMD-SHELL".                                                                                          |
| HealthCheck/Interval    |No||
| HealthCheck/Retries     |No||
| HealthCheck/Timeout     |No||
| HealthCheck/StartPeriod |No||

To create the containers, run:

```csharp
await client.CreateAsync();
```

Starting the containers is as simple as running:

```csharp
await client.StartAsync();
```

When done, you can remove the containers by using:

```csharp
await client.StopAndRemoveAsync();
```

All three methods return a collection of reports, one per container, that will help with diagnosing issues with setting up and running:

```csharp
var creationReports = await client.CreateAsync(); //List<CreationReport>
var startupReports = await client.CreateAsync(); //List<StartupReport>
var shutdownReports = await client.CreateAsync(); //List<StopAndRemoveReport>
```

## Database containers

For containers such as the postgres one, you can retrieve the connection string to access by checking its `AdditionalInformation`:

```csharp
var connectionString = client
    .Containers
    .First(x => x.Name == "postgres")
    .AdditionalInformation
    .ConnectionString;
```

The container property `Name` will always match the image name, i.e., for a generic container using image `MyImage`, `Name` will be `MyImage`.

## Expanding the library

You can create your own container setups by implementing a class that extends `IContainer`:

```csharp
public class MyContainer : IContainer
```

If setting up a database container, extend from `IDatabaseContainer` instead. While this is not required, this interface exposes methods that help with setting up a connection string.