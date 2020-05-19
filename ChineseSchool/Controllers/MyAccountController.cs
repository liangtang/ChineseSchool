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
using PayPal;
using PayPal.Api;

namespace ChineseSchool.Controllers
{
    

    [Authorize]
    public class MyAccountController : Controller
    {
        private static  ChineseSchoolEntities dbContext = new ChineseSchoolEntities();
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

        public ActionResult Index()
        {
            DashboardViewModel vm = new DashboardViewModel();
            ApplicationUser user = UserManager.FindByName(User.Identity.Name);
            decimal amountDue;
            if (dbContext.Transactions.AsNoTracking().Where(t => t.ActiveFlg && t.UserID == user.Id).Count() > 0)
            {
                amountDue = dbContext.Transactions.Where(t => t.ActiveFlg && t.UserID==user.Id && t.SemesterID == currentSemesterId).Sum(t => t.TransactionAmount);
            }
            else
            {
                amountDue = 0;
            }
            
            string userId = User.Identity.GetUserId();
            vm.User = user;
            vm.Parent = dbContext.Parents.AsNoTracking().FirstOrDefault(p => p.UserId == userId);
            vm.AmountDue = amountDue;
            if (vm.Parent != null)
            {
                vm.Students = vm.Parent.Students;
                foreach (Student stu in vm.Students)
                {
                    if (stu.ClassRegistrations == null || stu.ClassRegistrations.FirstOrDefault(c => c.SemesterId == currentSemesterId) == null)
                    {
                        stu.RegisterClass = false;
                    }
                    else
                    {
                        stu.RegisterClass = true;
                        if (stu.ClassRegistrations.FirstOrDefault(c => c.SemesterId == currentSemesterId).ClassId != null)
                        {
                            stu.Class = stu.ClassRegistrations.FirstOrDefault(c => c.SemesterId == currentSemesterId).Class.Classname;
                        }
                    }


                }
            }
            else
            {
                vm.Students = null;
            }
            ViewBag.currentTab = "DashBoard";
            ViewBag.currentSemesterId = currentSemesterId;
            return View(vm);
        }

