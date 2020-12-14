using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Sentinel.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all implementations of an interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services">ServiceCollection</param>
        /// <param name="assemblies">Assemblies to look in to get implementations, defaults to assembly the interface is in</param>
        /// <param name="lifetime">Service Lifetime, defaults to Transient</param>
        /// <returns></returns>
        public static IServiceCollection RegisterAllImplementations<T>(this IServiceCollection services, Assembly[] assemblies = null,
            ServiceLifetime lifetime = ServiceLifetime.Transient) where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException($"{typeof(T).Name} is not an interface");

            if (assemblies == null)
                assemblies = new[] {typeof(T).Assembly};

            var types =
                assemblies.SelectMany(
                    a => a.DefinedTypes.Where(x =>
                        x.IsClass && !x.IsAbstract && x.GetInterfaces().Contains(typeof(T))));

            foreach (var type in types)
                services.Add(new ServiceDescriptor(typeof(T), type, lifetime));

            return services;
        }
    }
}
