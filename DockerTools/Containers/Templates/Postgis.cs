﻿using Docker.DotNet;
using Kenbi.DockerTools.Exceptions;
using Kenbi.DockerTools.Models;
using Kenbi.DockerTools.Operations;
using Kenbi.DockerTools.Options.Container;

namespace Kenbi.DockerTools.Containers.Templates;

/// <summary>
/// Creates a new Postgis container.
/// </summary>
public sealed class Postgis : IDatabaseContainerTemplate
{
    public string Image => "postgis/postgis";
    public string Tag { get; private set; } = "16-3.4";
    public string Database { get; private set; } = "postgres";
    public string Username { get; private set; } = "postgres";
    public string Password { get; private set; } = "postgres";
    Dictionary<string, string> IContainerTemplate.Volumes { get; }

    IEnumerable<PortConfiguration> IContainerTemplate.Ports => new List<PortConfiguration>
    {
        new()
        {
            Container = "5432"
        }
    };

    public IList<string> EnvironmentVariables => new List<string>
    {
        $"POSTGRES_USER={this.Username}",
        $"PGUSER={this.Username}",
        $"POSTGRES_DB={this.Database}",
        $"PGDATABASE={this.Database}",
        $"POSTGRES_PASSWORD={this.Password}",
        $"PGPASSWORD={this.Password}"
    };

    HealthCheck IContainerTemplate.HealthCheck => new HealthCheck
    {
        Command = "pg_isready",
        Interval = new TimeSpan(0, 0, 30),
        Timeout = new TimeSpan(0, 0, 2),
        Retries = 5,
        StartPeriod = 60
    };

    void IContainerTemplate.ReplaceDefaultParameters(DockerToolsContainerOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Tag))
        {
            this.Tag = options.Tag;
        }

        if (!string.IsNullOrWhiteSpace(options.Database))
        {
            this.Database = options.Database;
        }

        if (!string.IsNullOrWhiteSpace(options.Username))
        {
            this.Username = options.Username;
        }

        if (!string.IsNullOrWhiteSpace(options.Password))
        {
            this.Password = options.Password;
        }
    }

    Task<ScriptExecutionResult> IContainerTemplate.PerformPostStartOperationsAsync(DockerClient client, string id, CancellationToken token)
    {
        return Task.FromResult(new ScriptExecutionResult(null));
    }

    string IContainerTemplate.GetConnectionString(string hostPort)
        => GetConnectionString(hostPort, this.Database);
    
    private string GetConnectionString(string hostPort, string database)
        => $"Server=localhost;Port={hostPort};Database={database};User Id={this.Username};Password={this.Password};";

    Task<ScriptExecutionResult> IContainerTemplate.RunScriptAsync(DockerClient client, string id, string script, CancellationToken token)
    {
        var command = $"psql -U {this.Username} -d {this.Database} -q -c";

        script = "SET client_min_messages TO WARNING; " + script;

        return CommandExecutionOperations.RunScriptAsync(client, id, command, script, token);
    }
    
    async Task<string> IDatabaseContainerTemplate.CreateDatabaseAsync(DockerClient client, string id, string name, string hostPort, CancellationToken token)
    {
        var script = $"psql -U {this.Username} -d {this.Database} -q -c 'SET client_min_messages TO WARNING' -c 'DROP DATABASE IF EXISTS {name}' -c 'CREATE DATABASE {name} WITH OWNER = {this.Username};'";

        var command = "bash -c";

        var result = await CommandExecutionOperations.RunScriptAsync(client, id, command, script, token);
        if (!result.Success)
            throw new UnableToSetupContainerException("Unable to create database");

        return GetConnectionString(hostPort, name);
    }
}