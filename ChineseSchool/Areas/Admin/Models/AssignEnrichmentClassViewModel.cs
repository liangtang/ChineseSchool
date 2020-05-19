using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Models;
using ChineseSchool.Entities;

namespace ChineseSchool.Areas.Admin.Models
{
    public class AssignEnrichmentClassViewModel
    {
        public Student Student { get; set; }
        public int AssignedEnrichmentClassId { get; set; }
        public List<EnrichmentClass> EnrichmentClasses { get; set; }
    }
}