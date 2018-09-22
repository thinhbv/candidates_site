using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc
{
    public static class DependencyResolverExtensions
    {
        public static object ResolveUnregistered(this IDependencyResolver resolver, Type type)
        {
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = resolver.GetService(parameter.ParameterType);
                        if (service == null) throw new CMSException("Unkown dependency: " + parameter.ParameterType);
                        parameterInstances.Add(service);
                    }
                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (CMSException ex)
                {
                    throw new Exception("No constructor was found that had all the dependencies satisfied.", ex);
                }
            }

            throw new Exception("No constructor was found that had all the dependencies satisfied.");
        }
    }
}