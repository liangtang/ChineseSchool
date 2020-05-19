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
using ChineseSchool.Utilities;
namespace ChineseSchool.Controllers
{
    public class RegisterController : Controller
    {
        
        private static ChineseSchoolEntities dbContext = new ChineseSchoolEntities();
        private int currentSemesterId = dbContext.Semesters.FirstOrDefault(s => s.ActiveFlg).SemesterID;
        
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

        public ActionResult Tutorial()
        {
            return View();
        }
        public ActionResult Register()
        {
            if (!User.Identity.IsAuthenticated)
            {
                //TempData["Message"] = "Please Log In";
                return RedirectToAction("Tutorial");
            }
            string email = User.Identity.GetUserName();
            Parent parent = dbContext.Parents.FirstOrDefault(p => p.PrimaryEmail == email);
            IEnumerable<Student> allStudents =new List<Student>();
            IEnumerable<Student> nonRegistered = new List<Student>();
            if (parent != null)
            {
                allStudents = dbContext.Students.Where(s => s.ParentsId == parent.ParentsId);
                nonRegistered = dbContext.Students.Where(s => s.ParentsId == parent.ParentsId && s.ClassRegistrations.Where(c => c.SemesterId == currentSemesterId).Count() == 0);
            }
            if (allStudents.Count()>0 && nonRegistered.Count() ==0)
            {
                TempData["Message"] = "All your children have been registered!";
                return RedirectToAction("Index", "MyAccount");
            }
            if (allStudents.Count()>0 && nonRegistered.Count()>0)
            {
                return RedirectToAction("ExistingStudentsRegister");
            }

            RegisterClassViewModel vm =new RegisterClassViewModel();
            vm.Classes = dbContext.Classes.Where(c=>c.ActiveFlg).ToList();
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.Where(e=>e.ActiveFlg).ToList();
            vm.PrimaryEmail = User.Identity.GetUserName();

            if (parent!=null)
            {
                var students = parent.Students.Where(s => !s.Deleted && (s.ClassRegistrations == null || s.ClassRegistrations.First(a=>a.SemesterId== currentSemesterId)==null)).ToList();
                if (students.Count>0)
                {
                    return RedirectToAction("RegisterExistStudents");
                }
                vm.Parent1Firstname = parent.Parent1LastName;
                vm.Parent1LastName = parent.Parent1LastName;
                vm.Parent2Firstname = parent.Parent2Firstname;
                vm.Parent2Lastname = parent.Parent2Lastname;
                vm.PrimaryPhone = parent.PrimaryPhone;
                vm.SecondaryPhone = parent.SecondaryPhone;
                vm.SecondaryEmail = parent.SecondaryEmail;
                vm.State = parent.State;
                vm.ZipCode = parent.ZipCode;
                vm.AddressLine1 = parent.AddressLine1;
                vm.AddressLine2 = parent.AddressLine2;
                vm.City = parent.City;
                
                //foreach(Student student in parent.Students)
                //{
                //    StudentViewModel stu = new StudentViewModel();
                //    stu.Firstname = student.Firstname;
                //    stu.Lastname = student.Lastname;
                //    stu.Gender = student.Gender;
                //    stu.Chinesename = student.Chinesename;
                //    stu.Birthday = student.Birthday.ToShortDateString();
                //    stu.EnrichmentClass = student.EnrichmentClass == null ? 0 : dbContext.EnrichmentClasses.FirstOrDefault(c => c.ClassName == student.EnrichmentClass && c.ActiveFlg).ClassID;
                //    stu.IfRegister = student.RegisterClass;
                //    stu.Class = student.Class==null? 0 : dbContext.Classes.FirstOrDefault(c=>c.Classname == student.Class && c.ActiveFlg).ClassId;
                //}

            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult Register(RegisterClassViewModel registry)
        {
            CheckStudents(registry.Students);
            int parentId;
            if (ModelState.IsValid)
            {
                parentId = SaveRegistry(registry);
                Parent parent = dbContext.Parents.AsNoTracking().FirstOrDefault(pa => pa.ParentsId == parentId);
                string msg = "Registration Comfirmation from Toledo Chinese School:<br /><br /><table><tr><th>FirstName</th><th>LastName</th><th>Register Chiness Class</th><th>Class</th><th>Enrichment Class</th></tr>";
                foreach (var stu in registry.Students)
                {

                    msg = msg + "<tr><td>" + stu.Firstname + "</td>" + "<td>" + stu.Lastname + "</td>";
                    if (stu.IfRegister)
                    {
                        msg = msg + "<td>Yes</td>";
                        if (stu.Class == null)
                        {
                            msg = msg + "<td>Not Assigned</td>";
                        }
                        else
                        {
                            Class cl = dbContext.Classes.FirstOrDefault(c => c.ClassId == stu.Class);
                            msg = msg + "<td>" + cl.Classname + "</td>";
                        }
                    }
                    else
                    {
                        msg = msg = "<td>No</td><td>N/A</td>";
                    }
                    if (stu.EnrichmentClass == 0)
                    {
                        msg = msg + "<td>N/A</td>";
                    }
                    else
                    {
                        var ec = dbContext.EnrichmentClasses.FirstOrDefault(e => e.ClassID == stu.EnrichmentClass);
                        msg = msg + "<td>" + ec.ClassName + "</td>";
                    }
                    msg = msg + "</tr>";
                }
                msg = msg + "</table>";
                msg = msg + "<br /><br />";
                msg = msg + "Chinese Center of Toledo<br />";
                msg = msg + "(419)450-9029<br /> ";
                msg = msg + "http://chinesecenteroftoledo.org<br />";
                MailSender ms = new MailSender("admin@chinesecenteroftoledo.org", "7348@Iris");
                ms.SendMail(parent.PrimaryEmail, "Registration Confirmation", msg);
                ms.SendMail("registration@chinesecenteroftoledo.org", "New Students Registered", msg);

                TempData["Message"] = "Please print the invoice and mail it with payment to CCT. Please write the invoice number on the check.";
                return RedirectToAction("Invoice", "MyAccount");
            }
            registry.Classes =  dbContext.Classes.ToList();
            registry.EnrichmentClasses = dbContext.EnrichmentClasses.ToList();
            registry.PrimaryEmail = User.Identity.GetUserName();
            return View(registry);
        }

        [HttpGet]
        public ActionResult ExistingStudentsRegister()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Message"] = "Please Log In";
                return RedirectToAction("Login", "Account", new { returnUrl="/register/ExistingStudentsRegister"});
            }
            string email = User.Identity.GetUserName();
            Parent parent = dbContext.Parents.FirstOrDefault(p => p.PrimaryEmail == email);
            if (parent == null)
            {
                return RedirectToAction("Register");
            }
            int id = parent.ParentsId;
            IEnumerable<Student> students = dbContext.Students.Where(s => s.ParentsId == id);
            if (students==null || students.Count() ==0)
            {
                return RedirectToAction("Register");
            }
            students = students.Where(s => s.ClassRegistrations.Where(c => c.SemesterId == this.currentSemesterId && c.ActiveFlag).Count() == 0);
            List<ExistingStudentViewModel> existingStudents = new List<ExistingStudentViewModel>();
            foreach (var student in students)
            {
                ExistingStudentViewModel existingStudent = new ExistingStudentViewModel();
                existingStudent.StudentId = student.StudentId;
                existingStudent.Firstname = student.Firstname;
                existingStudent.Lastname = student.Lastname;
                existingStudent.Chinesename = student.Chinesename;
                existingStudents.Add(existingStudent);
            }
            ExistingRegisterViewModel vm = new ExistingRegisterViewModel();
            vm.ParentId = parent.ParentsId;
            vm.Students = existingStudents;
            vm.Classes = dbContext.Classes.Where(c => c.ActiveFlg).ToList();
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.Where(e=>e.ActiveFlg).ToList();
            
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExistingStudentsRegister(ExistingRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                 using (System.Data.Entity.DbContextTransaction dbTran = dbContext.Database.BeginTransaction())
                {
                    foreach(ExistingStudentViewModel stu in model.Students)
                    {
                        dbContext.RegisterStudent(stu.StudentId, stu.IfRegister, stu.Class, stu.EnrichmentClass, User.Identity.GetUserId(), DateTime.Now);
                    }
                    dbContext.sp_updateinvoice(model.ParentId);
                    dbTran.Commit();
                }
                 Parent parent = dbContext.Parents.FirstOrDefault(pa => pa.ParentsId == model.ParentId);
                 string msg = "Registration Comfirmation from Toledo Chinese School:<br /><br /><table><tr><th>FirstName</th><th>LastName</th><th>Register Chiness Class</th><th>Class</th><th>Enrichment Class</th></tr>";
                 foreach (var stu in model.Students)
                 {

                     msg = msg + "<tr><td>" + stu.Firstname + "</td>" + "<td>" + stu.Lastname + "</td>";
                     if (stu.IfRegister)
                     {
                         msg = msg + "<td>Yes</td>";
                         if (stu.Class == null)
                         {
                             msg = msg + "<td>Not Assigned</td>";
                         }
                         else
                         {
                             Class cl = dbContext.Classes.FirstOrDefault(c => c.ClassId == stu.Class);
                             msg = msg + "<td>" + cl.Classname + "</td>";
                         }
                     }
                     else
                     {
                         msg = msg = "<td>No</td><td>N/A</td>";
                     }
                     if (stu.EnrichmentClass == 0)
                     {
                         msg = msg + "<td>N/A</td>";
                     }
                     else
                     {
                         var ec = dbContext.EnrichmentClasses.FirstOrDefault(e => e.ClassID == stu.EnrichmentClass);
                         msg = msg + "<td>" + ec.ClassName + "</td>";
                     }
                     msg = msg + "</tr>";
                 }
                 msg = msg + "</table>";
                 msg = msg + "<br /><br />";
                 msg = msg + "Chinese Center of Toledo<br />";
                 msg = msg + "(419)450-9029<br /> ";
                 msg = msg + "http://chinesecenteroftoledo.org<br />";
                 MailSender ms = new MailSender("admin@chinesecenteroftoledo.org", "7348@Iris");
                 ms.SendMail(parent.PrimaryEmail, "Registration Confirmation", msg);
                 ms.SendMail("registration@chinesecenteroftoledo.org", "New Students Registered", msg);
                 return RedirectToAction("Invoice", "MyAccount");
            }

            model.Classes = dbContext.Classes.Where(c=>c.ActiveFlg).ToList();
            model.EnrichmentClasses = dbContext.EnrichmentClasses.Where(e=>e.ActiveFlg).ToList();
            return View(model);
        }

