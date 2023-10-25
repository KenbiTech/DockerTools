using Kenbi.DockerTools.Containers.Interfaces;

namespace Kenbi.DockerTools.Containers;

/// <summary>
/// 
/// </summary>
public class PostgresContainerParameters : IContainerParameters
{
    internal string Username = "postgres";
    internal string Password = "postgres";
    internal string Database = "postgres";
    
    /// <summary>
    /// Allows for configuration of a default user.
    /// If not set, default "postgres" is used.
    /// </summary>
    /// <param name="username">The username to set.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If an empty or null value is supplied.</exception>
    public PostgresContainerParameters WithUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(
                nameof(username),
                "Username cannot be empty. If you wish to use the default value, do not invoke this method.");
        }

        this.Username = username;

        return this;
    }

    /// <summary>
    /// Allows for configuration of a default password.
    /// If not set, default "postgres" is used.
    /// </summary>
    /// <param name="password">The password to set.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If an empty or null value is supplied.</exception>
    public PostgresContainerParameters WithPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentNullException(
                nameof(password),
                "Password cannot be empty. If you wish to use the default value, do not invoke this method.");
        }
        
        this.Password = password;
        
        return this;
    }

    /// <summary>
    /// Allows for configuration of a default database.
    /// </summary>
    /// <param name="database">The database name to set.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If an empty or null value is supplied.</exception>
    public PostgresContainerParameters WithDefaultDatabase(string database)
    {
        if (string.IsNullOrWhiteSpace(database))
        {
            throw new ArgumentNullException(
                nameof(database),
                "Database cannot be empty. If you wish to use the default value, do not invoke this method.");
        }
        
        this.Database = database;
        
        return this;
    }
}