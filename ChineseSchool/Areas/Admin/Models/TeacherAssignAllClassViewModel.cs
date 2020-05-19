using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Entities;

namespace ChineseSchool.Areas.Admin.Models
{
    public class TeacherAssignAllClassViewModel
    {
        public List<Class> Classes { get; set; }
        public IEnumerable<v_TeacherClass> Teachers { get; set; }
    }
}