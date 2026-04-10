using System.Collections.Generic;
using BuergerPortal.Domain.Entities;

namespace BuergerPortal.Business.Services
{
    public interface ICitizenService
    {
        Citizen GetCitizen(int citizenId);
        Citizen GetCitizenWithApplications(int citizenId);
        Citizen? GetCitizenByTaxId(string taxId);
        IList<Citizen> GetAllCitizens();
        IList<Citizen> SearchCitizens(string searchTerm);
        void CreateCitizen(Citizen citizen);
        void UpdateCitizen(Citizen citizen);
        void DeleteCitizen(int citizenId);
        bool CitizenExists(string taxId);
    }
}
