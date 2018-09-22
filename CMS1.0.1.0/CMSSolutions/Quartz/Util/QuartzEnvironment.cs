﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using NLog;

namespace CMSSolutions.Quartz.Util
{
    /// <summary>
    /// Environment access helpers that fail gracefully if under medium trust.
    /// </summary>
    public static class QuartzEnvironment
    {
        private static readonly Logger log = LogManager.GetLogger(typeof(QuartzEnvironment).FullName);
        private static readonly bool isRunningOnMono = Type.GetType("Mono.Runtime") != null;

        /// <summary>
        /// Return whether we are currently running under Mono runtime.
        /// </summary>
        public static bool IsRunningOnMono
        {
            get { return isRunningOnMono; }
        }

        /// <summary>
        /// Retrieves the value of an environment variable from the current process.
        /// </summary>
        public static string GetEnvironmentVariable(string key)
        {
            try
            {
                return System.Environment.GetEnvironmentVariable(key);
            }
            catch (SecurityException)
            {
                log.Warn("Unable to read environment variable '{0}' due to security exception, probably running under medium trust", key);
            }
            return null;
        }

        /// <summary>
        /// Retrieves all environment variable names and their values from the current process.
        /// </summary>
        public static IDictionary<string, string> GetEnvironmentVariables()
        {
            var retValue = new Dictionary<string, string>();
            try
            {
                IDictionary variables = System.Environment.GetEnvironmentVariables();
                foreach (string key in variables.Keys)
                {
                    retValue[key] = variables[key] as string;
                }
            }
            catch (SecurityException)
            {
                log.Warn("Unable to read environment variables due to security exception, probably running under medium trust");
            }
            return retValue;
        }
    }
}