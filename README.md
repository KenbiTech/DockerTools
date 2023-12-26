# DockerTools

DockerTools is a simple wrapper on top of [Docker.DotNet](https://github.com/dotnet/Docker.DotNet).

DockerTools was created as a means of abstracting the complexity of managing database containers when running automated tests. It provides a simple, fluent approach to creating, interacting, and disposing of containers.

## How to use
## Version 2.x
### Creating a container
Install the package, then simply perform the following:
```csharp
var container = await new DockerToolsContainer<Postgres>()
           .CreateAsync();
```

If necessary, certain default values can be replaced by using the `WithParameters()` method:

```csharp
var container = await new DockerTools<Postgres>()
           .WithParameters(options => options
               .WithDatabase("MyDatabase")
               .WithUsername("myUser")
               .WithPassword("myPassword")
               .WithTag("14.3"))
           .CreateAsync();
```
Note that you don't have to call all options; simply use the ones you need.

By default, DockerTools will attempt to identify the best connection to the local Docker instance. if you want to force use of the Remote API, you can do so:

```csharp
var container = await new DockerTools<Postgres>()
           .WithConnection(options => options.ConnectionType = new RemoteApiConnection(new Uri("http://localhost:2375")))
           .CreateAsync();
```

Both methods can be combined:

```csharp
var container = await new DockerTools<Postgres>()
           .WithParameters(options => options
               .WithDatabase("myDatabase")
               .WithUsername("myUser")
               .WithPassword("myPassword")
               .WithTag("14.3"))
           .WithConnection(options => options.ConnectionType = new RemoteApiConnection(new Uri("http://localhost:2375")))
           .CreateAsync();
```

### Using the container
You can obtain the container's connection string by checking its `ConnectionString` property like this:
```csharp
var connectionString = container.ConnectionString;
```

You can also run scripts by calling the following method:
```csharp
var result = container.RunScriptAsync("SELECT * from users");
```
The method will return an object indicating if the script ran successfully, with an optional field
indicating the error if one exists.

### Clean up
When done with the container, simply invoke its `DisposeAsync` method:
```csharp
container.DisposeAsync();
```
This will remove the container from the Docker host and dispose of the underlying connection to it.

Note that the `CreateAsync()` method generates an `IAsyncDisposable` object, meaning you can wrap
the entire usage of it in an `await using` statement:
```csharp
await using (var container = await new DockerTools<Postgres>().CreateAsync())
{
    ...
}
```

### Supported Databases
Version 2.x uses the following images and versions for its databases:

| Container | Image | Version     |
|-----------|-------|-------------|
| Postgres  | postgres | 14.3        |
| Postgis   | postgis/postgis | 14-3.4      |
| SQL Server | mcr.microsoft.com/mssql/server | 2022-latest |

## Version 1.x
### Initializing a new connection

Install the package, then initialize a new client instance with

```csharp
var client = new DockerToolsClientConfiguration()
    .UsingEnvironmentDetection()
    .Create();
```

If necessary, you can specify the type of connection you want to use: via uri (for Docker Remote API), Windows Pipes, or Unix socket:

```csharp
var client = new DockerToolsClientConfiguration()
    .UsingUri(new Uri("http://localhost:2375"))
    .Create();
```

```csharp
var client = new DockerToolsClientConfiguration()
    .UsingWindowsPipes()
    .Create();
```

```csharp
var client = new DockerToolsClientConfiguration()
    .UsingUnixSocket()
    .Create();
```

If DockerTools cannot connect to the Docker instance, it will throw a `DockerUnreachableException`.

### Managing containers
For the built-in containers, simply do the following:

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
        .WithPassword("MyPassword")
        .WithVersion("16.1"))
    .Build();
```
Note that the parameters can vary from container to container.

Natively available containers are:

| Container | Image | Version |
|-----------|-------|---------|
| Postgres  | postgres | 14.3 |
| Postgis   | postgis/postgis | 14-3.3 |
| SQL Server | mcr.microsoft.com/mssql/server | 2022-latest |

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
| Image                   |Yes       | Must be formatted as "image name:tag", e.g., "postgres:14.3".                                                                                                                    |
| Ports                   |Yes       | At least one container port must be assigned. Confirm on the image documentation what port(s) to map. If no host port is mapped, Docker will automatically assign one at random. |
| Environment Variables   |N/A| Will depend on the image requirements. Confirm on the image documentation what environment variables are available/required.                                                     |
| Health Check            |No| Will depend on requirements and image used. Recommended when possible.                                                                                                           |
| Health Check/Test       |Yes| If configuring health checks, this must be supplied. First entry should be "CMD-SHELL".                                                                                          |
| HealthCheck/Interval    |No|                                                                                                                                                                                  |
| HealthCheck/Retries     |No|                                                                                                                                                                                  |
| HealthCheck/Timeout     |No|                                                                                                                                                                                  |
| HealthCheck/StartPeriod |No|                                                                                                                                                                                  |

To create the containers, run:

```csharp
await client.CreateAsync();
```
This is create the container on Docker, but will not start it. If the image is not present, it will be automatically obtained.

To start the container, run:

```csharp
await client.StartAsync();
```

When done, you can remove the containers by using:

```csharp
await client.StopAndRemoveAsync();
```
Note that only containers created by DockerTools are affected by this operation. Previously existing ones remain untouched.

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

## Running scripts

As of version 1.5.0, you can run scripts on the container itself. Note that if the container is an `IDatabaseContainer`, the script will be run directly on underlying database (only applies to the built-in container types of DockerTools).

To run a script, simply select the container you want, and run the `ExecuteCommandAsync` method:

```csharp
var container = client.Containers.First(x => x.Name == "postgres");
await container.ExecuteCommandAsync(...);
```

Notes:
- the container needs to be running (and healthy, if the container supports health checks) in order for the command to run.
- DockerTools does not currently report if the command ran successfully, or at all.

## Expanding the library

You can create your own container setups by implementing a class that extends `IContainer`:

```csharp
public class MyContainer : IContainer
```

If setting up a database container, extend from `IDatabaseContainer` instead. While this is not required, this interface exposes methods that help with setting up the connection string.

For both cases, if flexibility is needed when initializing a container, you can create your own parameters class by inheriting from `IContainerParameters`:

```csharp
public class MyContainerParameters : IContainerParameters
```