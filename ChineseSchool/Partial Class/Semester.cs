using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChineseSchool.Entities
{
    [MetadataTypeAttribute(typeof(Semester.ClassMetadata))]
    public partial class Semester
    {
        internal sealed class ClassMetadata
        {
            [Required]
            [Display(Name="Year")]
            [Range(2017,2030,ErrorMessage = "Incorrect Year!")]
            public int SemesterYear { get; set; }
            [Required]
            public string SemesterSeason { get; set; }
            [Required]
            [Display(Name="Registration Start Date")]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
            public System.DateTime RegisterStartDate { get; set; }
            [Required]
            [Display(Name="Registration End Date")]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
            public System.DateTime RegisterEndDate { get; set; }
            [Required]
            [Display(Name="First Child Tuition")]
            public decimal Price1 { get; set; }
            
            public string Price1Description { get; set; }
            [Required]
            [Display(Name="Second Child Tuition")]
            public decimal Price2 { get; set; }

            public string Price2Description { get; set; }
            [Required]
            [Display(Name="Third Child Tuition")]
            public decimal Price3 { get; set; }
            public string Price3Description { get; set; }
            [Required]
            [Display(Name="Fourth Child Tuition")]
            public Nullable<decimal> Price4 { get; set; }
            public string Price4Description { get; set; }
            [Required]
            [Display(Name="Fifth Child Tuition")]
            public Nullable<decimal> Price5 { get; set; }
            public string Price5Description { get; set; }
            public bool ActiveFlg { get; set; }
            public System.DateTime CreateTimeStemp { get; set; }
            public string CreateUserId { get; set; }
            public System.DateTime UpdateTimeStemp { get; set; }
            public string UpdateUserId { get; set; }
            [Required]
            [Display(Name="Registration Fee before End Date")]
            public decimal RegistrationFeeBeforeEndDate { get; set; }
            [Required]
            [Display(Name="Registration Fee after End Date")]
            public decimal RegistrationFeeAfterEndDate { get; set; }
            [Required]
            public decimal VolunteerDeposit { get; set; }
        }

    }
}