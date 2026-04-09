using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuergerPortal.Domain.Entities
{
    public class PublicOffice
    {
        public int OfficeId { get; set; }

        [Required]
        [StringLength(100)]
        public string OfficeName { get; set; }

        [Required]
        [StringLength(10)]
        public string DistrictCode { get; set; }

        [StringLength(100)]
        public string StreetAddress { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        public decimal DistrictMultiplier { get; set; }

        public virtual ICollection<ServiceApplication> Applications { get; set; }
        public virtual ICollection<FeeSchedule> FeeSchedules { get; set; }

        public PublicOffice()
        {
            Applications = new List<ServiceApplication>();
            FeeSchedules = new List<FeeSchedule>();
            DistrictMultiplier = 1.0m;
        }
    }
}
