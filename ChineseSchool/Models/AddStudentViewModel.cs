using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Entities;

namespace ChineseSchool.Models
{
    public class AddStudentViewModel
    {
        public List<StudentViewModel> Students { get; set; }

        public List<Class> Classes { get; set; }
        public List<EnrichmentClass> EnrichmentClasses { get; set; }
    }
}