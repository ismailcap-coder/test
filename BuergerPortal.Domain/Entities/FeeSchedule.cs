using System;
using System.ComponentModel.DataAnnotations;

namespace BuergerPortal.Domain.Entities
{
    public class FeeSchedule
    {
        public int FeeScheduleId { get; set; }

        public int ServiceTypeId { get; set; }

        [Required]
        [StringLength(10)]
        public string DistrictCode { get; set; }

        public decimal AdjustedBaseFee { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public virtual ServiceType ServiceType { get; set; }
    }
}
