using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChineseSchool.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ChineseSchool.Areas.Admin.Models;
using System.IO;
namespace ChineseSchool.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class StuffController : Controller
    {
        private static ChineseSchoolEntities dbContextStatic = new ChineseSchoolEntities();
        private int currentSemesterId = dbContextStatic.Semesters.FirstOrDefault(s => s.ActiveFlg).SemesterID;
        private ChineseSchoolEntities dbContext = new ChineseSchoolEntities();
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
        // GET: /Admin/Stuff/
        public ActionResult Index()
        {
            IEnumerable<Stuff> stuffs = dbContext.Stuffs.AsNoTracking().Where(s=>s.IsActive);
            return View(stuffs);
        }

        public ActionResult Create()
        {
            StuffViewModel vm = new StuffViewModel()
            {
                Stuff = new Stuff(),
                Positions = dbContext.Positions.Where(p=>p.IsActive).ToList(),
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StuffViewModel model)
        {
            CheckExclusivePosition(model);
            if(ModelState.IsValid)
            {
                model.Stuff.upsrt_dttm = DateTime.Now;
                model.Stuff.upsrt_user = User.Identity.GetUserName();
                model.Stuff.IsActive = true;
                dbContext.Stuffs.Add(model.Stuff);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            model.Positions = dbContext.Positions.Where(p => p.IsActive).ToList();
            return View();
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Invalid Stuff ID");
            }
            Stuff stuff = dbContext.Stuffs.FirstOrDefault(s => s.StuffId == id.Value);
            if (stuff==null)
            {
                return HttpNotFound("Stuff Id NOT Found");
            }
            StuffViewModel vm = new StuffViewModel()
            {
                Stuff = stuff,
                Positions = dbContext.Positions.Where(p => p.IsActive).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StuffViewModel model)
        {   
            CheckExclusivePosition(model);
            if (ModelState.IsValid)
            {
                Stuff stuff = dbContext.Stuffs.FirstOrDefault(s => s.StuffId == model.Stuff.StuffId && s.IsActive);
                if (stuff == null)
                {
                    return HttpNotFound("Stuff NOT Found");
                }
                stuff.Name = model.Stuff.Name;
                stuff.PositionId = model.Stuff.PositionId;
                stuff.Description = model.Stuff.Description;
                stuff.upsrt_user = User.Identity.GetUserName();
                stuff.upsrt_dttm = DateTime.Now;
                dbContext.SaveChanges();
                return RedirectToAction("Details", new { area = "Admin", controller = "Stuff", id = model.Stuff.StuffId });
            }
            StuffViewModel vm = new StuffViewModel()
            {
                 
                Stuff = dbContext.Stuffs.FirstOrDefault(s => s.StuffId == model.Stuff.StuffId && s.IsActive),
                Positions = dbContext.Positions.Where(p => p.IsActive).ToList()
            };
            return View(vm);
            
        }

        //public ActionResult Delete(int? id)
        //{
        //    if (!id.HasValue)
        //    {
        //        return HttpNotFound("Invalid Stuff ID");
        //    }
        //    Stuff stuff = dbContext.Stuffs.FirstOrDefault(s => s.StuffId == id.Value);
        //    if (stuff == null)
        //    {
        //        return HttpNotFound("Stuff Id NOT Found");
        //    }
        //    return View(stuff);
        //}

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Invalid Stuff ID");
            }
            Stuff stuff = dbContext.Stuffs.FirstOrDefault(s => s.StuffId == id.Value);
            if (stuff == null)
            {
                return HttpNotFound("Stuff Id NOT Found");
            }
            stuff.IsActive = false;
            stuff.upsrt_dttm = DateTime.Now;
            stuff.upsrt_user = User.Identity.GetUserName();
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddPhoto(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Invalid Stuff ID");
            }
            Stuff stuff = dbContext.Stuffs.FirstOrDefault(s => s.StuffId == id.Value);
            if (stuff == null)
            {
                return HttpNotFound("Stuff Id NOT Found");
            }
            ViewBag.currentTab = "AddPhoto";
            return View(id.Value);
        }

        [HttpPost]
        public ActionResult AddPhoto(HttpPostedFileBase file, int StuffId)
        {
            Stuff s = dbContext.Stuffs.FirstOrDefault(st => st.StuffId == StuffId && st.IsActive);
            if (s==null)
            {
                return HttpNotFound("Stuff NOT Found");
            }
            if (ModelState.IsValid)
            {
                var filename = Path.GetFileName(file.FileName);
                var newFilename = filename;
                var path = Server.MapPath("~/Content/stuffimages");
                var index = 1;
                while (System.IO.File.Exists(Path.Combine(path, newFilename)))
                {
                    newFilename = "Teacher_" + index.ToString() + "_" + newFilename;
                    index++;
                }
                var serverPath = Path.Combine(path, newFilename);
                file.SaveAs(serverPath);
                System.Drawing.Image i = null;
                System.Drawing.Image thum = null;
                i = System.Drawing.Image.FromFile(serverPath);
                thum = i.GetThumbnailImage(300, 300, null, new IntPtr());
                thum.Save(Path.Combine(path, "thumbnail_" + newFilename));
                s.ImagePath = newFilename;
                s.ThumbNailPath = "thumbnail_" + newFilename;
                s.upsrt_user = User.Identity.GetUserName();
                s.upsrt_dttm = DateTime.Now;
                dbContext.SaveChanges();
                return RedirectToAction("Details", new { area = "Admin", controller = "Stuff", id = s.StuffId });
            }
            return View(StuffId);
        }

        public ActionResult Details(int? id)
        {

            if (!id.HasValue)
            {
                return HttpNotFound("Invalid Stuff ID");
            }
            Stuff stuff = dbContext.Stuffs.FirstOrDefault(s => s.StuffId == id.Value);
            if (stuff == null)
            {
                return HttpNotFound("Stuff Id NOT Found");
            }
            ViewBag.currentTab = "Details";
            return View(stuff);
        }

        private void CheckExclusivePosition(StuffViewModel stuff)
        {
            Position pos = dbContext.Positions.FirstOrDefault(p => p.PositionID == stuff.Stuff.PositionId && p.IsActive);
            if (!pos.IsExclusive)
            {
               return;
            }
            Stuff existingStuff = dbContext.Stuffs.FirstOrDefault(s => s.PositionId == stuff.Stuff.PositionId && s.IsActive);
            if (existingStuff != null )
            {
                ModelState.AddModelError("Stuff.PositionID", "A Person with this position already exists, please delete or demote first");
                
            }
         
        }
	}
}