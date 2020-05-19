﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Models;
using ChineseSchool.Entities;

namespace ChineseSchool.Areas.Admin.Models
{
    public class StudentByEnrichmentClassViewModel
    {
        public IEnumerable<EnrichmentClass> EnrichmentClasses { get; set; }
        public IEnumerable<v_StudentEnrichmentClass> Students { get; set; }
        public int selected { get; set; }

        public string selectedName { get; set; }
    }
}