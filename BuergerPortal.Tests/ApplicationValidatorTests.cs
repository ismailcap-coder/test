using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuergerPortal.Business.Validators;
using BuergerPortal.Domain.Entities;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Tests
{
    [TestClass]
    public class ApplicationValidatorTests
    {
        private ApplicationValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new ApplicationValidator();
        }

        // Status transition tests
        [TestMethod]
        public void IsValidTransition_DraftToSubmitted_ReturnsTrue()
        {
            Assert.IsTrue(_validator.IsValidStatusTransition(
                ApplicationStatus.Draft, ApplicationStatus.Submitted));
        }

        [TestMethod]
        public void IsValidTransition_SubmittedToUnderReview_ReturnsTrue()
        {
            Assert.IsTrue(_validator.IsValidStatusTransition(
                ApplicationStatus.Submitted, ApplicationStatus.UnderReview));
        }

        [TestMethod]
        public void IsValidTransition_UnderReviewToApproved_ReturnsTrue()
        {
            Assert.IsTrue(_validator.IsValidStatusTransition(
                ApplicationStatus.UnderReview, ApplicationStatus.Approved));
        }

        [TestMethod]
        public void IsValidTransition_UnderReviewToRejected_ReturnsTrue()
        {
            Assert.IsTrue(_validator.IsValidStatusTransition(
                ApplicationStatus.UnderReview, ApplicationStatus.Rejected));
        }

        [TestMethod]
        public void IsValidTransition_UnderReviewToDocumentsRequested_ReturnsTrue()
        {
            Assert.IsTrue(_validator.IsValidStatusTransition(
                ApplicationStatus.UnderReview, ApplicationStatus.DocumentsRequested));
        }

        [TestMethod]
        public void IsValidTransition_DocumentsRequestedToUnderReview_ReturnsTrue()
        {
            Assert.IsTrue(_validator.IsValidStatusTransition(
                ApplicationStatus.DocumentsRequested, ApplicationStatus.UnderReview));
        }

        [TestMethod]
        public void IsValidTransition_DraftToApproved_ReturnsFalse()
        {
            Assert.IsFalse(_validator.IsValidStatusTransition(
                ApplicationStatus.Draft, ApplicationStatus.Approved));
        }

        [TestMethod]
        public void IsValidTransition_ApprovedToAny_ReturnsFalse()
        {
            Assert.IsFalse(_validator.IsValidStatusTransition(
                ApplicationStatus.Approved, ApplicationStatus.Draft));
            Assert.IsFalse(_validator.IsValidStatusTransition(
                ApplicationStatus.Approved, ApplicationStatus.Submitted));
            Assert.IsFalse(_validator.IsValidStatusTransition(
                ApplicationStatus.Approved, ApplicationStatus.Rejected));
        }

        [TestMethod]
        public void IsValidTransition_RejectedToAny_ReturnsFalse()
        {
            Assert.IsFalse(_validator.IsValidStatusTransition(
                ApplicationStatus.Rejected, ApplicationStatus.Draft));
            Assert.IsFalse(_validator.IsValidStatusTransition(
                ApplicationStatus.Rejected, ApplicationStatus.Submitted));
            Assert.IsFalse(_validator.IsValidStatusTransition(
                ApplicationStatus.Rejected, ApplicationStatus.Approved));
        }

        // New application validation tests
        [TestMethod]
        public void ValidateNewApplication_ValidData_ReturnsValid()
        {
            // Arrange
            var application = CreateValidApplication();
            var serviceType = CreateServiceType(ServiceCategory.Registration);
            var citizen = CreateCitizen(30);

            // Act
            var result = _validator.ValidateNewApplication(application, serviceType, citizen);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [TestMethod]
        public void ValidateNewApplication_MissingCitizen_ReturnsInvalid()
        {
            // Arrange
            var application = CreateValidApplication();
            application.CitizenId = 0;

            // Act
            var result = _validator.ValidateNewApplication(application, null, null);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 0);
        }

        [TestMethod]
        public void ValidateNewApplication_UnderAgeLicense_ReturnsInvalid()
        {
            // Arrange
            var application = CreateValidApplication();
            var serviceType = CreateServiceType(ServiceCategory.License);
            var citizen = CreateCitizen(16); // Under 18

            // Act
            var result = _validator.ValidateNewApplication(application, serviceType, citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 0);
        }

        [TestMethod]
        public void ValidateNewApplication_UnderAgeBuildingPermit_ReturnsInvalid()
        {
            // Arrange
            var application = CreateValidApplication();
            var serviceType = CreateServiceType(ServiceCategory.Permit);
            serviceType.ServiceCode = "PER-001";
            var citizen = CreateCitizen(19); // Under 21

            // Act
            var result = _validator.ValidateNewApplication(application, serviceType, citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void ValidateNewApplication_UnderAgeMarriage_ReturnsInvalid()
        {
            // Arrange
            var application = CreateValidApplication();
            var serviceType = CreateServiceType(ServiceCategory.Certificate);
            serviceType.ServiceCode = "CRT-001";
            var citizen = CreateCitizen(14); // Under 16

            // Act
            var result = _validator.ValidateNewApplication(application, serviceType, citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void ValidateNewApplication_AdultForLicense_ReturnsValid()
        {
            // Arrange
            var application = CreateValidApplication();
            var serviceType = CreateServiceType(ServiceCategory.License);
            var citizen = CreateCitizen(25);

            // Act
            var result = _validator.ValidateNewApplication(application, serviceType, citizen);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void ValidateNewApplication_NegativeFee_ReturnsInvalid()
        {
            // Arrange
            var application = CreateValidApplication();
            application.CalculatedFee = -10.00m;
            var serviceType = CreateServiceType(ServiceCategory.Document);
            var citizen = CreateCitizen(30);

            // Act
            var result = _validator.ValidateNewApplication(application, serviceType, citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        private ServiceApplication CreateValidApplication()
        {
            return new ServiceApplication
            {
                ApplicationNumber = "APP-2014-0001",
                CitizenId = 1,
                ServiceTypeId = 1,
                OfficeId = 1,
                Status = ApplicationStatus.Draft,
                CalculatedFee = 16.42m
            };
        }

        private ServiceType CreateServiceType(ServiceCategory category)
        {
            return new ServiceType
            {
                ServiceTypeId = 1,
                ServiceName = "Test Service",
                ServiceCode = "TST-001",
                Category = category,
                BaseFee = 12.00m,
                ProcessingDays = 5
            };
        }

        private Citizen CreateCitizen(int age)
        {
            return new Citizen
            {
                CitizenId = 1,
                FirstName = "Test",
                LastName = "Citizen",
                DateOfBirth = DateTime.Today.AddYears(-age),
                RegistrationDate = DateTime.Now
            };
        }
    }
}
