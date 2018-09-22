using System;
using Castle.Core.Logging;

namespace CMSSolutions.Logging
{
    internal class DefaultLoggerFactory : ILoggerFactory
    {
        public ILogger Create(Type type)
        {
            if (type != null)
            {
                return new NLogLogger(type);
            }

            throw new ArgumentNullException("type");
        }

        public ILogger Create(string name)
        {
            throw new NotSupportedException();
        }

        public ILogger Create(Type type, LoggerLevel level)
        {
            throw new NotSupportedException();
        }

        public ILogger Create(string name, LoggerLevel level)
        {
            throw new NotSupportedException();
        }
    }
}