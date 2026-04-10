using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuergerPortal.Business.Services;
using BuergerPortal.Domain.Entities;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Tests
{
    [TestClass]
    public class FeeCalculationServiceTests
    {
        private FeeCalculationService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _service = new FeeCalculationService();
        }

        [TestMethod]
        public void CalculateFee_StandardApplication_AppliesDistrictMultiplierAndVat()
        {
            // Arrange
            var serviceType = CreateServiceType(12.00m, ServiceCategory.Document);
            var office = CreateOffice(1.15m);
            var citizen = CreateCitizen(isLowIncome: false);

            // Act
            decimal fee = _service.CalculateFee(serviceType, office, citizen, false, 0);

            // Assert
            // BaseFee: 12.00 * 1.15 = 13.80
            // No express, no low-income, no multi-service, no late penalty
            // VAT: 13.80 * 1.19 = 16.422
            // Rounded: 16.42
            Assert.AreEqual(16.42m, fee);
        }

        [TestMethod]
        public void CalculateFee_ExpressProcessing_Adds50PercentSurcharge()
        {
            // Arrange
            var serviceType = CreateServiceType(100.00m, ServiceCategory.Permit);
            var office = CreateOffice(1.00m);
            var citizen = CreateCitizen(isLowIncome: false);

            // Act
            decimal fee = _service.CalculateFee(serviceType, office, citizen, true, 0);

            // Assert
            // BaseFee: 100.00 * 1.00 = 100.00
            // Express: 100.00 + 50.00 = 150.00
            // VAT: 150.00 * 1.19 = 178.50
            Assert.AreEqual(178.50m, fee);
        }

        [TestMethod]
        public void CalculateFee_LowIncomeCitizen_Applies30PercentDiscount()
        {
            // Arrange
            var serviceType = CreateServiceType(100.00m, ServiceCategory.Document);
            var office = CreateOffice(1.00m);
            var citizen = CreateCitizen(isLowIncome: true);

            // Act
            decimal fee = _service.CalculateFee(serviceType, office, citizen, false, 0);

            // Assert
            // BaseFee: 100.00 * 1.00 = 100.00
            // Low-income: 100.00 - 30.00 = 70.00
            // VAT: 70.00 * 1.19 = 83.30
            Assert.AreEqual(83.30m, fee);
        }

        [TestMethod]
        public void CalculateFee_MultiServiceDiscount_Applies10PercentFor3PlusApplications()
        {
            // Arrange
            var serviceType = CreateServiceType(100.00m, ServiceCategory.Certificate);
            var office = CreateOffice(1.00m);
            var citizen = CreateCitizen(isLowIncome: false);

            // Act
            decimal fee = _service.CalculateFee(serviceType, office, citizen, false, 3);

            // Assert
            // BaseFee: 100.00 * 1.00 = 100.00
            // Multi-service: 100.00 - 10.00 = 90.00
            // VAT: 90.00 * 1.19 = 107.10
            Assert.AreEqual(107.10m, fee);
        }

        [TestMethod]
        public void CalculateFee_NoMultiServiceDiscount_For2Applications()
        {
            // Arrange
            var serviceType = CreateServiceType(100.00m, ServiceCategory.Certificate);
            var office = CreateOffice(1.00m);
            var citizen = CreateCitizen(isLowIncome: false);

            // Act
            decimal fee = _service.CalculateFee(serviceType, office, citizen, false, 2);

            // Assert
            // BaseFee: 100.00 * 1.00 = 100.00
            // No multi-service discount (only 2 active)
            // VAT: 100.00 * 1.19 = 119.00
            Assert.AreEqual(119.00m, fee);
        }

        [TestMethod]
        public void CalculateFee_ExpressAndLowIncome_AppliesBothModifiers()
        {
            // Arrange
            var serviceType = CreateServiceType(100.00m, ServiceCategory.Document);
            var office = CreateOffice(1.00m);
            var citizen = CreateCitizen(isLowIncome: true);

            // Act
            decimal fee = _service.CalculateFee(serviceType, office, citizen, true, 0);

            // Assert
            // BaseFee: 100.00 * 1.00 = 100.00
            // Express: 100.00 + 50.00 = 150.00
            // Low-income: 150.00 - 45.00 = 105.00
            // VAT: 105.00 * 1.19 = 124.95
            Assert.AreEqual(124.95m, fee);
        }

        [TestMethod]
        public void CalculateFee_AllDiscountsAndSurcharges_AppliedInCorrectOrder()
        {
            // Arrange
            var serviceType = CreateServiceType(250.00m, ServiceCategory.Permit);
            var office = CreateOffice(1.20m);
            var citizen = CreateCitizen(isLowIncome: true);

            // Act
            decimal fee = _service.CalculateFee(serviceType, office, citizen, true, 5);

            // Assert
            // BaseFee: 250.00 * 1.20 = 300.00
            // Express: 300.00 + 150.00 = 450.00
            // Low-income: 450.00 - 135.00 = 315.00
            // Multi-service: 315.00 - 31.50 = 283.50
            // No late penalty (Permit category)
            // VAT: 283.50 * 1.19 = 337.365
            // Rounded: 337.37
            Assert.AreEqual(337.37m, fee);
        }

        [TestMethod]
        public void CalculateFee_EnforcesMinimumFee()
        {
            // Arrange - very low fee service
            var serviceType = CreateServiceType(1.00m, ServiceCategory.Document);
            var office = CreateOffice(1.00m);
            var citizen = CreateCitizen(isLowIncome: true);

            // Act
            decimal fee = _service.CalculateFee(serviceType, office, citizen, false, 5);

            // Assert
            // BaseFee: 1.00 * 1.00 = 1.00
            // Low-income: 1.00 - 0.30 = 0.70
            // Multi-service: 0.70 - 0.07 = 0.63
            // VAT: 0.63 * 1.19 = 0.7497 -> 0.75
            // Below minimum -> 5.00
            Assert.AreEqual(5.00m, fee);
        }

        [TestMethod]
        public void CalculateFee_HighDistrictMultiplier_IncreasesBaseFee()
        {
            // Arrange
            var serviceType = CreateServiceType(60.00m, ServiceCategory.License);
            var office = CreateOffice(1.50m);
            var citizen = CreateCitizen(isLowIncome: false);

            // Act
            decimal fee = _service.CalculateFee(serviceType, office, citizen, false, 0);

            // Assert
            // BaseFee: 60.00 * 1.50 = 90.00
            // VAT: 90.00 * 1.19 = 107.10
            Assert.AreEqual(107.10m, fee);
        }

        [TestMethod]
        public void CalculateFee_NullServiceType_ThrowsException()
        {
            // Arrange
            var office = CreateOffice(1.00m);
            var citizen = CreateCitizen(isLowIncome: false);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _service.CalculateFee(null!, office, citizen, false, 0));
        }

        [TestMethod]
        public void CalculateFee_NullOffice_ThrowsException()
        {
            // Arrange
            var serviceType = CreateServiceType(100.00m, ServiceCategory.Document);
            var citizen = CreateCitizen(isLowIncome: false);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _service.CalculateFee(serviceType, null!, citizen, false, 0));
        }

        [TestMethod]
        public void CalculateFee_NullCitizen_ThrowsException()
        {
            // Arrange
            var serviceType = CreateServiceType(100.00m, ServiceCategory.Document);
            var office = CreateOffice(1.00m);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => _service.CalculateFee(serviceType, office, null!, false, 0));
        }

        [TestMethod]
        public void GetExpressSurchargeRate_Returns50Percent()
        {
            Assert.AreEqual(0.50m, _service.GetExpressSurchargeRate());
        }

        [TestMethod]
        public void GetLowIncomeDiscountRate_Returns30Percent()
        {
            Assert.AreEqual(0.30m, _service.GetLowIncomeDiscountRate());
        }

        [TestMethod]
        public void GetMultiServiceDiscountRate_Returns10Percent()
        {
            Assert.AreEqual(0.10m, _service.GetMultiServiceDiscountRate());
        }

        [TestMethod]
        public void GetVatRate_Returns19Percent()
        {
            Assert.AreEqual(0.19m, _service.GetVatRate());
        }

        private ServiceType CreateServiceType(decimal baseFee, ServiceCategory category)
        {
            return new ServiceType
            {
                ServiceTypeId = 1,
                ServiceName = "Test Service",
                ServiceCode = "TST-001",
                Category = category,
                BaseFee = baseFee,
                ProcessingDays = 10,
                RequiresInPersonVisit = false
            };
        }

        private PublicOffice CreateOffice(decimal districtMultiplier)
        {
            return new PublicOffice
            {
                OfficeId = 1,
                OfficeName = "Test Office",
                DistrictCode = "TST-1",
                City = "Berlin",
                DistrictMultiplier = districtMultiplier
            };
        }

        private Citizen CreateCitizen(bool isLowIncome)
        {
            return new Citizen
            {
                CitizenId = 1,
                FirstName = "Test",
                LastName = "Citizen",
                DateOfBirth = new DateTime(1985, 1, 1),
                IsLowIncome = isLowIncome,
                RegistrationDate = DateTime.Now
            };
        }
    }
}
