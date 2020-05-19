﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Models;
using ChineseSchool.Entities;
namespace ChineseSchool.Models
{
    public class DashboardViewModel
    {
        public ApplicationUser User { get; set; }
        public Parent   Parent { get; set; }
        public IEnumerable<Student> Students { get; set; }
        public decimal AmountDue { get; set; }
    }
}