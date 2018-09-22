#region License

/*
 * All content copyright Terracotta, Inc., unless otherwise indicated. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */

#endregion License

using System;
using System.Net;
using NLog;
using CMSSolutions.Quartz.Spi;

namespace CMSSolutions.Quartz.Simpl
{
    /// <summary>
    /// Helper base class for host name lookup requiring instance id generators.
    /// </summary>
    /// <author>Marko Lahma</author>
    public abstract class HostNameBasedIdGenerator : IInstanceIdGenerator
    {
        protected const int IdMaxLengh = 50;

        private readonly Logger logger;

        protected HostNameBasedIdGenerator()
        {
            logger = LogManager.GetLogger(GetType().FullName);
        }

        /// <summary>
        /// Generate the instance id for a <see cref="IScheduler" />
        /// </summary>
        /// <returns> The clusterwide unique instance id.
        /// </returns>
        public abstract string GenerateInstanceId();

        protected string GetHostName(int maxLenght)
        {
            try
            {
                string hostName = GetHostAddress().HostName;
                if (hostName != null && hostName.Length > maxLenght)
                {
                    string newName = hostName.Substring(0, maxLenght);
                    logger.Warn("Host name '{0}' was too long, shortened to '{1}'", hostName, newName);
                    hostName = newName;
                }
                return hostName;
            }
            catch (Exception e)
            {
                throw new SchedulerException("Couldn't get host name!", e);
            }
        }

        protected virtual IPHostEntry GetHostAddress()
        {
            return Dns.GetHostByAddress(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString());
        }
    }
}