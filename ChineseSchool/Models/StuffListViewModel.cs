using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Entities;

namespace ChineseSchool.Models
{
    public class StuffListViewModel
    {
        public Stuff Principle { get; set; }
        public List<Stuff> VicePrinciples { get; set; }
        public List<Stuff> Administratives { get; set; }
        public Stuff BoardChair { get; set; }
        public List<Stuff> BoardMembers { get; set; }
        public List<Teacher> Teachers { get; set; }
    }
}