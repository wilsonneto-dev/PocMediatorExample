using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace PocMediatorExample.Mediator;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        params Type[] markers)
    {
        var handlerDictionary = new Dictionary<Type, Type>();
        foreach(var marker in markers)
        {
            var assembly = marker.Assembly;
            var requests = GetClassesImplementingINterfaces(assembly, typeof(IRequest<>));
            var handlers = GetClassesImplementingINterfaces(assembly, typeof(IHandler<,>));

            requests.ToList().ForEach(request =>
            {
                var handler = handlers.SingleOrDefault(h => h.GetInterfaces().Where(x => x.IsGenericType).Any(i => i.GetGenericArguments()[0] == request));
                if(handler is null) 
                    return;
                handlerDictionary.Add(request, handler);
            });

            var serviceDescriptors = handlers.Select(x => new ServiceDescriptor(x, x, lifetime));
            services.TryAdd(serviceDescriptors);
        }
        
        services.AddScoped<IMediator>(x => new Mediator(x.GetRequiredService, handlerDictionary));
        return services;
    }

    private static IEnumerable<Type> GetClassesImplementingINterfaces(Assembly assembly, Type type) 
        => assembly.ExportedTypes.Where(exportedTypes =>
            {
                var implementRequestType = exportedTypes.GetInterfaces()
                    .Where(x => x.IsGenericType)
                    .Any(x => x.GetGenericTypeDefinition() == type);

                return !exportedTypes.IsInterface && !exportedTypes.IsAbstract && implementRequestType;
            });
}
