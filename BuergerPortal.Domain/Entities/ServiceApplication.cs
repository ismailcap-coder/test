using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Domain.Entities
{
    public class ServiceApplication
    {
        public int ApplicationId { get; set; }

        [StringLength(20)]
        public string ApplicationNumber { get; set; }

        public int CitizenId { get; set; }
        public int ServiceTypeId { get; set; }
        public int OfficeId { get; set; }

        public ApplicationStatus Status { get; set; }

        public DateTime? SubmissionDate { get; set; }
        public DateTime? ReviewDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? DeadlineDate { get; set; }

        public bool IsExpressProcessing { get; set; }

        public decimal CalculatedFee { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        [StringLength(500)]
        public string RejectionReason { get; set; }

        public virtual Citizen Citizen { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public virtual PublicOffice Office { get; set; }
        public virtual ICollection<ApplicationDocument> Documents { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; }

        public ServiceApplication()
        {
            Documents = new List<ApplicationDocument>();
            AuditLogs = new List<AuditLog>();
            Status = ApplicationStatus.Draft;
        }
    }
}
