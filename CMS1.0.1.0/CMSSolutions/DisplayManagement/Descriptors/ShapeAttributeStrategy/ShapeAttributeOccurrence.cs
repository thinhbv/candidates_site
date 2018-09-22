using System;
using System.Reflection;
using Autofac.Core;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.DisplayManagement.Descriptors.ShapeAttributeStrategy
{
    public class ShapeAttributeOccurrence
    {
        private readonly Func<Feature> _feature;

        public ShapeAttributeOccurrence(ShapeAttribute shapeAttribute, MethodInfo methodInfo, IComponentRegistration registration, Func<Feature> feature)
        {
            ShapeAttribute = shapeAttribute;
            MethodInfo = methodInfo;
            Registration = registration;
            _feature = feature;
        }

        public ShapeAttribute ShapeAttribute { get; private set; }

        public MethodInfo MethodInfo { get; private set; }

        public IComponentRegistration Registration { get; private set; }

        public Feature Feature { get { return _feature(); } }
    }
}