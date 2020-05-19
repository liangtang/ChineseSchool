using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Models;
using ChineseSchool.Entities;

namespace ChineseSchool.Areas.Admin.Models
{
    public class StudentByClassViewModel
    {
        public IEnumerable<Class> Classes { get; set; }
        public IEnumerable<v_StudentClassDetails> Students { get; set; }
        public int selected { get; set; }

        public string selectedName { get; set; }
    }
}