using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Models;
using ChineseSchool.Entities;

namespace ChineseSchool.Areas.Admin.Models
{
    public class AssignClassViewModel
    {
        public Student Student { get; set; }
        public int AssignedClassId { get; set; }
        public List<Class> Classes { get; set; }
    }

}