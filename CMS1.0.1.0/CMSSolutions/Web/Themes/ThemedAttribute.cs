using System;

namespace CMSSolutions.Web.Themes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ThemedAttribute : Attribute
    {
        public ThemedAttribute()
        {
            Enabled = true;
        }

        public ThemedAttribute(bool isFullLayout)
        {
            Enabled = true;
            Minimal = !isFullLayout;
        }

        public bool Enabled { get; set; }

        public bool IsDashboard { get; set; }

        public bool Minimal { get; set; }

        public int Priority { get; set; }
    }
}