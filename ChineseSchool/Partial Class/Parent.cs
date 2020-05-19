using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChineseSchool.Entities
{
    [MetadataTypeAttribute(typeof(Parent.ParentMetadata))]
    public partial class Parent
    {
        internal sealed class ParentMetadata
        {
            public int ParentsId { get; set; }
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
            public System.DateTime CreateTimeStemp { get; set; }
            public string CreateUserId { get; set; }
            public System.DateTime UpdateTimeStemp { get; set; }
            public string UpdateUserId { get; set; }
        }
    }
}