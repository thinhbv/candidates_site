using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Autofac;
using CMSSolutions.Exceptions;
using CMSSolutions.Localization;

namespace CMSSolutions.Events
{
    public class DefaultEventBus : IEventBus
    {
        private readonly Func<IEnumerable<IEventHandler>> eventHandlers;
        private readonly IExceptionPolicy exceptionPolicy;
        private static readonly ConcurrentDictionary<string, MethodInfo> interfaceMethodsCache = new ConcurrentDictionary<string, MethodInfo>();
        private readonly IComponentContext componentContext;

        public DefaultEventBus(Func<IEnumerable<IEventHandler>> eventHandlers, IExceptionPolicy exceptionPolicy, IComponentContext componentContext)
        {
            this.eventHandlers = eventHandlers;
            this.exceptionPolicy = exceptionPolicy;
            this.componentContext = componentContext;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        #region IEventHandler

        public IEnumerable Notify(string messageName, IDictionary<string, object> eventData)
        {
            // call ToArray to ensure evaluation has taken place
            return NotifyHandlers(messageName, eventData).ToArray();
        }

        private IEnumerable<object> NotifyHandlers(string messageName, IDictionary<string, object> eventData)
        {
            string[] parameters = messageName.Split('.');
            if (parameters.Length != 2)
            {
                throw new ArgumentException(T("{0} is not formatted correctly", messageName).Text);
            }
            string interfaceName = parameters[0];
            string methodName = parameters[1];

            var handlers = eventHandlers().OrderByDescending(x => x.Priority).ToList();
            foreach (var eventHandler in handlers)
            {
                IEnumerable returnValue;
                if (TryNotifyHandler(eventHandler, interfaceName, methodName, eventData, out returnValue))
                {
                    if (returnValue != null)
                    {
                        foreach (var value in returnValue)
                        {
                            yield return value;
                        }
                    }
                }
            }
        }

        private bool TryNotifyHandler(IEventHandler eventHandler, string interfaceName, string methodName, IDictionary<string, object> eventData, out IEnumerable returnValue)
        {
            try
            {
                return TryInvoke(eventHandler, interfaceName, methodName, eventData, out returnValue);
            }
            catch (Exception exception)
            {
                if (!exceptionPolicy.HandleException(this, exception))
                {
                    throw;
                }

                returnValue = null;
                return false;
            }
        }

        private static bool TryInvoke(IEventHandler eventHandler, string interfaceName, string methodName, IDictionary<string, object> arguments, out IEnumerable returnValue)
        {
            Type type = eventHandler.GetType();
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (String.Equals(interfaceType.Name, interfaceName, StringComparison.OrdinalIgnoreCase))
                {
                    return TryInvokeMethod(eventHandler, interfaceType, methodName, arguments, out returnValue);
                }
            }
            returnValue = null;
            return false;
        }

        private static bool TryInvokeMethod(IEventHandler eventHandler, Type interfaceType, string methodName, IDictionary<string, object> arguments, out IEnumerable returnValue)
        {
            MethodInfo method = interfaceMethodsCache.GetOrAdd(String.Concat(eventHandler.GetType().FullName + "_" + interfaceType.Name, "_", methodName, "_", String.Join("_", arguments.Keys)), GetMatchingMethod(interfaceType, methodName, arguments));

            if (method != null)
            {
                var parameters = new List<object>();
                foreach (var methodParameter in method.GetParameters())
                {
                    parameters.Add(arguments[methodParameter.Name]);
                }
                var result = method.Invoke(eventHandler, parameters.ToArray());
                returnValue = result as IEnumerable;
                if (returnValue == null && result != null)
                    returnValue = new[] { result };
                return true;
            }
            returnValue = null;
            return false;
        }

        private static MethodInfo GetMatchingMethod(Type interfaceType, string methodName, IDictionary<string, object> arguments)
        {
            var allMethods = new List<MethodInfo>(interfaceType.GetMethods());
            var candidates = new List<MethodInfo>(allMethods);

            foreach (var method in allMethods)
            {
                if (String.Equals(method.Name, methodName, StringComparison.OrdinalIgnoreCase))
                {
                    ParameterInfo[] parameterInfos = method.GetParameters();
                    foreach (var parameter in parameterInfos)
                    {
                        if (!arguments.ContainsKey(parameter.Name))
                        {
                            candidates.Remove(method);
                            break;
                        }
                    }
                }
                else
                {
                    candidates.Remove(method);
                }
            }

            if (candidates.Count != 0)
            {
                return candidates.OrderBy(x => x.GetParameters().Length).Last();
            }

            return null;
        }

        public void Notify<TEventHandler>(Expression<Action<TEventHandler>> expression) where TEventHandler : IEventHandler
        {
            var action = expression.Compile();
            var type = typeof(TEventHandler);

            var allEventHandlers = eventHandlers();
            foreach (var eventHandler in allEventHandlers)
            {
                if (type.IsInstanceOfType(eventHandler))
                {
                    try
                    {
                        action.Invoke((TEventHandler)eventHandler);
                    }
                    catch (Exception exception)
                    {
                        if (!exceptionPolicy.HandleException(this, exception))
                        {
                            throw;
                        }
                    }
                }
            }
        }

        #endregion IEventHandler

        #region IContentHandler

        public void NotifyContentCreating<T>(T contentItem)
        {
            var handlers = componentContext.Resolve<IEnumerable<IContentHandler<T>>>().ToList();
            if (handlers.Count > 0)
            {
                var context = new CreateContentContext<T>(contentItem);
                foreach (var handler in handlers)
                {
                    handler.Creating(context);
                }
            }
        }

        public void NotifyContentCreated<T>(T contentItem)
        {
            var handlers = componentContext.Resolve<IEnumerable<IContentHandler<T>>>().ToList();
            if (handlers.Count > 0)
            {
                var context = new CreateContentContext<T>(contentItem);
                foreach (var handler in handlers)
                {
                    handler.Created(context);
                }
            }
        }

        public void NotifyContentUpdating<T>(T contentItem)
        {
            var handlers = componentContext.Resolve<IEnumerable<IContentHandler<T>>>().ToList();
            if (handlers.Count > 0)
            {
                var context = new UpdateContentContext<T>(contentItem);
                foreach (var handler in handlers)
                {
                    handler.Updating(context);
                }
            }
        }

        public void NotifyContentUpdated<T>(T contentItem)
        {
            var handlers = componentContext.Resolve<IEnumerable<IContentHandler<T>>>().ToList();
            if (handlers.Count > 0)
            {
                var context = new UpdateContentContext<T>(contentItem);
                foreach (var handler in handlers)
                {
                    handler.Updated(context);
                }
            }
        }

        public void NotifyContentRemoving<T>(T contentItem)
        {
            var handlers = componentContext.Resolve<IEnumerable<IContentHandler<T>>>().ToList();
            if (handlers.Count > 0)
            {
                var context = new RemoveContentContext<T>(contentItem);
                foreach (var handler in handlers)
                {
                    handler.Removing(context);
                }
            }
        }

        public void NotifyContentRemoved<T>(T contentItem)
        {
            var handlers = componentContext.Resolve<IEnumerable<IContentHandler<T>>>().ToList();
            if (handlers.Count > 0)
            {
                var context = new RemoveContentContext<T>(contentItem);
                foreach (var handler in handlers)
                {
                    handler.Removed(context);
                }
            }
        }

        #endregion IContentHandler
    }
}