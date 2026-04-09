using BuergerPortal.Domain.Entities;

namespace BuergerPortal.Business.Services
{
    public interface IFeeCalculationService
    {
        decimal CalculateFee(ServiceType serviceType, PublicOffice office, Citizen citizen, bool isExpress, int activeApplicationCount);
        decimal GetExpressSurchargeRate();
        decimal GetLowIncomeDiscountRate();
        decimal GetMultiServiceDiscountRate();
        decimal GetLateSubmissionPenaltyRate();
        decimal GetVatRate();
    }
}
