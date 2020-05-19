using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChineseSchool.Entities;
using ChineseSchool.Areas.Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.IO;
using System.Drawing;

namespace ChineseSchool.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class TeacherController : Controller
    {
        
        private static ChineseSchoolEntities dbContext = new ChineseSchoolEntities();
        private int currentSemesterId = dbContext.Semesters.FirstOrDefault(s => s.ActiveFlg).SemesterID;
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
        // GET: /Admin/Teacher/
        public ActionResult Index()
        {
            IEnumerable<Teacher> teachers = dbContext.Teachers.AsNoTracking().Where(t=>t.IsActive);
            return View(teachers);
        }

        public ActionResult Create()
        {
            Teacher t = new Teacher();
            return View(t);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Teacher t)
        {
            if (ModelState.IsValid)
            {
                t.IsActive = true;
                t.upsrt_dttm = DateTime.Now;
                t.upsrt_user = User.Identity.GetUserName();
                t.IsActive = true;
                dbContext.Teachers.Add(t);
                dbContext.SaveChanges();
                return RedirectToAction("Index", new { area = "Admin", controller = "Teacher" });
            }
            return View();
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Invalid Teacher Id");
            }
            Teacher t = dbContext.Teachers.AsNoTracking().FirstOrDefault(th => th.TeacherId == id.Value && th.IsActive);
            if (t==null)
            {
                return HttpNotFound("Teach NOT Found");
            }
            return View(t);
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Invalid Teacher Id");
            }
            Teacher t = dbContext.Teachers.FirstOrDefault(th => th.TeacherId == id.Value && th.IsActive);
            if (t == null)
            {
                return HttpNotFound("Teach NOT Found");
            }
            ViewBag.currentTab = "Edit";
            return View(t);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Teacher t)
        {
            if (ModelState.IsValid)
            {
                Teacher teacher = dbContext.Teachers.FirstOrDefault(tt => tt.TeacherId == t.TeacherId);
                teacher.Name = t.Name;
                teacher.Comment = t.Comment;
                teacher.upsrt_dttm = DateTime.Now;
                teacher.upsrt_user = User.Identity.GetUserName();
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult AddPhoto(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Invalid Teacher Id");
            }
            Teacher t = dbContext.Teachers.FirstOrDefault(th => th.TeacherId == id.Value && th.IsActive);
            if (t == null)
            {
                return HttpNotFound("Teach NOT Found");
            }
            ViewBag.currentTab = "AddPhoto";
            return View(t.TeacherId);
        }

        [HttpPost]
        public ActionResult AddPhoto(HttpPostedFileBase file, int TeacherId)
        {
            Teacher t = dbContext.Teachers.FirstOrDefault(tt => tt.TeacherId == TeacherId);
            if (t==null)
            {
                return HttpNotFound("Teacher NOT found");
            }
            if(ModelState.IsValid)
            {
                var filename = Path.GetFileName(file.FileName);
                var newFilename = filename;
                var path = Server.MapPath("~/Content/stuffimages");
                var index = 1;
                while (System.IO.File.Exists(Path.Combine(path,newFilename)))
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
                t.ImagePath = newFilename;
                t.ThumbNailPath = "thumbnail_" + newFilename;
                t.upsrt_user = User.Identity.GetUserName();
                t.upsrt_dttm = DateTime.Now;
                dbContext.SaveChanges();
                return RedirectToAction("Details", new { area = "Admin", controller = "Teacher", id = t.TeacherId });
            }
            return View();
        }
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound("Invalid Teacher Id");
            }
            Teacher t = dbContext.Teachers.FirstOrDefault(th => th.TeacherId == id.Value && th.IsActive);
            if (t == null)
            {
                return HttpNotFound("Teach NOT Found");
            }
            t.IsActive = false;
            t.upsrt_user = User.Identity.GetUserName();
            t.upsrt_dttm = DateTime.Now;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AssignClass(int? id)
        {if (!id.HasValue)
            {
                return HttpNotFound("Invalid Teacher Id");
            }
            Teacher t = dbContext.Teachers.AsNoTracking().FirstOrDefault(th => th.TeacherId == id.Value && th.IsActive);
            if (t == null)
            {
                return HttpNotFound("Teach NOT Found");
            }
            TeacherAssignClassViewModel vm = new TeacherAssignClassViewModel();
            vm.Teacher = t;
            vm.Classes = dbContext.Classes.AsNoTracking().Where(c => c.ActiveFlg).ToList();
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.AsNoTracking().Where(c => c.ActiveFlg).ToList();
            if (t.ClassTeacherAssignments != null && t.ClassTeacherAssignments.Count !=0)
            {
                vm.AssignedClassId = t.ClassTeacherAssignments.First().ClassId;
            }
            else
            {
                vm.AssignedClassId = 0;
            }
            if (t.EnrichmentClassTeacherAssignments.Where(a=>a.SemesterId == currentSemesterId) != null && t.EnrichmentClassTeacherAssignments.Where(a=>a.SemesterId == currentSemesterId).ToList().Count !=0)
            {
                vm.AssignedEnrichmentClassId = t.EnrichmentClassTeacherAssignments.Where(a => a.SemesterId == currentSemesterId).First().EnrichmentClassId;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignClass(int TeacherId, int? AssignedClassId, int AssignedEnrichmentClassId)
        {
          
            Teacher teacher = dbContext.Teachers.FirstOrDefault(t => t.TeacherId == TeacherId && t.IsActive);
            if (teacher == null)
            {
                return HttpNotFound("Teacher NOT Found");
            }
            List<ClassTeacherAssignment> assignments = teacher.ClassTeacherAssignments.Where(a => a.SemesterId == currentSemesterId).ToList();
            if (AssignedClassId.HasValue)
            {
                if (assignments == null || assignments.Count == 0)
                {
                    ClassTeacherAssignment a = new ClassTeacherAssignment()
                    {
                        TeacherId = TeacherId,
                        ClassId = AssignedClassId.Value,
                        SemesterId = currentSemesterId,
                        upsrt_dttm = DateTime.Now,
                        upsrt_user = User.Identity.GetUserName()

                    };

                    dbContext.ClassTeacherAssignments.Add(a);
                    dbContext.SaveChanges();
                    return RedirectToAction("Details", new { area = "Admin", controller = "Teacher", id = TeacherId });
                }
                ClassTeacherAssignment asn = assignments.First();
                if (asn.ClassId != AssignedClassId.Value)
                {
                    asn.ClassId = AssignedClassId.Value;
                    asn.upsrt_user = User.Identity.GetUserName();
                    asn.upsrt_dttm = DateTime.Now;
                }
                dbContext.SaveChanges();
               
            }
            else
            {
                if (assignments!=null && assignments.Count>0)
                {
                    dbContext.ClassTeacherAssignments.Remove(assignments.First());
                    dbContext.SaveChanges();
                   
                }
            }
            List<EnrichmentClassTeacherAssignment> eAssignments = teacher.EnrichmentClassTeacherAssignments.Where(a => a.SemesterId == currentSemesterId).ToList();
            if (AssignedEnrichmentClassId >0)
            {
               if (eAssignments == null || eAssignments.Count ==0)
               {
                   EnrichmentClassTeacherAssignment eAssignment = new EnrichmentClassTeacherAssignment()
                   {
                       TeacherId = TeacherId,
                       EnrichmentClassId = AssignedEnrichmentClassId,
                       SemesterId = currentSemesterId,
                       upsrt_dttm = DateTime.Now,
                       upsrt_user = User.Identity.GetUserName()

                   };
                   dbContext.EnrichmentClassTeacherAssignments.Add(eAssignment);
                   dbContext.SaveChanges();
                   return RedirectToAction("Details", new { area = "Admin", controller = "Teacher", id = TeacherId });
               }
               EnrichmentClassTeacherAssignment easn = eAssignments.First();
               if (easn.EnrichmentClassId != AssignedEnrichmentClassId)
               {
                   easn.EnrichmentClassId = AssignedEnrichmentClassId;
                   easn.upsrt_user = User.Identity.GetUserName();
                   easn.upsrt_dttm = DateTime.Now;
                   dbContext.SaveChanges();
               }
               return RedirectToAction("Details", new { area = "Admin", controller = "Teacher", id = TeacherId });
            }
            else
            {
                if (eAssignments != null && eAssignments.Count>0 )
                {
                    dbContext.EnrichmentClassTeacherAssignments.Remove(eAssignments.First());
                    dbContext.SaveChanges();
                    return RedirectToAction("Details", new { area = "Admin", controller = "Teacher", id = TeacherId });
                }

            }
            return RedirectToAction("Details", new { area = "Admin", controller = "Teacher", id = TeacherId });

            //return View();
        }

        public ActionResult AssignAllTeachers()
        {
            //TeacherAssignAllClassViewModel vm = new TeacherAssignAllClassViewModel();
            //vm.Classes = dbContext.Classes.Where(c => c.ActiveFlg).ToList();
            //vm.Teachers = dbContext.v_TeacherClass;
            TeacherAssignmentViewModel vm = new TeacherAssignmentViewModel()
            {
                Assignments = dbContext.v_TeacherAssignment.ToList(),
                Classes = dbContext.Classes.Where(c=>c.ActiveFlg).ToList(),
                EnrichmentClasses = dbContext.EnrichmentClasses.Where(e=>e.ActiveFlg).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignAllTeachers(TeacherAssignmentViewModel vm)
        {
            if (ModelState.IsValid)
            {
                foreach(var t in vm.Assignments)
                {
                    ClassTeacherAssignment teacherAssignment = dbContext.ClassTeacherAssignments.FirstOrDefault(a => a.TeacherId == t.TeacherId && a.SemesterId == currentSemesterId);
                    EnrichmentClassTeacherAssignment teacherEnrichmentAssignment = dbContext.EnrichmentClassTeacherAssignments.FirstOrDefault(a => a.TeacherId == t.TeacherId && a.SemesterId == currentSemesterId);

                    if (!t.ClassId.HasValue)
                    {
                        if (teacherAssignment!=null)
                        {
                            dbContext.ClassTeacherAssignments.Remove(teacherAssignment);
                            dbContext.SaveChanges();
                        }
                    }else
                    {
                        if (teacherAssignment == null)
                        {
                            teacherAssignment = new ClassTeacherAssignment()
                            {
                                ClassId = t.ClassId.Value,
                                TeacherId = t.TeacherId,
                                SemesterId = currentSemesterId,
                                upsrt_dttm=DateTime.Now,
                                upsrt_user = User.Identity.GetUserName()
                            };
                            dbContext.ClassTeacherAssignments.Add(teacherAssignment);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            if (teacherAssignment.ClassId != t.ClassId.Value)
                            {
                                teacherAssignment.ClassId = t.ClassId.Value;
                                teacherAssignment.upsrt_dttm = DateTime.Now;
                                teacherAssignment.upsrt_user = User.Identity.GetUserName();
                                dbContext.SaveChanges();
                            }
                        }
                        
                    }
                    if (!t.EnrichmentClassId.HasValue || t.EnrichmentClassId.Value ==0)
                    {
                        if (teacherEnrichmentAssignment != null)
                        {
                            dbContext.EnrichmentClassTeacherAssignments.Remove(teacherEnrichmentAssignment);
                            dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        if (teacherEnrichmentAssignment == null)
                        {
                            teacherEnrichmentAssignment = new EnrichmentClassTeacherAssignment()
                            {
                                EnrichmentClassId = t.EnrichmentClassId.Value,
                                TeacherId = t.TeacherId,
                                SemesterId = currentSemesterId,
                                upsrt_dttm = DateTime.Now,
                                upsrt_user = User.Identity.GetUserName()
                            };
                            dbContext.EnrichmentClassTeacherAssignments.Add(teacherEnrichmentAssignment);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            if (teacherEnrichmentAssignment.EnrichmentClassId != t.EnrichmentClassId.Value)
                            {
                                teacherEnrichmentAssignment.EnrichmentClassId = t.EnrichmentClassId.Value;
                                teacherEnrichmentAssignment.upsrt_dttm = DateTime.Now;
                                teacherEnrichmentAssignment.upsrt_user = User.Identity.GetUserName();
                                dbContext.SaveChanges();
                            }
                        }

                    }
                }
                return RedirectToAction("Index");
            }
            vm.Classes = dbContext.Classes.Where(c => c.ActiveFlg).ToList();
            return View();
        }
	}
}