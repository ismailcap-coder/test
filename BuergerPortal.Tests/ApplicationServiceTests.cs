using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BuergerPortal.Business.Services;
using BuergerPortal.Business.Validators;
using BuergerPortal.Data.Repositories;
using BuergerPortal.Domain.Entities;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Tests
{
    [TestClass]
    public class ApplicationServiceTests
    {
        private Mock<ServiceApplicationRepository> _mockAppRepo = null!;
        private Mock<CitizenRepository> _mockCitizenRepo = null!;
        private Mock<ServiceTypeRepository> _mockServiceTypeRepo = null!;
        private Mock<PublicOfficeRepository> _mockOfficeRepo = null!;
        private Mock<IFeeCalculationService> _mockFeeService = null!;
        private ApplicationValidator _validator = null!;
        private ApplicationService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockAppRepo = new Mock<ServiceApplicationRepository>(MockBehavior.Loose, new object[] { null! });
            _mockCitizenRepo = new Mock<CitizenRepository>(MockBehavior.Loose, new object[] { null! });
            _mockServiceTypeRepo = new Mock<ServiceTypeRepository>(MockBehavior.Loose, new object[] { null! });
            _mockOfficeRepo = new Mock<PublicOfficeRepository>(MockBehavior.Loose, new object[] { null! });
            _mockFeeService = new Mock<IFeeCalculationService>();
            _validator = new ApplicationValidator();

            _service = new ApplicationService(
                _mockAppRepo.Object,
                _mockCitizenRepo.Object,
                _mockServiceTypeRepo.Object,
                _mockOfficeRepo.Object,
                _mockFeeService.Object,
                _validator);
        }

        [TestMethod]
        public void CreateApplication_WithValidData_CreatesApplicationAndReturnsIt()
        {
            // Arrange
            var citizen = CreateTestCitizen(1);
            var serviceType = CreateTestServiceType(1, ServiceCategory.Registration);
            var office = CreateTestOffice(1);

            _mockCitizenRepo.Setup(r => r.GetById(1)).Returns(citizen);
            _mockServiceTypeRepo.Setup(r => r.GetById(1)).Returns(serviceType);
            _mockOfficeRepo.Setup(r => r.GetById(1)).Returns(office);
            _mockAppRepo.Setup(r => r.GetActiveApplicationCount(1)).Returns(0);
            _mockFeeService.Setup(f => f.CalculateFee(serviceType, office, citizen, false, 0)).Returns(16.42m);

            // Act
            var result = _service.CreateApplication(1, 1, 1, false, "Test notes");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ApplicationStatus.Draft, result.Status);
            Assert.AreEqual(16.42m, result.CalculatedFee);
            Assert.IsNotNull(result.ApplicationNumber);
            _mockAppRepo.Verify(r => r.Add(It.IsAny<ServiceApplication>()), Times.Once());
        }

        [TestMethod]
        public void CreateApplication_WithExpressProcessing_PassesExpressFlagToFeeService()
        {
            // Arrange
            var citizen = CreateTestCitizen(1);
            var serviceType = CreateTestServiceType(1, ServiceCategory.Document);
            var office = CreateTestOffice(1);

            _mockCitizenRepo.Setup(r => r.GetById(1)).Returns(citizen);
            _mockServiceTypeRepo.Setup(r => r.GetById(1)).Returns(serviceType);
            _mockOfficeRepo.Setup(r => r.GetById(1)).Returns(office);
            _mockAppRepo.Setup(r => r.GetActiveApplicationCount(1)).Returns(0);
            _mockFeeService.Setup(f => f.CalculateFee(serviceType, office, citizen, true, 0)).Returns(51.41m);

            // Act
            var result = _service.CreateApplication(1, 1, 1, true, "Express request");

            // Assert
            Assert.IsTrue(result.IsExpressProcessing);
            _mockFeeService.Verify(f => f.CalculateFee(serviceType, office, citizen, true, 0), Times.Once());
        }

        [TestMethod]
        public void CreateApplication_WithInvalidCitizen_ThrowsException()
        {
            // Arrange
            _mockCitizenRepo.Setup(r => r.GetById(999)).Returns((Citizen?)null);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _service.CreateApplication(999, 1, 1, false, "Notes"));
        }

        [TestMethod]
        public void CreateApplication_WithInvalidServiceType_ThrowsException()
        {
            // Arrange
            _mockCitizenRepo.Setup(r => r.GetById(1)).Returns(CreateTestCitizen(1));
            _mockServiceTypeRepo.Setup(r => r.GetById(999)).Returns((ServiceType?)null);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _service.CreateApplication(1, 999, 1, false, "Notes"));
        }

        [TestMethod]
        public void SubmitApplication_FromDraft_ChangesStatusToSubmitted()
        {
            // Arrange
            var application = CreateTestApplication(1, ApplicationStatus.Draft);
            _mockAppRepo.Setup(r => r.GetByIdWithDetails(1)).Returns(application);

            // Act
            _service.SubmitApplication(1, "TestUser");

            // Assert
            Assert.AreEqual(ApplicationStatus.Submitted, application.Status);
            Assert.IsNotNull(application.SubmissionDate);
            _mockAppRepo.Verify(r => r.Update(application), Times.Once());
        }

        [TestMethod]
        public void SubmitApplication_FromApproved_ThrowsException()
        {
            // Arrange
            var application = CreateTestApplication(1, ApplicationStatus.Approved);
            _mockAppRepo.Setup(r => r.GetByIdWithDetails(1)).Returns(application);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _service.SubmitApplication(1, "TestUser"));
        }

        [TestMethod]
        public void StartReview_FromSubmitted_ChangesStatusToUnderReview()
        {
            // Arrange
            var application = CreateTestApplication(1, ApplicationStatus.Submitted);
            _mockAppRepo.Setup(r => r.GetById(1)).Returns(application);

            // Act
            _service.StartReview(1, "Reviewer");

            // Assert
            Assert.AreEqual(ApplicationStatus.UnderReview, application.Status);
            Assert.IsNotNull(application.ReviewDate);
        }

        [TestMethod]
        public void ApproveApplication_FromUnderReview_ChangesStatusToApproved()
        {
            // Arrange
            var application = CreateTestApplication(1, ApplicationStatus.UnderReview);
            application.ReviewDate = DateTime.Now;
            _mockAppRepo.Setup(r => r.GetById(1)).Returns(application);

            // Act
            _service.ApproveApplication(1, "Reviewer");

            // Assert
            Assert.AreEqual(ApplicationStatus.Approved, application.Status);
            Assert.IsNotNull(application.CompletionDate);
        }

        [TestMethod]
        public void ApproveApplication_FromDraft_ThrowsException()
        {
            // Arrange
            var application = CreateTestApplication(1, ApplicationStatus.Draft);
            _mockAppRepo.Setup(r => r.GetById(1)).Returns(application);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _service.ApproveApplication(1, "Reviewer"));
        }

        [TestMethod]
        public void RejectApplication_WithReason_ChangesStatusToRejected()
        {
            // Arrange
            var application = CreateTestApplication(1, ApplicationStatus.UnderReview);
            _mockAppRepo.Setup(r => r.GetById(1)).Returns(application);

            // Act
            _service.RejectApplication(1, "Reviewer", "Incomplete documentation");

            // Assert
            Assert.AreEqual(ApplicationStatus.Rejected, application.Status);
            Assert.AreEqual("Incomplete documentation", application.RejectionReason);
            Assert.IsNotNull(application.CompletionDate);
        }

        [TestMethod]
        public void RejectApplication_WithoutReason_ThrowsException()
        {
            // Arrange
            var application = CreateTestApplication(1, ApplicationStatus.UnderReview);
            _mockAppRepo.Setup(r => r.GetById(1)).Returns(application);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _service.RejectApplication(1, "Reviewer", ""));
        }

        [TestMethod]
        public void RequestDocuments_FromUnderReview_ChangesStatusToDocumentsRequested()
        {
            // Arrange
            var application = CreateTestApplication(1, ApplicationStatus.UnderReview);
            _mockAppRepo.Setup(r => r.GetById(1)).Returns(application);

            // Act
            _service.RequestDocuments(1, "Reviewer", "Please provide ID copy");

            // Assert
            Assert.AreEqual(ApplicationStatus.DocumentsRequested, application.Status);
        }

        [TestMethod]
        public void RequestDocuments_FromDraft_ThrowsException()
        {
            // Arrange
            var application = CreateTestApplication(1, ApplicationStatus.Draft);
            _mockAppRepo.Setup(r => r.GetById(1)).Returns(application);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _service.RequestDocuments(1, "Reviewer", "Need documents"));
        }

        [TestMethod]
        public void GetActiveApplicationCount_ReturnsCountFromRepository()
        {
            // Arrange
            _mockAppRepo.Setup(r => r.GetActiveApplicationCount(1)).Returns(3);

            // Act
            var result = _service.GetActiveApplicationCount(1);

            // Assert
            Assert.AreEqual(3, result);
        }

        private Citizen CreateTestCitizen(int id)
        {
            return new Citizen
            {
                CitizenId = id,
                FirstName = "Anna",
                LastName = "Schmidt",
                DateOfBirth = new DateTime(1985, 3, 15),
                StreetAddress = "Teststrasse 1",
                City = "Berlin",
                PostalCode = "10117",
                TaxId = "12345678901",
                IsLowIncome = false,
                RegistrationDate = DateTime.Now
            };
        }

        private ServiceType CreateTestServiceType(int id, ServiceCategory category)
        {
            return new ServiceType
            {
                ServiceTypeId = id,
                ServiceName = "Test Service",
                ServiceCode = "TST-001",
                Category = category,
                BaseFee = 12.00m,
                ProcessingDays = 5,
                RequiresInPersonVisit = false
            };
        }

        private PublicOffice CreateTestOffice(int id)
        {
            return new PublicOffice
            {
                OfficeId = id,
                OfficeName = "Test Office Berlin",
                DistrictCode = "BER-T",
                City = "Berlin",
                DistrictMultiplier = 1.15m
            };
        }

        private ServiceApplication CreateTestApplication(int id, ApplicationStatus status)
        {
            return new ServiceApplication
            {
                ApplicationId = id,
                ApplicationNumber = "APP-2014-0001",
                CitizenId = 1,
                ServiceTypeId = 1,
                OfficeId = 1,
                Status = status,
                IsExpressProcessing = false,
                CalculatedFee = 16.42m,
                Notes = "Test application"
            };
        }
    }
}
