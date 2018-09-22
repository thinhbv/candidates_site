using System;

namespace CMSSolutions.ContentManagement.Messages
{
    public class MessageTemplate
    {
        public MessageTemplate(string name)
        {
            Name = name;
            Subject = name;
        }

        public Guid? OwnerId { get; set; }

        public string Name { get; private set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}