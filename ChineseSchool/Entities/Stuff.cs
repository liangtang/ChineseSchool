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
    
    public partial class Stuff
    {
        public int StuffId { get; set; }
        public string Name { get; set; }
        public int PositionId { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string ThumbNailPath { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime upsrt_dttm { get; set; }
        public string upsrt_user { get; set; }
    
        public virtual Position Position { get; set; }
    }
}