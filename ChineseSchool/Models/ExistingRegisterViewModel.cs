using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Entities;
using System.ComponentModel.DataAnnotations;

namespace ChineseSchool.Models
{
    public class ExistingRegisterViewModel
    {
        public int ParentId { get; set; }
        public List<ExistingStudentViewModel> Students { get; set; }
        public List<Class> Classes { get; set; }
        public List<EnrichmentClass> EnrichmentClasses { get; set; }
    }


    public class ExistingStudentViewModel
    {
        [Required]
        public int StudentId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string Chinesename { get; set; }
        public bool IfRegister { get; set; }

        public int? Class { get; set; }

        public string PreviousClass { get; set; }

        public string PreviousEnrichmentClass { get; set; }
        public int EnrichmentClass { get; set; }
    }
}