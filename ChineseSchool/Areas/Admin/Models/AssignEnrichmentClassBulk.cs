using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Models;
using ChineseSchool.Entities;

namespace ChineseSchool.Areas.Admin.Models
{
    public class StudentByEnrichmentClassBulkViewModel
    {
        public List<EnrichmentClass> Classes { get; set; }
        public IEnumerable<v_StudentEnrichmentClass> Students { get; set; }
        public int selected { get; set; }

        public string selectedName { get; set; }
    }
}