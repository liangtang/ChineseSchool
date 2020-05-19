using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChineseSchool.Entities
{
    [MetadataTypeAttribute(typeof(Teacher.TeacherMetadata))]
    public partial class Teacher
    {
        internal sealed class TeacherMetadata
        {
            [Required]
            public string Name { get; set; }
        }
    }
}