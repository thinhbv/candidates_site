using System;
using CMSSolutions.ContentManagement.Messages.Domain;
using CMSSolutions.Web.Mvc;

namespace CMSSolutions.ContentManagement.Messages.Models
{
    public class QueuedSmsModel : BaseModel<Guid>
    {
        public static implicit operator QueuedSmsModel(QueuedSms queuedSms)
        {
            return new QueuedSmsModel();
        }
    }
}