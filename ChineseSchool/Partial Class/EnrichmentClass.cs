using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChineseSchool.Entities
{
    [MetadataTypeAttribute(typeof(EnrichmentClassRegistration.EnrichmentClassRegistrationMetadata))]
    public partial class EnrichmentClassRegistration
    {
        internal sealed class EnrichmentClassRegistrationMetadata
        {
            public int EnrichmentClassRegistrationId { get; set; }
            public int StudentId { get; set; }
            public int SemesterId { get; set; }
            public int EnrichmentClassId { get; set; }
            [Required]
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public bool ActiveFlag { get; set; }
            public System.DateTime CreateTimeStemp { get; set; }
            public string CreateUserId { get; set; }
            public System.DateTime UpdateTimeStemp { get; set; }
            public string UpdateUserId { get; set; }
        }
    }
}