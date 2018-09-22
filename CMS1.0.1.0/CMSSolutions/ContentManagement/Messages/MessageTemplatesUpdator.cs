using System.Collections.Generic;
using System.Linq;
using Autofac;
using CMSSolutions.ContentManagement.Messages.Services;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Messages
{
    [Feature(Constants.Areas.Messages)]
    public class MessageTemplatesUpdator : IShellEvents
    {
        private readonly IComponentContext componentContext;

        public MessageTemplatesUpdator(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        public int Priority { get { return 0; } }

        public void Activated()
        {
            var providers = componentContext.Resolve<IEnumerable<IMessageTemplatesProvider>>();
            var templateService = componentContext.Resolve<IMessageTemplateService>();

            var templates = templateService.GetRecords();
            foreach (var provider in providers)
            {
                foreach (var template in provider.GetTemplates())
                {
                    if (templates.FirstOrDefault(x => x.Name == template.Name && x.OwnerId == template.OwnerId) == null)
                    {
                        var newTemplate = new Domain.MessageTemplate
                        {
                            Name = template.Name,
                            OwnerId = template.OwnerId,
                            Subject = template.Subject,
                            Body = template.Body,
                            Enabled = true
                        };
                        templateService.Insert(newTemplate);
                    }
                }
            }
        }

        public void Terminating()
        {
        }
    }
}