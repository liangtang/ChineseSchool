using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChineseSchool.Entities;
using ChineseSchool.Models;

namespace ChineseSchool.Controllers
{
    public class NavController : Controller
    {
        ChineseSchoolEntities dbContext = new ChineseSchoolEntities();
        public ActionResult GenerateTopNavBar()
        {
            TopNavBarViewModel vm = new TopNavBarViewModel();
            vm.classes = dbContext.Classes.AsNoTracking().Where(c=>c.ActiveFlg);
            vm.eClasses = dbContext.EnrichmentClasses.AsNoTracking().Where(c => c.ActiveFlg);
            vm.events = dbContext.Events.AsNoTracking().Where(e => e.Active);
            return View(vm);
        }
    }
}