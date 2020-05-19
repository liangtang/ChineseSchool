using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ChineseSchool.Models;
using ChineseSchool.Areas.Admin.Models;


namespace ChineseSchool.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class RoleController : Controller
    {
        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        //
        // GET: /Admin/Role/
        public ActionResult Index()
        {
            List<ApplicationRole> roles = new List<ApplicationRole>();
            if (RoleManager.Roles != null)
            {
                roles = RoleManager.Roles.ToList();
            }
            return View(roles);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await RoleManager.CreateAsync(new ApplicationRole(name));
                if (result.Succeeded)
                {
                    TempData["Message"] = "Role: " + name + " Created Successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return new HttpNotFoundResult("Role not found!");
            }
            IdentityResult result = await RoleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["Message"] = "Role: " + role.Name + " deleted Successfully!";
                return RedirectToAction("Index", new {Controller = "Role", Area="Admin" });
            }
            else
            {
                return View("Error", result.Errors);
            }

        }

        public async Task<ActionResult> Edit(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return new HttpNotFoundResult("Role Not Found!");
            }
            string[] memberIds = role.Users.Select(u => u.UserId).ToArray();
            IEnumerable<ApplicationUser> members = UserManager.Users.Where(x => memberIds.Any(m => m == x.Id));
            IEnumerable<ApplicationUser> nonMembers = UserManager.Users.Except(members);
            return View(new RoleEditModel()
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RoleModificationModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    result = await UserManager.AddToRoleAsync(userId, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                foreach (string userId in model.IdsToRemove ?? new string[] { })
                {
                    result = await UserManager.RemoveFromRoleAsync(userId, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("error", result.Errors);
                    }
                }
                return RedirectToAction("Index");
            }
            return View("Error", new string[] { "Role Not Found!" });
        }
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
	}
}