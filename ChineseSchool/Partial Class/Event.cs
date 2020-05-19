using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChineseSchool.Entities
{
    [MetadataTypeAttribute(typeof(Event.EventMetadata))]
    public partial class Event
    {
        internal sealed class EventMetadata
        {

            public int EventId { get; set; }
            [Required]
            public string EventName { get; set; }
            [Required]
            public System.DateTime EventDate { get; set; }
            public string EventDescription { get; set; }
            [UIHint("IsActive")]
            public bool Active { get; set; }
        }
    }
}