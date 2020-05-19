using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChineseSchool.Entities
{
    [MetadataTypeAttribute(typeof(Transaction.TransactionMetadata))]
    public partial class Transaction
    {
        internal sealed class TransactionMetadata
        {
            public int TransactionID { get; set; }
            [Required]
            public string UserID { get; set; }
            public int ParentsId { get; set; }
            [Required]

            public string TransactionType { get; set; }
            [Required]           
            [DataType(DataType.Date)]
            public System.DateTime TransactionDate { get; set; }
            public string TransactionDescription { get; set; }
            public decimal TransactionAmount { get; set; }
            public bool ActiveFlg { get; set; }
            public System.DateTime CreateTimeStemp { get; set; }
            public string CreateUserId { get; set; }
            public System.DateTime UpdateTimeStemp { get; set; }
            public string UpdateUserId { get; set; }
            public int SemesterID { get; set; }


        }
    }
}