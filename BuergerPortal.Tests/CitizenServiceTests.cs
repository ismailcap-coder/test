using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BuergerPortal.Business.Services;
using BuergerPortal.Business.Validators;
using BuergerPortal.Data.Repositories;
using BuergerPortal.Domain.Entities;

namespace BuergerPortal.Tests
{
    [TestClass]
    public class CitizenServiceTests
    {
        private Mock<CitizenRepository> _mockRepo = null!;
        private CitizenValidator _validator = null!;
        private CitizenService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<CitizenRepository>(MockBehavior.Loose, new object[] { null! });
            _validator = new CitizenValidator();
            _service = new CitizenService(_mockRepo.Object, _validator);
        }

        [TestMethod]
        public void GetCitizen_WithValidId_ReturnsCitizen()
        {
            // Arrange
            var citizen = CreateTestCitizen(1, "Anna", "Schmidt");
            _mockRepo.Setup(r => r.GetById(1)).Returns(citizen);

            // Act
            var result = _service.GetCitizen(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Anna", result.FirstName);
            Assert.AreEqual("Schmidt", result.LastName);
        }

        [TestMethod]
        public void GetCitizen_WithInvalidId_ThrowsException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetById(999)).Returns((Citizen?)null);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _service.GetCitizen(999));
        }

        [TestMethod]
        public void GetAllCitizens_ReturnsAllCitizens()
        {
            // Arrange
            var citizens = new List<Citizen>
            {
                CreateTestCitizen(1, "Anna", "Schmidt"),
                CreateTestCitizen(2, "Klaus", "Weber")
            };
            _mockRepo.Setup(r => r.GetAll()).Returns(citizens);

            // Act
            var result = _service.GetAllCitizens();

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void SearchCitizens_WithValidTerm_ReturnsFilteredResults()
        {
            // Arrange
            var citizens = new List<Citizen> { CreateTestCitizen(1, "Anna", "Schmidt") };
            _mockRepo.Setup(r => r.SearchByName("Schmidt")).Returns(citizens);

            // Act
            var result = _service.SearchCitizens("Schmidt");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Schmidt", result[0].LastName);
        }

        [TestMethod]
        public void SearchCitizens_WithEmptyTerm_ReturnsAllCitizens()
        {
            // Arrange
            var allCitizens = new List<Citizen>
            {
                CreateTestCitizen(1, "Anna", "Schmidt"),
                CreateTestCitizen(2, "Klaus", "Weber")
            };
            _mockRepo.Setup(r => r.GetAll()).Returns(allCitizens);

            // Act
            var result = _service.SearchCitizens("");

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void CreateCitizen_WithValidData_CallsRepositoryAdd()
        {
            // Arrange
            var citizen = CreateTestCitizen(0, "Petra", "Mueller");
            _mockRepo.Setup(r => r.GetByTaxId(citizen.TaxId!)).Returns((Citizen?)null);

            // Act
            _service.CreateCitizen(citizen);

            // Assert
            _mockRepo.Verify(r => r.Add(It.IsAny<Citizen>()), Times.Once());
        }

        [TestMethod]
        public void CreateCitizen_WithDuplicateTaxId_ThrowsException()
        {
            // Arrange
            var existing = CreateTestCitizen(1, "Anna", "Schmidt");
            var newCitizen = CreateTestCitizen(0, "Petra", "Mueller");
            newCitizen.TaxId = existing.TaxId;
            _mockRepo.Setup(r => r.GetByTaxId(existing.TaxId!)).Returns(existing);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _service.CreateCitizen(newCitizen));
        }

        [TestMethod]
        public void CreateCitizen_WithMissingFirstName_ThrowsValidationException()
        {
            // Arrange
            var citizen = CreateTestCitizen(0, "", "Mueller");
            _mockRepo.Setup(r => r.GetByTaxId(citizen.TaxId!)).Returns((Citizen?)null);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _service.CreateCitizen(citizen));
        }

        [TestMethod]
        public void UpdateCitizen_WithValidData_CallsRepositoryUpdate()
        {
            // Arrange
            var citizen = CreateTestCitizen(1, "Anna", "Schmidt-Weber");
            _mockRepo.Setup(r => r.GetByTaxId(citizen.TaxId!)).Returns(citizen);

            // Act
            _service.UpdateCitizen(citizen);

            // Assert
            _mockRepo.Verify(r => r.Update(It.IsAny<Citizen>()), Times.Once());
        }

        [TestMethod]
        public void UpdateCitizen_WithConflictingTaxId_ThrowsException()
        {
            // Arrange
            var existingOther = CreateTestCitizen(2, "Klaus", "Weber");
            var citizen = CreateTestCitizen(1, "Anna", "Schmidt");
            citizen.TaxId = existingOther.TaxId;
            _mockRepo.Setup(r => r.GetByTaxId(citizen.TaxId!)).Returns(existingOther);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _service.UpdateCitizen(citizen));
        }

        [TestMethod]
        public void DeleteCitizen_WithExistingApplications_ThrowsException()
        {
            // Arrange
            var citizen = CreateTestCitizen(1, "Anna", "Schmidt");
            citizen.Applications = new List<ServiceApplication> { new ServiceApplication() };
            _mockRepo.Setup(r => r.GetByIdWithApplications(1)).Returns(citizen);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => _service.DeleteCitizen(1));
        }

        [TestMethod]
        public void DeleteCitizen_WithNoApplications_CallsRepositoryDelete()
        {
            // Arrange
            var citizen = CreateTestCitizen(1, "Anna", "Schmidt");
            citizen.Applications = new List<ServiceApplication>();
            _mockRepo.Setup(r => r.GetByIdWithApplications(1)).Returns(citizen);

            // Act
            _service.DeleteCitizen(1);

            // Assert
            _mockRepo.Verify(r => r.Delete(citizen), Times.Once());
        }

        [TestMethod]
        public void CitizenExists_WithKnownTaxId_ReturnsTrue()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByTaxId("12345678901")).Returns(CreateTestCitizen(1, "Anna", "Schmidt"));

            // Act
            var result = _service.CitizenExists("12345678901");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CitizenExists_WithUnknownTaxId_ReturnsFalse()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByTaxId("00000000000")).Returns((Citizen?)null);

            // Act
            var result = _service.CitizenExists("00000000000");

            // Assert
            Assert.IsFalse(result);
        }

        private Citizen CreateTestCitizen(int id, string firstName, string lastName)
        {
            return new Citizen
            {
                CitizenId = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = new DateTime(1985, 3, 15),
                StreetAddress = "Teststrasse 1",
                City = "Berlin",
                PostalCode = "10117",
                PhoneNumber = "030-1234-5678",
                Email = firstName.ToLower() + "@example.de",
                TaxId = "12345678901",
                IsLowIncome = false,
                RegistrationDate = DateTime.Now
            };
        }
    }
}
