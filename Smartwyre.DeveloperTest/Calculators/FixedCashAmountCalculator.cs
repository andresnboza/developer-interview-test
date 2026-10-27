using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Calculators;

// A flat payout, regardless of how much was bought
public class FixedCashAmountCalculator : IIncentiveCalculator
{
    public IncentiveType Incentive => IncentiveType.FixedCashAmount;

    public SupportedIncentiveType SupportedIncentive => SupportedIncentiveType.FixedCashAmount;

    public bool CanCalculate(Rebate rebate, Product product, CalculateRebateRequest request) =>
        rebate.Amount != 0;

    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request) =>
        rebate.Amount;
}
