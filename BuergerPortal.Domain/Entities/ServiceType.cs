using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Domain.Entities
{
    public class ServiceType
    {
        public int ServiceTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string ServiceName { get; set; }

        [Required]
        [StringLength(10)]
        public string ServiceCode { get; set; }

        public ServiceCategory Category { get; set; }

        public decimal BaseFee { get; set; }

        public int ProcessingDays { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool RequiresInPersonVisit { get; set; }

        public virtual ICollection<ServiceApplication> Applications { get; set; }
        public virtual ICollection<FeeSchedule> FeeSchedules { get; set; }

        public ServiceType()
        {
            Applications = new List<ServiceApplication>();
            FeeSchedules = new List<FeeSchedule>();
        }
    }
}
