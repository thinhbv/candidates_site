using System;

namespace CMSSolutions.Environment.Extensions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class SuppressDependencyAttribute : Attribute
    {
        public SuppressDependencyAttribute(string fullName)
        {
            FullName = fullName;
        }

        public string FullName { get; set; }
    }
}