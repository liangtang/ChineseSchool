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
    
    public partial class v_TeacherAssignment
    {
        public int TeacherId { get; set; }
        public string Name { get; set; }
        public Nullable<int> ClassId { get; set; }
        public string ClassName { get; set; }
        public Nullable<int> EnrichmentClassId { get; set; }
        public string EnrichmentClassName { get; set; }
    }
}
