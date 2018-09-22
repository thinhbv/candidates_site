using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;

namespace CMSSolutions.Workflows.Domain
{
    [DataContract]
    public class Rule : BaseEntity<Guid>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("Priority")]
        public int Priority { get; set; }

        [DataMember]
        [DisplayName("Reevaluation")]
        public bool Reevaluation { get; set; }

        [DataMember]
        [DisplayName("Active")]
        public bool Active { get; set; }

        [DataMember]
        [DisplayName("Condition")]
        public string Condition { get; set; }

        [DataMember]
        [DisplayName("ThenActions")]
        public string ThenActions { get; set; }

        [DataMember]
        [DisplayName("ElseActions")]
        public string ElseActions { get; set; }
    }
}