using System;
using CMSSolutions.ContentManagement.Messages.Domain;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Messages.Models
{
    public class QueuedEmailModel : BaseModel<Guid>
    {
        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the From property
        /// </summary>
        [ControlLabel]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the FromName property
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Gets or sets the To property
        /// </summary>
        [ControlLabel]
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the ToName property
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// Gets or sets the CC
        /// </summary>
        public string Cc { get; set; }

        /// <summary>
        /// Gets or sets the Bcc
        /// </summary>
        public string Bcc { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        [ControlLabel]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the date and time of item creation in UTC
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime? SentOnUtc { get; set; }

        public static implicit operator QueuedEmailModel(QueuedEmail queuedEmail)
        {
            return new QueuedEmailModel
                       {
                           Id = queuedEmail.Id,
                           Priority = queuedEmail.Priority,
                           From = queuedEmail.FromAddress,
                           FromName = queuedEmail.FromName,
                           To = queuedEmail.ToAddress,
                           ToName = queuedEmail.ToName,
                           Subject = queuedEmail.Subject,
                           CreatedOnUtc = queuedEmail.CreatedOnUtc,
                           SentTries = queuedEmail.SentTries,
                           SentOnUtc = queuedEmail.SentOnUtc
                       };
        }
    }
}