using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Models;
namespace ChineseSchool.Areas.Admin.Models
{

        public class UsersViewModel
        {
            public List<ApplicationUser> Users { set; get; }
            public PageInfo PageInfo { get; set; }
        }
   
}