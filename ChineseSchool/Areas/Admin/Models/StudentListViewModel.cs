using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChineseSchool.Areas.Admin.Models
{
    public class StudentListViewModel
    {
        public int StudentId { get; set; }
        public int ParentsId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string MiddleInitial { get; set; }
        public string Chinesename { get; set; }
        public string Gender { get; set; }
    
        public string Birthday { get; set; }
        public string Class { get; set; }


        public string RegisterClass { get; set; }
        public string EnrichmentClass { get; set; }
        public string Deleted { get; set; }
    }
}