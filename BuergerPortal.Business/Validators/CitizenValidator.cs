using System;
using System.Text.RegularExpressions;
using BuergerPortal.Domain.Entities;

namespace BuergerPortal.Business.Validators
{
    public class CitizenValidator
    {
        private static readonly Regex TaxIdPattern = new Regex(@"^\d{11}$");
        private static readonly Regex PostalCodePattern = new Regex(@"^\d{5}$");
        private static readonly Regex EmailPattern = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public ValidationResult Validate(Citizen citizen)
        {
            var result = new ValidationResult();

            if (citizen == null)
            {
                result.AddError("Citizen cannot be null.");
                return result;
            }

            // First name validation
            if (string.IsNullOrEmpty(citizen.FirstName))
            {
                result.AddError("First name is required.");
            }
            else if (citizen.FirstName.Length > 50)
            {
                result.AddError("First name cannot exceed 50 characters.");
            }

            // Last name validation
            if (string.IsNullOrEmpty(citizen.LastName))
            {
                result.AddError("Last name is required.");
            }
            else if (citizen.LastName.Length > 50)
            {
                result.AddError("Last name cannot exceed 50 characters.");
            }

            // Date of birth validation
            if (citizen.DateOfBirth == default(DateTime))
            {
                result.AddError("Date of birth is required.");
            }
            else
            {
                int age = CalculateAge(citizen.DateOfBirth);
                if (age < 0 || age > 150)
                {
                    result.AddError("Date of birth is not valid.");
                }
                if (citizen.DateOfBirth > DateTime.Today)
                {
                    result.AddError("Date of birth cannot be in the future.");
                }
            }

            // Tax ID validation (German Steuerliche Identifikationsnummer: 11 digits)
            if (!string.IsNullOrEmpty(citizen.TaxId))
            {
                if (!TaxIdPattern.IsMatch(citizen.TaxId))
                {
                    result.AddError("Tax ID must be exactly 11 digits (German Steuerliche Identifikationsnummer).");
                }
            }
            else
            {
                result.AddError("Tax ID is required.");
            }

            // Address validation
            if (string.IsNullOrEmpty(citizen.StreetAddress))
            {
                result.AddError("Street address is required.");
            }

            if (string.IsNullOrEmpty(citizen.City))
            {
                result.AddError("City is required.");
            }

            // Postal code validation (German format: 5 digits)
            if (!string.IsNullOrEmpty(citizen.PostalCode))
            {
                if (!PostalCodePattern.IsMatch(citizen.PostalCode))
                {
                    result.AddError("Postal code must be exactly 5 digits (German format).");
                }
            }
            else
            {
                result.AddError("Postal code is required.");
            }

            // Email validation
            if (!string.IsNullOrEmpty(citizen.Email))
            {
                if (!EmailPattern.IsMatch(citizen.Email))
                {
                    result.AddError("Email address is not valid.");
                }
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
