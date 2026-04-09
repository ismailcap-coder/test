using System;
using BuergerPortal.Domain.Entities;

namespace BuergerPortal.Business.Services
{
    /// <summary>
    /// Fee calculation engine for BuergerPortal service applications.
    ///
    /// Formula:
    ///   1. Start with BaseFee from ServiceType
    ///   2. Multiply by DistrictMultiplier from PublicOffice
    ///   3. Add Express Processing Surcharge (50%) if applicable
    ///   4. Subtract Low-Income Discount (30%) if citizen qualifies
    ///   5. Subtract Multi-Service Discount (10%) if citizen has 3+ active applications
    ///   6. Add Late Submission Penalty (20%) if applicable
    ///   7. Add VAT (19%) to the resulting amount
    ///   8. Round to 2 decimal places
    ///   9. Enforce minimum fee of 5.00 EUR
    /// </summary>
    public class FeeCalculationService : IFeeCalculationService
    {
        private const decimal ExpressSurchargeRate = 0.50m;
        private const decimal LowIncomeDiscountRate = 0.30m;
        private const decimal MultiServiceDiscountRate = 0.10m;
        private const decimal MultiServiceThreshold = 3;
        private const decimal LateSubmissionPenaltyRate = 0.20m;
        private const decimal VatRate = 0.19m;
        private const decimal MinimumFee = 5.00m;

        public decimal CalculateFee(ServiceType serviceType, PublicOffice office, Citizen citizen,
            bool isExpress, int activeApplicationCount)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");
            if (office == null) throw new ArgumentNullException("office");
            if (citizen == null) throw new ArgumentNullException("citizen");

            // Step 1: Base fee
            decimal fee = serviceType.BaseFee;

            // Step 2: District multiplier
            fee = fee * office.DistrictMultiplier;

            // Step 3: Express surcharge (+50%)
            if (isExpress)
            {
                fee = fee + (fee * ExpressSurchargeRate);
            }

            // Step 4: Low-income discount (-30%)
            if (citizen.IsLowIncome)
            {
                fee = fee - (fee * LowIncomeDiscountRate);
            }

            // Step 5: Multi-service discount (-10% if 3+ active applications)
            if (activeApplicationCount >= MultiServiceThreshold)
            {
                fee = fee - (fee * MultiServiceDiscountRate);
            }

            // Step 6: Late submission penalty (+20%)
            // Applied if citizen registered more than 14 days ago and this is a registration service
            if (IsLateSubmission(serviceType, citizen))
            {
                fee = fee + (fee * LateSubmissionPenaltyRate);
            }

            // Step 7: VAT (19%)
            fee = fee + (fee * VatRate);

            // Step 8: Round to 2 decimal places
            fee = Math.Round(fee, 2, MidpointRounding.AwayFromZero);

            // Step 9: Enforce minimum fee
            if (fee < MinimumFee)
            {
                fee = MinimumFee;
            }

            return fee;
        }

        public decimal GetExpressSurchargeRate()
        {
            return ExpressSurchargeRate;
        }

        public decimal GetLowIncomeDiscountRate()
        {
            return LowIncomeDiscountRate;
        }

        public decimal GetMultiServiceDiscountRate()
        {
            return MultiServiceDiscountRate;
        }

        public decimal GetLateSubmissionPenaltyRate()
        {
            return LateSubmissionPenaltyRate;
        }

        public decimal GetVatRate()
        {
            return VatRate;
        }

        private bool IsLateSubmission(ServiceType serviceType, Citizen citizen)
        {
            // Late submission penalty only applies to Registration category services
            if (serviceType.Category != Domain.Enums.ServiceCategory.Registration)
            {
                return false;
            }

            // If the citizen registered more than 14 days after their registration date
            // this indicates a late Anmeldung (address registration)
            TimeSpan timeSinceRegistration = DateTime.Now - citizen.RegistrationDate;
            return timeSinceRegistration.TotalDays > 14;
        }
    }
}
