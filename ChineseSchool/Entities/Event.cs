//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChineseSchool.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Event
    {
        public Event()
        {
            this.Images = new HashSet<Image>();
            this.Videos = new HashSet<Video>();
        }
    
        public int EventId { get; set; }
        public string EventName { get; set; }
        public System.DateTime EventDate { get; set; }
        public string EventDescription { get; set; }
        public bool Active { get; set; }
        public string CreatedUser { get; set; }
        public Nullable<System.DateTime> CreateTimeStamp { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateTimeStamp { get; set; }
    
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
    }
}