using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Web.ViewModels
{
    public class ApplicationViewModel
    {
        public int ApplicationId { get; set; }

        [Display(Name = "Application Number")]
        public string? ApplicationNumber { get; set; }

        [Required(ErrorMessage = "Citizen is required.")]
        [Display(Name = "Citizen")]
        public int CitizenId { get; set; }

        [Required(ErrorMessage = "Service type is required.")]
        [Display(Name = "Service Type")]
        public int ServiceTypeId { get; set; }

        [Required(ErrorMessage = "Office is required.")]
        [Display(Name = "Public Office")]
        public int OfficeId { get; set; }

        [Display(Name = "Status")]
        public ApplicationStatus Status { get; set; }

        [Display(Name = "Submission Date")]
        public DateTime? SubmissionDate { get; set; }

        [Display(Name = "Review Date")]
        public DateTime? ReviewDate { get; set; }

        [Display(Name = "Completion Date")]
        public DateTime? CompletionDate { get; set; }

        [Display(Name = "Deadline Date")]
        public DateTime? DeadlineDate { get; set; }

        [Display(Name = "Express Processing")]
        public bool IsExpressProcessing { get; set; }

        [Display(Name = "Calculated Fee")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal CalculatedFee { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        [Display(Name = "Rejection Reason")]
        [DataType(DataType.MultilineText)]
        public string? RejectionReason { get; set; }

        // Display properties
        public string? CitizenName { get; set; }
        public string? ServiceTypeName { get; set; }
        public string? OfficeName { get; set; }

        // Dropdown lists
        public IEnumerable<SelectListItem>? Citizens { get; set; }
        public IEnumerable<SelectListItem>? ServiceTypes { get; set; }
        public IEnumerable<SelectListItem>? Offices { get; set; }
    }
}
