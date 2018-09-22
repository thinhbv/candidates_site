using System;
using System.Collections.Generic;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Environment.Extensions
{
    public class ExtensionEntry
    {
        public ExtensionDescriptor Descriptor { get; set; }

        public IEnumerable<Type> ExportedTypes { get; set; }
    }
}