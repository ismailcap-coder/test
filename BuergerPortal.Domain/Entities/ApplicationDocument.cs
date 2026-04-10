using System;
using System.ComponentModel.DataAnnotations;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Domain.Entities
{
    public class ApplicationDocument
    {
        public int DocumentId { get; set; }

        public int ApplicationId { get; set; }

        [Required]
        [StringLength(200)]
        public string DocumentName { get; set; } = null!;

        public DocumentType DocumentType { get; set; }

        public DateTime UploadDate { get; set; }

        public bool IsVerified { get; set; }

        [StringLength(100)]
        public string? VerifiedBy { get; set; }

        public virtual ServiceApplication Application { get; set; } = null!;
    }
}
