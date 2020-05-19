using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChineseSchool.Entities;

namespace ChineseSchool.Controllers
{
    public class ShowEventController : Controller
    {
        private static ChineseSchoolEntities dbContext = new ChineseSchoolEntities();
        // GET: Event
        public ActionResult Event(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Invalid event!");
            }
            Event e = dbContext.Events.FirstOrDefault(ev=>ev.EventId == id.Value);
            return View(e);
        }
    }
}