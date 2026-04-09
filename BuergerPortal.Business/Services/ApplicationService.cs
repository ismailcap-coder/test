using System;
using System.Collections.Generic;
using BuergerPortal.Business.Validators;
using BuergerPortal.Data.Repositories;
using BuergerPortal.Domain.Entities;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Business.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly ServiceApplicationRepository _applicationRepository;
        private readonly CitizenRepository _citizenRepository;
        private readonly ServiceTypeRepository _serviceTypeRepository;
        private readonly PublicOfficeRepository _officeRepository;
        private readonly IFeeCalculationService _feeService;
        private readonly ApplicationValidator _validator;

        public ApplicationService(
            ServiceApplicationRepository applicationRepository,
            CitizenRepository citizenRepository,
            ServiceTypeRepository serviceTypeRepository,
            PublicOfficeRepository officeRepository,
            IFeeCalculationService feeService,
            ApplicationValidator validator)
        {
            _applicationRepository = applicationRepository;
            _citizenRepository = citizenRepository;
            _serviceTypeRepository = serviceTypeRepository;
            _officeRepository = officeRepository;
            _feeService = feeService;
            _validator = validator;
        }

        public ServiceApplication GetApplication(int applicationId)
        {
            var application = _applicationRepository.GetById(applicationId);
            if (application == null)
            {
                throw new InvalidOperationException(
                    string.Format("Application with ID {0} not found.", applicationId));
            }
            return application;
        }

        public ServiceApplication GetApplicationWithDetails(int applicationId)
        {
            var application = _applicationRepository.GetByIdWithDetails(applicationId);
            if (application == null)
            {
                throw new InvalidOperationException(
                    string.Format("Application with ID {0} not found.", applicationId));
            }
            return application;
        }

        public IList<ServiceApplication> GetAllApplications()
        {
            return _applicationRepository.GetAll();
        }

        public IList<ServiceApplication> GetApplicationsByCitizen(int citizenId)
        {
            return _applicationRepository.GetByCitizenId(citizenId);
        }

        public IList<ServiceApplication> GetPendingApplications()
        {
            return _applicationRepository.GetPendingReview();
        }

        public IList<ServiceApplication> GetApplicationsByStatus(ApplicationStatus status)
        {
            return _applicationRepository.GetByStatus(status);
        }

        public ServiceApplication CreateApplication(int citizenId, int serviceTypeId, int officeId, bool isExpress, string notes)
        {
            var citizen = _citizenRepository.GetById(citizenId);
            if (citizen == null)
            {
                throw new InvalidOperationException(
                    string.Format("Citizen with ID {0} not found.", citizenId));
            }

            var serviceType = _serviceTypeRepository.GetById(serviceTypeId);
            if (serviceType == null)
            {
                throw new InvalidOperationException(
                    string.Format("Service type with ID {0} not found.", serviceTypeId));
            }

            var office = _officeRepository.GetById(officeId);
            if (office == null)
            {
                throw new InvalidOperationException(
                    string.Format("Public office with ID {0} not found.", officeId));
            }

            int activeCount = _applicationRepository.GetActiveApplicationCount(citizenId);
            decimal calculatedFee = _feeService.CalculateFee(serviceType, office, citizen, isExpress, activeCount);

            string appNumber = GenerateApplicationNumber();

            var application = new ServiceApplication
            {
                ApplicationNumber = appNumber,
                CitizenId = citizenId,
                ServiceTypeId = serviceTypeId,
                OfficeId = officeId,
                Status = ApplicationStatus.Draft,
                SubmissionDate = null,
                IsExpressProcessing = isExpress,
                CalculatedFee = calculatedFee,
                Notes = notes,
                DeadlineDate = DateTime.Now.AddDays(serviceType.ProcessingDays + (isExpress ? 0 : 14))
            };

            var validationResult = _validator.ValidateNewApplication(application, serviceType, citizen);
            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException(
                    "Application validation failed: " + string.Join("; ", validationResult.Errors));
            }

            _applicationRepository.Add(application);

            AddAuditLog(application.ApplicationId, "Application Created", null, (int)ApplicationStatus.Draft,
                "System", string.Format("Application {0} created for citizen {1}", appNumber, citizen.FullName));

            return application;
        }

        public void SubmitApplication(int applicationId, string performedBy)
        {
            var application = _applicationRepository.GetByIdWithDetails(applicationId);
            if (application == null)
            {
                throw new InvalidOperationException(
                    string.Format("Application with ID {0} not found.", applicationId));
            }

            if (!_validator.IsValidStatusTransition(application.Status, ApplicationStatus.Submitted))
            {
                throw new InvalidOperationException(
                    string.Format("Cannot transition from {0} to Submitted.", application.Status));
            }

            int previousStatus = (int)application.Status;
            application.Status = ApplicationStatus.Submitted;
            application.SubmissionDate = DateTime.Now;

            _applicationRepository.Update(application);
            AddAuditLog(applicationId, "Application Submitted", previousStatus,
                (int)ApplicationStatus.Submitted, performedBy, "Application submitted for review");
        }

        public void StartReview(int applicationId, string reviewerName)
        {
            var application = _applicationRepository.GetById(applicationId);
            if (application == null)
            {
                throw new InvalidOperationException(
                    string.Format("Application with ID {0} not found.", applicationId));
            }

            if (!_validator.IsValidStatusTransition(application.Status, ApplicationStatus.UnderReview))
            {
                throw new InvalidOperationException(
                    string.Format("Cannot transition from {0} to UnderReview.", application.Status));
            }

            int previousStatus = (int)application.Status;
            application.Status = ApplicationStatus.UnderReview;
            application.ReviewDate = DateTime.Now;

            _applicationRepository.Update(application);
            AddAuditLog(applicationId, "Review Started", previousStatus,
                (int)ApplicationStatus.UnderReview, reviewerName, "Review started by " + reviewerName);
        }

        public void RequestDocuments(int applicationId, string reviewerName, string notes)
        {
            var application = _applicationRepository.GetById(applicationId);
            if (application == null)
            {
                throw new InvalidOperationException(
                    string.Format("Application with ID {0} not found.", applicationId));
            }

            if (!_validator.IsValidStatusTransition(application.Status, ApplicationStatus.DocumentsRequested))
            {
                throw new InvalidOperationException(
                    string.Format("Cannot transition from {0} to DocumentsRequested.", application.Status));
            }

            int previousStatus = (int)application.Status;
            application.Status = ApplicationStatus.DocumentsRequested;
            application.Notes = string.IsNullOrEmpty(application.Notes)
                ? notes
                : application.Notes + Environment.NewLine + notes;

            _applicationRepository.Update(application);
            AddAuditLog(applicationId, "Documents Requested", previousStatus,
                (int)ApplicationStatus.DocumentsRequested, reviewerName, notes);
        }

        public void ApproveApplication(int applicationId, string reviewerName)
        {
            var application = _applicationRepository.GetById(applicationId);
            if (application == null)
            {
                throw new InvalidOperationException(
                    string.Format("Application with ID {0} not found.", applicationId));
            }

            if (!_validator.IsValidStatusTransition(application.Status, ApplicationStatus.Approved))
            {
                throw new InvalidOperationException(
                    string.Format("Cannot transition from {0} to Approved.", application.Status));
            }

            int previousStatus = (int)application.Status;
            application.Status = ApplicationStatus.Approved;
            application.CompletionDate = DateTime.Now;

            _applicationRepository.Update(application);
            AddAuditLog(applicationId, "Application Approved", previousStatus,
                (int)ApplicationStatus.Approved, reviewerName, "Application approved by " + reviewerName);
        }

        public void RejectApplication(int applicationId, string reviewerName, string rejectionReason)
        {
            var application = _applicationRepository.GetById(applicationId);
            if (application == null)
            {
                throw new InvalidOperationException(
                    string.Format("Application with ID {0} not found.", applicationId));
            }

            if (!_validator.IsValidStatusTransition(application.Status, ApplicationStatus.Rejected))
            {
                throw new InvalidOperationException(
                    string.Format("Cannot transition from {0} to Rejected.", application.Status));
            }

            if (string.IsNullOrEmpty(rejectionReason))
            {
                throw new InvalidOperationException("Rejection reason is required.");
            }

            int previousStatus = (int)application.Status;
            application.Status = ApplicationStatus.Rejected;
            application.CompletionDate = DateTime.Now;
            application.RejectionReason = rejectionReason;

            _applicationRepository.Update(application);
            AddAuditLog(applicationId, "Application Rejected", previousStatus,
                (int)ApplicationStatus.Rejected, reviewerName, "Rejected: " + rejectionReason);
        }

        public int GetActiveApplicationCount(int citizenId)
        {
            return _applicationRepository.GetActiveApplicationCount(citizenId);
        }

        private string GenerateApplicationNumber()
        {
            return string.Format("APP-{0:yyyy}-{1:D4}", DateTime.Now, new Random().Next(1, 9999));
        }

        private void AddAuditLog(int applicationId, string action, int? previousStatus, int? newStatus,
            string performedBy, string details)
        {
            var auditLog = new AuditLog
            {
                ApplicationId = applicationId,
                Action = action,
                PreviousStatus = previousStatus,
                NewStatus = newStatus,
                PerformedBy = performedBy,
                Timestamp = DateTime.Now,
                Details = details
            };
            _applicationRepository.AddAuditLog(auditLog);
        }
    }
}
