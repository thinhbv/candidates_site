using System.Collections.Generic;

namespace CMSSolutions.DisplayManagement.Descriptors
{
    public class ShapeTable
    {
        public IDictionary<string, ShapeDescriptor> Descriptors { get; set; }

        public IDictionary<string, ShapeBinding> Bindings { get; set; }
    }
}