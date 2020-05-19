using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Models;
using ChineseSchool.Entities;

namespace ChineseSchool.Areas.Admin.Models
{
    public class StudentDetailViewModel
    {
        public Parent Parent { get; set; }

        public Student Student { get; set; }

        public string Semester { get; set; }
        public string IfRegistered { get; set; }
        public string ClassAssigned { get; set; }
        public string EnrichmentmentClass { get; set; }
    }
}