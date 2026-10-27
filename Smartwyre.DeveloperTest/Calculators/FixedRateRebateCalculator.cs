using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Calculators;

// A percentage of what was spent: price x percentage x volume
public class FixedRateRebateCalculator : IIncentiveCalculator
{
    public IncentiveType Incentive => IncentiveType.FixedRateRebate;

    public SupportedIncentiveType SupportedIncentive => SupportedIncentiveType.FixedRateRebate;

    public bool CanCalculate(Rebate rebate, Product product, CalculateRebateRequest request) =>
        rebate.Percentage != 0 && product.Price != 0 && request.Volume != 0;

    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request) =>
        product.Price * rebate.Percentage * request.Volume;
}
