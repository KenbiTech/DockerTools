// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Kenbi.DockerTools;
using Kenbi.DockerTools.Containers.Templates;

Console.WriteLine("Starting new DockerTools run...");

var stopwatch = new Stopwatch();
stopwatch.Start();
var container = await new DockerTools<Postgres>()
    //.WithCleanUp(true)
    .CreateAsync();
stopwatch.Stop();
Console.WriteLine($"Initialization time: {stopwatch.ElapsedMilliseconds}ms");

const string script = @"CREATE TABLE spatial_ref_sys
(
    srid integer NOT NULL
);";
    
var result = await container.RunScriptAsync(script);

if (result)
{
    Console.WriteLine("Ran script successfully!");
}
else
{
    Console.WriteLine("Script failed with message: " + result);
}

await container.DisposeAsync();

Console.WriteLine("Execution ended, press Enter to terminate session...");
Console.ReadLine();