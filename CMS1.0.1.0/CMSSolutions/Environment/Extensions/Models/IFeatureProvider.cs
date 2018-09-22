using System.Collections.Generic;

namespace CMSSolutions.Environment.Extensions.Models
{
    public interface IFeatureProvider
    {
        IEnumerable<FeatureDescriptor> AvailableFeatures();
    }
}