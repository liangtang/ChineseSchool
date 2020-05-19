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
using ChineseSchool.Utilities;

namespace ChineseSchool.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdminRegisterController : Controller
    {
        private static ChineseSchoolEntities dbContext = new ChineseSchoolEntities();
        private ApplicationUserManager _userManager;
        private static Random random = new Random();
        private int currentSemesterId = dbContext.Semesters.FirstOrDefault(s => s.ActiveFlg).SemesterID;
        private int previousSemesterId = dbContext.Semesters.Where(s => !s.ActiveFlg).Max(s => s.SemesterID);

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
        // GET: /Admin/Register/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddStudent(string email)
        {
            if (email == null)
            {
                return View("AddStudentEmailCheck");
            }
            if (UserManager.FindByName(email) != null)
            {
                string userId = UserManager.FindByName(email).Id;
                if (dbContext.Parents.FirstOrDefault(p => p.UserId == userId) != null)
                {
                    return RedirectToAction("AddStudentExistingParents", new { controller = "AdminRegister", area = "Admin", email = email });
                }

            }
            IEnumerable<Student> allStudents = new List<Student>();
            IEnumerable<Student> nonRegistered = new List<Student>();
            RegisterClassViewModel vm = new RegisterClassViewModel();
            vm.Classes = dbContext.Classes.ToList();
            vm.PrimaryEmail = email;
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.ToList();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStudent(RegisterClassViewModel vm)
        {
            int parentsId = 0;
            CheckStudents(vm.Students);
            if(ModelState.IsValid)
            {
                parentsId = SaveParentAndStudent(vm);
                return RedirectToAction("Details", new { controller = "Parents", area = "Admin", parentId = parentsId }); 
            }

            return View(vm);
        }

        public ActionResult AddStudentExistingParents(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                return RedirectToAction("AddStudent");
            }
            Parent parent = dbContext.Parents.FirstOrDefault(p => p.PrimaryEmail.ToLower() == email.ToLower());
            if (parent == null)
            {
                TempData["Message"] = "Parents info does not exist, redirect to new registration";
                return RedirectToAction("NewRegistration");
            }
            AddStudentViewModel vm = new AddStudentViewModel();
            vm.Students = new List<StudentViewModel>();
            StudentViewModel stu = new StudentViewModel();
            ViewBag.parentId = parent.ParentsId;
            vm.Classes = dbContext.Classes.Where(c => c.ActiveFlg).ToList();
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.Where(e => e.ActiveFlg).ToList();
            ViewBag.currentTab = "Add Students";
            return View(vm);
          
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult AddStudentExistingParents(AddStudentViewModel model, int parentId)
        {
            CheckStudents(model.Students);
            Parent p = dbContext.Parents.FirstOrDefault(pa=>pa.ParentsId == parentId);
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                using (System.Data.Entity.DbContextTransaction dbTran = dbContext.Database.BeginTransaction())
                {
                    int stuCnt = 0;
                    foreach (var student in model.Students)
                    {
                        stuCnt++;

                        Student stu = new Student();

                        stu.ParentsId = parentId;
                        stu.Firstname = student.Firstname;
                        stu.Lastname = student.Lastname;
                        stu.Gender = student.Gender;
                        stu.Class = dbContext.Classes.FirstOrDefault(c => c.ClassId == student.Class) == null ? null : dbContext.Classes.FirstOrDefault(c => c.ClassId == student.Class).Classname;
                        stu.Birthday = String.IsNullOrWhiteSpace(student.Birthday) ? DateTime.MinValue : DateTime.Parse(student.Birthday);
                        stu.CreateTimeStemp = DateTime.Now;
                        stu.CreateUserId = User.Identity.GetUserId();
                        stu.RegisterClass = student.IfRegister;
                        stu.UpdateTimeStemp = DateTime.Now;
                        stu.UpdateUserId = User.Identity.GetUserId();
                       

                        dbContext.Students.Add(stu);
                        dbContext.SaveChanges();


                        
                    }

                    dbTran.Commit();
                    string msg = "Registration Comfirmation from Toledo Chinese School:<br /><br /><table><tr><th>FirstName</th><th>LastName</th><th>Register Chiness Class</th><th>Class</th><th>Enrichment Class</th></tr>";
                    foreach(var stu in model.Students)
                    {
                        
                        msg=msg+"<tr><td>"+stu.Firstname+"</td>" +"<td>"+stu.Lastname +"</td>";
                        if (stu.IfRegister)
                        {
                            msg=msg+"<td>Yes</td>";
                        }else
                        {
                            msg=msg="<td>No</td>";
                        }
                        msg = msg + "<td>" +stu.Class +"</td>";
                        if (stu.EnrichmentClass == 0)
                        {
                            msg = msg + "<td>N/A</td>";
                        }
                        else
                        {
                            var ec = dbContext.EnrichmentClasses.FirstOrDefault(e => e.ClassID == stu.EnrichmentClass);
                            msg = msg +"<td>"+ ec.ClassName+"</td>";
                        }
                        msg = msg + "</tr>";
                    }
                    msg=msg+"</table>";
                    msg = msg + "<br /><br />";
                    msg = msg + "Chinese Center of Toledo<br />";
                    msg = msg + "(419)450-9029<br /> ";
                    msg = msg + "http://chinesecenteroftoledo.org<br />";
                    MailSender ms = new MailSender("admin@chinesecenteroftoledo.org", "7348@Iris");
                    ms.SendMail(p.PrimaryEmail, "Registration Confirmation", msg);
                    ms.SendMail("registration@chinesecenteroftoledo.org", "New Students Registered", msg);

                    return RedirectToAction("Details", new { controller = "Parents", area = "Admin", parentId = parentId });
                }
            }
            model.Classes = dbContext.Classes.Where(c => c.ActiveFlg).ToList();
            model.EnrichmentClasses = dbContext.EnrichmentClasses.Where(e => e.ActiveFlg).ToList();
            ViewBag.currentTab = "Add Students";
            ViewBag.parentId = parentId;
            return View(model);
            
        }

        public ActionResult NewRegistration(string email)
        {
            if (email==null)
            {
                return View("NewRegCheckemail");
            }
            if (UserManager.FindByName(email)!=null)
            {
                string userId = UserManager.FindByName(email).Id;
                if (dbContext.Parents.FirstOrDefault(p => p.UserId == userId) != null)
                {
                    return RedirectToAction("ExistingRegistration", new { controller = "AdminRegister", area = "Admin", email = email });
                }

            }
            IEnumerable<Student> allStudents = new List<Student>();
            IEnumerable<Student> nonRegistered = new List<Student>();
            RegisterClassViewModel vm = new RegisterClassViewModel();
            vm.Classes = dbContext.Classes.ToList();
            vm.PrimaryEmail = email;
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.ToList();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewRegistration(RegisterClassViewModel vm)
        {
            CheckStudents(vm.Students);
            int parentsId;
            if (ModelState.IsValid)
            {
                parentsId=SaveRegistry(vm);
                var parent = dbContext.Parents.FirstOrDefault(p => p.ParentsId == parentsId);
                
                string msg = "Registration Comfirmation from Toledo Chinese School:<br /><br /><table><tr><th>FirstName</th><th>LastName</th><th>Register Chiness Class</th><th>Class</th><th>Enrichment Class</th></tr>";
                foreach (var stu in vm.Students)
                {

                    msg = msg + "<tr><td>" + stu.Firstname + "</td>" + "<td>" + stu.Lastname + "</td>";
                    if (stu.IfRegister)
                    {
                        msg = msg + "<td>Yes</td>";
                        if (stu.Class == null)
                        {
                            msg = msg + "<td>Not Assigned</td>";
                        }else
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


                return RedirectToAction("Details", new { controller = "Parents", area = "Admin", parentId = parentsId });
            }
            vm.Classes = dbContext.Classes.ToList();
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.ToList();
            return View(vm);
        }

        public ActionResult ExistingRegistration(string email)
        {
            if (email == null)
            {
                return View("ExistingRegCheckEmail");
            }
            if (UserManager.FindByName(email)==null)
            {
                TempData["Message"] = "User does not exist, redirect to new registration";
                return RedirectToAction("NewRegistration");
            }
            string userId = UserManager.FindByName(email).Id;
            if(dbContext.Parents.FirstOrDefault(p=>p.UserId==userId) == null)
            {
                TempData["Message"] = "Parents info does not exist, redirect to new registration";
                return RedirectToAction("NewRegistration");
            }
            int parentsId = dbContext.Parents.FirstOrDefault(p => p.UserId == userId).ParentsId;
            if (dbContext.Students.FirstOrDefault(s => s.ParentsId == parentsId) == null)
            {
                TempData["Message"] = "Student info does not exist, redirect to addon registration";
            }
            IEnumerable<Student> students = dbContext.Students.Where(s => s.ParentsId == parentsId && s.ClassRegistrations.Where(c=>c.SemesterId==currentSemesterId).Count()==0);
            if (students == null || students.Count() ==0)
            {
                TempData["Message"] = "All students under this account are registered. Please use Addon Registration to add more student.";
                return RedirectToAction("Details", new { controller = "Parents", area = "Admin", parentId = parentsId });
            }
            List<ExistingStudentViewModel> existingStudents = new List<ExistingStudentViewModel>();
            foreach (var student in students)
            {
                var previousRegistration = student.ClassRegistrations.LastOrDefault(p => p.SemesterId == previousSemesterId);
                string previousClass = previousRegistration == null ? null : (previousRegistration.Class==null? null: previousRegistration.Class.Classname);
                var previousEnrichmentRegistration = student.EnrichmentClassRegistrations.LastOrDefault(r=>r.SemesterId==previousSemesterId);
                string previousEnrichmentClass = previousEnrichmentRegistration == null? null:(previousEnrichmentRegistration.EnrichmentClass == null? null: previousEnrichmentRegistration.EnrichmentClass.ClassName);
                ExistingStudentViewModel existingStudent = new ExistingStudentViewModel();
                existingStudent.StudentId = student.StudentId;
                existingStudent.Firstname = student.Firstname;
                existingStudent.Lastname = student.Lastname;
                existingStudent.PreviousClass = previousClass;
                existingStudent.PreviousEnrichmentClass = previousEnrichmentClass;
                existingStudents.Add(existingStudent);
            }
            ExistingRegisterViewModel vm = new ExistingRegisterViewModel();
            vm.Students = existingStudents;
            vm.Classes = dbContext.Classes.Where(c => c.ActiveFlg).ToList();
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.Where(e => e.ActiveFlg).ToList();

            return View(vm);

           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExistingRegistration(ExistingRegisterViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Students == null || vm.Students.Count == 0)
                {
                    return HttpNotFound("Invalid student ID");
                }
                int studentId = vm.Students[0].StudentId;
                int parentId=dbContext.Students.FirstOrDefault(s=>s.StudentId== studentId).ParentsId;
                Parent parent = dbContext.Parents.FirstOrDefault(p => p.ParentsId == parentId);
                foreach (ExistingStudentViewModel stu in vm.Students)
                {
                    dbContext.RegisterStudent(stu.StudentId, stu.IfRegister, stu.Class, stu.EnrichmentClass, User.Identity.GetUserId(), DateTime.Now);
                    
                }
                dbContext.sp_updateinvoice(parentId);
                string msg = "Registration Comfirmation from Toledo Chinese School:<br /><br /><table><tr><th>FirstName</th><th>LastName</th><th>Register Chiness Class</th><th>Class</th><th>Enrichment Class</th></tr>";
                foreach (var stu in vm.Students)
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

                return RedirectToAction("Details", new { controller = "Parents", area = "Admin", parentId = parentId });

            }

            vm.Classes = dbContext.Classes.ToList();
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.ToList();
            return View(vm);
            
        }


        public ActionResult AddonRegistration(string email)
        {
            if (email == null)
            {
                return View("AddonCheckEmail");
            }
            //if (UserManager.FindByName(email) == null)
            //{
            //    TempData["Message"] = "User does not exist, redirect to new registration";
            //    return RedirectToAction("NewRegistration");
            //}
            //string userId = UserManager.FindByName(email).Id;
            //if (dbContext.Parents.FirstOrDefault(p => p.UserId == userId) == null)
            //{
            //    TempData["Message"] = "Parents info does not exist, redirect to new registration";
            //    return RedirectToAction("NewRegistration");
            //}
            Parent parent = dbContext.Parents.FirstOrDefault(p => p.PrimaryEmail.ToLower() == email.ToLower());
            if (parent == null)
            {
                TempData["Message"] = "Parents info does not exist, redirect to new registration";
                return RedirectToAction("NewRegistration");
            }
            AddStudentViewModel vm = new AddStudentViewModel();
            vm.Students = new List<StudentViewModel>();
            StudentViewModel stu = new StudentViewModel();
            ViewBag.parentId = parent.ParentsId;
            vm.Classes = dbContext.Classes.Where(c => c.ActiveFlg).ToList();
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.Where(e => e.ActiveFlg).ToList();
            ViewBag.currentTab = "Add Students";
            return View(vm);
         }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddonRegistration(AddStudentViewModel vm, int parentId)
        {
            CheckStudents(vm.Students);
            
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                Parent parent = dbContext.Parents.FirstOrDefault(p => p.ParentsId == parentId);
                using (System.Data.Entity.DbContextTransaction dbTran = dbContext.Database.BeginTransaction())
                {
                    int stuCnt = 0;
                    foreach (var student in vm.Students)
                    {
                        stuCnt++;

                        Student stu = new Student();
                        
                        stu.ParentsId = parentId;
                        stu.Firstname = student.Firstname;
                        stu.Lastname = student.Lastname;
                        stu.Gender = student.Gender;
                        stu.Class = dbContext.Classes.FirstOrDefault(c => c.ClassId == student.Class) == null ? null : dbContext.Classes.FirstOrDefault(c => c.ClassId == student.Class).Classname;
                        stu.Birthday = String.IsNullOrWhiteSpace(student.Birthday) ? DateTime.MinValue : DateTime.Parse(student.Birthday);
                        stu.CreateTimeStemp = DateTime.Now;
                        stu.CreateUserId = User.Identity.GetUserId();
                        stu.RegisterClass = student.IfRegister;
                        stu.UpdateTimeStemp = DateTime.Now;
                        stu.UpdateUserId = User.Identity.GetUserId();
                        stu.EnrichmentClass = student.EnrichmentClass == 0 ? null : dbContext.EnrichmentClasses.FirstOrDefault(c => c.ClassID == student.EnrichmentClass).ClassName;

                        dbContext.Students.Add(stu);
                        dbContext.SaveChanges();


                        dbContext.RegisterStudent(stu.StudentId, stu.RegisterClass, student.Class, student.EnrichmentClass, User.Identity.GetUserId(), DateTime.Now);
                    }

                    dbTran.Commit();

                    string msg = "Registration Comfirmation from Toledo Chinese School:<br /><br /><table><tr><th>FirstName</th><th>LastName</th><th>Register Chiness Class</th><th>Class</th><th>Enrichment Class</th></tr>";
                    foreach (var stu in vm.Students)
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

                    return RedirectToAction("Details", new { controller = "Parents", area = "Admin", parentId = parentId });
                    
                }
            }
            vm.Classes = dbContext.Classes.Where(c => c.ActiveFlg).ToList();
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.Where(e => e.ActiveFlg).ToList();
            ViewBag.currentTab = "Add Students";
            ViewBag.parentId = parentId;
            return View(vm);
        }

        private void CheckStudents(List<StudentViewModel> students)
        {
            List<Student> stus = new List<Student>();
            if (students == null || students.Count == 0)
            {
                ModelState.AddModelError("Students", "No Students Found!");
                return;
            }
            foreach (var student in students)
            {
                if (student.Firstname == null || student.Firstname.Length == 0 || student.Lastname == null || student.Lastname.Length == 0)
                {
                    ModelState.AddModelError("Students", "Student Name cannot be empty!");
                    return;
                }
                Student stu = new Student();
                stu.Firstname = student.Firstname;
                stu.Lastname = student.Lastname;
                stu.Gender = student.Gender;
                //stu.Class = student.Class;

            }
        }

        private int SaveParentAndStudent(RegisterClassViewModel registry)
        {
            string userId = null;
            int parentsId;
            if (UserManager.FindByName(registry.PrimaryEmail) == null)
            {
                var user = new ApplicationUser() { UserName = registry.PrimaryEmail, Firstname = registry.Parent1Firstname, Lastname = registry.Parent1LastName, Email = registry.PrimaryEmail };
                var result = UserManager.Create(user, RandomString(8));
                if (result.Succeeded)
                {
                    userId = UserManager.FindByName(registry.PrimaryEmail).Id;
                    string token = UserManager.GenerateEmailConfirmationToken(userId);
                    IdentityResult result1 = UserManager.ConfirmEmail(userId, token);
                }
                else
                {
                    throw (new InvalidOperationException("User cannot be created!"));
                }
            }
            else
            {
                userId = UserManager.FindByName(registry.PrimaryEmail).Id;
            }
            Semester semester = dbContext.Semesters.FirstOrDefault(s => s.ActiveFlg);
            Parent parent = new Parent();
            parent.UserId = userId;
            parent.Parent1Firstname = registry.Parent1Firstname;
            parent.Parent1LastName = registry.Parent1LastName;
            parent.Parent2Firstname = registry.Parent2Firstname;
            parent.Parent2Lastname = registry.Parent2Lastname;
            parent.PrimaryEmail = registry.PrimaryEmail;
            parent.PrimaryPhone = registry.PrimaryPhone;
            parent.PrimaryEmail = registry.PrimaryEmail;
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
                parentsId = parent.ParentsId;


                int stuCnt = 0;
                foreach (var student in registry.Students)
                {
                    stuCnt++;
                    Student stu = new Student();
                    stu.ParentsId = parent.ParentsId;
                    stu.Firstname = student.Firstname;
                    stu.Lastname = student.Lastname;
                    stu.Gender = student.Gender;
                    stu.Class = dbContext.Classes.FirstOrDefault(c => c.ClassId == student.Class) == null ? null : dbContext.Classes.FirstOrDefault(c => c.ClassId == student.Class).Classname;
                    stu.Birthday = String.IsNullOrWhiteSpace(student.Birthday) ? DateTime.MinValue : DateTime.Parse(student.Birthday);
                    stu.CreateTimeStemp = DateTime.Now;
                    stu.CreateUserId = User.Identity.GetUserId();
                    stu.RegisterClass = student.IfRegister;
                    stu.UpdateTimeStemp = DateTime.Now;
                    stu.UpdateUserId = User.Identity.GetUserId();
                    stu.EnrichmentClass = student.EnrichmentClass == 0 ? null : dbContext.EnrichmentClasses.FirstOrDefault(c => c.ClassID == student.EnrichmentClass).ClassName;

                    dbContext.Students.Add(stu);
                    dbContext.SaveChanges();


                }
                
                dbTran.Commit();
                return parentsId;
            }

        }

        private int SaveRegistry(RegisterClassViewModel registry)
        {
            string userId = null;
            int parentsId;
            if (UserManager.FindByName(registry.PrimaryEmail) == null)
            {
                var user = new ApplicationUser() { UserName = registry.PrimaryEmail, Firstname = registry.Parent1Firstname, Lastname = registry.Parent1LastName, Email = registry.PrimaryEmail };
                var result =  UserManager.Create(user, RandomString(8));
                if (result.Succeeded)
                {
                    userId = UserManager.FindByName(registry.PrimaryEmail).Id;
                    string token = UserManager.GenerateEmailConfirmationToken(userId);
                    IdentityResult result1 = UserManager.ConfirmEmail(userId, token);
                }
                else
                {
                    throw (new InvalidOperationException("User cannot be created!"));
                }
            }
            else
            {
                userId = UserManager.FindByName(registry.PrimaryEmail).Id;
            }
            Semester semester = dbContext.Semesters.FirstOrDefault(s => s.ActiveFlg);
            Parent parent = new Parent();
            parent.UserId = userId;
            parent.Parent1Firstname = registry.Parent1Firstname;
            parent.Parent1LastName = registry.Parent1LastName;
            parent.Parent2Firstname = registry.Parent2Firstname;
            parent.Parent2Lastname = registry.Parent2Lastname;
            parent.PrimaryEmail = registry.PrimaryEmail;
            parent.PrimaryPhone = registry.PrimaryPhone;
            parent.PrimaryEmail = registry.PrimaryEmail;
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
                parentsId=parent.ParentsId;


                int stuCnt = 0;
                foreach (var student in registry.Students)
                {
                    stuCnt++;
                    Student stu = new Student();
                    stu.ParentsId = parent.ParentsId;
                    stu.Firstname = student.Firstname;
                    stu.Lastname = student.Lastname;
                    stu.Gender = student.Gender;
                    stu.Class = dbContext.Classes.FirstOrDefault(c => c.ClassId == student.Class) == null ? null : dbContext.Classes.FirstOrDefault(c => c.ClassId == student.Class).Classname;
                    stu.Birthday = String.IsNullOrWhiteSpace(student.Birthday)?DateTime.MinValue: DateTime.Parse(student.Birthday);
                    stu.CreateTimeStemp = DateTime.Now;
                    stu.CreateUserId = User.Identity.GetUserId();
                    stu.RegisterClass = student.IfRegister;
                    stu.UpdateTimeStemp = DateTime.Now;
                    stu.UpdateUserId = User.Identity.GetUserId();
                    stu.EnrichmentClass = student.EnrichmentClass == 0 ? null : dbContext.EnrichmentClasses.FirstOrDefault(c => c.ClassID == student.EnrichmentClass).ClassName;

                    dbContext.Students.Add(stu);
                    dbContext.SaveChanges();


                    dbContext.RegisterStudent(stu.StudentId, stu.RegisterClass, student.Class, student.EnrichmentClass, User.Identity.GetUserId(), DateTime.Now);





                }
                dbContext.sp_updateinvoice(parent.ParentsId);
                dbTran.Commit();
                return parentsId;
            }

        }

        
        public  string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
	}
}