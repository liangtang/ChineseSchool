using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ChineseSchool.Models;
using System.Web.Security;
using ChineseSchool.Entities;

namespace ChineseSchool.Controllers
{
    public class HomeController : Controller
    {
        private static ChineseSchoolEntities dbContext = new ChineseSchoolEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Mission()
        {
            return View();
        }

        public ActionResult Seal()
        {
            return View();
        }

        public ActionResult Enrichment()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult StuffList()
        {
            StuffListViewModel vm = new StuffListViewModel()
            {
                Principle = dbContext.Stuffs.FirstOrDefault(p => p.Position.PositionName == "Principal" && p.IsActive),
                VicePrinciples = dbContext.Stuffs.Where(p => p.Position.PositionName == "Vice Principal" && p.IsActive).ToList(),
                Administratives = dbContext.Stuffs.Where(p => p.Position.PositionName != "Principal" && p.Position.PositionName != "Vice Principal" && p.IsActive && p.Position.PositionType.PositionTypeName=="Administrative").ToList(),
                BoardChair = dbContext.Stuffs.FirstOrDefault(p => p.Position.PositionName == "Board Chair" && p.IsActive),
                BoardMembers = dbContext.Stuffs.Where(p => p.Position.PositionName == "Board Member" && p.IsActive).ToList(),
                Teachers = dbContext.Teachers.AsNoTracking().Where(t=>t.IsActive).ToList()

            };
            return View(vm);
        }

        public ActionResult Register()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //return RedirectToAction()
            }
            return View();
        }
    }
}