using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ChineseSchool.Areas.Admin.Models;
using ChineseSchool.Models;


namespace ChineseSchool.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdminAccountController : Controller
    {

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
        // GET: /Admin/AdminAccount/
        public ActionResult Index(int page = 1)
        {
            UsersViewModel vm = new UsersViewModel();
            List<ApplicationUser> users = UserManager.Users.ToList();
            PageInfo pageInfo = new PageInfo() { PageSize = 20, CurrentPage = 1 };
            pageInfo.PageCount = (users.Count() - 1) / pageInfo.PageSize + 1;
            pageInfo.CurrentPage = page <= pageInfo.PageCount ? page : 1;
            pageInfo.Url = "/admin/adminaccount/index?page=";
            vm.Users = users.Skip((pageInfo.CurrentPage - 1) * pageInfo.PageSize).Take(pageInfo.PageSize).ToList();
            vm.PageInfo = pageInfo;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Message"] = "User Not  Found!";
                return RedirectToAction("index");
            }
            IdentityResult result = await UserManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Message"] = "User Deleted Successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error", result.Errors);
            }

        }

        public ActionResult Confirm(string id)
        {
            ApplicationUser user = UserManager.FindById(id);
            if (user == null)
            {
                return new HttpNotFoundResult("User Not Found!");
            }
            if (user.EmailConfirmed)
            {
                TempData["Message"] = "User Already Confirmed!";
                return RedirectToAction("Index");
            }
            string token = UserManager.GenerateEmailConfirmationToken(id);
            IdentityResult result;
            result = UserManager.ConfirmEmail(id, token);
            if (result.Succeeded)
            {
                TempData["Message"] = "User Successfully Confirmed!";
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error", result.Errors);
            }

        }
	}
}