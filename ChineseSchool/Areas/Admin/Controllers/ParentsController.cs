using System;
using System.IO;
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
    public class ParentsController : Controller
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

        //
        // GET: /Admin/Parents/
        public ActionResult Index()
        {
            IEnumerable<Parent> parents = dbContext.Parents.AsNoTracking().Where(p=>!p.Deleted).OrderBy(p=>p.Parent1LastName);
            return View(parents);
        }

        public ActionResult SearchParent()
        {
            return View();
        }

        public ActionResult SearchParentResults(string firstname, string lastname,string email)
        {
            IEnumerable<Parent> parents = new List<Parent>();
            parents = dbContext.Parents;
            if(!String.IsNullOrWhiteSpace(firstname))
            {
                parents = dbContext.Parents.Where(p => p.Parent1Firstname.ToLower() == firstname.Trim().ToLower() || (p.Parent2Firstname!=null && p.Parent2Firstname.ToLower() == firstname.Trim().ToLower()));
            }
            if (parents.Count() > 0 && !String.IsNullOrWhiteSpace(lastname))
            {
                parents = parents.Where(p => p.Parent1LastName.ToLower() == lastname.Trim().ToLower() || (p.Parent2Lastname!=null && p.Parent2Lastname.ToLower() == lastname.Trim().ToLower()));
            }
            if (parents.Count() >0 && !String.IsNullOrWhiteSpace(email))
            {
                parents = parents.Where(p => p.PrimaryEmail.ToLower() == email.Trim().ToLower() || (p.SecondaryEmail!=null && p.SecondaryEmail.ToLower() == email.Trim().ToLower()));
            }
            return View(parents);
        }

        public int GetBookChargeCount(int id)
        {
            IEnumerable<Transaction> bookCharges = dbContext.Transactions.Where(t => t.SemesterID == currentSemesterId && t.ParentsId == id && t.TransactionDescription=="Book Charge" && t.ActiveFlg);
            int bookChargeCount = 0;
            if (bookCharges==null)
            {
                bookChargeCount = 0;
            }else
            {
                bookChargeCount = bookCharges.Count();
            }
            IEnumerable<Student> students = GetRegistedStudents(id);
            int stuCount = 0;
            if (students==null)
            {
                stuCount = 0;
            }else
            {
                stuCount = students.Count();
            }
            if (stuCount>bookChargeCount)
            {
                return bookChargeCount;
            }else
            {
                return -1;
            }
        }

        public ActionResult Details(int? parentId)
        {
            ChineseSchoolEntities dbContext1 = new ChineseSchoolEntities();
            decimal amountDue = 0;
            if (!parentId.HasValue){
                return HttpNotFound("Invalid parent ID");
            }
            int id = parentId.Value;
            ParentDetailsViewModel vm = new ParentDetailsViewModel();
            Parent parent = dbContext1.Parents.AsNoTracking().FirstOrDefault(p => p.ParentsId == id);
            if (parent == null)
            {
                return HttpNotFound("Invalid parent ID");
            }
            string userId = parent.UserId;
            ApplicationUser user = UserManager.FindById(parent.UserId);
            if (dbContext1.Transactions.Where(t => t.ActiveFlg && t.UserID == user.Id && t.SemesterID == currentSemesterId) == null || dbContext1.Transactions.Where(t => t.ActiveFlg && t.UserID == user.Id && t.SemesterID == currentSemesterId).Count() == 0)
            {
                amountDue = 0;
            }
            else
            {
                amountDue = dbContext1.Transactions.Where(t => t.ActiveFlg && t.UserID == user.Id && t.SemesterID==currentSemesterId).Sum(t => t.TransactionAmount);
            }
            
            vm.User = user;
            vm.Parent = parent;
            vm.AmountDue = amountDue;
            vm.Students = vm.Parent.Students;
            foreach (Student stu in vm.Students)
            {
                if (stu.ClassRegistrations == null || stu.ClassRegistrations.FirstOrDefault(c => c.SemesterId == currentSemesterId) == null)
                {
                    stu.RegisterClass = false;
                    stu.Class = null;
                    
                }
                else
                {
                    stu.RegisterClass = true;
                    stu.Class = stu.ClassRegistrations.FirstOrDefault(c => c.SemesterId == currentSemesterId).Class == null ? "Not Assigned" : stu.ClassRegistrations.FirstOrDefault(c => c.SemesterId == currentSemesterId).Class.Classname;
                }

                if (stu.EnrichmentClassRegistrations==null || stu.EnrichmentClassRegistrations.FirstOrDefault(e=>e.SemesterId==currentSemesterId && e.ActiveFlag)==null)
                {
                    stu.EnrichmentClass = "N/A";
                }
                else
                {
                    stu.EnrichmentClass = stu.EnrichmentClassRegistrations.FirstOrDefault(e=>e.SemesterId==currentSemesterId && e.ActiveFlag).EnrichmentClass.ClassName;
                }
            }
            vm.Transactions = dbContext1.Transactions.Where(t => t.UserID == userId && t.SemesterID==currentSemesterId);
            ViewBag.currentTab = "Details";
            return View(vm);
            
        }

        public ActionResult Edit(int? parentId)
        {
            if (!parentId.HasValue)
            {
                return HttpNotFound("Invalid parent ID");
            }
            int id = parentId.Value;
            ParentDetailsViewModel vm = new ParentDetailsViewModel();
            Parent parent = dbContext.Parents.FirstOrDefault(p => p.ParentsId == id);
            if (parent == null)
            {
                return HttpNotFound("Invalid parent ID");
            }
            ViewBag.currentTab = "Edit";
            return View(parent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Parent parent)
        {
            if (ModelState.IsValid)
            {
                Parent par = dbContext.Parents.FirstOrDefault(p => p.ParentsId == parent.ParentsId);
                par.Parent1Firstname = parent.Parent1Firstname;
                par.Parent1LastName = parent.Parent1LastName;
                par.Parent2Firstname = parent.Parent2Firstname;
                par.Parent2Lastname = parent.Parent2Lastname;
                par.PrimaryPhone = parent.PrimaryPhone;
                par.SecondaryEmail = parent.SecondaryEmail;
                par.SecondaryPhone = parent.SecondaryPhone;
                par.AddressLine1 = parent.AddressLine1;
                par.AddressLine2 = parent.AddressLine2;
                par.City = parent.City;
                par.State = parent.State;
                par.ZipCode = parent.ZipCode;
                par.UpdateTimeStemp=DateTime.Now;
                par.UpdateUserId = User.Identity.GetUserId();
                dbContext.SaveChanges();
                return RedirectToAction("Details", new { parentId = par.ParentsId });
                 
            }
            return View();
        }

        public ActionResult SearchInvoice()
        {
            return View();
        }

        public ActionResult SearchInvoiceResults(string invoiceNo)
        {
            if (ModelState.IsValid)
            {
                Invoice inv = dbContext.Invoices.FirstOrDefault(i => i.InvoiceNo == invoiceNo);
                if (inv == null)
                {
                    TempData["Message"] = "Your Search Returns No Result, Please Try Again";
                    return View("SearchInvoice");
                }
                return RedirectToAction("Details",new {controller="Parents",area="Admin",parentId=inv.ParentId });
            }
            return View("SearchInvoice");
        }

        public ActionResult Transaction(int? parentId)
        {
            if (!parentId.HasValue)
            {
                return HttpNotFound("Invalid parentId");
            }
            int id = parentId.Value;
            Transaction tran = new Transaction();
            Parent parent = dbContext.Parents.FirstOrDefault(p => p.ParentsId == id);
            if (parent == null)
            {
                return HttpNotFound("Invalid ParentId");
            }
            tran.UserID = parent.UserId;
            tran.TransactionDate = DateTime.Now;
            return View(tran);
        }

        public ActionResult PayInvoice(int? parentId)
        {
            if (!parentId.HasValue)
            {
                return HttpNotFound("Invalid parentId");
            }
            int id = parentId.Value;
            Transaction tran = new Transaction();
            Parent parent = dbContext.Parents.FirstOrDefault(p => p.ParentsId == id);
            if (parent == null)
            {
                return HttpNotFound("Invalid ParentId");
            }
            Invoice inv = dbContext.Invoices.AsNoTracking().FirstOrDefault(i => i.ParentId == parent.ParentsId && i.SemesterId == currentSemesterId);
            tran.UserID = parent.UserId;
            tran.TransactionDate = DateTime.Now;
            tran.TransactionAmount = inv.Amount;
            tran.TransactionType = "Payment";
            tran.TransactionDescription = "Payment for: " + inv.InvoiceNo;
            return View("Transaction",tran);
        }
        [HttpPost]
        public ActionResult ApplyBookCharge(int? id)
        {
            ChineseSchoolEntities dbContext1 = new ChineseSchoolEntities();
            if (!id.HasValue)
            {
                return HttpNotFound("Invalid Parent Id!");
            }
            Semester currentSemester = dbContext1.Semesters.FirstOrDefault(s=>s.SemesterID==currentSemesterId);
            Transaction t = new Transaction();
            t.ParentsId = id.Value;
            t.SemesterID = currentSemesterId;
            t.TransactionAmount = currentSemester.BookCharge;
            t.TransactionDescription = "Book Charge";
            t.TransactionType = "Charge";
            t.TransactionDate = DateTime.Now;
            t.UpdateTimeStemp = DateTime.Now;
            t.UpdateUserId = User.Identity.GetUserId();
            t.CreateUserId = User.Identity.GetUserId();
            t.CreateTimeStemp = DateTime.Now;
            t.UserID = dbContext.Parents.FirstOrDefault(p => p.ParentsId == id.Value).UserId;
            t.ActiveFlg = true;
            dbContext1.Transactions.Add(t);
            dbContext1.SaveChanges();
            dbContext1.sp_updateinvoice(id);
            return RedirectToAction("Details",new {parentId = id.Value});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transaction(Transaction tran)
        {
            if (ModelState.IsValid)
            {
                if (tran.TransactionType == "Payment")
                {
                    tran.TransactionAmount = 0 - tran.TransactionAmount;
                }
                tran.ActiveFlg = true;
                tran.SemesterID = currentSemesterId;
                Parent parent = dbContext.Parents.FirstOrDefault(p => p.UserId == tran.UserID);
                tran.ParentsId = parent.ParentsId;
                
                tran.CreateTimeStemp = DateTime.Now;
                tran.CreateUserId = User.Identity.GetUserId();
                tran.UpdateTimeStemp = DateTime.Now;
                tran.UpdateUserId = User.Identity.GetUserId();
                dbContext.Transactions.Add(tran);
                dbContext.SaveChanges();
                dbContext.sp_updateinvoice(tran.ParentsId);
                if (tran.TransactionType.ToLower() == "payment")
                {
                    string msg = "A payment has been posted to your account:<br />";
                    msg = msg + "Transaction Amount: " + tran.TransactionAmount.ToString() + "<br />";
                    msg = msg + "Transaction Date: " + tran.TransactionDate.ToShortDateString() + "<br />";
                    msg = msg + "<br /> Please click <a href=\"https://www.chinesecenteroftoledo.org/myaccount to view the invoice\" ></a>";
                    msg = msg + "<br /><br />";
                    msg = msg + "Chinese Center of Toledo<br />";
                    msg = msg + "(419)450-9029<br /> ";
                    msg = msg + "http://chinesecenteroftoledo.org<br />";
                    MailSender sender = new MailSender("admin@chinesecenteroftoledo.org", "7348@Iris");
                    sender.SendMail(parent.PrimaryEmail, "A payment has been posted to your account", msg);
                }
                string m = "A payment has been posted <br />";
                m = m + "<table><tr><th>First Name</th><th>Last Name</th><th>Payment Date</th><th>Payment type</th><th>Payment Description</th><th>Processing User</th></tr>";
                m = m + "<tr><td>" + parent.Parent1Firstname + "</td><td>" + parent.Parent1LastName + "</td><td>" + tran.TransactionDate.ToShortDateString() + "</td><td>" + tran.TransactionType + "</td><td>" + tran.TransactionDescription + "</td><td>" + User.Identity.GetUserName() + "</td></tr></table>";
                m = m + "<br /><br />";
                m = m + "Chinese Center of Toledo<br />";
                m = m + "(419)450-9029<br /> ";
                m = m + "http://chinesecenteroftoledo.org<br />";
                MailSender s = new MailSender("admin@chinesecenteroftoledo.org", "7348@Iris");
                s.SendMail("registration@chinesecenteroftoledo.org", "A payment has been posted", m);
                return RedirectToAction("Details", new { parentId = tran.ParentsId });
            }

            return View();
        }

        public ActionResult ExcelTest()
        {
            
            return HttpNotFound("Excel file generated!");
        }

        public ActionResult ParentsNavBar( int parentId, string currentTab)
        {
            Parent p = dbContext.Parents.AsNoTracking().FirstOrDefault(a => a.ParentsId == parentId && !a.Deleted);
            ViewBag.currentTab = currentTab;
            return PartialView("_ParentsNavBar", p);
        }

        public FileStreamResult DownloadExcel()
        {
            string name = "ChineseSchoolRegistration_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xlsx";
            string fileName = System.IO.Path.Combine(Server.MapPath("~/Content"), name);
            GenerateExcel(fileName);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Dispostion", @"attachment; filename =""" + name + @"""");
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        private List<Student> GetRegistedStudents(int id)
        {
            List<Student> registedStudents = new List<Student>();
            List<Student> students = dbContext.Students.Where(s => s.ParentsId == id && !s.Deleted).ToList();
            foreach(var stu in students)
            {
                ClassRegistration reg = dbContext.ClassRegistrations.FirstOrDefault(r => r.StudentId == stu.StudentId && r.SemesterId == currentSemesterId && r.ActiveFlag);
                if (reg!=null)
                {
                    registedStudents.Add(stu);
                }
            }
            return registedStudents;
        }

        private void GenerateExcel(string fileName)
        {
            List<string> title = new List<string>() { "Student Name", "Parent Name",  "Birthday","phone","Email", "Chinese Class", "Enrichment Class" };
            List<List<string>> data = new List<List<string>>();
            List<string> record = null;
            List<v_CurrentRegistrationDetails> details = dbContext.v_CurrentRegistrationDetails.OrderBy(p=>p.StudentName).ToList();
            foreach(var detail in details)
            {
                record = new List<string>() { detail.StudentName, detail.ParentName, detail.Birthday, detail.Phone, detail.Email, detail.Classname, detail.EnrichmentClassname };
                data.Add(record);
            }
            //string fileName = System.IO.Path.Combine(Server.MapPath("~/Content"), "ChineseSchoolRegistration_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xlsx");
            ExcelWriter.WriteExcel(fileName, "All", title, data);
            IEnumerable<Class> classes = dbContext.Classes.Where(cl => cl.ActiveFlg);
            foreach(var c in classes)
            {
                List<v_CurrentRegistrationDetails> records = dbContext.v_CurrentRegistrationDetails.Where(cu => cu.Classname == c.Classname).OrderBy(p => p.StudentName).ToList();
                data = new List<List<string>>();
                foreach(var detail in records)
                {
                    record = new List<string>() { detail.StudentName, detail.ParentName, detail.Birthday, detail.Phone, detail.Email, detail.Classname, detail.EnrichmentClassname };
                    data.Add(record);
                }
                ExcelWriter.WriteExcel(fileName, c.Classname, title, data);
            }
            IEnumerable<EnrichmentClass> eClasses = dbContext.EnrichmentClasses.Where(e => e.ActiveFlg);
            foreach(var ec in eClasses)
            {
                List<v_CurrentRegistrationDetails> records = dbContext.v_CurrentRegistrationDetails.Where(cu => cu.EnrichmentClassname == ec.ClassName).OrderBy(p => p.StudentName).ToList();
                data = new List<List<string>>();
                foreach (var detail in records)
                {
                    record = new List<string>() { detail.StudentName, detail.ParentName, detail.Birthday, detail.Phone, detail.Email, detail.Classname, detail.EnrichmentClassname };
                    data.Add(record);
                }
                ExcelWriter.WriteExcel(fileName, ec.ClassName, title, data);
            }
        }
	}
}