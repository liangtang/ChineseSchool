using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Models;
using ChineseSchool.Entities;
namespace ChineseSchool.Areas.Admin.Models
{
    public class SetupViewModel
    {
        public Semester Semester { get; set; }
        public List<Class> Classes { get; set; }
        public List<EnrichmentClass> EnrichmentClasses { get; set; }
    }
}