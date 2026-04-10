using System;
using System.Collections.Generic;
using BuergerPortal.Business.Validators;
using BuergerPortal.Data.Repositories;
using BuergerPortal.Domain.Entities;

namespace BuergerPortal.Business.Services
{
    public class CitizenService : ICitizenService
    {
        private readonly CitizenRepository _citizenRepository;
        private readonly CitizenValidator _validator;

        public CitizenService(CitizenRepository citizenRepository, CitizenValidator validator)
        {
            _citizenRepository = citizenRepository;
            _validator = validator;
        }

        public Citizen GetCitizen(int citizenId)
        {
            var citizen = _citizenRepository.GetById(citizenId);
            if (citizen == null)
            {
                throw new InvalidOperationException(
                    string.Format("Citizen with ID {0} not found.", citizenId));
            }
            return citizen;
        }

        public Citizen GetCitizenWithApplications(int citizenId)
        {
            var citizen = _citizenRepository.GetByIdWithApplications(citizenId);
            if (citizen == null)
            {
                throw new InvalidOperationException(
                    string.Format("Citizen with ID {0} not found.", citizenId));
            }
            return citizen;
        }

        public Citizen? GetCitizenByTaxId(string taxId)
        {
            return _citizenRepository.GetByTaxId(taxId);
        }

        public IList<Citizen> GetAllCitizens()
        {
            return _citizenRepository.GetAll();
        }

        public IList<Citizen> SearchCitizens(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return _citizenRepository.GetAll();
            }
            return _citizenRepository.SearchByName(searchTerm);
        }

        public void CreateCitizen(Citizen citizen)
        {
            var validationResult = _validator.Validate(citizen);
            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException(
                    "Citizen validation failed: " + string.Join("; ", validationResult.Errors));
            }

            if (citizen.TaxId != null && _citizenRepository.GetByTaxId(citizen.TaxId) != null)
            {
                throw new InvalidOperationException(
                    string.Format("A citizen with Tax ID {0} already exists.", citizen.TaxId));
            }

            citizen.RegistrationDate = DateTime.Now;
            _citizenRepository.Add(citizen);
        }

        public void UpdateCitizen(Citizen citizen)
        {
            var validationResult = _validator.Validate(citizen);
            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException(
                    "Citizen validation failed: " + string.Join("; ", validationResult.Errors));
            }

            var existingByTax = citizen.TaxId != null ? _citizenRepository.GetByTaxId(citizen.TaxId) : null;
            if (existingByTax != null && existingByTax.CitizenId != citizen.CitizenId)
            {
                throw new InvalidOperationException(
                    string.Format("Another citizen with Tax ID {0} already exists.", citizen.TaxId));
            }

            _citizenRepository.Update(citizen);
        }

        public void DeleteCitizen(int citizenId)
        {
            var citizen = _citizenRepository.GetByIdWithApplications(citizenId);
            if (citizen == null)
            {
                throw new InvalidOperationException(
                    string.Format("Citizen with ID {0} not found.", citizenId));
            }
            if (citizen.Applications != null && citizen.Applications.Count > 0)
            {
                throw new InvalidOperationException(
                    "Cannot delete citizen with existing service applications.");
            }
            _citizenRepository.Delete(citizen);
        }

        public bool CitizenExists(string taxId)
        {
            return _citizenRepository.GetByTaxId(taxId) != null;
        }
    }
}
