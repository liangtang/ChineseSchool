using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ChineseSchool.Entities;

namespace ChineseSchool.Models
{
    public class NewRegisterViewModel
    {
        [Required]
        [Display(Name = "Parent 1 First name")]
        public string Parent1Firstname { get; set; }
        [Required]
        [Display(Name = "Parent 1 Last name")]
        public string Parent1LastName { get; set; }
        [Display(Name = "Parent 2 First name")]
        public string Parent2Firstname { get; set; }
        [Display(Name = "Parent 2 Last name")]
        public string Parent2Lastname { get; set; }
        [Display(Name = "Address Line 1")]

        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        [Required]
        [Display(Name = "Primary Phone")]
        public string PrimaryPhone { get; set; }
        [Display(Name = "Secondary Phone")]
        public string SecondaryPhone { get; set; }
        [Required]
        [Display(Name = "Primary Email")]
        public string PrimaryEmail { get; set; }
        [Display(Name = "Secondary Email")]
        public string SecondaryEmail { get; set; }

        public List<Student> Students { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterClassViewModel
    {
        [Required]
        [Display(Name = "Parent 1 First name")]
        public string Parent1Firstname { get; set; }
        [Required]
        [Display(Name = "Parent 1 Last name")]
        public string Parent1LastName { get; set; }
        [Display(Name = "Parent 2 First name")]
        public string Parent2Firstname { get; set; }
        [Display(Name = "Parent 2 Last name")]
        public string Parent2Lastname { get; set; }
        [Display(Name = "Address Line 1")]

        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        [Required]
        [Display(Name = "Primary Phone")]
        public string PrimaryPhone { get; set; }
        [Display(Name = "Secondary Phone")]
        public string SecondaryPhone { get; set; }
        [Required]
        [Display(Name = "Primary Email")]
        public string PrimaryEmail { get; set; }
        [Display(Name = "Secondary Email")]
        public string SecondaryEmail { get; set; }

        public List<StudentViewModel> Students { get; set; }
        public List<Class> Classes { get; set; }
        public List<EnrichmentClass> EnrichmentClasses { get; set; }
    }

    public class StudentViewModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string Chinesename { get; set; }
        public string Gender { get; set; }
        public string Birthday { get; set; }

        public bool IfRegister { get; set; }

        public int? Class { get; set; }

        
        public int EnrichmentClass { get; set; }
    }
}