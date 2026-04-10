using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Web.ViewModels
{
    public class CitizenViewModel
    {
        public int CitizenId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Street address is required.")]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postal code is required.")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Postal code must be exactly 5 digits.")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Tax ID is required.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Tax ID must be exactly 11 digits.")]
        [Display(Name = "Tax ID")]
        public string TaxId { get; set; } = string.Empty;

        [Display(Name = "Low Income")]
        public bool IsLowIncome { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public int ApplicationCount { get; set; }
    }
}
