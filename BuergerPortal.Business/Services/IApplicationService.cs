using System.Collections.Generic;
using BuergerPortal.Domain.Entities;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Business.Services
{
    public interface IApplicationService
    {
        ServiceApplication GetApplication(int applicationId);
        ServiceApplication GetApplicationWithDetails(int applicationId);
        IList<ServiceApplication> GetAllApplications();
        IList<ServiceApplication> GetApplicationsByCitizen(int citizenId);
        IList<ServiceApplication> GetPendingApplications();
        IList<ServiceApplication> GetApplicationsByStatus(ApplicationStatus status);
        ServiceApplication CreateApplication(int citizenId, int serviceTypeId, int officeId, bool isExpress, string notes);
        void SubmitApplication(int applicationId, string performedBy);
        void StartReview(int applicationId, string reviewerName);
        void RequestDocuments(int applicationId, string reviewerName, string notes);
        void ApproveApplication(int applicationId, string reviewerName);
        void RejectApplication(int applicationId, string reviewerName, string rejectionReason);
        int GetActiveApplicationCount(int citizenId);
    }
}
