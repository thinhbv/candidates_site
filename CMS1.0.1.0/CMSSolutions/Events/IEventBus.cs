using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CMSSolutions.Events
{
    public interface IEventBus : IDependency
    {
        IEnumerable Notify(string messageName, IDictionary<string, object> eventData);

        void Notify<TEventHandler>(Expression<Action<TEventHandler>> expression) where TEventHandler : IEventHandler;

        void NotifyContentCreating<T>(T contentItem);

        void NotifyContentCreated<T>(T contentItem);

        void NotifyContentUpdating<T>(T contentItem);

        void NotifyContentUpdated<T>(T contentItem);

        void NotifyContentRemoving<T>(T contentItem);

        void NotifyContentRemoved<T>(T contentItem);
    }
}