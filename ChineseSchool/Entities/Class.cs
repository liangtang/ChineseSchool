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
    
    public partial class Class
    {
        public Class()
        {
            this.ClassTeacherAssignments = new HashSet<ClassTeacherAssignment>();
            this.ClassRegistrations = new HashSet<ClassRegistration>();
        }
    
        public int ClassId { get; set; }
        public string Classname { get; set; }
        public string ClassDescription { get; set; }
        public bool ActiveFlg { get; set; }
        public System.DateTime CreateTimeStemp { get; set; }
        public string CreateUserId { get; set; }
        public System.DateTime UpdateTimeStemp { get; set; }
        public string UpdateUserId { get; set; }
        public string target { get; set; }
        public string Content { get; set; }
        public string Ability { get; set; }
        public string imagePath { get; set; }
    
        public virtual ICollection<ClassTeacherAssignment> ClassTeacherAssignments { get; set; }
        public virtual ICollection<ClassRegistration> ClassRegistrations { get; set; }
    }
}