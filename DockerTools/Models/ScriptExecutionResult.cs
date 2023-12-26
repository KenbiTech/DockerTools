namespace Kenbi.DockerTools.Models;

public class ScriptExecutionResult
{
    public string? ErrorMessage { get; }

    public bool Success { get; }
    
    internal ScriptExecutionResult(string? stderr)
    {
        if (!string.IsNullOrWhiteSpace(stderr))
        {
            this.ErrorMessage = stderr;
        }

        this.Success = string.IsNullOrWhiteSpace(this.ErrorMessage);
    }

    public static implicit operator bool(ScriptExecutionResult x) => x.Success;

    public override string? ToString() => this.ErrorMessage;
}