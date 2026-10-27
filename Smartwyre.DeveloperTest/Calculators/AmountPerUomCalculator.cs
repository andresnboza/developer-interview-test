using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Calculators;

// A fixed amount per unit of measure bought: amount x volume
public class AmountPerUomCalculator : IIncentiveCalculator
{
    public IncentiveType Incentive => IncentiveType.AmountPerUom;

    public SupportedIncentiveType SupportedIncentive => SupportedIncentiveType.AmountPerUom;

    public bool CanCalculate(Rebate rebate, Product product, CalculateRebateRequest request) =>
        rebate.Amount != 0 && request.Volume != 0;

    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request) =>
        rebate.Amount * request.Volume;
}