        public ActionResult ChangePassword()
        {
            ViewBag.currentTab = "ChangePassword";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = UserManager.ChangePassword(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Password Changed Successfully!";
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return View(model);
        }

        public ActionResult EditProfile()
        {
            ApplicationUser user = UserManager.FindByName(User.Identity.Name);
            if (user == null)
            {
                return new HttpNotFoundResult("User Not Found!");
            }
            ViewBag.currentTab = "EditProfile";

            return View(new EditProfileViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = UserManager.FindById(model.Id);
                if (user == null)
                {
                    return new HttpNotFoundResult("User Not Found!");
                }
                if (User.Identity.Name != user.UserName)
                {
                    return View("Error", new string[] { "Not Authorized!" });
                }
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                IdentityResult validateEmailResult = await UserManager.UserValidator.ValidateAsync(user);
                if (!validateEmailResult.Succeeded)
                {
                    foreach (var error in validateEmailResult.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(model);
                }
                IdentityResult result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Profile Updated Successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return View();
        }

        public ActionResult ParentsInfo()
        {
            ApplicationUser user = UserManager.FindByName(User.Identity.Name);
            if (user == null)
            {
                return new HttpNotFoundResult("User Not Found!");
            }
            Parent parent = dbContext.Parents.FirstOrDefault(p => p.UserId == user.Id);
            if (parent == null)
            {
                parent = new Parent() { UserId = user.Id, Parent1Firstname = user.Firstname, Parent1LastName = user.Lastname, PrimaryEmail = user.Email };
            }
            ViewBag.currentTab = "ParentInfo";
            return View(parent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ParentsInfo(Parent parent)
        {
            if (parent.UserId != User.Identity.GetUserId())
            {
                return View("Error", new string[] { "Not Authorized" });
            }
            if (ModelState.IsValid)
            {
                if (parent.ParentsId == null || parent.ParentsId==0)
                {


                    parent.CreateTimeStemp = DateTime.Now;
                    parent.CreateUserId = User.Identity.GetUserId();
                    parent.UpdateTimeStemp = DateTime.Now;
                    parent.UpdateUserId = User.Identity.GetUserId();
                    dbContext.Parents.Add(parent);
                    dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                Parent par = dbContext.Parents.FirstOrDefault(p => p.ParentsId == parent.ParentsId);
                if(par!=null)
                {
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
                    par.UpdateUserId = User.Identity.GetUserId();
                    par.UpdateTimeStemp = DateTime.Now;
                    dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.currentTab = "ParentInfo";
            return View(parent);
        }

        public ActionResult AddStudents()
        {
            string userId = User.Identity.GetUserId();
            Parent p = dbContext.Parents.FirstOrDefault(par => par.UserId == userId && !par.Deleted);
            if (p==null)
            {
                TempData["Message"] = "Parent info is empty, Please Add Parent info Before Proceed............";
                return RedirectToAction("ParentsInfo");
            }
            

            AddStudentViewModel vm = new AddStudentViewModel();
            vm.Students = new List<StudentViewModel>();
            vm.Classes = dbContext.Classes.Where(c => c.ActiveFlg).ToList();
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.Where(e => e.ActiveFlg).ToList();
            ViewBag.currentTab = "Add Students";
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult AddStudents(AddStudentViewModel vm)
        {
            CheckStudents(vm.Students);
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                int parentId = dbContext.Parents.FirstOrDefault(p => p.UserId == userId).ParentsId;
                using (System.Data.Entity.DbContextTransaction dbTran = dbContext.Database.BeginTransaction())
                {
                    int stuCnt = 0;
                    foreach (var student in vm.Students)
                    {
                        stuCnt++;

                        Student stu = new Student();
                        stu.ParentsId = dbContext.Parents.FirstOrDefault(p => p.UserId == userId).ParentsId;
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
                        stu.EnrichmentClass = student.EnrichmentClass == 0 ? null : dbContext.EnrichmentClasses.FirstOrDefault(c => c.ClassID == student.EnrichmentClass).ClassName;

                        dbContext.Students.Add(stu);
                        dbContext.SaveChanges();


                        dbContext.RegisterStudent(stu.StudentId, stu.RegisterClass, null, student.EnrichmentClass, User.Identity.GetUserId(), DateTime.Now);
                    }
                    dbContext.sp_updateinvoice(parentId);
                    dbTran.Commit();
                    return RedirectToAction("Index");
                }
            }
            vm.Classes = dbContext.Classes.Where(c => c.ActiveFlg).ToList();
            vm.EnrichmentClasses = dbContext.EnrichmentClasses.Where(e => e.ActiveFlg).ToList();
            ViewBag.currentTab = "Add Students";        
            return View(vm);
        }

        public ActionResult Invoice()
        {
            string userId = User.Identity.GetUserId();
            int parentId = dbContext.Parents.FirstOrDefault(p => p.UserId == userId).ParentsId;
            List<ChineseSchool.Entities.Transaction> trans = dbContext.Transactions.Where(t => t.UserID == userId && t.ActiveFlg && t.SemesterID == currentSemesterId).ToList();
            InvoiceViewModel vm = new InvoiceViewModel();
            vm.Invoice = dbContext.Invoices.FirstOrDefault(i => i.ParentId == parentId && i.SemesterId == currentSemesterId);
            vm.Transactions = trans;
            ViewBag.currentTab = "Invoice";
            return View(vm);
        }


        public ActionResult PrintInvoice(int? invoiceId)
        {
            if(!invoiceId.HasValue)
            {
                return HttpNotFound("Invalid Invoice Id");
            }
            ChineseSchool.Entities.Invoice invoice = dbContext.Invoices.FirstOrDefault(i => i.InvoiceId == invoiceId.Value);
            if (invoice == null)
            {
                return HttpNotFound("Invalid Invoice Id");
            }
            InvoiceViewModel vm = new InvoiceViewModel();
            vm.Invoice = invoice;
            vm.Parent = dbContext.Parents.FirstOrDefault(p => p.ParentsId == invoice.ParentId);
            vm.Transactions = dbContext.Transactions.Where(t => t.ParentsId == vm.Parent.ParentsId && t.SemesterID == currentSemesterId && t.ActiveFlg);
            return View(vm);
        }

        private void CheckStudents(List<StudentViewModel> students)
        {
            string userID = User.Identity.GetUserId();
            List<Student> stus = dbContext.Students.Where(s => s.Parent.UserId == userID).ToList();
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
                if (stus.FirstOrDefault(s => s.Firstname == student.Firstname && s.Lastname == student.Lastname) != null)
                {
                    ModelState.AddModelError("Students", "Student Already Exists");
                }
              
            }
        }

        public ActionResult MyAccountNavBar(string currentTab = null)
        {
            ViewBag.currentTab = currentTab;

            return PartialView("_MyAccountNavBar");
        }


        public ActionResult Payment()
        {
           /***********NOT IMPLEMENTED************************
            string userId = User.Identity.GetUserId();
            int parentId = dbContext.Parents.FirstOrDefault(p => p.UserId == userId).ParentsId;
            dbContext.sp_updateinvoice(parentId);
            ChineseSchool.Entities.Invoice invoice = dbContext.Invoices.FirstOrDefault(i => i.ParentId == parentId);

            if (invoice ==null || invoice.Amount<1)
            {
                TempData["Message"] =  "No payment needed" ;
                return RedirectToAction("Index");
            }

            var apiContext = GetApiContext();

            var payment = new Payment
            {
                experience_profile_id = "XP-5D6N-MAMR-C3Y2-QPCX",
                intent = "sale",
                payer = new Payer
                {
                    payment_method = "paypal"
                },
                transactions = new List<PayPal.Api.Transaction>
                {
                    new PayPal.Api.Transaction
                    {
                        description = "Chinese school tuition for: " + User.Identity.GetUserName() + "-" + DateTime.Now.ToString(),
                        amount = new Amount
                        {
                            currency = "USD",
                            total = invoice.Amount.ToString()
                        },
                        item_list = new ItemList()
                        {
                            items = new List<Item>()
                            {
                                new Item()
                                {
                                    description = "Chinese school tuition for: " + User.Identity.GetUserName(),
                                    currency = "USD",
                                    quantity = "1",
                                    price = invoice.Amount.ToString()
                                }
                            }
                        }
                    }
                },

                redirect_urls = new RedirectUrls
                {
                    return_url = "https://registration.chinesecenteroftoledo.org/MyAccount/return",
                    cancel_url = "https://registration.chinesecenteroftoledo.org/MyAccount/cancel"
                },
            };

            var createdPayment = payment.Create(apiContext);

            var transaction = new ChineseSchool.Entities.Transaction()
            {
                UserID = User.Identity.GetUserId(),
                ParentsId = parentId,
                TransactionType = "Payment",
                TransactionDate = DateTime.Now,
                TransactionDescription = createdPayment.id,
                TransactionAmount = -invoice.Amount,
                CreateTimeStemp = DateTime.Now,
                CreateUserId = User.Identity.GetUserId(),
                ActiveFlg = false,
                UpdateTimeStemp = DateTime.Now,
                UpdateUserId = User.Identity.GetUserId(),
                SemesterID =currentSemesterId

            };
            dbContext.Transactions.Add(transaction);
            dbContext.SaveChanges();

            var approvalUrl = createdPayment.links.FirstOrDefault(x => x.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase));
            return Redirect(approvalUrl.href);
            
        }

        public ActionResult Return(string payerId, string paymentId)
        {
            var apiContext = GetApiContext();
            string userId = User.Identity.GetUserId();

            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };

            var payment = new Payment()
            {
                id = paymentId
            };
            var executedPayment = payment.Execute(apiContext, paymentExecution);
            //try
            //{
                
            //}
            //catch (PayPalException ex)
            //{
            //    if (ex.InnerException is ConnectionException)
            //    {
            //        Console.Write(((ConnectionException)ex.InnerException).Response);
            //    }
            //    else
            //    {
            //        Console.Write(ex.Message);
            //    }
            //}





            var transaction = dbContext.Transactions.FirstOrDefault(t => t.UserID == userId && t.SemesterID == currentSemesterId && t.TransactionDescription == paymentId);
            transaction.ActiveFlg = true;
            dbContext.SaveChanges();
            dbContext.sp_updateinvoice(transaction.ParentsId);
            return RedirectToAction("Thankyou");
            ****************************************************************************/
            return HttpNotFound("This is currently disabled.......");
        }

        public ActionResult Thankyou()
        {
            return View();
        }
        public ActionResult Cancel()
        {
            return View();
        }

        private APIContext GetApiContext()
        {
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);
            return apiContext;
        }

	}
}