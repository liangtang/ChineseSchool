using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChineseSchool.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.ComponentModel.DataAnnotations;
using ChineseSchool.Areas.Admin.Models;
using System.IO;

namespace ChineseSchool.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class EventController : Controller
    {
        private static ChineseSchoolEntities dbContext = new ChineseSchoolEntities();
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
        // GET: Admin/Event
        public ActionResult Index()
        {
            IEnumerable<Event> events = dbContext.Events.AsNoTracking();
            return View(events);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Event ev)
        {
            if (ModelState.IsValid)
            {
                ev.CreatedUser = User.Identity.GetUserName();
                ev.CreateTimeStamp = DateTime.Now;
                ev.UpdateTimeStamp = DateTime.Now;
                ev.UpdateUser = User.Identity.GetUserName();
                ev.Active = true;
                dbContext.Events.Add(ev);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Invalid event ID");
            }

            Event e = dbContext.Events.FirstOrDefault(v => v.EventId == id.Value);
            return View(e);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Invalid event ID");
            }

            Event e = dbContext.Events.FirstOrDefault(v => v.EventId == id.Value);
            foreach(var f in e.Images)
            {
                if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Content/eventimages"), f.ImagePath)))
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/eventimages"), f.ImagePath));
                }
                if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Content/eventimages"), f.ThumbNailPath)))
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/eventimages"), f.ThumbNailPath));
                }

            }

            dbContext.usp_DeleteEvent(id.Value);

            return RedirectToAction("Index");
        }


        public ActionResult UploadImages(int id)
        {
            ViewBag.eventId = id;
            return View();
        }

        public ActionResult UploadVideo(int id)
        {
            Video v = new Video()
            {
                EventId = id
            };
            ViewBag.eventId = id;
            return View(v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadVideo(Video v)
        {
            if(ModelState.IsValid)
            { 
                v.upsrt_dttm = DateTime.Now;
                v.upsrt_user = User.Identity.GetUserName();
                dbContext.Videos.Add(v);
                dbContext.SaveChanges();
                return RedirectToAction("Details", new { area = "Admin", controller = "Event", id = v.EventId });
            }
            return View();
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Event id is not valid!");
            }

            Event e = dbContext.Events.FirstOrDefault(ev => ev.EventId == id.Value);
            if (e==null)
            {
                return HttpNotFound("Event not found!");
            }
            ViewBag.currentTab = "Edit";
            return View(e);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Event e, int[] deleteimage)
        {
            if (ModelState.IsValid)
            {
                Event ev = dbContext.Events.FirstOrDefault(evt => evt.EventId == e.EventId);
                ev.EventName = e.EventName;
                ev.EventDate = e.EventDate;
                ev.EventDescription = e.EventDescription;
                ev.UpdateUser = User.Identity.GetUserName();
                ev.UpdateTimeStamp = DateTime.Now;
                dbContext.SaveChanges();
                foreach(int id in deleteimage)
                {
                    Image i = dbContext.Images.FirstOrDefault(im=>im.ImageId == id);
                    dbContext.Images.Remove(i);
                }
                dbContext.SaveChanges();

                return RedirectToAction("Details", new { id = e.EventId });

            }
            return View();
        }
        [HttpPost]
        public ActionResult UploadImages(int EventId, HttpPostedFileBase[] files )
        {
            if (ModelState.IsValid)
            {
                foreach(HttpPostedFileBase file in files)
                {
                    var InputFilename = Path.GetFileName(file.FileName);
                    var newFileName = InputFilename;
                    var path = (Server.MapPath("~/content/eventimages"));
                    System.Drawing.Image image = null;
                    System.Drawing.Image imageThum = null;
                    int index =1;
                    while (System.IO.File.Exists(Path.Combine(path,newFileName)))
                    {
                        newFileName = index.ToString() + "_" + InputFilename;
                        index++;
                    }

                    var ServerSavePath = Path.Combine(path, newFileName);
                    file.SaveAs(ServerSavePath);
                    image = System.Drawing.Image.FromFile(ServerSavePath);
                    imageThum = image.GetThumbnailImage(200, 200, null, new IntPtr());
                    imageThum.Save(Path.Combine(path, "thumbnail_" + newFileName));

                    Image i = new Image()
                    {
                        EventId = EventId,
                        ImagePath = InputFilename,
                        ThumbNailPath = "thumbnail_" + newFileName,
                        CreatedUser = User.Identity.GetUserName(),
                        CreateTimeStamp = DateTime.Now,
                        UpdateUser = User.Identity.GetUserName(),
                        UpdateTimeStamp = DateTime.Now
                    };
                    dbContext.Images.Add(i);
                    dbContext.SaveChanges();
                   
                }
                return RedirectToAction("Details", new {id = EventId });
            }
            ViewBag.eventId = EventId;
            FileModel vm = new FileModel()
            {
                files = files
            };
            return View();
        }
    }
}