using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChineseSchool.Entities;

namespace ChineseSchool.Controllers
{
    public class curriculumController : Controller
    {
        private static ChineseSchoolEntities dbContext = new ChineseSchoolEntities();
        // GET: curriculum
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult programs()
        {
            return View();
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Invalid class Id");
            }
            Class cs = dbContext.Classes.AsNoTracking().FirstOrDefault(c=>c.ClassId == id.Value && c.ActiveFlg);
            if (cs==null)
            {
                return HttpNotFound("Class NOT Found");
            }

            return View(cs);
        }


    }
}