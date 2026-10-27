using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Calculators;

/// <summary>
/// Calculates the rebate amount for a single incentive type.
/// To support a new incentive type, add an implementation and register it with the <c>RebateService</c>.
/// </summary>
public interface IIncentiveCalculator
{
    /// <summary>
    /// The incentive type this calculator handles. Used by the service to pick the calculator for a rebate.
    /// </summary>
    IncentiveType Incentive { get; }

    /// <summary>
    /// The flag a product must have in <c>SupportedIncentives</c> for this calculator to apply.
    /// </summary>
    SupportedIncentiveType SupportedIncentive { get; }

    /// <summary>
    /// Checks that the rebate, product and request hold the values this incentive type needs.
    /// </summary>
    /// <returns><c>true</c> if <see cref="Calculate"/> can produce a valid amount; otherwise <c>false</c>.</returns>
    bool CanCalculate(Rebate rebate, Product product, CalculateRebateRequest request);

    /// <summary>
    /// Calculates the rebate amount. Only called after <see cref="CanCalculate"/> returned <c>true</c>.
    /// </summary>
    /// <returns>The rebate amount.</returns>
    decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request);
}
