using System;
using System.Diagnostics.CodeAnalysis;

namespace CMSSolutions.Web.Mvc
{
    /// <summary>
    /// Represents a URL route
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class UrlAttribute : Attribute
    {
        private string name;
        private string url;

        public UrlAttribute()
        {
        }

        /// <param name="url">Route URL</param>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public UrlAttribute(string url)
        {
            Url = url;
        }

        /// <param name="url">Route URL</param>
        /// <param name="priority">Priority number.</param>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public UrlAttribute(string url, int priority)
        {
            Url = url;
            Priority = priority;
        }

        /// <param name="url">Route URL</param>
        /// <param name="defaults">A list of URL parameter defaults delimited by semicolon. Ex.: category=general;order=name</param>
        /// <param name="constraints">A list of URL parameter constraints delimited by semicolon. Ex.: category=[a-z]+;id=[0-9]+</param>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public UrlAttribute(string url, string defaults, string constraints)
        {
            Url = url;
            Defaults = defaults;
            Constraints = constraints;
        }

        /// <param name="url">Route URL</param>
        /// <param name="defaults">A list of URL parameter defaults delimited by semicolon. Ex.: category=general;order=name</param>
        /// <param name="constraints">A list of URL parameter constraints delimited by semicolon. Ex.: category=[a-z]+;id=[0-9]+</param>
        /// <param name="priority">Order number. Default: -1</param>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#",
            Justification = "This is not a regular URL as it may contain special routing characters.")]
        public UrlAttribute(string url, string defaults, string constraints, int priority)
        {
            Url = url;
            Defaults = defaults;
            Constraints = constraints;
            Priority = priority;
        }

        /// <summary>
        /// Name of the route
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Route name must not be null or empty.", "value");
                }

                name = value;
            }
        }

        /// <summary>
        /// Route URL
        /// </summary>
        public string Url
        {
            get { return url; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Route URL must not be null.");
                }

                url = value;
            }
        }

        public string BaseUrl { get; set; }

        /// <summary>
        /// A list of URL parameter defaults delimited by semicolon. Ex.: category=general;order=name
        /// </summary>
        public string Defaults { get; set; }

        /// <summary>
        /// A list of URL parameter constraints delimited by semicolon. Ex.: category=[a-z]+;id=[0-9]+
        /// </summary>
        public string Constraints { get; set; }

        /// <summary>
        /// Order number of the route. Default: 0
        /// </summary>
        public int Priority { get; set; }
    }
}