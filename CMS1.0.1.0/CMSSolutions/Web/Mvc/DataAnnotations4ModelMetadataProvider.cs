using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Autofac;
using CMSSolutions.Localization;
using IContainer = Autofac.IContainer;

namespace CMSSolutions.Web.Mvc
{
    public class DataAnnotations4ModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        private readonly IContainer container;
        private static readonly string localizerScope = typeof(DataAnnotations4ModelMetadataProvider).FullName;

        public DataAnnotations4ModelMetadataProvider(IContainer container)
        {
            this.container = container;
        }

        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var allAttributes = attributes.ToList();

            var metadata = base.CreateMetadata(allAttributes, containerType, modelAccessor, modelType, propertyName);

            var display = allAttributes.OfType<DisplayAttribute>().FirstOrDefault();
            if (display != null)
            {
                var localizer = GetLocalizer();

                if (!string.IsNullOrEmpty(display.Name))
                {
                    metadata.DisplayName = localizer(display.Name).Text;
                }

                if (!string.IsNullOrEmpty(display.Description))
                {
                    metadata.Description = localizer(display.Description).Text;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(propertyName))
                {
                    var localizer = GetLocalizer();
                    metadata.DisplayName = localizer(propertyName).Text;
                }
            }

            return metadata;
        }

        private Localizer GetLocalizer()
        {
            var workContextAccessor = container.Resolve<IWorkContextAccessor>();
            var workContext = workContextAccessor.GetContext();
            return LocalizationUtilities.Resolve(workContext, localizerScope);
        }
    }
}