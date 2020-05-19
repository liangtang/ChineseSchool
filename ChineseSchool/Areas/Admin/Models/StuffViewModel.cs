using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Entities;

namespace ChineseSchool.Areas.Admin.Models
{
    public class StuffViewModel
    {
        public Stuff Stuff { get; set; }
        public List<Position> Positions { get; set; }
    }
}