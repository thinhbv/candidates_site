using System;

namespace CMSSolutions.Web.UI
{
    public class ResourcesLookupEventArgs : EventArgs
    {
        internal ResourcesLookupEventArgs(ResourceType resourceType, ScriptRegister script, StyleRegister style)
        {
            ResourceType = resourceType;
            Script = script;
            Style = style;
        }

        public ScriptRegister Script { get; private set; }

        public StyleRegister Style { get; private set; }

        public ResourceType ResourceType { get; private set; }
    }
}