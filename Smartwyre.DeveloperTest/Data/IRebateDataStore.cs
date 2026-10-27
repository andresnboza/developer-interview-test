using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

/// <summary>
/// Persistence for rebates and the results calculated against them.
/// </summary>
public interface IRebateDataStore
{
    /// <summary>
    /// Looks up a rebate by its identifier.
    /// </summary>
    /// <param name="rebateIdentifier">The identifier of the rebate to retrieve.</param>
    /// <returns>The matching rebate, or <c>null</c> if none exists.</returns>
    Rebate GetRebate(string rebateIdentifier);

    /// <summary>
    /// Saves the outcome of a successful rebate calculation.
    /// </summary>
    /// <param name="rebate">The rebate the calculation was made against.</param>
    /// <param name="rebateAmount">The calculated rebate amount.</param>
    void StoreCalculationResult(Rebate rebate, decimal rebateAmount);
}
