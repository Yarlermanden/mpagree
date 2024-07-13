using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Server.Utils;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddTypesImplementing<TInterface>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TInterface : class
	{
		return services.Add(Reflection.Implements<TInterface>().Select(impl => new ServiceDescriptor(impl, impl, lifetime)));
	}

	public static IServiceCollection AddImplementations<TInterface>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TInterface : class
	{
		return services.Add(Reflection.Implements<TInterface>().Select(impl => new ServiceDescriptor(typeof(TInterface), impl, lifetime)));
	}
}


public static class Reflection
{
	public static IEnumerable<Type> All(Type assemblyType)
	{
		return assemblyType.GetTypeInfo().Assembly.GetTypes();
	}

	public static IEnumerable<Type> ClassesInNamespace(Assembly assembly, string nameSpace)
	{
		return assembly.GetTypes().Where(t => !t.GetTypeInfo().IsInterface && !t.GetTypeInfo().IsAbstract && string.Equals(t.Namespace, nameSpace, StringComparison.InvariantCultureIgnoreCase));
	}

	public static IEnumerable<Type> Implements<TInterface>() where TInterface : class
	{
		return All(typeof(TInterface)).Where(t => !t.GetTypeInfo().IsInterface && !t.GetTypeInfo().IsAbstract && typeof(TInterface).IsAssignableFrom(t));
	}

	public static IEnumerable<Type> ImplementsInAssembly<TInterface>(Type assemblyType) where TInterface : class
	{
		return All(assemblyType).Where(t => !t.GetTypeInfo().IsInterface && !t.GetTypeInfo().IsAbstract && typeof(TInterface).IsAssignableFrom(t));
	}
}