        private void CheckStudents(List<StudentViewModel> students)
        {
            List<Student> stus = new List<Student>();
            if (students==null || students.Count == 0)
            {
                ModelState.AddModelError("Students", "No Students Found!");
                return ;
            }


            foreach (var student in students)
            {
                if(student.Firstname ==null || student.Firstname.Length == 0 || student.Lastname == null || student.Lastname.Length == 0)
                {
                    ModelState.AddModelError("Students", "Student Name cannot be empty!");
                    return ;
                }
                Student stu = new Student();
                stu.Firstname = student.Firstname;
                stu.Lastname = student.Lastname;
                stu.Gender = student.Gender;
                //stu.Class = student.Class;
                
            }
        }

        private int SaveRegistry(RegisterClassViewModel registry)
        {
            Semester semester = dbContext.Semesters.FirstOrDefault(s => s.ActiveFlg);
            int parentId;
            Parent parent = new Parent();
            parent.UserId = User.Identity.GetUserId();
            parent.Parent1Firstname = registry.Parent1Firstname;
            parent.Parent1LastName = registry.Parent1LastName;
            parent.Parent2Firstname = registry.Parent2Firstname;
            parent.Parent2Lastname = registry.Parent2Lastname;
            parent.PrimaryEmail = registry.PrimaryEmail;
            parent.PrimaryPhone = registry.PrimaryPhone;
            parent.SecondaryEmail = registry.SecondaryEmail;
            parent.SecondaryPhone = registry.SecondaryPhone;
            parent.AddressLine1 = registry.AddressLine1;
            parent.AddressLine2 = registry.AddressLine2;
            parent.City = registry.City;
            parent.State = registry.State;
            parent.ZipCode = registry.ZipCode;
            parent.CreateUserId = User.Identity.GetUserId();
            parent.UpdateUserId = User.Identity.GetUserId();
            parent.CreateTimeStemp = DateTime.Now;
            parent.UpdateUserId = User.Identity.GetUserId();
            parent.UpdateTimeStemp = DateTime.Now;

            using (System.Data.Entity.DbContextTransaction dbTran = dbContext.Database.BeginTransaction())
            {
                
                dbContext.Parents.Add(parent);
                dbContext.SaveChanges();
                parentId = parent.ParentsId;
 

                int stuCnt = 0;
                foreach(var student in registry.Students)
                {
                    stuCnt++;
                    Student stu = new Student();
                    stu.ParentsId = parent.ParentsId;
                    stu.Firstname = student.Firstname;
                    stu.Lastname = student.Lastname;
                    stu.Gender = student.Gender;
                    stu.Class = dbContext.Classes.FirstOrDefault(c => c.ClassId == student.Class) == null ? null : dbContext.Classes.FirstOrDefault(c => c.ClassId == student.Class).Classname;
                    stu.Birthday = DateTime.Parse(student.Birthday);
                    stu.CreateTimeStemp = DateTime.Now;
                    stu.CreateUserId = User.Identity.GetUserId();
                    stu.RegisterClass = student.IfRegister;
                    stu.UpdateTimeStemp = DateTime.Now;
                    stu.UpdateUserId = User.Identity.GetUserId();
                    stu.EnrichmentClass = student.EnrichmentClass==0 ? null:dbContext.EnrichmentClasses.FirstOrDefault(c=>c.ClassID == student.EnrichmentClass).ClassName;

                    dbContext.Students.Add(stu);
                    dbContext.SaveChanges();


                    dbContext.RegisterStudent(stu.StudentId, stu.RegisterClass, student.Class, student.EnrichmentClass, User.Identity.GetUserId(), DateTime.Now);



   
                   
                }
                dbContext.sp_updateinvoice(parent.ParentsId);
                dbTran.Commit();
            }
            return parentId;

        }
	}
}