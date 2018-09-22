using System;
using Castle.Core.Logging;
using NLog;

namespace CMSSolutions.Logging
{
    public class NLogLogger : LevelFilteredLogger
    {
        private readonly Logger logger;

        public NLogLogger(Type type)
        {
            logger = LogManager.GetLogger(type.FullName);
            if (logger.IsDebugEnabled)
            {
                Level = LoggerLevel.Debug;
            }
            else if (logger.IsInfoEnabled)
            {
                Level = LoggerLevel.Info;
            }
            else if (logger.IsWarnEnabled)
            {
                Level = LoggerLevel.Warn;
            }
            else if (logger.IsErrorEnabled)
            {
                Level = LoggerLevel.Error;
            }
            else if (logger.IsFatalEnabled)
            {
                Level = LoggerLevel.Fatal;
            }
        }

        public override ILogger CreateChildLogger(string loggerName)
        {
            throw new NotSupportedException("NLog does not support child logger.");
        }

        protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
        {
            switch (loggerLevel)
            {
                case LoggerLevel.Off:
                    break;
                case LoggerLevel.Fatal:
                    logger.LogException(LogLevel.Fatal, message, exception);
                    break;
                case LoggerLevel.Error:
                    logger.LogException(LogLevel.Error, message, exception);
                    break;
                case LoggerLevel.Warn:
                    logger.LogException(LogLevel.Warn, message, exception);
                    break;
                case LoggerLevel.Info:
                    logger.LogException(LogLevel.Info, message, exception);
                    break;
                case LoggerLevel.Debug:
                    logger.LogException(LogLevel.Debug, message, exception);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("loggerLevel");
            }
        }
    }
}
