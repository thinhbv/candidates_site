using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;

namespace CMSSolutions.ContentManagement.Pages.Domain
{
    [DataContract]
    public class PageTemplate : BaseEntity<Guid>
    {
        [DataMember]
        [DisplayName("Content")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("HtmlContent")]
        public string HtmlContent { get; set; }
    }
}