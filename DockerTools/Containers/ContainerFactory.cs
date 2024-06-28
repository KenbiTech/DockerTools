using Docker.DotNet;
using Kenbi.DockerTools.Containers.Templates;

namespace Kenbi.DockerTools.Containers;

internal static class ContainerFactory
{
    public static IContainer<T> Create<T>(string id, DockerClient client, T containerTemplate, string hostPort)
        where T : class, IContainerTemplate
    {
        if (typeof(IDatabaseContainerTemplate).IsAssignableFrom(typeof(T)))
        {
            IDatabaseContainer<IDatabaseContainerTemplate> container = CreateDatabaseContainer(id, client, (IDatabaseContainerTemplate)containerTemplate, hostPort);
            
            return (IContainer<T>) container;
        }

        return new Container<T>(id, client, containerTemplate, hostPort);
    }
    
    private static IDatabaseContainer<T> CreateDatabaseContainer<T>(string id, DockerClient client, T containerTemplate, string hostPort)
        where T : class, IDatabaseContainerTemplate
    {
        return new DatabaseContainer<T>(id, client, containerTemplate, hostPort);
    }
}