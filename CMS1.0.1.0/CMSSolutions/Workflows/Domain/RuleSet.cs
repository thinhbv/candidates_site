using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;

namespace CMSSolutions.Workflows.Domain
{
    [DataContract]
    public class RuleSet : BaseEntity<Guid>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }
    }
}