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
    
    public partial class Transaction
    {
        public int TransactionID { get; set; }
        public string UserID { get; set; }
        public int ParentsId { get; set; }
        public string TransactionType { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public string TransactionDescription { get; set; }
        public decimal TransactionAmount { get; set; }
        public bool ActiveFlg { get; set; }
        public System.DateTime CreateTimeStemp { get; set; }
        public string CreateUserId { get; set; }
        public System.DateTime UpdateTimeStemp { get; set; }
        public string UpdateUserId { get; set; }
        public int SemesterID { get; set; }
    
        public virtual Parent Parent { get; set; }
        public virtual Semester Semester { get; set; }
    }
}
