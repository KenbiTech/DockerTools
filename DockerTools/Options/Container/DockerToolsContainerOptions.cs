namespace Kenbi.DockerTools.Options.Container;

/// <summary>
/// Options for setting container configurations.
/// </summary>
public class DockerToolsContainerOptions
{
    internal string? Tag;
    internal string? Database;
    internal string? Username;
    internal string? Password;
    internal Guid? InstanceId;

    /// <summary>
    /// Allows using a specific version of the container image.
    /// </summary>
    /// <param name="value">The version to use; check image repository for valid versions.</param>
    /// <returns>The <see cref="DockerToolsContainerOptions"/> instance.</returns>
    public DockerToolsContainerOptions WithTag(string value)
    {
        this.Tag = value;

        return this;
    }
    
    /// <summary>
    /// Replaces the default database.
    /// </summary>
    /// <param name="value">The new database name.</param>
    /// <returns>The <see cref="DockerToolsContainerOptions"/> instance.</returns>
    public DockerToolsContainerOptions WithDatabase(string value)
    {
        this.Database = value;
        
        return this;
    }

    /// <summary>
    /// Replaces the default username.
    /// </summary>
    /// <param name="value">The new username.</param>
    /// <returns>The <see cref="DockerToolsContainerOptions"/> instance.</returns>
    public DockerToolsContainerOptions WithUsername(string value)
    {
        this.Username = value;

        return this;
    }

    /// <summary>
    /// Replaces the default password.
    /// </summary>
    /// <param name="value">The new password.</param>
    /// <returns>The <see cref="DockerToolsContainerOptions"/> instance.</returns>
    public DockerToolsContainerOptions WithPassword(string value)
    {
        this.Password = value;

        return this;
    }

    internal DockerToolsContainerOptions WithInstanceId(Guid value)
    {
        this.InstanceId = value;

        return this;
    }
}