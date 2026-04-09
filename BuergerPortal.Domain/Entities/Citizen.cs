using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuergerPortal.Domain.Entities
{
    public class Citizen
    {
        public int CitizenId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        [StringLength(100)]
        public string StreetAddress { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(5)]
        public string PostalCode { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(11)]
        public string TaxId { get; set; }

        public bool IsLowIncome { get; set; }

        public DateTime RegistrationDate { get; set; }

        public virtual ICollection<ServiceApplication> Applications { get; set; }

        public Citizen()
        {
            Applications = new List<ServiceApplication>();
        }

        public string FullName
        {
            get { return LastName + ", " + FirstName; }
        }
    }
}
