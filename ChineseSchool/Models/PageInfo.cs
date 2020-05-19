using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChineseSchool.Models
{
    public class PageInfo
    {
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public string Url { get; set; }
    }
}