using System;
using Autofac;

namespace CMSSolutions.Environment
{
    public interface IShellContainerRegistrations
    {
        Action<ContainerBuilder> Registrations { get; }
    }

    public class ShellContainerRegistrations : IShellContainerRegistrations
    {
        public ShellContainerRegistrations()
        {
            Registrations = builder => { };
        }

        public Action<ContainerBuilder> Registrations { get; private set; }
    }
}