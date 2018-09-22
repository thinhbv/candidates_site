using System;
using System.Collections.Generic;

namespace CMSSolutions.Environment.Extensions.Models
{
    public class Feature
    {
        public FeatureDescriptor Descriptor { get; set; }

        public IEnumerable<Type> ExportedTypes { get; set; }

        public override string ToString()
        {
            return Descriptor.Id;
        }
    }
}