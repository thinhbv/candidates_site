using System;

namespace CMSSolutions.Environment.Extensions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureAttribute : Attribute
    {
        public FeatureAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}