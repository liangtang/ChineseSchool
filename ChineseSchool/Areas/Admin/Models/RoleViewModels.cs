using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Models;
using System.ComponentModel.DataAnnotations;

namespace ChineseSchool.Areas.Admin.Models
{
    public class RoleEditModel
    {
        public ApplicationRole Role { get; set; }
        public IEnumerable<ApplicationUser> Members { get; set; }
        public IEnumerable<ApplicationUser> NonMembers { get; set; }
    }

    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToRemove { get; set; }
    }
}