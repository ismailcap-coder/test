using System;
using System.ComponentModel.DataAnnotations;

namespace BuergerPortal.Domain.Entities
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }

        public int ApplicationId { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; }

        public int? PreviousStatus { get; set; }

        public int? NewStatus { get; set; }

        [Required]
        [StringLength(100)]
        public string PerformedBy { get; set; }

        public DateTime Timestamp { get; set; }

        [StringLength(500)]
        public string Details { get; set; }

        public virtual ServiceApplication Application { get; set; }
    }
}
