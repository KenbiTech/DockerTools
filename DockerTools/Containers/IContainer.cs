﻿using Docker.DotNet;
using Kenbi.DockerTools.Containers.Templates;
using Kenbi.DockerTools.Models;

namespace Kenbi.DockerTools.Containers;

/// <summary>
/// Represents an instance of a DockerTools container tracker.
/// </summary>
public interface IContainer : IAsyncDisposable, IDisposable
{
    internal DockerClient Client { get; }
    
    /// <summary>
    /// The identifier of the container on Docker.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Run a script on the container's database.
    /// </summary>
    /// <param name="script">The script to run.</param>
    /// <param name="token">A cancellation token. Optional.</param>
    /// <returns>True if script ran successfully; false otherwise.</returns>
    public Task<ScriptExecutionResult> RunScriptAsync(string script, CancellationToken token = default);
    
    public Type GetTemplateType();
}