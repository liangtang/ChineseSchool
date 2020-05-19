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
using ChineseSchool.Entities;
using System.Data.Entity;


namespace ChineseSchool.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class StudentController : Controller
    {
        private static ChineseSchoolEntities dbContext = new ChineseSchoolEntities();
       
        private int currentSemesterId = dbContext.Semesters.FirstOrDefault(s => s.ActiveFlg).SemesterID;
        private int previousSemesterId = dbContext.Semesters.Where(s => !s.ActiveFlg).Max(s => s.SemesterID);

        //private ApplicationSignInManager _signInManager;
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
        public ActionResult Index()
        {
            IEnumerable<Student> students = dbContext.Students.AsNoTracking().Where(c=>!c.Deleted).OrderBy(c=>c.Lastname);
            StudentListViewModel stu = null;
            List<StudentListViewModel> stus = new List<StudentListViewModel>();
            foreach (Student student in students)
            {
                stu = new StudentListViewModel()
                {
                    StudentId=student.StudentId,
                    ParentsId = student.ParentsId,
                    Firstname = student.Firstname,
                    Lastname = student.Lastname,
                    Chinesename = student.Chinesename,
                    Gender = student.Gender,
                    Birthday = student.Birthday.HasValue? student.Birthday.Value.ToShortDateString() : null
                };
                
                ClassRegistration cr = student.ClassRegistrations.FirstOrDefault(c=>c.SemesterId==currentSemesterId && c.ActiveFlag);
                if (cr!=null)
                {
                    stu.RegisterClass = "Yes";
                    if (cr.Class != null)
                    {
                        stu.Class = cr.Class.Classname;
                    }

                    if (cr.Class != null)
                    {
                        stu.Class = cr.Class.Classname;
                    }
                    else
                    {
                        stu.Class = null;
                    }
                    
                }else
                {
                    stu.Class = null;
                }

                

                if (student.EnrichmentClassRegistrations.FirstOrDefault(c => c.SemesterId == currentSemesterId && c.ActiveFlag) != null)
                {
                    stu.EnrichmentClass = student.EnrichmentClassRegistrations.FirstOrDefault(c => c.SemesterId == currentSemesterId && c.ActiveFlag).EnrichmentClass.ClassName;
                }
                stus.Add(stu);
            }
           
            return View(stus);
        }

        public ActionResult Details(int? studentId)
        {
            if (!studentId.HasValue)
            {
                return HttpNotFound("Invalid student Id");
            }
            Student student = dbContext.Students.AsNoTracking().FirstOrDefault(s => s.StudentId == studentId.Value);
            if (student == null)
            {
                return HttpNotFound("Invalid student Id");
            }
            StudentDetailViewModel vm = new StudentDetailViewModel();
            vm.Parent = dbContext.Parents.FirstOrDefault(p => p.ParentsId == student.ParentsId);
            vm.Student = student;
            if (student.ClassRegistrations.Where(c => c.SemesterId == currentSemesterId).Count() > 0)
            {
                vm.IfRegistered = "Yes";
                Class c = dbContext.ClassRegistrations.FirstOrDefault(cl => cl.SemesterId == currentSemesterId && cl.StudentId == student.StudentId).Class;
                vm.ClassAssigned = (c == null) ? "Not Assigned" : c.Classname;
            }
            
            if (dbContext.EnrichmentClassRegistrations.Where(e => e.StudentId == studentId && e.SemesterId == currentSemesterId).Count() > 0)
            {
                vm.EnrichmentmentClass = dbContext.EnrichmentClassRegistrations.FirstOrDefault(e => e.StudentId == student.StudentId && e.SemesterId == currentSemesterId).EnrichmentClass.ClassName;
            }
            else
            {
                vm.EnrichmentmentClass = "NONE";
            }
            return View(vm);
        }

        public ActionResult Edit(int? studentId)
        {
            if (!studentId.HasValue)
            {
                return HttpNotFound("Invalid student id");
            }
            Student student = dbContext.Students.FirstOrDefault(s => s.StudentId == studentId.Value);
            if (student == null)
            {
                return HttpNotFound("Invalid student id");
            }
            ViewBag.currentTab = "Edit";
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                Student stu = dbContext.Students.FirstOrDefault(s => s.StudentId == student.StudentId);
                stu.Firstname = student.Firstname;
                stu.Lastname = student.Lastname;
                stu.MiddleInitial = student.MiddleInitial;
                stu.Gender = student.Gender;
                stu.Birthday = student.Birthday;
                dbContext.SaveChanges();
                return RedirectToAction("Details", "parents", new { area = "Admin", parentId = stu.ParentsId });
            }
            return View();
        }
        public ActionResult StudentsByEnrichmentClass(int? classId)
        {
            ChineseSchool.Entities.EnrichmentClass cl = null;
            if (classId.HasValue && classId.Value > 0)
            {
                cl = dbContext.EnrichmentClasses.FirstOrDefault(c => c.ClassID == classId.Value);
            }
            StudentByEnrichmentClassViewModel vm = new StudentByEnrichmentClassViewModel();
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.Where(e=>e.ActiveFlg);
            if (classId.HasValue && classId.Value == 0)
            {
                vm.Students = dbContext.v_StudentEnrichmentClass.Where(s => s.Classname == null).OrderBy(v => v.Parent1LastName);
                vm.selectedName = "Not Assigned";
                vm.selected = 0;
            }
            else if (!classId.HasValue || classId.Value == -1)
            {
                vm.Students = dbContext.v_StudentEnrichmentClass.OrderBy(v => v.Parent1LastName);
                vm.selected = -1;
                vm.selectedName = "All";
            }
            else
            {
                vm.Students = dbContext.v_StudentEnrichmentClass.Where(c => c.Classname == cl.ClassName).OrderBy(v => v.Parent1LastName);
                vm.selected = cl.ClassID;
                vm.selectedName = cl.ClassName;
            }
            return View(vm);
        }
        public ActionResult StudentsByClass(int? classId)
        {
             ChineseSchool.Entities.Class cl = null; 
            if(classId.HasValue && classId.Value>0)
            {
              cl = dbContext.Classes.FirstOrDefault(c => c.ClassId == classId.Value);
            }
            
            
            StudentByClassViewModel vm = new StudentByClassViewModel();
            vm.Classes = dbContext.Classes.Where(c => c.ActiveFlg);
            if (classId.HasValue && classId.Value == 0)
            {
                vm.Students = dbContext.v_StudentClassDetails.Where(s => s.Classname == null).OrderBy(v => v.Parent1LastName);
                vm.selectedName = "Not Assigned";
                vm.selected = 0;
            }
            else if (!classId.HasValue || classId.Value ==-1)
            {
                vm.Students = dbContext.v_StudentClassDetails.OrderBy(v => v.Parent1LastName);
                vm.selected = -1;
                vm.selectedName = "All";
            }else
            {
                vm.Students = dbContext.v_StudentClassDetails.Where(c => c.Classname == cl.Classname).OrderBy(v => v.Parent1LastName);
                vm.selected = cl.ClassId;
                vm.selectedName = cl.Classname;
            }
            
            return View(vm);
        }
        public ActionResult AssignClass(int? studentId)
        {
            if (!studentId.HasValue)
            {
                return HttpNotFound("Invalid student id");
            }
            Student student = dbContext.Students.FirstOrDefault(s => s.StudentId == studentId.Value);
            if (student == null)
            {
                return HttpNotFound("Invalid student id");
            }
            AssignClassViewModel vm = new AssignClassViewModel();
            vm.Student = student;
            vm.AssignedClassId = 0;
            if (student.ClassRegistrations != null && student.ClassRegistrations.Count() > 0)
            {
                ClassRegistration cr = dbContext.ClassRegistrations.FirstOrDefault(r => r.StudentId == studentId.Value && r.SemesterId == currentSemesterId && r.ActiveFlag);
                if (cr.ClassId != null)
                {
                    vm.AssignedClassId = cr.ClassId.Value;
                }
               
            }

            vm.Classes = dbContext.Classes.Where(c=>c.ActiveFlg).ToList();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignClass(AssignClass model)
        {
            if (ModelState.IsValid)
            {
                ClassRegistration classRegistration = dbContext.ClassRegistrations.FirstOrDefault(c=>c.SemesterId==currentSemesterId && c.StudentId==model.StudentId);
                classRegistration.ClassId = model.Class;
                dbContext.SaveChanges();
                return RedirectToAction("Details", new { studentId = model.StudentId });
            }
            return View();
        }

        public ActionResult RemoveEnrichment(int? studentId)
        {
            if (!studentId.HasValue)
            {
                return HttpNotFound("Invalid student id");
            }
            Student student = dbContext.Students.FirstOrDefault(s => s.StudentId == studentId.Value);
            if (student == null)
            {
                return HttpNotFound("Invalid student id");
            }
            if (student.EnrichmentClassRegistrations.Where(e=>e.ActiveFlag && e.SemesterId == currentSemesterId) == null)
            {
                TempData["Message"] = "This Student does not have Enrichment Class Registered!";
                return RedirectToAction("Details", new { studentId = studentId }); 
            }
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveEnrichment(int studentId)
        {
            if(ModelState.IsValid)
            {
                dbContext.sp_RemoveEnrichment(studentId, User.Identity.GetUserId());
                return RedirectToAction("Details", new { studentId = studentId }); 
            }
            return View();
        }

        public ActionResult AddEnrichment(int? studentId)
        {
            if (!studentId.HasValue)
            {
                return HttpNotFound("Invalid student id");
            }
            Student student = dbContext.Students.FirstOrDefault(s => s.StudentId == studentId.Value);
            if (student == null)
            {
                return HttpNotFound("Invalid student id");
            }
            AssignEnrichmentClassViewModel vm = new AssignEnrichmentClassViewModel();
            vm.Student = student;
            vm.AssignedEnrichmentClassId = 0;
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.Where(e => e.ActiveFlg).ToList();
            ViewBag.currentTab = "AddEnrichment";
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEnrichment(int studentId, int EnrichmentClass)
        {
            if(ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                dbContext.sp_AddEnrichment(studentId, EnrichmentClass,userId);
                return RedirectToAction("Details", new { studentId = studentId }); 
            }
            return View();
        }

        public ActionResult AssignStudentByClass(int?  classId)
        {
            StudentByClassBulkViewModel vm = new StudentByClassBulkViewModel();
            vm.Classes = dbContext.Classes.Where(c => c.ActiveFlg).ToList();

            if(!classId.HasValue ||(classId.HasValue && classId.Value==-1))
            {
                vm.selected = -1;
                vm.selectedName = "All";
                vm.Students = dbContext.v_StudentClass.AsNoTracking().ToList().OrderBy(v => v.Lastname);
                
                

            }
            if(classId.HasValue && classId.Value==0)
            {
                vm.selected = 0;
                vm.selectedName = "Unassigned";
                vm.Students = dbContext.v_StudentClass.Where(s => s.Classname == null).AsNoTracking().ToList().OrderBy(v => v.Parent1LastName);
            }

            if (classId.HasValue && classId.Value>0)
            {
                ChineseSchool.Entities.Class cl = dbContext.Classes.FirstOrDefault(c => c.ClassId == classId.Value);
                vm.Students = dbContext.v_StudentClass.Where(s => s.Classname == cl.Classname).AsNoTracking().ToList().OrderBy(v => v.Parent1LastName);
                vm.selected = cl.ClassId;
                vm.selectedName = cl.Classname;
            }

            foreach (var student in vm.Students)
            {
                Student stu = dbContext.Students.FirstOrDefault(s => s.StudentId == student.StudentId);
                ClassRegistration previousClassRegistration = stu.ClassRegistrations.LastOrDefault(r => r.SemesterId == previousSemesterId);
                if (previousClassRegistration != null)
                {
                    if (previousClassRegistration.ClassId != null)
                    {
                        student.PreviousClass = previousClassRegistration.Class.Classname;
                    }
                    else
                    {
                        student.PreviousClass = null;
                    }
                }
                else
                {
                    student.PreviousClass = null;
                }
            }
            
            return View(vm);
        }

        public ActionResult AssignStudentByEnrichmentClass(int? classId)
        {
            StudentByEnrichmentClassBulkViewModel vm = new StudentByEnrichmentClassBulkViewModel();
            vm.Classes = dbContext.EnrichmentClasses.Where(c => c.ActiveFlg).ToList();
            if (!classId.HasValue || (classId.HasValue && classId.Value == -1))
            {
                vm.selected = -1;
                vm.selectedName = "All";
                vm.Students = dbContext.v_StudentEnrichmentClass.AsNoTracking().ToList().OrderBy(v=>v.Parent1LastName);


            }
            if (classId.HasValue && classId.Value == 0)
            {
                vm.selected = 0;
                vm.selectedName = "Unassigned";
                vm.Students = dbContext.v_StudentEnrichmentClass.Where(s => s.Classname == null).AsNoTracking().ToList().OrderBy(v => v.Parent1LastName);
            }
            if (classId.HasValue && classId.Value > 0)
            {
                ChineseSchool.Entities.EnrichmentClass cl = dbContext.EnrichmentClasses.FirstOrDefault(c => c.ClassID == classId.Value);
                vm.Students = dbContext.v_StudentEnrichmentClass.Where(s => s.Classname == cl.ClassName).AsNoTracking().ToList().OrderBy(v => v.Parent1LastName);
                vm.selected = cl.ClassID;
                vm.selectedName = cl.ClassName;
            }

            foreach (var student in vm.Students)
            {
                Student stu = dbContext.Students.FirstOrDefault(s => s.StudentId == student.StudentId);
                EnrichmentClassRegistration previousEnrichmentClassRegistration = stu.EnrichmentClassRegistrations.LastOrDefault(r => r.SemesterId == previousSemesterId);
                if (previousEnrichmentClassRegistration != null)
                {
                    if (previousEnrichmentClassRegistration.EnrichmentClassId != null)
                    {
                        student.PreviousEnrichmentClass = previousEnrichmentClassRegistration.EnrichmentClass.ClassName;
                    }
                    else
                    {
                        student.PreviousEnrichmentClass = null;
                    }
                }
                else
                {
                    student.PreviousEnrichmentClass = null;
                }
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignStudentByClass(int selectedId, List<v_StudentClass> students )
        {
            if (ModelState.IsValid)
            {
                foreach(var stu in students)
                {
                    ClassRegistration classReg = dbContext.ClassRegistrations.FirstOrDefault(r=>r.StudentId==stu.StudentId && r.SemesterId==currentSemesterId);

                    classReg.ClassId = stu.classId;

                    dbContext.Entry(classReg).State = EntityState.Modified;
                    dbContext.SaveChanges();
                 }
            }
            return RedirectToAction("AssignStudentByClass", new { classId = selectedId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignStudentByEnrichmentClass(int selectedId, List<v_StudentEnrichmentClass> students)
        {
            if (ModelState.IsValid)
            {
                foreach(var stu in students)
                {
                    EnrichmentClassRegistration classReg = dbContext.EnrichmentClassRegistrations.FirstOrDefault(r => r.StudentId == stu.StudentId && r.SemesterId == currentSemesterId);
                    EnrichmentClass ec = dbContext.EnrichmentClasses.FirstOrDefault(e => e.ClassID == stu.classId);
                    classReg.EnrichmentClassId = stu.classId;
                    classReg.Amount = ec.Price1;
                    dbContext.Entry(classReg).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            return RedirectToAction("AssignStudentByEnrichmentClass", new  { classId = selectedId });
        }
      

        public ActionResult StudentNavBar(int studentId, string currentTab)
        {
            Student student = dbContext.Students.FirstOrDefault(s => s.StudentId == studentId);
            ViewBag.currentTab = currentTab;
            return PartialView("_StudentNavBar", student);
        }


	}


}