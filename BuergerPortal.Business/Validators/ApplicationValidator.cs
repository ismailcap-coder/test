using System;
using System.Collections.Generic;
using BuergerPortal.Domain.Entities;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Business.Validators
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }

        public ValidationResult()
        {
            IsValid = true;
            Errors = new List<string>();
        }

        public void AddError(string error)
        {
            IsValid = false;
            Errors.Add(error);
        }
    }

    public class ApplicationValidator
    {
        // Valid status transitions: Dictionary<FromStatus, AllowedToStatuses>
        private static readonly Dictionary<ApplicationStatus, ApplicationStatus[]> ValidTransitions =
            new Dictionary<ApplicationStatus, ApplicationStatus[]>
            {
                { ApplicationStatus.Draft, new[] { ApplicationStatus.Submitted } },
                { ApplicationStatus.Submitted, new[] { ApplicationStatus.UnderReview, ApplicationStatus.Rejected } },
                { ApplicationStatus.UnderReview, new[] { ApplicationStatus.Approved, ApplicationStatus.Rejected, ApplicationStatus.DocumentsRequested } },
                { ApplicationStatus.DocumentsRequested, new[] { ApplicationStatus.UnderReview, ApplicationStatus.Rejected } },
                { ApplicationStatus.Approved, new ApplicationStatus[0] },
                { ApplicationStatus.Rejected, new ApplicationStatus[0] }
            };

        public bool IsValidStatusTransition(ApplicationStatus fromStatus, ApplicationStatus toStatus)
        {
            if (!ValidTransitions.ContainsKey(fromStatus))
            {
                return false;
            }

            ApplicationStatus[] allowed = ValidTransitions[fromStatus];
            for (int i = 0; i < allowed.Length; i++)
            {
                if (allowed[i] == toStatus)
                {
                    return true;
                }
            }
            return false;
        }

        public ValidationResult ValidateNewApplication(ServiceApplication application, ServiceType serviceType, Citizen citizen)
        {
            var result = new ValidationResult();

            if (application.CitizenId <= 0)
            {
                result.AddError("Citizen is required.");
            }

            if (application.ServiceTypeId <= 0)
            {
                result.AddError("Service type is required.");
            }

            if (application.OfficeId <= 0)
            {
                result.AddError("Public office is required.");
            }

            if (string.IsNullOrEmpty(application.ApplicationNumber))
            {
                result.AddError("Application number is required.");
            }

            // Validate citizen age for certain services
            if (citizen != null && serviceType != null)
            {
                int citizenAge = CalculateAge(citizen.DateOfBirth);

                // Business license requires citizen to be at least 18
                if (serviceType.Category == ServiceCategory.License && citizenAge < 18)
                {
                    result.AddError("Citizen must be at least 18 years old to apply for a license.");
                }

                // Building permits require citizen to be at least 21
                if (serviceType.Category == ServiceCategory.Permit && serviceType.ServiceCode == "PER-001" && citizenAge < 21)
                {
                    result.AddError("Citizen must be at least 21 years old to apply for a building permit.");
                }

                // Marriage certificate requires citizen to be at least 16
                if (serviceType.Category == ServiceCategory.Certificate && serviceType.ServiceCode == "CRT-001" && citizenAge < 16)
                {
                    result.AddError("Citizen must be at least 16 years old to apply for a marriage certificate.");
                }
            }

            // Validate fee
            if (application.CalculatedFee < 0)
            {
                result.AddError("Calculated fee cannot be negative.");
            }

            return result;
        }

        public ValidationResult ValidateStatusChange(ServiceApplication application, ApplicationStatus newStatus)
        {
            var result = new ValidationResult();

            if (!IsValidStatusTransition(application.Status, newStatus))
            {
                result.AddError(string.Format(
                    "Invalid status transition from {0} to {1}.", application.Status, newStatus));
            }

            // Rejection requires a reason
            if (newStatus == ApplicationStatus.Rejected && string.IsNullOrEmpty(application.RejectionReason))
            {
                result.AddError("Rejection reason is required when rejecting an application.");
            }

            // Cannot approve without a review date
            if (newStatus == ApplicationStatus.Approved && application.ReviewDate == null)
            {
                result.AddError("Application must be reviewed before approval.");
            }

            return result;
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }
    }
}
