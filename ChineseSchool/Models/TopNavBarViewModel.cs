using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Entities;

namespace ChineseSchool.Models
{
    public class TopNavBarViewModel
    {
        public IEnumerable<Class> classes;
        public IEnumerable<EnrichmentClass> eClasses;
        public IEnumerable<Event> events;
    }
}