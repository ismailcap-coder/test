using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuergerPortal.Business.Validators;
using BuergerPortal.Domain.Entities;

namespace BuergerPortal.Tests
{
    [TestClass]
    public class CitizenValidatorTests
    {
        private CitizenValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new CitizenValidator();
        }

        [TestMethod]
        public void Validate_ValidCitizen_ReturnsValid()
        {
            // Arrange
            var citizen = CreateValidCitizen();

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [TestMethod]
        public void Validate_NullCitizen_ReturnsInvalid()
        {
            // Act
            var result = _validator.Validate(null);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_MissingFirstName_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.FirstName = null;

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 0);
        }

        [TestMethod]
        public void Validate_EmptyFirstName_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.FirstName = "";

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_FirstNameTooLong_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.FirstName = new string('A', 51);

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_MissingLastName_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.LastName = "";

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_InvalidTaxId_Short_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.TaxId = "12345"; // Too short

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_InvalidTaxId_Letters_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.TaxId = "1234567890A"; // Contains letter

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_MissingTaxId_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.TaxId = null;

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_ValidTaxId_11Digits_ReturnsValid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.TaxId = "12345678901";

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_FutureDateOfBirth_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.DateOfBirth = DateTime.Today.AddDays(1);

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_DefaultDateOfBirth_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.DateOfBirth = default(DateTime);

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_MissingStreetAddress_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.StreetAddress = "";

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_MissingCity_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.City = "";

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_InvalidPostalCode_4Digits_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.PostalCode = "1234"; // German postal codes are 5 digits

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_InvalidPostalCode_WithLetters_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.PostalCode = "1234A";

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_ValidPostalCode_5Digits_ReturnsValid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.PostalCode = "10117";

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_MissingPostalCode_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.PostalCode = null;

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_InvalidEmail_ReturnsInvalid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.Email = "not-an-email";

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_ValidEmail_ReturnsValid()
        {
            // Arrange
            var citizen = CreateValidCitizen();
            citizen.Email = "anna@example.de";

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_EmptyEmail_IsAllowed()
        {
            // Arrange - email is optional
            var citizen = CreateValidCitizen();
            citizen.Email = null;

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_MultipleErrors_ReportsAllErrors()
        {
            // Arrange
            var citizen = new Citizen
            {
                FirstName = "",
                LastName = "",
                DateOfBirth = default(DateTime),
                TaxId = null,
                StreetAddress = "",
                City = "",
                PostalCode = null
            };

            // Act
            var result = _validator.Validate(citizen);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count >= 6); // Multiple fields invalid
        }

        private Citizen CreateValidCitizen()
        {
            return new Citizen
            {
                CitizenId = 1,
                FirstName = "Anna",
                LastName = "Schmidt",
                DateOfBirth = new DateTime(1985, 3, 15),
                StreetAddress = "Unter den Linden 17",
                City = "Berlin",
                PostalCode = "10117",
                PhoneNumber = "030-1234-5678",
                Email = "anna.schmidt@example.de",
                TaxId = "12345678901",
                IsLowIncome = false,
                RegistrationDate = DateTime.Now
            };
        }
    }
}
