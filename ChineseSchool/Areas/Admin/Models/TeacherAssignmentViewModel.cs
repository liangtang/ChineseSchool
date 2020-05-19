using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Entities;

namespace ChineseSchool.Areas.Admin.Models
{
    public class TeacherAssignmentViewModel
    {
        public List<v_TeacherAssignment> Assignments { get; set; }
        public List<Class> Classes { get; set; }
        public List<EnrichmentClass> EnrichmentClasses { get; set; }
    }
}