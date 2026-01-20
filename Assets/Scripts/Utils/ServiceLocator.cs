using System.Collections.Generic;
using System;

public static class ServiceLocator
{

    static Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void RegisterService<T>(T service)
    {
        var type = typeof(T);
        if (!services.ContainsKey(type))
        {
            services.Add(type, service);
        }
    }

    public static T GetService<T>()
    {
        var type = typeof(T);
        if (services.ContainsKey(type))
        {
            return (T)services[type];
        }
        return default(T);
    }
    
}