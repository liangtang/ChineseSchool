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
using ChineseSchool.Areas.Admin.Models;
using System.Web.Security;
using ChineseSchool.Entities;
using System.Data.Entity;
using System.IO;
namespace ChineseSchool.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class SetupController : Controller
    {
        private static ChineseSchoolEntities dbContext = new ChineseSchoolEntities();
        //
        // GET: /Admin/Setup/
        public ActionResult Index()
        {
            SetupViewModel vm = new SetupViewModel();
            vm.Semester = dbContext.Semesters.AsNoTracking().FirstOrDefault(s => s.ActiveFlg);
            vm.Classes = dbContext.Classes.Where(c => c.ActiveFlg).ToList();
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.Where(e => e.ActiveFlg).ToList();
            return View(vm);
        }

        public ActionResult NewSemester()
        {
            return View(new Semester());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewSemester(Semester sm)
        {
            if(ModelState.IsValid)
            {
                sm.ActiveFlg = true;
                sm.CreateUserId = User.Identity.GetUserId();
                sm.CreateTimeStemp = DateTime.Now;
                sm.UpdateUserId = User.Identity.GetUserId();
                sm.UpdateTimeStemp = DateTime.Now;
                Semester oldSemester = dbContext.Semesters.AsNoTracking().FirstOrDefault(s => s.ActiveFlg);
                if (oldSemester != null)
                {
                    oldSemester.ActiveFlg = false;
                    dbContext.Entry(oldSemester).State = EntityState.Modified;
                }

                
                dbContext.Semesters.Add(sm);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult EditSemester(int? semesterId)
        {
            if (!semesterId.HasValue)
            {
                return HttpNotFound("Invalid semester id");
            }
            Semester s = dbContext.Semesters.FirstOrDefault(a => a.SemesterID == semesterId.Value);
            if(s==null)
            {
                return HttpNotFound("Invalid semester id");
            }
            return View(s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSemester(Semester s)
        {
            if(ModelState.IsValid)
            {
                
                var semester = dbContext.Semesters.FirstOrDefault(k => k.SemesterID == s.SemesterID);
                semester.SemesterYear = s.SemesterYear;
                semester.SemesterSeason = s.SemesterSeason;
                semester.Price1 = s.Price1;
                semester.Price2 = s.Price2;
                semester.Price3 = s.Price3;
                semester.Price4 = s.Price4;
                semester.Price5 = s.Price5;
                semester.Price1Description = s.Price1Description;
                semester.Price2Description = s.Price2Description;
                semester.Price3Description = s.Price3Description;
                semester.Price4Description = s.Price4Description;
                semester.Price5Description = s.Price5Description;
                semester.RegisterStartDate = s.RegisterStartDate;
                semester.RegisterEndDate = s.RegisterEndDate;
                semester.RegistrationFeeAfterEndDate = s.RegistrationFeeAfterEndDate;
                semester.RegistrationFeeBeforeEndDate = s.RegistrationFeeBeforeEndDate;
                semester.BookCharge = s.BookCharge;
                semester.VolunteerDeposit = s.VolunteerDeposit;
                semester.UpdateUserId = User.Identity.GetUserId();
                semester.UpdateTimeStemp = DateTime.Now;
                
                dbContext.SaveChanges();
                return RedirectToAction("index");
            }
            return View(s);
        }
	     public ActionResult EditClass(int? classId)
          {
             if(!classId.HasValue)
             {
                 return HttpNotFound("Invalid class id");
             }
             Class c = dbContext.Classes.FirstOrDefault(a => a.ClassId == classId.Value);
             if (c == null)
             {
                 return HttpNotFound("Invalid class id");
             }
              return View(c);
          }

        [HttpPost]
        [ValidateAntiForgeryToken]
         public ActionResult EditClass(Class c)
         {
            if(ModelState.IsValid)
            {
                Class cla = dbContext.Classes.FirstOrDefault(a => a.ClassId == c.ClassId);
                cla.Classname = c.Classname;
                cla.ClassDescription = c.ClassDescription;
                cla.Ability = c.Ability;
                cla.Content = c.Content;
                cla.target = c.target;
                cla.UpdateTimeStemp = DateTime.Now;
                cla.UpdateUserId = User.Identity.GetUserId();
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c);
         }

        public ActionResult ClassDetail(int? classId)
        {
            if(!classId.HasValue)
            {
                return HttpNotFound("Invalid Class Id");
            }
            ChineseSchool.Entities.Class c = dbContext.Classes.FirstOrDefault(cl => cl.ClassId == classId.Value && cl.ActiveFlg);
            if (c == null)
            {
                return HttpNotFound("Class not found");
            }

            return View(c);
        }

        public ActionResult AddClass()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddClass(Class c)
        {
            if (ModelState.IsValid)
            {
                c.ActiveFlg = true;
                c.CreateTimeStemp = DateTime.Now;
                c.UpdateTimeStemp = DateTime.Now;
                c.CreateUserId = User.Identity.GetUserId();
                c.UpdateUserId = User.Identity.GetUserId();
                dbContext.Classes.Add(c);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c);
        }

        public ActionResult DeleteClass(int? classId)
        {

            if (!classId.HasValue)
            {
                return HttpNotFound("Invalid class id");
            }
            Class c = dbContext.Classes.FirstOrDefault(a => a.ClassId == classId.Value);
            if (c == null)
            {
                return HttpNotFound("Invalid class id");
            }
            c.ActiveFlg = false;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult EditEnrichmentClass(int? classId)
        {
            if (!classId.HasValue)
            {
                return HttpNotFound("Invalid class id");
            }
            EnrichmentClass c = dbContext.EnrichmentClasses.FirstOrDefault(a => a.ClassID == classId.Value);
            if (c == null)
            {
                return HttpNotFound("Invalid class id");
            }
            return View(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEnrichmentClass(EnrichmentClass e)
        {
            if(ModelState.IsValid)
            {
                EnrichmentClass en = dbContext.EnrichmentClasses.FirstOrDefault(a => a.ClassID == e.ClassID);
                en.ClassName = e.ClassName;
                en.Price1 = e.Price1;
                en.Price1Description = e.Price1Description;
                en.Price2 = e.Price2;
                en.Price2Descrioption = e.Price2Descrioption;
                en.UpdateUserId = User.Identity.GetUserId();
                en.UpdateTimeStemp = DateTime.Now;
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(e);
        }

        public ActionResult AddEnrichmentClass()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEnrichmentClass(EnrichmentClass e)
        {
            if (ModelState.IsValid)
            {
                e.ActiveFlg = true;
                e.CreateTimeStemp = DateTime.Now;
                e.UpdateTimeStemp = DateTime.Now;
                e.CreateUserId = User.Identity.GetUserId();
                e.UpdateUserId = User.Identity.GetUserId();
                dbContext.EnrichmentClasses.Add(e);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult LoadClassImage(int? classId)
        {
            if (!classId.HasValue)
            {
                return HttpNotFound("Invalid class id");
            }

            Class c = dbContext.Classes.FirstOrDefault(cl=>cl.ClassId==classId.Value);
            if (c==null)
            {
                return HttpNotFound("Class not found");
            }
            return View(classId);
        }

        [HttpPost]
        public ActionResult LoadClassImage(HttpPostedFileBase file, int classId)
        {
            Class c = dbContext.Classes.FirstOrDefault(cl => cl.ClassId == classId && cl.ActiveFlg);
            if (c==null)
            {
                return HttpNotFound("Class not found");
            }

            if (ModelState.IsValid)
            {
                var fileName = Path.GetFileName(file.FileName);
                var newFileName = fileName;
                var path = Server.MapPath("~/Content/image");
                int index = 1;
                while (System.IO.File.Exists(Path.Combine(path,newFileName)))
                {
                    newFileName = index.ToString() + "_" + fileName;
                    index++;
                }
                file.SaveAs(Path.Combine(path, newFileName));
                c.imagePath = newFileName;
                c.UpdateTimeStemp = DateTime.Now;
                c.UpdateUserId = User.Identity.GetUserId();
                dbContext.SaveChanges();
                return RedirectToAction("ClassDetail",new {classId=classId});
            }
            return View(classId);
        }

        public ActionResult DeleteEnrichmentClass(int? classId)
        {
            if (!classId.HasValue)
            {
                return HttpNotFound("Invalid class id");
            }
            EnrichmentClass c = dbContext.EnrichmentClasses.FirstOrDefault(a => a.ClassID == classId.Value);
            if (c == null)
            {
                return HttpNotFound("Invalid class id");
            }
            c.ActiveFlg = false;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }

   
}