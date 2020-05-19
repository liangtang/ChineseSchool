using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChineseSchool.Entities
{
    [MetadataTypeAttribute(typeof(Class.ClassMetadata))]
    public partial class Class
    {
        internal sealed class ClassMetadata
        {
            public int ClassId { get; set; }
            [Required]
            public string Classname { get; set; }
            public string ClassDescription { get; set; }
            public string ActiveFlg { get; set; }
            public System.DateTime CreateTimeStemp { get; set; }
            public string CreateUserId { get; set; }
            public System.DateTime UpdateTimeStemp { get; set; }
            public string UpdateUserId { get; set; }
        }
    }
}