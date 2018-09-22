using Autofac;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Environment.ShellBuilders.Models;

namespace CMSSolutions.Environment.ShellBuilders
{
    public class ShellContext
    {
        public ShellSettings Settings { get; set; }

        public ShellDescriptor Descriptor { get; set; }

        public ShellBlueprint Blueprint { get; set; }

        public ILifetimeScope LifetimeScope { get; set; }

        public ICMSShell Shell { get; set; }
    }
}