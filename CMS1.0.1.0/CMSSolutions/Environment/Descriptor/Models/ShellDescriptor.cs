using System.Collections.Generic;

namespace CMSSolutions.Environment.Descriptor.Models
{
    /// <summary>
    /// Contains a snapshot of a tenant's enabled features.
    /// The information is drawn out of the shell via IShellDescriptorManager
    /// and cached by the host via IShellDescriptorCache. It is
    /// passed to the ICompositionStrategy to build the ShellBlueprint.
    /// </summary>
    public class ShellDescriptor
    {
        public ShellDescriptor()
        {
            Features = new List<ShellFeature>();
        }

        public int SerialNumber { get; set; }

        public IList<ShellFeature> Features { get; set; }
    }
}